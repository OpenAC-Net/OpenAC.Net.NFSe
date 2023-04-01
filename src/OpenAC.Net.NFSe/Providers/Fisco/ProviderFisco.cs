// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 01-11-2023
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 03-15-2023
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
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

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
        xmlLote.Append("<GerarNfseEnvio>");
        xmlLote.Append(xmlLoteRps);
        xmlLote.Append("</GerarNfseEnvio>");

        //var xmlLote = new StringBuilder();
        //xmlLote.Append("<recepcionarLoteRpsSincrono xmlns=\"https://www.fisco.net.br/wsnfseabrasf/ServicosNFSEAbrasf.asmx\">");
        //xmlLote.Append("<xml>");
        //xmlLote.Append(xmlLoteRps);
        //xmlLote.Append("</xml>");
        //xmlLote.Append("</recepcionarLoteRpsSincrono>");
        retornoWebservice.XmlEnvio = xmlLote.ToString();
    }

    protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(AjustarRetorno(retornoWebservice.XmlRetorno));

        retornoWebservice.Lote = xmlRet?.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;
        retornoWebservice.Situacao = xmlRet?.ElementAnyNs("Situacao")?.GetValue<string>() ?? "0";
        retornoWebservice.Sucesso = !retornoWebservice.Erros.Any();
    }

    protected override XElement WriteInfoRPS(NotaServico nota)
    {
        //var incentivadorCultural = nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2;

        //string regimeEspecialTributacao;
        //string optanteSimplesNacional;
        //if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
        //{
        //    regimeEspecialTributacao = "6";
        //    optanteSimplesNacional = "1";
        //}
        //else
        //{
        //    var regime = (int)nota.RegimeEspecialTributacao;
        //    regimeEspecialTributacao = regime == 0 ? string.Empty : regime.ToString();
        //    optanteSimplesNacional = "2";
        //}

        var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "1" : "2";

        var infoRps = new XElement("InfDeclaracaoPrestacaoServico");

        var Rps = new XElement("Rps");
        Rps.Add(WriteIdentificacao(nota));
        Rps.AddChild(AdicionarTag(TipoCampo.Dat, "", "DataEmissao", 10, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
        //Rps.AddChild(AdicionarTag(TipoCampo.Int, "", "NaturezaOperacao", 1, 1, Ocorrencia.Obrigatoria, nota.NaturezaOperacao));
        //Rps.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
        //Rps.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
        //Rps.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivadorCultural", 1, 1, Ocorrencia.Obrigatoria, incentivadorCultural));
        Rps.AddChild(AdicionarTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, situacao));

        infoRps.Add(Rps);
        return infoRps;
    }

    protected override XElement WriteRps(NotaServico nota)
    {
        var rps = new XElement("Rps");
        var infoRps = WriteInfoRPS(nota);
        rps.Add(infoRps);

        infoRps.AddChild(WriteRpsSubstituto(nota));
        infoRps.AddChild(AdicionarTag(TipoCampo.Dat, "", "Competencia", 10, 10, Ocorrencia.Obrigatoria, nota.Competencia));
        infoRps.AddChild(WriteServicosValoresRps(nota));
        infoRps.AddChild(WritePrestadorRps(nota));
        infoRps.AddChild(WriteTomadorRps(nota));
        infoRps.AddChild(WriteIntermediarioRps(nota));
        infoRps.AddChild(WriteConstrucaoCivilRps(nota));

        return rps;
    }

    protected override XElement WritePrestadorRps(NotaServico nota)
    {
        var prestador = new XElement("Prestador");

        var cpfCnpjPrestador = new XElement("CpfCnpj");

        cpfCnpjPrestador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj));
        prestador.Add(cpfCnpjPrestador);
        prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Prestador.InscricaoMunicipal));

        return prestador;
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
        //NAO PRECISA ASSINAR NO FISCO
        //retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "Pedido", "InfPedidoCancelamento", Certificado);
    }

    #endregion Services

    #region Methods

    private static string AjustarRetorno(string retorno)
    {
        for (var i = 0; i < escapedCharacters.Length; i++)
        {
            retorno = retorno.Replace(escapedCharacters[i], unescapedCharacters[i]);
        }
        retorno = retorno.Replace("xmlns=\"\"", "");
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