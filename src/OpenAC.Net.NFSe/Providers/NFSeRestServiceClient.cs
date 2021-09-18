// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Diego Martins
// Created          : 08-30-2021
//
// ***********************************************************************
// <copyright file="ProviderBase.cs" company="OpenAC .Net">
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
using System.IO;
using System.Net;

namespace OpenAC.Net.NFSe.Providers
{
    public abstract class NFSeRestServiceClient
    {
        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="tipoUrl"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected NFSeRestServiceClient(ProviderBase provider, TipoUrl tipoUrl)
        {
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

        #endregion Constructors

        #region Properties

        /// <summary>
        ///
        /// </summary>
        public ProviderBase Provider { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool EhHomologação => Provider.Configuracoes.WebServices.Ambiente == DFe.Core.Common.DFeTipoAmbiente.Homologacao;

        /// <summary>
        ///
        /// </summary>
        public string PrefixoEnvio { get; protected set; }

        /// <summary>
        ///
        /// </summary>
        public string PrefixoResposta { get; protected set; }

        /// <summary>
        ///
        /// </summary>
        public string EnvelopeEnvio { get; protected set; }

        /// <summary>
        ///
        /// </summary>
        public string EnvelopeRetorno { get; protected set; }

        #endregion Properties

        #region Methods

        protected virtual string Execute(string action)
        {
            return Execute(action, null, "GET");
        }

        protected virtual string Execute(string action, string message, string method)
        {
            try
            {
                var webRequest = CreateWebRequest(action, method);
                if (!string.IsNullOrEmpty(message))
                    using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                    {
                        streamWriter.Write(message);
                        streamWriter.Flush();
                    }
                var retornoStr = GetResponse(webRequest.GetResponse());
                return retornoStr;
            }
            catch (WebException ex)
            {
                var res = GetResponse(ex.Response);
                throw new Exception(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetResponse(WebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    var retorno = reader.ReadToEnd();
                    response.Close();
                    return retorno;
                }
            }
        }

        protected abstract WebRequest CreateWebRequest(string action, string method);

        #endregion Methods
    }
}