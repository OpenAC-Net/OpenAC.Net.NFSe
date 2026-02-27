// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 12-24-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="ProviderWebIss2.cs" company="OpenAC .Net">
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

using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using System;
using System.Linq;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers;

// ReSharper disable once InconsistentNaming
internal sealed class ProviderWebIss2 : ProviderABRASF202
{
    #region Constructors

    public ProviderWebIss2(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "WebISS2";
        Versao = VersaoNFSe.ve202;
    }

    #endregion Constructors

    #region Methods

    protected override XElement WriteServicosRps(NotaServico nota)
    {
        var servico = new XElement("Servico");

        servico.Add(WriteValoresRps(nota));

        servico.AddChild(AddTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));

        if (nota.Servico.ResponsavelRetencao.HasValue)
            servico.AddChild(AddTag(TipoCampo.Int, "", "ResponsavelRetencao", 1, 1, Ocorrencia.NaoObrigatoria, (int)nota.Servico.ResponsavelRetencao + 1));

        servico.AddChild(AddTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoCnae", 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoNbs", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoNbs));
        servico.AddChild(AddTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoMunicipio", 1, 20, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));
        servico.AddChild(AddTag(TipoCampo.Int, "", "CodigoPais", 4, 4, Ocorrencia.MaiorQueZero, nota.Servico.CodigoPais));
        servico.AddChild(AddTag(TipoCampo.Int, "", "ExigibilidadeISS", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Servico.ExigibilidadeIss + 1));
        servico.AddChild(AddTag(TipoCampo.Int, "", "MunicipioIncidencia", 7, 7, Ocorrencia.MaiorQueZero, nota.Servico.MunicipioIncidencia));
        servico.AddChild(AddTag(TipoCampo.Str, "", "NumeroProcesso", 1, 30, Ocorrencia.NaoObrigatoria, nota.Servico.NumeroProcesso));

        return servico;
    }

    protected override XElement WriteRps(NotaServico nota)
    {
        var rootRps = new XElement("Rps");

        var infServico = new XElement("InfDeclaracaoPrestacaoServico", new XAttribute("Id", $"R{nota.IdentificacaoRps.Numero.OnlyNumbers()}"));
        rootRps.Add(infServico);

        infServico.Add(WriteRpsRps(nota));

        infServico.AddChild(AddTag(TipoCampo.Dat, "", "Competencia", 10, 10, Ocorrencia.Obrigatoria, nota.Competencia));

        infServico.AddChild(WriteServicosRps(nota));
        infServico.AddChild(WritePrestadorRps(nota));
        infServico.AddChild(WriteTomadorRps(nota));
        infServico.AddChild(WriteIntermediarioRps(nota));
        infServico.AddChild(WriteConstrucaoCivilRps(nota));

        var regimeEspecialTributacao = nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional
                ? "6"
                : ((int)nota.RegimeEspecialTributacao).ToString();

        bool optanteSimplesNacional = false;

        switch (nota.RegimeEspecialTributacao)
        {
            case RegimeEspecialTributacao.SimplesNacional:
            case RegimeEspecialTributacao.MicroEmpresarioIndividual:
            case RegimeEspecialTributacao.MicroEmpresarioEmpresaPP:
                optanteSimplesNacional = true;
                break;
        }

        if (nota.RegimeEspecialTributacao != RegimeEspecialTributacao.Nenhum)
            infServico.AddChild(AddTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));

        infServico.AddChild(AddTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional ? 1 : 2));
        infServico.AddChild(AddTag(TipoCampo.Int, "", "IncentivoFiscal", 1, 1, Ocorrencia.Obrigatoria, nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2));

        var IBSCBS = WriteIBSCBSRps(nota);

        if (IBSCBS != null)
            infServico.AddChild(IBSCBS);

        return rootRps;
    }
    XElement? WriteIBSCBSRps(NotaServico nota)
    {
        var info = nota.Servico.Valores.IBSCBS;
        if (info == null) return null;

        var ibsCbs = new XElement("IBSCBS");

        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "finNFSe", 1, 1, Ocorrencia.Obrigatoria, info.FinalidadeNFSe));
        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "indFinal", 1, 1, Ocorrencia.Obrigatoria, info.IndicadorFinal));
        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "cIndOp", 6, 6, Ocorrencia.Obrigatoria, info.CodigoIndicadorOperacao));
        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "tpOper", 1, 1, Ocorrencia.NaoObrigatoria, info.TipoOperacao));
        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "indDest", 1, 1, Ocorrencia.Obrigatoria, info.IndicadorDestinatario));

        var valores = new XElement("valores");
        var trib = new XElement("trib");
        var gIbsCbs = new XElement("gIBSCBS");
        gIbsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "CST", 3, 3, Ocorrencia.Obrigatoria, info.Valores.Tributos.SituacaoClassificacao.CodigoSituacaoTributaria));
        gIbsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "cClassTrib", 6, 6, Ocorrencia.Obrigatoria, info.Valores.Tributos.SituacaoClassificacao.CodigoClassificacaoTributaria));
        trib.AddChild(gIbsCbs);
        valores.AddChild(trib);
        ibsCbs.AddChild(valores);

        return ibsCbs;
    }

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet, "EnviarLoteRpsSincronoResposta");
        if (retornoWebservice.Erros.Count != 0) return;

        retornoWebservice.Data = xmlRet.Root?.ElementAnyNs("DataRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;

        MensagemErro(retornoWebservice, xmlRet, "EnviarLoteRpsSincronoResposta");

        var listaNfse = xmlRet.Root.ElementAnyNs("ListaNfse");

        if (listaNfse is null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
            return;
        }

        retornoWebservice.Sucesso = true;

        foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
        {            
            var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
            var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            var chaveNfSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
            var dataNfSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
            var numeroRps = nfse.ElementAnyNs("DeclaracaoPrestacaoServico")?
                .ElementAnyNs("InfDeclaracaoPrestacaoServico")?
                .ElementAnyNs("Rps")?
                .ElementAnyNs("IdentificacaoRps")?
                .ElementAnyNs("Numero").GetValue<string>() ?? string.Empty;

            retornoWebservice.Protocolo = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;

            GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNfSe}-.xml", dataNfSe);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota == null)
            {
                nota = notas.Load(compNfse.ToString());
            }
            else
            {
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.IdentificacaoNFSe.Chave = chaveNfSe;
                nota.IdentificacaoNFSe.DataEmissao = dataNfSe;
                nota.XmlOriginal = compNfse.ToString();
            }

            nota.Protocolo = retornoWebservice.Protocolo;
        }
    }
    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new WebIss2ServiceClient(this, tipo);
    }

    #endregion Methods
}