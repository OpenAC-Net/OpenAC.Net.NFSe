// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 04-04-2022
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 14-04-2022
// ***********************************************************************
// <copyright file="ProviderSystemPro.cs" company="OpenAC .Net">
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

using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class ProviderAssessorPublico : ProviderABRASF201
    {
        #region Constructors

        public ProviderAssessorPublico(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "AssessorPublico";
        }

        #endregion Constructors

        #region Methods

        #region Protected Methods

        //protected override string GerarCabecalho()
        //{
        //    //TODOS OS TIPOS URL TEM O MESMO URL. USEI ENVIARSINCRONO, MAS PODERIA SER QUALQUER OUTRO
        //    string Url = this.GetUrl(TipoUrl.EnviarSincrono).Replace("?wsdl", "");

        //    var cabecalho = new System.Text.StringBuilder();
        //    cabecalho.Append("<cabecalho versao=\"1.00\" xmlns="+ Url + ">");
        //    cabecalho.Append("<versaoDados>1.00</versaoDados>");
        //    cabecalho.Append("</cabecalho>");
        //    return cabecalho.ToString();
        //}

        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            //NAO PRECISA ASSINAR
            //retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsEnvio", "LoteRps", Certificado);
        }

        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
            //NAO PRECISA ASSINAR
            //retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsSincronoEnvio", "LoteRps", Certificado);
        }

        private string GetLocalServico(NotaServico nota)
        {
            if (nota.Servico.MunicipioIncidencia == 0 || nota.Servico.MunicipioIncidencia == nota.Servico.CodigoMunicipio)
            {
                return "D";// D para dentro do município
            }
            else if (nota.Servico.CodigoPais > 0 && nota.Servico.CodigoPais != 1058)
            {
                return "P"; //P para fora do país
            }
            else
            {
                return "F"; //F para fora do município
            }
        }

        private string GetRetido(NotaServico nota) => !string.IsNullOrEmpty(nota.Servico.IdentifNaoExigibilidade) ? nota.Servico.IdentifNaoExigibilidade : "N";

        private string GetDentroPais(NotaServico nota) => nota.Tomador.Tipo == TipoTomador.Sigiss.JuridicaForaPais ? "N" : "S";

        private string FormataValor(decimal valor)
        {
            var numberFormat = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;
            if (decimal.TryParse(valor.ToString(), out var vDecimal)) return string.Format(numberFormat, "{0:0.00}", vDecimal);
            return "0.00";
        }

        protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Nenhuma RPS informada." });
            if (notas.Count > 1) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Apenas uma RPS pode ser enviada em modo Sincrono." });
            if (retornoWebservice.Erros.Count > 0) return;

            var xmlLote = new StringBuilder();
            xmlLote.Append("<NFSE>");
            xmlLote.Append("<IDENTIFICACAO>");
            xmlLote.Append($"<MESCOMP>{int.Parse(notas[0].IdentificacaoRps.DataEmissao.ToString("MM"))}</MESCOMP>");
            xmlLote.Append($"<ANOCOMP>{notas[0].IdentificacaoRps.DataEmissao.ToString("yyyy")}</ANOCOMP>");
            xmlLote.Append($"<INSCRICAO>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</INSCRICAO>");
            xmlLote.Append($"<VERSAO>1.00</VERSAO>");
            xmlLote.Append("</IDENTIFICACAO>");
            xmlLote.Append("<NOTAS>");
            int Sequencia = 1;
            foreach (NotaServico nota in notas)
            {
                if (string.IsNullOrEmpty(nota.Tomador.Endereco.TipoLogradouro))
                    nota.Tomador.Endereco.TipoLogradouro = "RUA";
                
                xmlLote.Append("<NOTA>");
                xmlLote.Append($"<LOTE>{nota.IdentificacaoRps.Numero}</LOTE>");
                xmlLote.Append($"<SEQUENCIA>{Sequencia}</SEQUENCIA>");
                xmlLote.Append($"<DATAEMISSAO>{nota.IdentificacaoRps.DataEmissao.ToString("dd/MM/yyyy")}</DATAEMISSAO>");
                xmlLote.Append($"<HORAEMISSAO>{nota.IdentificacaoRps.DataEmissao.ToString("HH:mm:ss")}</HORAEMISSAO>");
                xmlLote.Append($"<LOCAL>{GetLocalServico(nota)}</LOCAL>");
                if (GetLocalServico(nota) != "D")
                {
                    xmlLote.Append($"<UFFORA>{nota.Servico.UfIncidencia}</UFFORA>");
                    xmlLote.Append($"<MUNICIPIOFORA>{nota.Servico.MunicipioIncidencia}</MUNICIPIOFORA>");
                }
                xmlLote.Append("<SITUACAO>1</SITUACAO>");
                xmlLote.Append($"<RETIDO>{GetRetido(nota)}</RETIDO>");
                xmlLote.Append($"<ATIVIDADE>{nota.Servico.ItemListaServico}</ATIVIDADE>");
                xmlLote.Append($"<ALIQUOTAAPLICADA>{FormataValor(nota.Servico.Valores.Aliquota)}</ALIQUOTAAPLICADA>");
                xmlLote.Append($"<DEDUCAO>{FormataValor(nota.Servico.Valores.ValorDeducoes)}</DEDUCAO>");
                xmlLote.Append($"<IMPOSTO>{FormataValor(nota.Servico.Valores.ValorIss)}</IMPOSTO>");
                xmlLote.Append($"<RETENCAO>{FormataValor(nota.Servico.Valores.ValorIssRetido)}</RETENCAO>");
                xmlLote.Append($"<OBSERVACAO>{nota.OutrasInformacoes}</OBSERVACAO>");
                xmlLote.Append($"<CPFCNPJ>{nota.Tomador.CpfCnpj}</CPFCNPJ>");
                xmlLote.Append($"<NOMERAZAO>{nota.Tomador.RazaoSocial}</NOMERAZAO>");
                xmlLote.Append($"<NOMEFANTASIA>{nota.Tomador.NomeFantasia}</NOMEFANTASIA>");
                xmlLote.Append($"<MUNICIPIO>{nota.Tomador.Endereco.CodigoMunicipio}</MUNICIPIO>");
                xmlLote.Append($"<BAIRRO>{nota.Tomador.Endereco.Bairro}</BAIRRO>");
                xmlLote.Append($"<CEP>{nota.Tomador.Endereco.Cep}</CEP>");
                xmlLote.Append($"<PREFIXO>{nota.Tomador.Endereco.TipoLogradouro}</PREFIXO>");
                xmlLote.Append($"<LOGRADOURO>{nota.Tomador.Endereco.Logradouro}</LOGRADOURO>");
                xmlLote.Append($"<COMPLEMENTO>{nota.Tomador.Endereco.Complemento}</COMPLEMENTO>");
                xmlLote.Append($"<NUMERO>{nota.Tomador.Endereco.Numero}</NUMERO>");
                xmlLote.Append($"<EMAIL>{nota.Tomador.DadosContato.Email}</EMAIL>");
                xmlLote.Append($"<DENTROPAIS>{GetDentroPais(nota)}</DENTROPAIS>");
                xmlLote.Append($"<PIS>{FormataValor(nota.Servico.Valores.AliquotaPis)}</PIS>");
                xmlLote.Append("<RETPIS>N</RETPIS>");
                xmlLote.Append($"<COFINS>{FormataValor(nota.Servico.Valores.AliquotaCofins)}</COFINS>");
                xmlLote.Append("<RETCOFINS>N</RETCOFINS>");
                xmlLote.Append($"<INSS>{FormataValor(nota.Servico.Valores.AliquotaInss)}</INSS>");
                xmlLote.Append($"<IR>{FormataValor(nota.Servico.Valores.AliquotaIR)}</IR>");
                xmlLote.Append("<RETIR>N</RETIR>");
                xmlLote.Append($"<CSLL>{FormataValor(nota.Servico.Valores.AliquotaCsll)}</CSLL>");
                xmlLote.Append("<RETCSLL>N</RETCSLL>");
                xmlLote.Append("<SERVICOS>");
                xmlLote.Append("<SERVICO>");
                xmlLote.Append($"<DESCRICAO>{nota.Servico.Discriminacao}</DESCRICAO>");
                xmlLote.Append($"<VALORUNIT>{nota.Servico.Valores.ValorServicos}</VALORUNIT>");
                xmlLote.Append("<QUANTIDADE>1.00</QUANTIDADE>");
                xmlLote.Append("<DESCONTO>0.00</DESCONTO>");
                xmlLote.Append("</SERVICO>");
                xmlLote.Append("</SERVICOS>");
                xmlLote.Append("</NOTA>");
                Sequencia++;
            }
            xmlLote.Append("</NOTAS>");
            xmlLote.Append("</NFSE>");
            string xml = xmlLote.ToString();
            retornoWebservice.XmlEnvio = xml;
        }

        protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            if (retornoWebservice.NumeroRps < 1)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da RPS não informado para a consulta." });
                return;
            }

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<NFSE>");
            loteBuilder.Append("<IDENTIFICACAO>");
            loteBuilder.Append($"<INSCRICAO>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</INSCRICAO>");
            loteBuilder.Append($"<LOTE>{retornoWebservice.NumeroRps}</LOTE>");
            loteBuilder.Append("</IDENTIFICACAO>");
            loteBuilder.Append("</NFSE>");

            retornoWebservice.XmlEnvio = loteBuilder.ToString();
        }

        protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de etorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            var compNfse = xmlRet.ElementAnyNs("NFSE")?.ElementAnyNs("NOTA");
            var numeroNFSe = compNfse.ElementAnyNs("LINK")?.GetValue<string>() ?? string.Empty;
            if (string.IsNullOrEmpty(numeroNFSe))
            {
                retornoWebservice.Sucesso = false;
                return;
            }

            retornoWebservice.Sucesso = true;
        }

        protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            // Analisa mensagem de retorno
            retornoWebservice.Sucesso = retornoWebservice.XmlRetorno.Trim().IsNumeric();
        }

        protected override IServiceClient GetClient(TipoUrl tipo) => new AssessorPublicoServiceClient(this, tipo, Certificado);

        #endregion Protected Methods

        #region Abstract

        protected override bool PrecisaValidarSchema(TipoUrl tipo)
        {
            return false;
        }
        #endregion

        #endregion Methods
    }
}
