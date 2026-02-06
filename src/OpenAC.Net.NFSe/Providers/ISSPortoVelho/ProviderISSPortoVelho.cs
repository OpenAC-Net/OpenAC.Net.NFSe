// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-22-2018
//
// Last Modified By : Leandro Rossi (rossism.com.br)
// Last Modified On : 14-04-2023
// ***********************************************************************
// <copyright file="ISSNetServiceClient.cs" company="OpenAC .Net">
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
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderISSPortoVelho : ProviderABRASF200
{
    #region Constructors

    public ProviderISSPortoVelho(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "ISSPortoVelho";
    }

    #endregion Constructors

    #region Methods

    #region Protected Methods

    #region RPS

    protected override XElement WriteRps(NotaServico nota)
    {
        var rootRps = new XElement("Rps");

        var infServico = new XElement(
            "InfDeclaracaoPrestacaoServico",
            new XAttribute("Id", $"RPS{nota.IdentificacaoRps.Numero}")
        );

        rootRps.Add(infServico);

        infServico.Add(WriteRpsRps(nota));

        infServico.AddChild(AddTag(TipoCampo.Dat, "", "Competencia", 0, 0, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
        infServico.AddChild(WriteServicosRps(nota));
        infServico.AddChild(WritePrestadorRps(nota));
        infServico.AddChild(WriteTomadorRps(nota));
        infServico.AddChild(WriteIntermediarioRps(nota));
        infServico.AddChild(WriteConstrucaoCivilRps(nota));
        var regimeEspecialTributacao = nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional
                ? "6"
                : ((int)nota.RegimeEspecialTributacao).ToString();

        if (nota.RegimeEspecialTributacao != RegimeEspecialTributacao.Nenhum)
        {
            infServico.AddChild(AddTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
        }
        infServico.AddChild(AddTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional ? 1 : 2));
        infServico.AddChild(AddTag(TipoCampo.Int, "","IncentivoFiscal", 1, 1, Ocorrencia.Obrigatoria, nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2));

        return rootRps;
    }

    protected override XElement WriteRpsRps(NotaServico nota)
    {
        var rps = new XElement("Rps");

        rps.Add(WriteIdentificacaoRps(nota));

        rps.AddChild(AddTag(TipoCampo.Dat, "", "DataEmissao", 0, 0, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
        rps.AddChild(AddTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Situacao + 1));

        rps.AddChild(WriteSubstituidoRps(nota));

        return rps;
    }

    protected override XElement WriteServicosRps(NotaServico nota)
    {
        var servico = new XElement("Servico");

        servico.Add(WriteValoresRps(nota));

        servico.AddChild(AddTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));
        if (nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao)
            servico.AddChild(AddTag(TipoCampo.Int, "", "ResponsavelRetencao", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.ResponsavelRetencao == ResponsavelRetencao.Prestador ? 2 : 1));

        servico.AddChild(AddTag(TipoCampo.Str, "", "ItemListaServico", 1, 10, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoCnae", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
        servico.AddChild(AddTag(TipoCampo.StrNumber, "", "CodigoServicoNacional", 1, 9, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoIndicadorOperacao));
        servico.AddChild(AddTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
        servico.AddChild(AddTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));
        servico.AddChild(AddTag(TipoCampo.Int, "", "ExigibilidadeISS", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Servico.ExigibilidadeIss is >= 1 and <= 6 ? (int)nota.Servico.ExigibilidadeIss : 1));
        servico.AddChild(AddTag(TipoCampo.Int, "", "MunicipioIncidencia", 7, 7, Ocorrencia.NaoObrigatoria, nota.Servico.MunicipioIncidencia));
        servico.AddChild(AddTag(TipoCampo.Str, "", "NumeroProcesso", 1, 30, Ocorrencia.NaoObrigatoria, nota.Servico.NumeroProcesso));

        return servico;
    }

    protected override XElement WriteValoresRps(NotaServico nota)
    {
        var valores = nota.Servico.Valores;

        valores.BaseCalculo = valores.ValorServicos;
        valores.ValorIss = Math.Round(valores.ValorServicos * valores.Aliquota / 100m, 2);
        valores.DescontoCondicionado = 0;

        return base.WriteValoresRps(nota);
    }

    protected override XElement? WriteTomadorRps(NotaServico nota)
    {
        if (nota.Tomador.CpfCnpj.IsEmpty()) return null;

        var tomador = new XElement("TomadorServico");

        var ideTomador = new XElement("IdentificacaoTomador");
        tomador.Add(ideTomador);

        var cpfCnpjTomador = new XElement("CpfCnpj");
        ideTomador.Add(cpfCnpjTomador);

        cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

        ideTomador.AddChild(AddTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

        tomador.AddChild(AddTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));

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

            endereco.AddChild(AddTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
            endereco.AddChild(AddTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoMunicipio.ToString()));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
            endereco.AddChild(AddTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
        }

        if (!nota.Tomador.DadosContato.Telefone.IsEmpty() ||
            !nota.Tomador.DadosContato.Email.IsEmpty())
        {
            var contato = new XElement("Contato");
            tomador.Add(contato);

            contato.AddChild(AddTag(TipoCampo.StrNumber, "", "Telefone", 1, 11, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));
            contato.AddChild(AddTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
        }

        return tomador;
    }

    #endregion RPS

    #region Services
    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.Lote == 0)
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Lote não informado." });

        if (notas.Count == 0)
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "RPS não informado." });

        if (retornoWebservice.Erros.Any()) return;

        if (notas.Count > 1)
        {
            retornoWebservice.Erros.Add(new EventoRetorno
            {
                Codigo = "0",
                Descricao = "O método GerarNfse aceita apenas um RPS por envio."
            });
            return;
        }

        var nota = notas.First();

        var xmlRps = WriteXmlRps(nota, false, false);
        var xml = new StringBuilder();
        xml.Append($"<GerarNfseEnvio {GetNamespace()}>");
        xml.Append(xmlRps);
        xml.Append("</GerarNfseEnvio>");

        retornoWebservice.XmlEnvio = xml.ToString();
    }
    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "InfDeclaracaoPrestacaoServico", "Id", Certificado);
    }
    protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet, "GerarNfseResposta");
        if (retornoWebservice.Erros.Any()) return;

        retornoWebservice.Sucesso = xmlRet.Root.ElementAnyNs("ListaNfse") != null;

        if (!retornoWebservice.Sucesso) return;

        retornoWebservice.Data = xmlRet.Root.ElementAnyNs("ListaNfse").ElementAnyNs("CompNfse").ElementAnyNs("Nfse").ElementAnyNs("InfNfse").ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;
        retornoWebservice.Protocolo = xmlRet.Root.ElementAnyNs("ListaNfse").ElementAnyNs("CompNfse").ElementAnyNs("Nfse").ElementAnyNs("InfNfse").ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? "";

        var listaNfse = xmlRet.Root.ElementAnyNs("ListaNfse");

        if (listaNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
            return;
        }

        foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
        {
            var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
            var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            var chaveNFSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
            var dataNFSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
            var numeroRps = nfse.ElementAnyNs("DeclaracaoPrestacaoServico")?
                                .ElementAnyNs("InfDeclaracaoPrestacaoServico")?
                                .ElementAnyNs("Rps")?
                                .ElementAnyNs("IdentificacaoRps")?
                                .ElementAnyNs("Numero").GetValue<string>() ?? string.Empty;

            GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota == null)
            {
                nota = notas.Load(compNfse.ToString());
            }
            else
            {
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.IdentificacaoNFSe.Chave = chaveNFSe;
            }

            nota.Protocolo = retornoWebservice.Protocolo;
        }
    }
    protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet, "ConsultarLoteRpsResposta");
        if (retornoWebservice.Erros.Any()) return;

        var listaNfse = xmlRet.Root?.ElementAnyNs("ListaNfse");
        if (listaNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno
            {
                Codigo = "0",
                Descricao = "ListaNfse não encontrada."
            });
            return;
        }

        foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
        {
            var nfse = compNfse.ElementAnyNs("Nfse")?.ElementAnyNs("InfNfse");
            if (nfse == null) continue;

            var numero = nfse.ElementAnyNs("Numero")?.GetValue<string>();
            var chave = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>();
            var data = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;

            GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numero}-{chave}.xml", data);

            var numeroRps =
                nfse.ElementAnyNs("DeclaracaoPrestacaoServico")?
                    .ElementAnyNs("InfDeclaracaoPrestacaoServico")?
                    .ElementAnyNs("Rps")?
                    .ElementAnyNs("IdentificacaoRps")?
                    .ElementAnyNs("Numero")?
                    .GetValue<string>();

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota != null)
            {
                nota.IdentificacaoNFSe.Numero = numero;
                nota.IdentificacaoNFSe.Chave = chave;
                nota.IdentificacaoNFSe.DataEmissao = data;
            }
        }

        retornoWebservice.Sucesso = true;
    }

    protected override void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string xmlTag)
    {
        var mensagens = xmlRet?.ElementAnyNs(xmlTag);
        mensagens = mensagens?.ElementAnyNs("ListaMensagemRetorno") ?? mensagens?.ElementAnyNs("ListaMensagemRetornoLote");
        if (mensagens == null) return;

        foreach (var mensagem in mensagens.ElementsAnyNs("MensagemRetorno"))
        {
            var codigoRetorno = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>();

            if (!string.IsNullOrEmpty(codigoRetorno) && codigoRetorno == "L000") //Emitido com Sucesso
                return;

            var evento = new EventoRetorno
            {
                Codigo = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty,
                Descricao = mensagem?.ElementAnyNs("Mensagem")?.GetValue<string>() ?? string.Empty,
                Correcao = mensagem?.ElementAnyNs("Correcao")?.GetValue<string>() ?? string.Empty
            };

            retornoWs.Erros.Add(evento);
        }
    }

    protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet, "ConsultarNfseRpsResposta");
        if (retornoWebservice.Erros.Any()) return;

        var compNfse = xmlRet.ElementAnyNs("ConsultarNfseRpsResposta")?.ElementAnyNs("CompNfse");

        if (compNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (CompNfse)" });
            return;
        }

        var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
        var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
        var chaveNFSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
        var dataNFSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
        var numeroRps = nfse.ElementAnyNs("DeclaracaoPrestacaoServico")?
            .ElementAnyNs("IdentificacaoRps")?
            .ElementAnyNs("Numero").GetValue<string>() ?? string.Empty;

        GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

        // Carrega a nota fiscal na coleção de Notas Fiscais
        var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);

        if (nota == null)
        {
            nota = notas.Load(compNfse.ToString());
        }
        else
        {
            nota.IdentificacaoNFSe.Numero = numeroNFSe;
            nota.IdentificacaoNFSe.Chave = chaveNFSe;
            nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
        }

        retornoWebservice.Nota = nota;
        retornoWebservice.Sucesso = true;
    }
    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

        MensagemErro(retornoWebservice, xmlRet, "CancelarNfseResposta");
        if (retornoWebservice.Erros.Any()) return;

        var confirmacao = xmlRet
            .ElementAnyNs("CancelarNfseResposta")
            ?.ElementAnyNs("RetCancelamento")
            ?.ElementAnyNs("NfseCancelamento")
            ?.ElementAnyNs("Confirmacao");

        if (confirmacao == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno
            {
                Codigo = "0",
                Descricao = "Confirmação do cancelamento não encontrada."
            });
            return;
        }

        retornoWebservice.Data = confirmacao.ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;

        var numero = confirmacao
            .ElementAnyNs("Pedido")
            ?.ElementAnyNs("InfPedidoCancelamento")
            ?.ElementAnyNs("IdentificacaoNfse")
            ?.ElementAnyNs("Numero")
            ?.GetValue<string>();

        retornoWebservice.NumeroNFSe = numero ?? string.Empty;
        retornoWebservice.Sucesso = retornoWebservice.Data != DateTime.MinValue;
    }
    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Porto Velho não suporta envio síncrono.");
    }

    #endregion Services

    protected override IServiceClient GetClient(TipoUrl tipo) => new ISSPortoVelhoServiceClient(this, tipo, Certificado);

    protected override string GetSchema(TipoUrl tipo) => "nfse_v2-04.xsd";

    protected override string GetNamespace() => "xmlns=\"http://www.abrasf.org.br/nfse.xsd\"";

    #endregion Protected Methods

    #endregion Methods
}