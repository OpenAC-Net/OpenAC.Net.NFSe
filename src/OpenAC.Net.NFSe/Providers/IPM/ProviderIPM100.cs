// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 03-29-2023
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 03-29-2023
//
// ***********************************************************************
// <copyright file="ProviderIPM2.cs" company="OpenAC .Net">
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

using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Common;

namespace OpenAC.Net.NFSe.Providers;

internal class ProviderIPM100 : ProviderBase
{
    #region Constructors

    public ProviderIPM100(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "IPM";
        Versao = VersaoNFSe.ve100;
        NaoGerarGrupoRps = municipio.Parametros.ContainsKey(nameof(NaoGerarGrupoRps));
        UsarVirgulaDecimais = true;
    }

    #endregion Constructors

    #region Properties

    protected bool GerarId { get; set; } = false;

    protected bool NaoGerarGrupoRps { get; set; }

    #endregion Properties

    #region Methods

    public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        return WriteXmlRps(nota, identado, showDeclaration);
    }

    public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        var xmlDoc = new XDocument(new XDeclaration("1.0", "ISO-8859-1", null));
        xmlDoc.Add(WriteRps(nota));
        return xmlDoc.AsString(identado, showDeclaration, encode: OpenEncoding.ISO88591);
    }

    private XElement WriteRps(NotaServico nota)
    {
        var nfse = GerarId
            ? new XElement("nfse", new XAttribute("id", $"rps:{nota.IdentificacaoRps.Numero}"))
            : new XElement("nfse");

        if (Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Homologacao)
        {
            if (!NaoGerarGrupoRps)
            {
                nfse.AddChild(AdicionarTag(TipoCampo.Str, "#2", "identificador", 1, 80, Ocorrencia.Obrigatoria,
                    $"nfseh_{nota.IdentificacaoRps.Numero}.{nota.IdentificacaoRps.Serie}"));
            }

            nfse.AddChild(AdicionarTag(TipoCampo.Str, "#3", "nfse_teste", 1, 1, Ocorrencia.Obrigatoria, "1"));
        }
        else
        {
            if (!NaoGerarGrupoRps)
            {
                nfse.AddChild(AdicionarTag(TipoCampo.Str, "#2", "identificador", 1, 80, Ocorrencia.Obrigatoria,
                    $"nfse_{nota.IdentificacaoRps.Numero}.{nota.IdentificacaoRps.Serie}"));
            }
        }

        var identificacao = GerarIdentificacaoRps(nota);
        if (identificacao != null)
            nfse.AddChild(identificacao);
        nfse.AddChild(GerarValoresServico(nota));
        nfse.AddChild(GerarPrestador(nota));
        nfse.AddChild(GerarTomador(nota));
        nfse.AddChild(GerarItens(nota));

        // Removido a condição da versão para gerar o grupo Forma de Pagamento para
        // a cidade de Panambi/RS
        if (nota.Situacao == SituacaoNFSeRps.Normal)
            nfse.AddChild(GerarFormaPagamento(nota));

        return nfse;
    }

    private XElement? GerarIdentificacaoRps(NotaServico nota)
    {
        if (nota.IdentificacaoRps.Numero.ToInt32() == 0 || NaoGerarGrupoRps)
            return null;

        var rps = new XElement("rps");
        rps.AddChild(AdicionarTag(TipoCampo.Str, "#1", "nro_recibo_provisorio", 1, 12, Ocorrencia.Obrigatoria,
            nota.IdentificacaoRps.Numero));

        rps.AddChild(AdicionarTag(TipoCampo.Str, "#1", "serie_recibo_provisorio", 1, 2, Ocorrencia.Obrigatoria,
            nota.IdentificacaoRps.Serie));

        rps.AddChild(AdicionarTag(TipoCampo.Str, "#1", "data_emissao_recibo_provisorio", 1, 10, Ocorrencia.Obrigatoria,
            FormataData(nota.IdentificacaoRps.DataEmissao)));

        rps.AddChild(AdicionarTag(TipoCampo.Str, "#1", "hora_emissao_recibo_provisorio", 1, 8, Ocorrencia.Obrigatoria,
            FormataHora(nota.IdentificacaoRps.DataEmissao)));

        return rps;
    }

    private XElement GerarValoresServico(NotaServico nota)
    {
        var valor = new XElement("nf");
        if (nota.Situacao == SituacaoNFSeRps.Cancelado)
        {
            valor.AddChild(AdicionarTag(TipoCampo.Str, "#1", "numero", 0, 9, Ocorrencia.Obrigatoria,
                nota.IdentificacaoNFSe.Numero));

            valor.AddChild(AdicionarTag(TipoCampo.Str, "#1", "situacao", 1, 1, Ocorrencia.Obrigatoria, "C"));
        }

        valor.AddChild(AdicionarTag(TipoCampo.Str, "#1", "data_fato_gerador", 1, 10, Ocorrencia.NaoObrigatoria,
            FormataData(nota.Competencia)));

        valor.AddChild(AdicionarTag(TipoCampo.De2, "#1", "valor_total", 1, 15, Ocorrencia.Obrigatoria,
            nota.Servico.Valores.ValorServicos));

        valor.AddChild(AdicionarTag(TipoCampo.De2, "#1", "valor_desconto", 1, 15, Ocorrencia.NaoObrigatoria,
            nota.Servico.Valores.DescontoIncondicionado));

        valor.AddChild(AdicionarTag(TipoCampo.De2, "#1", "valor_ir", 1, 15, Ocorrencia.Obrigatoria,
            nota.Servico.Valores.ValorIr));

        valor.AddChild(AdicionarTag(TipoCampo.De2, "#1", "valor_inss", 1, 15, Ocorrencia.NaoObrigatoria,
            nota.Servico.Valores.ValorInss));

        valor.AddChild(AdicionarTag(TipoCampo.De2, "#1", "valor_contribuicao_social", 1, 15, 0,
            nota.Servico.Valores.ValorCsll));

        valor.AddChild(AdicionarTag(TipoCampo.De2, "#1", "valor_rps", 1, 15, Ocorrencia.NaoObrigatoria,
            nota.Servico.Valores.OutrasRetencoes));

        valor.AddChild(AdicionarTag(TipoCampo.De2, "#1", "valor_pis", 1, 15, Ocorrencia.NaoObrigatoria,
            nota.Servico.Valores.ValorPis));

        valor.AddChild(AdicionarTag(TipoCampo.De2, "#1", "valor_cofins", 1, 15, Ocorrencia.NaoObrigatoria,
            nota.Servico.Valores.ValorCofins));

        valor.AddChild(AdicionarTag(TipoCampo.Str, "#1", "observacao", 1, 1000, Ocorrencia.NaoObrigatoria,
            nota.OutrasInformacoes));

        return valor;
    }

    private XElement GerarPrestador(NotaServico nota)
    {
        var prestador = new XElement("prestador");
        prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "cpfcnpj", 1, 14, Ocorrencia.Obrigatoria,
            nota.Prestador.CpfCnpj.OnlyNumbers()));
        prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "cidade", 1, 14, Ocorrencia.Obrigatoria,
            nota.Prestador.Endereco.CodigoMunicipio));

        return prestador;
    }

    private XElement GerarTomador(NotaServico nota)
    {
        var tomador = new XElement("tomador");
        if (!nota.Tomador.DocEstrangeiro.IsEmpty())
        {
            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "tipo", 1, 14, Ocorrencia.Obrigatoria, "E"));
            tomador.AddChild(AdicionarTag(TipoCampo.Str, "#1", "identificador", 1, 20, Ocorrencia.Obrigatoria,
                nota.Tomador.DocEstrangeiro.Trim()));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "#1", "estado", 1, 100, Ocorrencia.Obrigatoria,
                nota.Tomador.Endereco.Uf));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "#1", "pais", 1, 100, Ocorrencia.Obrigatoria,
                nota.Tomador.Endereco.Pais));
        }
        else
        {
            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "tipo", 1, 14, Ocorrencia.Obrigatoria,
                nota.Tomador.CpfCnpj.IsCNPJ() ? "J" : "F"));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "cpfcnpj", 1, 14, Ocorrencia.Obrigatoria,
                nota.Tomador.CpfCnpj.OnlyNumbers()));

            if (nota.Tomador.CpfCnpj.IsCNPJ())
                tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "ie", 1, 20, Ocorrencia.Obrigatoria,
                    nota.Tomador.InscricaoEstadual.OnlyNumbers()));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "nome_razao_social", 1, 115, Ocorrencia.Obrigatoria,
                nota.Tomador.RazaoSocial));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "sobrenome_nome_fantasia", 1, 115, Ocorrencia.Obrigatoria,
                nota.Tomador.NomeFantasia));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "logradouro", 1, 125, Ocorrencia.Obrigatoria,
                nota.Tomador.Endereco.Logradouro));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "email", 1, 120, Ocorrencia.Obrigatoria,
                nota.Tomador.DadosContato.Email));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "numero_residencia", 1, 120, Ocorrencia.Obrigatoria,
                nota.Tomador.Endereco.Numero));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "complemento", 1, 120, Ocorrencia.Obrigatoria,
                nota.Tomador.Endereco.Complemento));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "ponto_referencia", 1, 120, Ocorrencia.Obrigatoria, ""));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "bairro", 1, 120, Ocorrencia.Obrigatoria,
                nota.Tomador.Endereco.Bairro));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "cidade", 1, 120, Ocorrencia.Obrigatoria,
                nota.Tomador.DocEstrangeiro.IsEmpty()
                    ? nota.Tomador.Endereco.CodigoMunicipio
                    : nota.Tomador.Endereco.Municipio));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "cep", 1, 120, Ocorrencia.Obrigatoria,
                nota.Tomador.Endereco.Cep.OnlyNumbers()));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "ddd_fone_comercial", 1, 120, Ocorrencia.Obrigatoria,
                nota.Tomador.DadosContato.DDD));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "fone_comercial", 1, 120, Ocorrencia.Obrigatoria,
                nota.Tomador.DadosContato.Telefone));

            tomador.AddChild(
                AdicionarTag(TipoCampo.Str, "", "ddd_fone_residencial", 1, 120, Ocorrencia.Obrigatoria, ""));
            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "fone_residencial", 1, 120, Ocorrencia.Obrigatoria, ""));
            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "ddd_fax", 1, 120, Ocorrencia.Obrigatoria, ""));
            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "fone_fax", 1, 120, Ocorrencia.Obrigatoria, ""));
        }

        return tomador;
    }

    private XElement GerarItens(NotaServico nota)
    {
        var itens = new XElement("itens");

        foreach (var item in nota.Servico.ItemsServico)
        {
            var lista = new XElement("lista");
            itens.AddChild(lista);

            lista.AddChild(AdicionarTag(TipoCampo.Str, "#", "tributa_municipio_prestador", 1, 1, Ocorrencia.Obrigatoria,
                item.Tributavel == NFSeSimNao.Sim ? "S" : "N"));

            lista.AddChild(AdicionarTag(TipoCampo.Str, "#", "codigo_local_prestacao_servico", 1, 9,
                Ocorrencia.Obrigatoria, item.MunicipioIncidencia));

            lista.AddChild(AdicionarTag(TipoCampo.Str, "#", "unidade_codigo", 1, 9, Ocorrencia.NaoObrigatoria, ""));
            lista.AddChild(AdicionarTag(TipoCampo.De2, "#", "unidade_quantidade", 1, 15, Ocorrencia.NaoObrigatoria,
                item.Quantidade));

            lista.AddChild(AdicionarTag(TipoCampo.De10, "#", "unidade_valor_unitario", 1, 15, Ocorrencia.NaoObrigatoria,
                item.ValorUnitario));

            lista.AddChild(AdicionarTag(TipoCampo.Str, "#", "codigo_item_lista_servico", 1, 9, Ocorrencia.Obrigatoria,
                item.ItemListaServico.OnlyNumbers()));

            lista.AddChild(AdicionarTag(TipoCampo.Str, "#", "codigo_atividade", 1, 9, Ocorrencia.Obrigatoria,
                item.Codigo.OnlyNumbers()));

            lista.AddChild(AdicionarTag(TipoCampo.Str, "#", "descritivo", 1, 1000, Ocorrencia.Obrigatoria,
                item.Descricao.IsEmpty() ? nota.Servico.Discriminacao : item.Descricao));

            if (item.Aliquota == 0)
                lista.AddChild(AdicionarTag(TipoCampo.De4, "#", "aliquota_item_lista_servico", 1, 15,
                    Ocorrencia.Obrigatoria,
                    nota.Servico.Valores.Aliquota));
            else
                lista.AddChild(AdicionarTag(TipoCampo.De4, "#", "aliquota_item_lista_servico", 1, 15,
                    Ocorrencia.Obrigatoria,
                    item.Aliquota));

            if (nota.Servico.Valores.ValorDeducoes <= 0)
            {
                lista.AddChild(nota.Servico.Valores.IssRetido == SituacaoTributaria.Normal
                    ? AdicionarTag(TipoCampo.Str, "", "situacao_tributaria", 1, 2, Ocorrencia.Obrigatoria,
                        nota.Tomador.Tipo == TipoTomador.OrgaoPublicoMunicipal ? "01" : "00")
                    : AdicionarTag(TipoCampo.Str, "", "situacao_tributaria", 1, 2, Ocorrencia.Obrigatoria, "02"));
            }
            else
            {
                lista.AddChild(nota.Servico.Valores.IssRetido == SituacaoTributaria.Normal
                    ? AdicionarTag(TipoCampo.Str, "", "situacao_tributaria", 1, 2, Ocorrencia.Obrigatoria,
                        nota.Tomador.Tipo == TipoTomador.OrgaoPublicoMunicipal ? "04" : "03")
                    : AdicionarTag(TipoCampo.Str, "", "situacao_tributaria", 1, 2, Ocorrencia.Obrigatoria, "05"));
            }

            lista.AddChild(AdicionarTag(TipoCampo.De2, "#", "valor_tributavel", 1, 15, Ocorrencia.NaoObrigatoria,
                item.ValorServicos));

            lista.AddChild(AdicionarTag(TipoCampo.De2, "#", "valor_deducao", 1, 15, Ocorrencia.NaoObrigatoria,
                item.ValorDeducoes));

            lista.AddChild(AdicionarTag(TipoCampo.De2, "", "valor_issrf", 1, 15, Ocorrencia.Obrigatoria,
                nota.Servico.Valores.IssRetido != SituacaoTributaria.Normal ? item.ValorIss : 0));
        }

        if (nota.Servico.ItemsServico.Count > 10)
            WAlerta("#", "lista", "", ErrMsgMaiorMaximo + "10");

        return itens;
    }

    private XElement GerarFormaPagamento(NotaServico nota)
    {
        var formaPagamento = new XElement("forma_pagamento");

        var forma = nota.Pagamento.Forma switch
        {
            FormaPagamento.AVista => "1",
            FormaPagamento.APrazo => "2",
            FormaPagamento.Deposito => "3",
            FormaPagamento.NaApresentacao => "4",
            FormaPagamento.CartaoDebito => "5",
            FormaPagamento.CartaoCredito => "6",
            FormaPagamento.Cheque => "7",
            FormaPagamento.PIX => "8",
            _ => throw new OpenException("Forma de pagamento invalida")
        };

        formaPagamento.AddChild(
            AdicionarTag(TipoCampo.Str, "#1", "tipo_pagamento", 1, 1, Ocorrencia.Obrigatoria, forma));
        if (nota.Pagamento.Parcelas.Count <= 0) return formaPagamento;

        var parcelas = new XElement("parcelas");
        formaPagamento.AddChild(parcelas);

        foreach (var item in nota.Pagamento.Parcelas)
        {
            var parcela = new XElement("parcela");
            parcelas.AddChild(parcela);

            parcela.AddChild(AdicionarTag(TipoCampo.Str, "#", "numero", 1, 2, Ocorrencia.Obrigatoria,
                item.Parcela));

            parcela.AddChild(AdicionarTag(TipoCampo.De2, "#", "valor", 1, 15, Ocorrencia.Obrigatoria,
                item.Valor));

            parcela.AddChild(AdicionarTag(TipoCampo.Str, "#", "data_vencimento", 10, 10, Ocorrencia.Obrigatoria,
                FormataData(item.DataVencimento)));
        }

        return formaPagamento;
    }

    private static string FormataData(DateTime valor) => valor.ToString("dd/MM/yyyy");

    private static string FormataHora(DateTime valor) => valor.ToString("HH:mm:ss");

    protected override IServiceClient GetClient(TipoUrl tipo) => new IPM100ServiceClient(this, tipo);

    protected override string GerarCabecalho() => "";

    protected override string GetSchema(TipoUrl tipo) => "";

    protected override bool PrecisaValidarSchema(TipoUrl tipo) => false;

    public override NotaServico LoadXml(XDocument? xml)
    {
        Guard.Against<XmlException>(xml == null, "Xml invalido.");
        var notaXml = xml?.ElementAnyNs("nfse");
        Guard.Against<XmlException>(notaXml == null, "Xml de RPS ou NFSe invalido.");

        var nota = new NotaServico(Configuracoes)
        {
            IdentificacaoNFSe =
            {
                // Nota Fiscal
                Numero = notaXml.ElementAnyNs("nf").ElementAnyNs("numero_nfse")?.GetValue<string>() ?? string.Empty,
                Chave = notaXml.ElementAnyNs("nf").ElementAnyNs("cod_verificador_autenticidade")?.GetValue<string>() ??
                        string.Empty,
                DataEmissao = DateTime.Parse(notaXml.ElementAnyNs("nf").ElementAnyNs("data_nfse")?.GetValue<string>() +
                                             " " + notaXml.ElementAnyNs("nf").ElementAnyNs("hora_nfse")
                                                 ?.GetValue<string>())
            }
        };

        if (notaXml.ElementAnyNs("rps") != null)
        {
            // RPS
            nota.IdentificacaoRps.Numero =
                notaXml.ElementAnyNs("rps")?.ElementAnyNs("nro_recibo_provisorio")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoRps.Serie =
                notaXml.ElementAnyNs("rps")?.ElementAnyNs("serie_recibo_provisorio")?.GetValue<string>() ??
                string.Empty;
            nota.IdentificacaoRps.Tipo = TipoRps.RPS;
            nota.IdentificacaoRps.DataEmissao = DateTime.Parse(
                notaXml.ElementAnyNs("rps")?.ElementAnyNs("data_emissao_recibo_provisorio")?.GetValue<string>() + " " +
                notaXml.ElementAnyNs("rps")?.ElementAnyNs("hora_emissao_recibo_provisorio")?.GetValue<string>());
        }

        if (string.IsNullOrEmpty(nota.IdentificacaoRps.Numero))
        {
            nota.IdentificacaoRps.Numero = notaXml.ElementAnyNs("identificacao")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoRps.Serie = notaXml.ElementAnyNs("nf")?.ElementAnyNs("serie_nfse")?.GetValue<string>() ??
                                          string.Empty;
            nota.IdentificacaoRps.DataEmissao = nota.IdentificacaoNFSe.DataEmissao;
        }

        // Situação do RPS
        nota.Situacao =
            (notaXml.ElementAnyNs("nf")?.ElementAnyNs("situacao_codigo_nfse")?.GetValue<string>() ?? string.Empty) ==
            "2"
                ? SituacaoNFSeRps.Cancelado
                : SituacaoNFSeRps.Normal;

        // Serviços e Valores
        nota.Servico.Valores.ValorDeducoes = decimal.Parse(notaXml.ElementAnyNs("itens")?.ElementAnyNs("lista")
            ?.ElementAnyNs("valor_deducao")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.ValorPis =
            decimal.Parse(notaXml.ElementAnyNs("nf")?.ElementAnyNs("valor_pis")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.ValorCofins =
            decimal.Parse(notaXml.ElementAnyNs("nf")?.ElementAnyNs("valor_cofins")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.ValorInss =
            decimal.Parse(notaXml.ElementAnyNs("nf")?.ElementAnyNs("valor_inss")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.ValorIr =
            decimal.Parse(notaXml.ElementAnyNs("nf")?.ElementAnyNs("valor_ir")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.ValorCsll =
            decimal.Parse(notaXml.ElementAnyNs("nf")?.ElementAnyNs("valor_contribuicao_social")?.GetValue<string>() ??
                          "0");

        var codSituacaoTributaria = notaXml.ElementAnyNs("itens")?.ElementAnyNs("lista")
            ?.ElementAnyNs("situacao_tributaria")?.GetValue<string>();
        nota.Servico.Valores.IssRetido = codSituacaoTributaria == "2" || codSituacaoTributaria == "5"
            ? SituacaoTributaria.Retencao
            : SituacaoTributaria.Normal;

        nota.Servico.Valores.ValorIss = decimal.Parse(notaXml.ElementAnyNs("itens")?.ElementAnyNs("lista")
            ?.ElementAnyNs("valor_issrf")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.BaseCalculo = decimal.Parse(notaXml.ElementAnyNs("itens")?.ElementAnyNs("lista")
            ?.ElementAnyNs("valor_tributavel")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.Aliquota = decimal.Parse(notaXml.ElementAnyNs("itens")?.ElementAnyNs("lista")
            ?.ElementAnyNs("aliquota_item_lista_servico")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.ValorLiquidoNfse =
            decimal.Parse(notaXml.ElementAnyNs("nf")?.ElementAnyNs("valor_total")?.GetValue<string>() ?? "0");
        nota.Servico.Valores.ValorIssRetido = nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao
            ? nota.Servico.Valores.ValorIss
            : 0;
        nota.Servico.ItemListaServico =
            notaXml.ElementAnyNs("itens")?.ElementAnyNs("lista")?.ElementAnyNs("codigo_item_lista_servico")
                ?.GetValue<string>() ?? string.Empty;
        nota.Servico.Discriminacao =
            notaXml.ElementAnyNs("itens")?.ElementAnyNs("lista")?.ElementAnyNs("descritivo")?.GetValue<string>() ??
            string.Empty;

        nota.Servico.Valores.ValorServicos = nota.Servico.Valores.BaseCalculo + nota.Servico.Valores.ValorDeducoes;

        // Prestador
        nota.Prestador.CpfCnpj = notaXml.ElementAnyNs("prestador")?.ElementAnyNs("cpfcnpj")?.GetValue<string>() ??
                                 string.Empty;

        // Tomador
        nota.Tomador.CpfCnpj =
            notaXml.ElementAnyNs("tomador")?.ElementAnyNs("cpfcnpj")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.RazaoSocial =
            notaXml.ElementAnyNs("tomador")?.ElementAnyNs("nome_razao_social")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.NomeFantasia =
            notaXml.ElementAnyNs("tomador")?.ElementAnyNs("sobrenome_nome_fantasia")?.GetValue<string>() ??
            string.Empty;
        nota.Tomador.Endereco.Logradouro =
            notaXml.ElementAnyNs("tomador")?.ElementAnyNs("logradouro")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Numero =
            notaXml.ElementAnyNs("tomador")?.ElementAnyNs("numero_residencia")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Complemento =
            notaXml.ElementAnyNs("tomador")?.ElementAnyNs("complemento")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Bairro =
            notaXml.ElementAnyNs("tomador")?.ElementAnyNs("bairro")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Cep =
            notaXml.ElementAnyNs("tomador")?.ElementAnyNs("cep")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Municipio = notaXml.ElementAnyNs("tomador")?.ElementAnyNs("cidade")?.GetValue<string>() ??
                                          string.Empty;
        nota.Tomador.Endereco.Uf =
            notaXml.ElementAnyNs("tomador")?.ElementAnyNs("estado")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Pais =
            notaXml.ElementAnyNs("tomador")?.ElementAnyNs("pais")?.GetValue<string>() ?? string.Empty;

        return nota;
    }

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (Municipio.CodigoSiafi == 0)
            retornoWebservice.Erros.Add(new Evento
                { Codigo = "0", Descricao = "Faltou informar o codigo Siafi(codigo tom) no cadastro de cidades" });

        switch (notas.Count)
        {
            case 0:
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
                break;

            case > 1:
                retornoWebservice.Erros.Add(new Evento
                    { Codigo = "0", Descricao = "Este provedor aceita apenas uma RPS por vez" });
                break;
        }

        if (retornoWebservice.Erros.Count > 0) return;

        var nota = notas[0];
        var xmlRps = WriteXmlRps(nota);

        GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml",
            nota.IdentificacaoRps.DataEmissao);

        retornoWebservice.XmlEnvio = xmlRps;
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "nfse", "", Certificado);
    }

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        try
        {
            if (retornoWebservice.XmlRetorno.IsValidXml())
            {
                retornoWebservice.Erros.Add(new Evento
                {
                    Codigo = "999",
                    Correcao = string.Empty,
                    Descricao = "Erro no xml de retorno inválido."
                });

                return;
            }

            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var mensagens = xmlRet.Root?.ElementAnyNs("mensagem");
            if (mensagens != null)
            {
                foreach (var item in mensagens.Elements())
                {
                    var erro = item.GetValue<string>();
                    retornoWebservice.Erros.Add(new Evento
                    {
                        Codigo = erro.Substring(0, 5),
                        Correcao = string.Empty,
                        Descricao = erro
                    });
                }
                
                return;
            }

            var nfse = xmlRet.Root?.ElementAnyNs("nfse");
            if (nfse == null)
            {
                retornoWebservice.Erros.Add(new Evento
                {
                    Codigo = "999",
                    Correcao = string.Empty,
                    Descricao = "Erro no xml de retorno, elemento nfse não encontrado."
                });

                return;
            }

            retornoWebservice.Protocolo = nfse.ElementAnyNs("rps")
                .ElementAnyNs("nro_recibo_provisorio")
                .GetValue<string>();

            if (retornoWebservice.Protocolo.IsEmpty())
                retornoWebservice.Protocolo = nfse.ElementAnyNs("identificador").GetValue<string>();

            retornoWebservice.Sucesso = !retornoWebservice.Protocolo.IsEmpty();

            retornoWebservice.Data = DateTime.Parse(xmlRet.Root?.ElementAnyNs("nfse")
                .ElementAnyNs("nf")
                .ElementAnyNs("data_nfse")
                .GetValue<string>() ?? "");

            var numeroNfSe =
                xmlRet.Root?.ElementAnyNs("nfse")
                    .ElementAnyNs("nf")
                    .ElementAnyNs("numero_nfse")
                    .GetValue<string>() ?? string.Empty;
            var dataNfSe =
                DateTime.Parse(
                    xmlRet.Root?.ElementAnyNs("nfse")?.ElementAnyNs("nf")?.ElementAnyNs("data_nfse")
                        .GetValue<string>() + " " + xmlRet.Root?.ElementAnyNs("nfse")?.ElementAnyNs("nf")
                        .ElementAnyNs("hora_nfse")?.GetValue<string>());
            var chaveNfSe =
                xmlRet.Root?.ElementAnyNs("nfse")?.ElementAnyNs("nf")?.ElementAnyNs("cod_verificador_autenticidade")
                    ?.GetValue<string>() ?? string.Empty;
            var numeroRps = xmlRet.Root?.ElementAnyNs("nfse")?.ElementAnyNs("rps")
                ?.ElementAnyNs("nro_recibo_provisorio")?.GetValue<string>();

            if (string.IsNullOrEmpty(numeroRps))
                numeroRps = xmlRet.Root?.ElementAnyNs("nfse")?.ElementAnyNs("identificador")?.GetValue<string>();

            GravarNFSeEmDisco(xmlRet.AsString(true), $"NFSe-{numeroNfSe}-{chaveNfSe}-.xml", dataNfSe);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota == null)
            {
                notas.Load(retornoWebservice.XmlRetorno);
            }
            else
            {
                nota.IdentificacaoNFSe.Chave = chaveNfSe;
                nota.IdentificacaoNFSe.Numero = numeroNfSe;
                nota.XmlOriginal = retornoWebservice.XmlRetorno;
            }
        }
        catch (Exception e)
        {
            retornoWebservice.Erros.Add(new Evento
            {
                Codigo = $"999 - {nameof(e)}",
                Correcao = "",
                Descricao = e.Message
            });
        }
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

    protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
    {
        // Ignore
    }

    protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice,
        NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

        var numeroNfSe = xmlRet.Root?.ElementAnyNs("nf")?.ElementAnyNs("numero_nfse")?.GetValue<string>() ??
                         string.Empty;
        var dataNfSe = DateTime.Parse(xmlRet.Root?.ElementAnyNs("nf")?.ElementAnyNs("data_nfse")?.GetValue<string>() +
                                      " " + xmlRet.Root?.ElementAnyNs("nf")?.ElementAnyNs("hora_nfse")
                                          ?.GetValue<string>());
        var chaveNfSe =
            xmlRet.Root?.ElementAnyNs("nf")?.ElementAnyNs("cod_verificador_autenticidade")?.GetValue<string>() ??
            string.Empty;
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
            nota.IdentificacaoNFSe.DataEmissao = dataNfSe;
            nota.IdentificacaoNFSe.Numero = numeroNfSe;
            nota.IdentificacaoNFSe.Chave = chaveNfSe;
            nota.XmlOriginal = retornoWebservice.XmlRetorno;
        }

        retornoWebservice.Sucesso = true;
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

    protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        // Ignore
    }

    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet =
            XDocument.Parse(
                retornoWebservice.XmlRetorno.Replace("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>", ""));

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

    #region Não Implementados

    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas) =>
        throw new NotImplementedException();

    protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice) =>
        throw new NotImplementedException();

    protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) =>
        throw new NotImplementedException();

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice,
        NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice) =>
        throw new NotImplementedException();

    protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice,
        NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void
        PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas) =>
        throw new NotImplementedException();

    protected override void AssinarEnviar(RetornoEnviar retornoWebservice) => throw new NotImplementedException();

    protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice) =>
        throw new NotImplementedException();

    protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice) =>
        throw new NotImplementedException();

    protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) =>
        throw new NotImplementedException();

    protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice) =>
        throw new NotImplementedException();

    protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice) =>
        throw new NotImplementedException();

    protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice) =>
        throw new NotImplementedException();

    protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas) =>
        throw new NotImplementedException();

    protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice) =>
        throw new NotImplementedException();

    protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) =>
        throw new NotImplementedException();

    protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice,
        NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice,
        NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice,
        NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice,
        NotaServicoCollection notas) => throw new NotImplementedException();

    #endregion Não Implementados

    #endregion Methods
}