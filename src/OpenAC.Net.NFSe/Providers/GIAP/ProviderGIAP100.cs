// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rodolfo Duarte
// Created          : 05-15-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="ProviderSaoPaulo.cs" company="OpenAC .Net">
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
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.DFe.Core.Service;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers;

public class ProviderGIAP100 : ProviderBase
{
    private int _numeroLote;

    #region Constructors

    public ProviderGIAP100(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "GIAP";
        Versao = VersaoNFSe.ve100;
    }

    #endregion Constructors

    #region Methods

    #region Xml

    public override NotaServico LoadXml(XDocument xml)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    public override string WriteXmlRps(NotaServico nota, bool identado, bool showDeclaration)
    {
        var xmlDoc = new XDocument(new XDeclaration("1.0", "ISO-8859-1", null));
        var notaFiscal = new XElement("notaFiscal");
        xmlDoc.Add(notaFiscal);

        var dadosPrtestador = new XElement("dadosPrestador");
        notaFiscal.Add(dadosPrtestador);
        dadosPrtestador.AddChild(AddTag(TipoCampo.Str, "", "dataEmissao", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao.ToString("dd/MM/yyyy")));
        dadosPrtestador.AddChild(AddTag(TipoCampo.StrNumber, "", "im", 1, 15, Ocorrencia.Obrigatoria, nota.Prestador.InscricaoMunicipal));
        dadosPrtestador.AddChild(AddTag(TipoCampo.Int, "", "numeroRps", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
        dadosPrtestador.AddChild(AddTag(TipoCampo.Int, "", "loteRps", 1, 15, Ocorrencia.Obrigatoria, nota.NumeroLote));

        var dadosServico = new XElement("dadosServico");
        notaFiscal.Add(dadosServico);
        dadosServico.AddChild(AddTag(TipoCampo.Str, "", "cep", 0, 9, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Cep.ZeroFill(8)));
        dadosServico.AddChild(AddTag(TipoCampo.Str, "", "logradouro", 0, 255, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Logradouro));
        dadosServico.AddChild(AddTag(TipoCampo.Str, "", "numero", 0, 50, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Numero));
        dadosServico.AddChild(AddTag(TipoCampo.Str, "", "complemento", 0, 100, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Complemento));
        dadosServico.AddChild(AddTag(TipoCampo.Str, "", "bairro", 0, 100, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Bairro));
        dadosServico.AddChild(AddTag(TipoCampo.Str, "", "cidade", 1, 255, Ocorrencia.Obrigatoria, nota.EnderecoPrestacao.Pais.ToLower() == "brasil" ? nota.EnderecoPrestacao.Municipio : "EXTERIOR"));
        dadosServico.AddChild(AddTag(TipoCampo.Str, "", "uf", 2, 2, Ocorrencia.Obrigatoria, nota.EnderecoPrestacao.Pais.ToLower() == "brasil" ? nota.EnderecoPrestacao.Uf: "EX"));
        dadosServico.AddChild(AddTag(TipoCampo.Str, "", "pais", 1, 200, Ocorrencia.Obrigatoria, nota.EnderecoPrestacao.Pais));

        var dadosTomador = new XElement("dadosTomador");
        notaFiscal.Add(dadosTomador);
        dadosTomador.AddChild(AddTag(TipoCampo.Str, "", "aoConsumidor", 1, 1, Ocorrencia.Obrigatoria, "N"));
        dadosTomador.AddChild(AddTag(TipoCampo.Str, "", "nomeTomador", 1, 255, Ocorrencia.Obrigatoria, nota.Tomador.RazaoSocial));
        dadosTomador.AddChild(AddTag(TipoCampo.Str, "", "tipoDoc", 1, 1, Ocorrencia.Obrigatoria, nota.Tomador.CpfCnpj.Length <= 11 ? "F" : "J"));
        dadosTomador.AddChild(AddTag(TipoCampo.Str, "", "documento", 1, 14, Ocorrencia.Obrigatoria, nota.Tomador.CpfCnpj));
        dadosTomador.AddChild(AddTag(TipoCampo.Str, "", "ie", 0, 30, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoEstadual));
        dadosTomador.AddChild(AddTag(TipoCampo.Str, "", "email", 0, 255, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
        dadosTomador.AddChild(AddTag(TipoCampo.Str, "", "cep", 1, 9, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Cep.ZeroFill(8)));
        dadosTomador.AddChild(AddTag(TipoCampo.Str, "", "logradouro", 0, 255, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Logradouro));
        dadosTomador.AddChild(AddTag(TipoCampo.Str, "", "numero", 0, 50, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Numero));
        dadosTomador.AddChild(AddTag(TipoCampo.Str, "", "complemento", 0, 100, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
        dadosTomador.AddChild(AddTag(TipoCampo.Str, "", "bairro", 0, 100, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Bairro));
        dadosTomador.AddChild(AddTag(TipoCampo.Str, "", "cidade", 1, 255, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Pais.ToLower() == "brasil" ? nota.Tomador.Endereco.Municipio : "EXTERIOR"));
        dadosTomador.AddChild(AddTag(TipoCampo.Str, "", "uf", 2, 2, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Pais.ToLower() == "brasil" ? nota.Tomador.Endereco.Uf : "EX"));
        dadosTomador.AddChild(AddTag(TipoCampo.Str, "", "pais", 1, 200, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Pais));

        var detalheServico = new XElement("detalheServico");
        notaFiscal.Add(detalheServico);
        detalheServico.AddChild(AddTag(TipoCampo.De2, "", "cofins", 1, 16, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorCofins));
        detalheServico.AddChild(AddTag(TipoCampo.De2, "", "csll", 1, 16, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorCsll));
        detalheServico.AddChild(AddTag(TipoCampo.De2, "", "inss", 1, 16, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorInss));
        detalheServico.AddChild(AddTag(TipoCampo.De2, "", "ir", 1, 16, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorIr));
        detalheServico.AddChild(AddTag(TipoCampo.De2, "", "pisPasep", 1, 16, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorPis));
        detalheServico.AddChild(AddTag(TipoCampo.De2, "", "deducaoMaterial", 1, 16, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorDeducoes));
        detalheServico.AddChild(AddTag(TipoCampo.De2, "", "descontoIncondicional", 1, 16, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.DescontoIncondicionado));
        detalheServico.AddChild(AddTag(TipoCampo.Int, "", "issRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? "1" : "0"));
        detalheServico.AddChild(AddTag(TipoCampo.Str, "", "obs", 0, 500, Ocorrencia.NaoObrigatoria, nota.InformacoesComplementares));
        
        var itemServico = new XElement("item");
        detalheServico.Add(itemServico);
        itemServico.AddChild(AddTag(TipoCampo.Int, "", "codigo", 1, 16, Ocorrencia.Obrigatoria, nota.Servico.CodigoTributacaoMunicipio.ZeroFill(4)));
        itemServico.AddChild(AddTag(TipoCampo.Str, "", "cnae", 1, 16, Ocorrencia.Obrigatoria, nota.Servico.CodigoCnae.ZeroFill(7)));
        itemServico.AddChild(AddTag(TipoCampo.Str, "", "descricao", 1, 500, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
        itemServico.AddChild(AddTag(TipoCampo.De2, "", "aliquota", 0, 16, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.Aliquota));
        itemServico.AddChild(AddTag(TipoCampo.De2, "", "valor", 1, 16, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));

        return xmlDoc.AsString(identado, showDeclaration, Encoding.UTF8);
    }

    public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    #endregion Xml

    #region Services

    protected override bool PrecisaValidarSchema(TipoUrl tipo) => false;

    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.Lote == 0)
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Lote não informado." });

        if (notas.Count == 0)
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "RPS não informado." });

        if (retornoWebservice.Erros.Count > 0)
            return;

        var xmlRPS = new StringBuilder();
        foreach (var nota in notas)
        {
            nota.NumeroLote = retornoWebservice.Lote;

            var xmlRps = WriteXmlRps(nota, false, false);
            xmlRPS.Append(xmlRps);
            GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
        }

        var loteBuilder = new StringBuilder();
        loteBuilder.Append("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>");
        loteBuilder.Append("<nfe>");
        loteBuilder.Append(xmlRPS);
        loteBuilder.Append("</nfe>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        //Este provedor não requer assinatura
    }

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet.Root, "");
        if (retornoWebservice.Erros.Any()) return; 

        retornoWebservice.Sucesso = true;

        foreach (var mensagem in xmlRet.Root.ElementsAnyNs("notaFiscal"))
        {
            var numeroRPS = mensagem?.ElementAnyNs("numeroRps")?.GetValue<string>() ?? "";
            var nota = notas.FirstOrDefault(t => t.IdentificacaoRps.Numero == numeroRPS);

            if (nota == null)
                continue;

            nota.IdentificacaoNFSe.Numero = mensagem?.ElementAnyNs("numeroNota")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoNFSe.DataEmissao = mensagem?.ElementAnyNs("dataEmissaoNF")?.GetValue<DateTime>() ?? DateTime.MinValue;
            nota.IdentificacaoNFSe.Chave = mensagem?.ElementAnyNs("codigoVerificacao")?.GetValue<string>() ?? string.Empty;
        }

        retornoWebservice.Protocolo = retornoWebservice.Lote.ToString();
    }

    protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        if (retornoWebservice.Protocolo.IsEmpty())
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Código de Verificação Inexistente." });
            return;
        }

        var loteBuilder = new StringBuilder();
        loteBuilder.Append("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>");
        loteBuilder.Append("<consulta>");
        loteBuilder.Append($"<inscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</inscricaoMunicipal>");
        loteBuilder.Append($"<codigoVerificacao>{retornoWebservice.Protocolo}</codigoVerificacao>");
        loteBuilder.Append("</consulta>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        //Este provedor não requer assinatura
    }

    protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

        retornoWebservice.Sucesso = true;

        var nota = notas.AddNew();
        nota.IdentificacaoNFSe.Numero = xmlRet.Root.ElementAnyNs("numeroNota")?.GetValue<string>();
        nota.IdentificacaoNFSe.DataEmissao = xmlRet.Root.ElementAnyNs("dataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;
        nota.IdentificacaoNFSe.Chave = xmlRet.Root.ElementAnyNs("codVerificacao")?.GetValue<string>();

        nota.Situacao = (
            xmlRet.Root
                .ElementAnyNs("notaExiste")?
                .GetValue<string>() ?? "Nao") == "Nao"
            ? SituacaoNFSeRps.Cancelado
            : SituacaoNFSeRps.Normal;
    }

    protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        if (retornoWebservice.NumeroNFSe.IsEmpty())
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Número da NFSe não informado para cancelamento." });
            return;
        }

        var loteBuilder = new StringBuilder();
        loteBuilder.Append("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>");
        loteBuilder.Append("<nfe>");
        loteBuilder.Append("<cancelaNota>");
        loteBuilder.Append($"<codigoMotivo>{retornoWebservice.CodigoCancelamento}</codigoMotivo>");
        loteBuilder.Append($"<numeroNota>{retornoWebservice.NumeroNFSe}</numeroNota>");
        loteBuilder.Append("</cancelaNota>");
        loteBuilder.Append("</nfe>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        //Este provedor não requer assinatura
    }

    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet.Root, "");
        if (retornoWebservice.Erros.Any()) return;

        foreach(var nota in notas)
            nota.Situacao = SituacaoNFSeRps.Cancelado;
    }

    protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    #endregion Services

    #region Private Methods

    protected override string GetSchema(TipoUrl tipo)
    {
        return tipo switch
        {
            TipoUrl.Enviar => "PedidoEnvioLoteRPS_v01.xsd",
            TipoUrl.EnviarSincrono => "PedidoEnvioRPS_v01.xsd",
            TipoUrl.ConsultarSituacao => "PedidoInformacoesLote_v01.xsd",
            TipoUrl.ConsultarLoteRps => "PedidoConsultaLote_v01.xsd",
            TipoUrl.ConsultarSequencialRps => "",
            TipoUrl.ConsultarNFSeRps => "PedidoConsultaNFe_v01.xsd",
            TipoUrl.ConsultarNFSe => "PedidoConsultaNFe_v01.xsd",
            TipoUrl.CancelarNFSe => "PedidoCancelamentoNFe_v01.xsd",
            TipoUrl.CancelarNFSeLote => "PedidoCancelamentoLote_v01.xsd",
            TipoUrl.SubstituirNFSe => "",
            _ => throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null),
        };
    }

