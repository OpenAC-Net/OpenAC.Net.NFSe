// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Diego Martins
// Created          : 08-30-2021
//
// ***********************************************************************
// <copyright file="ProviderBase.cs" company="OpenAC .Net">
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderSigISSWeb : ProviderBase
{
    public ProviderSigISSWeb(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "SigISSWeb";
        Versao = VersaoNFSe.ve100;
    }

    private static string FormataDecimal(decimal valor) => valor.ToString("0.00").Replace(".", ",");

    protected override IServiceClient GetClient(TipoUrl tipo) => new SigISSWebServiceClient(this, tipo);

    public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        var xmldoc = new XDocument(new XDeclaration("1.0", "ISO-8859-1", null));
        var notaTag = new XElement("notafiscal");
        xmldoc.Add(notaTag);

        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cnpj_cpf_prestador", 1, 14, Ocorrencia.Obrigatoria, nota.Prestador.CpfCnpj.OnlyNumbers()));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cnpj_cpf_destinatario", 1, 14, Ocorrencia.Obrigatoria, nota.Tomador.CpfCnpj.OnlyNumbers()));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "pessoa_destinatario", 1, 1, Ocorrencia.Obrigatoria, nota.Tomador.CpfCnpj.OnlyNumbers().Length == 11 ? "F" : "J"));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "exterior_dest", 1, 1, Ocorrencia.Obrigatoria, nota.Tomador.EnderecoExterior.EnderecoCompleto == null ? 0 : 1));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "ie_destinatario", 1, 15, Ocorrencia.Obrigatoria, nota.Tomador.InscricaoEstadual));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "im_destinatario", 1, 15, Ocorrencia.Obrigatoria, nota.Tomador.InscricaoMunicipal));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "razao_social_destinatario", 1, 100, Ocorrencia.Obrigatoria, nota.Tomador.RazaoSocial));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "endereco_destinatario", 1, 60, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Logradouro));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "numero_ende_destinatario", 1, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Numero));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "complemento_ende_destinatario", 1, 40, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Complemento));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "bairro_destinatario", 1, 100, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Bairro));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cep_destinatario", 1, 8, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Cep));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cidade_destinatario", 1, 100, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Municipio));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "uf_destinatario", 1, 2, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Uf));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "pais_destinatario", 1, 50, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Pais));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "fone_destinatario", 1, 30, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.Telefone));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "email_destinatario", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.Email));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "valor_nf", 1, 14, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorLiquidoNfse));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "deducao", 1, 14, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorDeducoes));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "valor_servico", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorServicos)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "forma_de_pagamento", 1, 40, Ocorrencia.Obrigatoria, string.Empty));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "descricao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "id_codigo_servico", 1, 6, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cancelada", 1, 1, Ocorrencia.Obrigatoria, "N"));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "iss_retido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? "S" : "N"));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "aliq_iss", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.Aliquota)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "valor_iss", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorIss)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "bc_pis", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.BaseCalculo)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "aliq_pis", 1, 5, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.AliquotaPis)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "valor_pis", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorPis)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "bc_cofins", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.BaseCalculo)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "aliq_cofins", 1, 5, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.AliquotaCofins)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "valor_cofins", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorCofins)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "bc_csll", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.BaseCalculo)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "aliq_csll", 1, 5, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.AliquotaCsll)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "valor_csll", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorCsll)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "bc_irrf", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.BaseCalculo)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "aliq_irrf", 1, 5, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.AliquotaIR)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "valor_irrf", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorIr)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "bc_inss", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.BaseCalculo)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "aliq_inss", 1, 5, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.AliquotaInss)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "valor_inss", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorInss)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "rps", 1, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "serie_rps", 1, 3, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "data_emissao", 1, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao.ToString("dd/MM/yyyy")));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "sistema_gerador", 1, 15, Ocorrencia.Obrigatoria, "OpenAC.Net.NFSe"));

        return xmldoc.Root.AsString(identado, showDeclaration, Encoding.UTF8);
    }

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (notas.Count == 0) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "RPS não informado." });
        if (notas.Count > 1) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Este provedor aceita apenas uma RPS por vez" });
        if (retornoWebservice.Erros.Count > 0) return;

        var nota = notas[0];
        var xmlRps = WriteXmlRps(nota);

        GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);

        retornoWebservice.XmlEnvio = xmlRps;
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        var message = new StringBuilder();
        message.Append("<Consulta>");
        message.Append($"<NumeroRPS>{retornoWebservice.NumeroRps}</NumeroRPS>");
        message.Append($"<SerieRPS>{retornoWebservice.Serie}</SerieRPS>");
        message.Append("</Consulta>");
        retornoWebservice.XmlEnvio = message.ToString();
    }

    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        var message = new StringBuilder();
        message.Append("<Cancela>");
        message.Append($"<NumeroNFSe>{retornoWebservice.NumeroNFSe}</NumeroNFSe>");
        message.Append($"<SerieNFSe>{retornoWebservice.SerieNFSe}</SerieNFSe>");
        message.Append($"<Motivo>{retornoWebservice.Motivo}</Motivo>");
        message.Append("</Cancela>");
        retornoWebservice.XmlEnvio = message.ToString();
    }

    protected override bool PrecisaValidarSchema(TipoUrl tipo)
    {
        return false;
    }

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

        retornoWebservice.Data = xmlRet.Root?.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.MinValue;
        retornoWebservice.Protocolo = xmlRet.Root?.ElementAnyNs("codigo")?.GetValue<string>() ?? string.Empty;
        retornoWebservice.Sucesso = !retornoWebservice.Protocolo.IsEmpty();

        if (!retornoWebservice.Sucesso) return;

        var numeroNFSe = xmlRet.Root.ElementAnyNs("numero_nf")?.GetValue<string>() ?? string.Empty;
        var chaveNFSe = xmlRet.Root.ElementAnyNs("codigo")?.GetValue<string>() ?? string.Empty;
        var dataNFSe = xmlRet.Root.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.Now;
        var numeroRps = xmlRet.Root.ElementAnyNs("rps")?.GetValue<string>() ?? string.Empty;

        GravarNFSeEmDisco(xmlRet.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

        var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
        if (nota == null)
        {
            notas.Load(retornoWebservice.XmlRetorno);
        }
        else
        {
            nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
            nota.IdentificacaoNFSe.Numero = numeroNFSe;
            nota.IdentificacaoNFSe.Chave = chaveNFSe;
            nota.XmlOriginal = retornoWebservice.XmlRetorno;
        }
    }

    protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

        var numeroNFSe = xmlRet.Root.ElementAnyNs("numero_nf")?.GetValue<string>() ?? string.Empty;
        var chaveNFSe = xmlRet.Root.ElementAnyNs("codigo")?.GetValue<string>() ?? string.Empty;
        var dataNFSe = xmlRet.Root.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.Now;
        var numeroRps = xmlRet.Root.ElementAnyNs("rps")?.GetValue<string>() ?? string.Empty;

        GravarNFSeEmDisco(xmlRet.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

        var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
        if (nota == null)
        {
            nota = notas.Load(retornoWebservice.XmlRetorno);
        }
        else
        {
            nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
            nota.IdentificacaoNFSe.Numero = numeroNFSe;
            nota.IdentificacaoNFSe.Chave = chaveNFSe;
            nota.XmlOriginal = retornoWebservice.XmlRetorno;
        }

        retornoWebservice.Nota = nota;
        retornoWebservice.Sucesso = true;
    }

    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == retornoWebservice.NumeroNFSe);

        retornoWebservice.Sucesso = true;
        nota.Situacao = SituacaoNFSeRps.Cancelado;
        nota.Cancelamento.MotivoCancelamento = retornoWebservice.Motivo;
        nota.Cancelamento.DataHora = retornoWebservice.Data;
    }

    public override NotaServico LoadXml(XDocument xml)
    {
        Guard.Against<XmlException>(xml == null, "Xml invalido.");
        XElement notaXml = xml.ElementAnyNs("notafiscal");
        Guard.Against<XmlException>(notaXml == null, "Xml de RPS ou NFSe invalido.");

        var nota = new NotaServico(Configuracoes);

        // Nota Fiscal
        nota.IdentificacaoNFSe.Numero = notaXml.ElementAnyNs("numero_nf")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoNFSe.Chave = notaXml.ElementAnyNs("codigo")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoNFSe.DataEmissao = notaXml.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.MinValue;

        // RPS
        nota.IdentificacaoRps.Numero = notaXml.ElementAnyNs("rps")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoRps.Serie = notaXml.ElementAnyNs("serie_rps")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoRps.Tipo = TipoRps.RPS;
        nota.IdentificacaoRps.DataEmissao = notaXml.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.MinValue;

        // Situação do RPS
        nota.Situacao = (notaXml.ElementAnyNs("cancelada")?.GetValue<string>() ?? string.Empty) == "S" ? SituacaoNFSeRps.Cancelado : SituacaoNFSeRps.Normal;

        // Serviços e Valores
        nota.Servico.Valores.ValorServicos = notaXml.ElementAnyNs("valor_servico")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorDeducoes = notaXml.ElementAnyNs("deducao")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorPis = notaXml.ElementAnyNs("valor_pis")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorCofins = notaXml.ElementAnyNs("valor_cofins")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorInss = notaXml.ElementAnyNs("valor_inss")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorIr = notaXml.ElementAnyNs("valor_irrf")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorCsll = notaXml.ElementAnyNs("valor_csll")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.IssRetido = (notaXml.ElementAnyNs("iss_retido")?.GetValue<string>() ?? string.Empty) == "S" ? SituacaoTributaria.Retencao : SituacaoTributaria.Normal;
        nota.Servico.Valores.ValorIss = notaXml.ElementAnyNs("valor_iss")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.BaseCalculo = notaXml.ElementAnyNs("valor_servico")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.Aliquota = notaXml.ElementAnyNs("aliq_iss")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorLiquidoNfse = notaXml.ElementAnyNs("valor_nf")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorIssRetido = nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? nota.Servico.Valores.ValorIss : 0;
        nota.Servico.ItemListaServico = notaXml.ElementAnyNs("id_codigo_servico")?.GetValue<string>() ?? string.Empty;
        nota.Servico.Discriminacao = notaXml.ElementAnyNs("descricao")?.GetValue<string>() ?? string.Empty;

        // Prestador
        nota.Prestador.CpfCnpj = notaXml.ElementAnyNs("cnpj_cpf_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.InscricaoMunicipal = notaXml.ElementAnyNs("im_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.RazaoSocial = notaXml.ElementAnyNs("razao_social_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Logradouro = notaXml.ElementAnyNs("endereco_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Numero = notaXml.ElementAnyNs("numero_ende_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Complemento = notaXml.ElementAnyNs("complemento_ende_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Bairro = notaXml.ElementAnyNs("bairro_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Municipio = notaXml.ElementAnyNs("cidade_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Uf = notaXml.ElementAnyNs("uf_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Cep = notaXml.ElementAnyNs("cep_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.DadosContato.Email = notaXml.ElementAnyNs("email_prestador")?.GetValue<string>() ?? string.Empty;

        // Tomador
        nota.Tomador.CpfCnpj = notaXml.ElementAnyNs("cnpj_cpf_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.InscricaoMunicipal = notaXml.ElementAnyNs("im_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.RazaoSocial = notaXml.ElementAnyNs("razao_social_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Logradouro = notaXml.ElementAnyNs("endereco_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Numero = notaXml.ElementAnyNs("numero_ende_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Complemento = notaXml.ElementAnyNs("complemento_ende_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Bairro = notaXml.ElementAnyNs("bairro_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Cep = notaXml.ElementAnyNs("cep_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Municipio = notaXml.ElementAnyNs("cidade_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Uf = notaXml.ElementAnyNs("uf_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Pais = notaXml.ElementAnyNs("pais_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.DadosContato.Telefone = notaXml.ElementAnyNs("fone_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.DadosContato.Email = notaXml.ElementAnyNs("email_destinatario")?.GetValue<string>() ?? string.Empty;

        return nota;
    }

    protected override string GerarCabecalho() => string.Empty;

    protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
    {
        //
    }

    protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        //
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        //
    }

    #region Não implementados

    public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        throw new NotImplementedException();
    }

    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException();
    }

    protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        throw new NotImplementedException();
    }

    protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        throw new NotImplementedException();
    }

    protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
        throw new NotImplementedException();
    }

    protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        throw new NotImplementedException();
    }

    protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException();
    }

    protected override void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException();
    }

    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
        throw new NotImplementedException();
    }

    protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        throw new NotImplementedException();
    }

    protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        throw new NotImplementedException();
    }

    protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
        throw new NotImplementedException();
    }

    protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        throw new NotImplementedException();
    }

    protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice)
    {
        throw new NotImplementedException();
    }

    protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice)
    {
        throw new NotImplementedException();
    }

    protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException();
    }

    protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        throw new NotImplementedException();
    }

    protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException();
    }

    protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
        throw new NotImplementedException();
    }

    protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException();
    }

    protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException();
    }

    protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException();
    }

    protected override string GetSchema(TipoUrl tipo)
    {
        throw new NotImplementedException();
    }

    #endregion Não implementados
}