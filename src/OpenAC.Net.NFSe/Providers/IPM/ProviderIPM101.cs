// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 30-05-2022
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 30-05-2022
//
// ***********************************************************************
// <copyright file="ProviderIPM.cs" company="OpenAC .Net">
//		       		   The MIT License (MIT)
//	     	Copyright (c) 2014 - 2023 Projeto OpenAC .Net
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
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal class ProviderIPM101 : ProviderBase
{
    public ProviderIPM101(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "IPM";
        Versao = VersaoNFSe.ve101;
    }

    private static string FormataDecimal(decimal valor)
    {
        return valor.ToString("0.00").Replace(".", ",");
    }

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new IPM101ServiceClient(this, tipo);
    }

    public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        var xmldoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));

        var nfseTag = new XElement("nfse", new XAttribute("id", $"rps:{nota.IdentificacaoRps.Numero}"));
        xmldoc.Add(nfseTag);

        if (!string.IsNullOrEmpty(nota.IdentificacaoRps.Numero))
        {
            var rpsTag = new XElement("rps");

            rpsTag.AddChild(AdicionarTag(TipoCampo.Int, "", "nro_recibo_provisorio", 1, 9, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Int, "", "serie_recibo_provisorio", 1, 2, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "data_emissao_recibo_provisorio", 10, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao.ToString("dd/MM/yyyy")));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "hora_emissao_recibo_provisorio", 8, 8, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao.ToString("HH:mm:ss")));
            
            nfseTag.Add(rpsTag);
        }

        //IMPORTANTÍSSIMO - IPM, algumas prefeituras não permitem a utilização de RPS. Nestes casos, a única forma de fazer essa nota fiscal ser única e ter um vinculo com seu sistema, é passaro nro do RPS no campo identificador
        nfseTag.AddChild(AdicionarTag(TipoCampo.Str, "", "identificador", 1, 80, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));

        var notaTag = new XElement("nf");
        notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_total", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorLiquidoNfse)));
        notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_desconto", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.DescontoCondicionado)));
        notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_ir", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorIr)));
        notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_inss", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorInss)));
        notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_contribuicao_social", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.AliquotaCsll)));
        notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_rps", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.OutrasRetencoes)));
        notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_pis", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorPis)));
        notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_cofins", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorCofins)));
        notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "observacao", 1, 2000, Ocorrencia.Obrigatoria, ""));

        nfseTag.Add(notaTag);

        //PRESTADOR
        var prestadorTag = new XElement("prestador");
        prestadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "cpfcnpj", 1, 14, Ocorrencia.Obrigatoria, nota.Prestador.CpfCnpj.OnlyNumbers()));
        prestadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "cidade", 1, 14, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.CodigoMunicipio));

        nfseTag.Add(prestadorTag);

        //TOMADOR
        var tomadorTag = new XElement("tomador");
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tipo", 1, 14, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.CodigoPais != nota.Prestador.Endereco.CodigoPais ? "E" : nota.Tomador.CpfCnpj.OnlyNumbers().Length == 14 ? "J" : "F"));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "cpfcnpj", 1, 14, Ocorrencia.Obrigatoria, nota.Tomador.CpfCnpj.OnlyNumbers()));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "ie", 1, 20, Ocorrencia.Obrigatoria, nota.Tomador.InscricaoEstadual.OnlyNumbers()));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "nome_razao_social", 1, 115, Ocorrencia.Obrigatoria, nota.Tomador.RazaoSocial));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "sobrenome_nome_fantasia", 1, 115, Ocorrencia.Obrigatoria, nota.Tomador.NomeFantasia));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "logradouro", 1, 125, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Logradouro));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "email", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.Email));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "numero_residencia", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Numero));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "complemento", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Complemento));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "ponto_referencia", 1, 120, Ocorrencia.Obrigatoria, ""));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "bairro", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Bairro));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "cidade", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.CodigoMunicipio));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "cep", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Cep));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "ddd_fone_comercial", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.DDD));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "fone_comercial", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.Telefone));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "ddd_fone_residencial", 1, 120, Ocorrencia.Obrigatoria, ""));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "fone_residencial", 1, 120, Ocorrencia.Obrigatoria, ""));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "ddd_fax", 1, 120, Ocorrencia.Obrigatoria, ""));
        tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "fone_fax", 1, 120, Ocorrencia.Obrigatoria, ""));

        nfseTag.Add(tomadorTag);

        //SERVICO
        var itensTag = new XElement("itens");
        var listaTag = new XElement("lista");
        listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "codigo_local_prestacao_servico", 1, 7, Ocorrencia.Obrigatoria, nota.Servico.MunicipioIncidencia));
        listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "codigo_item_lista_servico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico.OnlyNumbers()));
        listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "descritivo", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
        listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "aliquota_item_lista_servico", 1, 6, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.Aliquota)));

        if (nota.Servico.Valores.ValorDeducoes <= 0)
        {
            listaTag.AddChild(nota.Servico.Valores.IssRetido == SituacaoTributaria.Normal ?
                AdicionarTag(TipoCampo.Str, "", "situacao_tributaria", 1, 2, Ocorrencia.Obrigatoria, nota.Tomador.Tipo == TipoTomador.OrgaoPublicoMunicipal ? "01" : "00") :
                AdicionarTag(TipoCampo.Str, "", "situacao_tributaria", 1, 2, Ocorrencia.Obrigatoria, "02"));
        }
        else
        {
            listaTag.AddChild(nota.Servico.Valores.IssRetido == SituacaoTributaria.Normal ?
                AdicionarTag(TipoCampo.Str, "", "situacao_tributaria", 1, 2, Ocorrencia.Obrigatoria, nota.Tomador.Tipo == TipoTomador.OrgaoPublicoMunicipal ? "04" : "03") :
                AdicionarTag(TipoCampo.Str, "", "situacao_tributaria", 1, 2, Ocorrencia.Obrigatoria, "05"));
        }

        listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_tributavel", 1, 15, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.BaseCalculo)));
        listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_deducao", 1, 15, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorDeducoes)));
        listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_issrf", 1, 15, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.IssRetido != SituacaoTributaria.Normal ? nota.Servico.Valores.ValorIss : 0)));
        listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tributa_municipio_prestador", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.MunicipioIncidencia == nota.Prestador.Endereco.CodigoMunicipio ? "S" : "N"));
        listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "unidade_codigo", 1, 15, Ocorrencia.Obrigatoria, ""));
        listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "unidade_quantidade", 1, 15, Ocorrencia.Obrigatoria, ""));
        listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "unidade_valor_unitario", 1, 15, Ocorrencia.Obrigatoria, ""));

        itensTag.Add(listaTag);
        nfseTag.Add(itensTag);

        return xmldoc.Root.AsString(identado, showDeclaration, Encoding.UTF8);
    }

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (Municipio.CodigoSiafi == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Faltou informar o codigo Siafi(codigo tom) no cadastro de cidades" });

        if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
        if (notas.Count > 1) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Este provedor aceita apenas uma RPS por vez" });
        if (retornoWebservice.Erros.Count > 0) return;

        var nota = notas[0];
        var xmlRps = WriteXmlRps(nota);

        GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);

        retornoWebservice.XmlEnvio = xmlRps;
    }

    protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        var message = new StringBuilder();
        message.Append("<nfse>");
        message.Append("<pesquisa>");
        message.Append($"<codigo_autenticidade>{retornoWebservice.Protocolo}</codigo_autenticidade>");
        message.Append("</pesquisa>");
        message.Append("</nfse>");
        retornoWebservice.XmlEnvio = message.ToString();
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException();
    }

    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        var message = new StringBuilder();

        message.Append("<nfse>");
        message.Append("<nf>");
        message.Append($"<numero>{retornoWebservice.NumeroNFSe}</numero>");
        message.Append($"<serie_nfse>{retornoWebservice.SerieNFSe}</serie_nfse>");
        message.Append($"<situacao>C</situacao>");
        message.Append($"<observacao>{retornoWebservice.Motivo}</observacao>");
        message.Append("</nf>");
        message.Append("<prestador>");
        message.Append($"<cpfcnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</cpfcnpj>");
        message.Append($"<cidade>{Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio.ZeroFill(9)}</cidade>");
        message.Append("</prestador>");
        message.Append("</nfse>");

        retornoWebservice.XmlEnvio = message.ToString();
    }

    protected override bool PrecisaValidarSchema(TipoUrl tipo)
    {
        return false;
    }

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        try
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            retornoWebservice.Data = DateTime.Parse(xmlRet.Root?.ElementAnyNs("nfse")?.ElementAnyNs("nf")?.ElementAnyNs("data_nfse")?.GetValue<string>());
            retornoWebservice.Protocolo = xmlRet.Root?.ElementAnyNs("nfse")?.ElementAnyNs("rps")?.ElementAnyNs("nro_recibo_provisorio")?.GetValue<string>();

            if (string.IsNullOrEmpty(retornoWebservice.Protocolo))
                retornoWebservice.Protocolo = xmlRet.Root?.ElementAnyNs("nfse")?.ElementAnyNs("identificador")?.GetValue<string>();

            retornoWebservice.Sucesso = !retornoWebservice.Protocolo.IsEmpty();

            if (!retornoWebservice.Sucesso)
            {
                var mensagens = xmlRet.Root.ElementAnyNs("mensagem");

                foreach (var item in mensagens.Elements())
                {
                    var erro = item.GetValue<string>();

                    var codigo = erro.Substring(0, 5);
                    var mensagemErro = erro;

                    retornoWebservice.Erros.Add(new Evento
                    {
                        Codigo = codigo,
                        Correcao = null,
                        Descricao = mensagemErro
                    });
                }

                return;
            }

            var numeroNFSe = xmlRet.Root?.ElementAnyNs("nfse")?.ElementAnyNs("nf")?.ElementAnyNs("numero_nfse")?.GetValue<string>() ?? string.Empty;
            var dataNFSe = DateTime.Parse(xmlRet.Root?.ElementAnyNs("nfse")?.ElementAnyNs("nf")?.ElementAnyNs("data_nfse")?.GetValue<string>() + " " + xmlRet.Root?.ElementAnyNs("nfse")?.ElementAnyNs("nf")?.ElementAnyNs("hora_nfse")?.GetValue<string>());
            var chaveNFSe = xmlRet.Root?.ElementAnyNs("nfse")?.ElementAnyNs("nf")?.ElementAnyNs("cod_verificador_autenticidade")?.GetValue<string>() ?? string.Empty;
            var numeroRps = xmlRet.Root?.ElementAnyNs("nfse")?.ElementAnyNs("rps")?.ElementAnyNs("nro_recibo_provisorio")?.GetValue<string>();

            if (string.IsNullOrEmpty(numeroRps))
                numeroRps = xmlRet.Root?.ElementAnyNs("nfse")?.ElementAnyNs("identificador")?.GetValue<string>();

            GravarNFSeEmDisco(xmlRet.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota == null)
            {
                notas.Load(retornoWebservice.XmlRetorno);
            }
            else
            {
                nota.IdentificacaoNFSe.Chave = chaveNFSe;
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.XmlOriginal = retornoWebservice.XmlRetorno;
            }
        }
        catch
        {

        }
    }

    protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        
        var numeroNFSe = xmlRet.Root?.ElementAnyNs("nf")?.ElementAnyNs("numero_nfse")?.GetValue<string>() ?? string.Empty;
        var dataNFSe = DateTime.Parse(xmlRet.Root?.ElementAnyNs("nf")?.ElementAnyNs("data_nfse")?.GetValue<string>() + " " + xmlRet.Root?.ElementAnyNs("nf")?.ElementAnyNs("hora_nfse")?.GetValue<string>());
        var chaveNFSe = xmlRet.Root?.ElementAnyNs("nf")?.ElementAnyNs("cod_verificador_autenticidade")?.GetValue<string>() ?? string.Empty;
        var numeroRps = xmlRet.Root?.ElementAnyNs("rps")?.ElementAnyNs("nro_recibo_provisorio")?.GetValue<string>();

        if (string.IsNullOrEmpty(numeroRps))
            numeroRps = xmlRet.Root?.ElementAnyNs("identificador")?.GetValue<string>();

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

        retornoWebservice.Sucesso = true;
    }

    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

        var mensagens = xmlRet.Root.ElementAnyNs("mensagem");

        if (mensagens != null)
        {
            foreach (var item in mensagens.Elements())
            {
                var erro = item.GetValue<string>();

                var codigo = erro.Substring(0, 5);

                if (int.Parse(codigo) == 1)
                    continue;

                var mensagemErro = erro;

                retornoWebservice.Erros.Add(new Evento
                {
                    Codigo = codigo,
                    Correcao = null,
                    Descricao = mensagemErro
                });
            }

            if (retornoWebservice.Erros.Any())
                return;
        }

        retornoWebservice.Data = DateTime.Now;
        retornoWebservice.Sucesso = true;

        var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == retornoWebservice.NumeroNFSe);
        if (nota == null) return;

        nota.Situacao = SituacaoNFSeRps.Cancelado;
        nota.Cancelamento.MotivoCancelamento = retornoWebservice.Motivo;
        nota.Cancelamento.DataHora = retornoWebservice.Data;
    }

    public override NotaServico LoadXml(XDocument xml)
    {
        Guard.Against<XmlException>(xml == null, "Xml invalido.");
        XElement notaXml = xml.ElementAnyNs("nfse");
        Guard.Against<XmlException>(notaXml == null, "Xml de RPS ou NFSe invalido.");

        var nota = new NotaServico(Configuracoes);

        // Nota Fiscal
        nota.IdentificacaoNFSe.Numero = notaXml.ElementAnyNs("nf").ElementAnyNs("numero_nfse")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoNFSe.Chave = notaXml.ElementAnyNs("nf").ElementAnyNs("cod_verificador_autenticidade")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoNFSe.DataEmissao = DateTime.Parse(notaXml.ElementAnyNs("nf").ElementAnyNs("data_nfse")?.GetValue<string>() + " " + notaXml.ElementAnyNs("nf").ElementAnyNs("hora_nfse")?.GetValue<string>());

        if (notaXml.ElementAnyNs("rps") != null)
        {
            // RPS
            nota.IdentificacaoRps.Numero = notaXml.ElementAnyNs("rps")?.ElementAnyNs("nro_recibo_provisorio")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoRps.Serie = notaXml.ElementAnyNs("rps")?.ElementAnyNs("serie_recibo_provisorio")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoRps.Tipo = TipoRps.RPS;
            nota.IdentificacaoRps.DataEmissao = DateTime.Parse(notaXml.ElementAnyNs("rps")?.ElementAnyNs("data_emissao_recibo_provisorio")?.GetValue<string>() + " " + notaXml.ElementAnyNs("rps")?.ElementAnyNs("hora_emissao_recibo_provisorio")?.GetValue<string>());
        }

        if (string.IsNullOrEmpty(nota.IdentificacaoRps.Numero))
        {
            nota.IdentificacaoRps.Numero = notaXml.ElementAnyNs("identificacao")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoRps.Serie = notaXml.ElementAnyNs("nf")?.ElementAnyNs("serie_nfse")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoRps.DataEmissao = nota.IdentificacaoNFSe.DataEmissao;
        }

        // Situação do RPS
        nota.Situacao = (notaXml.ElementAnyNs("nf")?.ElementAnyNs("situacao_codigo_nfse")?.GetValue<string>() ?? string.Empty) == "2" ? SituacaoNFSeRps.Cancelado : SituacaoNFSeRps.Normal;

        // Serviços e Valores
        nota.Servico.Valores.ValorDeducoes = decimal.Parse(notaXml.ElementAnyNs("itens")?.ElementAnyNs("lista")?.ElementAnyNs("valor_deducao")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.ValorPis = decimal.Parse(notaXml.ElementAnyNs("nf")?.ElementAnyNs("valor_pis")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.ValorCofins = decimal.Parse(notaXml.ElementAnyNs("nf")?.ElementAnyNs("valor_cofins")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.ValorInss = decimal.Parse(notaXml.ElementAnyNs("nf")?.ElementAnyNs("valor_inss")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.ValorIr = decimal.Parse(notaXml.ElementAnyNs("nf")?.ElementAnyNs("valor_ir")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.ValorCsll = decimal.Parse(notaXml.ElementAnyNs("nf")?.ElementAnyNs("valor_contribuicao_social")?.GetValue<string>() ?? "0");

        var codSituacaoTributaria = notaXml.ElementAnyNs("itens")?.ElementAnyNs("lista")?.ElementAnyNs("situacao_tributaria")?.GetValue<string>();
        nota.Servico.Valores.IssRetido = codSituacaoTributaria == "2" || codSituacaoTributaria == "5" ? SituacaoTributaria.Retencao : SituacaoTributaria.Normal;

        nota.Servico.Valores.ValorIss = decimal.Parse(notaXml.ElementAnyNs("itens")?.ElementAnyNs("lista")?.ElementAnyNs("valor_issrf")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.BaseCalculo = decimal.Parse(notaXml.ElementAnyNs("itens")?.ElementAnyNs("lista")?.ElementAnyNs("valor_tributavel")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.Aliquota = decimal.Parse(notaXml.ElementAnyNs("itens")?.ElementAnyNs("lista")?.ElementAnyNs("aliquota_item_lista_servico")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.ValorLiquidoNfse = decimal.Parse(notaXml.ElementAnyNs("nf")?.ElementAnyNs("valor_total")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.ValorIssRetido = nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? nota.Servico.Valores.ValorIss : 0;
        nota.Servico.ItemListaServico = notaXml.ElementAnyNs("itens")?.ElementAnyNs("lista")?.ElementAnyNs("codigo_item_lista_servico")?.GetValue<string>() ?? string.Empty;
        nota.Servico.Discriminacao = notaXml.ElementAnyNs("itens")?.ElementAnyNs("lista")?.ElementAnyNs("descritivo")?.GetValue<string>() ?? string.Empty;

        nota.Servico.Valores.ValorServicos = nota.Servico.Valores.BaseCalculo + nota.Servico.Valores.ValorDeducoes;

        // Prestador
        nota.Prestador.CpfCnpj = notaXml.ElementAnyNs("prestador")?.ElementAnyNs("cpfcnpj")?.GetValue<string>() ?? string.Empty;

        // Tomador
        nota.Tomador.CpfCnpj = notaXml.ElementAnyNs("tomador")?.ElementAnyNs("cpfcnpj")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.RazaoSocial = notaXml.ElementAnyNs("tomador")?.ElementAnyNs("nome_razao_social")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.NomeFantasia = notaXml.ElementAnyNs("tomador")?.ElementAnyNs("sobrenome_nome_fantasia")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Logradouro = notaXml.ElementAnyNs("tomador")?.ElementAnyNs("logradouro")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Numero = notaXml.ElementAnyNs("tomador")?.ElementAnyNs("numero_residencia")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Complemento = notaXml.ElementAnyNs("tomador")?.ElementAnyNs("complemento")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Bairro = notaXml.ElementAnyNs("tomador")?.ElementAnyNs("bairro")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Cep = notaXml.ElementAnyNs("tomador")?.ElementAnyNs("cep")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Municipio = notaXml.ElementAnyNs("tomador")?.ElementAnyNs("cidade")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Uf = notaXml.ElementAnyNs("tomador")?.ElementAnyNs("estado")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Pais = notaXml.ElementAnyNs("tomador")?.ElementAnyNs("pais")?.GetValue<string>() ?? string.Empty;

        return nota;
    }

    protected override string GerarCabecalho()
    {
        return string.Empty;
    }

    protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
    {
        return;
    }

    protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        return;
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "nfse", "", Certificado);
    }

    #region Não implementados

    public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true) => throw new NotImplementedException();

    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice) => throw new NotImplementedException();

    protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) => throw new NotImplementedException();

    protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice) => throw new NotImplementedException();

    protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void AssinarEnviar(RetornoEnviar retornoWebservice) => throw new NotImplementedException();

    protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice) => throw new NotImplementedException();

    protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        //NAO PRECISA ASSINAR A CONSULTA
        return;
    }

    protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) => throw new NotImplementedException();

    protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice) => throw new NotImplementedException();

    protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice) => throw new NotImplementedException();

    protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice) => throw new NotImplementedException();

    protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice) => throw new NotImplementedException();

    protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) => throw new NotImplementedException();

    protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override string GetSchema(TipoUrl tipo) => throw new NotImplementedException();

    #endregion Não implementados
}