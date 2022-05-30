// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 30-05-2022
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 30-05-2022
//
// ***********************************************************************
// <copyright file="ProviderBase.cs" company="OpenAC .Net">
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
using System.Collections.Specialized;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;

namespace OpenAC.Net.NFSe.Providers
{
    public class IPMServiceClient : NFSeRestServiceClient, IServiceClient
    {
        #region Constructors

        public IPMServiceClient(ProviderBase provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string EnviarSincrono(string cabec, string msg) => Post(msg, "multipart/form-data");

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            throw new NotImplementedException();
            //var xml = XDocument.Parse(msg);
            //var numerorps = xml.Root?.ElementAnyNs("NumeroRPS")?.GetValue<string>();
            //var serierps = xml.Root?.ElementAnyNs("SerieRPS")?.GetValue<string>();
            //return Get($"/nfes/pegaxml/{numerorps}/serierps/{serierps}", "application/xml");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
            //var xml = XDocument.Parse(msg);
            //var numeronf = xml.Root?.ElementAnyNs("NumeroNFSe")?.GetValue<string>();
            //var serie = xml.Root?.ElementAnyNs("SerieNFSe")?.GetValue<string>();
            //var motivo = xml.Root?.ElementAnyNs("Motivo")?.GetValue<string>();
            //return Get($"/nfes/cancela/{numeronf}/serie/{serie}/motivo/{motivo}", "application/xml");
        }

        public string Enviar(string cabec, string msg) => throw new NotImplementedException();

        public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException();

        public string ConsultarLoteRps(string cabec, string msg) => throw new NotImplementedException();

        public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException();

        public string ConsultarNFSe(string cabec, string msg) => throw new NotImplementedException();

        public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException();

        public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException();

        protected override string Authentication()
        {
            string authenticationString = string.Concat(Provider.Configuracoes.WebServices.Usuario, ":", Provider.Configuracoes.WebServices.Senha);
            string base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
            return "Basic " + base64EncodedAuthenticationString;
        }

        protected string Post(string message, string contentyType)
        {
            var url = Url;

            try
            {
                var auth = Authentication();
                var headers = !auth.IsEmpty() ? new NameValueCollection { { AuthenticationHeader, auth } } : null;

                EnvelopeEnvio = message;

                Execute(contentyType, "POST", headers);
                return EnvelopeRetorno;
            }
            finally
            {
                Url = url;
            }
        }


        #endregion Methods
    }
}