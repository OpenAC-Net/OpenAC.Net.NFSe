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

using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

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

        public string EnviarSincrono(string cabec, string msg) => Post(msg);

        public string ConsultarNFSeRps(string cabec, string msg) => throw new NotImplementedException();

        public string CancelarNFSe(string cabec, string msg) => throw new NotImplementedException();

        public string Enviar(string cabec, string msg) => throw new NotImplementedException();

        public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException();

        public string ConsultarLoteRps(string cabec, string msg) => Post(msg);

        public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException();

        public string ConsultarNFSe(string cabec, string msg) => throw new NotImplementedException();

        public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException();

        public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException();

        public bool ValidarUsernamePassword()
        {
            return !string.IsNullOrEmpty(Provider.Configuracoes.WebServices.Usuario) && !string.IsNullOrEmpty(Provider.Configuracoes.WebServices.Senha);
        }

        protected override string Authentication()
        {
            var result = ValidarUsernamePassword();
            if (!result) throw new OpenDFeCommunicationException("Faltou informar username e/ou password");

            string authenticationString = string.Concat(Provider.Configuracoes.WebServices.Usuario, ":", Provider.Configuracoes.WebServices.Senha);
            string base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
            return "Basic " + base64EncodedAuthenticationString;
        }

        protected string Post(string message)
        {
            var url = Url;

            try
            {
                var auth = Authentication();
                var headers = !auth.IsEmpty() ? new NameValueCollection { { AuthenticationHeader, auth } } : null;

                EnvelopeEnvio = message;

                GravarSoap(EnvelopeEnvio, $"{DateTime.Now:yyyyMMddssfff}_{PrefixoEnvio}_envio.xml");

                string NomeArquivo = System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".xml");
                System.IO.File.WriteAllText(NomeArquivo, EnvelopeEnvio);

                HttpUploadFile(Url, NomeArquivo, "file", "image/jpeg", headers, auth);

                return EnvelopeRetorno;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                Url = url;
            }
        }

        private void HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc, string auth)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            wr.Headers.Add("Authorization", auth);

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                EnvelopeRetorno = GetResponse(wresp);
            }
            catch
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }

        #endregion Methods
    }
}
