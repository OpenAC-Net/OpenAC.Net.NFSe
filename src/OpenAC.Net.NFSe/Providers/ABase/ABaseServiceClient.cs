// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 29-07-2022
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 29-07-2022
// ***********************************************************************
// <copyright file="ABaseServiceClient.cs" company="OpenAC .Net">
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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class ABaseServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public ABaseServiceClient(ProviderABase provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
        {
        }

        public ABaseServiceClient(ProviderABase provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfs:RecepcionarLoteRps>");
            message.Append("<nfs:nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfs:nfseCabecMsg>");
            message.Append("<nfs:nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfs:nfseDadosMsg>");
            message.Append("</nfs:RecepcionarLoteRps>");

            return Execute("http://nfse.abase.com.br/NFSeWS/RecepcionarLoteRps", message.ToString(), "RecepcionarLoteRpsResponse");
        }

        public string EnviarSincrono(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor ! Utilize o envio assincrono");

        public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfs:ConsultaLoteRps>");
            message.Append("<nfs:nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfs:nfseCabecMsg>");
            message.Append("<nfs:nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfs:nfseDadosMsg>");
            message.Append("</nfs:ConsultaLoteRps>");

            return Execute("http://nfse.abase.com.br/NFSeWS/ConsultaLoteRps", message.ToString(), "ConsultaLoteRpsResponse");
        }

        public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfs:ConsultaNfseRps>");
            message.Append("<nfs:nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfs:nfseCabecMsg>");
            message.Append("<nfs:nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfs:nfseDadosMsg>");
            message.Append("</nfs:ConsultaNfseRps>");

            return Execute("http://nfse.abase.com.br/NFSeWS/ConsultaNfseRps", message.ToString(), "ConsultaNfseRpsResponse");
        }

        public string ConsultarNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfs:CancelaNfse>");
            message.Append("<nfs:nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfs:nfseCabecMsg>");
            message.Append("<nfs:nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfs:nfseDadosMsg>");
            message.Append("</nfs:CancelaNfse>");

            return Execute("http://nfse.abase.com.br/NFSeWS/CancelaNfse", message.ToString(), "CancelaNfseResponse");
        }

        public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        private string Execute(string soapAction, string message, string responseTag)
        {
            return Execute(soapAction, message, "", responseTag, "xmlns:nfs=\"http://nfse.abase.com.br/NFSeWS\"");
        }

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element != null)
            {
                var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
                throw new OpenDFeCommunicationException(exMessage);
            }

            var reader = xmlDocument.ElementAnyNs(responseTag[0]).CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml().Replace("ns2:", string.Empty);
        }

        #endregion Methods


    }
}