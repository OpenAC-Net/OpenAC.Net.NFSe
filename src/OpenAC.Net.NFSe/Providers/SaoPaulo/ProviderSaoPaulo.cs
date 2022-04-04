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
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
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
    internal sealed class ProviderSaoPaulo : ProviderBase
    {
        #region Constructors

        public ProviderSaoPaulo(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "São Paulo";
        }

        #endregion Constructors

        #region Methods

        #region Xml

        public override NotaServico LoadXml(XDocument xml)
        {
            Guard.Against<XmlException>(xml == null, "Xml invalido.");

            XElement rootDoc = xml.Root;
            Guard.Against<XmlException>(rootDoc == null, "Xml de RPS ou NFSe invalido.");

            var ret = new NotaServico(Configuracoes);
            ret.Assinatura = rootDoc.ElementAnyNs("Assinatura")?.GetValue<string>() ?? string.Empty;

            // Nota Fiscal
            ret.IdentificacaoNFSe.Numero = rootDoc.ElementAnyNs("ChaveNFe")?.ElementAnyNs("NumeroNFe")?.GetValue<string>() ?? string.Empty;
            ret.IdentificacaoNFSe.Chave = rootDoc.ElementAnyNs("ChaveNFe")?.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
            ret.Prestador.InscricaoMunicipal = rootDoc.ElementAnyNs("ChaveNFe")?.ElementAnyNs("InscricaoPrestador")?.GetValue<string>() ?? string.Empty;

            ret.IdentificacaoNFSe.DataEmissao = rootDoc.ElementAnyNs("DataEmissaoNFe")?.GetValue<DateTime>() ?? DateTime.MinValue;
            ret.NumeroLote = rootDoc.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;

            // RPS
            ret.IdentificacaoRps.Numero = rootDoc.ElementAnyNs("ChaveRPS")?.ElementAnyNs("NumeroRPS")?.GetValue<string>() ?? string.Empty;
            ret.IdentificacaoRps.Serie = rootDoc.ElementAnyNs("ChaveRPS")?.ElementAnyNs("SerieRPS")?.GetValue<string>() ?? string.Empty;
            if (ret.Prestador.InscricaoMunicipal == "")
                ret.Prestador.InscricaoMunicipal = rootDoc.ElementAnyNs("ChaveRPS")?.ElementAnyNs("InscricaoPrestador")?.GetValue<string>() ?? string.Empty;

            switch (rootDoc.ElementAnyNs("TipoRPS")?.GetValue<string>() ?? string.Empty)
            {
                case "RPS":
                    ret.IdentificacaoRps.Tipo = TipoRps.RPS;
                    break;

                case "RPS-M":
                    ret.IdentificacaoRps.Tipo = TipoRps.NFConjugada;
                    break;

                case "RPS-C":
                    ret.IdentificacaoRps.Tipo = TipoRps.Cupom;
                    break;
            }

            ret.IdentificacaoRps.DataEmissao = rootDoc.ElementAnyNs("DataEmissaoRPS")?.GetValue<DateTime>() ?? DateTime.MinValue;

            // Tipo da Tributação
            switch (rootDoc.ElementAnyNs("TributacaoNFe")?.GetValue<string>() ?? string.Empty)
            {
                case "T":
                    ret.TipoTributacao = TipoTributacao.Tributavel;
                    break;

                case "F":
                    ret.TipoTributacao = TipoTributacao.ForaMun;
                    break;

                case "A":
                    ret.TipoTributacao = TipoTributacao.Isenta;
                    break;

                case "B":
                    ret.TipoTributacao = TipoTributacao.ForaMunIsento;
                    break;

                case "M":
                    ret.TipoTributacao = TipoTributacao.Imune;
                    break;

                case "N":
                    ret.TipoTributacao = TipoTributacao.ForaMunImune;
                    break;

                case "X":
                    ret.TipoTributacao = TipoTributacao.Suspensa;
                    break;

                case "V":
                    ret.TipoTributacao = TipoTributacao.ForaMunSuspensa;
                    break;

                case "P":
                    ret.TipoTributacao = TipoTributacao.ExpServicos;
                    break;
            }

            switch (rootDoc.ElementAnyNs("StatusNFe")?.GetValue<string>() ?? string.Empty)
            {
                case "N":
                    ret.Situacao = SituacaoNFSeRps.Normal;
                    break;

                case "F":
                    ret.Situacao = SituacaoNFSeRps.Cancelado;
                    break;
            }

            ret.Servico.Discriminacao = rootDoc.ElementAnyNs("Discriminacao")?.GetValue<string>() ?? string.Empty;
            ret.Servico.Valores.ValorServicos = rootDoc.ElementAnyNs("ValorServicos")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.Aliquota = rootDoc.ElementAnyNs("AliquotaServicos")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.ValorIss = rootDoc.ElementAnyNs("ValorISS")?.GetValue<decimal>() ?? 0;
            ret.Servico.ItemListaServico = rootDoc.ElementAnyNs("CodigoServico")?.GetValue<string>() ?? string.Empty;
            ret.Servico.Valores.ValorCargaTributaria = rootDoc.ElementAnyNs("ValorCargaTributaria")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.AliquotaCargaTributaria = rootDoc.ElementAnyNs("PercentualCargaTributaria")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.FonteCargaTributaria = rootDoc.ElementAnyNs("FonteCargaTributaria")?.GetValue<string>() ?? string.Empty;
            ret.ValorCredito = rootDoc.ElementAnyNs("ValorCredito")?.GetValue<decimal>() ?? 0;

            switch (rootDoc.ElementAnyNs("ISSRetido")?.GetValue<string>() ?? string.Empty)
            {
                case "true":
                    ret.Servico.Valores.IssRetido = SituacaoTributaria.Retencao;
                    break;

                case "false":
                    ret.Servico.Valores.IssRetido = SituacaoTributaria.Normal;
                    break;
            }

            ret.Prestador.CpfCnpj = rootDoc.ElementAnyNs("CPFCNPJPrestador")?.ElementAnyNs("CNPJ")?.GetValue<string>() ?? string.Empty;
            if (ret.Prestador.CpfCnpj == "")
                ret.Prestador.CpfCnpj = rootDoc.ElementAnyNs("CPFCNPJPrestador")?.ElementAnyNs("CPF")?.GetValue<string>() ?? string.Empty;
            ret.Prestador.RazaoSocial = rootDoc.ElementAnyNs("RazaoSocialPrestador")?.GetValue<string>() ?? string.Empty;
            var endPrestador = rootDoc.ElementAnyNs("EnderecoPrestador");
            if (endPrestador != null)
            {
                ret.Prestador.Endereco.TipoLogradouro = endPrestador.ElementAnyNs("TipoLogradouro")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.Endereco.Logradouro = endPrestador.ElementAnyNs("Logradouro")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.Endereco.Numero = endPrestador.ElementAnyNs("NumeroEndereco")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.Endereco.Complemento = endPrestador.ElementAnyNs("ComplementoEndereco")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.Endereco.Bairro = endPrestador.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.Endereco.CodigoMunicipio = endPrestador.ElementAnyNs("Cidade")?.GetValue<int>() ?? 0;
                ret.Prestador.Endereco.Uf = endPrestador.ElementAnyNs("UF")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.Endereco.Cep = endPrestador.ElementAnyNs("CEP")?.GetValue<string>() ?? string.Empty;
            }
            ret.Prestador.DadosContato.Email = rootDoc.ElementAnyNs("EmailPrestador")?.GetValue<string>() ?? string.Empty;

            ret.Tomador.CpfCnpj = rootDoc.ElementAnyNs("CPFCNPJTomador")?.ElementAnyNs("CNPJ")?.GetValue<string>() ?? string.Empty;
            if (ret.Tomador.CpfCnpj == "")
                ret.Tomador.CpfCnpj = rootDoc.ElementAnyNs("CPFCNPJTomador")?.ElementAnyNs("CPF")?.GetValue<string>() ?? string.Empty;
            ret.Tomador.RazaoSocial = rootDoc.ElementAnyNs("RazaoSocialTomador")?.GetValue<string>() ?? string.Empty;
            var endTomador = rootDoc.ElementAnyNs("EnderecoTomador");
            if (endTomador != null)
            {
                ret.Tomador.Endereco.TipoLogradouro = endTomador.ElementAnyNs("TipoLogradouro")?.GetValue<string>() ?? string.Empty;
                ret.Tomador.Endereco.Logradouro = endTomador.ElementAnyNs("Logradouro")?.GetValue<string>() ?? string.Empty;
                ret.Tomador.Endereco.Numero = endTomador.ElementAnyNs("NumeroEndereco")?.GetValue<string>() ?? string.Empty;
                ret.Tomador.Endereco.Complemento = endTomador.ElementAnyNs("ComplementoEndereco")?.GetValue<string>() ?? string.Empty;
                ret.Tomador.Endereco.Bairro = endTomador.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                ret.Tomador.Endereco.CodigoMunicipio = endTomador.ElementAnyNs("Cidade")?.GetValue<int>() ?? 0;
                ret.Tomador.Endereco.Uf = endTomador.ElementAnyNs("UF")?.GetValue<string>() ?? string.Empty;
                ret.Tomador.Endereco.Cep = endTomador.ElementAnyNs("CEP")?.GetValue<string>() ?? string.Empty;
            }
            ret.Tomador.DadosContato.Email = rootDoc.ElementAnyNs("EmailTomador")?.GetValue<string>() ?? string.Empty;

            return ret;
        }

        public override string WriteXmlRps(NotaServico nota, bool identado, bool showDeclaration)
        {
            string tipoRps;
            switch (nota.IdentificacaoRps.Tipo)
            {
                case TipoRps.RPS:
                    tipoRps = "RPS";
                    break;

                case TipoRps.NFConjugada:
                    tipoRps = "RPS-M";
                    break;

                case TipoRps.Cupom:
                    tipoRps = "RPS-C";
                    break;

                default:
                    tipoRps = "";
                    break;
            }

            string tipoTributacao;
            switch (nota.TipoTributacao)
            {
                case TipoTributacao.Tributavel:
                    tipoTributacao = "T";
                    break;

                case TipoTributacao.ForaMun:
                    tipoTributacao = "F";
                    break;

                case TipoTributacao.Isenta:
                    tipoTributacao = "A";
                    break;

                case TipoTributacao.ForaMunIsento:
                    tipoTributacao = "B";
                    break;

                case TipoTributacao.Imune:
                    tipoTributacao = "M";
                    break;

                case TipoTributacao.ForaMunImune:
                    tipoTributacao = "N";
                    break;

                case TipoTributacao.Suspensa:
                    tipoTributacao = "X";
                    break;

                case TipoTributacao.ForaMunSuspensa:
                    tipoTributacao = "V";
                    break;

                case TipoTributacao.ExpServicos:
                    tipoTributacao = "P";
                    break;

                default:
                    tipoTributacao = "";
                    break;
            }

            var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "N" : "C";

            var issRetido = nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? "true" : "false";

            // RPS
            XNamespace ns = "";

            var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            var rps = new XElement(ns + "RPS");
            xmlDoc.Add(rps);

            var hashRps = GetHashRps(nota);
            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "Assinatura", 1, 2000, Ocorrencia.Obrigatoria, hashRps));

            var chaveRPS = new XElement("ChaveRPS");
            rps.Add(chaveRPS);
            chaveRPS.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoPrestador", 1, 15, Ocorrencia.Obrigatoria, nota.Prestador.InscricaoMunicipal));
            chaveRPS.AddChild(AdicionarTag(TipoCampo.Str, "", "SerieRPS", 1, 5, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
            chaveRPS.AddChild(AdicionarTag(TipoCampo.Int, "", "NumeroRPS", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));

            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoRPS", 1, 1, Ocorrencia.Obrigatoria, tipoRps));
            rps.AddChild(AdicionarTag(TipoCampo.Dat, "", "DataEmissao", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "StatusRPS", 1, 1, Ocorrencia.Obrigatoria, situacao));
            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "TributacaoRPS", 1, 1, Ocorrencia.Obrigatoria, tipoTributacao));

            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorDeducoes));
            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPIS", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorPis));
            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCOFINS", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorCofins));
            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorINSS", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorInss));
            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIR", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorIr));
            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCSLL", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorCsll));

            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));
            rps.AddChild(AdicionarTag(TipoCampo.De4, "", "AliquotaServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.Aliquota / 100));  // Valor Percentual - Exemplos: 1% => 0.01   /   25,5% => 0.255   /   100% => 1
            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "ISSRetido", 1, 4, Ocorrencia.Obrigatoria, issRetido));

            if (!nota.Tomador.CpfCnpj.IsEmpty())
            {
                var tomadorCpfCnpj = new XElement("CPFCNPJTomador");
                rps.Add(tomadorCpfCnpj);
                tomadorCpfCnpj.AddChild(AdicionarTagCNPJCPF("", "CPF", "CNPJ", nota.Tomador.CpfCnpj));
            }

            rps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoMunicipalTomador", 1, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));
            rps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoEstadualTomador", 1, 19, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoEstadual));
            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocialTomador", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));

            if (!nota.Tomador.Endereco.Logradouro.IsEmpty())
            {
                var endereco = new XElement("EnderecoTomador");
                rps.AddChild(endereco);
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoLogradouro", 1, 3, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.TipoLogradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Logradouro", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroEndereco", 1, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Numero));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "ComplementoEndereco", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
                endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cidade", 1, 7, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.CodigoMunicipio));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "UF", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
                endereco.AddChild(AdicionarTag(TipoCampo.StrNumberFill, "", "CEP", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
            }

            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "EmailTomador", 1, 75, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));

            if (!nota.Intermediario.CpfCnpj.IsEmpty())
            {
                var intermediarioCpfCnpj = new XElement("CPFCNPJIntermediario");
                rps.Add(intermediarioCpfCnpj);
                intermediarioCpfCnpj.AddChild(AdicionarTagCNPJCPF("", "CPF", "CNPJ", nota.Intermediario.CpfCnpj));

                rps.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipalIntermediario", 1, 8, 0, nota.Intermediario.InscricaoMunicipal));
                rps.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocialIntermediario", 1, 115, 0, nota.Intermediario.RazaoSocial));

                var issRetidoIntermediario = nota.Intermediario.IssRetido == SituacaoTributaria.Retencao ? "true" : "false";
                rps.AddChild(AdicionarTag(TipoCampo.Str, "", "ISSRetidoIntermediario", 1, 4, Ocorrencia.Obrigatoria, issRetidoIntermediario));
                rps.AddChild(AdicionarTag(TipoCampo.Str, "", "EmailIntermediario", 1, 75, Ocorrencia.NaoObrigatoria, nota.Intermediario.EMail));
            }

            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));

            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCargaTributaria", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorCargaTributaria));
            rps.AddChild(AdicionarTag(TipoCampo.De4, "", "PercentualCargaTributaria", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.AliquotaCargaTributaria / 100));
            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "FonteCargaTributaria", 1, 10, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.FonteCargaTributaria));

            rps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoCEI", 1, 12, Ocorrencia.NaoObrigatoria, nota.ConstrucaoCivil.CodigoCEI));
            rps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "MatriculaObra", 1, 12, Ocorrencia.NaoObrigatoria, nota.ConstrucaoCivil.Matricula));
            //rps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "MunicipioPrestacao", 1, 7, Ocorrencia.MaiorQueZero, nota.Servico.CodigoMunicipio));
            rps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "NumeroEncapsulamento", 1, 7, Ocorrencia.NaoObrigatoria, nota.Material.NumeroEncapsulamento));

            return xmlDoc.AsString(identado, showDeclaration, Encoding.UTF8);
        }

        //ToDo: Verificar o motivo de não salvar o xml da NFSe.
        public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true)
        {
            throw new NotImplementedException();
        }

        #endregion Xml

        #region Services

        protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            if (retornoWebservice.Lote == 0)
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });

            if (notas.Count == 0)
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });

            if (retornoWebservice.Erros.Count > 0)
                return;

            var xmlRPS = new StringBuilder();
            foreach (var nota in notas)
            {
                var xmlRps = WriteXmlRps(nota, false, false);
                xmlRPS.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }

            xmlRPS.Replace("<RPS>", "<RPS xmlns=\"\">");

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<PedidoEnvioLoteRPS xmlns=\"http://www.prefeitura.sp.gov.br/nfe\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            loteBuilder.Append("<Cabecalho xmlns=\"\" Versao=\"1\">");
            loteBuilder.Append($"<CPFCNPJRemetente><CNPJ>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</CNPJ></CPFCNPJRemetente>");
            loteBuilder.Append($"<transacao>true</transacao>");
            loteBuilder.Append($"<dtInicio>{notas.Min(x => x.IdentificacaoRps.DataEmissao):yyyy-MM-dd}</dtInicio>");
            loteBuilder.Append($"<dtFim>{notas.Max(x => x.IdentificacaoRps.DataEmissao):yyyy-MM-dd}</dtFim>");
            loteBuilder.Append($"<QtdRPS>{notas.Count}</QtdRPS>");
            loteBuilder.Append(string.Format(CultureInfo.InvariantCulture, "<ValorTotalServicos>{0:0.00}</ValorTotalServicos>", notas.Sum(x => x.Servico.Valores.ValorServicos)));
            loteBuilder.Append(string.Format(CultureInfo.InvariantCulture, "<ValorTotalDeducoes>{0:0.00}</ValorTotalDeducoes>", notas.Sum(x => x.Servico.Valores.ValorDeducoes)));
            loteBuilder.Append("</Cabecalho>");
            loteBuilder.Append(xmlRPS);
            loteBuilder.Append("</PedidoEnvioLoteRPS>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "PedidoEnvioLoteRPS", "", Certificado);
        }

        protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "RetornoEnvioLoteRPS");
            if (retornoWebservice.Erros.Any()) return;

            retornoWebservice.Sucesso = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;
            retornoWebservice.Lote = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("InformacoesLote")?.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;
            retornoWebservice.Data = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("InformacoesLote")?.ElementAnyNs("DataEnvioLote")?.GetValue<DateTime>() ?? DateTime.MinValue;

            if (!retornoWebservice.Sucesso)
                return;

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
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<PedidoInformacoesLote xmlns=\"http://www.prefeitura.sp.gov.br/nfe\" xmlns:xsi = \"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd = \"http://www.w3.org/2001/XMLSchema\">");
            loteBuilder.Append("<Cabecalho xmlns=\"\" Versao=\"1\">");
            loteBuilder.Append($"<CPFCNPJRemetente><CNPJ>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</CNPJ></CPFCNPJRemetente>");
            loteBuilder.Append($"<NumeroLote>{retornoWebservice.Lote}</NumeroLote>");
            loteBuilder.Append($"<InscricaoPrestador>{Configuracoes.PrestadorPadrao.InscricaoMunicipal.ZeroFill(8)}</InscricaoPrestador>");
            loteBuilder.Append("</Cabecalho>");
            loteBuilder.Append("</PedidoInformacoesLote>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "PedidoInformacoesLote", "", Certificado);
        }

        protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "RetornoInformacoesLote");
            if (retornoWebservice.Erros.Any()) return;

            retornoWebservice.Sucesso = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;
            retornoWebservice.Lote = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("InformacoesLote")?.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;
        }

        protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<PedidoConsultaLote xmlns=\"http://www.prefeitura.sp.gov.br/nfe\" xmlns:xsi = \"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd = \"http://www.w3.org/2001/XMLSchema\">");
            loteBuilder.Append("<Cabecalho xmlns=\"\" Versao=\"1\">");
            loteBuilder.Append($"<CPFCNPJRemetente><CNPJ>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</CNPJ></CPFCNPJRemetente>");
            loteBuilder.Append($"<NumeroLote>{retornoWebservice.Lote}</NumeroLote>");
            loteBuilder.Append("</Cabecalho>");
            loteBuilder.Append("</PedidoConsultaLote>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "PedidoConsultaLote", "", Certificado);
        }

        protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "RetornoConsulta");
            if (retornoWebservice.Erros.Any()) return;

            retornoWebservice.Sucesso = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;

            var notasServico = new List<NotaServico>();

            foreach (var nfse in xmlRet.Root.ElementsAnyNs("NFe"))
            {
                var chaveNFSe = nfse.ElementAnyNs("ChaveNFe");
                var numeroNFSe = chaveNFSe?.ElementAnyNs("NumeroNFe")?.GetValue<string>() ?? string.Empty;
                var codigoVerificacao = chaveNFSe?.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;

                var dataNFSe = nfse.ElementAnyNs("DataEmissaoNFe")?.GetValue<DateTime>() ?? DateTime.Now;

                var chaveRPS = nfse.ElementAnyNs("ChaveRPS");

                var numeroRps = chaveRPS?.ElementAnyNs("NumeroRPS")?.GetValue<string>() ?? string.Empty;

                GravarNFSeEmDisco(nfse.ToString(), $"NFSe-{numeroNFSe}-{codigoVerificacao}-.xml", dataNFSe);

                var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
                if (nota == null)
                {
                    nota = LoadXml(nfse.ToString());
                    notas.Add(nota);
                }
                else
                {
                    nota.IdentificacaoNFSe.Numero = numeroNFSe;
                    nota.IdentificacaoNFSe.Chave = codigoVerificacao;
                    nota.XmlOriginal = nfse.ToString();
                }

                notasServico.Add(nota);
            }

            retornoWebservice.Notas = notasServico.ToArray();
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

            retornoWebservice.XmlEnvio = ConsultarRPSNFSe(retornoWebservice.NumeroRps, retornoWebservice.Serie, 0);
        }

        protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "p1:PedidoConsultaNFe", "", Certificado);
        }

        protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "RetornoConsulta");
            if (retornoWebservice.Erros.Any()) return;

            retornoWebservice.Sucesso = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;

            if (!retornoWebservice.Sucesso) return;

            var xmlNFSe = xmlRet.Root.ElementAnyNs("NFe");
            var numeroNFSe = xmlNFSe.ElementAnyNs("ChaveNFe")?.ElementAnyNs("NumeroNFe")?.GetValue<string>() ?? string.Empty;
            var chaveNFSe = xmlNFSe.ElementAnyNs("ChaveNFe")?.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
            var dataNFSe = xmlNFSe.ElementAnyNs("DataEmissaoNFe")?.GetValue<DateTime>() ?? DateTime.Now;
            var numeroRps = xmlNFSe.ElementAnyNs("ChaveRPS")?.ElementAnyNs("NumeroRPS")?.GetValue<string>() ?? string.Empty;

            GravarNFSeEmDisco(xmlNFSe.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota == null)
            {
                nota = notas.Load(xmlNFSe.ToString());
            }
            else
            {
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.IdentificacaoNFSe.Chave = chaveNFSe;
                nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
                nota.XmlOriginal = xmlNFSe.ToString();
            }

            retornoWebservice.Nota = nota;
        }

        protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            if (retornoWebservice.NumeroNFse < 1)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número do RPS/NFSe não informado para a consulta." });
                return;
            }

            retornoWebservice.XmlEnvio = ConsultarRPSNFSe(0, "", retornoWebservice.NumeroNFse);
        }

        protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "p1:PedidoConsultaNFe", "", Certificado);
        }

        protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "RetornoConsulta");
            if (retornoWebservice.Erros.Any()) return;

            retornoWebservice.Sucesso = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;

            if (!retornoWebservice.Sucesso) return;

            var notasServico = xmlRet.Root.ElementsAnyNs("NFe").Select(nfse => LoadXml(nfse.ToString())).ToList();
            retornoWebservice.Notas = notasServico.ToArray();
            notasServico.AddRange(notasServico);
        }

        protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            if (retornoWebservice.NumeroNFSe.IsEmpty())
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe não informado para cancelamento." });
                return;
            }

            // Hash Cancelamento
            var hash = Configuracoes.PrestadorPadrao.InscricaoMunicipal.ZeroFill(8) + retornoWebservice.NumeroNFSe.ZeroFill(12);

            var hashAssinado = "";
