// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Diego Martins
// Created          : 08-30-2021
//
// Last Modified By : Rafael Dias
// Last Modified On : 27-08-2022
// ***********************************************************************
// <copyright file="NFSeRestServiceClient.cs" company="OpenAC .Net">
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
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using OpenAC.Net.Core.Extensions;

namespace OpenAC.Net.NFSe.Providers
{
    public abstract class NFSeRestServiceClient : NFSeHttpServiceClient
    {
        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="tipoUrl"></param>
        protected NFSeRestServiceClient(ProviderBase provider, TipoUrl tipoUrl) : base(provider, tipoUrl, provider.Certificado)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="tipoUrl"></param>
        /// <param name="certificado"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected NFSeRestServiceClient(ProviderBase provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado)
        {
        }

        #endregion Constructors

        #region Properties

        public string AuthenticationHeader { get; protected set; } = "Authorization";

        #endregion Properties

        #region Methods

        protected string Get(string action, string contentyType)
        {
            var url = Url;

            try
            {
                SetAction(action);
                EnvelopeEnvio = string.Empty;

                var auth = Authentication();
                var headers = !auth.IsEmpty() ? new NameValueCollection { { AuthenticationHeader, auth } } : null;

                Execute(contentyType, "GET", headers);
                return EnvelopeRetorno;
            }
            finally
            {
                Url = url;
            }
        }

        protected string Post(string action, string message, string contentyType)
        {
            var url = Url;

            try
            {
                SetAction(action);

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

        protected string Upload(string action, string message, bool useDefaultAuth = false, bool keepAlive = false)
        {
            var url = Url;

            try
            {
                SetAction(action);

                var auth = Authentication();
                var headers = !auth.IsEmpty() ? new NameValueCollection { { AuthenticationHeader, auth } } : null;

                EnvelopeEnvio = message;

                var fileName = $"{DateTime.Now:yyyyMMddssfff}_{PrefixoEnvio}_envio.xml";
                GravarSoap(EnvelopeEnvio, fileName);

                var arquivoEnvio = Path.Combine(Path.GetTempPath(), fileName);
                File.WriteAllText(arquivoEnvio, EnvelopeEnvio);

                var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                var boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                var request = WebRequest.CreateHttp(Url);
                request.Method = "POST";
                request.UseDefaultCredentials = useDefaultAuth;
                request.KeepAlive = keepAlive;

                request.ContentType = "multipart/form-data; boundary=" + boundary;

                if (!ValidarCertificadoServidor())
                    request.ServerCertificateValidationCallback += (_, _, _, _) => true;

                if (Provider.TimeOut.HasValue)
                    request.Timeout = Provider.TimeOut.Value.Milliseconds;

                if (headers?.Count > 0)
                    request.Headers.Add(headers);

                if (Certificado != null)
                    request.ClientCertificates.Add(Certificado);

                using (var streamWriter = request.GetRequestStream())
                {
                    streamWriter.Write(boundarybytes, 0, boundarybytes.Length);
                    var formitembytes = Encoding.UTF8.GetBytes(arquivoEnvio);
                    streamWriter.Write(formitembytes, 0, formitembytes.Length);

                    streamWriter.Write(boundarybytes, 0, boundarybytes.Length);

                    var headerTemplate =
                        $"Content-Disposition: form-data; name=\"file\"; filename=\"{fileName}\"\r\nContent-Type: text/xml\r\n\r\n";
                    var headerbytes = Encoding.UTF8.GetBytes(headerTemplate);
                    streamWriter.Write(headerbytes, 0, headerbytes.Length);

                    using (var fileStream = new FileStream(arquivoEnvio, FileMode.Open, FileAccess.Read))
                    {
                        int bytesRead;
                        var buffer = new byte[4096];
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                            streamWriter.Write(buffer, 0, bytesRead);
                    }

                    var trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                    streamWriter.Write(trailer, 0, trailer.Length);
                }

                var response = request.GetResponse();
                EnvelopeRetorno = GetResponse(response);

                GravarSoap(EnvelopeRetorno, $"{DateTime.Now:yyyyMMddssfff}_{PrefixoResposta}_retorno.xml");

                return EnvelopeRetorno;
            }
            finally
            {
                Url = url;
            }
        }

        protected virtual string Authentication() => "";

        protected void SetAction(string action)
        {
            if (Url == null) Url = "";
            Url = !Url.EndsWith("/") ? $"{Url}/{action}" : $"{Url}{action}";
        }

        #endregion Methods
    }
}