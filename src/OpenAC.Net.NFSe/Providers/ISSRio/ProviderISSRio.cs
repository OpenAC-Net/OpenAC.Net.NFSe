// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 08-16-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-16-2018
// ***********************************************************************
// <copyright file="ProviderNotaCarioca.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		Copyright (c) 2014 - 2024 Projeto OpenAC .Net
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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

// ReSharper disable once InconsistentNaming
internal sealed class ProviderISSRio : ProviderABRASF
{
    #region Constructors

    public ProviderISSRio(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "ISSRio";
        Versao = VersaoNFSe.ve100;
    }

    #endregion Constructors

    #region Methods

    protected override XElement WriteInfoRPS(NotaServico nota)
    {
        var incentivadorCultural = nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2;

        string regimeEspecialTributacao;
        string optanteSimplesNacional;
        if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
        {
            regimeEspecialTributacao = "";
            optanteSimplesNacional = "1";
        }
        else
        {
            var regime = (int)nota.RegimeEspecialTributacao;
            regimeEspecialTributacao = regime == 0 ? string.Empty : regime.ToString();
            optanteSimplesNacional = "2";
        }

        var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "1" : "2";

        var infoRps = new XElement("InfRps", new XAttribute("Id", $"R{nota.IdentificacaoRps.Numero}"));

        infoRps.Add(WriteIdentificacao(nota));
        infoRps.AddChild(AddTag(TipoCampo.DatHor, "", "DataEmissao", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "NaturezaOperacao", 1, 1, Ocorrencia.Obrigatoria, nota.NaturezaOperacao));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "IncentivadorCultural", 1, 1, Ocorrencia.Obrigatoria, incentivadorCultural));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, situacao));

        return infoRps;
    }

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (notas.Count == 0) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Nenhuma RPS informada." });
        if (notas.Count > 1) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Apenas uma RPS pode ser enviada em modo Sincrono." });
        if (retornoWebservice.Erros.Count > 0) return;

        var xmlLote = new StringBuilder();
        xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        xmlLote.Append("<GerarNfseEnvio xmlns=\"http://notacarioca.rio.gov.br/WSNacional/XSD/1/nfse_pcrj_v01.xsd\">");

        var xmlRps = WriteXmlRps(notas[0], false, false);
        XmlSigning.AssinarXml(xmlRps, "Rps", "InfRps", Certificado);
        GravarRpsEmDisco(xmlRps, $"Rps-{notas[0].IdentificacaoRps.DataEmissao:yyyyMMdd}-{notas[0].IdentificacaoRps.Numero}.xml", notas[0].IdentificacaoRps.DataEmissao);

        xmlLote.Append(xmlRps);
        xmlLote.Append("</GerarNfseEnvio>");
        retornoWebservice.XmlEnvio = xmlLote.ToString();
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        //
    }

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet.Root);
        if (retornoWebservice.Erros.Any()) return;

        var compNfse = xmlRet.ElementAnyNs("GerarNfseResposta")?.ElementAnyNs("CompNfse");
        var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
        var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
        var chaveNFSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
        var dataNFSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
        var numeroRps = nfse?.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;

        GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

        var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
        if (nota == null)
        {
            nota = LoadXml(compNfse.ToString());
        }
        else
        {
            nota.IdentificacaoNFSe.Numero = numeroNFSe;
            nota.IdentificacaoNFSe.Chave = chaveNFSe;
        }

        notas.Add(nota);
        retornoWebservice.Sucesso = true;
    }

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new ISSRioServiceClient(this, tipo);
    }

    protected override string GetSchema(TipoUrl tipo)
    {
        return "nfse.xsd";
    }

    #endregion Methods
}