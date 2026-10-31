using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;

namespace OpenAC.Net.NFSe.Test;

// Os testes usam as instâncias estáticas de SetupOpenNFSe.
[NotInParallel("SetupOpenNFSe")]
public class TestProviderGinfes : IDisposable
{
    private readonly Stream rps;
    
    public TestProviderGinfes()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        rps = assembly.GetManifestResourceStream("OpenAC.Net.NFSe.Test.Resources.RpsGinfes.xml") ?? new MemoryStream();
    }

    [Test]
    public async Task TestarGeracaoLeituraRps()
    {
        var openNFSe = SetupOpenNFSe.Ginfes;

        openNFSe.NotasServico.Clear();
        openNFSe.NotasServico.Load(rps);

        await Assert.That(openNFSe.NotasServico.Count == 1).IsTrue().Because("Erro ao carregar a Rps");

        var rpsGerada = openNFSe.NotasServico[0].GetXml();

        rps.Position = 0;
        var xml = XDocument.Load(rps);
        var rpsOriginal = xml.AsString(true);

        await Assert.That(rpsGerada == rpsOriginal).IsTrue().Because("Erro na Geração do Xml da Rps");
    }

    [Test]
    public async Task TestarGeracaoLeituraNFSe()
    {
        var openNFSe = SetupOpenNFSe.Ginfes;

        openNFSe.NotasServico.Clear();
        openNFSe.NotasServico.Load(rps);

        await Assert.That(openNFSe.NotasServico.Count == 1).IsTrue().Because("Erro ao carregar a NFSe");

        var nfseGerada = openNFSe.NotasServico[0].GetXml();

        rps.Position = 0;
        var xml = XDocument.Load(rps);
        var nfseOriginal = xml.AsString(true);

        await Assert.That(nfseGerada == nfseOriginal).IsTrue().Because("Erro na Geração do Xml da NFSe");
    }

    public void Dispose()
    {
        rps.Dispose();
    }
}