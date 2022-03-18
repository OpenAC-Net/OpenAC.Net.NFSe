// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Diego Martins
// Created          : 08-29-2021
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="ProviderCuritiba.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2021 Projeto OpenAC .Net
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
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class ProviderCuritiba : ProviderBase
    {
        #region Constructors

        public ProviderCuritiba(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Curitiba";
        }

        #endregion Constructors

        #region Methods

        #region LoadXML

        public override NotaServico LoadXml(XDocument xml)
        {
            Guard.Against<XmlException>(xml == null, "Xml invalido.");

            XElement rootDoc;
            XElement rootCanc = null;
            XElement rootSub = null;

            var isRps = false;
            var isNFSe = false;

            var rootGrupo = xml.ElementAnyNs("tcCompNfse");
            if (rootGrupo != null)
            {
                isNFSe = true;
                rootDoc = rootGrupo.ElementAnyNs("Nfse")?.ElementAnyNs("InfNfse");
                rootCanc = rootGrupo.ElementAnyNs("NfseCancelamento");
                rootSub = rootGrupo.ElementAnyNs("NfseSubstituicao");
            }
            else
            {
                rootDoc = xml.ElementAnyNs("Rps");
                if (rootDoc != null)
                {
                    isRps = true;
                    rootDoc = rootDoc.ElementAnyNs("InfRps");
                }
            }

            Guard.Against<XmlException>(rootDoc == null, "Xml de RPS ou NFSe invalido.");

            var ret = new NotaServico(Configuracoes);
            if (isNFSe)
            {
                // Nota Fiscal
                ret.IdentificacaoNFSe.Numero = rootDoc.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                ret.IdentificacaoNFSe.Chave = rootDoc.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
                ret.IdentificacaoNFSe.DataEmissao = rootDoc.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;
            }

            // RPS
            var rootRps = rootDoc.ElementAnyNs("IdentificacaoRps");
            if (rootRps != null)
            {
                ret.IdentificacaoRps.Numero = rootRps.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                ret.IdentificacaoRps.Serie = rootRps.ElementAnyNs("Serie")?.GetValue<string>() ?? string.Empty;
                ret.IdentificacaoRps.Tipo = rootRps.ElementAnyNs("Tipo")?.GetValue<TipoRps>() ?? TipoRps.RPS;
            }

            if (isNFSe)
                ret.IdentificacaoRps.DataEmissao = rootDoc.ElementAnyNs("DataEmissaoRps")?.GetValue<DateTime>() ?? DateTime.MinValue;
            else
                ret.IdentificacaoRps.DataEmissao = rootDoc.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;

            // Natureza da Operação
            ret.NaturezaOperacao = rootDoc.ElementAnyNs("NaturezaOperacao").GetValue<int>();

            // Simples Nacional
            if (rootDoc.ElementAnyNs("OptanteSimplesNacional")?.GetValue<int>() == 1)
            {
                ret.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
            }
            else
            {
                // Regime Especial de Tributaçao
                switch (rootDoc.ElementAnyNs("RegimeEspecialTributacao")?.GetValue<int>())
                {
                    case 1:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresaMunicipal;
                        break;

                    case 2:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.Estimativa;
                        break;

                    case 3:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.SociedadeProfissionais;
                        break;

                    case 4:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.Cooperativa;
                        break;

                    case 5:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresarioIndividual;
                        break;

                    case 6:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresarioEmpresaPP;
                        break;
                }
            }

            // Incentivador Culturalstr
            switch (rootDoc.ElementAnyNs("IncentivadorCultural")?.GetValue<int>())
            {
                case 1:
                    ret.IncentivadorCultural = NFSeSimNao.Sim;
                    break;

                case 2:
                    ret.IncentivadorCultural = NFSeSimNao.Nao;
                    break;
            }

            // Situação do RPS
            if (isRps)
            {
                ret.Situacao = (rootDoc.ElementAnyNs("Status")?.GetValue<string>() ?? string.Empty) == "2" ? SituacaoNFSeRps.Cancelado : SituacaoNFSeRps.Normal;
                // RPS Substituido
                var rootRpsSubstituido = rootDoc.ElementAnyNs("RpsSubstituido");
                if (rootRpsSubstituido != null)
                {
                    ret.RpsSubstituido.NumeroRps = rootRpsSubstituido.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                    ret.RpsSubstituido.Serie = rootRpsSubstituido.ElementAnyNs("Serie")?.GetValue<string>() ?? string.Empty;
                    ret.RpsSubstituido.Tipo = rootRpsSubstituido.ElementAnyNs("Tipo")?.GetValue<TipoRps>() ?? TipoRps.RPS;
                }
            }

            if (isNFSe)
            {
                ret.Competencia = rootDoc.ElementAnyNs("Competencia")?.GetValue<DateTime>() ?? DateTime.MinValue;
                ret.RpsSubstituido.NumeroNfse = rootDoc.ElementAnyNs("NfseSubstituida")?.GetValue<string>() ?? string.Empty;
                ret.OutrasInformacoes = rootDoc.ElementAnyNs("OutrasInformacoes")?.GetValue<string>() ?? string.Empty;
            }

            // Serviços e Valores
            var rootServico = rootDoc.ElementAnyNs("Servico");
            if (rootServico != null)
            {
                var rootServicoValores = rootServico.ElementAnyNs("Valores");
                if (rootServicoValores != null)
                {
                    ret.Servico.Valores.ValorServicos = rootServicoValores.ElementAnyNs("ValorServicos")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorDeducoes = rootServicoValores.ElementAnyNs("ValorDeducoes")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorPis = rootServicoValores.ElementAnyNs("ValorPis")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorCofins = rootServicoValores.ElementAnyNs("ValorCofins")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorInss = rootServicoValores.ElementAnyNs("ValorInss")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorIr = rootServicoValores.ElementAnyNs("ValorIr")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorCsll = rootServicoValores.ElementAnyNs("ValorCsll")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.IssRetido = (rootServicoValores.ElementAnyNs("IssRetido")?.GetValue<int>() ?? 0) == 1 ? SituacaoTributaria.Retencao : SituacaoTributaria.Normal;
                    ret.Servico.Valores.ValorIss = rootServicoValores.ElementAnyNs("ValorIss")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorOutrasRetencoes = rootServicoValores.ElementAnyNs("OutrasRetencoes")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.BaseCalculo = rootServicoValores.ElementAnyNs("BaseCalculo")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.Aliquota = rootServicoValores.ElementAnyNs("Aliquota")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorLiquidoNfse = rootServicoValores.ElementAnyNs("ValorLiquidoNfse")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorIssRetido = rootServicoValores.ElementAnyNs("ValorIssRetido")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.DescontoCondicionado = rootServicoValores.ElementAnyNs("DescontoCondicionado")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.DescontoIncondicionado = rootServicoValores.ElementAnyNs("DescontoIncondicionado")?.GetValue<decimal>() ?? 0;
                }
                ret.Servico.ItemListaServico = rootServico.ElementAnyNs("ItemListaServico")?.GetValue<string>() ?? string.Empty;
                ret.Servico.CodigoCnae = rootServico.ElementAnyNs("CodigoCnae")?.GetValue<string>() ?? string.Empty;
                ret.Servico.CodigoTributacaoMunicipio = rootServico.ElementAnyNs("CodigoTributacaoMunicipio")?.GetValue<string>() ?? string.Empty;
                ret.Servico.Discriminacao = rootServico.ElementAnyNs("Discriminacao")?.GetValue<string>() ?? string.Empty;
                ret.Servico.CodigoMunicipio = rootServico.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
            }
            if (isNFSe)
            {
                ret.ValorCredito = rootDoc.ElementAnyNs("ValorCredito")?.GetValue<decimal>() ?? 0;
            }

            if (isNFSe)
            {
                // Prestador (Nota Fiscal)
                var rootPrestador = rootDoc.ElementAnyNs("PrestadorServico");
                if (rootPrestador != null)
                {
                    var rootPretadorIdentificacao = rootPrestador.ElementAnyNs("IdentificacaoPrestador");
                    if (rootPretadorIdentificacao != null)
                    {
                        ret.Prestador.CpfCnpj = rootPretadorIdentificacao.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.InscricaoMunicipal = rootPretadorIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
                    }
                    ret.Prestador.RazaoSocial = rootPrestador.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.NomeFantasia = rootPrestador.ElementAnyNs("NomeFantasia")?.GetValue<string>() ?? string.Empty;
                    var rootPrestadorEndereco = rootPrestador.ElementAnyNs("Endereco");
                    if (rootPrestadorEndereco != null)
                    {
                        ret.Prestador.Endereco.Logradouro = rootPrestadorEndereco.ElementAnyNs("Endereco")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.Endereco.Numero = rootPrestadorEndereco.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.Endereco.Complemento = rootPrestadorEndereco.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.Endereco.Bairro = rootPrestadorEndereco.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.Endereco.CodigoMunicipio = rootPrestadorEndereco.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
                        ret.Prestador.Endereco.Uf = rootPrestadorEndereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.Endereco.Cep = rootPrestadorEndereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
                    }
                    var rootPrestadorContato = rootPrestador.ElementAnyNs("Contato");
                    if (rootPrestadorContato != null)
                    {
                        ret.Prestador.DadosContato.DDD = "";
                        ret.Prestador.DadosContato.Telefone = rootPrestadorContato.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.DadosContato.Email = rootPrestadorContato.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
                    }
                }
            }
            else
            {
                // Prestador (RPS)
                var rootPrestador = rootDoc.ElementAnyNs("Prestador");
                if (rootPrestador != null)
                {
                    ret.Prestador.CpfCnpj = rootPrestador.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.InscricaoMunicipal = rootPrestador.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Tomador
            var rootTomador = rootDoc.ElementAnyNs(isNFSe ? "TomadorServico" : "Tomador");
            if (rootTomador != null)
            {
                var rootTomadorIdentificacao = rootTomador.ElementAnyNs("IdentificacaoTomador");
                if (rootTomadorIdentificacao != null)
                {
                    var rootTomadorIdentificacaoCpfCnpj = rootTomadorIdentificacao.ElementAnyNs("CpfCnpj");
                    if (rootTomadorIdentificacaoCpfCnpj != null)
                    {
                        ret.Tomador.CpfCnpj = rootTomadorIdentificacaoCpfCnpj.ElementAnyNs("Cpf")?.GetValue<string>() ?? string.Empty;
                        if (ret.Tomador.CpfCnpj.IsEmpty())
                        {
                            ret.Tomador.CpfCnpj = rootTomadorIdentificacaoCpfCnpj.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                        }
                    }
                    ret.Tomador.InscricaoMunicipal = rootTomadorIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
                }
                ret.Tomador.RazaoSocial = rootTomador.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
                var rootTomadorEndereco = rootTomador.ElementAnyNs("Endereco");
                if (rootTomadorEndereco != null)
                {
                    ret.Tomador.Endereco.Logradouro = rootTomadorEndereco.ElementAnyNs("Endereco")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Numero = rootTomadorEndereco.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Complemento = rootTomadorEndereco.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Bairro = rootTomadorEndereco.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.CodigoMunicipio = rootTomadorEndereco.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
                    ret.Tomador.Endereco.Uf = rootTomadorEndereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Cep = rootTomadorEndereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
                }
                var rootTomadorContato = rootTomador.ElementAnyNs("Contato");
                if (rootTomadorContato != null)
                {
                    ret.Tomador.DadosContato.DDD = "";
                    ret.Tomador.DadosContato.Telefone = rootTomadorContato.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.DadosContato.Email = rootTomadorContato.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Intermediario
            var rootIntermediarioIdentificacao = rootDoc.ElementAnyNs("IntermediarioServico");
            if (rootIntermediarioIdentificacao != null)
            {
                ret.Intermediario.RazaoSocial = rootIntermediarioIdentificacao.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
                var rootIntermediarioIdentificacaoCpfCnpj = rootIntermediarioIdentificacao.ElementAnyNs("CpfCnpj");
                if (rootIntermediarioIdentificacaoCpfCnpj != null)
                {
                    ret.Intermediario.CpfCnpj = rootIntermediarioIdentificacaoCpfCnpj.ElementAnyNs("Cpf")?.GetValue<string>() ?? string.Empty;
                    if (ret.Intermediario.CpfCnpj.IsEmpty())
                        ret.Intermediario.CpfCnpj = rootIntermediarioIdentificacaoCpfCnpj.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                }
                ret.Intermediario.InscricaoMunicipal = rootIntermediarioIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
            }

            if (isNFSe)
            {
                // Orgão Gerador
                var rootOrgaoGerador = rootDoc.ElementAnyNs("OrgaoGerador");
                if (rootOrgaoGerador != null)
                {
                    ret.OrgaoGerador.CodigoMunicipio = rootOrgaoGerador.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
                    ret.OrgaoGerador.Uf = rootOrgaoGerador.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Construção Civil
            var rootConstrucaoCivil = rootDoc.ElementAnyNs("ConstrucaoCivil");
            if (rootConstrucaoCivil != null)
            {
                ret.ConstrucaoCivil.CodigoObra = rootConstrucaoCivil.ElementAnyNs("CodigoObra")?.GetValue<string>() ?? string.Empty;
                ret.ConstrucaoCivil.ArtObra = rootConstrucaoCivil.ElementAnyNs("Art")?.GetValue<string>() ?? string.Empty;
            }

            // Verifica se a NFSe está cancelada
            if (rootCanc != null)
            {
                ret.Situacao = SituacaoNFSeRps.Cancelado;
                ret.Cancelamento.Pedido.CodigoCancelamento = rootCanc.ElementAnyNs("Confirmacao").ElementAnyNs("Pedido").ElementAnyNs("InfPedidoCancelamento")?.ElementAnyNs("CodigoCancelamento")?.GetValue<string>() ?? string.Empty;
                ret.Cancelamento.DataHora = rootCanc.ElementAnyNs("Confirmacao").ElementAnyNs("DataHoraCancelamento")?.GetValue<DateTime>() ?? DateTime.MinValue;
                ret.Cancelamento.Signature = LoadSignature(rootCanc.ElementAnyNs("Signature"));
                ret.Cancelamento.Pedido.Signature = LoadSignature(rootCanc.ElementAnyNs("Confirmacao").ElementAnyNs("Pedido").ElementAnyNs("Signature"));
            }

            if (rootSub != null)
            {
                ret.RpsSubstituido.NFSeSubstituidora = rootSub.ElementAnyNs("SubstituicaoNfse").ElementAnyNs("NfseSubstituidora").GetValue<string>();
                ret.RpsSubstituido.Signature = LoadSignature(rootSub.ElementAnyNs("SubstituicaoNfse").ElementAnyNs("Signature"));
            }

            return ret;
        }

        #endregion LoadXML

        #region RPS

        public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
        {
            var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            xmlDoc.Add(WriteRps(nota));
            return xmlDoc.AsString(identado, showDeclaration);
        }

        private XElement WriteRps(NotaServico nota)
        {
            var rps = new XElement("Rps");
            var infoRps = WriteInfoRPS(nota);
            rps.Add(infoRps);

            infoRps.AddChild(WriteRpsSubstituto(nota));
            infoRps.AddChild(WriteServicosValoresRps(nota));
            infoRps.AddChild(WritePrestadorRps(nota));
            infoRps.AddChild(WriteTomadorRps(nota));
            infoRps.AddChild(WriteIntermediarioRps(nota));
            infoRps.AddChild(WriteConstrucaoCivilRps(nota));

            return rps;
        }

        private XElement WriteInfoRPS(NotaServico nota)
        {
            var incentivadorCultural = nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2;

            string regimeEspecialTributacao;
            string optanteSimplesNacional;
            if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
            {
                regimeEspecialTributacao = "1";
                optanteSimplesNacional = "1";
            }
            else
            {
                var regime = (int)nota.RegimeEspecialTributacao;
                regimeEspecialTributacao = regime == 0 ? string.Empty : regime.ToString();
                optanteSimplesNacional = "2";
            }

            var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "1" : "2";

            var infoRps = new XElement("InfRps");

            infoRps.Add(WriteIdentificacao(nota));
            infoRps.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissao", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "NaturezaOperacao", 1, 1, Ocorrencia.Obrigatoria, nota.NaturezaOperacao));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivadorCultural", 1, 1, Ocorrencia.Obrigatoria, incentivadorCultural));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, situacao));

            return infoRps;
        }

        private XElement WriteIdentificacao(NotaServico nota)
        {
            string tipoRps;
            switch (nota.IdentificacaoRps.Tipo)
            {
                case TipoRps.RPS:
                    tipoRps = "1";
                    break;

                case TipoRps.NFConjugada:
                    tipoRps = "2";
                    break;

                case TipoRps.Cupom:
                    tipoRps = "3";
                    break;

                default:
                    tipoRps = "0";
                    break;
            }

            var ideRps = new XElement("IdentificacaoRps");
            ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
            ideRps.AddChild(AdicionarTag(TipoCampo.Str, "", "Serie", 1, 5, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
            ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", 1, 1, Ocorrencia.Obrigatoria, tipoRps));

            return ideRps;
        }

        private XElement WriteRpsSubstituto(NotaServico nota)
        {
            if (nota.RpsSubstituido.NumeroRps.IsEmpty()) return null;

            string tipoRpsSubstituido;
            switch (nota.RpsSubstituido.Tipo)
            {
                case TipoRps.RPS:
                    tipoRpsSubstituido = "1";
                    break;

                case TipoRps.NFConjugada:
                    tipoRpsSubstituido = "2";
                    break;

                case TipoRps.Cupom:
                    tipoRpsSubstituido = "3";
                    break;

                default:
                    tipoRpsSubstituido = "0";
                    break;
            }

            var rpsSubstituido = new XElement("RpsSubstituido");

            rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.RpsSubstituido.NumeroRps));
            rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Serie", 1, 5, Ocorrencia.Obrigatoria, nota.RpsSubstituido.Serie));
            rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", 1, 1, Ocorrencia.Obrigatoria, tipoRpsSubstituido));

            return rpsSubstituido;
        }

        private XElement WriteServicosValoresRps(NotaServico nota)
        {
            var servico = new XElement("Servico");
            var valores = new XElement("Valores");
            servico.AddChild(valores);

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));

            valores.AddChild(AdicionarTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIssRetido", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIssRetido));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "BaseCalculo", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.BaseCalculo));

            // Valor Percentual - Exemplos: 5% => 0.05   /   25,5% => 0.255   /   100% => 1
            valores.AddChild(AdicionarTag(TipoCampo.De4, "", "Aliquota", 1, 15, Ocorrencia.MaiorQueZero, (nota.Servico.Valores.Aliquota / 100)));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorLiquidoNfse", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorLiquidoNfse));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));

            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));

            servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoCnae", 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));

            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
            servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoMunicipio", 1, 7, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));

            return servico;
        }

        private XElement WritePrestadorRps(NotaServico nota)
        {
            var prestador = new XElement("Prestador");

            //var cpfCnpjPrestador = new XElement("CpfCnpj");
            //prestador.Add(cpfCnpjPrestador);

            //cpfCnpjPrestador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj));

            prestador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj));
            prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Prestador.InscricaoMunicipal));

            return prestador;
        }

        private XElement WriteTomadorRps(NotaServico nota)
        {
            var tomador = new XElement("Tomador");

            var ideTomador = new XElement("IdentificacaoTomador");
            tomador.Add(ideTomador);

            var cpfCnpjTomador = new XElement("CpfCnpj");
            ideTomador.Add(cpfCnpjTomador);

            if (!string.IsNullOrEmpty(nota.Tomador.CpfCnpj)) // Existem NFS-e que é possível ser emitida sem tomador
            {
                cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

                ideTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

                tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));

                if (!nota.Tomador.Endereco.Logradouro.IsEmpty() || !nota.Tomador.Endereco.Numero.IsEmpty() ||
                    !nota.Tomador.Endereco.Complemento.IsEmpty() || !nota.Tomador.Endereco.Bairro.IsEmpty() ||
                    nota.Tomador.Endereco.CodigoMunicipio > 0 || !nota.Tomador.Endereco.Uf.IsEmpty() ||
                    !nota.Tomador.Endereco.Cep.IsEmpty())
                {
                    var endereco = new XElement("Endereco");
                    tomador.Add(endereco);

                    endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
                    endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
                    endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
                    endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
                    endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoMunicipio));
                    endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
                    endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
                }

                if (!nota.Tomador.DadosContato.DDD.IsEmpty() || !nota.Tomador.DadosContato.Telefone.IsEmpty() ||
                    !nota.Tomador.DadosContato.Email.IsEmpty())
                {
                    var contato = new XElement("Contato");
                    tomador.Add(contato);

                    contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Telefone", 1, 11, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));
                    contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
                }
            }
            else
            {
                cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", "00000000000"));
            }

            return tomador;
        }

        private XElement WriteIntermediarioRps(NotaServico nota)
        {
            if (nota.Intermediario.RazaoSocial.IsEmpty()) return null;

            var intermediario = new XElement("Intermediario");

            var ideIntermediario = new XElement("IdentificacaoIntermediario");
            intermediario.Add(ideIntermediario);

            var cpfCnpj = new XElement("CpfCnpj");
            ideIntermediario.Add(cpfCnpj);

            cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Intermediario.CpfCnpj));

            ideIntermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria,
                nota.Intermediario.InscricaoMunicipal));

            intermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria,
                nota.Intermediario.RazaoSocial));

            return intermediario;
        }

        private XElement WriteConstrucaoCivilRps(NotaServico nota)
        {
            if (nota.ConstrucaoCivil.CodigoObra.IsEmpty()) return null;

            var construcao = new XElement("ContrucaoCivil");

            construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoObra", 1, 15, Ocorrencia.NaoObrigatoria, nota.ConstrucaoCivil.CodigoObra));
            construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "Art", 1, 15, Ocorrencia.Obrigatoria, nota.ConstrucaoCivil.ArtObra));

            return construcao;
        }

        public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true)
        {
            var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            var compNfse = new XElement("CompNfse");

            compNfse.AddChild(WriteNFSe(nota));
            compNfse.AddChild(WriteNFSeCancelamento(nota));
            compNfse.AddChild(WriteNFSeSubstituicao(nota));

            xmlDoc.AddChild(compNfse);
            return xmlDoc.AsString(identado, showDeclaration);
        }

        private XElement WriteNFSe(NotaServico nota)
        {
            var incentivadorCultural = nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2;

            string tipoRps;
            switch (nota.IdentificacaoRps.Tipo)
            {
                case TipoRps.RPS:
                    tipoRps = "1";
                    break;

                case TipoRps.NFConjugada:
                    tipoRps = "2";
                    break;

                case TipoRps.Cupom:
                    tipoRps = "3";
                    break;

                default:
                    tipoRps = "0";
                    break;
            }

            string tipoRpsSubstituido;
            switch (nota.RpsSubstituido.Tipo)
            {
                case TipoRps.RPS:
                    tipoRpsSubstituido = "1";
                    break;

                case TipoRps.NFConjugada:
                    tipoRpsSubstituido = "2";
                    break;

                case TipoRps.Cupom:
                    tipoRpsSubstituido = "3";
                    break;

                default:
                    tipoRpsSubstituido = "0";
                    break;
            }

            string regimeEspecialTributacao;
            string optanteSimplesNacional;
            if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
            {
                regimeEspecialTributacao = "6";
                optanteSimplesNacional = "1";
            }
            else
            {
                regimeEspecialTributacao = ((int)nota.RegimeEspecialTributacao).ToString();
                optanteSimplesNacional = "2";
            }

            var nfse = new XElement("Nfse");

            var infNfse = new XElement("InfNfse", new XAttribute("Id", nota.IdentificacaoNFSe.Numero));
            nfse.Add(infNfse);

            infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.Numero));
            infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoVerificacao", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.Chave));
            infNfse.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissao", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.DataEmissao));

            var infRps = new XElement("IdentificacaoRps");
            infNfse.Add(infRps);

            infRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
            infRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Serie", 1, 5, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
            infRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", 1, 1, Ocorrencia.Obrigatoria, tipoRps));

            infNfse.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissaoRps", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "NaturezaOperacao", 1, 1, Ocorrencia.Obrigatoria, nota.NaturezaOperacao));
            infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
            infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
            infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivadorCultural", 1, 1, Ocorrencia.Obrigatoria, incentivadorCultural));
            infNfse.AddChild(AdicionarTag(TipoCampo.Dat, "", "Competencia", 10, 10, Ocorrencia.Obrigatoria, nota.Competencia));

            if (!string.IsNullOrWhiteSpace(nota.RpsSubstituido.NumeroRps))
            {
                var rpsSubstituido = new XElement("RpsSubstituido");

                rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.RpsSubstituido.NumeroRps));
                rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Serie", 1, 5, Ocorrencia.Obrigatoria, nota.RpsSubstituido.Serie));
                rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", 1, 1, Ocorrencia.Obrigatoria, tipoRpsSubstituido));

                infNfse.AddChild(rpsSubstituido);
            }

            var servico = WriteServicosValoresNFSe(nota);
            infNfse.AddChild(servico);

            var prestador = WritePrestadorNFSe(nota);
            infNfse.AddChild(prestador);

            var tomador = WriteTomadorNFSe(nota);
            infNfse.AddChild(tomador);

            if (!nota.Intermediario.RazaoSocial.IsEmpty())
            {
                var intServico = WriteIntermediarioNFSe(nota);
                infNfse.AddChild(intServico);
            }

            if (!nota.ConstrucaoCivil.CodigoObra.IsEmpty())
            {
                var conCivil = WriteConstrucaoCivilNFSe(nota);
                infNfse.AddChild(conCivil);
            }

            if (nota.OrgaoGerador.CodigoMunicipio != 0)
            {
                var orgGerador = new XElement("OrgaoGerador");
                infNfse.AddChild(orgGerador);

                orgGerador.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoMunicipio", 1, 7, Ocorrencia.NaoObrigatoria, nota.OrgaoGerador.CodigoMunicipio));
                orgGerador.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.OrgaoGerador.Uf));
            }

            nfse.AddChild(WriteSignature(nota.Signature));

            return nfse;
        }

        private XElement WriteNFSeCancelamento(NotaServico nota)
        {
            if (nota.Situacao != SituacaoNFSeRps.Cancelado) return null;

            var nfSeCancelamento = new XElement("NfseCancelamento");

            var confirmacao = new XElement("Confirmacao");
            nfSeCancelamento.AddChild(confirmacao);

            confirmacao.AddChild(WriteSignature(nota.Cancelamento.Signature));

            var pedido = new XElement("Pedido", new XAttribute("Id", nota.Cancelamento.Id));
            confirmacao.AddChild(pedido);

            var infPedidoCancelamento = new XElement("InfPedidoCancelamento", new XAttribute("Id", nota.Cancelamento.Id));
            pedido.AddChild(infPedidoCancelamento);

            var identificacaoNfse = new XElement("IdentificacaoNfse");
            infPedidoCancelamento.AddChild(identificacaoNfse);

            identificacaoNfse.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.Cancelamento.Pedido.IdentificacaoNFSe.Numero));
            identificacaoNfse.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj.ZeroFill(14)));
            identificacaoNfse.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoMunicipal", 1, 15, Ocorrencia.Obrigatoria, nota.Prestador.InscricaoMunicipal));
            identificacaoNfse.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoMunicipio", 1, 7, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.CodigoMunicipio));

            infPedidoCancelamento.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoCancelamento", 1, 4, Ocorrencia.Obrigatoria, nota.Cancelamento.Pedido.CodigoCancelamento));

            pedido.AddChild(WriteSignature(nota.Cancelamento.Pedido.Signature));

            confirmacao.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataHoraCancelamento", 20, 20, Ocorrencia.Obrigatoria, nota.Cancelamento.DataHora));

            return nfSeCancelamento;
        }

        private XElement WriteNFSeSubstituicao(NotaServico nota)
        {
            if (nota.RpsSubstituido.NFSeSubstituidora.IsEmpty()) return null;

            var substituidora = new XElement("NfseSubstituicao");
            var substituicaoNfse = new XElement("SubstituicaoNfse", new XAttribute("Id", nota.RpsSubstituido.Id));
            substituidora.AddChild(substituicaoNfse);

            substituicaoNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "NfseSubstituidora", 1, 15, Ocorrencia.Obrigatoria, nota.RpsSubstituido.NFSeSubstituidora));
            substituicaoNfse.AddChild(WriteSignature(nota.RpsSubstituido.Signature));

            return substituidora;
        }

        private XElement WriteServicosValoresNFSe(NotaServico nota)
        {
            var servico = new XElement("Servico");
            var valores = new XElement("Valores");
            servico.AddChild(valores);

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));

            valores.AddChild(AdicionarTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIssRetido", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIssRetido));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "BaseCalculo", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.BaseCalculo));
            valores.AddChild(AdicionarTag(TipoCampo.De4, "", "Aliquota", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota / 100)); // Valor Percentual - Exemplos: 1% => 0.01   /   25,5% => 0.255   /   100% => 1
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorLiquidoNfse", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorLiquidoNfse));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));

            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));

            servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoCnae", 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));

            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
            servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoMunicipio", 1, 7, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));

            return servico;
        }

        private XElement WritePrestadorNFSe(NotaServico nota)
        {
            var prestador = new XElement("Prestador");

            var cpfCnpjPrestador = new XElement("CpfCnpj");
            prestador.Add(cpfCnpjPrestador);

            cpfCnpjPrestador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj));

            prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Prestador.InscricaoMunicipal));

            return prestador;
        }

        private XElement WriteTomadorNFSe(NotaServico nota)
        {
            var tomador = new XElement("Tomador");

            var ideTomador = new XElement("IdentificacaoTomador");
            tomador.Add(ideTomador);

            var cpfCnpjTomador = new XElement("CpfCnpj");
            ideTomador.Add(cpfCnpjTomador);

            cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

            ideTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));

            if (!nota.Tomador.Endereco.Logradouro.IsEmpty() || !nota.Tomador.Endereco.Numero.IsEmpty() ||
                !nota.Tomador.Endereco.Complemento.IsEmpty() || !nota.Tomador.Endereco.Bairro.IsEmpty() ||
                nota.Tomador.Endereco.CodigoMunicipio > 0 || !nota.Tomador.Endereco.Uf.IsEmpty() ||
                !nota.Tomador.Endereco.Cep.IsEmpty())
            {
                var endereco = new XElement("Endereco");
                tomador.Add(endereco);

                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
                endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoMunicipio));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
                endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
            }

            if (!nota.Tomador.DadosContato.DDD.IsEmpty() || !nota.Tomador.DadosContato.Telefone.IsEmpty() ||
                !nota.Tomador.DadosContato.Email.IsEmpty())
            {
                var contato = new XElement("Contato");
                tomador.Add(contato);

                contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Telefone", 1, 11, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));
                contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
            }

            return tomador;
        }

        private XElement WriteIntermediarioNFSe(NotaServico nota)
        {
            var intermediario = new XElement("Intermediario");

            var ideIntermediario = new XElement("IdentificacaoIntermediario");
            intermediario.Add(ideIntermediario);

            var cpfCnpj = new XElement("CpfCnpj");
            ideIntermediario.Add(cpfCnpj);

            cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Intermediario.CpfCnpj));

            ideIntermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria,
                nota.Intermediario.InscricaoMunicipal));

            intermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria,
                nota.Intermediario.RazaoSocial));

            return intermediario;
        }

        private XElement WriteConstrucaoCivilNFSe(NotaServico nota)
        {
            var construcao = new XElement("ConstrucaoCivil");

            construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoObra", 1, 15, Ocorrencia.NaoObrigatoria, nota.ConstrucaoCivil.CodigoObra));
            construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "Art", 1, 15, Ocorrencia.Obrigatoria, nota.ConstrucaoCivil.ArtObra));

            return construcao;
        }

        #endregion RPS

        #region Services

        protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            if (retornoWebservice.Lote == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
            if (retornoWebservice.Erros.Count > 0) return;

            var xmlLoteRps = new StringBuilder();

            foreach (var nota in notas)
            {
                var xmlRps = WriteXmlRps(nota, false, false);
                xmlLoteRps.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }

            var xmlLote = new StringBuilder();
            xmlLote.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlLote.Append("<RecepcionarLoteRps xmlns=\"http://www.e-governeapps2.com.br/\">");
            xmlLote.Append("<EnviarLoteRpsEnvio>");
            xmlLote.Append("<LoteRps>");
            xmlLote.Append($"<NumeroLote>{retornoWebservice.Lote}</NumeroLote>");
            xmlLote.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            xmlLote.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            xmlLote.Append($"<QuantidadeRps>{notas.Count}</QuantidadeRps>");
            xmlLote.Append("<ListaRps>");
            xmlLote.Append(xmlLoteRps);
            xmlLote.Append("</ListaRps>");
            xmlLote.Append("</LoteRps>");
            xmlLote.Append("</EnviarLoteRpsEnvio>");
            xmlLote.Append("</RecepcionarLoteRps >");

            var document = XDocument.Parse(xmlLote.ToString());

            retornoWebservice.XmlEnvio = document.AsString();
        }

        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsEnvio", "", Certificado);
        }

        protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            loteBuilder.Append("<ConsultarSituacaoLoteRps xmlns=\"http://www.e-governeapps2.com.br/\">");
            loteBuilder.Append("<ConsultarSituacaoLoteRpsEnvio>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarSituacaoLoteRpsEnvio>");
            loteBuilder.Append("</ConsultarSituacaoLoteRps>");

            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            loteBuilder.Append("<ConsultarLoteRps xmlns=\"http://www.e-governeapps2.com.br/\">");
            loteBuilder.Append("<ConsultarLoteRpsEnvio>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarLoteRpsEnvio>");
            loteBuilder.Append("</ConsultarLoteRps>");

            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            if (retornoWebservice.NumeroRps < 1)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe não informado para a consulta." });
                return;
            }

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            loteBuilder.Append("<ConsultarNfseRps xmlns=\"http://www.e-governeapps2.com.br/\">");
            loteBuilder.Append("<ConsultarNfseRpsEnvio>");
            loteBuilder.Append("<IdentificacaoRps>");
            loteBuilder.Append($"<Numero>{retornoWebservice.NumeroRps}</Numero>");
            loteBuilder.Append($"<Serie>{retornoWebservice.Serie}</Serie>");
            loteBuilder.Append($"<Tipo>{(int)retornoWebservice.Tipo + 1}<Tipo>");
            loteBuilder.Append("</IdentificacaoRps>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append("</ConsultarNfseRpsEnvio>");
            loteBuilder.Append("</ConsultarNfseRps>");

            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            var loteBuilder = new StringBuilder();

            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            loteBuilder.Append("<ConsultarNfse xmlns=\"http://www.e-governeapps2.com.br/\">");
            loteBuilder.Append("<ConsultarNfseEnvio>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");

            if (retornoWebservice.NumeroNFse > 0)
                loteBuilder.Append($"<NumeroNfse>{retornoWebservice.NumeroNFse}</NumeroNfse>");

            if (retornoWebservice.Inicio.HasValue && retornoWebservice.Fim.HasValue)
            {
                loteBuilder.Append("<PeriodoEmissao>");
                loteBuilder.Append($"<DataInicial>{retornoWebservice.Inicio:yyyy-MM-dd}</DataInicial>");
                loteBuilder.Append($"<DataFinal>{retornoWebservice.Fim:yyyy-MM-dd}</DataFinal>");
                loteBuilder.Append("</PeriodoEmissao>");
            }

            if (!retornoWebservice.CPFCNPJTomador.IsEmpty())
            {
                loteBuilder.Append("<Tomador>");
                loteBuilder.Append("<CpfCnpj>");
                loteBuilder.Append(retornoWebservice.CPFCNPJTomador.IsCNPJ()
                    ? $"<Cnpj>{retornoWebservice.CPFCNPJTomador.ZeroFill(14)}</Cnpj>"
                    : $"<Cpf>{retornoWebservice.CPFCNPJTomador.ZeroFill(11)}</Cpf>");
                loteBuilder.Append("</CpfCnpj>");
                if (!retornoWebservice.IMTomador.IsEmpty())
                    loteBuilder.Append($"<InscricaoMunicipal>{retornoWebservice.CPFCNPJTomador}</InscricaoMunicipal>");
                loteBuilder.Append("</Tomador>");
            }

            if (!retornoWebservice.NomeIntermediario.IsEmpty() && !retornoWebservice.CPFCNPJIntermediario.IsEmpty())
            {
                loteBuilder.Append("<IntermediarioServico>");
                loteBuilder.Append($"<RazaoSocial>{retornoWebservice.NomeIntermediario}</RazaoSocial>");
                loteBuilder.Append("<CpfCnpj>");
                loteBuilder.Append(retornoWebservice.CPFCNPJIntermediario.IsCNPJ()
                    ? $"<Cnpj>{retornoWebservice.CPFCNPJIntermediario.ZeroFill(14)}</Cnpj>"
                    : $"<Cpf>{retornoWebservice.CPFCNPJIntermediario.ZeroFill(11)}</Cpf>");
                loteBuilder.Append("</CpfCnpj>");
                if (!retornoWebservice.IMIntermediario.IsEmpty())
                    loteBuilder.Append($"<InscricaoMunicipal>{retornoWebservice.IMIntermediario}</InscricaoMunicipal>");
                loteBuilder.Append("</IntermediarioServico>");
            }

            loteBuilder.Append("</ConsultarNfseEnvio>");
            loteBuilder.Append("</ConsultarNfse>");

            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            if (retornoWebservice.NumeroNFSe.IsEmpty() || retornoWebservice.CodigoCancelamento.IsEmpty())
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe/Codigo de cancelamento não informado para cancelamento." });
                return;
            }

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            loteBuilder.Append("<CancelarNfse xmlns=\"http://www.e-governeapps2.com.br/\">");
            loteBuilder.Append("<CancelarNfseEnvio>");
            loteBuilder.Append("<Pedido>");
            loteBuilder.Append("<InfPedidoCancelamento>");
            loteBuilder.Append("<IdentificacaoNfse>");
            loteBuilder.Append($"<Numero>{retornoWebservice.NumeroNFSe}</Numero>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append($"<CodigoMunicipio>{Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio}</CodigoMunicipio>");
            loteBuilder.Append("</IdentificacaoNfse>");
            loteBuilder.Append($"<CodigoCancelamento>{retornoWebservice.CodigoCancelamento}</CodigoCancelamento>");

            if (!retornoWebservice.Motivo.IsEmpty())
                loteBuilder.Append($"<MotivoCancelamentoNfse>{retornoWebservice.Motivo}</MotivoCancelamentoNfse>");

            loteBuilder.Append("</InfPedidoCancelamento>");
            loteBuilder.Append("</Pedido>");
            loteBuilder.Append("</CancelarNfseEnvio>");
            loteBuilder.Append("</CancelarNfse>");

            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "InfPedidoCancelamento", "", Certificado);
        }

        #endregion Services

        #region Protected Methods

        protected override IServiceClient GetClient(TipoUrl tipo)
        {
            if (tipo == TipoUrl.ConsultarNFSe)
                return new CuritibaServiceClient(this, tipo, Certificado);

            return new CuritibaServiceClient(this, tipo);
        }

        private string GetNamespace()
        {
            return "xmlns=\"http://www.e-governeapps2.com.br/\"";
        }

        private void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet,
            string elementName = "ListaMensagemRetorno", string messageElement = "MensagemRetorno")
        {
            var listaMenssagens = xmlRet?.ElementAnyNs(elementName);
            if (listaMenssagens == null) return;

            foreach (var mensagem in listaMenssagens.ElementsAnyNs(messageElement))
            {
                var evento = new Evento
                {
                    Codigo = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty,
                    Descricao = mensagem?.ElementAnyNs("Mensagem")?.GetValue<string>() ?? string.Empty,
                    Correcao = mensagem?.ElementAnyNs("Correcao")?.GetValue<string>() ?? string.Empty
                };

                retornoWs.Erros.Add(evento);
            }
        }

        protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var xmlConsutarSituacaoLoteResult = xmlRet.ElementAnyNs("ConsultarSituacaoLoteRpsResponse").ElementAnyNs("ConsultarSituacaoLoteRpsResult");
            MensagemErro(retornoWebservice, xmlConsutarSituacaoLoteResult);

            retornoWebservice.Lote = xmlConsutarSituacaoLoteResult?.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;
            retornoWebservice.Situacao = xmlConsutarSituacaoLoteResult?.ElementAnyNs("Situacao")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.Sucesso = !retornoWebservice.Erros.Any();
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            return "nfse.xsd";
        }

        protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
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

        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
        }

        protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
        }

        protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
        }

        protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
        }

        protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
        {
        }

        protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
        }

        protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice)
        {
        }

        protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var xmlRecepcionarLoteRpsResult = xmlRet.ElementAnyNs("RecepcionarLoteRpsResponse").ElementAnyNs("RecepcionarLoteRpsResult");
            MensagemErro(retornoWebservice, xmlRecepcionarLoteRpsResult);
            if (retornoWebservice.Erros.Any()) return;

            retornoWebservice.Data = xmlRecepcionarLoteRpsResult?.ElementAnyNs("DataRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Protocolo = xmlRecepcionarLoteRpsResult?.ElementAnyNs("Protocolo")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.Sucesso = !retornoWebservice.Protocolo.IsEmpty();

            if (!retornoWebservice.Sucesso) return;

            foreach (var nota in notas)
            {
                nota.NumeroLote = retornoWebservice.Lote;
            }
        }

        protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException();
        }

        protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var xmlConsultarLoteRpsResult = xmlRet.ElementAnyNs("ConsultarLoteRpsResponse").ElementAnyNs("ConsultarLoteRpsResult");
            MensagemErro(retornoWebservice, xmlConsultarLoteRpsResult);
            if (retornoWebservice.Erros.Any()) return;

            var retornoLote = xmlRet.ElementAnyNs("ConsultarLoteRpsResponse");
            var consultarLoteRpsResult = retornoLote?.ElementAnyNs("ConsultarLoteRpsResult");
            var listaNfse = consultarLoteRpsResult?.ElementAnyNs("ListaNfse");

            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
                return;
            }

            retornoWebservice.Sucesso = true;

            foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
            {
                var nfse = compNfse.ElementAnyNs("tcCompNfse").ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
                var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                var chaveNFSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
                var dataEmissao = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
                var numeroRps = nfse?.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataEmissao);

                var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
                if (nota == null)
                {
                    notas.Load(compNfse.ToString());
                }
                else
                {
                    nota.IdentificacaoNFSe.Numero = numeroNFSe;
                    nota.IdentificacaoNFSe.Chave = chaveNFSe;
                    nota.IdentificacaoNFSe.DataEmissao = dataEmissao;
                    nota.XmlOriginal = compNfse.AsString();
                }
            }
        }

        protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var xmlConsultarNFSePorRpsResult = xmlRet.ElementAnyNs("ConsultarNfsePorRpsResponse").ElementAnyNs("ConsultarNfsePorRpsResult");
            MensagemErro(retornoWebservice, xmlConsultarNFSePorRpsResult);
            if (retornoWebservice.Erros.Any()) return;

            var compNfse = xmlRet.ElementAnyNs("ConsultarNfsePorRpsResult")?.ElementAnyNs("CompNfse");
            if (compNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (CompNfse)" });
                return;
            }

            // Carrega a nota fiscal na coleção de Notas Fiscais
            var nota = LoadXml(compNfse.AsString());
            notas.Add(nota);

            retornoWebservice.Sucesso = true;
        }

        protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var xmlConsultarNfseResult = xmlRet.ElementAnyNs("ConsultarNfseResponse").ElementAnyNs("ConsultarNfseResult");
            MensagemErro(retornoWebservice, xmlConsultarNfseResult);
            if (retornoWebservice.Erros.Any()) return;

            var retornoLote = xmlRet.ElementAnyNs("ConsultarNfseResponse");
            var listaNfse = retornoLote?.ElementAnyNs("ConsultarNfseResult").ElementAnyNs("ListaNfse");
            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
                return;
            }

            var notasServico = new List<NotaServico>();

            foreach (var compNfse in listaNfse.ElementAnyNs("CompNfse").ElementsAnyNs("tcCompNfse"))
            {
                // Carrega a nota fiscal na coleção de Notas Fiscais
                var nota = LoadXml(compNfse.AsString());
                notas.Add(nota);
                notasServico.Add(nota);
            }

            retornoWebservice.Notas = notasServico.ToArray();
            retornoWebservice.Sucesso = true;
        }

        protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var xmlCancelarNfseResult = xmlRet.ElementAnyNs("CancelarNfseResponse").ElementAnyNs("CancelarNfseResult");
            MensagemErro(retornoWebservice, xmlCancelarNfseResult);
            if (retornoWebservice.Erros.Any()) return;

            var confirmacaoCancelamento = xmlCancelarNfseResult.ElementAnyNs("Cancelamento")?
                                                                .ElementAnyNs("Confirmacao");

            if (confirmacaoCancelamento == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Confirmação do cancelamento não encontrada!" });
                return;
            }

            retornoWebservice.Data = confirmacaoCancelamento.ElementAnyNs("DataHoraCancelamento")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Sucesso = retornoWebservice.Data != DateTime.MinValue;
            retornoWebservice.CodigoCancelamento = confirmacaoCancelamento.ElementAnyNs("Pedido").ElementAnyNs("InfPedidoCancelamento")
                                                                          .ElementAnyNs("CodigoCancelamento").GetValue<string>();

            var numeroNFSe = confirmacaoCancelamento.ElementAnyNs("Pedido").ElementAnyNs("InfPedidoCancelamento")?
                                 .ElementAnyNs("IdentificacaoNfse")?.ElementAnyNs("Numero").GetValue<string>() ?? string.Empty;

            // Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
            var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == numeroNFSe);
            if (nota == null) return;

            nota.Situacao = SituacaoNFSeRps.Cancelado;
            nota.Cancelamento.Pedido.CodigoCancelamento = retornoWebservice.CodigoCancelamento;
            nota.Cancelamento.DataHora = retornoWebservice.Data;
        }

        protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException();
        }

        protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException();
        }

        protected override string GerarCabecalho()
        {
            return $"<cabecalho versao=\"1.00\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>1.00</versaoDados></cabecalho>";
        }

        #endregion Protected Methods

        #endregion Methods
    }
}