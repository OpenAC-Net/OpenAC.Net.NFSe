// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 08-16-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="NotaCariocaServiceClient.cs" company="OpenAC .Net">
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
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;

namespace OpenAC.Net.NFSe.Providers
{
    // ReSharper disable once InconsistentNaming
    internal sealed class NotaCariocaServiceClient : NFSeSOAP11ServiceClient, IServiceClient
    {
        #region Constructors

        public NotaCariocaServiceClient(ProviderNotaCarioca provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
            if (!(Endpoint?.Binding is BasicHttpBinding binding))
                return;

            binding.MaxReceivedMessageSize = long.MaxValue;
            binding.MaxBufferPoolSize = long.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<not:RecepcionarLoteRpsRequest>");
            message.Append("<not:inputXML>");
            message.AppendCData(msg);
            message.Append("</not:inputXML>");
            message.Append("</not:RecepcionarLoteRpsRequest>");

            return Execute("http://notacarioca.rio.gov.br/RecepcionarLoteRps", message.ToString(), "RecepcionarLoteRpsResponse");
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<not:GerarNfseRequest>");
            message.Append("<not:inputXML>");
            message.AppendCData(msg);
            message.Append("</not:inputXML>");
            message.Append("</not:GerarNfseRequest>");

            return Execute("http://notacarioca.rio.gov.br/GerarNfse", message.ToString(), "GerarNfseResponse");
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<not:ConsultarSituacaoLoteRpsRequest>");
            message.Append("<not:inputXML>");
            message.AppendCData(msg);
            message.Append("</not:inputXML>");
            message.Append("</not:ConsultarSituacaoLoteRpsRequest>");

            return Execute("http://notacarioca.rio.gov.br/ConsultarSituacaoLoteRps", message.ToString(), "ConsultarSituacaoLoteRpsResponse");
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<not:ConsultarLoteRpsRequest>");
            message.Append("<not:inputXML>");
            message.AppendCData(msg);
            message.Append("</not:inputXML>");
            message.Append("</not:ConsultarLoteRpsRequest>");

            return Execute("http://notacarioca.rio.gov.br/ConsultarLoteRps", message.ToString(), "ConsultarLoteRpsResponse");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<not:ConsultarNfsePorRpsRequest>");
            message.Append("<not:inputXML>");
            message.AppendCData(msg);
            message.Append("</not:inputXML>");
            message.Append("</not:ConsultarNfsePorRpsRequest>");

            return Execute("http://notacarioca.rio.gov.br/ConsultarNfsePorRps", message.ToString(), "ConsultarNfsePorRpsResponse");
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<not:ConsultarNfseRequest>");
            message.Append("<not:inputXML>");
            message.AppendCData(msg);
            message.Append("</not:inputXML>");
            message.Append("</not:ConsultarNfseRequest>");

            return Execute("http://notacarioca.rio.gov.br/ConsultarNfse", message.ToString(), "ConsultarNfseResponse");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<not:CancelarNfseRequest>");
            message.Append("<not:inputXML>");
            message.AppendCData(msg);
            message.Append("</not:inputXML>");
            message.Append("</not:CancelarNfseRequest>");

            return Execute("http://notacarioca.rio.gov.br/CancelarNfse", message.ToString(), "CancelarNfseResponse");
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
            return Execute(soapAction, message, "", responseTag, "xmlns:not=\"http://notacarioca.rio.gov.br/\"");
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("outputXML").Value;
        }

        #endregion Methods
    }
}