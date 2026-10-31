using System.Reflection;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Test;

public class TestConsultaRpsISSSaoPaulo
{
    [Test]
    [Arguments(LayoutISSSaoPaulo.Layout1)]
    [Arguments(LayoutISSSaoPaulo.Layout2)]
    public async Task ConsultaSemNotaPreservaAlertasENaoAutorizaRps(LayoutISSSaoPaulo layout)
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

        await Assert.That(retorno.Sucesso).IsFalse();
        await Assert.That(retorno.Nota).IsNull();
        await Assert.That((await Assert.That(retorno.Erros).HasSingleItem()).Codigo).IsEqualTo("0");
        await Assert.That(retorno.Erros[0].Descricao).IsEqualTo("Consulta do RPS não retornou uma NFS-e.");
        await Assert.That((await Assert.That(retorno.Alertas).HasSingleItem()).Codigo).IsEqualTo("100");
        await Assert.That((await Assert.That(notas).HasSingleItem())).IsSameReferenceAs(nota);
        await Assert.That(string.IsNullOrEmpty(nota.IdentificacaoNFSe.Numero)).IsTrue();
        await Assert.That(retorno.XmlRetorno).IsEqualTo(xml);
    }

    [Test]
    [Arguments(LayoutISSSaoPaulo.Layout1)]
    [Arguments(LayoutISSSaoPaulo.Layout2)]
    public async Task ConsultaComErroPreservaMensagemDaPrefeitura(LayoutISSSaoPaulo layout)
    {
        var config = CriarConfiguracao(layout);
        var notas = new NotaServicoCollection(config);
        var xml = "<RetornoConsulta><Cabecalho><Sucesso>false</Sucesso></Cabecalho>" +
                  "<Erro><Codigo>999</Codigo><Descricao>Erro de teste</Descricao></Erro></RetornoConsulta>";

        var retorno = TratarRetorno(config, notas, xml);

        await Assert.That(retorno.Sucesso).IsFalse();
        await Assert.That(retorno.Nota).IsNull();
        await Assert.That((await Assert.That(retorno.Erros).HasSingleItem()).Codigo).IsEqualTo("999");
        await Assert.That(retorno.Erros[0].Descricao).IsEqualTo("Erro de teste");
        await Assert.That(notas).IsEmpty();
    }

    [Test]
    [Arguments(LayoutISSSaoPaulo.Layout1)]
    [Arguments(LayoutISSSaoPaulo.Layout2)]
    public async Task ConsultaComNotaMantemAutorizacao(LayoutISSSaoPaulo layout)
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

        await Assert.That(retorno.Sucesso).IsTrue();
        await Assert.That(retorno.Erros).IsEmpty();
        await Assert.That(retorno.Nota).IsSameReferenceAs(nota);
        await Assert.That((await Assert.That(notas).HasSingleItem())).IsSameReferenceAs(nota);
        await Assert.That(nota.IdentificacaoNFSe.Numero).IsEqualTo("987");
        await Assert.That(nota.IdentificacaoNFSe.Chave).IsEqualTo("ABC123");
        await Assert.That(nota.IdentificacaoNFSe.DataEmissao).IsEqualTo(new DateTime(2026, 10, 9, 8, 18, 15));
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