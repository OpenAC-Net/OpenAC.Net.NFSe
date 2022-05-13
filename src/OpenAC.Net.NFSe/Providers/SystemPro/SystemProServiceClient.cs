// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 18-08-2021
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 12-04-2022
// ***********************************************************************
// <copyright file="SystemProServiceClient.cs" company="OpenAC .Net">
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
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class SystemProServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public SystemProServiceClient(ProviderSystemPro provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg) => throw new NotImplementedException("Enviar nao implementada/suportada para este provedor.");

        public string EnviarSincrono(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ns2:EnviarLoteRpsSincrono xmlns:ns2=\"http://NFSe.wsservices.systempro.com.br/\">");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ns2:EnviarLoteRpsSincrono>");

            return Execute("", message.ToString(), "EnviarLoteRpsSincronoResponse");
        }

        public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException("ConsultarSituacao nao implementada/suportada para este provedor.");

        public string ConsultarLoteRps(string cabec, string msg) => throw new NotImplementedException("ConsultarLoteRps nao implementada/suportada para este provedor.");

        public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException("ConsultarSequencialRps nao implementada/suportada para este provedor.");

        public string ConsultarNFSeRps(string cabec, string msg) => throw new NotImplementedException("ConsultarNFSeRps nao implementada/suportada para este provedor.");

        public string ConsultarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ns2:ConsultarNfseFaixa xmlns:ns2=\"http://NFSe.wsservices.systempro.com.br/\">");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ns2:ConsultarNfseFaixa>");

            return Execute("", message.ToString(), "ConsultarNfseFaixaResponse");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ns2:CancelarNfse xmlns:ns2=\"http://NFSe.wsservices.systempro.com.br/\">");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ns2:CancelarNfse>");

            return Execute("", message.ToString(), "CancelarNfseResponse");
        }

        public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException();

        public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException();

        private string Execute(string action, string message, params string[] responseTag)
        {
            return Execute(action, message, responseTag, new string[0]);
        }

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element == null)
            {
                element = responseTag.Aggregate(xmlDocument, (current, tag) => current.ElementAnyNs(tag));
                return element.ToString();
            }

            var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
            throw new OpenDFeCommunicationException(exMessage);
        }

        #endregion Methods
    }
}