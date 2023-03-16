// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira/Transis
// Created          : 16-03-2023
//
// Last Modified By : Felipe Silveira/Transis
// Last Modified On : 16-03-2020
// ***********************************************************************
// <copyright file="ProviderSimplISSv2.cs" company="OpenAC .Net">
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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class ProviderSimplISSv2 : ProviderBase
    {
        #region Constructors

        public ProviderSimplISSv2(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "SimplISSv2";
        }

        #endregion Constructors

        #region Methods


        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "InfDeclaracaoPrestacaoServico", Certificado);
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsEnvio", "nfse:LoteRps", Certificado);
        }

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
            xmlLote.Append($"<EnviarLoteRpsEnvio {GetNamespace()}>");
            xmlLote.Append($"<nfse:LoteRps Id=\"L{retornoWebservice.Lote}\">");
            xmlLote.Append($"<nfse:NumeroLote>{retornoWebservice.Lote}</nfse:NumeroLote>");
            xmlLote.Append("<nfse:CpfCnpj>");
            xmlLote.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
            ? $"<nfse:Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</nfse:Cnpj>"
            : $"<nfse:Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</nfse:Cpf>");
            xmlLote.Append("</nfse:CpfCnpj>");
            xmlLote.Append($"<nfse:InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</nfse:InscricaoMunicipal>");
            xmlLote.Append($"<nfse:QuantidadeRps>{notas.Count}</nfse:QuantidadeRps>");
            xmlLote.Append("<nfse:ListaRps>");
            xmlLote.Append(xmlLoteRps);
            xmlLote.Append("</nfse:ListaRps>");
            xmlLote.Append("</nfse:LoteRps>");
            xmlLote.Append("</EnviarLoteRpsEnvio>");

            retornoWebservice.XmlEnvio = xmlLote.ToString();
        }

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

            infoRps.AddChild(AdicionarTag(TipoCampo.Dat, "", "Competencia", 10, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
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
                regimeEspecialTributacao = "6";
                optanteSimplesNacional = "1";
            }
            else
            {
                var regime = (int)nota.RegimeEspecialTributacao;
                regimeEspecialTributacao = regime == 0 ? string.Empty : regime.ToString();
                optanteSimplesNacional = "2";
            }

            var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "1" : "2";

            var infoRps = new XElement("InfDeclaracaoPrestacaoServico", new XAttribute("Id", $"R{nota.IdentificacaoRps.Numero}"));

            var rps = new XElement("Rps");

            rps.Add(WriteIdentificacao(nota));
            rps.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissao", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            rps.AddChild(AdicionarTag(TipoCampo.Int, "", "NaturezaOperacao", 1, 1, Ocorrencia.Obrigatoria, nota.NaturezaOperacao));
            rps.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
            rps.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
            rps.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivadorCultural", 1, 1, Ocorrencia.Obrigatoria, incentivadorCultural));
            rps.AddChild(AdicionarTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, situacao));

            infoRps.Add(rps);
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
            ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Serie", 1, 5, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
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

            // Valor Percentual - Exemplos: 1% => 0.01   /   25,5% => 0.255   /   100% => 1
            valores.AddChild(AdicionarTag(TipoCampo.De4, "", "Aliquota", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota / 100));
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
            var CpfCnpj = new XElement("CpfCnpj");
            CpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj));
            prestador.AddChild(CpfCnpj);
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

            var construcao = new XElement("ConstrucaoCivil");

            construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoObra", 1, 15, Ocorrencia.NaoObrigatoria, nota.ConstrucaoCivil.CodigoObra));
            construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "Art", 1, 15, Ocorrencia.Obrigatoria, nota.ConstrucaoCivil.ArtObra));

            return construcao;
        }

        protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            if (retornoWebservice.NumeroRps < 1)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da RPS não informado para a consulta." });
                return;
            }

            var loteBuilder = new StringBuilder();

            loteBuilder.Append($"<sis:ConsultarNfseRpsEnvio {GetNamespace()}>");
            loteBuilder.Append("<nfse:IdentificacaoRps>");
            loteBuilder.Append($"<nfse:Numero>{retornoWebservice.NumeroRps}</nfse:Numero>");
            loteBuilder.Append($"<nfse:Serie>{retornoWebservice.Serie}</nfse:Serie>");
            loteBuilder.Append($"<nfse:Tipo>{(int)retornoWebservice.Tipo + 1}</nfse:Tipo>");
            loteBuilder.Append("</nfse:IdentificacaoRps>");
            loteBuilder.Append("<nfse:Prestador>");
            loteBuilder.Append("<nfse:CpfCnpj>");
            loteBuilder.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
            ? $"<nfse:Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</nfse:Cnpj>"
            : $"<nfse:Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</nfse:Cpf>");
            loteBuilder.Append("</nfse:CpfCnpj>");
            loteBuilder.Append($"<nfse:InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</nfse:InscricaoMunicipal>");
            loteBuilder.Append("</nfse:Prestador>");
            loteBuilder.Append("</sis:ConsultarNfseRpsEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        private string GetNamespace() => "xmlns:sis=\"http://www.sistema.com.br/Sistema.Ws.Nfse\" xmlns:nfse=\"http://www.abrasf.org.br/nfse.xsd\"";

        protected override IServiceClient GetClient(TipoUrl tipo) => new SimplISSv2ServiceClient(this, tipo);

        protected override string GetSchema(TipoUrl tipo) => "nfse.xsd";

        protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            if (retornoWebservice.Erros.Any()) return;

            var compNfse = xmlRet.ElementAnyNs("ConsultarNfseRpsResposta")?.ElementAnyNs("CompNfse");
            if (compNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (CompNfse)" });
                return;
            }

            var infNFSe = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
            var numeroNFSe = infNFSe.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            var chaveNFSe = infNFSe.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
            var dataNFSe = infNFSe.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
            var numeroRps = infNFSe.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;

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
                nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
                nota.XmlOriginal = compNfse.ToString();
            }

            retornoWebservice.Nota = nota;
            retornoWebservice.Sucesso = true;
        }
        protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "EnviarLoteRpsResposta");
            if (retornoWebservice.Erros.Any()) return;

            retornoWebservice.Lote = xmlRet.Root?.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;
            retornoWebservice.Data = xmlRet.Root?.ElementAnyNs("DataRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Protocolo = xmlRet.Root?.ElementAnyNs("Protocolo")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.Sucesso = retornoWebservice.Lote > 0;

            if (!retornoWebservice.Sucesso)
                return;

            // ReSharper disable once SuggestVarOrType_SimpleTypes
            foreach (NotaServico nota in notas)
            {
                nota.NumeroLote = retornoWebservice.Lote;
            }
        }

        private static void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string xmlTag)
        {
            var mensagens = xmlRet?.ElementAnyNs(xmlTag);
            mensagens = mensagens?.ElementAnyNs("Messages");
            if (mensagens == null)
                return;

            foreach (var mensagem in mensagens.ElementsAnyNs("Message"))
            {
                retornoWs.Erros.Add(new Evento
                {
                    Codigo = mensagem?.ElementAnyNs("Id")?.GetValue<string>() ?? string.Empty,
                    Descricao = mensagem?.ElementAnyNs("Description")?.GetValue<string>() ?? string.Empty,
                    Correcao = mensagem?.ElementAnyNs("Description")?.GetValue<string>() ?? string.Empty
                });
            }
        }
        #endregion Methods

        #region Not Implemented Methods

        public override NotaServico LoadXml(XDocument xml)
        {
            throw new NotImplementedException("Metodo LoadXml não implementado");
        }
        public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }
        protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }
        protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }
        protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }
        protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }
        protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }
        protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
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

        protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
        }

        protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
        }

        protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
        }

        protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
        {
        }

        protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
        }

        protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
        {
        }

        protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice)
        {
        }

        protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice)
        {
        }

        protected override void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override string GerarCabecalho()
        {
            return "";
        }

        #endregion Not Implemented Methods

    }
}