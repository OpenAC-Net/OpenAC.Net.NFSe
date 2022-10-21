using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAC.Net.NFSe.DANFSe.ReportNative.Danfe;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.Test;
using Xunit;

namespace OpenAC.Net.NFSe.Test
{
    public class ReportTest
    {
        [Fact]
        public void GerarDanfeNFseEmFormatoHtml()
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
            nota.Servico.Descricao = @"
                                    OGGI
                                        Manutencao em marcha shimano acero
                                        Lavagem completa
                                        lubrificacao de conduites
                                        Reparo suspensao completa
                                        tyuntlgbyuuvytbgti
                                        002
                                        AAAAAAAAAAAAAAAA
                                        Valor Total: 843,98
                                    OGGI
                                        Manutencao em marcha shimano acero
                                        Lavagem completa
                                        lubrificacao de conduites
                                        Reparo suspensao completa
                                        tyuntlgbyuuvytbgti
                                        002
                                        AAAAAAAAAAAAAAAA
                                        Valor Total: 843,98
                                    OGGI
                                        Manutencao em marcha shimano acero
                                        Lavagem completa
                                        lubrificacao de conduites
                                        Reparo suspensao completa
                                        tyuntlgbyuuvytbgti
                                        002
                                        AAAAAAAAAAAAAAAA
                                        Valor Total: 843,98
                                    OGGI
                                        Manutencao em marcha shimano acero
                                        Lavagem completa
                                        lubrificacao de conduites
                                        Reparo suspensao completa
                                        tyuntlgbyuuvytbgti
                                        002
                                        AAAAAAAAAAAAAAAA
                                        Valor Total: 843,98

                        ";
            nota.Servico.Discriminacao = "Reparo de Bike";
            nota.Servico.CodigoCnae = "4763603";
            nota.Servico.Valores.ValorLiquidoNfse = 329.91M;

            nota.Tomador.Tipo = TipoTomador.Sigiss.PessoaFisica;
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
            nota.IdentificacaoRps.Numero = "3";
            nota.IdentificacaoRps.Serie = "A";
            nota.IdentificacaoNFSe.DataEmissao = DateTime.Now;

            nota.Prestador.CpfCnpj = "44818198000190";
            nota.Prestador.InscricaoMunicipal = "0010040441011";
            nota.Prestador.CpfCnpj = "44818198000190";
            nota.Prestador.NomeFantasia = "Nome Fantasia";

            var danfeInfo = new DanfeInfo();
            danfeInfo.Titulo = "Recibo Provisório de Serviço - RPS";
            danfeInfo.NomePrefeitura = "Prefeitura de Lauro de Freitas";
            danfeInfo.NotaServico = nota;

            var doc = new DanfeNFSeHtml(danfeInfo);
            doc.SalvarDocHtml("C:\\", "NFSe.htm");

        }


        /*
         Dica para usar em sistemas Web
         - Usar o componente encontrado no endereco: https://printjs.crabbly.com/
         - Recomendacao de uso em sistema web
        //===========================================================================
        // Exibir modal para impressao de Danfe 55
        //===========================================================================
        function imprimirStringHtmlDanfeNFSeA4(strHtml, afterRefresh = false) {
        const body = obterConteudoBody(strHtml);
        const body2 = `<html><body>${body}</body></html>`; 
        const style = `.titulo { font-size:1.4em; } .tabela { border: 2px solid #000; border-spacing: 0px; margin: 0px 0px 0px 0px; white-space:nowrap; } .tabela tr td { border: 1px solid #000; } .tabela table tr td { border-style: none; border-width: 0px; } .cabecalho { line-height: normal; font-style: normal; font-weight: normal; font-variant: normal; font-size: 13px; font-family: 'Courier New'; margin: 0px 0px 0px 2px; line-height: 9px; } .tabela dl { border: 0px none #000; border-spacing: 0px; margin: 0px 0px 0px 0px; } .tabela dt { margin: 0px 5px; text-transform: uppercase; font: 13px 'Courier New'; color: #000; white-space: nowrap; } .tabela dd { margin: 0px 5px 1px; color: #369; line-height: normal; font-style: normal; font-variant: normal; font-size: 13px; font-family: "Courier New"; height: 14px; white-space: nowrap; } .sub-titulo { line-height: normal; font-style: normal; font-weight: bold; font-variant: normal; font-size: 13px; font-family: "Courier New"; margin: 0px 0px 0px 0px; white-space: pre-line; } .corpo { color: #369; line-height: normal; font-style: normal; font-weight: bold; font-variant: normal; font-size: 13px; font-family: "Courier New"; margin: 0px 0px 0px 0px; white-space: nowrap; letter-spacing: -1px; } .campo_titulo { white-space: break-spaces; line-height: normal; font-style: normal; font-weight: bold; font-variant: normal; font-size: 15px; font-family: 'Courier New'; margin: 0px 0px 0px 0px; white-space: nowrap; word-spacing: -1px; } .campo_valor { color: #369; line-height: normal; font-style: normal; font-weight: bold; font-variant: normal; font-size: 14px; font-family: "Courier New"; margin: 0px 0px 0px 0px; white-space: pre-line; display: inline-block; max-width: 780px; } `
        printJS({ style: style, printable: body2, type: 'raw-html' });

        }

         */

    }
}
