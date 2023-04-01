using System.IO;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using Xunit;

namespace OpenAC.Net.NFSe.Test;

public class TestProviderGinfes
{
    [Fact]
    public void TestarGeracaoLeituraRps()
    {
        var openNFSe = SetupOpenNFSe.Ginfes;

        openNFSe.NotasServico.Clear();

        var dados = new MemoryStream(Properties.Resources.Exemplo_Rps_Ginfes);
        openNFSe.NotasServico.Load(dados);

        Assert.True(openNFSe.NotasServico.Count == 1, "Erro ao carregar a Rps");

        var rpsGerada = openNFSe.NotasServico[0].GetXml();

        dados.Position = 0;
        var xml = XDocument.Load(dados);
        var rpsOriginal = xml.AsString(true);

        Assert.True(rpsGerada == rpsOriginal, "Erro na Geração do Xml da Rps");
    }

    [Fact]
    public void TestarGeracaoLeituraNFSe()
    {
        var openNFSe = SetupOpenNFSe.Ginfes;

        openNFSe.NotasServico.Clear();

        var dados = new MemoryStream(Properties.Resources.Exemplo_Rps_Ginfes);
        openNFSe.NotasServico.Load(dados);

        Assert.True(openNFSe.NotasServico.Count == 1, "Erro ao carregar a NFSe");

        var nfseGerada = openNFSe.NotasServico[0].GetXml();

        dados.Position = 0;
        var xml = XDocument.Load(dados);
        var nfseOriginal = xml.AsString(true);

        Assert.True(nfseGerada == nfseOriginal, "Erro na Geração do Xml da NFSe");
    }
}