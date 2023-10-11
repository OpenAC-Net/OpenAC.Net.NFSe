using System.Reflection;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using Xunit;

namespace OpenAC.Net.NFSe.Test;

public class TestProviderGinfes : IDisposable
{
    private readonly Stream rps;
    
    public TestProviderGinfes()
    {
        var assembly = Assembly.GetExecutingAssembly();
        rps = assembly.GetManifestResourceStream("OpenAC.Net.NFSe.Test.Resources.RpsGinfes.xml") ?? new MemoryStream();
    }

    [Fact]
    public void TestarGeracaoLeituraRps()
    {
        var openNFSe = SetupOpenNFSe.Ginfes;

        openNFSe.NotasServico.Clear();
        openNFSe.NotasServico.Load(rps);

        Assert.True(openNFSe.NotasServico.Count == 1, "Erro ao carregar a Rps");

        var rpsGerada = openNFSe.NotasServico[0].GetXml();

        rps.Position = 0;
        var xml = XDocument.Load(rps);
        var rpsOriginal = xml.AsString(true);

        Assert.True(rpsGerada == rpsOriginal, "Erro na Geração do Xml da Rps");
    }

    [Fact]
    public void TestarGeracaoLeituraNFSe()
    {
        var openNFSe = SetupOpenNFSe.Ginfes;

        openNFSe.NotasServico.Clear();
        openNFSe.NotasServico.Load(rps);

        Assert.True(openNFSe.NotasServico.Count == 1, "Erro ao carregar a NFSe");

        var nfseGerada = openNFSe.NotasServico[0].GetXml();

        rps.Position = 0;
        var xml = XDocument.Load(rps);
        var nfseOriginal = xml.AsString(true);

        Assert.True(nfseGerada == nfseOriginal, "Erro na Geração do Xml da NFSe");
    }

    public void Dispose()
    {
        rps.Dispose();
    }
}
