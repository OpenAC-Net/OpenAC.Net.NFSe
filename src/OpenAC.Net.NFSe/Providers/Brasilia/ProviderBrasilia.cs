﻿// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-22-2018
//
// Last Modified By : Rafael Dias
// Last Modified On : 05-22-2018
// ***********************************************************************
// <copyright file="ProviderBetha.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2022 Projeto OpenAC .Net
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
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class ProviderBrasilia : ProviderABRASF
    {
        #region Fields

        private readonly XNamespace tc = "http://www.issnetonline.com.br/webserviceabrasf/vsd/tipos_complexos.xsd";

        #endregion Fields

        #region Constructors

        public ProviderBrasilia(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "ABRASFv2";
            Versao = "2.04";
            UsaPrestadorEnvio = true;
        }

        public string Versao { get; protected set; }

        public bool UsaPrestadorEnvio { get; protected set; }

        #endregion Constructors

        #region Methods

        #region RPS

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
            xmlLote.Append("<EnviarLoteRpsEnvio xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            xmlLote.Append($"<LoteRps Id=\"lote{retornoWebservice.Lote.ToString().PadLeft(2, '0')}\" versao=\"{Versao}\">");
            xmlLote.Append($"<NumeroLote>{retornoWebservice.Lote}</NumeroLote>");

            xmlLote.Append(WritePrestadorRps(notas[0]));

            xmlLote.Append($"<QuantidadeRps>{notas.Count}</QuantidadeRps>");
            xmlLote.Append("<ListaRps>");
            xmlLote.Append(xmlLoteRps);
            xmlLote.Append("</ListaRps>");
            xmlLote.Append("</LoteRps>");
            xmlLote.Append("</EnviarLoteRpsEnvio>");

            var document = XDocument.Parse(xmlLote.ToString());
            //document.ElementAnyNs("EnviarLoteRpsEnvio").AddAttribute(new XAttribute(XNamespace.Xmlns + "tc", tc));
            //NFSeUtil.ApplyNamespace(document.Root, tc, "LoteRps", "EnviarLoteRpsEnvio");

            retornoWebservice.XmlEnvio = document.AsString();
        }

        protected override XElement WriteRps(NotaServico nota)
        {
            var rps = new XElement("Rps");

            var infoDeclaracao = new XElement("InfDeclaracaoPrestacaoServico");
            infoDeclaracao.Add(WriteRpsInfDeclaracao(nota));
            infoDeclaracao.AddChild(AdicionarTag(TipoCampo.Dat, "", "Competencia", 1, 1, Ocorrencia.Obrigatoria, nota.Competencia));
            infoDeclaracao.AddChild(WriteServicosValoresRps(nota));
            infoDeclaracao.AddChild(WritePrestadorRps(nota));
            infoDeclaracao.AddChild(WriteTomadorRps(nota));
            infoDeclaracao.AddChild(WriteIntermediarioRps(nota));
            infoDeclaracao.AddChild(WriteConstrucaoCivilRps(nota));

            if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
            {
                infoDeclaracao.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, "6"));
                infoDeclaracao.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, "1"));
            }
            else
            {
                var regimeEspecialTributacao = nota.RegimeEspecialTributacao == 0 ? string.Empty : nota.RegimeEspecialTributacao.ToString();

                infoDeclaracao.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
                infoDeclaracao.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, "2"));
            }

            infoDeclaracao.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivoFiscal", 1, 1, Ocorrencia.Obrigatoria, (nota.IncentivadorFiscal == NFSeSimNao.Sim ? 1 : 2)));

            rps.Add(infoDeclaracao);

            return rps;
        }

        

        

        private XElement WriteRpsInfDeclaracao(NotaServico nota)
        {
            var rps = new XElement("Rps");

            rps.Add(WriteIdentificacao(nota));
            rps.AddChild(AdicionarTag(TipoCampo.Dat, "", "DataEmissao", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            rps.AddChild(AdicionarTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, (nota.Situacao == SituacaoNFSeRps.Normal ? "1" : "2")));

            return rps;
        }

        protected override XElement WritePrestadorRps(NotaServico nota)
        {
            var prestador = new XElement("Prestador");

            var cpfCnpjPrestador = new XElement("CpfCnpj");
            prestador.Add(cpfCnpjPrestador);

            cpfCnpjPrestador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj));
            prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Prestador.InscricaoMunicipal));

            return prestador;
        }

        protected override XElement WriteTomadorRps(NotaServico nota)
        {
            var tomador = new XElement("TomadorServico");

            var ideTomador = new XElement("IdentificacaoTomador");
            tomador.Add(ideTomador);

            var cpfCnpjTomador = new XElement("CpfCnpj");
            ideTomador.Add(cpfCnpjTomador);

            if (!string.IsNullOrEmpty(nota.Tomador.CpfCnpj)) // Existem NFS-e que é possível ser emitida sem tomador
            {
                cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

                ideTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, "123456"));

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

        protected override XElement WriteServicosValoresRps(NotaServico nota)
        {
            var servico = new XElement("Servico");
            var valores = new XElement("Valores");

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorDeducoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorPis));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorCofins));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorInss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorIr));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorCsll));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.OutrasRetencoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValTotTributos", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorCargaTributaria));
            //valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorIss));
            // Valor Percentual - Exemplos: 1% => 1.00   /   25,5% => 25.5   /   100% => 100
            //valores.AddChild(AdicionarTag(TipoCampo.De2, "", "Aliquota", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.Aliquota));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.DescontoIncondicionado));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.DescontoCondicionado));

            servico.AddChild(valores);

            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.IssRetidoFonte.GetHashCode()));
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "ResponsavelRetencao", 0, 1, Ocorrencia.NaoObrigatoria, nota.Servico.ResponsavelRetencao));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoCnae", 0, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoNbs", 0, 9, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoNbs));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 0, 7, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPais", 0, 4, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoPais));
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "ExigibilidadeISS", 1, 2, Ocorrencia.Obrigatoria, nota.Servico.ExigibilidadeIss.GetHashCode() + 1));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "IdentifNaoExigibilidade", 0, 4, Ocorrencia.NaoObrigatoria, nota.Servico.IdentifNaoExigibilidade));
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "MunicipioIncidencia", 0, 7, Ocorrencia.NaoObrigatoria, nota.Servico.MunicipioIncidencia));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroProcesso", 0, 30, Ocorrencia.NaoObrigatoria, nota.Servico.NumeroProcesso));

            return servico;
        }

        protected override XElement WriteConstrucaoCivilRps(NotaServico nota)
        {
            if (nota.ConstrucaoCivil.CodigoObra.IsEmpty()) return null;

            var construcao = new XElement("ConstrucaoCivil");

            construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoObra", 1, 15, Ocorrencia.NaoObrigatoria, nota.ConstrucaoCivil.CodigoObra));
            construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "Art", 1, 15, Ocorrencia.Obrigatoria, nota.ConstrucaoCivil.ArtObra));

            return construcao;
        }

        #endregion RPS

        #region Services

        

        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "Rps", "", Certificado);
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
            loteBuilder.Append("<ConsultarSituacaoLoteRpsEnvio xmlns=\"http://www.issnetonline.com.br/webserviceabrasf/vsd/servico_consultar_situacao_lote_rps_envio.xsd\" xmlns:tc=\"http://www.issnetonline.com.br/webserviceabrasf/vsd/tipos_complexos.xsd\">");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<tc:CpfCnpj><tc:Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tc:Cnpj></tc:CpfCnpj>");
            loteBuilder.Append($"<tc:InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</tc:InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarSituacaoLoteRpsEnvio>");

            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            loteBuilder.Append("<ConsultarLoteRpsEnvio xmlns=\"http://www.issnetonline.com.br/webserviceabrasf/vsd/servico_consultar_lote_rps_envio.xsd\" xmlns:tc=\"http://www.issnetonline.com.br/webserviceabrasf/vsd/tipos_complexos.xsd\">");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<tc:CpfCnpj><tc:Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tc:Cnpj></tc:CpfCnpj>");
            loteBuilder.Append($"<tc:InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</tc:InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarLoteRpsEnvio>");

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
            loteBuilder.Append("<ConsultarNfseRpsEnvio xmlns=\"http://www.issnetonline.com.br/webserviceabrasf/vsd/servico_consultar_nfse_rps_envio.xsd\" xmlns:tc=\"http://www.issnetonline.com.br/webserviceabrasf/vsd/tipos_complexos.xsd\">");
            loteBuilder.Append("<IdentificacaoRps>");
            loteBuilder.Append($"<tc:Numero>{retornoWebservice.NumeroRps}</tc:Numero>");
            loteBuilder.Append($"<tc:Serie>{retornoWebservice.Serie}</tc:Serie>");
            loteBuilder.Append($"<tc:Tipo>{(int)retornoWebservice.Tipo + 1}</tc:Tipo>");
            loteBuilder.Append("</IdentificacaoRps>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<tc:CpfCnpj><tc:Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tc:Cnpj></tc:CpfCnpj>");
            loteBuilder.Append($"<tc:InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</tc:InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append("</ConsultarNfseRpsEnvio>");

            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            var loteBuilder = new StringBuilder();

            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            loteBuilder.Append("<ConsultarNfseEnvio xmlns=\"http://www.issnetonline.com.br/webserviceabrasf/vsd/servico_consultar_nfse_envio.xsd\" xmlns:tc=\"http://www.issnetonline.com.br/webserviceabrasf/vsd/tipos_complexos.xsd\">");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<tc:CpfCnpj><tc:Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tc:Cnpj></tc:CpfCnpj>");
            loteBuilder.Append($"<tc:InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</tc:InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");

            if (retornoWebservice.NumeroNFse > 0)
                loteBuilder.Append($"<NumeroNfse>{retornoWebservice.NumeroNFse}</tc:NumeroNfse>");

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
                loteBuilder.Append("<tc:CpfCnpj>");
                loteBuilder.Append(retornoWebservice.CPFCNPJTomador.IsCNPJ()
                    ? $"<tc:Cnpj>{retornoWebservice.CPFCNPJTomador.ZeroFill(14)}</tc:Cnpj>"
                    : $"<tc:Cpf>{retornoWebservice.CPFCNPJTomador.ZeroFill(11)}</tc:Cpf>");
                loteBuilder.Append("</tc:CpfCnpj>");
                if (!retornoWebservice.IMTomador.IsEmpty())
                    loteBuilder.Append($"<tc:InscricaoMunicipal>{retornoWebservice.CPFCNPJTomador}</tc:InscricaoMunicipal>");
                loteBuilder.Append("</Tomador>");
            }

            if (!retornoWebservice.NomeIntermediario.IsEmpty() && !retornoWebservice.CPFCNPJIntermediario.IsEmpty())
            {
                loteBuilder.Append("<IntermediarioServico>");
                loteBuilder.Append($"<tc:RazaoSocial>{retornoWebservice.NomeIntermediario}</tc:RazaoSocial>");
                loteBuilder.Append("<tc:CpfCnpj>");
                loteBuilder.Append(retornoWebservice.CPFCNPJIntermediario.IsCNPJ()
                    ? $"<tc:Cnpj>{retornoWebservice.CPFCNPJIntermediario.ZeroFill(14)}</tc:Cnpj>"
                    : $"<tc:Cpf>{retornoWebservice.CPFCNPJIntermediario.ZeroFill(11)}</tc:Cpf>");
                loteBuilder.Append("</tc:CpfCnpj>");
                if (!retornoWebservice.IMIntermediario.IsEmpty())
                    loteBuilder.Append($"<tc:InscricaoMunicipal>{retornoWebservice.IMIntermediario}</tc:InscricaoMunicipal>");
                loteBuilder.Append("</IntermediarioServico>");
            }

            loteBuilder.Append("</ConsultarNfseEnvio>");

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
            loteBuilder.Append("<pl:CancelarNfseEnvio xmlns:pl=\"http://www.issnetonline.com.br/webserviceabrasf/vsd/servico_cancelar_nfse_envio.xsd\" xmlns:tc=\"http://www.issnetonline.com.br/webserviceabrasf/vsd/tipos_complexos.xsd\">");
            loteBuilder.Append("<Pedido>");
            loteBuilder.Append("<tc:InfPedidoCancelamento>");
            loteBuilder.Append("<tc:IdentificacaoNfse>");
            loteBuilder.Append($"<tc:Numero>{retornoWebservice.NumeroNFSe}</tc:Numero>");
            loteBuilder.Append($"<tc:Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tc:Cnpj>");
            loteBuilder.Append($"<tc:InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</tc:InscricaoMunicipal>");
            loteBuilder.Append($"<tc:CodigoMunicipio>{Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio}</tc:CodigoMunicipio>");
            loteBuilder.Append("</tc:IdentificacaoNfse>");
            loteBuilder.Append($"<tc:CodigoCancelamento>{retornoWebservice.CodigoCancelamento}</tc:CodigoCancelamento>");

            if (!retornoWebservice.Motivo.IsEmpty())
                loteBuilder.Append($"<tc:MotivoCancelamentoNfse>{retornoWebservice.Motivo}</tc:MotivoCancelamentoNfse>");

            loteBuilder.Append("</tc:InfPedidoCancelamento>");
            loteBuilder.Append("</Pedido>");
            loteBuilder.Append("</pl:CancelarNfseEnvio>");

            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "Pedido", "", Certificado);
        }

        #endregion Services

        #region Protected Methods

        protected override IServiceClient GetClient(TipoUrl tipo)
        {
            return new BrasiliaServiceClient(this, tipo);
        }

        protected override string GetNamespace()
        {
            return "xmlns=\"http://www.issnetonline.com.br/webserviceabrasf/vsd/servico_enviar_lote_rps_envio.xsd\"";
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            return "nfse_v204.xsd";
        }

        protected override bool PrecisaValidarSchema(TipoUrl tipo) => false;

        #endregion Protected Methods

        #endregion Methods
    }
}