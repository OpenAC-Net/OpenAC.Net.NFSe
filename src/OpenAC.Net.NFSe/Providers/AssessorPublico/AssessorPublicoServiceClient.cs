// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 04-04-2022
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 04-14-2022
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
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class AssessorPublicoServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public AssessorPublicoServiceClient(ProviderAssessorPublico provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, SoapVersion.Soap12)
        {
            //MessageVersion = SoapVersion.Soap11;
        }

        #endregion Constructors

        #region Methods

        private static string GeraHashMD5(string texto)
        {
            byte[] btyScr = System.Text.ASCIIEncoding.ASCII.GetBytes(texto);

            System.Security.Cryptography.MD5CryptoServiceProvider ObjMd5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] BtyRes = ObjMd5.ComputeHash(btyScr);
            int Parte1 = BtyRes.Length * 2;
            int Parte2 = BtyRes.Length / 8; //esta certo aqui sem (decimal)
            int intTotal = Parte1 + Parte2;
            StringBuilder strRes = new StringBuilder(intTotal);

            for (int intI = 0; intI <= BtyRes.Length - 1; intI++)
            {
                strRes.Append(BitConverter.ToString(BtyRes, intI, 1));
            }
            ObjMd5?.Dispose();

            return (strRes.ToString().TrimEnd(new char[] { ' ' })).ToLowerInvariant(); ;
        }

        public string Enviar(string cabec, string msg) => throw new NotImplementedException("Enviar nao implementada/suportada para este provedor.");

        public string EnviarSincrono(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfse:Nfse.Execute>");
            message.Append("<nfse:Operacao>1</nfse:Operacao>");
            message.Append($"<nfse:Usuario>{Provider.Configuracoes.WebServices.Usuario}</nfse:Usuario>");
            message.Append($"<nfse:Senha>{GeraHashMD5(Provider.Configuracoes.WebServices.Senha)}</nfse:Senha>");
            message.Append("<nfse:Webxml>");
            message.Append(msg.Replace("<", "&lt;").Replace(">", "&gt;"));
            message.Append("</nfse:Webxml>");
            message.Append("</nfse:Nfse.Execute>");

            return Execute("", message.ToString(), "EnviarLoteRpsSincronoResponse");
        }

        public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException("ConsultarSituacao nao implementada/suportada para este provedor.");

        public string ConsultarLoteRps(string cabec, string msg) => throw new NotImplementedException("ConsultarLoteRps nao implementada/suportada para este provedor.");

        public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException("ConsultarSequencialRps nao implementada/suportada para este provedor.");

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfse:Nfse.Execute>");
            message.Append("<nfse:Operacao>3</nfse:Operacao>");
            message.Append($"<nfse:Usuario>{Provider.Configuracoes.WebServices.Usuario}</nfse:Usuario>");
            message.Append($"<nfse:Senha>{GeraHashMD5(Provider.Configuracoes.WebServices.Senha)}</nfse:Senha>");
            message.Append("<nfse:Webxml>");
            message.Append(msg.Replace("<", "&lt;").Replace(">", "&gt;"));
            message.Append("</nfse:Webxml>");
            message.Append("</nfse:Nfse.Execute>");

            return Execute("", message.ToString(), "EnviarLoteRpsSincronoResponse");
        }

        public string ConsultarNFSe(string cabec, string msg) => throw new NotImplementedException("ConsultarNFSe nao implementada/suportada para este provedor.");

        public string CancelarNFSe(string cabec, string msg) => throw new NotImplementedException();

        public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException();

        public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException();

        private string Execute(string action, string message, params string[] responseTag)
        {
            var result = ValidarUsernamePassword();
            if (!result) throw new DFe.Core.OpenDFeCommunicationException("Faltou informar username e/ou password");

            return Execute(action, message, responseTag, new string[0]);
        }

        private bool ValidarUsernamePassword()
        {
            return !string.IsNullOrEmpty(Provider.Configuracoes.WebServices.Usuario) && !string.IsNullOrEmpty(Provider.Configuracoes.WebServices.Senha);
        }

        protected override string Execute(string soapAction, string message, string soapHeader, string[] responseTag, params string[] soapNamespaces)
        {
            string contetType;
            NameValueCollection headers;
            //switch (MessageVersion)
            //{
            //    case SoapVersion.Soap11:
            //        contetType = $"text/xml; charset={CharSet}";
            //        headers = new NameValueCollection { { "SOAPAction", soapAction } };
            //        break;

            //    case SoapVersion.Soap12:
            contetType = $"application/soap+xml; charset={CharSet};action={soapAction}";
            headers = null;
            //        break;

            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}

            var envelope = new StringBuilder();
            envelope.Append("<soap:Envelope xmlns:nfse=\"nfse\" xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\">");

            envelope.Append("<soap:Header/>");
            envelope.Append("<soap:Body>");
            envelope.Append(message);
            envelope.Append("</soap:Body>");
            envelope.Append("</soap:Envelope>");
            EnvelopeEnvio = envelope.ToString();

            Execute(contetType, "POST", headers);

            var xmlDocument = XDocument.Parse(EnvelopeRetorno);
            var body = xmlDocument.ElementAnyNs("Envelope");
            var EnvelopeBody = xmlDocument.ElementAnyNs("Envelope")?.ElementAnyNs("Body")?.ElementAnyNs("Nfse.ExecuteResponse");
            if (EnvelopeBody != null)
                body = EnvelopeBody;

            return TratarRetorno(body, responseTag);
        }

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            var element = xmlDocument?.ElementAnyNs("Mensagem");
            if (element == null)
                return xmlDocument.ToString();
            else
                return xmlDocument.ElementAnyNs("Mensagem").GetValue<string>();
        }

        #endregion Methods
    }
}
