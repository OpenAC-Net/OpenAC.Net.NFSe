// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Flávio Vodzinski
// Created          : 04-24-2024
//
// Last Modified By : Rafael Dias
// Last Modified On : 08-15-2024
// ***********************************************************************
// <copyright file="ProviderMegasoft.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2024 Projeto OpenAC .Net
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
using OpenAC.Net.DFe.Core.Document;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal class ProviderMegasoft : ProviderABRASF200
{
    #region Constructors

    public ProviderMegasoft(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "Megasoft";
    }

    #endregion Constructors

    #region Methods

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        switch (notas.Count)
        {
            case 0:
                retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "RPS não informado." });
                break;
            case > 3:
                retornoWebservice.Erros.Add(new EventoRetorno
                    { Codigo = "0", Descricao = "Apenas 3 RPS podem ser enviados em modo Sincrono." });
                break;
        }

        if (retornoWebservice.Erros.Count > 0) return;

        var xmlLoteRps = new StringBuilder();

        foreach (var nota in notas)
        {
            var xmlRps = WriteXmlRps(nota, false, false);
            xmlLoteRps.Append(xmlRps);
            GravarRpsEmDisco(xmlRps,
                $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml",
                nota.IdentificacaoRps.DataEmissao);
        }

        var xmlLote = new StringBuilder();
        xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        xmlLote.Append($"<GerarNfseEnvio {GetNamespace()}>");
        xmlLote.Append(xmlLoteRps);

        xmlLote.Append("</GerarNfseEnvio>");
        retornoWebservice.XmlEnvio = xmlLote.ToString();
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice,
        NotaServicoCollection notas)
    {
        var xmlConsulta = new StringBuilder();
        xmlConsulta.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        xmlConsulta.Append($"<ConsultarNfseRpsEnvio {GetNamespace()}>");
        xmlConsulta.Append("<IdentificacaoRps>");
        xmlConsulta.Append($"<Numero>{retornoWebservice.NumeroRps}</Numero>");
        xmlConsulta.Append("</IdentificacaoRps>");
        xmlConsulta.Append("<Prestador>");
        xmlConsulta.Append("<CpfCnpj>");
        switch (Configuracoes.PrestadorPadrao.CpfCnpj.Length)
        {
            case 11:
            {
                xmlConsulta.Append($"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj}</Cpf>");
                break;
            }
            case 14:
            {
                xmlConsulta.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj}</Cnpj>");
                break;
            }
        }

        xmlConsulta.Append("</CpfCnpj>");
        xmlConsulta.Append(
            $"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        xmlConsulta.Append("</Prestador>");
        xmlConsulta.Append("</ConsultarNfseRpsEnvio>");
        retornoWebservice.XmlEnvio = xmlConsulta.ToString();
    }

    protected override XElement WriteIdentificacaoRps(NotaServico nota)
    {
        var indRps = new XElement("IdentificacaoRps");

        indRps.AddChild(AddTag(TipoCampo.StrNumber, "", "Numero", 1, 15, Ocorrencia.Obrigatoria,
            nota.IdentificacaoRps.Numero));

        return indRps;
    }

    protected override XElement WriteRpsRps(NotaServico nota)
    {
        var rps = new XElement("Rps");

        rps.Add(WriteIdentificacaoRps(nota));

        rps.AddChild(AddTag(TipoCampo.DatHor, "", "DataEmissao", 10, 10, Ocorrencia.Obrigatoria,
            nota.IdentificacaoRps.DataEmissao));

        return rps;
    }

    protected override XElement WriteRps(NotaServico nota)
    {
        var rootRps = new XElement("Rps");

        var infServico = new XElement("InfDeclaracaoPrestacaoServico",
            new XAttribute("Id", $"R{nota.IdentificacaoRps.Numero.OnlyNumbers()}"));
        rootRps.Add(infServico);

        infServico.Add(WriteRpsRps(nota));

        infServico.AddChild(WriteServicosRps(nota));
        infServico.AddChild(WritePrestadorRps(nota));
        infServico.AddChild(WriteTomadorRps(nota));
        infServico.AddChild(WriteIntermediarioRps(nota));
        infServico.AddChild(WriteConstrucaoCivilRps(nota));

        return rootRps;
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps",
            "InfDeclaracaoPrestacaoServico", Certificado);
    }

    protected override XElement? WriteTomadorRps(NotaServico nota)
    {
        var tomador = new XElement("Tomador");

        if (!nota.Tomador.CpfCnpj.IsEmpty())
        {
            var ideTomador = new XElement("IdentificacaoTomador");
            tomador.Add(ideTomador);

            var cpfCnpjTomador = new XElement("CpfCnpj");
            ideTomador.Add(cpfCnpjTomador);

            cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

            ideTomador.AddChild(AddTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15,
                Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));
        }

        tomador.AddChild(AddTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria,
            nota.Tomador.RazaoSocial));

        if (!nota.Tomador.Endereco.Logradouro.IsEmpty() ||
            !nota.Tomador.Endereco.Numero.IsEmpty() ||
            !nota.Tomador.Endereco.Complemento.IsEmpty() ||
            !nota.Tomador.Endereco.Bairro.IsEmpty() ||
            nota.Tomador.Endereco.CodigoMunicipio > 0 ||
            !nota.Tomador.Endereco.Uf.IsEmpty() ||
            nota.Tomador.Endereco.CodigoPais > 0 ||
            !nota.Tomador.Endereco.Cep.IsEmpty())
        {
            var endereco = new XElement("Endereco");
            tomador.Add(endereco);

            endereco.AddChild(AddTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria,
                nota.Tomador.Endereco.Logradouro));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria,
                nota.Tomador.Endereco.Numero));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria,
                nota.Tomador.Endereco.Complemento));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria,
                nota.Tomador.Endereco.Bairro));
            endereco.AddChild(AddTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria,
                nota.Tomador.Endereco.Cep));
            endereco.AddChild(AddTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero,
                nota.Tomador.Endereco.CodigoMunicipio));
        }

        return tomador;
    }

    protected override XElement WriteServicosRps(NotaServico nota)
    {
        var servico = new XElement("Servico");

        servico.Add(WriteValoresRps(nota));

        servico.AddChild(AddTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria,
            nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoMunicipio", 1, 20, Ocorrencia.Obrigatoria,
            nota.Servico.CodigoMunicipio));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria,
            nota.Servico.CodigoTributacaoMunicipio));
        servico.AddChild(AddTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria,
            nota.Servico.Discriminacao));

        return servico;
    }

    protected override string GerarCabecalho()
    {
        var cabecalho = new StringBuilder();
        cabecalho.Append("<cabecalho versao=\"1.00\" xmlns=\"http://megasoftarrecadanet.com.br/xsd/nfse_v01.xsd\">");
        cabecalho.Append("<versaoDados>1.00</versaoDados>");
        cabecalho.Append("</cabecalho>");
        return cabecalho.ToString();
    }

    protected override void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string xmlTag)
    {
        var mensagens = xmlRet?.ElementAnyNs("ListaMensagemRetorno");
        if (mensagens == null) return;
        
        foreach (var mensagem in mensagens.ElementsAnyNs("MensagemRetorno"))
        {
            var evento = new EventoRetorno
            {
                Codigo = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty,
                Descricao = mensagem?.ElementAnyNs("Mensagem")?.GetValue<string>() ?? string.Empty,
                Correcao = mensagem?.ElementAnyNs("Correcao")?.GetValue<string>() ?? string.Empty
            };

            retornoWs.Erros.Add(evento);
        }
    }

    protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice,
        NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno)?.ElementAnyNs("ConsultarNfseRpsResposta");
        MensagemErro(retornoWebservice, xmlRet, "ConsultarNfseRpsResposta");
        if (retornoWebservice.Erros.Any()) return;

        var compNfse = xmlRet?.ElementAnyNs("CompNfse");

        if (compNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno
                { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (CompNfse)" });
            return;
        }

        var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
        var numeroNfSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
        var chaveNfSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
        var dataNfSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
        var numeroRps = nfse.ElementAnyNs("DeclaracaoPrestacaoServico")?
            .ElementAnyNs("InfDeclaracaoPrestacaoServico")?
            .ElementAnyNs("Rps")?
            .ElementAnyNs("IdentificacaoRps")?
            .ElementAnyNs("Numero").GetValue<string>() ?? string.Empty;

        GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNfSe}-{chaveNfSe}-.xml", dataNfSe);

        // Carrega a nota fiscal na coleção de Notas Fiscais
        var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);

        if (nota == null)
        {
            nota = notas.Load(compNfse.ToString());
        }
        else
        {
            nota.IdentificacaoNFSe.Numero = numeroNfSe;
            nota.IdentificacaoNFSe.Chave = chaveNfSe;
            nota.IdentificacaoNFSe.DataEmissao = dataNfSe;
            nota.XmlOriginal = compNfse.ToString();

            var nfseCancelamento = compNfse.ElementAnyNs("NfseCancelamento");

            if (nfseCancelamento != null)
            {
                nota.Situacao = SituacaoNFSeRps.Cancelado;

                var confirmacaoCancelamento = nfseCancelamento
                    .ElementAnyNs("Confirmacao");

                if (confirmacaoCancelamento != null)
                {
                    var pedido = confirmacaoCancelamento.ElementAnyNs("Pedido");

                    if (pedido != null)
                    {
                        var codigoCancelamento = pedido
                            .ElementAnyNs("InfPedidoCancelamento")
                            .ElementAnyNs("CodigoCancelamento")
                            .GetValue<string>();

                        nota.Cancelamento.Pedido.CodigoCancelamento = codigoCancelamento;

                        nota.Cancelamento.Signature = DFeSignature.Load(pedido.ElementAnyNs("Signature").ToString());
                    }
                }

                nota.Cancelamento.DataHora = confirmacaoCancelamento
                    .ElementAnyNs("DataHora")
                    .GetValue<DateTime>();
            }
        }

        retornoWebservice.Nota = nota;
        retornoWebservice.Sucesso = true;
    }

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet.Root, "GerarNfseResposta");
        if (retornoWebservice.Erros.Count != 0) return;

        var infNfse = xmlRet.ElementAnyNs("GerarNfseResposta")?.ElementAnyNs("ListaNfse")?.ElementAnyNs("CompNfse")
            ?.ElementAnyNs("Nfse")?.ElementAnyNs("InfNfse");
        var numeroNfSe = infNfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
        var chaveNfSe = infNfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
        var dataNfSe = infNfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
        var numeroRps =
            infNfse?.ElementAnyNs("DeclaracaoPrestacaoServico")?.ElementAnyNs("InfDeclaracaoPrestacaoServico")
                ?.ElementAnyNs("Rps")?.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ??
            string.Empty;

        GravarNFSeEmDisco(infNfse.AsString(true), $"NFSe-{numeroNfSe}-{chaveNfSe}-.xml", dataNfSe);

        var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
        if (nota != null)
        {
            nota.IdentificacaoNFSe.Numero = numeroNfSe;
            nota.IdentificacaoNFSe.Chave = chaveNfSe;
            nota.IdentificacaoNFSe.DataEmissao = dataNfSe;
        }

        retornoWebservice.Sucesso = true;
    }

    protected override IServiceClient GetClient(TipoUrl tipo) => new MegasoftServiceCliente(this, tipo);

    protected override string GetNamespace() => "xmlns=\"http://megasoftarrecadanet.com.br/xsd/nfse_v01.xsd\"";

    protected override string GetSchema(TipoUrl tipo) => "nfse_v01.xsd";

    protected override bool PrecisaValidarSchema(TipoUrl tipo) => false;

    #endregion
}