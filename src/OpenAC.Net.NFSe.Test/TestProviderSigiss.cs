using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Test;

// Os testes usam as instâncias estáticas de SetupOpenNFSe.
[NotInParallel("SetupOpenNFSe")]
public class TestProviderSigiss
{
    [Test]
    public async Task EmissaoNota()
    {
        var openNFSe = SetupOpenNFSe.Sigiss;

        //Dados WebService
        openNFSe.Configuracoes.WebServices.Ambiente = DFe.Core.Common.DFeTipoAmbiente.Producao;
        openNFSe.Configuracoes.WebServices.CodigoMunicipio = 3529005;
        openNFSe.Configuracoes.WebServices.Usuario = "";
        openNFSe.Configuracoes.WebServices.Senha = "";
        openNFSe.Configuracoes.PrestadorPadrao.CpfCnpj = "";

        //adicionado rps
        var nota = openNFSe.NotasServico.AddNew();
        nota.Prestador.CpfCnpj = "";
        nota.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
        nota.Servico.Valores.Aliquota = 4.9588M;
        nota.Servico.CodigoTributacaoMunicipio = "104";
        nota.NaturezaOperacao = NaturezaOperacao.Sigiss.TributadaNoPrestador;
        nota.Servico.Valores.ValorServicos = 0.07M;
        nota.Servico.Valores.BaseCalculo = 0.07M;
        nota.Servico.Descricao = "serviço teste";
        nota.Servico.CodigoNbs = "115021005";
        nota.Tomador.Tipo = TipoTomador.NaoIdentificado;
        /*nota.Tomador.CpfCnpj = "";
        nota.Tomador.DadosContato.Telefone = "";
        nota.Tomador.DadosContato.Email = "";*/
        //nota.IdentificacaoRps.Numero = "1";
        //nota.IdentificacaoRps.Serie = "100";

        //enviando
        var retorno = openNFSe.Enviar(0);
        var retornoconsulta = openNFSe.ConsultaNFSe(int.Parse(retorno.Protocolo));

        await Assert.That(retorno.Sucesso).IsTrue();
    }

    [Test]
    public async Task ConsultarNota()
    {
        var openNFSe = SetupOpenNFSe.Sigiss;

        //Dados WebService
        openNFSe.Configuracoes.WebServices.Ambiente = DFe.Core.Common.DFeTipoAmbiente.Producao;
        openNFSe.Configuracoes.WebServices.CodigoMunicipio = 3529005;
        openNFSe.Configuracoes.WebServices.Usuario = "";
        openNFSe.Configuracoes.WebServices.Senha = "";
        openNFSe.Configuracoes.PrestadorPadrao.CpfCnpj = "";

        //Dados Prestador
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio = 3529005;
        openNFSe.Configuracoes.WebServices.AguardarConsultaRet = 60 * 5; //5 minutos de timeout

        //enviando requisicao de cancelamento
        var retorno = openNFSe.ConsultaNFSe(50000);//nao existe
        //var retorno2 = openNFSe.ConsultaNFSe(42219);//recusado
        //var retorno3 = openNFSe.ConsultaNFSe(42218);//sucesso        
        await Assert.That(retorno.Sucesso).IsTrue();
    }

    [Test]
    public async Task CancelarNota()
    {
        var openNFSe = SetupOpenNFSe.Sigiss;

        //Dados WebService
        openNFSe.Configuracoes.WebServices.Ambiente = DFe.Core.Common.DFeTipoAmbiente.Producao;
        openNFSe.Configuracoes.WebServices.CodigoMunicipio = 3529005;
        openNFSe.Configuracoes.WebServices.Usuario = "";
        openNFSe.Configuracoes.WebServices.Senha = "";
        openNFSe.Configuracoes.PrestadorPadrao.CpfCnpj = "";

        //enviando requisicao de cancelamento
        var retorno = openNFSe.CancelarNFSe("email@email.com", "42216", "teste desenvolvimento danilo breda");
        await Assert.That(retorno.Sucesso).IsTrue();
    }
}