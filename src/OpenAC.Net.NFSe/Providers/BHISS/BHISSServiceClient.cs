// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 25-01-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 25-01-2020
// ***********************************************************************
// <copyright file="BHISSServiceClient.cs" company="OpenAC .Net">
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

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class BHISSServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public BHISSServiceClient(ProviderBHISS provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:RecepcionarLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:RecepcionarLoteRpsRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/RecepcionarLoteRps", message.ToString(), "RecepcionarLoteRpsResponse");
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:GerarNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:GerarNfseRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/GerarNfse", message.ToString(), "GerarNfseResponse");
        }

        public string CancelarNFSe(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:CancelarNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:CancelarNfseRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/CancelarNfse", message.ToString(), "CancelarNfseResponse");
        }

        public string ConsultarLoteRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:ConsultarLoteRpsRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/ConsultarLoteRps", message.ToString(), "ConsultarLoteRpsResponse");
        }

        public string ConsultarNFSe(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:ConsultarNfseRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/ConsultarNfse", message.ToString(), "ConsultarNfseResponse");
        }

        public string ConsultarNFSeRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarNfsePorRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:ConsultarNfsePorRpsRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/ConsultarNfsePorRps", message.ToString(), "ConsultarNfsePorRpsResponse");
        }

        public string ConsultarSituacao(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarSituacaoLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:ConsultarSituacaoLoteRpsRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/ConsultarSituacaoLoteRps", message.ToString(), "ConsultarSituacaoLoteRpsResponse");
        }

        public string ConsultarSequencialRps(string nfseCabecMsg, string nfseDadosMsg) => throw new NotImplementedException();

        public string CancelarNFSeLote(string nfseCabecMsg, string nfseDadosMsg) => throw new NotImplementedException();

        public string SubstituirNFSe(string nfseCabecMsg, string nfseDadosMsg) => throw new NotImplementedException();

        private string Execute(string action, string message, string responseTag) => Execute(action, message, "", responseTag, "xmlns:ws=\"http://ws.bhiss.pbh.gov.br\"");

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element == null) return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("outputXML").Value;
            var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";

            throw new OpenDFeCommunicationException(exMessage);
        }

        #endregion Methods
    }
}