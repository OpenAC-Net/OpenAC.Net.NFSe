using System.Reflection;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.Providers;
using Xunit;

namespace OpenAC.Net.NFSe.Test;

public class TestConsultaRpsISSSaoPaulo
{
    [Theory]
    [InlineData(LayoutISSSaoPaulo.Layout1)]
    [InlineData(LayoutISSSaoPaulo.Layout2)]
    public void ConsultaSemNotaPreservaAlertasENaoAutorizaRps(LayoutISSSaoPaulo layout)
    {
        var config = CriarConfiguracao(layout);
        var notas = new NotaServicoCollection(config);
        var nota = notas.AddNew();
        nota.IdentificacaoRps.Numero = "12234";
        var xml = "<RetornoConsulta xmlns=\"http://www.prefeitura.sp.gov.br/nfe\">" +
                  "<Cabecalho xmlns=\"\" Versao=\"2\"><Sucesso>true</Sucesso></Cabecalho>" +
                  "<Alerta xmlns=\"\"><Codigo>100</Codigo><Descricao>Alerta de teste</Descricao></Alerta>" +
                  "</RetornoConsulta>";

        var retorno = TratarRetorno(config, notas, xml);

        Assert.False(retorno.Sucesso);
        Assert.Null(retorno.Nota);
        Assert.Equal("0", Assert.Single(retorno.Erros).Codigo);
        Assert.Equal("Consulta do RPS não retornou uma NFS-e.", retorno.Erros[0].Descricao);
        Assert.Equal("100", Assert.Single(retorno.Alertas).Codigo);
        Assert.Same(nota, Assert.Single(notas));
        Assert.True(string.IsNullOrEmpty(nota.IdentificacaoNFSe.Numero));
        Assert.Equal(xml, retorno.XmlRetorno);
    }

    [Theory]
    [InlineData(LayoutISSSaoPaulo.Layout1)]
    [InlineData(LayoutISSSaoPaulo.Layout2)]
    public void ConsultaComErroPreservaMensagemDaPrefeitura(LayoutISSSaoPaulo layout)
    {
        var config = CriarConfiguracao(layout);
        var notas = new NotaServicoCollection(config);
        var xml = "<RetornoConsulta><Cabecalho><Sucesso>false</Sucesso></Cabecalho>" +
                  "<Erro><Codigo>999</Codigo><Descricao>Erro de teste</Descricao></Erro></RetornoConsulta>";

        var retorno = TratarRetorno(config, notas, xml);

        Assert.False(retorno.Sucesso);
        Assert.Null(retorno.Nota);
        Assert.Equal("999", Assert.Single(retorno.Erros).Codigo);
        Assert.Equal("Erro de teste", retorno.Erros[0].Descricao);
        Assert.Empty(notas);
    }

    [Theory]
    [InlineData(LayoutISSSaoPaulo.Layout1)]
    [InlineData(LayoutISSSaoPaulo.Layout2)]
    public void ConsultaComNotaMantemAutorizacao(LayoutISSSaoPaulo layout)
    {
        var config = CriarConfiguracao(layout);
        var notas = new NotaServicoCollection(config);
        var nota = notas.AddNew();
        nota.IdentificacaoRps.Numero = "12234";
        var xml = "<RetornoConsulta><Cabecalho><Sucesso>true</Sucesso></Cabecalho>" +
                  "<NFe><ChaveNFe><InscricaoPrestador>123456789012</InscricaoPrestador>" +
                  "<NumeroNFe>987</NumeroNFe><CodigoVerificacao>ABC123</CodigoVerificacao></ChaveNFe>" +
                  "<DataEmissaoNFe>2026-10-09T08:18:15</DataEmissaoNFe>" +
                  "<ChaveRPS><NumeroRPS>12234</NumeroRPS></ChaveRPS></NFe></RetornoConsulta>";

        var retorno = TratarRetorno(config, notas, xml);

        Assert.True(retorno.Sucesso);
        Assert.Empty(retorno.Erros);
        Assert.Same(nota, retorno.Nota);
        Assert.Same(nota, Assert.Single(notas));
        Assert.Equal("987", nota.IdentificacaoNFSe.Numero);
        Assert.Equal("ABC123", nota.IdentificacaoNFSe.Chave);
        Assert.Equal(new DateTime(2026, 10, 9, 8, 18, 15), nota.IdentificacaoNFSe.DataEmissao);
    }

    private static ConfigNFSe CriarConfiguracao(LayoutISSSaoPaulo layout)
    {
        var config = new ConfigNFSe();
        config.Geral.Salvar = false;
        config.Arquivos.Salvar = false;
        config.WebServices.CodigoMunicipio = 3550308;
        config.WebServices.LayoutISSSaoPaulo = layout;
        return config;
    }

    private static RetornoConsultarNFSeRps TratarRetorno(ConfigNFSe config,
        NotaServicoCollection notas, string xml)
    {
        using var provider = ProviderManager.GetProvider(config);
        var retorno = new RetornoConsultarNFSeRps();
        typeof(RetornoWebservice).GetProperty(nameof(RetornoWebservice.XmlRetorno))!.SetValue(retorno, xml);
        provider.GetType().GetMethod("TratarRetornoConsultarNFSeRps",
            BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(provider, new object[] { retorno, notas });
        return retorno;
    }
}