    protected override IServiceClient GetClient(TipoUrl tipo) => new GIAPServiceClient100(this, tipo);

    protected override string GerarCabecalho() => "";

    private static void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string xmlTag)
    {
        if (!xmlRet?.ElementsAnyNs("notaFiscal").Any() ?? false)
        {
            var evento = new EventoRetorno
            {
                Codigo = xmlRet?.ElementAnyNs("statusEmissao")?.GetValue<string>() ?? string.Empty,
                Descricao = xmlRet?.ElementAnyNs("messages")?.GetValue<string>() ?? string.Empty,
            };

            retornoWs.Erros.Add(evento);

            return;
        }

        foreach (var mensagem in xmlRet.ElementsAnyNs("notaFiscal"))
        {
            var statusEmissao = mensagem?.ElementAnyNs("statusEmissao")?.GetValue<int>();

            if (statusEmissao == 400)
            {
                var evento = new EventoRetorno
                {
                    Codigo = mensagem?.ElementAnyNs("statusEmissao")?.GetValue<string>() ?? string.Empty,
                    Descricao = mensagem?.ElementAnyNs("messages")?.GetValue<string>() ?? string.Empty,
                };

                var chave = xmlRet?.ElementAnyNs("numeroRps");
                if (chave != null)
                {
                    evento.IdentificacaoRps.Numero = chave.ElementAnyNs("numeroRps")?.GetValue<string>() ?? string.Empty;
                }

                retornoWs.Erros.Add(evento);
            }
        }
    }

    #endregion Private Methods

    #endregion Methods
}