#if NETSTANDARD2_0
            var rsa = (RSACng)Certificado.PrivateKey;
#else
            var rsa = (RSACryptoServiceProvider)Certificado.PrivateKey;
#endif

            var hashBytes = Encoding.ASCII.GetBytes(hash);
#if NETSTANDARD2_0
            var signData = rsa.SignData(hashBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
#else
            var signData = rsa.SignData(hashBytes, new SHA1CryptoServiceProvider());
#endif
            hashAssinado = Convert.ToBase64String(signData);

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<PedidoCancelamentoNFe xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.prefeitura.sp.gov.br/nfe\">");
            loteBuilder.Append("<Cabecalho xmlns=\"\" Versao=\"1\">");
            loteBuilder.Append($"<CPFCNPJRemetente><CNPJ>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</CNPJ></CPFCNPJRemetente>");
            loteBuilder.Append($"<transacao>true</transacao>");
            loteBuilder.Append("</Cabecalho>");
            loteBuilder.Append("<Detalhe xmlns=\"\">");
            loteBuilder.Append("<ChaveNFe>");
            loteBuilder.Append($"<InscricaoPrestador>{Configuracoes.PrestadorPadrao.InscricaoMunicipal.ZeroFill(8)}</InscricaoPrestador>");
            loteBuilder.Append($"<NumeroNFe>{retornoWebservice.NumeroNFSe}</NumeroNFe>");
            loteBuilder.Append("</ChaveNFe>");
            loteBuilder.Append($"<AssinaturaCancelamento>{hashAssinado}</AssinaturaCancelamento>");
            loteBuilder.Append("</Detalhe>");
            loteBuilder.Append("</PedidoCancelamentoNFe>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "PedidoCancelamentoNFe", "", Certificado);
        }

        protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "RetornoCancelamentoNFe");
            if (retornoWebservice.Erros.Any()) return;

            retornoWebservice.Sucesso = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;

            // Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
            var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == retornoWebservice.NumeroNFSe);
            if (nota == null) return;

            // No caso de São Paulo, não retorna o XML da NotaFiscal Cancelada.
            // Por este motivo, não grava o arquivo NFSe-{nota.IdentificacaoNFSe.Chave}-{nota.IdentificacaoNFSe.Numero}.xml
            nota.Situacao = SituacaoNFSeRps.Cancelado;
            nota.Cancelamento.Pedido.CodigoCancelamento = retornoWebservice.CodigoCancelamento;
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

        #region Private Methods

        private string ConsultarRPSNFSe(int numeroRPS, string serieRPS, int numeroNFSe)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<p1:PedidoConsultaNFe xmlns:p1=\"http://www.prefeitura.sp.gov.br/nfe\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            loteBuilder.Append("<Cabecalho Versao=\"1\">");
            loteBuilder.Append($"<CPFCNPJRemetente><CNPJ>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</CNPJ></CPFCNPJRemetente>");
            loteBuilder.Append("</Cabecalho>");
            loteBuilder.Append("<Detalhe>");
            if (numeroRPS > 0)
            {
                // RPS
                loteBuilder.Append("<ChaveRPS>");
                loteBuilder.Append($"<InscricaoPrestador>{Configuracoes.PrestadorPadrao.InscricaoMunicipal.ZeroFill(8)}</InscricaoPrestador>");
                loteBuilder.Append($"<SerieRPS>{serieRPS}</SerieRPS>");
                loteBuilder.Append($"<NumeroRPS>{numeroRPS}</NumeroRPS>");
                loteBuilder.Append("</ChaveRPS>");
            }
            else
            {
                // NFSe
                loteBuilder.Append("<ChaveNFe>");
                loteBuilder.Append($"<InscricaoPrestador>{Configuracoes.PrestadorPadrao.InscricaoMunicipal.ZeroFill(8)}</InscricaoPrestador>");
                loteBuilder.Append($"<NumeroNFe>{numeroNFSe}</NumeroNFe>");
                loteBuilder.Append("</ChaveNFe>");
            }
            loteBuilder.Append("</Detalhe>");
            loteBuilder.Append("</p1:PedidoConsultaNFe>");
            return loteBuilder.ToString();
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            switch (tipo)
            {
                case TipoUrl.Enviar:
                    return "PedidoEnvioLoteRPS_v01.xsd";

                case TipoUrl.EnviarSincrono:
                    return "PedidoEnvioRPS_v01.xsd";

                case TipoUrl.ConsultarSituacao:
                    return "PedidoInformacoesLote_v01.xsd";

                case TipoUrl.ConsultarLoteRps:
                    return "PedidoConsultaLote_v01.xsd";

                case TipoUrl.ConsultarSequencialRps:
                    return "";

                case TipoUrl.ConsultarNFSeRps:
                    return "PedidoConsultaNFe_v01.xsd";

                case TipoUrl.ConsultarNFSe:
                    return "PedidoConsultaNFe_v01.xsd";

                case TipoUrl.CancelarNFSe:
                    return "PedidoCancelamentoNFe_v01.xsd";

                case TipoUrl.CancelarNFSeLote:
                    return "PedidoCancelamentoLote_v01.xsd";

                case TipoUrl.SubstituirNFSe:
                    return "";

                default:
                    throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null);
            }
        }

        protected override IServiceClient GetClient(TipoUrl tipo) => new SaoPauloServiceClient(this, tipo);

        protected override string GerarCabecalho() => "";

        private string GetHashRps(NotaServico nota)
        {
            string tipoTributacao;
            switch (nota.TipoTributacao)
            {
                case TipoTributacao.Tributavel:
                    tipoTributacao = "T";
                    break;

                case TipoTributacao.ForaMun:
                    tipoTributacao = "F";
                    break;

                case TipoTributacao.Isenta:
                    tipoTributacao = "A";
                    break;

                case TipoTributacao.ForaMunIsento:
                    tipoTributacao = "B";
                    break;

                case TipoTributacao.Imune:
                    tipoTributacao = "M";
                    break;

                case TipoTributacao.ForaMunImune:
                    tipoTributacao = "N";
                    break;

                case TipoTributacao.Suspensa:
                    tipoTributacao = "X";
                    break;

                case TipoTributacao.ForaMunSuspensa:
                    tipoTributacao = "V";
                    break;

                case TipoTributacao.ExpServicos:
                    tipoTributacao = "P";
                    break;

                default:
                    tipoTributacao = "?";
                    break;
            }

            var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "N" : "C";
            var issRetido = nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? "S" : "N";

            var indCpfCnpjTomador = "3";
            switch (nota.Tomador.CpfCnpj?.Length)
            {
                case 11:
                    indCpfCnpjTomador = "1";
                    break;

                case 14:
                    indCpfCnpjTomador = "2";
                    break;
            }

            // Assinatura do RPS
            string hash = nota.Prestador.InscricaoMunicipal.PadLeft(8, '0') +
                          nota.IdentificacaoRps.Serie.PadRight(5, ' ') +
                          nota.IdentificacaoRps.Numero.PadLeft(12, '0') +
                          nota.IdentificacaoRps.DataEmissao.Year.ToString().PadLeft(4, '0') +
                          nota.IdentificacaoRps.DataEmissao.Month.ToString().PadLeft(2, '0') +
                          nota.IdentificacaoRps.DataEmissao.Day.ToString().PadLeft(2, '0') +
                          tipoTributacao +
                          situacao +
                          issRetido +
                          Convert.ToInt32(nota.Servico.Valores.ValorServicos * 100).ToString().PadLeft(15, '0') +
                          Convert.ToInt32(nota.Servico.Valores.ValorDeducoes * 100).ToString().PadLeft(15, '0') +
                          nota.Servico.ItemListaServico.PadLeft(5, '0') +
                          indCpfCnpjTomador +
                          nota.Tomador.CpfCnpj?.PadLeft(14, '0');
            if (!nota.Intermediario.CpfCnpj.IsEmpty())
            {
                var indCpfCnpjIntermediario = "3";
                switch (nota.Intermediario.CpfCnpj.Length)
                {
                    case 11:
                        indCpfCnpjIntermediario = "1";
                        break;

                    case 14:
                        indCpfCnpjIntermediario = "2";
                        break;
                }

                var issRetidoIntermediario = nota.Intermediario.IssRetido == SituacaoTributaria.Retencao ? "S" : "N";
                hash = hash +
                       indCpfCnpjIntermediario +
                       nota.Intermediario.CpfCnpj.PadLeft(14, '0') +
                       issRetidoIntermediario;
            }

#if  NETSTANDARD2_0
            var rsa = (RSACng)Certificado.PrivateKey;
#else
            var rsa = (RSACryptoServiceProvider)Certificado.PrivateKey;
#endif

            var hashBytes = Encoding.ASCII.GetBytes(hash);
#if NETSTANDARD2_0
            var signData = rsa.SignData(hashBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
#else
            var signData = rsa.SignData(hashBytes, new SHA1CryptoServiceProvider());
#endif
            return Convert.ToBase64String(signData);
        }

        private static void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string xmlTag)
        {
            var mensagens = xmlRet?.ElementAnyNs(xmlTag);
            if (mensagens == null)
                return;

            foreach (var mensagem in mensagens.ElementsAnyNs("Alerta"))
            {
                var evento = new Evento
                {
                    Codigo = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty,
                    Descricao = mensagem?.ElementAnyNs("Descricao")?.GetValue<string>() ?? string.Empty
                };
                var chave = mensagens?.ElementAnyNs("ChaveRPSNFe");
                if (chave != null)
                {
                    evento.IdentificacaoNfse.Chave = chave.ElementAnyNs("ChaveNFe")?.GetValue<string>() ?? string.Empty;
                    evento.IdentificacaoRps.Numero = chave.ElementAnyNs("ChaveRPS")?.GetValue<string>() ?? string.Empty;
                }
                retornoWs.Alertas.Add(evento);
            }

            foreach (var mensagem in mensagens.ElementsAnyNs("Erro"))
            {
                var evento = new Evento
                {
                    Codigo = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty,
                    Descricao = mensagem?.ElementAnyNs("Descricao")?.GetValue<string>() ?? string.Empty
                };
                var chave = mensagens?.ElementAnyNs("ChaveRPSNFe");
                if (chave != null)
                {
                    evento.IdentificacaoNfse.Chave = chave.ElementAnyNs("ChaveNFe")?.GetValue<string>() ?? string.Empty;
                    evento.IdentificacaoRps.Numero = chave.ElementAnyNs("ChaveRPS")?.GetValue<string>() ?? string.Empty;
                }
                retornoWs.Erros.Add(evento);
            }
        }

        #endregion Private Methods

        #endregion Methods
    }
}