// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 30-05-2022
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 30-05-2022
//
// ***********************************************************************
// <copyright file="ProviderBase.cs" company="OpenAC .Net">
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
using System.Globalization;
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
    internal sealed class ProviderIPM : ProviderBase
    {
        public ProviderIPM(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "IPM";
        }

        private string FormataDecimal(decimal valor)
        {
            return valor.ToString("0.00").Replace(".", ",");
        }

        protected override IServiceClient GetClient(TipoUrl tipo)
        {
            return new IPMServiceClient(this, tipo);
        }

        private string GetTipoTomadorIPM(int tipoTomador, string cpfCnpj)
        {
            if (tipoTomador == 0)
            {
                if (string.IsNullOrEmpty(cpfCnpj))
                    return "F";

                if (cpfCnpj.Length == 14)
                {
                    return "J";
                }
                return "F";
            }
            if (tipoTomador == TipoTomador.Sigiss.PessoaFisica)
            {
                //F para Pessoa Física
                return "F";
            }
            else if(tipoTomador == TipoTomador.Sigiss.JuridicaForaPais)
            {
                //E para Estrangeiro
                return "E";
            }
            else
            {
                //J para Pessoa Jurídica
                return "J";
            }
        }
        public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
        {
            var xmldoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));

            var nfseTag = new XElement("nfse", new XAttribute("id", $"rps:{nota.IdentificacaoRps.Numero}"));
            xmldoc.Add(nfseTag);

            var notaTag = new XElement("nf");
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_total", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorServicos)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_desconto", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.DescontoCondicionado)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_ir", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorIr)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_inss", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorInss)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_contribuicao_social", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.AliquotaCsll)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_rps", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorLiquidoNfse)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_pis", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorPis)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_cofins", 1, 14, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorCofins)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "observacao", 1, 2000, Ocorrencia.Obrigatoria, ""));

            nfseTag.Add(notaTag);

            //PRESTADOR
            var prestadorTag = new XElement("prestador");
            prestadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "cpfcnpj", 1, 14, Ocorrencia.Obrigatoria, nota.Prestador.CpfCnpj.OnlyNumbers()));
            prestadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "cidade", 1, 14, Ocorrencia.Obrigatoria, Municipio.CodigoSiafi));

            nfseTag.Add(prestadorTag);

            //TOMADOR
            var tomadorTag = new XElement("tomador");
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tipo", 1, 14, Ocorrencia.Obrigatoria, GetTipoTomadorIPM(nota.Tomador.Tipo, nota.Tomador.CpfCnpj.OnlyNumbers())));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "cpfcnpj", 1, 14, Ocorrencia.Obrigatoria, nota.Tomador.CpfCnpj.OnlyNumbers()));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "ie", 1, 20, Ocorrencia.Obrigatoria, nota.Tomador.InscricaoEstadual.OnlyNumbers()));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "nome_razao_social", 1, 115, Ocorrencia.Obrigatoria, nota.Tomador.RazaoSocial));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "sobrenome_nome_fantasia", 1, 115, Ocorrencia.Obrigatoria, nota.Tomador.NomeFantasia));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "logradouro", 1, 125, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Logradouro));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "email", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.Email));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "numero_residencia", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Numero));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "complemento", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Complemento));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "ponto_referencia", 1, 120, Ocorrencia.Obrigatoria, ""));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "bairro", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Bairro));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "cidade", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.CodigoMunicipio));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "cep", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Cep));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "ddd_fone_comercial", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.DDD));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "fone_comercial", 1, 120, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.Telefone));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "ddd_fone_residencial", 1, 120, Ocorrencia.Obrigatoria, ""));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "fone_residencial", 1, 120, Ocorrencia.Obrigatoria, ""));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "ddd_fax", 1, 120, Ocorrencia.Obrigatoria, ""));
            tomadorTag.AddChild(AdicionarTag(TipoCampo.Str, "", "fone_fax", 1, 120, Ocorrencia.Obrigatoria, ""));

            nfseTag.Add(tomadorTag);

            //SERVICO
            var itensTag = new XElement("itens");
            var listaTag = new XElement("lista");
            listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "codigo_local_prestacao_servico", 1, 7, Ocorrencia.Obrigatoria, Municipio.CodigoSiafi));
            listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "codigo_item_lista_servico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico.OnlyNumbers()));
            listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "descritivo", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
            listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "aliquota_item_lista_servico", 1, 6, Ocorrencia.Obrigatoria, nota.Servico.Valores.Aliquota));
            listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "situacao_tributaria", 1, 2, Ocorrencia.Obrigatoria, "00"));
            listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_tributavel", 1, 15, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorServicos)));
            listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_deducao", 1, 15, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorDeducoes)));
            listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor_issrf", 1, 15, Ocorrencia.Obrigatoria, FormataDecimal(nota.Servico.Valores.ValorIss)));
            listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tributa_municipio_prestador", 1, 15, Ocorrencia.Obrigatoria, "S"));
            listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "unidade_codigo", 1, 15, Ocorrencia.Obrigatoria, ""));
            listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "unidade_quantidade", 1, 15, Ocorrencia.Obrigatoria, ""));
            listaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "unidade_valor_unitario", 1, 15, Ocorrencia.Obrigatoria, ""));

            itensTag.Add(listaTag);
            nfseTag.Add(itensTag);

            return xmldoc.Root.AsString(identado, showDeclaration, Encoding.UTF8);
        }

        protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
            if (notas.Count > 1) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Este provedor aceita apenas uma RPS por vez" });
            if (retornoWebservice.Erros.Count > 0) return;

            var nota = notas[0];
            var xmlRps = WriteXmlRps(nota);

            GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);

            retornoWebservice.XmlEnvio = xmlRps;
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

        protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException();
        }

        protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            var message = new StringBuilder();
            message.Append("<Cancela>");
            message.Append($"<NumeroNFSe>{retornoWebservice.NumeroNFSe}</NumeroNFSe>");
            message.Append($"<SerieNFSe>{retornoWebservice.SerieNFSe}</SerieNFSe>");
            message.Append($"<Motivo>{retornoWebservice.Motivo}</Motivo>");
            message.Append("</Cancela>");
            retornoWebservice.XmlEnvio = message.ToString();
        }

        protected override bool PrecisaValidarSchema(TipoUrl tipo)
        {
            return false;
        }

        protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            try
            {
                var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

                retornoWebservice.Data = xmlRet.Root?.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.MinValue;
                retornoWebservice.Protocolo = xmlRet.Root?.ElementAnyNs("codigo")?.GetValue<string>() ?? string.Empty;
                retornoWebservice.Sucesso = !retornoWebservice.Protocolo.IsEmpty();

                if (!retornoWebservice.Sucesso) return;

                var numeroNFSe = xmlRet.Root.ElementAnyNs("numero_nf")?.GetValue<string>() ?? string.Empty;
                var chaveNFSe = xmlRet.Root.ElementAnyNs("codigo")?.GetValue<string>() ?? string.Empty;
                var dataNFSe = xmlRet.Root.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.Now;
                var numeroRps = xmlRet.Root.ElementAnyNs("rps")?.GetValue<string>() ?? string.Empty;

                GravarNFSeEmDisco(xmlRet.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

                var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
                if (nota == null)
                {
                    notas.Load(retornoWebservice.XmlRetorno);
                }
                else
                {
                    nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
                    nota.IdentificacaoNFSe.Numero = numeroNFSe;
                    nota.IdentificacaoNFSe.Chave = chaveNFSe;
                    nota.XmlOriginal = retornoWebservice.XmlRetorno;
                }
            }
            catch { }
        }

        protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            var numeroNFSe = xmlRet.Root.ElementAnyNs("numero_nf")?.GetValue<string>() ?? string.Empty;
            var chaveNFSe = xmlRet.Root.ElementAnyNs("codigo")?.GetValue<string>() ?? string.Empty;
            var dataNFSe = xmlRet.Root.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.Now;
            var numeroRps = xmlRet.Root.ElementAnyNs("rps")?.GetValue<string>() ?? string.Empty;

            GravarNFSeEmDisco(xmlRet.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota == null)
            {
                nota = notas.Load(retornoWebservice.XmlRetorno);
            }
            else
            {
                nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.IdentificacaoNFSe.Chave = chaveNFSe;
                nota.XmlOriginal = retornoWebservice.XmlRetorno;
            }

            retornoWebservice.Nota = nota;
            retornoWebservice.Sucesso = true;
        }

        protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
        {
            var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == retornoWebservice.NumeroNFSe);

            retornoWebservice.Sucesso = true;
            nota.Situacao = SituacaoNFSeRps.Cancelado;
            nota.Cancelamento.MotivoCancelamento = retornoWebservice.Motivo;
            nota.Cancelamento.DataHora = retornoWebservice.Data;
        }

        public override NotaServico LoadXml(XDocument xml)
        {
            Guard.Against<XmlException>(xml == null, "Xml invalido.");
            XElement notaXml = xml.ElementAnyNs("notafiscal");
            Guard.Against<XmlException>(notaXml == null, "Xml de RPS ou NFSe invalido.");

            var nota = new NotaServico(Configuracoes);

            // Nota Fiscal
            nota.IdentificacaoNFSe.Numero = notaXml.ElementAnyNs("numero_nf")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoNFSe.Chave = notaXml.ElementAnyNs("codigo")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoNFSe.DataEmissao = notaXml.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.MinValue;

            // RPS
            nota.IdentificacaoRps.Numero = notaXml.ElementAnyNs("rps")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoRps.Serie = notaXml.ElementAnyNs("serie_rps")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoRps.Tipo = TipoRps.RPS;
            nota.IdentificacaoRps.DataEmissao = notaXml.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.MinValue;

            // Situação do RPS
            nota.Situacao = (notaXml.ElementAnyNs("cancelada")?.GetValue<string>() ?? string.Empty) == "S" ? SituacaoNFSeRps.Cancelado : SituacaoNFSeRps.Normal;

            // Serviços e Valores
            nota.Servico.Valores.ValorServicos = notaXml.ElementAnyNs("valor_servico")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorDeducoes = notaXml.ElementAnyNs("deducao")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorPis = notaXml.ElementAnyNs("valor_pis")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorCofins = notaXml.ElementAnyNs("valor_cofins")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorInss = notaXml.ElementAnyNs("valor_inss")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorIr = notaXml.ElementAnyNs("valor_irrf")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorCsll = notaXml.ElementAnyNs("valor_csll")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.IssRetido = (notaXml.ElementAnyNs("iss_retido")?.GetValue<string>() ?? string.Empty) == "S" ? SituacaoTributaria.Retencao : SituacaoTributaria.Normal;
            nota.Servico.Valores.ValorIss = notaXml.ElementAnyNs("valor_iss")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.BaseCalculo = notaXml.ElementAnyNs("valor_servico")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.Aliquota = notaXml.ElementAnyNs("aliq_iss")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorLiquidoNfse = notaXml.ElementAnyNs("valor_nf")?.GetValue<decimal>() ?? 0;
            nota.Servico.Valores.ValorIssRetido = nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? nota.Servico.Valores.ValorIss : 0;
            nota.Servico.ItemListaServico = notaXml.ElementAnyNs("id_codigo_servico")?.GetValue<string>() ?? string.Empty;
            nota.Servico.Discriminacao = notaXml.ElementAnyNs("descricao")?.GetValue<string>() ?? string.Empty;

            // Prestador
            nota.Prestador.CpfCnpj = notaXml.ElementAnyNs("cnpj_cpf_prestador")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.InscricaoMunicipal = notaXml.ElementAnyNs("im_prestador")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.RazaoSocial = notaXml.ElementAnyNs("razao_social_prestador")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Logradouro = notaXml.ElementAnyNs("endereco_prestador")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Numero = notaXml.ElementAnyNs("numero_ende_prestador")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Complemento = notaXml.ElementAnyNs("complemento_ende_prestador")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Bairro = notaXml.ElementAnyNs("bairro_prestador")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Municipio = notaXml.ElementAnyNs("cidade_prestador")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Uf = notaXml.ElementAnyNs("uf_prestador")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Cep = notaXml.ElementAnyNs("cep_prestador")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.DadosContato.Email = notaXml.ElementAnyNs("email_prestador")?.GetValue<string>() ?? string.Empty;

            // Tomador
            nota.Tomador.CpfCnpj = notaXml.ElementAnyNs("cnpj_cpf_destinatario")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.InscricaoMunicipal = notaXml.ElementAnyNs("im_destinatario")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.RazaoSocial = notaXml.ElementAnyNs("razao_social_destinatario")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.Logradouro = notaXml.ElementAnyNs("endereco_destinatario")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.Numero = notaXml.ElementAnyNs("numero_ende_destinatario")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.Complemento = notaXml.ElementAnyNs("complemento_ende_destinatario")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.Bairro = notaXml.ElementAnyNs("bairro_destinatario")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.Cep = notaXml.ElementAnyNs("cep_destinatario")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.Municipio = notaXml.ElementAnyNs("cidade_destinatario")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.Uf = notaXml.ElementAnyNs("uf_destinatario")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.Pais = notaXml.ElementAnyNs("pais_destinatario")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.DadosContato.Telefone = notaXml.ElementAnyNs("fone_destinatario")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.DadosContato.Email = notaXml.ElementAnyNs("email_destinatario")?.GetValue<string>() ?? string.Empty;

            return nota;
        }

        protected override string GerarCabecalho()
        {
            return string.Empty;
        }

        protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
        {
            return;
        }

        protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            return;
        }

        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "nfse", "", Certificado);
        }

        #region Não implementados

        public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true) => throw new NotImplementedException();

        protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

        protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice) => throw new NotImplementedException();

        protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) => throw new NotImplementedException();

        protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice) => throw new NotImplementedException();

        protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

        protected override void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

        protected override void AssinarEnviar(RetornoEnviar retornoWebservice) => throw new NotImplementedException();

        protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice) => throw new NotImplementedException();

        protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            //NAO PRECISA ASSINAR A CONSULTA
            return;
        }

        protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) => throw new NotImplementedException();

        protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice) => throw new NotImplementedException();

        protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice) => throw new NotImplementedException();

        protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice) => throw new NotImplementedException();

        protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

        protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice) => throw new NotImplementedException();

        protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas) 
        {
            try
            {
                var xmlDoc = new XmlDocument();
                //verifica se a mensagem eh xml para exibicao correta do erro
                xmlDoc.LoadXml(retornoWebservice.XmlRetorno);
            }
            catch
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = retornoWebservice.XmlRetorno });
                //LIMPA O XML RETORNO PARA NAO DAR ERRO DE PARSE MAIS ADIANTE
                retornoWebservice.XmlRetorno = null;
            }

            return;
        }

        protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) => throw new NotImplementedException();

        protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

        protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

        protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

        protected override string GetSchema(TipoUrl tipo) => throw new NotImplementedException();

        #endregion Não implementados
    }
}
