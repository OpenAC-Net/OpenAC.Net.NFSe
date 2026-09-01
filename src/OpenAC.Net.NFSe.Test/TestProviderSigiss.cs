using OpenAC.Net.NFSe.Nota;
using Xunit;

namespace OpenAC.Net.NFSe.Test;

public class TestProviderSigiss
{
    [Fact]
    public void EmissaoNota()
    {
        var openNFSe = SetupOpenNFSe.Sigiss;

        //adicionado rps
        var nota = openNFSe.NotasServico.AddNew();
        nota.Prestador.CpfCnpj = "37761587000161";
        nota.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
        nota.Servico.Valores.Aliquota = 2;
        nota.Servico.CodigoTributacaoMunicipio = "802";
        nota.NaturezaOperacao = NaturezaOperacao.Sigiss.TributadaNoPrestador;
        nota.Servico.Valores.ValorServicos = 29.91M;
        nota.Servico.Valores.BaseCalculo = 29.91M;
        nota.Servico.Descricao = "serviço teste";
        nota.Tomador.Tipo = TipoTomador.NaoIdentificado;
        nota.Tomador.DadosContato.Email = "a@a.com";

        //enviando
        var retorno = openNFSe.Enviar(0);

        Assert.True(retorno.Sucesso);
    }

    [Fact]
    public void CancelarNota()
    {
        var openNFSe = SetupOpenNFSe.Sigiss;

        //enviando requisicao de cancelamento
        var retorno = openNFSe.CancelarNFSe("a@a.com", "7125", "motivo teste testetestetesteteste");
        Assert.True(retorno.Sucesso);
    }
}