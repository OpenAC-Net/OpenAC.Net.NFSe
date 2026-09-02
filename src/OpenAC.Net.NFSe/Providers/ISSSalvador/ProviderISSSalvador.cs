// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Fabio Pimenta Correa
// Created          : 28-06-2026
//
// Last Modified By : Fabio Pimenta Correa
// Last Modified On : 28-06-2026
// ***********************************************************************
// <copyright file="ProviderSimplISSv2.cs" company="OpenAC .Net">
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

using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Document;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderISSSalvador : ProviderABRASF
{
    #region Constructors

    public ProviderISSSalvador(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "ISSSalvador";
    }

    #endregion Constructors

    #region Methods

    protected override string GetNamespace() => "xmlns=\"http://www.abrasf.org.br/ABRASF/arquivos/nfse.xsd\"";

    protected override IServiceClient GetClient(TipoUrl tipo) => new ISSSalvadorServiceClient(this, tipo);

    protected override string GetSchema(TipoUrl tipo) => "nfse.xsd";

    protected override bool PrecisaValidarSchema(TipoUrl tipo)
    {
        return true;
    }
    
    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
         retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "InfRps","id", Certificado);
         retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsEnvio", "LoteRps","id", Certificado);
    }

  
    protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice) { /* sem assinatura;*/ }
    protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)  { /*  sem assinatura; */}
    protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice) { /*  sem assinatura; */}



    #endregion Methods

    #region RPS
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
        xmlLote.Append($"<EnviarLoteRpsEnvio {GetNamespace()}>");
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
        ;

        retornoWebservice.XmlEnvio = xmlLote.ToString().Replace("Id=", "id=");//para salvador o ID é minúsculo
    }

   

    protected override XElement WriteServicosValoresRps(NotaServico nota)
    {
        var servico = new XElement("Servico");
        var valores = new XElement("Valores");
        servico.AddChild(valores);

        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));

        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));

        valores.AddChild(AddTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));

        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorIssRetido", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIssRetido));
        valores.AddChild(AddTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));
        valores.AddChild(AddTag(TipoCampo.De2, "", "BaseCalculo", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.BaseCalculo));

        // Valor Percentual - Exemplos: 1% => 0.01   /   25,5% => 0.255   /   100% => 1
        valores.AddChild(AddTag(TipoCampo.De4, "", "Aliquota", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota / 100));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorLiquidoNfse", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorLiquidoNfse));
        valores.AddChild(AddTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
        valores.AddChild(AddTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));

        servico.AddChild(AddTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));

        servico.AddChild(AddTag(TipoCampo.StrNumber, "", "CodigoCnae", 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));

        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
        servico.AddChild(AddTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
        servico.AddChild(AddTag(TipoCampo.StrNumber, "", "CodigoMunicipio", 1, 7, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));

        servico.AddChild(AddTag(TipoCampo.Str, "", "NBS", 1, 9, Ocorrencia.Obrigatoria, nota.Servico.CodigoNbs));
        servico.AddChild(AddTag(TipoCampo.Str, "", "cClassTrib", 1, 6, Ocorrencia.Obrigatoria, nota.Servico.CodigoClassificacaoTributaria));
        servico.AddChild(AddTag(TipoCampo.Str, "", "INDOP", 1, 6, Ocorrencia.Obrigatoria, nota.Servico.CodigoIndicadorOperacao));


        return servico;
    }

    /*
    protected override XElement WriteValoresRps(NotaServico nota)
    {
        var valores = new XElement("Valores");

        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));
        valores.AddChild(AddTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValTotTributos", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValTotTributos));

        var valorISS = nota.Servico.Valores.ValorIss;

        if (valorISS <= 0 && nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao && nota.Servico.Valores.ValorIssRetido > 0)
            valorISS = nota.Servico.Valores.ValorIssRetido;

        if (nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao)
            valores.AddChild(AddTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, valorISS));

        if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional || nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao)
            valores.AddChild(AddTag(TipoCampo.De2, "", "Aliquota", 1, 5, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota));

        valores.AddChild(AddTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
        valores.AddChild(AddTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));


        var trib = WriteTribRps(nota);
        if (trib != null)
            valores.AddChild(trib);

        var IBSCBS = WriteIBSCBSRps(nota);

        if (IBSCBS != null)
            valores.AddChild(IBSCBS);


        return valores;
    }
    */
    /*
    protected XElement? WriteTribRps(NotaServico nota)
    {
        if (string.IsNullOrWhiteSpace(nota.Servico.Valores.TipoRetencaoPisCofins))
            return null;

        var trib = new XElement("trib");
        var tribFed = WriteTribFedRps(nota);
        if (tribFed != null)
            trib.AddChild(tribFed);

        var tribTot = WriteTribTotRps(nota);
        if (tribTot != null)
            trib.AddChild(tribTot);

        return trib;
    }
    */
    /*
    protected XElement WriteTribFedRps(NotaServico nota)
    {
        var valores = nota.Servico.Valores;

        var tribFed = new XElement("tribFed");
        var piscofins = new XElement("piscofins");
        piscofins.AddChild(AddTag(TipoCampo.StrNumber, "", "CST", 1, 1, Ocorrencia.Obrigatoria, valores.CstPisCofins));
        piscofins.AddChild(AddTag(TipoCampo.De2, "", "vBCPisCofins", 1, 1, Ocorrencia.Obrigatoria, valores.BaseCalculo));
        piscofins.AddChild(AddTag(TipoCampo.De2, "", "pAliqPis", 1, 1, Ocorrencia.Obrigatoria, valores.AliquotaPis));
        piscofins.AddChild(AddTag(TipoCampo.De2, "", "pAliqCofins", 1, 1, Ocorrencia.Obrigatoria, valores.AliquotaCofins));
        piscofins.AddChild(AddTag(TipoCampo.De2, "", "vPis", 1, 1, Ocorrencia.Obrigatoria, valores.ValorPis));
        piscofins.AddChild(AddTag(TipoCampo.De2, "", "vCofins", 1, 1, Ocorrencia.Obrigatoria, valores.ValorCofins));
        piscofins.AddChild(AddTag(TipoCampo.StrNumber, "", "tpRetPisCofins", 1, 1, Ocorrencia.Obrigatoria, valores.TipoRetencaoPisCofins));

        tribFed.AddChild(piscofins);

        return tribFed;
    }
    */
    /*
    protected XElement? WriteTribTotRps(NotaServico nota)
    {
        var valores = nota.Servico.Valores;

        if (!valores.AliquotaTotalEstadual.HasValue && !valores.AliquotaTotalEstadual.HasValue && valores.AliquotaTotalMunicipal.HasValue)
            return null;

        var totTrib = new XElement("totTrib");
        var pTotTrib = new XElement("pTotTrib");
        pTotTrib.AddChild(AddTag(TipoCampo.De2, "", "pTotTribFed", 1, 1, Ocorrencia.Obrigatoria, valores.AliquotaTotalEstadual ?? 0));
        pTotTrib.AddChild(AddTag(TipoCampo.De2, "", "pTotTribEst", 1, 1, Ocorrencia.Obrigatoria, valores.AliquotaTotalEstadual ?? 0));
        pTotTrib.AddChild(AddTag(TipoCampo.De2, "", "pTotTribMun", 1, 1, Ocorrencia.Obrigatoria, valores.AliquotaTotalMunicipal ?? 0));

        totTrib.AddChild(pTotTrib);

        return totTrib;
    }
    */
    /*
    protected XElement? WriteIBSCBSRps(NotaServico nota)
    {
        var info = nota.Servico.Valores.IBSCBS;
        if (info == null) return null;

        var ibsCbs = new XElement("IBSCBS");

        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "finNFSe", 1, 1, Ocorrencia.Obrigatoria, info.FinalidadeNFSe));
        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "indFinal", 1, 1, Ocorrencia.Obrigatoria, info.IndicadorFinal));
        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "cIndOp", 6, 6, Ocorrencia.Obrigatoria, info.CodigoIndicadorOperacao));
        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "tpOper", 1, 1, Ocorrencia.NaoObrigatoria, info.TipoOperacao));

        var referencias = info.ReferenciasNFSe.Where(x => !x.IsEmpty()).ToList();
        if (referencias.Count > 0)
        {
            var gRefNFSe = new XElement("gRefNFSe");
            foreach (var referencia in referencias)
            {
                gRefNFSe.AddChild(AddTag(TipoCampo.Str, "", "refNFSe", 1, 50, Ocorrencia.Obrigatoria, referencia));
            }

            ibsCbs.AddChild(gRefNFSe);
        }

        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "tpEnteGov", 1, 1, Ocorrencia.NaoObrigatoria, info.TipoEnteGov));
        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "indDest", 1, 1, Ocorrencia.Obrigatoria, info.IndicadorDestinatario));

        var valores = new XElement("valores");
        var reeRepRes = info.Valores.ReembolsoRepasseRessarcimento;
        if (reeRepRes?.Documentos?.Count > 0)
        {
            var gReeRepRes = new XElement("gReeRepRes");

            foreach (var documento in reeRepRes.Documentos)
            {
                var documentos = new XElement("documentos");
                var docAdicionado = false;

                if (documento.DocumentoDFeNacional != null)
                {
                    var dFe = new XElement("dFeNacional");
                    dFe.AddChild(AddTag(TipoCampo.StrNumber, "", "tipoChaveDFe", 1, 1, Ocorrencia.Obrigatoria, documento.DocumentoDFeNacional.TipoChaveDFe));
                    dFe.AddChild(AddTag(TipoCampo.Str, "", "xTipoChaveDFe", 1, 255, Ocorrencia.NaoObrigatoria, documento.DocumentoDFeNacional.DescricaoTipoChaveDFe));
                    dFe.AddChild(AddTag(TipoCampo.Str, "", "chaveDFe", 1, 50, Ocorrencia.Obrigatoria, documento.DocumentoDFeNacional.ChaveDFe));
                    documentos.AddChild(dFe);
                    docAdicionado = true;
                }
                else if (documento.DocumentoFiscalOutro != null)
                {
                    var docFiscal = new XElement("docFiscalOutro");
                    docFiscal.AddChild(AddTag(TipoCampo.StrNumber, "", "cMunDocFiscal", 7, 7, Ocorrencia.Obrigatoria, documento.DocumentoFiscalOutro.CodigoMunicipioDocumentoFiscal));
                    docFiscal.AddChild(AddTag(TipoCampo.Str, "", "nDocFiscal", 1, 255, Ocorrencia.Obrigatoria, documento.DocumentoFiscalOutro.NumeroDocumentoFiscal));
                    docFiscal.AddChild(AddTag(TipoCampo.Str, "", "xDocFiscal", 1, 255, Ocorrencia.Obrigatoria, documento.DocumentoFiscalOutro.DescricaoDocumentoFiscal));
                    documentos.AddChild(docFiscal);
                    docAdicionado = true;
                }
                else if (documento.DocumentoOutro != null)
                {
                    var docOutro = new XElement("docOutro");
                    docOutro.AddChild(AddTag(TipoCampo.Str, "", "nDoc", 1, 255, Ocorrencia.Obrigatoria, documento.DocumentoOutro.NumeroDocumento));
                    docOutro.AddChild(AddTag(TipoCampo.Str, "", "xDoc", 1, 255, Ocorrencia.Obrigatoria, documento.DocumentoOutro.DescricaoDocumento));
                    documentos.AddChild(docOutro);
                    docAdicionado = true;
                }

                if (!docAdicionado) continue;

                if (documento.Fornecedor != null)
                {
                    var fornec = new XElement("fornec");

                    if (!documento.Fornecedor.Cnpj.IsEmpty())
                        fornec.AddChild(AddTag(TipoCampo.StrNumber, "", "CNPJ", 14, 14, Ocorrencia.Obrigatoria, documento.Fornecedor.Cnpj));
                    else if (!documento.Fornecedor.Cpf.IsEmpty())
                        fornec.AddChild(AddTag(TipoCampo.StrNumber, "", "CPF", 11, 11, Ocorrencia.Obrigatoria, documento.Fornecedor.Cpf));
                    else if (!documento.Fornecedor.Nif.IsEmpty())
                        fornec.AddChild(AddTag(TipoCampo.Str, "", "NIF", 1, 40, Ocorrencia.Obrigatoria, documento.Fornecedor.Nif));
                    else if (!documento.Fornecedor.CodigoNaoNif.IsEmpty())
                        fornec.AddChild(AddTag(TipoCampo.StrNumber, "", "cNaoNIF", 1, 1, Ocorrencia.Obrigatoria, documento.Fornecedor.CodigoNaoNif));

                    fornec.AddChild(AddTag(TipoCampo.Str, "", "xNome", 1, 150, Ocorrencia.Obrigatoria, documento.Fornecedor.Nome));

                    if (fornec.HasElements)
                        documentos.AddChild(fornec);
                }

                documentos.AddChild(AddTag(TipoCampo.Dat, "", "dtEmiDoc", 1, 1, Ocorrencia.Obrigatoria, documento.DataEmissaoDocumento));
                documentos.AddChild(AddTag(TipoCampo.Dat, "", "dtCompDoc", 1, 1, Ocorrencia.Obrigatoria, documento.DataCompetenciaDocumento));
                documentos.AddChild(AddTag(TipoCampo.StrNumber, "", "tpReeRepRes", 2, 2, Ocorrencia.Obrigatoria, documento.TipoReeRepRes));
                documentos.AddChild(AddTag(TipoCampo.Str, "", "xTpReeRepRes", 1, 150, Ocorrencia.NaoObrigatoria, documento.DescricaoTipoReeRepRes));
                documentos.AddChild(AddTag(TipoCampo.De2, "", "vlrReeRepRes", 1, 15, Ocorrencia.Obrigatoria, documento.ValorReeRepRes));

                gReeRepRes.Add(documentos);
            }

            if (gReeRepRes.HasElements)
                valores.AddChild(gReeRepRes);
        }

        var trib = new XElement("trib");
        var gIbsCbs = new XElement("gIBSCBS");
        gIbsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "CST", 3, 3, Ocorrencia.Obrigatoria, info.Valores.Tributos.SituacaoClassificacao.CodigoSituacaoTributaria));
        gIbsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "cClassTrib", 6, 6, Ocorrencia.Obrigatoria, info.Valores.Tributos.SituacaoClassificacao.CodigoClassificacaoTributaria));
        trib.AddChild(gIbsCbs);
        valores.AddChild(trib);

        valores.AddChild(AddTag(TipoCampo.StrNumber, "", "cLocalidadeIncid", 7, 7, Ocorrencia.Obrigatoria, info.Valores.CodigoLocalidadeIncidencia));
        valores.AddChild(AddTag(TipoCampo.De2, "", "pRedutor", 1, 5, Ocorrencia.Obrigatoria, info.Valores.PercentualRedutor));
        valores.AddChild(AddTag(TipoCampo.De2, "", "vBC", 1, 15, Ocorrencia.MaiorQueZero, info.Valores.ValorBaseCalculo));

        ibsCbs.AddChild(valores);

        return ibsCbs;
    }
    */
    #endregion RPS
}
