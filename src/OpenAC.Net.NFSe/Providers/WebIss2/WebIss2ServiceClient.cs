// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 12-24-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 27-01-2020
// ***********************************************************************
// <copyright file="WebIss2ServiceClient.cs" company="OpenAC .Net">
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

using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class WebIss2ServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public WebIss2ServiceClient(ProviderWebIss2 provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap12)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:RecepcionarLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</e:RecepcionarLoteRpsRequest>");

            return Execute("http://nfse.abrasf.org.br/RecepcionarLoteRps", message.ToString(), "RecepcionarLoteRpsResponse");
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:RecepcionarLoteRpsSincronoRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</e:RecepcionarLoteRpsSincronoRequest>");

            return Execute("http://nfse.abrasf.org.br/RecepcionarLoteRpsSincrono", message.ToString(), "RecepcionarLoteRpsSincronoResponse");
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            throw new System.NotImplementedException();
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:ConsultarLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</e:ConsultarLoteRpsRequest>");

            return Execute("http://nfse.abrasf.org.br/ConsultarLoteRps", message.ToString(), "ConsultarLoteRpsResponse");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new System.NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:ConsultarNfsePorRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</e:ConsultarNfsePorRpsRequest>");

            return Execute("http://nfse.abrasf.org.br/ConsultarNfsePorRps", message.ToString(), "ConsultarNfsePorRpsResponse");
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:ConsultarNfseServicoPrestadoRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</e:ConsultarNfseServicoPrestadoRequest>");

            return Execute("http://nfse.abrasf.org.br/ConsultarNfseServicoPrestado", message.ToString(), "ConsultarNfseServicoPrestadoResponse");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:CancelarNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</e:CancelarNfseRequest>");

            return Execute("http://nfse.abrasf.org.br/CancelarNfse", message.ToString(), "CancelarNfseResponse");
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new System.NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:SubstituirNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</e:SubstituirNfseRequest>");

            return Execute("http://nfse.abrasf.org.br/SubstituirNfse", message.ToString(), "SubstituirNfseResponse");
        }

        private string Execute(string soapAction, string message, string responseTag)
        {
            return Execute(soapAction, message, "", responseTag, "xmlns:e=\"http://nfse.abrasf.org.br\"");
        }

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element != null)
            {
                var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
                throw new OpenDFeCommunicationException(exMessage);
            }

            return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("outputXML").Value;
        }

        #endregion Methods
    }
}