// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 10-02-2014
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="ProviderDSF.cs" company="OpenAC .Net">
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
    internal sealed class ProviderDSF : ProviderBase
    {
        #region Internal Types

        private enum TipoEvento
        {
            Erros,
            Alertas,
            ListNFSeRps
        }

        #endregion Internal Types

        #region Fields

        private string situacao;

        private string recolhimento;

        private string tributacao;

        private string operacao;

        private string assinatura;

        #endregion Fields

        #region Constructors

        public ProviderDSF(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "DSF";
        }

        #endregion Constructors

        #region Methods

        #region Public

        public override NotaServico LoadXml(XDocument xml)
        {
            var root = xml.ElementAnyNs("Nota") ?? xml.ElementAnyNs("RPS");
            Guard.Against<XmlException>(root == null, "Xml de Nota/RPS invalida.");

            var ret = new NotaServico(Configuracoes);

            // Prestador
            ret.Prestador.InscricaoMunicipal = root.ElementAnyNs("InscricaoMunicipalPrestador").GetValue<string>();
            ret.Prestador.RazaoSocial = root.ElementAnyNs("RazaoSocialPrestador").GetValue<string>();
            ret.Prestador.DadosContato.DDD = root.ElementAnyNs("DDDPrestador").GetValue<string>();
            ret.Prestador.DadosContato.Telefone = root.ElementAnyNs("TelefonePrestador").GetValue<string>();
            ret.Intermediario.CpfCnpj = root.ElementAnyNs("CPFCNPJIntermediario").GetValue<string>();

            // Tomador
            ret.Tomador.InscricaoMunicipal = root.ElementAnyNs("InscricaoMunicipalTomador").GetValue<string>();
            ret.Tomador.CpfCnpj = root.ElementAnyNs("CPFCNPJTomador").GetValue<string>();
            ret.Tomador.RazaoSocial = root.ElementAnyNs("RazaoSocialTomador").GetValue<string>();
            ret.Tomador.Endereco.TipoLogradouro = root.ElementAnyNs("TipoLogradouroTomador").GetValue<string>();
            ret.Tomador.Endereco.Logradouro = root.ElementAnyNs("LogradouroTomador").GetValue<string>();
            ret.Tomador.Endereco.Numero = root.ElementAnyNs("NumeroEnderecoTomador").GetValue<string>();
            ret.Tomador.Endereco.Complemento = root.ElementAnyNs("ComplementoEnderecoTomador").GetValue<string>();
            ret.Tomador.Endereco.TipoBairro = root.ElementAnyNs("TipoBairroTomador").GetValue<string>();
            ret.Tomador.Endereco.Bairro = root.ElementAnyNs("BairroTomador").GetValue<string>();
            ret.Tomador.Endereco.CodigoMunicipio = root.ElementAnyNs("CidadeTomador").GetValue<int>();
            ret.Tomador.Endereco.Municipio = root.ElementAnyNs("CidadeTomadorDescricao").GetValue<string>();
            ret.Tomador.Endereco.Cep = root.ElementAnyNs("CEPTomador").GetValue<string>();
            ret.Tomador.DadosContato.Email = root.ElementAnyNs("EmailTomador").GetValue<string>();
            ret.Tomador.DadosContato.DDD = root.ElementAnyNs("DDDTomador").GetValue<string>();
            ret.Tomador.DadosContato.Telefone = root.ElementAnyNs("TelefoneTomador").GetValue<string>();

            // Dados NFSe
            ret.IdentificacaoNFSe.Numero = root.ElementAnyNs("NumeroNota").GetValue<string>();
            ret.IdentificacaoNFSe.DataEmissao = root.ElementAnyNs("DataProcessamento").GetValue<DateTime>();
            ret.NumeroLote = root.ElementAnyNs("NumeroLote").GetValue<int>();
            ret.IdentificacaoNFSe.Chave = root.ElementAnyNs("CodigoVerificacao").GetValue<string>();

            //RPS
            ret.IdentificacaoRps.Numero = root.ElementAnyNs("NumeroRPS").GetValue<string>();
            ret.IdentificacaoRps.DataEmissao = root.ElementAnyNs("DataEmissaoRPS").GetValue<DateTime>();
            ret.IdentificacaoRps.SeriePrestacao = root.ElementAnyNs("SeriePrestacao").GetValue<string>();

            // RPS Substituido
            ret.RpsSubstituido.Serie = root.ElementAnyNs("SerieRPSSubstituido").GetValue<string>();
            ret.RpsSubstituido.NumeroRps = root.ElementAnyNs("NumeroRPSSubstituido").GetValue<string>();
            ret.RpsSubstituido.NumeroNfse = root.ElementAnyNs("NumeroNFSeSubstituida").GetValue<string>();
            ret.RpsSubstituido.DataEmissaoNfseSubstituida = root.ElementAnyNs("DataEmissaoNFSeSubstituida").GetValue<DateTime>();

            // Servico
            ret.Servico.CodigoCnae = root.ElementAnyNs("CodigoAtividade").GetValue<string>();
            ret.Servico.Valores.Aliquota = root.ElementAnyNs("AliquotaAtividade").GetValue<decimal>();
            ret.Servico.Valores.IssRetido = root.ElementAnyNs("TipoRecolhimento").GetValue<char>() == 'A' ? SituacaoTributaria.Normal : SituacaoTributaria.Retencao;
            ret.Servico.CodigoMunicipio = root.ElementAnyNs("MunicipioPrestacao").GetValue<int>();
            ret.Servico.Municipio = root.ElementAnyNs("MunicipioPrestacaoDescricao").GetValue<string>();

            ret.NaturezaOperacao = root.ElementAnyNs("Operacao").GetValue<char>();

            switch (root.ElementAnyNs("Tributacao").GetValue<char>())
            {
                case 'C':
                    ret.TipoTributacao = TipoTributacao.Isenta;
                    break;

                case 'F':
                    ret.TipoTributacao = TipoTributacao.Imune;
                    break;

                case 'K':
                    ret.TipoTributacao = TipoTributacao.DepositoEmJuizo;
                    break;

                case 'E':
                    ret.TipoTributacao = TipoTributacao.NaoIncide;
                    break;

                case 'N':
                    ret.TipoTributacao = TipoTributacao.NaoTributavel;
                    break;

                case 'G':
                    ret.TipoTributacao = TipoTributacao.TributavelFixo;
                    break;

                case 'H':
                    ret.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
                    ret.TipoTributacao = TipoTributacao.Tributavel;
                    break;

                case 'M':
                    ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresaMunicipal;
                    ret.TipoTributacao = TipoTributacao.Tributavel;
                    break;

                //Tributavel
                default:
                    ret.TipoTributacao = TipoTributacao.Tributavel;
                    break;
            }

            ret.Servico.Valores.ValorPis = root.ElementAnyNs("ValorPIS").GetValue<decimal>();
            ret.Servico.Valores.ValorCofins = root.ElementAnyNs("ValorCOFINS").GetValue<decimal>();
            ret.Servico.Valores.ValorInss = root.ElementAnyNs("ValorINSS").GetValue<decimal>();
            ret.Servico.Valores.ValorIr = root.ElementAnyNs("ValorIR").GetValue<decimal>();
            ret.Servico.Valores.ValorCsll = root.ElementAnyNs("ValorCSLL").GetValue<decimal>();
            ret.Servico.Valores.AliquotaPis = root.ElementAnyNs("AliquotaPIS").GetValue<decimal>();
            ret.Servico.Valores.AliquotaCofins = root.ElementAnyNs("AliquotaCOFINS").GetValue<decimal>();
            ret.Servico.Valores.AliquotaInss = root.ElementAnyNs("AliquotaINSS").GetValue<decimal>();
            ret.Servico.Valores.AliquotaIR = root.ElementAnyNs("AliquotaIR").GetValue<decimal>();
            ret.Servico.Valores.AliquotaCsll = root.ElementAnyNs("AliquotaCSLL").GetValue<decimal>();
            ret.Servico.Descricao = root.ElementAnyNs("DescricaoRPS").GetValue<string>();

            //Outros
            ret.Cancelamento.MotivoCancelamento = root.ElementAnyNs("MotCancelamento").GetValue<string>();

            //Deduções
            var deducoes = root.ElementAnyNs("Deducoes");
            if (deducoes != null && deducoes.HasElements)
            {
                foreach (var node in deducoes.Descendants())
                {
                    var deducaoRoot = node.ElementAnyNs("Deducao");
                    var deducao = ret.Servico.Deducoes.AddNew();
                    deducao.DeducaoPor = (DeducaoPor)Enum.Parse(typeof(DeducaoPor), deducaoRoot.ElementAnyNs("DeducaoPor").GetValue<string>());
                    deducao.TipoDeducao = deducaoRoot.ElementAnyNs("TipoDeducao").GetValue<string>().ToEnum(
                        new[] { "", "Despesas com Materiais", "Despesas com Mercadorias", "Despesas com Subempreitada",
                                "Servicos de Veiculacao e Divulgacao", "Mapa de Const. Civil", "Servicos" },
                        new[] { TipoDeducao.Nenhum, TipoDeducao.Materiais, TipoDeducao.Mercadorias, TipoDeducao.SubEmpreitada,
                                TipoDeducao.VeiculacaoeDivulgacao, TipoDeducao.MapadeConstCivil, TipoDeducao.Servicos });
                    deducao.CPFCNPJReferencia = deducaoRoot.ElementAnyNs("CPFCNPJReferencia").GetValue<string>();
                    deducao.NumeroNFReferencia = deducaoRoot.ElementAnyNs("NumeroNFReferencia").GetValue<int?>();
                    deducao.ValorTotalReferencia = deducaoRoot.ElementAnyNs("ValorTotalReferencia").GetValue<decimal>();
                    deducao.PercentualDeduzir = deducaoRoot.ElementAnyNs("PercentualDeduzir").GetValue<decimal>();
                    deducao.ValorDeduzir = deducaoRoot.ElementAnyNs("ValorDeduzir").GetValue<decimal>();
                }
            }

            //Serviços
            var servicos = root.ElementAnyNs("Itens");
            if (servicos != null && servicos.HasElements)
            {
                foreach (var node in servicos.Descendants())
                {
                    var servicoRoot = node.ElementAnyNs("Item");
                    var servico = ret.Servico.ItensServico.AddNew();
                    servico.Descricao = servicoRoot.ElementAnyNs("DiscriminacaoServico").GetValue<string>();
                    servico.Quantidade = servicoRoot.ElementAnyNs("Quantidade").GetValue<decimal>();
                    servico.ValorServicos = servicoRoot.ElementAnyNs("ValorUnitario").GetValue<decimal>();
                    servico.Tributavel = servicoRoot.ElementAnyNs("Tributavel").GetValue<string>() == "S" ? NFSeSimNao.Sim : NFSeSimNao.Nao;
                }
            }

            return ret;
        }

        public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
        {
            GerarCampos(nota);

            var xmldoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            var rpsTag = new XElement("RPS", new XAttribute("Id", $"rps:{nota.Id}"));
            xmldoc.Add(rpsTag);

            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "Assinatura", 1, 2000, Ocorrencia.Obrigatoria, assinatura));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipalPrestador", 01, Municipio.TamanhoIm, Ocorrencia.Obrigatoria, nota.Prestador.InscricaoMunicipal.OnlyNumbers()));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocialPrestador", 1, 120, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Prestador.RazaoSocial.RemoveAccent() : nota.Prestador.RazaoSocial));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoRPS", 1, 20, Ocorrencia.Obrigatoria, "RPS"));

            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "SerieRPS", 01, 2, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie.IsEmpty() ? "NF" : nota.IdentificacaoRps.Serie));

            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroRPS", 1, 12, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
            rpsTag.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissaoRPS", 1, 21, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "SituacaoRPS", 1, 1, Ocorrencia.Obrigatoria, situacao));

            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "SerieRPSSubstituido", 0, 2, Ocorrencia.NaoObrigatoria, nota.RpsSubstituido.Serie));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroRPSSubstituido", 0, 2, Ocorrencia.NaoObrigatoria, nota.RpsSubstituido.NumeroRps));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroNFSeSubstituida", 0, 2, Ocorrencia.NaoObrigatoria, nota.RpsSubstituido.NumeroNfse));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Dat, "", "DataEmissaoNFSeSubstituida", 0, 2, Ocorrencia.NaoObrigatoria, nota.RpsSubstituido.DataEmissaoNfseSubstituida));

            rpsTag.AddChild(AdicionarTag(TipoCampo.Int, "", "SeriePrestacao", 01, 02, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.SeriePrestacao.IsEmpty() ? "99" : nota.IdentificacaoRps.SeriePrestacao.OnlyNumbers()));

            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipalTomador", 1, Municipio.TamanhoIm, Ocorrencia.Obrigatoria, nota.Tomador.InscricaoMunicipal.OnlyNumbers()));
            rpsTag.AddChild(AdicionarTagCNPJCPF("", "CPFCNPJTomador", "CPFCNPJTomador", nota.Tomador.CpfCnpj));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocialTomador", 1, 120, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Tomador.RazaoSocial.RemoveAccent() : nota.Tomador.RazaoSocial));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "DocTomadorEstrangeiro", 0, 20, Ocorrencia.Obrigatoria, nota.Tomador.DocTomadorEstrangeiro));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoLogradouroTomador", 0, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.TipoLogradouro));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "LogradouroTomador", 1, 50, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Tomador.Endereco.Logradouro.RemoveAccent() : nota.Tomador.Endereco.Logradouro));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroEnderecoTomador", 1, 9, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Numero));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "ComplementoEnderecoTomador", 1, 30, Ocorrencia.NaoObrigatoria, RetirarAcentos ? nota.Tomador.Endereco.Complemento.RemoveAccent() : nota.Tomador.Endereco.Complemento));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoBairroTomador", 0, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.TipoBairro));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "BairroTomador", 1, 50, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Tomador.Endereco.Bairro.RemoveAccent() : nota.Tomador.Endereco.Bairro));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "CidadeTomador", 1, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.CodigoMunicipio));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "CidadeTomadorDescricao", 1, 50, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Tomador.Endereco.Municipio.RemoveAccent() : nota.Tomador.Endereco.Municipio));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "CEPTomador", 1, 8, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Cep.OnlyNumbers()));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "EmailTomador", 1, 60, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.Email));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoAtividade", 1, 9, Ocorrencia.Obrigatoria, nota.Servico.CodigoCnae));
            rpsTag.AddChild(AdicionarTag(TipoCampo.De2, "", "AliquotaAtividade", 1, 11, Ocorrencia.Obrigatoria, nota.Servico.Valores.Aliquota));

            //valores serviço
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoRecolhimento", 01, 01, Ocorrencia.Obrigatoria, recolhimento));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "MunicipioPrestacao", 1, 10, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio.ZeroFill(Municipio.TamanhoIm)));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "MunicipioPrestacaoDescricao", 01, 30, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Servico.Municipio.RemoveAccent() : nota.Servico.Municipio));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "Operacao", 01, 01, Ocorrencia.Obrigatoria, operacao));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "Tributacao", 01, 01, Ocorrencia.Obrigatoria, tributacao));

            //Valores
            rpsTag.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPIS", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorPis));
            rpsTag.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCOFINS", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorCofins));
            rpsTag.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorINSS", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorInss));
            rpsTag.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIR", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorIr));
            rpsTag.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCSLL", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorCsll));

            //Aliquotas
            rpsTag.AddChild(AdicionarTag(TipoCampo.De4, "", "AliquotaPIS", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaPis));
            rpsTag.AddChild(AdicionarTag(TipoCampo.De4, "", "AliquotaCOFINS", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaCofins));
            rpsTag.AddChild(AdicionarTag(TipoCampo.De4, "", "AliquotaINSS", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaInss));
            rpsTag.AddChild(AdicionarTag(TipoCampo.De4, "", "AliquotaIR", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaIR));
            rpsTag.AddChild(AdicionarTag(TipoCampo.De4, "", "AliquotaCSLL", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaCsll));

            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "DescricaoRPS", 1, 1500, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Servico.Descricao.RemoveAccent() : nota.Servico.Descricao));

            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "DDDPrestador", 0, 3, Ocorrencia.Obrigatoria, nota.Prestador.DadosContato.DDD.OnlyNumbers()));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "TelefonePrestador", 0, 8, Ocorrencia.Obrigatoria, nota.Prestador.DadosContato.Telefone.OnlyNumbers()));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "DDDTomador", 0, 03, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.DDD.OnlyNumbers()));
            rpsTag.AddChild(AdicionarTag(TipoCampo.Str, "", "TelefoneTomador", 0, 8, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.Telefone.OnlyNumbers()));

            if (!nota.Intermediario.CpfCnpj.IsEmpty())
                rpsTag.AddChild(AdicionarTagCNPJCPF("", "CPFCNPJIntermediario", "CPFCNPJIntermediario", nota.Intermediario.CpfCnpj));

            rpsTag.AddChild(GerarServicos(nota.Servico.ItensServico));
            if (nota.Servico.Deducoes.Count > 0)
                rpsTag.AddChild(GerarDeducoes(nota.Servico.Deducoes));

            return xmldoc.Root.AsString(identado, showDeclaration, Encoding.UTF8);
        }

        public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true)
        {
            GerarCampos(nota);

            var xmldoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            var notaTag = new XElement("Nota");
            xmldoc.Add(notaTag);

            notaTag.AddChild(AdicionarTag(TipoCampo.Int, "", "NumeroNota", 1, 11, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.Numero));
            notaTag.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataProcessamento", 1, 21, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.DataEmissao));
            notaTag.AddChild(AdicionarTag(TipoCampo.Int, "", "NumeroLote", 1, 11, Ocorrencia.Obrigatoria, nota.NumeroLote));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoVerificacao", 1, 200, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.Chave));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "Assinatura", 1, 2000, Ocorrencia.Obrigatoria, nota.Assinatura));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipalPrestador", 01, Municipio.TamanhoIm, Ocorrencia.Obrigatoria, nota.Prestador.InscricaoMunicipal.OnlyNumbers()));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocialPrestador", 1, 120, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Prestador.RazaoSocial.RemoveAccent() : nota.Prestador.RazaoSocial));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoRPS", 1, 20, Ocorrencia.Obrigatoria, "RPS"));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "SerieRPS", 01, 02, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie.IsEmpty() ? "NF" : nota.IdentificacaoRps.Serie));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroRPS", 1, 12, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
            notaTag.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissaoRPS", 1, 21, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "SituacaoRPS", 1, 1, Ocorrencia.Obrigatoria, situacao));

            if (!nota.RpsSubstituido.NumeroRps.IsEmpty())
            {
                notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "SerieRPSSubstituido", 0, 2, Ocorrencia.Obrigatoria, "NF"));
                notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroRPSSubstituido", 0, 2, Ocorrencia.Obrigatoria, nota.RpsSubstituido.NumeroRps));
                notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroNFSeSubstituida", 0, 2, Ocorrencia.Obrigatoria, nota.RpsSubstituido.NumeroNfse));
                notaTag.AddChild(AdicionarTag(TipoCampo.Dat, "", "DataEmissaoNFSeSubstituida", 0, 2, Ocorrencia.Obrigatoria, nota.RpsSubstituido.DataEmissaoNfseSubstituida));
            }

            notaTag.AddChild(AdicionarTag(TipoCampo.Int, "", "SeriePrestacao", 1, 2, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.SeriePrestacao.IsEmpty() ? "99" : nota.IdentificacaoRps.SeriePrestacao));

            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipalTomador", 1, Municipio.TamanhoIm, Ocorrencia.Obrigatoria, nota.Tomador.InscricaoMunicipal.OnlyNumbers()));
            notaTag.AddChild(AdicionarTagCNPJCPF("", "CPFCNPJTomador", "CPFCNPJTomador", nota.Tomador.CpfCnpj));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocialTomador", 1, 120, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Tomador.RazaoSocial.RemoveAccent() : nota.Tomador.RazaoSocial));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "DocTomadorEstrangeiro", 0, 20, Ocorrencia.Obrigatoria, nota.Tomador.DocTomadorEstrangeiro));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoLogradouroTomador", 0, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.TipoLogradouro));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "LogradouroTomador", 1, 50, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Tomador.Endereco.Logradouro.RemoveAccent() : nota.Tomador.Endereco.Logradouro));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroEnderecoTomador", 1, 9, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Numero));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "ComplementoEnderecoTomador", 1, 30, Ocorrencia.NaoObrigatoria, RetirarAcentos ? nota.Tomador.Endereco.Complemento.RemoveAccent() : nota.Tomador.Endereco.Complemento));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoBairroTomador", 0, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.TipoBairro));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "BairroTomador", 1, 50, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Tomador.Endereco.Bairro.RemoveAccent() : nota.Tomador.Endereco.Bairro));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "CidadeTomador", 1, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.CodigoMunicipio));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "CidadeTomadorDescricao", 1, 50, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Tomador.Endereco.Municipio.RemoveAccent() : nota.Tomador.Endereco.Municipio));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "CEPTomador", 1, 8, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Cep.OnlyNumbers()));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "EmailTomador", 1, 60, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.Email));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoAtividade", 1, 9, Ocorrencia.Obrigatoria, nota.Servico.CodigoCnae));
            notaTag.AddChild(AdicionarTag(TipoCampo.De2, "", "AliquotaAtividade", 1, 11, Ocorrencia.Obrigatoria, nota.Servico.Valores.Aliquota));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoRecolhimento", 01, 01, Ocorrencia.Obrigatoria, recolhimento));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "MunicipioPrestacao", 01, 10, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio.ZeroFill(7)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "MunicipioPrestacaoDescricao", 01, 30, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Servico.Municipio.RemoveAccent() : nota.Servico.Municipio));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "Operacao", 01, 01, Ocorrencia.Obrigatoria, operacao));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "Tributacao", 01, 01, Ocorrencia.Obrigatoria, tributacao));

            //Valores
            notaTag.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPIS", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorPis));
            notaTag.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCOFINS", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorCofins));
            notaTag.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorINSS", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorInss));
            notaTag.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIR", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorIr));
            notaTag.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCSLL", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorCsll));

            //Aliquotas criar propriedades
            notaTag.AddChild(AdicionarTag(TipoCampo.De4, "", "AliquotaPIS", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaPis));
            notaTag.AddChild(AdicionarTag(TipoCampo.De4, "", "AliquotaCOFINS", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaCofins));
            notaTag.AddChild(AdicionarTag(TipoCampo.De4, "", "AliquotaINSS", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaInss));
            notaTag.AddChild(AdicionarTag(TipoCampo.De4, "", "AliquotaIR", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaIR));
            notaTag.AddChild(AdicionarTag(TipoCampo.De4, "", "AliquotaCSLL", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaCsll));

            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "DescricaoRPS", 1, 1500, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Servico.Descricao.RemoveAccent() : nota.Servico.Descricao));

            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "DDDPrestador", 0, 3, Ocorrencia.Obrigatoria, nota.Prestador.DadosContato.DDD.OnlyNumbers()));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "TelefonePrestador", 0, 8, Ocorrencia.Obrigatoria, nota.Prestador.DadosContato.Telefone.OnlyNumbers()));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "DDDTomador", 0, 03, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.DDD.OnlyNumbers()));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "TelefoneTomador", 0, 8, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.Telefone.OnlyNumbers()));

            if (nota.Situacao == SituacaoNFSeRps.Cancelado)
                notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "MotCancelamento", 1, 80, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Cancelamento.MotivoCancelamento.RemoveAccent() : nota.Cancelamento.MotivoCancelamento));

            if (!nota.Intermediario.CpfCnpj.IsEmpty())
                notaTag.AddChild(AdicionarTagCNPJCPF("", "CPFCNPJIntermediario", "CPFCNPJIntermediario", nota.Intermediario.CpfCnpj));

            notaTag.AddChild(GerarServicos(nota.Servico.ItensServico));
            if (nota.Servico.Deducoes.Count > 0)
                notaTag.AddChild(GerarDeducoes(nota.Servico.Deducoes));

            return xmldoc.Root.AsString(identado, showDeclaration, Encoding.UTF8);
        }

        #endregion Public

        #region Services

        protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            var rpsOrg = notas.OrderBy(x => x.IdentificacaoRps.DataEmissao);
            var valorTotal = notas.Sum(nota => nota.Servico.Valores.ValorServicos);
            var deducaoTotal = notas.Sum(nota => nota.Servico.Valores.ValorDeducoes);

            retornoWebservice.XmlEnvio = GerarEnvEnvio(rpsOrg.First().IdentificacaoRps.DataEmissao,
                rpsOrg.Last().IdentificacaoRps.DataEmissao, notas.Count, valorTotal, deducaoTotal, retornoWebservice.Lote.ToString());

            var xmlNotas = new StringBuilder();
            foreach (var nota in notas)
            {
                var xmlRps = WriteXmlRps(nota, false, false);
                xmlNotas.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }

            retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.SafeReplace("%NOTAS%", xmlNotas.ToString());
        }

        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "ns1:ReqEnvioLoteRPS", "Lote", Certificado, true);
        }

        protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var cabecalho = xmlRet.ElementAnyNs("Cabecalho");

            retornoWebservice.Sucesso = cabecalho?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;
            retornoWebservice.Lote = cabecalho?.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;
            retornoWebservice.Data = cabecalho?.ElementAnyNs("DataEnvioLote")?.GetValue<DateTime>() ?? DateTime.MinValue;

            var erros = xmlRet.ElementAnyNs("Erros");
            retornoWebservice.Erros.AddRange(ProcessarEventos(TipoEvento.Erros, erros));

            var alertas = xmlRet.ElementAnyNs("Alertas");
            retornoWebservice.Alertas.AddRange(ProcessarEventos(TipoEvento.Alertas, alertas));

            if (!retornoWebservice.Sucesso) return;

            foreach (var nota in notas)
            {
                nota.NumeroLote = retornoWebservice.Lote;
            }
        }

        protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            var rpsOrg = notas.OrderBy(x => x.IdentificacaoRps.DataEmissao);
            var valorTotal = notas.Sum(nota => nota.Servico.Valores.ValorServicos);
            var deducaoTotal = notas.Sum(nota => nota.Servico.Valores.ValorDeducoes);

            retornoWebservice.XmlEnvio = GerarEnvEnvio(rpsOrg.First().IdentificacaoRps.DataEmissao,
                rpsOrg.Last().IdentificacaoRps.DataEmissao, notas.Count, valorTotal, deducaoTotal, retornoWebservice.Lote.ToString());

            var xmlNotas = new StringBuilder();

            // ReSharper disable once SuggestVarOrType_SimpleTypes
            foreach (NotaServico nota in notas)
            {
                var xmlRps = WriteXmlRps(nota, false, false);
                xmlNotas.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }

            retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.SafeReplace("%NOTAS%", xmlNotas.ToString());
        }

        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "ns1:ReqEnvioLoteRPS", "Lote", Certificado, true);
        }

        protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var cabecalho = xmlRet.ElementAnyNs("Cabecalho");

            retornoWebservice.Sucesso = cabecalho?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;
            retornoWebservice.Lote = cabecalho?.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;
            retornoWebservice.Data = cabecalho?.ElementAnyNs("DataEnvioLote")?.GetValue<DateTime>() ?? DateTime.MinValue;

            var erros = xmlRet.ElementAnyNs("Erros");
            retornoWebservice.Erros.AddRange(ProcessarEventos(TipoEvento.Erros, erros));

            var alertas = xmlRet.ElementAnyNs("Alertas");
            retornoWebservice.Alertas.AddRange(ProcessarEventos(TipoEvento.Alertas, alertas));

            if (!retornoWebservice.Sucesso) return;

            // ReSharper disable once SuggestVarOrType_SimpleTypes
            foreach (NotaServico nota in notas)
            {
                nota.NumeroLote = retornoWebservice.Lote;
            }

            var nfseRps = ProcessarEventos(TipoEvento.ListNFSeRps, xmlRet.Element("ChavesNFSeRPS"));
            if (nfseRps == null) return;

            foreach (var nfse in nfseRps)
            {
                var numeroRps = nfse.IdentificacaoRps.Numero;
                var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
                if (nota == null) continue;

                nota.IdentificacaoNFSe.Numero = nfse.IdentificacaoNfse.Numero;
                nota.IdentificacaoNFSe.Chave = nfse.IdentificacaoNfse.Chave;

                var xmlNFSe = WriteXmlNFSe(nota);
                GravarNFSeEmDisco(xmlNFSe, $"NFSe-{nota.IdentificacaoNFSe.Chave}-{nota.IdentificacaoNFSe.Numero}.xml", nota.IdentificacaoNFSe.DataEmissao);
            }
        }

        protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<ns1:ReqConsultaLote xmlns:ns1=\"http://localhost:8080/WsNFe2/lote\" xmlns:tipos=\"http://localhost:8080/WsNFe2/tp\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://localhost:8080/WsNFe2/lote  http://localhost:8080/WsNFe2/xsd/ReqConsultaLote.xsd\">");
            loteBuilder.Append("<Cabecalho>");
            loteBuilder.Append($"<CodCidade>{Municipio.CodigoSiafi}</CodCidade>");
            loteBuilder.Append($"<CPFCNPJRemetente>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</CPFCNPJRemetente>");
            loteBuilder.Append("<Versao>1</Versao>");
            loteBuilder.Append($"<NumeroLote>{retornoWebservice.Lote}</NumeroLote>");
            loteBuilder.Append("</Cabecalho>");
            loteBuilder.Append("</ns1:ReqConsultaLote>");

            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            //Não precisa assinar.
        }

        protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var cabecalho = xmlRet.ElementAnyNs("Cabecalho");

            retornoWebservice.Sucesso = cabecalho?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;
            retornoWebservice.Lote = cabecalho?.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;

            var erros = xmlRet.ElementAnyNs("Erros");
            retornoWebservice.Erros.AddRange(ProcessarEventos(TipoEvento.Erros, erros));

            var alertas = xmlRet.ElementAnyNs("Alertas");
            retornoWebservice.Alertas.AddRange(ProcessarEventos(TipoEvento.Alertas, alertas));

            var nfses = xmlRet.ElementAnyNs("ListaNFSe");
            if (nfses == null) return;

            var notasServico = new List<NotaServico>();

            foreach (var nfse in nfses.ElementsAnyNs("ConsultaNFSe"))
            {
                var numeroRps = nfse.ElementAnyNs("NumeroRPS")?.GetValue<string>() ?? string.Empty;
                var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
                if (nota == null) continue;

                nota.IdentificacaoNFSe.Numero = nfse.ElementAnyNs("NumeroNFe")?.GetValue<string>() ?? string.Empty;
                nota.IdentificacaoNFSe.Chave = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;

                var xml = WriteXmlNFSe(nota);
                GravarNFSeEmDisco(xml, $"NFSe-{nota.IdentificacaoNFSe.Chave}-{nota.IdentificacaoNFSe.Numero}.xml", nota.IdentificacaoNFSe.DataEmissao);
                notasServico.Add(nota);
            }

            retornoWebservice.Notas = notasServico.ToArray();
        }

        protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            var lote = new StringBuilder();
            lote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            lote.Append("<ns1:ConsultaSeqRps xmlns:ns1=\"http://localhost:8080/WsNFe2/lote\" xmlns:tipos=\"http://localhost:8080/WsNFe2/tp\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://localhost:8080/WsNFe2/lote http://localhost:8080/WsNFe2/xsd/ConsultaSeqRps.xsd\">");
            lote.Append("<Cabecalho>");
            lote.Append($"<CodCid>{Municipio.CodigoSiafi}</CodCid>");
            lote.Append($"<IMPrestador>{Configuracoes.PrestadorPadrao.InscricaoMunicipal.ZeroFill(Municipio.TamanhoIm)}</IMPrestador>");
            lote.Append($"<CPFCNPJRemetente>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</CPFCNPJRemetente>");
            lote.Append($"<SeriePrestacao>{retornoWebservice.Serie}</SeriePrestacao>");
            lote.Append("<Versao>1</Versao>");
            lote.Append("</Cabecalho>");
            lote.Append("</ns1:ConsultaSeqRps>");
            retornoWebservice.XmlEnvio = lote.ToString();
        }

        protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            //Não precisa assinar.
        }

        protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var cabecalho = xmlRet.ElementAnyNs("Cabecalho");

            retornoWebservice.Sucesso = true;
            retornoWebservice.UltimoNumero = cabecalho?.ElementAnyNs("NroUltimoRps")?.GetValue<int>() ?? 0;

            var erros = xmlRet.ElementAnyNs("Erros");
            retornoWebservice.Erros.AddRange(ProcessarEventos(TipoEvento.Erros, erros));

            var alertas = xmlRet.ElementAnyNs("Alertas");
            retornoWebservice.Alertas.AddRange(ProcessarEventos(TipoEvento.Alertas, alertas));
        }

        protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            var lote = new StringBuilder();
            lote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            lote.Append("<ns1:ReqConsultaNFSeRPS xmlns:ns1=\"http://localhost:8080/WsNFe2/lote\" xmlns:tipos=\"http://localhost:8080/WsNFe2/tp\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://localhost:8080/WsNFe2/lote http://localhost:8080/WsNFe2/xsd/ReqConsultaNFSeRPS.xsd\">");
            lote.Append("<Cabecalho>");
            lote.Append($"<CodCidade>{Municipio.CodigoSiafi}</CodCidade>");
            lote.Append($"<CPFCNPJRemetente>{Configuracoes.PrestadorPadrao.CpfCnpj.OnlyNumbers().ZeroFill(14)}</CPFCNPJRemetente>");
            lote.Append("<transacao>true</transacao>");
            lote.Append("<Versao>1</Versao>");
            lote.Append("</Cabecalho>");
            lote.Append($"<Lote Id=\"{retornoWebservice.NumeroRps}\">");

            if (notas.Count(x => !x.IdentificacaoNFSe.Numero.IsEmpty()) > 1)
            {
                lote.Append("<NotaConsulta>");
                foreach (var nota in notas.Where(x => !x.IdentificacaoNFSe.Numero.IsEmpty()))
                {
                    lote.Append($"<Nota Id=\"nota:{nota.IdentificacaoNFSe.Numero}\">");
                    lote.Append($"<InscricaoMunicipalPrestador>{nota.Prestador.InscricaoMunicipal.OnlyNumbers().ZeroFill(Municipio.TamanhoIm)}</InscricaoMunicipalPrestador>");
                    lote.Append($"<NumeroNota >{nota.IdentificacaoNFSe.Numero}</NumeroNota >");
                    lote.Append($"<CodigoVerificacao>{nota.IdentificacaoNFSe.Chave}</CodigoVerificacao>");
                    lote.Append("</Nota>");
                }

                lote.Append("</NotaConsulta>");
            }

            if (notas.Count(x => x.IdentificacaoNFSe.Numero.IsEmpty()) > 1)
            {
                lote.Append("<RPSConsulta>");

                foreach (var nota in notas.Where(x => x.IdentificacaoNFSe.Numero.IsEmpty()))
                {
                    lote.Append($"<RPS Id=\"rps:{nota.IdentificacaoRps.Numero}\">");
                    lote.Append($"<InscricaoMunicipalPrestador>{nota.Prestador.InscricaoMunicipal.OnlyNumbers().ZeroFill(Municipio.TamanhoIm)}</InscricaoMunicipalPrestador>");
                    lote.Append($"<NumeroRPS>{nota.IdentificacaoRps.Numero}</NumeroRPS>");
                    lote.Append($"<SeriePrestacao>{nota.IdentificacaoRps.SeriePrestacao}</SeriePrestacao>");
                    lote.Append("</RPS>");
                }

                lote.Append("</RPSConsulta>");
            }

            lote.Append("</Lote>");
            lote.Append("</ns1:ReqConsultaNFSeRPS>");
            retornoWebservice.XmlEnvio = lote.ToString();
        }

        protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "ns1:ReqConsultaNFSeRPS", "Lote", Certificado, true);
        }

        protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            var erros = xmlRet.ElementAnyNs("Erros");
            retornoWebservice.Erros.AddRange(ProcessarEventos(TipoEvento.Erros, erros));

            var alertas = xmlRet.ElementAnyNs("Alertas");
            retornoWebservice.Alertas.AddRange(ProcessarEventos(TipoEvento.Alertas, alertas));

            retornoWebservice.Sucesso = !retornoWebservice.Erros.Any();

            var retNotas = new NotaServico[0];
            var notasXml = xmlRet.ElementAnyNs("NotasConsultadas");
            if (notasXml != null && notasXml.HasElements)
                retNotas = notasXml.ElementsAnyNs("Nota").Select(element => LoadXml(element.AsString())).ToArray();

            if (!retNotas.Any()) return;

            notas.Clear();
            notas.AddRange(retNotas);
        }

        protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            var lote = new StringBuilder();
            lote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            lote.Append(
                "<ns1:ReqConsultaNotas xmlns:ns1=\"http://localhost:8080/WsNFe2/lote\" xmlns:tipos=\"http://localhost:8080/WsNFe2/tp\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://localhost:8080/WsNFe2/lote http://localhost:8080/WsNFe2/xsd/ReqConsultaNotas.xsd\">");
            lote.Append("<Cabecalho Id=\"Consulta:notas\">");
            lote.Append($"<CodCidade>{Municipio.CodigoSiafi}</CodCidade>");
            lote.Append($"<CPFCNPJRemetente>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</CPFCNPJRemetente>");
            lote.Append($"<InscricaoMunicipalPrestador>{Configuracoes.PrestadorPadrao.InscricaoMunicipal.ZeroFill(Municipio.TamanhoIm)}</InscricaoMunicipalPrestador>");
            lote.Append($"<dtInicio>{retornoWebservice.Inicio:yyyy-MM-dd}</dtInicio>");
            lote.Append($"<dtFim>{retornoWebservice.Fim:yyyy-MM-dd}</dtFim>");
            lote.Append($"<NotaInicial>{retornoWebservice.NumeroNFse}</NotaInicial>");
            lote.Append("<Versao>1</Versao>");
            lote.Append("</Cabecalho>");
            lote.Append("</ns1:ReqConsultaNotas>");
            retornoWebservice.XmlEnvio = lote.ToString();
        }

        protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "ns1:ReqConsultaNotas", "Cabecalho", Certificado, true);
        }

        protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            var erros = xmlRet.ElementAnyNs("Erros");
            retornoWebservice.Erros.AddRange(ProcessarEventos(TipoEvento.Erros, erros));

            retornoWebservice.Sucesso = !retornoWebservice.Erros.Any();

            var notasXml = xmlRet.ElementAnyNs("Notas");
            if (notasXml == null || !notasXml.HasElements) return;

            retornoWebservice.Notas = notasXml.ElementsAnyNs("Nota").Select(element => LoadXml(element.AsString())).ToArray();
            notas.AddRange(retornoWebservice.Notas);
        }

        protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            var loteCancelamento = new StringBuilder();
            loteCancelamento.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteCancelamento.Append("<ns1:ReqCancelamentoNFSe xmlns:ns1=\"http://localhost:8080/WsNFe2/lote\" xmlns:tipos=\"http://localhost:8080/WsNFe2/tp\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://localhost:8080/WsNFe2/lote http://localhost:8080/WsNFe2/xsd/ReqCancelamentoNFSe.xsd\">");
            loteCancelamento.Append("<Cabecalho>");
            loteCancelamento.Append($"<CodCidade>{Municipio.CodigoSiafi}</CodCidade>");
            loteCancelamento.Append($"<CPFCNPJRemetente>{Configuracoes.PrestadorPadrao.CpfCnpj.OnlyNumbers().ZeroFill(14)}</CPFCNPJRemetente>");
            loteCancelamento.Append("<transacao>true</transacao>");
            loteCancelamento.Append("<Versao>1</Versao>");
            loteCancelamento.Append("</Cabecalho>");
            loteCancelamento.Append("<Lote Id=\"lote:1\">"); //Checar se o numero do lote é necessario ou pode ser sempre o mesmo.

            loteCancelamento.Append($"<Nota Id=\"nota:{retornoWebservice.NumeroNFSe}\">");
            loteCancelamento.Append($"<InscricaoMunicipalPrestador>{Configuracoes.PrestadorPadrao.InscricaoMunicipal.OnlyNumbers()}</InscricaoMunicipalPrestador>");
            loteCancelamento.Append($"<NumeroNota>{retornoWebservice.NumeroNFSe}</NumeroNota>");
            loteCancelamento.Append($"<CodigoVerificacao>{retornoWebservice.CodigoCancelamento}</CodigoVerificacao>");
            loteCancelamento.Append($"<MotivoCancelamento>{retornoWebservice.Motivo}</MotivoCancelamento>");
            loteCancelamento.Append("</Nota>");

            loteCancelamento.Append("</Lote>");
            loteCancelamento.Append("</ns1:ReqCancelamentoNFSe>");

            retornoWebservice.XmlEnvio = loteCancelamento.ToString();
        }

        protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "ns1:ReqCancelamentoNFSe", "Lote", Certificado, true);
        }

        protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var cabecalho = xmlRet.ElementAnyNs("Cabecalho");

            retornoWebservice.Sucesso = cabecalho?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;
            retornoWebservice.Data = cabecalho?.ElementAnyNs("DataEnvioLote")?.GetValue<DateTime>() ?? DateTime.MinValue;

            var erros = xmlRet.ElementAnyNs("Erros");
            retornoWebservice.Erros.AddRange(ProcessarEventos(TipoEvento.Erros, erros));

            var alertas = xmlRet.ElementAnyNs("Alertas");
            retornoWebservice.Alertas.AddRange(ProcessarEventos(TipoEvento.Alertas, alertas));

            var notasCanceladas = xmlRet.ElementAnyNs("NotasCanceladas");
            if (notasCanceladas == null) return;

            foreach (var notaCancelada in notasCanceladas.ElementsAnyNs("Nota"))
            {
                var numeroRps = notaCancelada.ElementAnyNs("NumeroNota")?.GetValue<string>() ?? string.Empty;
                var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == numeroRps.Trim());
                if (nota == null) continue;

                nota.Situacao = SituacaoNFSeRps.Cancelado;
                nota.Cancelamento.MotivoCancelamento = notaCancelada.ElementAnyNs("MotivoCancelamento")?.GetValue<string>() ?? string.Empty;
                nota.IdentificacaoNFSe.Chave = notaCancelada.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;

                var xmlNFSe = WriteXmlNFSe(nota);
                GravarNFSeEmDisco(xmlNFSe, $"NFSe-{nota.IdentificacaoNFSe.Chave}-{nota.IdentificacaoNFSe.Numero}-Canc.xml", nota.IdentificacaoNFSe.DataEmissao);
            }
        }

        protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
        {
            var loteCancelamento = new StringBuilder();
            loteCancelamento.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteCancelamento.Append("<ns1:ReqCancelamentoNFSe xmlns:ns1=\"http://localhost:8080/WsNFe2/lote\" xmlns:tipos=\"http://localhost:8080/WsNFe2/tp\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://localhost:8080/WsNFe2/lote http://localhost:8080/WsNFe2/xsd/ReqCancelamentoNFSe.xsd\">");
            loteCancelamento.Append("<Cabecalho>");
            loteCancelamento.Append($"<CodCidade>{Municipio.CodigoSiafi}</CodCidade>");
            loteCancelamento.Append($"<CPFCNPJRemetente>{Configuracoes.PrestadorPadrao.CpfCnpj.OnlyNumbers().ZeroFill(14)}</CPFCNPJRemetente>");
            loteCancelamento.Append("<transacao>true</transacao>");
            loteCancelamento.Append("<Versao>1</Versao>");
            loteCancelamento.Append("</Cabecalho>");
            loteCancelamento.Append($"<Lote Id=\"lote:{retornoWebservice.Lote}\">");

            foreach (var nota in notas)
            {
                var numeroNota = nota.IdentificacaoNFSe.Numero.Trim();
                loteCancelamento.Append($"<Nota Id=\"nota:{numeroNota}\">");
                loteCancelamento.Append($"<InscricaoMunicipalPrestador>{nota.Prestador.InscricaoMunicipal.OnlyNumbers()}</InscricaoMunicipalPrestador>");
                loteCancelamento.Append($"<NumeroNota>{numeroNota}</NumeroNota>");
                loteCancelamento.Append($"<CodigoVerificacao>{nota.IdentificacaoNFSe.Chave}</CodigoVerificacao>");
                loteCancelamento.Append($"<MotivoCancelamento>{nota.Cancelamento.MotivoCancelamento}</MotivoCancelamento>");
                loteCancelamento.Append("</Nota>");
            }

            loteCancelamento.Append("</Lote>");
            loteCancelamento.Append("</ns1:ReqCancelamentoNFSe>");

            retornoWebservice.XmlEnvio = loteCancelamento.ToString();
        }

        protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "ns1:ReqCancelamentoNFSe", "Lote", Certificado, true);
        }

        protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var cabecalho = xmlRet.ElementAnyNs("Cabecalho");

            retornoWebservice.Sucesso = cabecalho?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;
            retornoWebservice.Lote = cabecalho?.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;

            var erros = xmlRet.ElementAnyNs("Erros");
            retornoWebservice.Erros.AddRange(ProcessarEventos(TipoEvento.Erros, erros));

            var alertas = xmlRet.ElementAnyNs("Alertas");
            retornoWebservice.Alertas.AddRange(ProcessarEventos(TipoEvento.Alertas, alertas));

            if (!retornoWebservice.Sucesso) return;

            var notasCanceladas = xmlRet.ElementAnyNs("NotasCanceladas");
            if (notasCanceladas == null) return;

            foreach (var notaCancelada in notasCanceladas.ElementsAnyNs("Nota"))
            {
                var numeroRps = notaCancelada.ElementAnyNs("NumeroNota")?.GetValue<string>() ?? string.Empty;
                var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == numeroRps.Trim());
                if (nota == null) continue;

                nota.Situacao = SituacaoNFSeRps.Cancelado;
                nota.Cancelamento.MotivoCancelamento = notaCancelada.ElementAnyNs("MotivoCancelamento")?.GetValue<string>() ?? string.Empty;
                nota.IdentificacaoNFSe.Chave = notaCancelada.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;

                var xmlNFSe = WriteXmlNFSe(nota);
                GravarNFSeEmDisco(xmlNFSe, $"NFSe-{nota.IdentificacaoNFSe.Chave}-{nota.IdentificacaoNFSe.Numero}.xml", nota.IdentificacaoNFSe.DataEmissao);
            }
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

        #region Private

        protected override IServiceClient GetClient(TipoUrl tipo)
        {
            return new DSFServiceClient(this, tipo);
        }

        protected override string GerarCabecalho()
        {
            return "";
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            switch (tipo)
            {
                case TipoUrl.Enviar:
                case TipoUrl.EnviarSincrono:
                    return "ReqEnvioLoteRPS.xsd";

                case TipoUrl.SubstituirNFSe:
                case TipoUrl.ConsultarSituacao:
                    return "";

                case TipoUrl.ConsultarLoteRps:
                    return "ReqConsultaLote.xsd";

                case TipoUrl.ConsultarSequencialRps:
                    return "ConsultaSeqRps.xsd";

                case TipoUrl.ConsultarNFSeRps:
                    return "ReqConsultaNFSeRPS.xsd";

                case TipoUrl.ConsultarNFSe:
                    return "ReqConsultaNotas.xsd";

                case TipoUrl.CancelarNFSeLote:
                case TipoUrl.CancelarNFSe:
                    return "ReqCancelamentoNFSe.xsd";

                default:
                    throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null);
            }
        }

        private string GerarEnvEnvio(DateTime dataIni, DateTime dataFim, int total, decimal valorTotal, decimal valorDeducao, string lote)
        {
            var xmlLote = new StringBuilder();
            xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlLote.Append("<ns1:ReqEnvioLoteRPS xmlns:ns1=\"http://localhost:8080/WsNFe2/lote\" xmlns:tipos=\"http://localhost:8080/WsNFe2/tp\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://localhost:8080/WsNFe2/lote http://localhost:8080/WsNFe2/xsd/ReqEnvioLoteRPS.xsd\">");
            xmlLote.Append("<Cabecalho>");
            xmlLote.Append($"<CodCidade>{Municipio.CodigoSiafi}</CodCidade>");
            xmlLote.Append($"<CPFCNPJRemetente>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</CPFCNPJRemetente>");
            xmlLote.Append($"<RazaoSocialRemetente>{Configuracoes.PrestadorPadrao.RazaoSocial}</RazaoSocialRemetente>");
            xmlLote.Append("<transacao/>");
            xmlLote.Append($"<dtInicio>{dataIni:yyyy-MM-dd}</dtInicio>");
            xmlLote.Append($"<dtFim>{dataFim:yyyy-MM-dd}</dtFim>");
            xmlLote.Append($"<QtdRPS>{total}</QtdRPS>");
            xmlLote.Append($"<ValorTotalServicos>{valorTotal:0.00}</ValorTotalServicos>");
            xmlLote.Append($"<ValorTotalDeducoes>{valorDeducao:0.00}</ValorTotalDeducoes>");
            xmlLote.Append("<Versao>1</Versao>");
            xmlLote.Append("<MetodoEnvio>WS</MetodoEnvio>");
            xmlLote.Append("</Cabecalho>");
            xmlLote.Append($"<Lote Id=\"lote:{lote}\">");
            xmlLote.Append("%NOTAS%");
            xmlLote.Append("</Lote>");
            xmlLote.Append("</ns1:ReqEnvioLoteRPS>");

            return xmlLote.ToString();
        }

        private static IEnumerable<Evento> ProcessarEventos(TipoEvento tipo, XElement eventos)
        {
            var ret = new List<Evento>();
            if (eventos == null) return ret.ToArray();

            string nome;
            switch (tipo)
            {
                case TipoEvento.Erros:
                    nome = "Erro";
                    break;

                case TipoEvento.Alertas:
                    nome = "Alerta";
                    break;

                case TipoEvento.ListNFSeRps:
                    nome = "ChaveNFSeRPS";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null);
            }

            foreach (var evento in eventos.ElementsAnyNs(nome))
            {
                var item = new Evento();

                if (tipo != TipoEvento.ListNFSeRps)
                {
                    item.Codigo = evento.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty;
                    item.Descricao = evento.ElementAnyNs("Descricao")?.GetValue<string>() ?? string.Empty;
                }

                var ideRps = evento.ElementAnyNs("ChaveRPS");
                if (ideRps != null)
                {
                    item.IdentificacaoRps.Numero = ideRps.ElementAnyNs("NumeroRPS")?.GetValue<string>() ?? string.Empty;
                    item.IdentificacaoRps.Serie = ideRps.ElementAnyNs("SerieRPS")?.GetValue<string>() ?? string.Empty;
                    item.IdentificacaoRps.DataEmissao = ideRps.ElementAnyNs("DataEmissaoRPS")?.GetValue<DateTime>() ?? DateTime.MinValue;
                }

                var ideNFSe = evento.ElementAnyNs("ChaveNFe");
                if (ideNFSe != null)
                {
                    item.IdentificacaoNfse.Numero = ideNFSe.ElementAnyNs("NumeroNFe")?.GetValue<string>() ?? string.Empty;
                    item.IdentificacaoNfse.Chave = ideNFSe.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
                }

                ret.Add(item);
            }

            return ret.ToArray();
        }

        private void GerarCampos(NotaServico nota)
        {
            recolhimento = nota.Servico.Valores.IssRetido == SituacaoTributaria.Normal ? "A" : "R";
            situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "N" : "C";
            operacao = $"{(char)nota.NaturezaOperacao}";

            switch (nota.TipoTributacao)
            {
                case TipoTributacao.Isenta:
                    tributacao = "C";
                    break;

                case TipoTributacao.Imune:
                    tributacao = "F";
                    break;

                case TipoTributacao.DepositoEmJuizo:
                    tributacao = "K";
                    break;

                case TipoTributacao.NaoIncide:
                    tributacao = "E";
                    break;

                case TipoTributacao.NaoTributavel:
                    tributacao = "N";
                    break;

                case TipoTributacao.TributavelFixo:
                    tributacao = "G";
                    break;

                //Tributavel
                default:
                    tributacao = "T";
                    break;
            }

            if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
                tributacao = "H";

            if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.MicroEmpresarioIndividual)
                tributacao = "M";

            var valor = nota.Servico.Valores.ValorServicos - nota.Servico.Valores.ValorDeducoes;
            var rec = nota.Servico.Valores.IssRetido == SituacaoTributaria.Normal ? "N" : "S";
            var sign = $"{nota.Prestador.InscricaoMunicipal.ZeroFill(Municipio.TamanhoIm)}{nota.IdentificacaoRps.Serie.FillLeft(5)}" + $"{nota.IdentificacaoRps.Numero.ZeroFill(12)}{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}{tributacao} " + $"{situacao}{rec}{Math.Round(valor * 100).ToString().ZeroFill(15)}" + $"{Math.Round(nota.Servico.Valores.ValorDeducoes * 100).ToString().ZeroFill(15)}" + $"{nota.Servico.CodigoCnae.ZeroFill(10)}{nota.Tomador.CpfCnpj.ZeroFill(14)}";

            assinatura = sign.ToSha1Hash().ToLowerInvariant();
        }

        private XElement GerarServicos(IEnumerable<Servico> servicos)
        {
            var itensTag = new XElement("Itens");

            foreach (var servico in servicos)
            {
                var itemTag = new XElement("Item");
                var sTributavel = servico.Tributavel == NFSeSimNao.Sim ? "S" : "N";
                itemTag.AddChild(AdicionarTag(TipoCampo.Str, "", "DiscriminacaoServico", 1, 80, Ocorrencia.Obrigatoria, RetirarAcentos ? servico.Descricao.RemoveAccent() : servico.Descricao));
                itemTag.AddChild(AdicionarTag(TipoCampo.De4, "", "Quantidade", 1, 15, Ocorrencia.Obrigatoria, servico.Quantidade));
                itemTag.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorUnitario", 1, 20, Ocorrencia.Obrigatoria, servico.ValorUnitario));
                itemTag.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorTotal", 1, 18, Ocorrencia.Obrigatoria, servico.ValorTotal));
                itemTag.AddChild(AdicionarTag(TipoCampo.Str, "", "Tributavel", 1, 1, Ocorrencia.NaoObrigatoria, sTributavel));
                itensTag.AddChild(itemTag);
            }

            return itensTag;
        }

        private XElement GerarDeducoes(IEnumerable<Deducao> deducoes)
        {
            var deducoesTag = new XElement("Deducoes");
            foreach (var deducao in deducoes)
            {
                var deducaoTag = new XElement("Deducao");
                deducaoTag.AddChild(AdicionarTag(TipoCampo.Str, "", "DeducaoPor", 1, 20, Ocorrencia.Obrigatoria, deducao.DeducaoPor.ToString()));
                deducaoTag.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoDeducao", 0, 255, Ocorrencia.Obrigatoria, deducao.TipoDeducao.GetDescription(new[]
                {
                    TipoDeducao.Nenhum, TipoDeducao.Materiais, TipoDeducao.Mercadorias, TipoDeducao.SubEmpreitada, TipoDeducao.VeiculacaoeDivulgacao, TipoDeducao.MapadeConstCivil, TipoDeducao.Servicos
                }, new[]
                {
                    "", "Despesas com Materiais", "Despesas com Mercadorias", "Despesas com Subempreitada", "Servicos de Veiculacao e Divulgacao", "Mapa de Const. Civil", "Servicos"
                })));

                deducaoTag.AddChild(AdicionarTag(TipoCampo.Str, "", "CPFCNPJReferencia", 0, 14, Ocorrencia.Obrigatoria, deducao.CPFCNPJReferencia.OnlyNumbers()));
                deducaoTag.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroNFReferencia", 0, 10, Ocorrencia.Obrigatoria, deducao.NumeroNFReferencia));
                deducaoTag.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorTotalReferencia", 0, 18, Ocorrencia.Obrigatoria, deducao.ValorTotalReferencia));
                deducaoTag.AddChild(AdicionarTag(TipoCampo.De2, "", "PercentualDeduzir", 0, 8, Ocorrencia.Obrigatoria, deducao.PercentualDeduzir));
                deducoesTag.AddChild(deducaoTag);
            }

            return deducoesTag;
        }

        #endregion Private

        #endregion Methods
    }
}