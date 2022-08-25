// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 30-05-2022
//
// Last Modified By : Rafael Dias
// Last Modified On : 02-07-2022
//
// ***********************************************************************
// <copyright file="IPMServiceClient.cs" company="OpenAC .Net">
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

using OpenAC.Net.DFe.Core;
using System;
using System.IO;
using System.Net;
using System.Text;

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

        public string EnviarSincrono(string cabec, string msg) => UploadIPM("", msg);

        public string ConsultarLoteRps(string cabec, string msg) => UploadIPM("", msg);

        public string ConsultarNFSeRps(string cabec, string msg) => throw new NotImplementedException();

        public string CancelarNFSe(string cabec, string msg) => throw new NotImplementedException();

        public string Enviar(string cabec, string msg) => throw new NotImplementedException();

        public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException();

        public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException();

        public string ConsultarNFSe(string cabec, string msg) => throw new NotImplementedException();

        public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException();

        public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException();

        public bool ValidarUsernamePassword() => !string.IsNullOrEmpty(Provider.Configuracoes.WebServices.Usuario) && !string.IsNullOrEmpty(Provider.Configuracoes.WebServices.Senha);

        protected override string Authentication()
        {
            var result = ValidarUsernamePassword();
            if (!result) throw new OpenDFeCommunicationException("Faltou informar username e/ou password");

            string authenticationString = string.Concat(Provider.Configuracoes.WebServices.Usuario, ":", Provider.Configuracoes.WebServices.Senha);
            string base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
            return "Basic " + base64EncodedAuthenticationString;
        }

        protected string UploadIPM(string action, string message)
        {
            var url = Url;

            try
            {
                var auth = Authentication();
                var fileName = $"{DateTime.Now:yyyyMMddssfff}_{PrefixoEnvio}_envio.xml";
                var arquivoEnvio = Path.Combine(Path.GetTempPath(), fileName);
                File.WriteAllText(arquivoEnvio, message);

                //Identificate separator
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                //Encoding
                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                //Creation and specification of the request
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url); //sVal is id for the webService
                wr.ContentType = "multipart/form-data; boundary=" + boundary;
                wr.Method = "POST";
                wr.KeepAlive = true;
                wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

                wr.Headers.Add("Authorization: " + auth); //AUTHENTIFICATION END
                Stream rs = wr.GetRequestStream();


                //string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}"; //For the POST's format

                //Writting of the file
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(arquivoEnvio);
                rs.Write(formitembytes, 0, formitembytes.Length);

                rs.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = $"Content-Disposition: form-data; name=\"file\"; filename=\"{fileName}\"\r\nContent-Type: text/xml\r\n\r\n";
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(headerTemplate);
                rs.Write(headerbytes, 0, headerbytes.Length);

                FileStream fileStream = new FileStream(arquivoEnvio, FileMode.Open, FileAccess.Read);
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
                rs = null;

                WebResponse wresp = null;
                try
                {
                    //Get the response
                    wresp = wr.GetResponse();
                    Stream stream2 = wresp.GetResponseStream();
                    StreamReader reader2 = new StreamReader(stream2);
                    EnvelopeRetorno = reader2.ReadToEnd();
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }
                finally
                {
                    if (wresp != null)
                    {
                        wresp.Close();
                        wresp = null;
                    }
                    wr = null;
                }

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