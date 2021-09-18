// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 18-08-2021
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 18-08-2021
// ***********************************************************************
// <copyright file="SystemProServiceClient.cs" company="OpenAC .Net">
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
    internal sealed class SystemProServiceClient : NFSeSOAP11ServiceClient, IServiceClient
    {
        #region Constructors

        public SystemProServiceClient(ProviderSystemPro provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado)
        {
        }

        public SystemProServiceClient(ProviderSystemPro provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
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
            message.Append("<ns2:EnviarLoteRpsSincrono xmlns:ns2=\"http://NFSe.wsservices.systempro.com.br/\">");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ns2:EnviarLoteRpsSincrono>");

            return Execute("*", message.ToString(), "EnviarLoteRpsSincronoResponse");
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ns2:ConsultarLoteRps xmlns:ns2=\"http://NFSe.wsservices.systempro.com.br/\">");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ns2:ConsultarLoteRps>");

            return Execute("*", message.ToString(), "ConsultarLoteRpsResponse");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ns2:ConsultarNfseRps xmlns:ns2=\"http://NFSe.wsservices.systempro.com.br/\">");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ns2:ConsultarNfseRps>");

            return Execute("*", message.ToString(), "ConsultarNfseRpsResponse");
        }

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

            return Execute("*", message.ToString(), "ConsultarNfseFaixaResponse");
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

            return Execute("*", message.ToString(), "CancelarNfseResponse");
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        private string Execute(string action, string message, params string[] responseTag)
        {
            return Execute(action, message, responseTag, new string[0]);
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element == null)
                return xmlDocument.Root.ElementAnyNs("return")?.Value;

            var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
            throw new OpenDFeCommunicationException(exMessage);
        }

        //protected override Message WriteSoapEnvelope(string message, string soapAction, string soapHeader, string[] soapNamespaces)
        //{
        //    var envelope = new StringBuilder();
        //    envelope.Append("<Soap:Envelope xmlns:Soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
        //    envelope.Append("<Soap:Body>");
        //    envelope.Append(message);
        //    envelope.Append("</Soap:Body>");
        //    envelope.Append("</Soap:Envelope>");

        //    //envelope.Append("<S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">");
        //    //envelope.Append(soapHeader.IsEmpty() ? "<SOAP-ENV:Header/>" : $"<SOAP-ENV:Header>{soapHeader}</SOAP-ENV:Header>");
        //    //envelope.Append("<S:Body>");
        //    //envelope.Append(message);
        //    //envelope.Append("</S:Body>");
        //    //envelope.Append("</S:Envelope>");

        //    //Separei em uma variável para conseguir visualizar o envelope em formato XML durante a depuração
        //    string EnvelopeString = envelope.ToString();
        //    StringReader SR = new StringReader(EnvelopeString);
        //    XmlReader XmlR = XmlReader.Create(SR);
        //    var request = Message.CreateMessage(XmlR, int.MaxValue, Endpoint.Binding.MessageVersion);

        //    //Define a action no Header por ser SOAP 1.1
        //    var requestMessage = new HttpRequestMessageProperty();
        //    requestMessage.Headers["SOAPAction"] = soapAction;

        //    //Define a action no content type por ser SOAP 1.2
        //    //var requestMessage = new HttpRequestMessageProperty();
        //    //requestMessage.Headers["Content-Type"] = $"application/soap+xml;charset=UTF-8;action=\"{soapAction}\"";

        //    request.Properties[HttpRequestMessageProperty.Name] = requestMessage;

        //    return request;
        //}

        #endregion Methods
    }
}