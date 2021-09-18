// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 07-30-2021
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 07-30-2021
// ***********************************************************************
// <copyright file="ProviderSpeedGov.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2021 Projeto OpenAC .Net
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
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers.SpeedGov
{
    internal sealed class ProviderSpeedGov : ProviderABRASF
    {
        #region Fields

        private readonly XNamespace p1 = "http://ws.speedgov.com.br/tipos_v1.xsd";

        #endregion Fields

        #region Constructors

        public ProviderSpeedGov(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "SpeedGov";
        }

        #endregion Constructors

        #region Methods

        #region Services

        protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            if (retornoWebservice.Lote == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
            if (retornoWebservice.Erros.Count > 0) return;

            var xmlLoteRps = new StringBuilder();

            foreach (var nota in notas)
            {
                var xmlRps = WriteXmlRps(nota, false, false);
                xmlLoteRps.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }

            var xmlLote = new StringBuilder();
            xmlLote.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlLote.Append("<p:EnviarLoteRpsEnvio xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\" xmlns:p=\"http://ws.speedgov.com.br/enviar_lote_rps_envio_v1.xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://ws.speedgov.com.br/enviar_lote_rps_envio_v1.xsd enviar_lote_rps_envio_v1.xsd\">");
            xmlLote.Append("<p:LoteRps Id=\"\">");
            xmlLote.Append($"<NumeroLote>{retornoWebservice.Lote}</NumeroLote>");
            xmlLote.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            xmlLote.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            xmlLote.Append($"<QuantidadeRps>{notas.Count}</QuantidadeRps>");
            xmlLote.Append("<ListaRps>");
            xmlLote.Append(xmlLoteRps);
            xmlLote.Append("</ListaRps>");
            xmlLote.Append("</p:LoteRps>");
            xmlLote.Append("</p:EnviarLoteRpsEnvio>");

            var document = XDocument.Parse(xmlLote.ToString());
            document.ElementAnyNs("EnviarLoteRpsEnvio").AddAttribute(new XAttribute(XNamespace.Xmlns + "p1", p1));
            NFSeUtil.ApplyNamespace(document.Root, p1, "LoteRps", "EnviarLoteRpsEnvio");

            retornoWebservice.XmlEnvio = document.AsString();
        }

        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "p:EnviarLoteRpsEnvio", "", Certificado);
        }

        protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        #endregion Services

        #region Protected Methods

        protected override IServiceClient GetClient(TipoUrl tipo)
        {
            return new SpeedGovServiceClient(this, tipo);
        }

        protected override string GerarCabecalho()
        {
            var cabecalho = new System.Text.StringBuilder();
            //cabecalho.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            cabecalho.Append("<p:cabecalho versao=\"1\" xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\" xmlns:p=\"http://ws.speedgov.com.br/cabecalho_v1.xsd\" xmlns:p1=\"http://ws.speedgov.com.br/tipos_v1.xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://ws.speedgov.com.br/cabecalho_v1.xsd cabecalho_v1.xsd\">");
            cabecalho.Append("<versaoDados>1</versaoDados>");
            cabecalho.Append("</p:cabecalho>");
            return cabecalho.ToString();
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            switch (tipo)
            {
                case TipoUrl.Enviar: return "enviar_lote_rps_envio_v1.xsd";
                case TipoUrl.ConsultarSituacao: return "consultar_situacao_lote_rps_envio_v1.xsd";
                case TipoUrl.ConsultarLoteRps: return "consultar_lote_rps_envio_v1.xsd";
                case TipoUrl.ConsultarNFSeRps: return "consultar_nfse_rps_envio_v1.xsd";
                case TipoUrl.ConsultarNFSe: return "consultar_nfse_envio_v1.xsd";
                case TipoUrl.CancelarNFSe: return "cancelar_nfse_envio_v1.xsd";
                default: throw new System.ArgumentOutOfRangeException(nameof(tipo), tipo, @"Valor incorreto ou serviço não suportado.");
            }
        }

        protected override string GetNamespace()
        {
            return string.Empty;
        }

        #endregion Protected Methods

        #endregion Methods
    }
}