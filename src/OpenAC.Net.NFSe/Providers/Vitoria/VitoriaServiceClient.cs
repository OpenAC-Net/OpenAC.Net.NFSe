// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 21-01-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 21-01-2020
// ***********************************************************************
// <copyright file="VitoriaServiceClient.cs" company="OpenAC .Net">
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

using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class VitoriaServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public VitoriaServiceClient(ProviderVitoria provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap12)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<RecepcionarLoteRps xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append("<mensagemXML>");
            message.AppendEnvio(msg);
            message.Append("</mensagemXML>");
            message.Append("</RecepcionarLoteRps>");

            return Execute("http://www.abrasf.org.br/nfse.xsd/RecepcionarLoteRps", message.ToString(), "RecepcionarLoteRps");
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<RecepcionarLoteRpsSincrono xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append("<mensagemXML>");
            message.AppendEnvio(msg);
            message.Append("</mensagemXML>");
            message.Append("</RecepcionarLoteRpsSincrono>");

            return Execute("http://www.abrasf.org.br/nfse.xsd/RecepcionarLoteRpsSincrono", message.ToString(), "RecepcionarLoteRpsSincrono");
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            throw new System.NotImplementedException();
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarLoteRps xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append("<mensagemXML>");
            message.AppendEnvio(msg);
            message.Append("</mensagemXML>");
            message.Append("</ConsultarLoteRps>");

            return Execute("http://www.abrasf.org.br/nfse.xsd/ConsultarLoteRps", message.ToString(), "ConsultarLoteRps");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new System.NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarNfsePorRps xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append("<mensagemXML>");
            message.AppendEnvio(msg);
            message.Append("</mensagemXML>");
            message.Append("</ConsultarNfsePorRps>");

            return Execute("http://www.abrasf.org.br/nfse.xsd/ConsultarNfsePorRps", message.ToString(), "ConsultarNfsePorRps");
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarNfseServicoPrestado xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append("<mensagemXML>");
            message.AppendEnvio(msg);
            message.Append("</mensagemXML>");
            message.Append("</ConsultarNfseServicoPrestado>");

            return Execute("http://www.abrasf.org.br/nfse.xsd/ConsultarNfseServicoPrestado", message.ToString(), "ConsultarNfseServicoPrestado");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<CancelarNfse xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append("<mensagemXML>");
            message.AppendEnvio(msg);
            message.Append("</mensagemXML>");
            message.Append("</CancelarNfse>");

            return Execute("http://www.abrasf.org.br/nfse.xsd/CancelarNfse", message.ToString(), "CancelarNfse");
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new System.NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<SubstituirNfse xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append("<mensagemXML>");
            message.AppendEnvio(msg);
            message.Append("</mensagemXML>");
            message.Append("</SubstituirNfse>");

            return Execute("http://www.abrasf.org.br/nfse.xsd/SubstituirNfse", message.ToString(), "SubstituirNfse");
        }

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element != null)
                throw new OpenDFeCommunicationException(element.ElementAnyNs("Reason").GetValue<string>());

            return xmlDocument.ElementAnyNs(responseTag[0] + "Response").ElementAnyNs(responseTag[0] + "Result").Value;
        }

        #endregion Methods
    }
}