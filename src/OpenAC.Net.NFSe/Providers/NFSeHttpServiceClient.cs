// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 09-03-2022
//
// Last Modified By : Rafael Dias
// Last Modified On : 09-03-2022
// ***********************************************************************
// <copyright file="NFSeHttpServiceClient.cs" company="OpenAC .Net">
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
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Common;

namespace OpenAC.Net.NFSe.Providers
{
    public abstract class NFSeHttpServiceClient : IDisposable
    {
        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="tipoUrl"></param>
        protected NFSeHttpServiceClient(ProviderBase provider, TipoUrl tipoUrl) : this(provider, tipoUrl, provider.Certificado)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="tipoUrl"></param>
        /// <param name="certificado"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected NFSeHttpServiceClient(ProviderBase provider, TipoUrl tipoUrl, X509Certificate2 certificado)
        {
            Certificado = certificado;
            Url = provider.GetUrl(tipoUrl)?.Replace("?wsdl", "");
            Provider = provider;

            switch (tipoUrl)
            {
                case TipoUrl.Enviar:
                    PrefixoEnvio = "lot";
                    PrefixoResposta = "lot";
                    break;

                case TipoUrl.EnviarSincrono:
                    PrefixoEnvio = "lot-sinc";
                    PrefixoResposta = "lot-sinc";
                    break;

                case TipoUrl.ConsultarSituacao:
                    PrefixoEnvio = "env-sit-lot";
                    PrefixoResposta = "rec-sit-lot";
                    break;

                case TipoUrl.ConsultarLoteRps:
                    PrefixoEnvio = "con-lot";
                    PrefixoResposta = "con-lot";
                    break;

                case TipoUrl.ConsultarSequencialRps:
                    PrefixoEnvio = "seq-rps";
                    PrefixoResposta = "seq-rps";
                    break;

                case TipoUrl.ConsultarNFSeRps:
                    PrefixoEnvio = "con-rps-nfse";
                    PrefixoResposta = "con-rps-nfse";
                    break;

                case TipoUrl.ConsultarNFSe:
                    PrefixoEnvio = "con-nfse";
                    PrefixoResposta = "con-nfse";
                    break;

                case TipoUrl.CancelarNFSe:
                    PrefixoEnvio = "canc-nfse";
                    PrefixoResposta = "canc-nfse";
                    break;

                case TipoUrl.SubstituirNFSe:
                    PrefixoEnvio = "sub-nfse";
                    PrefixoResposta = "sub-nfse";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(tipoUrl), tipoUrl, null);
            }
        }

        ~NFSeHttpServiceClient() => Dispose(false);

        #endregion Constructors

        #region Properties

        public string PrefixoEnvio { get; protected set; }

        public string PrefixoResposta { get; protected set; }

        public string EnvelopeEnvio { get; protected set; }

        public string EnvelopeRetorno { get; protected set; }

        public ProviderBase Provider { get; set; }

        public bool EhHomologacao => Provider.Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Homologacao;

        protected string Url { get; set; }

        protected X509Certificate2 Certificado { get; set; }

        protected bool IsDisposed { get; private set; }

        #endregion Properties

        #region Methods

        protected void Execute(string contentType, string method, NameValueCollection headers = null)
        {
            var protocolos = ServicePointManager.SecurityProtocol;
            ServicePointManager.SecurityProtocol = Provider.Configuracoes.WebServices.Protocolos;

            try
            {
                if (!EnvelopeEnvio.IsEmpty())
                    GravarSoap(EnvelopeEnvio, $"{DateTime.Now:yyyyMMddssfff}_{PrefixoEnvio}_envio.xml");

                var request = WebRequest.CreateHttp(Url);
                request.Method = method.IsEmpty() ? "POST" : method;
                request.ContentType = contentType;

                if (!ValidarCertificadoServidor())
                    request.ServerCertificateValidationCallback += (_, _, _, _) => true;

                if (Provider.TimeOut.HasValue)
                    request.Timeout = Provider.TimeOut.Value.Milliseconds;

                if (headers?.Count > 0 && Provider.Name != NFSeProvider.Sigep.ToString())
                    request.Headers.Add(headers);

                if (!string.IsNullOrWhiteSpace(Provider.Configuracoes.WebServices.Proxy))
                {
                    var webProxy = new WebProxy(Provider.Configuracoes.WebServices.Proxy, true);
                    request.Proxy = webProxy;
                }

                if (Certificado != null)
                    request.ClientCertificates.Add(Certificado);

                if (!EnvelopeEnvio.IsEmpty())
                {
                    using var streamWriter = new StreamWriter(request.GetRequestStream());
                    streamWriter.Write(EnvelopeEnvio);
                    streamWriter.Flush();
                }

                var response = request.GetResponse();
                EnvelopeRetorno = GetResponse(response);

                GravarSoap(EnvelopeRetorno, $"{DateTime.Now:yyyyMMddssfff}_{PrefixoResposta}_retorno.xml");
            }
            catch (Exception ex) when (ex is not OpenDFeCommunicationException)
            {
                throw new OpenDFeCommunicationException(ex.Message, ex);
            }
            finally
            {
                ServicePointManager.SecurityProtocol = protocolos;
            }
        }

        protected static string GetResponse(WebResponse response)
        {
            var stream = response.GetResponseStream();
            Guard.Against<OpenDFeCommunicationException>(stream == null, "Erro ao ler retorno do servidor.");

            using (stream)
            {
                using var reader = new StreamReader(stream!);
                var retorno = reader.ReadToEnd();
                response.Close();
                return retorno;
            }
        }

        protected virtual bool ValidarCertificadoServidor() => true;

        /// <summary>
        /// Salvar o arquivo xml no disco de acordo com as propriedades.
        /// </summary>
        /// <param name="conteudoArquivo"></param>
        /// <param name="nomeArquivo"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected virtual void GravarSoap(string conteudoArquivo, string nomeArquivo)
        {
            if (Provider.Configuracoes.WebServices.Salvar == false) return;

            var path = Provider.Configuracoes.Arquivos.GetPathSoap(DateTime.Now, Provider.Configuracoes.PrestadorPadrao.CpfCnpj);
            nomeArquivo = Path.Combine(path, nomeArquivo);
            File.WriteAllText(nomeArquivo, conteudoArquivo, Encoding.UTF8);
        }

        /// <inheritdoc />
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Dispose all managed and unmanaged resources.
            Dispose(true);

            // Take this object off the finalization queue and prevent finalization code for this
            // object from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the managed resources implementing <see cref="IDisposable"/>.
        /// </summary>
        protected virtual void DisposeManaged()
        {
        }

        /// <summary>
        /// Disposes the unmanaged resources implementing <see cref="IDisposable"/>.
        /// </summary>
        protected virtual void DisposeUnmanaged()
        {
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources, called from the finalizer only.</param>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (IsDisposed)
                return;

            // If disposing managed and unmanaged resources.
            if (disposing)
            {
                DisposeManaged();
            }

            DisposeUnmanaged();

            IsDisposed = true;
        }

        #endregion Methods
    }
}