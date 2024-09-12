// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : angelomachado
// Created          : 01-23-2020
//
// Last Modified By : angelomachado
// Last Modified On : 01-23-2020
// ***********************************************************************
// <copyright file="ProviderEquiplano.cs" company="OpenAC .Net">
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
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderEquiplano : ProviderBase
{
    #region Constructors

    public ProviderEquiplano(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "Equiplano";
    }

    #endregion Constructors

    #region Methods

    #region RPS

    //ToDo: Fazer a leitura do xml.
    public override NotaServico LoadXml(XDocument xml)
    {
        Guard.Against<XmlException>(xml == null, "Xml invalido.");

        var rootRps = xml.ElementAnyNs("rps");

        var ret = new NotaServico(Configuracoes);
        ret.IdentificacaoRps.Numero = rootRps.ElementAnyNs("nrRps")?.GetValue<string>();
        ret.IdentificacaoRps.DataEmissao = rootRps.ElementAnyNs("dtEmissaoRps").GetValue<DateTime>();
        ret.Prestador.NumeroEmissorRps = rootRps.ElementAnyNs("nrEmissorRps")?.GetValue<string>();
        ret.Situacao = rootRps.ElementAnyNs("stRps")?.GetValue<string>() == "1" ? SituacaoNFSeRps.Normal : SituacaoNFSeRps.Cancelado;

        switch (rootRps.ElementAnyNs("tpTributacao")?.GetValue<string>())
        {
            case "1":
                {
                    ret.TipoTributacao = TipoTributacao.Tributavel;
                    break;
                }
            case "2":
                {
                    ret.TipoTributacao = TipoTributacao.ForaMun;
                    break;
                }
            case "3":
                {
                    ret.TipoTributacao = TipoTributacao.Imune;
                    break;
                }
            case "4":
                {
                    ret.TipoTributacao = TipoTributacao.Suspensa;
                    break;
                }
        }

        ret.Servico.ResponsavelRetencao = rootRps.ElementAnyNs("isIssRetido")?.GetValue<string>() == "1" ? ResponsavelRetencao.Prestador : ResponsavelRetencao.Tomador;

        var tomador = rootRps.ElementAnyNs("tomador");
        var documento = tomador?.ElementAnyNs("documento");

        switch (documento.ElementAnyNs("tpDocumento")?.GetValue<string>())
        {
            case "1":
            case "2":
                {
                    ret.Tomador.CpfCnpj = documento.ElementAnyNs("nrDocumento")?.GetValue<string>();
                    break;
                }
            case "3":
                {
                    ret.Tomador.DocEstrangeiro = documento.ElementAnyNs("nrDocumento")?.GetValue<string>();
                    break;
                }
        }

        ret.Tomador.RazaoSocial = tomador?.ElementAnyNs("nmTomador")?.GetValue<string>();
        ret.Tomador.Endereco.Logradouro = tomador?.ElementAnyNs("dsEndereco")?.GetValue<string>();
        ret.Tomador.Endereco.Numero = tomador?.ElementAnyNs("nrEndereco")?.GetValue<string>();
        ret.Tomador.Endereco.Complemento = tomador?.ElementAnyNs("dsComplemento")?.GetValue<string>();
        ret.Tomador.Endereco.Bairro = tomador?.ElementAnyNs("nmBairro")?.GetValue<string>();
        ret.Tomador.Endereco.CodigoMunicipio = tomador?.ElementAnyNs("nrCidadeIbge")?.GetValue<int>() ?? 0;
        ret.Tomador.Endereco.Bairro = tomador?.ElementAnyNs("nmBairro")?.GetValue<string>();
        ret.Tomador.Endereco.Uf = tomador?.ElementAnyNs("nmUf")?.GetValue<string>();
        ret.Tomador.Endereco.Pais = tomador?.ElementAnyNs("nmPais")?.GetValue<string>();
        ret.Tomador.Endereco.Cep = tomador?.ElementAnyNs("nrCep")?.GetValue<string>();

        var servico = rootRps.ElementAnyNs("listaServicos")?.ElementAnyNs("servico");

        ret.Servico.Discriminacao = servico?.ElementAnyNs("dsDiscriminacaoServico")?.GetValue<string>();
        ret.Servico.Valores.ValorIss = servico?.ElementAnyNs("vlIssServico")?.GetValue<decimal>() ?? 0;
        ret.Servico.Valores.BaseCalculo = servico?.ElementAnyNs("vlBaseCalculo")?.GetValue<decimal>() ?? 0;
        ret.Servico.Valores.Aliquota = servico?.ElementAnyNs("vlAliquota")?.GetValue<decimal>() ?? 0;
        ret.Servico.Valores.ValorServicos = servico?.ElementAnyNs("vlServico")?.GetValue<decimal>() ?? 0;
        ret.Servico.ItemListaServico = servico?.ElementAnyNs("nrServicoItem")?.GetValue<int>().ToString("00") + "." + servico?.ElementAnyNs("nrServicoSubItem")?.GetValue<int>().ToString("00");

        ret.Servico.Valores.ValorServicos = rootRps.ElementAnyNs("vlTotalRps")?.GetValue<decimal>() ?? 0;
        ret.Servico.Valores.ValorLiquidoNfse = rootRps.ElementAnyNs("vlLiquidoRps")?.GetValue<decimal>() ?? 0;

        return ret;
    }

    public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
        xmlDoc.Add(WriteRps(nota));
        return xmlDoc.AsString(identado, showDeclaration);
    }

    //ToDo: Verificar o motivo de não ter geração do xml da NFSe
    public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
        xmlDoc.Add(WriteNFSe(nota));
        return xmlDoc.AsString(identado, showDeclaration);
    }

    private XElement WriteNFSe(NotaServico nota)
    {
        var nfs = new XElement("nfs");
        //nfs.AddAttribute(new XAttribute("xmlns", "https://www.esnfs.com.br/xsd"));
        nfs.AddChild(new XElement("nrNfs", nota.IdentificacaoNFSe.Numero));
        nfs.AddChild(new XElement("cdAutenticacao", nota.IdentificacaoNFSe.Chave));
        nfs.AddChild(AddTag(TipoCampo.DatHor, "", "dtEmissaoNfs", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.DataEmissao));
        nfs.AddChild(new XElement("nrRps", nota.IdentificacaoRps.Numero));
        nfs.AddChild(new XElement("nrEmissorRps", int.Parse(nota.Prestador.NumeroEmissorRps)));
        nfs.AddChild(AddTag(TipoCampo.Dat, "", "dtEmissaoRps", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));


        var tpTributacao = string.Empty;
        switch (nota.TipoTributacao)
        {
            case TipoTributacao.Tributavel:
                tpTributacao = "Tributado no município";
                break;

            case TipoTributacao.ForaMun:
                tpTributacao = "Tributado em outro município";
                break;

            case TipoTributacao.Imune:
                tpTributacao = "Isento/Imune";
                break;

            case TipoTributacao.Isenta:
                tpTributacao = "Isento/Imune";
                break;

            case TipoTributacao.Suspensa:
                tpTributacao = "Suspenso/Decisão judicial";
                break;
        }

        nfs.AddChild(new XElement("tpTributacao", tpTributacao));
        nfs.AddChild(new XElement("isOptanteSimplesNacional", nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional ? "Optante" : "Não optante"));
        nfs.AddChild(new XElement("isIssRetido", nota.Servico.ResponsavelRetencao == ResponsavelRetencao.Prestador ? "Sim" : "Não"));
        nfs.AddChild(new XElement("isNfsCancelada", nota.Situacao == SituacaoNFSeRps.Cancelado ? "Sim" : "Não"));
        nfs.AddChild(new XElement("isNfsCartaCorrecao", "Não"));

        nfs.AddChild(WriteServicosNFSe(nota));

        nfs.AddChild(AddTag(TipoCampo.De2, "", "vlCofins", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorCofins));
        nfs.AddChild(AddTag(TipoCampo.De2, "", "vlAliquotaCofins", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaCofins));
        nfs.AddChild(AddTag(TipoCampo.De2, "", "vlCsll", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorCsll));
        nfs.AddChild(AddTag(TipoCampo.De2, "", "vlAliquotaCsll", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaCsll));
        nfs.AddChild(AddTag(TipoCampo.De2, "", "vlInss", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorInss));
        nfs.AddChild(AddTag(TipoCampo.De2, "", "vlAliquotaInss", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaInss));
        nfs.AddChild(AddTag(TipoCampo.De2, "", "vlIrpj", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorIr));
        nfs.AddChild(AddTag(TipoCampo.De2, "", "vlAliquotaIrpj", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaIR));
        nfs.AddChild(AddTag(TipoCampo.De2, "", "vlPis", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorPis));
        nfs.AddChild(AddTag(TipoCampo.De2, "", "vlAliquotaPis", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaPis));
        nfs.AddChild(AddTag(TipoCampo.De2, "", "vlBaseCalculo", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.BaseCalculo));
        nfs.AddChild(AddTag(TipoCampo.De2, "", "vlTotalNota", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
        nfs.AddChild(AddTag(TipoCampo.De2, "", "vlTotalDeducoes", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorDeducoes));
        nfs.AddChild(AddTag(TipoCampo.De2, "", "vlImposto", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValTotTributos));

        nfs.AddChild(WritePrestadorNFSe(nota));
        nfs.AddChild(WriteTomadorNFSe(nota));

        return nfs;
    }

    private XElement WritePrestadorNFSe(NotaServico nota)
    {
        var prestador = new XElement("prestadorServico");
        prestador.AddChild(new XElement("nmPrestador", nota.Prestador.RazaoSocial));
        prestador.AddChild(new XElement("nrDocumento", nota.Prestador.CpfCnpj));
        prestador.AddChild(new XElement("nrInscricaoMunicipal", nota.Prestador.InscricaoMunicipal));
        prestador.AddChild(new XElement("dsEndereco", nota.Prestador.Endereco.Logradouro));
        prestador.AddChild(new XElement("nrEndereco", nota.Prestador.Endereco.Numero));
        prestador.AddChild(new XElement("nmPais", nota.Prestador.Endereco.Pais));
        prestador.AddChild(new XElement("nmCidade", nota.Prestador.Endereco.Municipio));
        prestador.AddChild(new XElement("nmBairro", nota.Prestador.Endereco.Bairro));
        prestador.AddChild(new XElement("nmUf", nota.Prestador.Endereco.Uf));
        prestador.AddChild(new XElement("nrCep", nota.Prestador.Endereco.Cep));

        return prestador;
    }

    private XElement WriteTomadorNFSe(NotaServico nota)
    {
        var tomador = new XElement("tomadorServico");
        tomador.AddChild(new XElement("nmTomador", nota.Tomador.RazaoSocial));
        tomador.AddChild(new XElement("nrDocumento", nota.Tomador.CpfCnpj));
        tomador.AddChild(new XElement("dsEndereco", nota.Tomador.Endereco.Logradouro));
        tomador.AddChild(new XElement("nrEndereco", nota.Tomador.Endereco.Numero));
        tomador.AddChild(new XElement("nmPais", nota.Tomador.Endereco.Pais));
        tomador.AddChild(new XElement("nmCidade", nota.Tomador.Endereco.Municipio));
        tomador.AddChild(new XElement("nmBairro", nota.Tomador.Endereco.Bairro));
        tomador.AddChild(new XElement("nmUf", nota.Tomador.Endereco.Uf));
        tomador.AddChild(new XElement("cdIbge", nota.Tomador.Endereco.CodigoMunicipio));
        tomador.AddChild(new XElement("nrCep", nota.Tomador.Endereco.Cep));

        return tomador;
    }

    private XElement WriteServicosNFSe(NotaServico nota)
    {
        var listaServicos = new XElement("servicos");
        var servico = new XElement("servico");

        servico.AddChild(new XElement("nrServico", nota.Servico.ItemListaServico));
        servico.AddChild(new XElement("dsDiscriminacaoServico", nota.Servico.Discriminacao));
        servico.AddChild(AddTag(TipoCampo.De2, "", "vlAliquota", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.Aliquota));
        servico.AddChild(AddTag(TipoCampo.De2, "", "vlDeducao", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorDeducoes));
        servico.AddChild(AddTag(TipoCampo.De2, "", "vlServico", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));

        listaServicos.Add(servico);

        return listaServicos;
    }

    private XElement WriteRps(NotaServico nota)
    {
        var rps = new XElement("rps");
        rps.AddChild(new XElement("nrRps", nota.IdentificacaoRps.Numero));
        rps.AddChild(new XElement("nrEmissorRps", int.Parse(nota.Prestador.NumeroEmissorRps)));

        rps.AddChild(AddTag(TipoCampo.DatHor, "", "dtEmissaoRps", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
        var stRps = nota.Situacao == SituacaoNFSeRps.Normal ? "1" : "2";

        rps.AddChild(new XElement("stRps", stRps));

        var tpTributacao = "1";
        switch (nota.TipoTributacao)
        {
            case TipoTributacao.Tributavel:
                tpTributacao = "1";
                break;

            case TipoTributacao.ForaMun:
                tpTributacao = "2";
                break;

            case TipoTributacao.Imune:
                tpTributacao = "3";
                break;

            case TipoTributacao.Isenta:
                tpTributacao = "3";
                break;

            case TipoTributacao.Suspensa:
                tpTributacao = "4";
                break;
        }
        rps.AddChild(new XElement("tpTributacao", tpTributacao));

        var issRetido = nota.Servico.ResponsavelRetencao == ResponsavelRetencao.Prestador ? "1" : "2";
        rps.AddChild(new XElement("isIssRetido", issRetido));

        rps.AddChild(WriteTomadorRps(nota));

        rps.AddChild(WriteValoresServicos(nota));

        rps.AddChild(new XElement("vlTotalRps", nota.Servico.Valores.ValorServicos));
        rps.AddChild(new XElement("vlLiquidoRps", nota.Servico.Valores.ValorLiquidoNfse));

        if (issRetido == "1")
            rps.AddChild(WriteRetencoes(nota));

        if (!nota.DiscriminacaoImpostos.IsEmpty())
            rps.AddChild(new XElement("dsImpostos", nota.DiscriminacaoImpostos));

        return rps;
    }

    private XElement WriteValoresServicos(NotaServico nota)
    {
        var listaServicos = new XElement("listaServicos");
        if (nota.Servico.ItemsServico.Count > 0)
        {
            foreach (var servicoItem in nota.Servico.ItemsServico)
            {
                var iSerItem = 0;
                var iSerSubItem = 0;
                var iAux = int.Parse(Regex.Replace(servicoItem.ItemListaServico, "[^0-9]", "")); //Ex.: 1402, 901

                if (iAux > 999)
                {
                    iSerItem = int.Parse(iAux.ToString().Substring(0, 2)); //14
                    iSerSubItem = int.Parse(iAux.ToString().Substring(2, 2)); //2
                }
                else
                {
                    iSerItem = int.Parse(iAux.ToString().Substring(0, 1)); //9
                    iSerSubItem = int.Parse(iAux.ToString().Substring(1, 2)); //1
                }

                var servico = new XElement("servico");
                servico.AddChild(new XElement("nrServicoItem", iSerItem));
                servico.AddChild(new XElement("nrServicoSubItem", iSerSubItem));
                servico.AddChild(new XElement("vlServico", servicoItem.ValorUnitario));
                servico.AddChild(new XElement("vlAliquota", servicoItem.Aliquota));

                XElement deducao = WriteDeducoes(nota);
                if (deducao != null)
                    servico.AddChild(deducao);

                servico.AddChild(AddTag(TipoCampo.De2, "", "vlBaseCalculo", 1, 15, Ocorrencia.MaiorQueZero, servicoItem.BaseCalculo));
                servico.AddChild(AddTag(TipoCampo.De2, "", "vlIssServico", 1, 15, Ocorrencia.MaiorQueZero, servicoItem.ValorIss));
                servico.AddChild(AddTag(TipoCampo.Str, "", "dsDiscriminacaoServico", 1, 15, Ocorrencia.MaiorQueZero, servicoItem.Discriminacao));
                listaServicos.Add(servico);
            }
        }
        else
        {
            var iSerItem = 0;
            var iSerSubItem = 0;
            var iAux = int.Parse(Regex.Replace(nota.Servico.ItemListaServico, "[^0-9]", "")); //Ex.: 1402, 901

            if (iAux > 999)
            {
                iSerItem = int.Parse(iAux.ToString().Substring(0, 2)); //14
                iSerSubItem = int.Parse(iAux.ToString().Substring(2, 2)); //2
            }
            else
            {
                iSerItem = int.Parse(iAux.ToString().Substring(0, 1)); //9
                iSerSubItem = int.Parse(iAux.ToString().Substring(1, 2)); //1
            }

            var servico = new XElement("servico");
            servico.AddChild(new XElement("nrServicoItem", iSerItem));
            servico.AddChild(new XElement("nrServicoSubItem", iSerSubItem));
            servico.AddChild(new XElement("vlServico", nota.Servico.Valores.ValorServicos));
            servico.AddChild(new XElement("vlAliquota", nota.Servico.Valores.Aliquota));

            XElement deducao = WriteDeducoes(nota);
            if (deducao != null)
                servico.AddChild(deducao);

            servico.AddChild(AddTag(TipoCampo.De2, "", "vlBaseCalculo", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.BaseCalculo));
            servico.AddChild(AddTag(TipoCampo.De2, "", "vlIssServico", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
            servico.AddChild(AddTag(TipoCampo.Str, "", "dsDiscriminacaoServico", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Discriminacao));

            listaServicos.Add(servico);
        }

        return listaServicos;
    }

    private XElement WriteDeducoes(NotaServico nota)
    {
        if (nota.Servico.Valores.ValorDeducoes == 0) return null;
        var deducao = new XElement("deducao");
        deducao.AddChild(AddTag(TipoCampo.De2, "", "vlDeducao", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
        deducao.AddChild(AddTag(TipoCampo.Str, "", "dsJustificativaDeducao", 1, 115, Ocorrencia.Obrigatoria, nota.Servico.Valores.JustificativaDeducao));
        return deducao;
    }

    private XElement WriteRetencoes(NotaServico nota)
    {
        var retencoes = new XElement("retencoes");

        if (nota.Servico.Valores.ValorCofins > 0)
            retencoes.AddChild(AddTag(TipoCampo.De2, "", "vlCofins", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorCofins));

        if (nota.Servico.Valores.ValorCsll > 0)
            retencoes.AddChild(AddTag(TipoCampo.De2, "", "VlCsll", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorCsll));

        if (nota.Servico.Valores.ValorInss > 0)
            retencoes.AddChild(AddTag(TipoCampo.De2, "", "vlInss", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorInss));

        if (nota.Servico.Valores.ValorIr > 0)
            retencoes.AddChild(AddTag(TipoCampo.De2, "", "vlIrrf", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorIr));

        if (nota.Servico.Valores.ValorPis > 0)
            retencoes.AddChild(AddTag(TipoCampo.De2, "", "vlPis", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorPis));

        if (nota.Servico.Valores.ValorIssRetido > 0)
            retencoes.AddChild(AddTag(TipoCampo.De2, "", "vlIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIssRetido));

        if (nota.Servico.Valores.AliquotaCofins > 0)
            retencoes.AddChild(AddTag(TipoCampo.De2, "", "vlAliquotaCofins", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.AliquotaCofins));

        if (nota.Servico.Valores.AliquotaCsll > 0)
            retencoes.AddChild(AddTag(TipoCampo.De2, "", "vlAliquotaCsll", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.AliquotaCsll));

        if (nota.Servico.Valores.AliquotaInss > 0)
            retencoes.AddChild(AddTag(TipoCampo.De2, "", "vlAliquotaInss", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.AliquotaInss));

        if (nota.Servico.Valores.AliquotaIR > 0)
            retencoes.AddChild(AddTag(TipoCampo.De2, "", "vlAliquotaIrrf", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.AliquotaIR));

        if (nota.Servico.Valores.AliquotaPis > 0)
            retencoes.AddChild(AddTag(TipoCampo.De2, "", "vlAliquotaPis", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.AliquotaPis));

        return retencoes;
    }

    private XElement WriteTomadorRps(NotaServico nota)
    {
        string sTpDoc;
        if (!string.IsNullOrEmpty(nota.Tomador.DocEstrangeiro))
            sTpDoc = "3"; // Estrangeiro
        else if (nota.Tomador.CpfCnpj.IsCNPJ())
            sTpDoc = "2"; // CNPJ
        else
            sTpDoc = "1"; // CPF

        var tomador = new XElement("tomador");
        var documento = new XElement("documento");
        documento.AddChild(AdicionarTagCNPJCPF("", "nrDocumento", "nrDocumento", nota.Tomador.CpfCnpj));
        documento.AddChild(AddTag(TipoCampo.Int, "", "tpDocumento", 1, 1, Ocorrencia.Obrigatoria, sTpDoc));
        if (!nota.Tomador.DocEstrangeiro.IsEmpty())
            documento.AddChild(AddTag(TipoCampo.Str, "", "dsDocumentoEstrangeiro", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.DocEstrangeiro));
        tomador.Add(documento);

        tomador.AddChild(AddTag(TipoCampo.Str, "", "nmTomador", 1, 80, Ocorrencia.Obrigatoria, nota.Tomador.RazaoSocial));
        tomador.AddChild(AddTag(TipoCampo.Str, "", "dsEmail", 0, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));

        tomador.AddChild(AddTag(TipoCampo.Str, "", "nrInscricaoEstadual", 1, 20, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoEstadual));
        // tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "nrInscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

        if (!nota.Tomador.Endereco.Logradouro.IsEmpty())
            tomador.AddChild(AddTag(TipoCampo.Str, "", "dsEndereco", 0, 40, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
        if (!nota.Tomador.Endereco.Numero.IsEmpty())
            tomador.AddChild(AddTag(TipoCampo.StrNumber, "", "nrEndereco", 0, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
        if (!nota.Tomador.Endereco.Complemento.IsEmpty())
            tomador.AddChild(AddTag(TipoCampo.Str, "", "dsComplemento", 0, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
        if (!nota.Tomador.Endereco.Bairro.IsEmpty())
            tomador.AddChild(AddTag(TipoCampo.Str, "", "nmBairro", 0, 25, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
        if (nota.Tomador.Endereco.CodigoMunicipio > 0)
            tomador.AddChild(AddTag(TipoCampo.Int, "", "nrCidadeIbge", 7, 7, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoMunicipio));
        if (!nota.Tomador.Endereco.Uf.IsEmpty())
            tomador.AddChild(AddTag(TipoCampo.Str, "", "nmUf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));

        if (!nota.Tomador.DocEstrangeiro.IsEmpty())
            tomador.AddChild(AddTag(TipoCampo.Str, "", "nmCidadeEstrangeira", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Municipio));

        if (!nota.Tomador.Endereco.Pais.IsEmpty())
            tomador.AddChild(AddTag(TipoCampo.Str, "", "nmPais", 1, 100, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Pais));

        if (!nota.Tomador.Endereco.Cep.IsEmpty())
            tomador.AddChild(AddTag(TipoCampo.StrNumber, "", "nrCep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));

        if (!nota.Tomador.DadosContato.DDD.IsEmpty() || !nota.Tomador.DadosContato.Telefone.IsEmpty())
            tomador.AddChild(AddTag(TipoCampo.StrNumber, "", "nrTelefone", 1, 11, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));

        return tomador;
    }

    #endregion RPS

    #region Services

    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.Lote == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
        if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
        if (retornoWebservice.Erros.Count > 0) return;

        var xmlListaRps = new StringBuilder();
        xmlListaRps.Append("<listaRps>");

        foreach (var nota in notas)
        {
            var xmlRps = WriteXmlRps(nota, false, false);
            xmlListaRps.Append(xmlRps);
            GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
        }

        xmlListaRps.Append("</listaRps>");

        var optanteSimplesNacional = notas.First().RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional ? "1" : "2";

        retornoWebservice.XmlEnvio = new StringBuilder()
            .Append("<es:enviarLoteRpsEnvio xmlns:es=\"http://www.equiplano.com.br/esnfs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.equiplano.com.br/enfs esRecepcionarLoteRpsEnvio_01.xsd\">")
            .Append("<lote>")
            .Append($"<nrLote>{ retornoWebservice.Lote }</nrLote>")
            .Append($"<qtRps>{ notas.Count }</qtRps>")
            .Append("<nrVersaoXml>1</nrVersaoXml>")
            .Append("<prestador>")
            .Append($"<nrCnpj>{ Configuracoes.PrestadorPadrao.CpfCnpj }</nrCnpj>")
            .Append($"<nrInscricaoMunicipal>{ Configuracoes.PrestadorPadrao.InscricaoMunicipal }</nrInscricaoMunicipal>")
            .Append($"<isOptanteSimplesNacional>{ optanteSimplesNacional }</isOptanteSimplesNacional>")
            .Append($"<idEntidade>{ Municipio.IdEntidade }</idEntidade>")
            .Append("</prestador>")
            .Append(xmlListaRps)
            .Append("</lote>")
            .Append("</es:enviarLoteRpsEnvio>")
            .ToString();
    }

    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "es:enviarLoteRpsEnvio", "", Certificado);
    }

    protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno.HtmlDecode());

        var rootElement = xmlRet.ElementAnyNs("esEnviarLoteRpsResposta");
        MensagemErro(retornoWebservice, rootElement, "mensagemRetorno");
        if (retornoWebservice.Erros.Count > 0) return;

        var protocoloElement = rootElement?.ElementAnyNs("protocolo");

        retornoWebservice.Lote = protocoloElement?.ElementAnyNs("nrLote")?.GetValue<int>() ?? 0;
        retornoWebservice.Data = protocoloElement?.ElementAnyNs("dtRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;
        retornoWebservice.Protocolo = protocoloElement?.ElementAnyNs("nrProtocolo")?.GetValue<string>() ?? string.Empty;
        retornoWebservice.Sucesso = retornoWebservice.Lote > 0;

        if (!retornoWebservice.Sucesso) return;

        foreach (var nota in notas)
        {
            nota.NumeroLote = retornoWebservice.Lote;
        }
    }

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        retornoWebservice.XmlEnvio = new StringBuilder()
            .Append("<es:esConsultarSituacaoLoteRpsEnvio xmlns:es=\"http://www.equiplano.com.br/esnfs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.equiplano.com.br/enfs esConsultarLoteRpsEnvio_v01.xsd\">")
            .Append("<prestador>")
            .Append($"<cnpj>{ Configuracoes.PrestadorPadrao.CpfCnpj }</cnpj>")
            .Append($"<idEntidade>{ Municipio.IdEntidade }</idEntidade>")
            .Append("</prestador>")
            .Append($"<nrLoteRps>{ retornoWebservice.Lote }</nrLoteRps>")
            .Append("</es:esConsultarSituacaoLoteRpsEnvio>")
            .ToString();
    }

    protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "es:esConsultarSituacaoLoteRpsEnvio", "", Certificado);
    }

    protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        var rootElement = xmlRet.ElementAnyNs("esConsultarSituacaoLoteRpsResposta");
        MensagemErro(retornoWebservice, rootElement, "mensagemRetorno");

        retornoWebservice.Lote = rootElement?.ElementAnyNs("nrLoteRps")?.GetValue<int>() ?? 0;
        retornoWebservice.Situacao = rootElement?.ElementAnyNs("stLote")?.GetValue<string>() ?? "0";
        retornoWebservice.Sucesso = !retornoWebservice.Erros.Any();
    }

    protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        retornoWebservice.XmlEnvio = new StringBuilder()
            .Append("<es:esConsultarLoteRpsEnvio xmlns:es=\"http://www.equiplano.com.br/esnfs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.equiplano.com.br/enfs esConsultarLoteRpsEnvio_v01.xsd\">")
            .Append("<prestador>")
            .Append($"<cnpj>{ Configuracoes.PrestadorPadrao.CpfCnpj }</cnpj>")
            .Append($"<idEntidade>{ Municipio.IdEntidade }</idEntidade>")
            .Append("</prestador>")
            .Append($"<nrLoteRps>{ retornoWebservice.Lote }</nrLoteRps>")
            .Append("</es:esConsultarLoteRpsEnvio>")
            .ToString();
    }

    protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "es:esConsultarLoteRpsEnvio", "", Certificado);
    }

    protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        var rootElement = xmlRet.ElementAnyNs("esConsultarLoteRpsResposta");
        MensagemErro(retornoWebservice, rootElement, "mensagemRetorno");
        if (retornoWebservice.Erros.Count > 0) return;

        var elementRoot = xmlRet.ElementAnyNs("esConsultarLoteRpsResposta");

        var listaNfse = elementRoot.ElementAnyNs("listaNfse");

        if (listaNfse == null)
        {
            retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (listaNfse)" });
            return;
        }

        foreach (var nfse in listaNfse.ElementsAnyNs("nfse"))
        {
            var nota = new NotaServico(Configuracoes);
            nota.IdentificacaoNFSe.Chave = nfse?.ElementAnyNs("cdAutenticacao")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoNFSe.Numero = nfse?.ElementAnyNs("nrNfse")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoRps.Numero = nfse?.ElementAnyNs("nrRps")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoNFSe.DataEmissao = nfse.ElementAnyNs("dtEmissaoNfs")?.GetValue<DateTime>() ?? DateTime.MinValue;

            notas.Add(nota);
        }

        retornoWebservice.Sucesso = true;
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
        if (retornoWebservice.NumeroRps < 1)
        {
            retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número do RPS/NFSe não informado para a consulta." });
            return;
        }

        retornoWebservice.XmlEnvio = new StringBuilder()
            .Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>")
            .Append("<es:esConsultarNfsePorRpsEnvio xmlns:es=\"http://www.equiplano.com.br/esnfs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.equiplano.com.br/enfs esConsultarNfsePorRpsEnvio_v01.xsd\">")
            .Append("<rps>")
            .Append($"<nrRps>{retornoWebservice.NumeroRps}</nrRps>")
            .Append($"<nrEmissorRps>{int.Parse(Configuracoes.PrestadorPadrao.NumeroEmissorRps)}</nrEmissorRps>")
            .Append("</rps>")
            .Append("<prestador>")
            .Append($"<cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj}</cnpj>")
            .Append($"<idEntidade>{Municipio.IdEntidade}</idEntidade>")
            .Append("</prestador>")
            .Append("</es:esConsultarNfsePorRpsEnvio>")
            .ToString();
    }

    protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "es:esConsultarNfsePorRpsEnvio", "", Certificado);
    }

    protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

        MensagemErro(retornoWebservice, xmlRet.ElementAnyNs("esConsultarNfsePorRpsResposta"), "mensagemRetorno");
        if (retornoWebservice.Erros.Count > 0) return;

        var elementRoot = xmlRet.ElementAnyNs("esConsultarNfsePorRpsResposta");

        var nfse = elementRoot.ElementAnyNs("nfse");

        if (nfse == null)
        {
            retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (nfse)" });
            return;
        }

        var nota = new NotaServico(Configuracoes);
        nota.IdentificacaoNFSe.Chave = nfse?.ElementAnyNs("cdAutenticacao")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoNFSe.Numero = nfse?.ElementAnyNs("nrNfse")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoRps.Numero = nfse?.ElementAnyNs("nrRps")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoNFSe.DataEmissao = nfse.ElementAnyNs("dtEmissaoNfs")?.GetValue<DateTime>() ?? DateTime.MinValue;
        nota.Situacao = SituacaoNFSeRps.Normal;

        var infoCancelamento = nfse.ElementAnyNs("cancelamento");
        if (infoCancelamento != null)
        {
            nota.Cancelamento.DataHora = infoCancelamento.ElementAnyNs("dtCancelamento")?.GetValue<DateTime>() ?? DateTime.MinValue;
            nota.Cancelamento.MotivoCancelamento = infoCancelamento?.ElementAnyNs("dsCancelamento")?.GetValue<string>() ?? string.Empty;
            nota.Situacao = SituacaoNFSeRps.Cancelado;
        }

        notas.Add(nota);
        retornoWebservice.Nota = nota;
        retornoWebservice.Sucesso = true;
    }

    protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        var xml = new StringBuilder();
        xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        xml.Append("<es:esConsultarNfseEnvio xmlns:es=\"http://www.equiplano.com.br/esnfs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.equiplano.com.br/enfs esConsultarNfsePorRpsEnvio_v01.xsd\">");
        xml.Append("<prestador>");
        xml.Append($"<cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj}</cnpj>");
        xml.Append($"<idEntidade>{Municipio.IdEntidade}</idEntidade>");
        xml.Append("</prestador>");

        if (retornoWebservice.NumeroNFse > 0)
        {
            xml.Append($"<nrNfse>{ retornoWebservice.NumeroNFse }</nrNfse>");
        }
        else
        {
            if (!retornoWebservice.Inicio.HasValue || !retornoWebservice.Fim.HasValue)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Data de início ou fim não informada para a consulta." });
                return;
            }

            xml.Append("<periodoEmissao>");
            xml.Append($"<dtInicial>{retornoWebservice.Inicio:yyyy'-'MM'-'dd'T'HH':'mm':'ss}</dtInicial>");
            xml.Append($"<dtFinal>{retornoWebservice.Fim:yyyy'-'MM'-'dd'T'HH':'mm':'ss}</dtFinal>");
            xml.Append("</periodoEmissao>");
        }

        xml.Append("</es:esConsultarNfseEnvio>");

        retornoWebservice.XmlEnvio = xml.ToString(); ;
    }

    protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "es:esConsultarNfseEnvio", "", Certificado);
    }

    protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet.ElementAnyNs("esConsultarNfseResposta"), "mensagemRetorno");
        if (retornoWebservice.Erros.Count > 0) return;

        var elementRoot = xmlRet.ElementAnyNs("esConsultarNfseResposta");
        var listaNfse = elementRoot.ElementAnyNs("listaNfse");

        if (listaNfse == null)
        {
            retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (listaNfse)" });
            return;
        }

        var notasServico = new List<NotaServico>();

        foreach (var nfse in listaNfse.ElementsAnyNs("nfse"))
        {
            var nota = new NotaServico(Configuracoes);
            nota.IdentificacaoNFSe.Chave = nfse?.ElementAnyNs("cdAutenticacao")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoNFSe.Numero = nfse?.ElementAnyNs("nrNfse")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoRps.Numero = nfse?.ElementAnyNs("nrRps")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoNFSe.DataEmissao = nfse.ElementAnyNs("dtEmissaoNfs")?.GetValue<DateTime>() ?? DateTime.MinValue;
            nota.Situacao = SituacaoNFSeRps.Normal;

            var infoCancelamento = nfse.ElementAnyNs("cancelamento");
            if (infoCancelamento != null)
            {
                nota.Cancelamento.DataHora = infoCancelamento.ElementAnyNs("dtCancelamento")?.GetValue<DateTime>() ?? DateTime.MinValue;
                nota.Cancelamento.MotivoCancelamento = infoCancelamento?.ElementAnyNs("dsCancelamento")?.GetValue<string>() ?? string.Empty;
                nota.Situacao = SituacaoNFSeRps.Cancelado;
            }

            notas.Add(nota);
            notasServico.Add(nota);
        }

        retornoWebservice.Notas = notasServico.ToArray();
        retornoWebservice.Sucesso = true;
    }

    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        if (retornoWebservice.NumeroNFSe.IsEmpty())
        {
            retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe não informado para cancelamento." });
            return;
        }

        retornoWebservice.XmlEnvio = new StringBuilder()
            .Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>")
            .Append("<es:esCancelarNfseEnvio xmlns:es=\"http://www.equiplano.com.br/esnfs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.equiplano.com.br/enfs esCancelarNfseEnvio_v01.xsd\">")
            .Append("<prestador>")
            .Append($"<cnpj>{ Configuracoes.PrestadorPadrao.CpfCnpj }</cnpj>")
            .Append($"<idEntidade>{ Municipio.IdEntidade }</idEntidade>")
            .Append("</prestador>")
            .Append($"<nrNfse>{ retornoWebservice.NumeroNFSe }</nrNfse>")
            .Append($"<dsMotivoCancelamento>{ retornoWebservice.Motivo }</dsMotivoCancelamento>")
            .Append("</es:esCancelarNfseEnvio>")
            .ToString();
    }

    protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "es:esCancelarNfseEnvio", "", Certificado);
    }

    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

        var rootElement = xmlRet.ElementAnyNs("esCancelarNfseResposta");
        MensagemErro(retornoWebservice, rootElement, "mensagemRetorno");
        if (retornoWebservice.Erros.Count > 0) return;

        var sucesso = rootElement.ElementAnyNs("sucesso");
        if (sucesso == null)
        {
            retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Confirmação do cancelamento não encontrada!" });
            return;
        }

        retornoWebservice.Data = rootElement.ElementAnyNs("dtCancelamento")?.GetValue<DateTime>() ?? DateTime.MinValue;
        retornoWebservice.Sucesso = retornoWebservice.Data != DateTime.MinValue;

        // Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
        var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == retornoWebservice.NumeroNFSe);
        if (nota == null) return;

        nota.Situacao = SituacaoNFSeRps.Cancelado;
        nota.Cancelamento.Pedido.CodigoCancelamento = retornoWebservice.CodigoCancelamento;
        nota.Cancelamento.DataHora = rootElement.ElementAnyNs("dtCancelamento")?.GetValue<DateTime>() ?? DateTime.MinValue;
        nota.Cancelamento.MotivoCancelamento = retornoWebservice.Motivo;
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

    #endregion Methods

    #region Private Methods

    protected override string GerarCabecalho() => "";

    protected override string GetSchema(TipoUrl tipo)
    {
        switch (tipo)
        {
            case TipoUrl.Enviar:
                return "esRecepcionarLoteRpsEnvio_v01.xsd";

            case TipoUrl.EnviarSincrono:
                return "";

            case TipoUrl.ConsultarSituacao:
                return "esConsultarSituacaoLoteRpsEnvio_v01.xsd";

            case TipoUrl.ConsultarLoteRps:
                return "esConsultarLoteRpsEnvio_v01.xsd";

            case TipoUrl.ConsultarSequencialRps:
                return "";

            case TipoUrl.ConsultarNFSeRps:
                return "esConsultarNfsePorRpsEnvio_v01.xsd";

            case TipoUrl.ConsultarNFSe:
                return "esConsultarNfseEnvio_v01.xsd";

            case TipoUrl.CancelarNFSe:
                return "esCancelarNfseEnvio_v01.xsd";

            case TipoUrl.CancelarNFSeLote:
                return "";

            case TipoUrl.SubstituirNFSe:
                return "";

            default:
                throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null);
        }
    }

    protected override IServiceClient GetClient(TipoUrl tipo) => new EquiplanoServiceClient(this, tipo);

    private static void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string xmlTag, string elementName = "listaErros", string messageElement = "erro")
    {
        var listaErros = xmlRet?.ElementAnyNs(xmlTag)?.ElementAnyNs(elementName);
        if (listaErros == null) return;

        foreach (var erro in listaErros.ElementsAnyNs(messageElement))
        {
            var evento = new Evento
            {
                Codigo = erro?.ElementAnyNs("cdMensagem")?.GetValue<string>() ?? string.Empty,
                Descricao = erro?.ElementAnyNs("dsMensagem")?.GetValue<string>() ?? string.Empty,
            };

            retornoWs.Erros.Add(evento);
        }
    }

    #endregion Private Methods
}