using System;
using OpenAC.Net.NFSe.Nota;
using Xunit;

namespace OpenAC.Net.NFSe.Test;

public class TestAbrasfV1
{
    [Fact]
    public void EmissaoNota()
    {
        var openNFSe = SetupOpenNFSe.Abrasf;

        //adicionado rps
        var nota = openNFSe.NotasServico.AddNew();

        nota.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
        nota.NaturezaOperacao = NaturezaOperacao.ABRASF.TributacaoNoMunicipio;
        nota.Situacao = SituacaoNFSeRps.Normal;
        nota.IncentivadorCultural = NFSeSimNao.Nao;


        nota.Servico.Valores.Aliquota = 5;
        nota.Servico.CodigoTributacaoMunicipio = "802";
        nota.Servico.ItemListaServico = "14.01";
        nota.Servico.Valores.ValorServicos = 29.91M;
        nota.Servico.Valores.BaseCalculo = 29.91M;
        nota.Servico.Valores.ValorIss = 1.49m;
        nota.Servico.Descricao = "Reparo de Bike";
        nota.Servico.Discriminacao = "Reparo de Bike";
        nota.Servico.CodigoCnae = "4763603";


        nota.Tomador.Tipo = TipoTomador.PessoaFisica;
        nota.Tomador.CpfCnpj = "94782024568";
        nota.Tomador.RazaoSocial = "Carlos";
        nota.Tomador.Endereco.Logradouro = "Rua principal";
        nota.Tomador.Endereco.Numero = "2";
        nota.Tomador.Endereco.Bairro = "Centro";
        nota.Tomador.Endereco.CodigoMunicipio = 2919207;
        nota.Tomador.Endereco.Uf = "BA";
        nota.Tomador.Endereco.Cep = "40210245";
        nota.Tomador.DadosContato.Email = "a@a.com";


        nota.IdentificacaoRps.Tipo = TipoRps.RPS;
        nota.IdentificacaoRps.Numero = "2";
        nota.IdentificacaoRps.Serie = "A";
        nota.IdentificacaoNFSe.DataEmissao = DateTime.Now;

        nota.Prestador.CpfCnpj = "44818198000190";
        nota.Prestador.InscricaoMunicipal = "0010040441011";
        nota.Prestador.CpfCnpj = "44818198000190";

        var xml = nota.GetXml();

        var response = openNFSe.Enviar(1);
        //enviando
        //var retorno = openNFSe.Enviar(1);

        // Assert.True(retorno.Sucesso);
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