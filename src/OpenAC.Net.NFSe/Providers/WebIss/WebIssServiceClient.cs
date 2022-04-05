// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 01-13-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="WebIssServiceClient.cs" company="OpenAC .Net">
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
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;

namespace OpenAC.Net.NFSe.Providers
{
    // ReSharper disable once InconsistentNaming
    internal sealed class WebIssServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public WebIssServiceClient(ProviderWebIss provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<RecepcionarLoteRps xmlns=\"http://tempuri.org/\">");
            message.Append("<cabec>");
            message.AppendCData(cabec);
            message.Append("</cabec>");
            message.Append("<msg>");
            message.AppendCData(msg);
            message.Append("</msg>");
            message.Append("</RecepcionarLoteRps>");

            return Execute("http://tempuri.org/INfseServices/RecepcionarLoteRps", message.ToString());
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarSituacaoLoteRps xmlns=\"http://tempuri.org/\">");
            message.Append("<cabec>");
            message.AppendCData(cabec);
            message.Append("</cabec>");
            message.Append("<msg>");
            message.AppendCData(msg);
            message.Append("</msg>");
            message.Append("</ConsultarSituacaoLoteRps>");

            return Execute("http://tempuri.org/INfseServices/ConsultarSituacaoLoteRps", message.ToString());
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarLoteRps xmlns=\"http://tempuri.org/\">");
            message.Append("<cabec>");
            message.AppendCData(cabec);
            message.Append("</cabec>");
            message.Append("<msg>");
            message.AppendCData(msg);
            message.Append("</msg>");
            message.Append("</ConsultarLoteRps>");

            return Execute("http://tempuri.org/INfseServices/ConsultarLoteRps", message.ToString());
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarNfsePorRps xmlns=\"http://tempuri.org/\">");
            message.Append("<cabec>");
            message.AppendCData(cabec);
            message.Append("</cabec>");
            message.Append("<msg>");
            message.AppendCData(msg);
            message.Append("</msg>");
            message.Append("</ConsultarNfsePorRps>");

            return Execute("http://tempuri.org/INfseServices/ConsultarNfsePorRps", message.ToString());
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarNfse xmlns=\"http://tempuri.org/\">");
            message.Append("<cabec>");
            message.AppendCData(cabec);
            message.Append("</cabec>");
            message.Append("<msg>");
            message.AppendCData(msg);
            message.Append("</msg>");
            message.Append("</ConsultarNfse>");

            return Execute("http://tempuri.org/INfseServices/ConsultarNfse", message.ToString());
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<CancelarNfse xmlns=\"http://tempuri.org/\">");
            message.Append("<cabec>");
            message.AppendCData(cabec);
            message.Append("</cabec>");
            message.Append("<msg>");
            message.AppendCData(msg);
            message.Append("</msg>");
            message.Append("</CancelarNfse>");

            return Execute("http://tempuri.org/INfseServices/CancelarNfse", message.ToString());
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        private string Execute(string soapAction, string message)
        {
            return Execute(soapAction, message, "");
        }

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element == null) return xmlDocument.ToString();

            var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
            throw new OpenDFeCommunicationException(exMessage);
        }

        #endregion Methods
    }
}