// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 01-11-2023
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 01-11-2023
// ***********************************************************************
// <copyright file="ProviderFisco.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2023 Projeto OpenAC .Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers
{
    // ReSharper disable once InconsistentNaming
    internal sealed class ProviderFisco : ProviderABRASF
    {
        #region Fields

        private static readonly string[] escapedCharacters = { "&amp;", "&lt;", "&gt;", "&quot;", "&apos;" };
        private static readonly string[] unescapedCharacters = { "&", "<", ">", "\"", "\'" };

        #endregion Fields

        #region Constructors

        public ProviderFisco(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Fisco";
        }

        #endregion Constructors

        #region Services

        protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            var xmlLoteRps = new StringBuilder();

            foreach (var nota in notas)
            {
                var xmlRps = WriteXmlRps(nota, false, false);
                xmlLoteRps.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }

            var xmlLote = new StringBuilder();
            xmlLote.Append("<EnviarLoteRpsEnvio>");
            xmlLote.Append($"<LoteRps Id=\"L{retornoWebservice.Lote}\">");
            xmlLote.Append($"<NumeroLote>{retornoWebservice.Lote}</NumeroLote>");
            xmlLote.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            xmlLote.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            xmlLote.Append($"<QuantidadeRps>{notas.Count}</QuantidadeRps>");
            xmlLote.Append("<ListaRps>");
            xmlLote.Append(xmlLoteRps);
            xmlLote.Append("</ListaRps>");
            xmlLote.Append("</LoteRps>");
            xmlLote.Append("</EnviarLoteRpsEnvio>");
            retornoWebservice.XmlEnvio = xmlLote.ToString();
        }

        protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(AjustarRetorno(retornoWebservice.XmlRetorno));

            MensagemErro(retornoWebservice, xmlRet.Root);
            if (retornoWebservice.Erros.Count > 0) return;

            retornoWebservice.Lote = xmlRet?.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;
            retornoWebservice.Data = xmlRet?.ElementAnyNs("DataRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Protocolo = xmlRet?.ElementAnyNs("Protocolo")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.Sucesso = retornoWebservice.Lote > 0;

            if (!retornoWebservice.Sucesso) return;

            // ReSharper disable once SuggestVarOrType_SimpleTypes
            foreach (NotaServico nota in notas)
            {
                nota.NumeroLote = retornoWebservice.Lote;
            }
        }

        protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            var xmlLoteRps = new StringBuilder();

            foreach (var nota in notas)
            {
                var xmlRps = WriteXmlRps(nota, false, false);
                xmlLoteRps.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }
            
            var xmlLote = new StringBuilder();
            xmlLote.Append("<recepcionarLoteRpsSincrono xmlns=\"https://www.fisco.net.br/wsnfseabrasf/ServicosNFSEAbrasf.asmx\">");
            xmlLote.Append("<xml>");
            xmlLote.Append(xmlLoteRps);
            xmlLote.Append("</xml>");
            xmlLote.Append("</recepcionarLoteRpsSincrono>");
            retornoWebservice.XmlEnvio = xmlLote.ToString();
        }

        protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<WS_ConsultarSituacaoLoteRps.Execute xmlns:fiss=\"FISS-LEX\">");
            loteBuilder.Append("<fiss:Consultarsituacaoloterpsenvio>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");
            loteBuilder.Append("</fiss:Consultarsituacaoloterpsenvio>");
            loteBuilder.Append("</WS_ConsultarSituacaoLoteRps.Execute>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(AjustarRetorno(retornoWebservice.XmlRetorno));

            retornoWebservice.Lote = xmlRet?.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;
            retornoWebservice.Situacao = xmlRet?.ElementAnyNs("Situacao")?.GetValue<string>() ?? "0";
            retornoWebservice.Sucesso = !retornoWebservice.Erros.Any();
        }

        protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<WS_ConsultaLoteRps.Execute xmlns:fiss=\"FISS-LEX\">");
            loteBuilder.Append("<fiss:Consultarloterpsenvio>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");
            loteBuilder.Append("</fiss:Consultarloterpsenvio>");
            loteBuilder.Append("</WS_ConsultaLoteRps.Execute>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(AjustarRetorno(retornoWebservice.XmlRetorno));
            MensagemErro(retornoWebservice, xmlRet.Root);
            if (retornoWebservice.Erros.Any()) return;

            var rootElement = xmlRet.ElementAnyNs("WS_ConsultaLoteRps.ExecuteResponse").ElementAnyNs("Consultarloterpsresposta");
            var listaNfse = rootElement?.ElementAnyNs("ListaNfse");

            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Nenhuma NFSe retornada pelo webservice." });
                return;
            }

            retornoWebservice.Sucesso = true;

            var notasServicos = new List<NotaServico>();

            foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
            {
                var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
                var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                var chaveNFSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
                var dataNFSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
                var numeroRps = nfse?.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

                var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
                if (nota == null)
                {
                    notas.Load(compNfse.ToString());
                }
                else
                {
                    nota.IdentificacaoNFSe.Numero = numeroNFSe;
                    nota.IdentificacaoNFSe.Chave = chaveNFSe;
                }

                notasServicos.Add(nota);
            }

            retornoWebservice.Notas = notasServicos.ToArray();
        }

        protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<WS_ConsultaNfsePorRps.Execute xmlns:fiss=\"FISS-LEX\">");
            loteBuilder.Append("<fiss:Consultarnfserpsenvio>");
            loteBuilder.Append("<IdentificacaoRps>");
            loteBuilder.Append($"<Numero>{retornoWebservice.NumeroRps}</Numero>");
            loteBuilder.Append($"<Serie>{retornoWebservice.Serie}</Serie>");
            loteBuilder.Append($"<Tipo>{(int)retornoWebservice.Tipo + 1}</Tipo>");
            loteBuilder.Append("</IdentificacaoRps>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append("</fiss:Consultarnfserpsenvio>");
            loteBuilder.Append("</WS_ConsultaNfsePorRps.Execute>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(AjustarRetorno(retornoWebservice.XmlRetorno));
            MensagemErro(retornoWebservice, xmlRet.Root, "Listamensagemretorno", "tcMensagemRetorno");
            if (retornoWebservice.Erros.Any()) return;

            var elementRoot = xmlRet.ElementAnyNs("WS_ConsultaNfsePorRps.ExecuteResponse");
            var compNfse = elementRoot.ElementAnyNs("Consultarnfserpsresposta")?.ElementAnyNs("CompNfse");
            if (compNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (CompNfse)" });
                return;
            }

            // Carrega a nota fiscal na coleção de Notas Fiscais
            var nota = LoadXml(compNfse.AsString());
            notas.Add(nota);

            retornoWebservice.Nota = nota;
            retornoWebservice.Sucesso = true;
        }

        protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<WS_ConsultaNfse.Execute xmlns:fiss=\"FISS-LEX\">");
            loteBuilder.Append("<fiss:Consultarnfseenvio>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");

            if (retornoWebservice.NumeroNFse > 0)
                loteBuilder.Append($"<NumeroNfse>{retornoWebservice.NumeroNFse}</NumeroNfse>");

            if (retornoWebservice.Inicio.HasValue && retornoWebservice.Fim.HasValue)
            {
                loteBuilder.Append("<PeriodoEmissao>");
                loteBuilder.Append($"<DataInicial>{retornoWebservice.Inicio:yyyy-MM-dd}</DataInicial>");
                loteBuilder.Append($"<DataFinal>{retornoWebservice.Fim:yyyy-MM-dd}</DataFinal>");
                loteBuilder.Append("</PeriodoEmissao>");
            }

            if (!retornoWebservice.CPFCNPJTomador.IsEmpty())
            {
                loteBuilder.Append("<Tomador>");
                loteBuilder.Append("<CpfCnpj>");
                loteBuilder.Append(retornoWebservice.CPFCNPJTomador.IsCNPJ()
                    ? $"<Cnpj>{retornoWebservice.CPFCNPJTomador.ZeroFill(14)}</Cnpj>"
                    : $"<Cpf>{retornoWebservice.CPFCNPJTomador.ZeroFill(11)}</Cpf>");
                loteBuilder.Append("</CpfCnpj>");
                if (!retornoWebservice.IMTomador.IsEmpty())
                    loteBuilder.Append($"<InscricaoMunicipal>{retornoWebservice.IMTomador}</InscricaoMunicipal>");
                loteBuilder.Append("</Tomador>");
            }

            if (!retornoWebservice.NomeIntermediario.IsEmpty() && !retornoWebservice.CPFCNPJIntermediario.IsEmpty())
            {
                loteBuilder.Append("<IntermediarioServico>");
                loteBuilder.Append($"<RazaoSocial>{retornoWebservice.NomeIntermediario}</RazaoSocial>");
                loteBuilder.Append(retornoWebservice.CPFCNPJIntermediario.IsCNPJ()
                    ? $"<Cnpj>{retornoWebservice.CPFCNPJIntermediario.ZeroFill(14)}</Cnpj>"
                    : $"<Cpf>{retornoWebservice.CPFCNPJIntermediario.ZeroFill(11)}</Cpf>");
                loteBuilder.Append("</CpfCnpj>");
                if (!retornoWebservice.IMIntermediario.IsEmpty())
                    loteBuilder.Append($"<InscricaoMunicipal>{retornoWebservice.IMIntermediario}</InscricaoMunicipal>");
                loteBuilder.Append("</IntermediarioServico>");
            }

            loteBuilder.Append("</fiss:Consultarnfseenvio>");
            loteBuilder.Append("</WS_ConsultaNfse.Execute>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(AjustarRetorno(retornoWebservice.XmlRetorno));
            MensagemErro(retornoWebservice, xmlRet.Root, "Listamensagemretorno", "tcMensagemRetorno");

            if (retornoWebservice.Erros.Count > 0) return;

            var retornoLote = xmlRet.ElementAnyNs("WS_ConsultaNfse.ExecuteResponse").ElementAnyNs("Consultarnfseresposta");
            var listaNfse = retornoLote?.ElementAnyNs("ListaNfse");
            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
                return;
            }

            var notasServico = new List<NotaServico>();

            foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
            {
                var nota = LoadXml(compNfse.AsString());
                notas.Add(nota);
                notasServico.Add(nota);
            }

            retornoWebservice.Notas = notasServico.ToArray();
            retornoWebservice.Sucesso = true;
        }

        protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            var pedidoCancelamento = new StringBuilder();
            pedidoCancelamento.Append("<Pedido>");
            pedidoCancelamento.Append($"<InfPedidoCancelamento Id=\"N{retornoWebservice.NumeroNFSe}\">");
            pedidoCancelamento.Append("<IdentificacaoNfse>");
            pedidoCancelamento.Append($"<Numero>{retornoWebservice.NumeroNFSe}</Numero>");
            pedidoCancelamento.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            pedidoCancelamento.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            pedidoCancelamento.Append($"<CodigoMunicipio>{Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio}</CodigoMunicipio>");
            pedidoCancelamento.Append("</IdentificacaoNfse>");
            pedidoCancelamento.Append($"<CodigoCancelamento>{retornoWebservice.CodigoCancelamento}</CodigoCancelamento>");
            pedidoCancelamento.Append("</InfPedidoCancelamento>");
            pedidoCancelamento.Append("</Pedido>");

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<CWS_CancelarNfse.Execute xmlns:fiss=\"FISS-LEX\">");
            loteBuilder.Append("<fiss:Cancelarnfseenvio>");
            loteBuilder.Append(AjustarEnvio(pedidoCancelamento.ToString()));
            loteBuilder.Append("</fiss:Cancelarnfseenvio>");
            loteBuilder.Append("</CWS_CancelarNfse.Execute>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
            //NAO PRECISA ASSINAR
            //retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsSincronoEnvio", "LoteRps", Certificado);
        }

        protected override bool PrecisaValidarSchema(TipoUrl tipo)
        {
            return false;
        }

        protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "Pedido", "InfPedidoCancelamento", Certificado);
        }

        protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(AjustarRetorno(retornoWebservice.XmlRetorno));
            MensagemErro(retornoWebservice, xmlRet.Root);
            if (retornoWebservice.Erros.Count > 0) return;

            var confirmacaoCancelamento = xmlRet.ElementAnyNs("CancelarNfseResposta")?.ElementAnyNs("RetCancelamento")?.ElementAnyNs("NfseCancelamento")?.ElementAnyNs("Confirmacao");
            if (confirmacaoCancelamento == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Confirmação do cancelamento não encontrada!" });
                return;
            }

            retornoWebservice.Data = confirmacaoCancelamento.ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Sucesso = retornoWebservice.Data != DateTime.MinValue;

            // Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
            var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == retornoWebservice.NumeroNFSe);
            if (nota == null) return;

            nota.Situacao = SituacaoNFSeRps.Cancelado;
            nota.Cancelamento.Pedido.CodigoCancelamento = retornoWebservice.CodigoCancelamento;
            nota.Cancelamento.DataHora = confirmacaoCancelamento.ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;
            nota.Cancelamento.MotivoCancelamento = retornoWebservice.Motivo;
        }

        #endregion Services

        #region Methods

        private static string AjustarEnvio(string envio)
        {
            for (var i = 0; i < escapedCharacters.Length; i++)
            {
                envio = envio.Replace(unescapedCharacters[i], escapedCharacters[i]);
            }
            return envio;
        }

        private static string AjustarRetorno(string retorno)
        {
            for (var i = 0; i < escapedCharacters.Length; i++)
            {
                retorno = retorno.Replace(escapedCharacters[i], unescapedCharacters[i]);
            }
            retorno = retorno.Replace("xmlns=\"\"", "");
            retorno = retorno.Replace("xmlns=\"FISS-LEX\"", "");
            return retorno;
        }

        protected override IServiceClient GetClient(TipoUrl tipo)
        {
            return new FiscoServiceClient(this, tipo);
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            return "nfse.xsd";
        }

        #endregion Methods
    }
}