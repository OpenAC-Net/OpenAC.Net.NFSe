// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-22-2018
//
// Last Modified By : Rafael Dias
// Last Modified On : 05-22-2018
// ***********************************************************************
// <copyright file="BethaServiceClient.cs" company="OpenAC .Net">
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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class ISSNetServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public ISSNetServiceClient(ProviderISSNet provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap12)
        {
        }

        public ISSNetServiceClient(ProviderISSNet provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap12)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<RecepcionarLoteRps xmlns=\"http://www.issnetonline.com.br/webservice/nfd\">");
            message.Append("<xml>");
            message.AppendCData("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + msg);
            message.Append("</xml>");
            message.Append("</RecepcionarLoteRps>");

            return Execute("http://www.issnetonline.com.br/webservice/nfd/RecepcionarLoteRps", message.ToString(), "RecepcionarLoteRps");
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultaSituacaoLoteRPS xmlns=\"http://www.issnetonline.com.br/webservice/nfd\">");
            message.Append("<xml>");
            message.AppendCData("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + msg);
            message.Append("</xml>");
            message.Append("</ConsultaSituacaoLoteRPS>");

            return Execute("http://www.issnetonline.com.br/webservice/nfd/ConsultaSituacaoLoteRPS", message.ToString(), "ConsultaSituacaoLoteRPS");
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarLoteRps xmlns=\"http://www.issnetonline.com.br/webservice/nfd\">");
            message.Append("<xml>");
            message.AppendCData("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + msg);
            message.Append("</xml>");
            message.Append("</ConsultarLoteRps>");

            return Execute("http://www.issnetonline.com.br/webservice/nfd/ConsultarLoteRps", message.ToString(), "ConsultarLoteRps");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarNFSePorRPS xmlns=\"http://www.issnetonline.com.br/webservice/nfd\">");
            message.Append("<xml>");
            message.AppendCData("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + msg);
            message.Append("</xml>");
            message.Append("</ConsultarNFSePorRPS>");

            return Execute("http://www.issnetonline.com.br/webservice/nfd/ConsultarNFSePorRPS", message.ToString(), "ConsultarNFSePorRPS");
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarNfse xmlns=\"http://www.issnetonline.com.br/webservice/nfd\">");
            message.Append("<xml>");
            message.AppendCData("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + msg);
            message.Append("</xml>");
            message.Append("</ConsultarNfse>");

            return Execute("http://www.issnetonline.com.br/webservice/nfd/ConsultarNfse", message.ToString(), "ConsultarNfse");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<CancelarNfse xmlns=\"http://www.issnetonline.com.br/webservice/nfd\">");
            message.Append("<xml>");
            message.AppendCData("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + msg);
            message.Append("</xml>");
            message.Append("</CancelarNfse>");

            return Execute("http://www.issnetonline.com.br/webservice/nfd/CancelarNfse", message.ToString(), "CancelarNfse");
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        private string Execute(string action, string message, string responseTag)
        {
            return Execute(action, message, responseTag, new string[0]);
        }

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element != null)
            {
                var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
                throw new OpenDFeCommunicationException(exMessage);
            }

            return xmlDocument.ElementAnyNs(responseTag[0] + "Response").ElementAnyNs(responseTag[0] + "Result").Value;
        }

        #endregion Methods
    }
}