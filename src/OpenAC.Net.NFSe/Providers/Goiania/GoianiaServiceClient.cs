// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 07-30-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-30-2017
// ***********************************************************************
// <copyright file="IBetha2Service.cs" company="OpenAC .Net">
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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class GoianiaServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public GoianiaServiceClient(ProviderGoiania provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:GerarNfse>");
            message.Append("<ws:ArquivoXML>");
            message.AppendCData(msg);
            message.Append("</ws:ArquivoXML>");
            message.Append("</ws:GerarNfse>");

            return Execute("http://nfse.goiania.go.gov.br/ws/GerarNfse", new[] { "GerarNfseResponse", "GerarNfseResult" }, message.ToString());
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarNfseRps>");
            message.Append("<ws:ArquivoXML>");
            message.AppendCData(msg);
            message.Append("</ws:ArquivoXML>");
            message.Append("</ws:ConsultarNfseRps>");

            return Execute("http://nfse.goiania.go.gov.br/ws/ConsultarNfseRps", new[] { "ConsultarNfseRpsResponse", "ConsultarNfseRpsResult" }, message.ToString());
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        private string Execute(string soapAction, string[] responseTag, string message)
        {
            return Execute(soapAction, message, "", responseTag, "xmlns:ws=\"http://nfse.goiania.go.gov.br/ws/\"");
        }

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element == null) return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs(responseTag[1]).Value;

            var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
            throw new OpenDFeCommunicationException(exMessage);
        }

        #endregion Methods
    }
}