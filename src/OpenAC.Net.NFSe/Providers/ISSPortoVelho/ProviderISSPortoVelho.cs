﻿// ***********************************************************************
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
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderISSPortoVelho : ProviderABRASF203
{
    #region Constructors

    public ProviderISSPortoVelho(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "ISSPortoVelho";
    }

    #endregion Constructors

    #region Methods

    protected override void LoadPrestador(NotaServico nota, XElement rootNFSe)
    {
        // Endereco Prestador
        var prestadorServico = rootNFSe.ElementAnyNs("PrestadorServico");
        if (prestadorServico == null) return;

        nota.Prestador.RazaoSocial = prestadorServico.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.NomeFantasia = prestadorServico.ElementAnyNs("NomeFantasia")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.NomeFantasia = prestadorServico.ElementAnyNs("NomeFantasia")?.GetValue<string>() ?? string.Empty;

        // Endereco Prestador
        var enderecoPrestador = prestadorServico.ElementAnyNs("Endereco");
        if (enderecoPrestador != null)
        {
            nota.Prestador.Endereco.Logradouro = enderecoPrestador.ElementAnyNs("Endereco")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Numero = enderecoPrestador.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Complemento = enderecoPrestador.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Bairro = enderecoPrestador.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.CodigoMunicipio = enderecoPrestador.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
            nota.Prestador.Endereco.Uf = enderecoPrestador.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Cep = enderecoPrestador.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
        }

        // Contato Prestador
        var contatoPrestador = rootNFSe.ElementAnyNs("Contato");
        if (contatoPrestador != null)
        {
            nota.Prestador.DadosContato.Telefone = contatoPrestador.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.DadosContato.Email = contatoPrestador.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
        }
    }

    protected override void LoadTomador(NotaServico nota, XElement rpsRoot)
    {
        // Tomador
        var rootTomador = rpsRoot.ElementAnyNs("Tomador");
        if (rootTomador == null) return;

        var tomadorIdentificacao = rootTomador.ElementAnyNs("IdentificacaoTomador");
        if (tomadorIdentificacao != null)
        {
            nota.Tomador.CpfCnpj = tomadorIdentificacao.ElementAnyNs("CpfCnpj")?.GetCPF_CNPJ();
            nota.Tomador.InscricaoMunicipal = tomadorIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
        }

        nota.Tomador.DocEstrangeiro = rootTomador.ElementAnyNs("NifTomador")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.RazaoSocial = rootTomador.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;

        var endereco = rootTomador.ElementAnyNs("Endereco");
        if (endereco != null)
        {
            nota.Tomador.Endereco.Logradouro = endereco.ElementAnyNs("Endereco")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.Numero = endereco.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.Complemento = endereco.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.Bairro = endereco.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.CodigoMunicipio = endereco.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
            nota.Tomador.Endereco.Uf = endereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.CodigoPais = endereco.ElementAnyNs("CodigoPais")?.GetValue<int>() ?? 0;
            nota.Tomador.Endereco.Cep = endereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
        }

        var enderecoExterior = rootTomador.ElementAnyNs("EnderecoExterior");
        if (enderecoExterior != null)
        {
            nota.Tomador.EnderecoExterior.CodigoPais = enderecoExterior.ElementAnyNs("CodigoPais")?.GetValue<int>() ?? 0;
            nota.Tomador.EnderecoExterior.EnderecoCompleto = enderecoExterior.ElementAnyNs("EnderecoCompletoExterior")?.GetValue<string>() ?? string.Empty;
        }

        var rootTomadorContato = rootTomador.ElementAnyNs("Contato");
        if (rootTomadorContato == null) return;

        nota.Tomador.DadosContato.DDD = "";
        nota.Tomador.DadosContato.Telefone = rootTomadorContato.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.DadosContato.Email = rootTomadorContato.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
    }

    protected override void LoadNFSe(NotaServico nota, XElement rootNFSe)
    {
        nota.IdentificacaoNFSe.Numero = rootNFSe.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoNFSe.Chave = rootNFSe.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoNFSe.DataEmissao = rootNFSe.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;

        nota.RpsSubstituido.NumeroNfse = rootNFSe.ElementAnyNs("NfseSubstituida")?.GetValue<string>() ?? string.Empty;
        nota.OutrasInformacoes = rootNFSe.ElementAnyNs("OutrasInformacoes")?.GetValue<string>() ?? string.Empty;

        // Valores NFSe
        var valoresNFSe = rootNFSe.ElementAnyNs("ValoresNfse");
        if (valoresNFSe != null)
        {
            nota.Servico.Valores.BaseCalculo = valoresNFSe.ElementAnyNs("BaseCalculo")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorIss = valoresNFSe.ElementAnyNs("ValorIss")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorLiquidoNfse = valoresNFSe.ElementAnyNs("ValorLiquidoNfse")?.GetValue<decimal>() ?? 0;

            var aliquota = valoresNFSe.ElementAnyNs("Aliquota")?.GetValue<decimal>() ?? 0;

            // Aliquota vem bruta na nfse de portovelho
            if (aliquota != 0)
                nota.Servico.Valores.Aliquota = aliquota / 100;
        }

        nota.DescricaoCodigoTributacaoMunicipio = rootNFSe.ElementAnyNs("DescricaoCodigoTributacaoMunicípio")?.GetValue<string>() ?? string.Empty;
        nota.ValorCredito = rootNFSe.ElementAnyNs("ValorCredito")?.GetValue<decimal>() ?? 0;

        LoadPrestador(nota, rootNFSe);

        // Orgão Gerador
        var rootOrgaoGerador = rootNFSe.ElementAnyNs("OrgaoGerador");
        if (rootOrgaoGerador == null) return;

        nota.OrgaoGerador.CodigoMunicipio = rootOrgaoGerador.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
        nota.OrgaoGerador.Uf = rootOrgaoGerador.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
    }

    public override NotaServico LoadXml(XDocument xml)
    {
        Guard.Against<XmlException>(xml == null, "Xml invalido.");

        XElement rootNFSe = null;
        XElement rootCanc = null;
        XElement rootSub = null;
        XElement rootRps;

        //var rootGrupo = xml.ElementAnyNs("ConsultarLoteRpsResposta");
        //var rootGrupoListaNfse = rootGrupo?.ElementAnyNs("ListaNfse");
        var rootCompNfse = xml.ElementAnyNs("CompNfse");

        if (rootCompNfse != null)
        {
            rootNFSe = rootCompNfse.ElementAnyNs("Nfse")?.ElementAnyNs("InfNfse");
            rootCanc = rootCompNfse.ElementAnyNs("NfseCancelamento");
            rootSub = rootCompNfse.ElementAnyNs("NfseSubstituicao");
            rootRps = rootNFSe.ElementAnyNs("DeclaracaoPrestacaoServico")?.ElementAnyNs("InfDeclaracaoPrestacaoServico");
        }
        else
        {
            rootRps = xml.ElementAnyNs("Rps").ElementAnyNs("InfDeclaracaoPrestacaoServico");
        }

        Guard.Against<XmlException>(rootNFSe == null && rootRps == null, "Xml de RPS ou NFSe invalido.");

        var ret = new NotaServico(Configuracoes);

        if (rootRps != null) //Goiania não retorna o RPS, somente a NFSe
            LoadRps(ret, rootRps);

        if (rootNFSe != null)
        {
            LoadNFSe(ret, rootNFSe);
            if (rootSub != null) LoadNFSeSub(ret, rootSub);
            if (rootCanc != null) LoadNFSeCancelada(ret, rootCanc);
        }

        return ret;
    }

    protected override IServiceClient GetClient(TipoUrl tipo) => new ISSPortoVelhoServiceClient(this, tipo);

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
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));

        valores.AddChild(AddTag(TipoCampo.De2, "", "Aliquota", 1, 5, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota));
        valores.AddChild(AddTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
        valores.AddChild(AddTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));

        return valores;
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

        string regimeEspecialTributacao;
        string optanteSimplesNacional;

        if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
        {
            regimeEspecialTributacao = "1";
            optanteSimplesNacional = "1";
        }
        else
        {
            var regime = nota.RegimeEspecialTributacao;

            switch (regime)
            {
                case RegimeEspecialTributacao.MicroEmpresaMunicipal:
                    regimeEspecialTributacao = "5";
                    break;

                case RegimeEspecialTributacao.Estimativa:
                    regimeEspecialTributacao = "2";
                    break;

                case RegimeEspecialTributacao.SociedadeProfissionais:
                    regimeEspecialTributacao = "3";
                    break;

                case RegimeEspecialTributacao.Cooperativa:
                    regimeEspecialTributacao = "4";
                    break;

                case RegimeEspecialTributacao.MicroEmpresarioIndividual:
                    regimeEspecialTributacao = "7";
                    break;

                case RegimeEspecialTributacao.MicroEmpresarioEmpresaPP:
                    regimeEspecialTributacao = "6";
                    break;

                default:
                    regimeEspecialTributacao = "1";
                    break;
            }
            //regimeEspecialTributacao = ((int)nota.RegimeEspecialTributacao).ToString();
            optanteSimplesNacional = "2";
        }

        infServico.AddChild(AddTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
        infServico.AddChild(AddTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
        infServico.AddChild(AddTag(TipoCampo.Int, "", "IncentivoFiscal", 1, 1, Ocorrencia.Obrigatoria, nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2));

        return rootRps;
    }

    #endregion Methods
}