// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Dheizon Gonçalves
// Created          : 26-07-2022
//
// Last Modified By : Dheizon Gonçalves 
// Last Modified On : 29-05-2023
// ***********************************************************************
// <copyright file="ProviderSJP.cs" company="OpenAC .Net">
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
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderISSSJP : ProviderABRASF
{
    #region Constructors

    public ProviderISSSJP(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "ISSSJP";
    }

    #endregion Constructors

    #region Methods

    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.Lote == 0) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Lote não informado." });
        if (notas.Count == 0) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "RPS não informado." });
        if (retornoWebservice.Erros.Count > 0) return;

        var xmlLoteRps = new StringBuilder();

        foreach (var nota in notas)
        {
            var xmlRps = WriteXmlRps(nota, false, false);

            xmlLoteRps.Append(xmlRps);
            GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
        }

        var xmlLote = new StringBuilder();

        xmlLote.Append("<EnviarLoteRpsEnvio xmlns=\"http://nfe.sjp.pr.gov.br/servico_enviar_lote_rps_envio_v03.xsd\">");
        xmlLote.Append($"<LoteRps Id=\"L{retornoWebservice.Lote}\">");
        xmlLote.Append($"<NumeroLote xmlns=\"http://nfe.sjp.pr.gov.br/tipos_v03.xsd\">{retornoWebservice.Lote}</NumeroLote>");
        xmlLote.Append($"<Cnpj xmlns=\"http://nfe.sjp.pr.gov.br/tipos_v03.xsd\">{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
        xmlLote.Append($"<InscricaoMunicipal xmlns=\"http://nfe.sjp.pr.gov.br/tipos_v03.xsd\">{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        xmlLote.Append($"<QuantidadeRps xmlns=\"http://nfe.sjp.pr.gov.br/tipos_v03.xsd\">{notas.Count}</QuantidadeRps>");
        xmlLote.Append("<ListaRps xmlns=\"http://nfe.sjp.pr.gov.br/tipos_v03.xsd\">");
        xmlLote.Append(xmlLoteRps);
        xmlLote.Append("</ListaRps>");
        xmlLote.Append("</LoteRps>");
        xmlLote.Append("</EnviarLoteRpsEnvio>");

        var document = XDocument.Parse(xmlLote.ToString());

        retornoWebservice.XmlEnvio = document.AsString();

    }

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new ISSSJPServiceClient(this, tipo);
    }

    protected override string GerarCabecalho()
    {
        var cabecalho = new StringBuilder();
        cabecalho.Append("<cabecalho versao=\"3\"> xmlns:ns2=\"http://nfe.sjp.pr.gov.br/cabecalho_v03.xsd\"");
        cabecalho.Append("<versaoDados>3</versaoDados>");
        cabecalho.Append("</cabecalho>");
        return cabecalho.ToString();
    }

    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "EnviarLoteRpsEnvio", "LoteRps", Certificado);
    }

    protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "ConsultarSituacaoLoteRpsEnvio", "", Certificado);
    }

    protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "p:ConsultarNfseRpsEnvio", "", Certificado);
    }

    protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "ConsultarNfseEnvio", "", Certificado);
    }

    protected override string GetSchema(TipoUrl tipo)
    {
        switch (tipo)
        {
            case TipoUrl.Enviar:
                return "servico_enviar_lote_rps_envio_v03.xsd";

            case TipoUrl.ConsultarSituacao:
                return "servico_consultar_situacao_lote_rps_envio_v03.xsd";

            case TipoUrl.ConsultarLoteRps:
                return "servico_consultar_lote_rps_envio_v03.xsd";

            case TipoUrl.CancelarNFSe:
                return "servico_cancelar_nfse_envio_v03.xsd";

            case TipoUrl.ConsultarNFSe:
                return "servico_consultar_nfse_envio_v03.xsd";

            case TipoUrl.ConsultarNFSeRps:
                return "servico_consultar_nfse_rps_envio_v03.xsd";

            case TipoUrl.EnviarSincrono:
            case TipoUrl.ConsultarSequencialRps:
            case TipoUrl.CancelarNFSeLote:
            case TipoUrl.SubstituirNFSe:
                return "";

            default:
                throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null);
        }
    }

    protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        var xmlLote = new StringBuilder();

        xmlLote.Append($"<ConsultarLoteRpsEnvio xmlns=\"http://nfe.sjp.pr.gov.br/servico_consultar_lote_rps_envio_v03.xsd\" xmlns:tipos=\"http://nfe.sjp.pr.gov.br/tipos_v03.xsd\" xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\" Id=\"{retornoWebservice.Lote}\">");
        xmlLote.Append($"<Prestador>");
        xmlLote.Append($"<tipos:Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tipos:Cnpj>");
        xmlLote.Append($"<tipos:InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
        xmlLote.Append("</Prestador>");
        xmlLote.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");
        xmlLote.Append("</ConsultarLoteRpsEnvio>");

        retornoWebservice.XmlEnvio = xmlLote.ToString();
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.NumeroRps < 1)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Número da NFSe não informado para a consulta." });
            return;
        }

        var xmlLote = new StringBuilder();

        xmlLote.Append($"<p:ConsultarNfseRpsEnvio xmlns:p=\"http://nfe.sjp.pr.gov.br/servico_consultar_nfse_rps_envio_v03.xsd\" xmlns:p1=\"http://nfe.sjp.pr.gov.br/tipos_v03.xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
        xmlLote.Append($"<p:IdentificacaoRps>");
        xmlLote.Append($"<p1:Numero>{retornoWebservice.NumeroRps}</p1:Numero>");
        xmlLote.Append($"<p1:Serie>{retornoWebservice.Serie}</p1:Serie>");
        xmlLote.Append($"<p1:Tipo>{(int)retornoWebservice.Tipo + 1}</p1:Tipo>");
        xmlLote.Append("</p:IdentificacaoRps>");
        xmlLote.Append($"<p:Prestador>");
        xmlLote.Append($"<p1:Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</p1:Cnpj>");
        xmlLote.Append($"<p1:InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</p1:InscricaoMunicipal>");
        xmlLote.Append("</p:Prestador>");
        xmlLote.Append("</p:ConsultarNfseRpsEnvio>");

        retornoWebservice.XmlEnvio = xmlLote.ToString();
    }

    protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        var loteBuilder = new StringBuilder();

        loteBuilder.Append("<ConsultarNfseEnvio xmlns=\"http://nfe.sjp.pr.gov.br/servico_consultar_nfse_envio_v03.xsd\" xmlns:tipos=\"http://nfe.sjp.pr.gov.br/tipos_v03.xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
        loteBuilder.Append("<Prestador>");
        loteBuilder.Append($"<tipos:Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tipos:Cnpj>");
        loteBuilder.Append($"<tipos:InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
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

        loteBuilder.Append("</ConsultarNfseEnvio>");

        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        // Monta mensagem de envio
        var loteBuilder = new StringBuilder();

        loteBuilder.Append($"<ConsultarSituacaoLoteRpsEnvio xmlns=\"http://nfe.sjp.pr.gov.br/servico_consultar_situacao_lote_rps_envio_v03.xsd\" xmlns:tipos=\"http://nfe.sjp.pr.gov.br/tipos_v03.xsd\" xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\" Id=\"{retornoWebservice.Lote}\">");
        loteBuilder.Append($"<Prestador>");
        loteBuilder.Append($"<tipos:Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tipos:Cnpj>");
        loteBuilder.Append($"<tipos:InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
        loteBuilder.Append("</Prestador>");
        loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");
        loteBuilder.Append("</ConsultarSituacaoLoteRpsEnvio>");

        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override string GetNamespace()
    {
        return "xmlns:tipos=\"http://nfe.sjp.pr.gov.br/\"";
    }

    protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        XElement xmlRet = XElement.Parse(retornoWebservice.XmlRetorno);

        XElement returnElement = xmlRet.Element(XName.Get("ConsultarSituacaoLoteRpsV3Response", "http://nfe.sjp.pr.gov.br"))
            .Element("return");

        XElement consultarSituacaoLoteRpsResposta = XElement.Parse(returnElement.Value);

        MensagemErro(retornoWebservice, consultarSituacaoLoteRpsResposta);

        retornoWebservice.Lote = consultarSituacaoLoteRpsResposta?.Element(XName.Get("NumeroLote", "http://nfe.sjp.pr.gov.br/servico_consultar_situacao_lote_rps_resposta_v03.xsd"))?.GetValue<int>() ?? 0;
        retornoWebservice.Situacao = consultarSituacaoLoteRpsResposta?.Element(XName.Get("Situacao", "http://nfe.sjp.pr.gov.br/servico_consultar_situacao_lote_rps_resposta_v03.xsd"))?.GetValue<string>() ?? string.Empty;
        retornoWebservice.Sucesso = !retornoWebservice.Erros.Any();

    }

    protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        XElement xmlRet = XElement.Parse(retornoWebservice.XmlRetorno);

        XElement returnElement = xmlRet.Element(XName.Get("RecepcionarLoteRpsV3Response", "http://nfe.sjp.pr.gov.br")).Element("return");

        XElement recepcionarLoteRpsResponse = XElement.Parse(returnElement.Value);

        MensagemErro(retornoWebservice, recepcionarLoteRpsResponse);
        if (retornoWebservice.Erros.Any()) return;

        retornoWebservice.Data = recepcionarLoteRpsResponse?.ElementAnyNs("DataRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;
        retornoWebservice.Protocolo = recepcionarLoteRpsResponse?.ElementAnyNs("Protocolo")?.GetValue<string>() ?? string.Empty;
        retornoWebservice.Sucesso = !retornoWebservice.Protocolo.IsEmpty();

        if (!retornoWebservice.Sucesso) return;

        foreach (var nota in notas)
        {
            nota.NumeroLote = retornoWebservice.Lote;
        }
    }

    protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
    {
        XElement xmlRet = XElement.Parse(retornoWebservice.XmlRetorno);
        XElement returnElement = xmlRet.Element(XName.Get("ConsultarLoteRpsV3Response", "http://nfe.sjp.pr.gov.br")).Element("return");
        XElement xmlConsultarLoteRpsResult = XElement.Parse(returnElement.Value);

        // Analisa mensagem de retorno            
        MensagemErro(retornoWebservice, xmlConsultarLoteRpsResult);
        if (retornoWebservice.Erros.Any()) return;

        var listaNfse = xmlConsultarLoteRpsResult?.ElementAnyNs("ListaNfse");

        if (listaNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
            return;
        }

        retornoWebservice.Sucesso = true;

        foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
        {
            var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
            var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            var chaveNFSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
            var dataEmissao = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
            var numeroRps = nfse?.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataEmissao);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota == null)
            {
                notas.Load(compNfse.ToString());
            }
            else
            {
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.IdentificacaoNFSe.Chave = chaveNFSe;
                nota.IdentificacaoNFSe.DataEmissao = dataEmissao;
                nota.XmlOriginal = compNfse.AsString();
            }
        }
        retornoWebservice.Notas = notas.ToArray();
    }

    protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        XElement xmlRet = XElement.Parse(retornoWebservice.XmlRetorno);
        XElement returnElement = xmlRet.Element(XName.Get("ConsultarNfsePorRpsV3Response", "http://nfe.sjp.pr.gov.br")).Element("return");
        XElement xmlConsultarNFSePorRpsResult = XElement.Parse(returnElement.Value);

        MensagemErro(retornoWebservice, xmlConsultarNFSePorRpsResult);
        if (retornoWebservice.Erros.Any()) return;

        var compNfse = xmlConsultarNFSePorRpsResult.ElementAnyNs("CompNfse");
        if (compNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (CompNfse)" });
            return;
        }

        // Carrega a nota fiscal na coleção de Notas Fiscais
        var nota = LoadXml(compNfse.AsString());
        notas.Add(nota);

        retornoWebservice.Sucesso = true;
    }

    protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
    {

        XElement xmlRet = XElement.Parse(retornoWebservice.XmlRetorno);
        XElement returnElement = xmlRet.Element(XName.Get("ConsultarNfseV3Response", "http://nfe.sjp.pr.gov.br")).Element("return");
        XElement xmlConsultarNfseResult = XElement.Parse(returnElement.Value);

        MensagemErro(retornoWebservice, xmlConsultarNfseResult);
        if (retornoWebservice.Erros.Any()) return;

        var listaNfse = xmlConsultarNfseResult?.ElementAnyNs("ListaNfse");
        if (listaNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
            return;
        }

        var notasServico = new List<NotaServico>();

        foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
        {
            // Carrega a nota fiscal na coleção de Notas Fiscais
            var nota = LoadXml(compNfse.AsString());
            notas.Add(nota);
            notasServico.Add(nota);
        }

        retornoWebservice.Notas = notasServico.ToArray();
        retornoWebservice.Sucesso = true;
    }

    #endregion Methods

    #region RPS
    protected override XElement WriteRps(NotaServico nota)
    {
        var rps = new XElement("Rps");
        var infoRps = WriteInfoRPS(nota);
        rps.Add(infoRps);

        infoRps.AddChild(WriteRpsSubstituto(nota));
        infoRps.AddChild(WriteServicosValoresRps(nota));
        infoRps.AddChild(WritePrestadorRps(nota));
        infoRps.AddChild(WriteTomadorRps(nota));
        infoRps.AddChild(WriteIntermediarioRps(nota));
        infoRps.AddChild(WriteConstrucaoCivilRps(nota));

        return rps;
    }

    protected override XElement WriteInfoRPS(NotaServico nota)
    {
        var incentivadorCultural = nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2;

        string regimeEspecialTributacao;
        string optanteSimplesNacional;
        if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
        {
            regimeEspecialTributacao = "1";
            optanteSimplesNacional = "1";
        }
        else
        {
            var regime = (int)nota.RegimeEspecialTributacao;
            regimeEspecialTributacao = regime == 0 ? string.Empty : regime.ToString();
            optanteSimplesNacional = "2";
        }

        var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "1" : "2";

        var infoRps = new XElement("InfRps", new XAttribute("Id", $"{nota.IdentificacaoRps.Numero}"));

        infoRps.Add(WriteIdentificacao(nota));
        infoRps.AddChild(AddTag(TipoCampo.DatHor, "", "DataEmissao", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "NaturezaOperacao", 1, 1, Ocorrencia.Obrigatoria, nota.NaturezaOperacao));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "IncentivadorCultural", 1, 1, Ocorrencia.Obrigatoria, incentivadorCultural));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, situacao));

        return infoRps;
    }

    protected override XElement WriteIdentificacao(NotaServico nota)
    {
        string tipoRps;
        switch (nota.IdentificacaoRps.Tipo)
        {
            case TipoRps.RPS:
                tipoRps = "1";
                break;

            case TipoRps.NFConjugada:
                tipoRps = "2";
                break;

            case TipoRps.Cupom:
                tipoRps = "3";
                break;

            default:
                tipoRps = "0";
                break;
        }

        var ideRps = new XElement("IdentificacaoRps");
        ideRps.AddChild(AddTag(TipoCampo.Int, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
        ideRps.AddChild(AddTag(TipoCampo.Str, "", "Serie", 1, 5, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
        ideRps.AddChild(AddTag(TipoCampo.Int, "", "Tipo", 1, 1, Ocorrencia.Obrigatoria, tipoRps));

        return ideRps;
    }

    #endregion

}