// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Diego Martins
// Created          : 08-29-2021
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="NFeCidadesServiceClient.cs" company="OpenAC .Net">
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

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class AmericanaServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public AmericanaServiceClient(ProviderAmericana provider, TipoUrl tipoUrl) : base(provider, tipoUrl, provider.Certificado, SoapVersion.Soap11)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfe:RecepcionarLoteRpsRequest>");
            message.Append("<nfe:inputXML>");
            message.AppendCData(msg);
            message.Append("</nfe:inputXML>");
            message.Append("</nfe:RecepcionarLoteRpsRequest>");

            return Execute("http://www.nfe.com.br/RecepcionarLoteRps", message.ToString(), "RecepcionarLoteRpsResponse");
        }

        public string EnviarSincrono(string cabec, string msg) => throw new NotImplementedException();

        public string ConsultarSituacao(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfe:ConsultarSituacaoLoteRpsRequest>");
            message.Append("<nfe:inputXML>");
            message.AppendCData(msg);
            message.Append("</nfe:inputXML>");
            message.Append("</nfe:ConsultarSituacaoLoteRpsRequest>");

            return Execute("http://www.nfe.com.br/ConsultarSituacaoLoteRps", message.ToString(), "ConsultarSituacaoLoteRpsResponse");
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfe:ConsultarLoteRpsRequest>");
            message.Append("<nfe:inputXML>");
            message.AppendCData(msg);
            message.Append("</nfe:inputXML>");
            message.Append("</nfe:ConsultarLoteRpsRequest>");

            return Execute("http://www.nfe.com.br/ConsultarLoteRps", message.ToString(), "ConsultarLoteRpsResponse");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfe:ConsultarNfsePorRpsRequest>");
            message.Append("<nfe:inputXML>");
            message.AppendCData(msg);
            message.Append("</nfe:inputXML>");
            message.Append("</nfe:ConsultarNfsePorRpsRequest>");

            return Execute("http://www.nfe.com.br/ConsultarNfsePorRps", message.ToString(), "ConsultarNfsePorRpsResponse");
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfe:ConsultarNfseRequest>");
            message.Append("<nfe:inputXML>");
            message.AppendCData(msg);
            message.Append("</nfe:inputXML>");
            message.Append("</nfe:ConsultarNfseRequest>");

            return Execute("http://www.nfe.com.br/ConsultarNfse", message.ToString(), "ConsultarNfseResponse");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfe:CancelarNfseRequest>");
            message.Append("<nfe:inputXML>");
            message.AppendCData(msg);
            message.Append("</nfe:inputXML>");
            message.Append("</nfe:CancelarNfseRequest>");

            return Execute("http://www.nfe.com.br/CancelarNfse", message.ToString(), "CancelarNfseResponse");
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        private string Execute(string soapAction, string message, string responseTag)
        {
            return Execute(soapAction, message, "", responseTag, "xmlns:nfe=\"http://www.nfe.com.br/\"", "xmlns=\"http://www.nfe.com.br/WSNacional/XSD/1/nfse_municipal_v01.xsd\"");
        }

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("outputXML").Value;
        }

        #endregion Methods
    }
}