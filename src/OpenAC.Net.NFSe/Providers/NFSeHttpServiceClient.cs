// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 09-03-2022
//
// Last Modified By : Rafael Dias
// Last Modified On : 01-04-2023
// ***********************************************************************
// <copyright file="NFSeHttpServiceClient.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2023 Projeto OpenAC .Net
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Extensions;

namespace OpenAC.Net.NFSe.Providers;

public abstract class NFSeHttpServiceClient : IDisposable
{
    #region Inner Types

    public enum AuthScheme
    {
        [DFeEnum("Basic")]
        Basic,
        [DFeEnum("Bearer")]
        Bearer,
    }

    #endregion Inner Types
    
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

    public string PrefixoEnvio { get; }

    public string PrefixoResposta { get; }

    public string EnvelopeEnvio { get; protected set; }

    public string EnvelopeRetorno { get; protected set; }

    public ProviderBase Provider { get; set; }

    public bool EhHomologacao => Provider.Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Homologacao;

    protected string Url { get; set; }

    protected X509Certificate2 Certificado { get; set; }

    protected bool IsDisposed { get; private set; }

    protected AuthScheme AuthenticationScheme { get; set; } = AuthScheme.Basic;

    protected Encoding Charset { get; set; } = Encoding.UTF8;

    #endregion Properties

    #region Methods

    protected void Execute(HttpContent content, HttpMethod method)
    {
        try
        {
            Guard.Against<ArgumentNullException>(content == null && method == HttpMethod.Post, nameof(content));

            if (!EnvelopeEnvio.IsEmpty())
                GravarEnvio(EnvelopeEnvio, $"{DateTime.Now:yyyyMMddssfff}_{PrefixoEnvio}_envio.xml");

            var handler = new HttpClientHandler();

            if (!ValidarCertificadoServidor())
                handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            handler.SslProtocols = (SslProtocols)Provider.Configuracoes.WebServices.Protocolos;

            if (Certificado != null)
                handler.ClientCertificates.Add(Certificado);

            var client = new HttpClient(handler);

            if (Provider.TimeOut.HasValue)
                client.Timeout = Provider.TimeOut.Value;

            var request = new HttpRequestMessage(method, Url);

            var assemblyName = GetType().Assembly.GetName();
            var productValue = new ProductInfoHeaderValue(assemblyName.Name, assemblyName.Version.ToString());
            var commentValue = new ProductInfoHeaderValue("(+https://github.com/OpenAC-Net/OpenAC.Net.NFSe)");

            request.Headers.UserAgent.Add(productValue);
            request.Headers.UserAgent.Add(commentValue);

            var auth = Authentication();
            if (!auth.IsEmpty())
                request.Headers.Authorization = new AuthenticationHeaderValue(AuthenticationScheme.GetDFeValue(), auth);

            if (content != null)
                request.Content = content;

            var response = client.SendAsync(request).GetAwaiter().GetResult().EnsureSuccessStatusCode();
            EnvelopeRetorno = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            GravarEnvio(EnvelopeRetorno, $"{DateTime.Now:yyyyMMddssfff}_{PrefixoResposta}_retorno.xml");
            client.Dispose();
        }
        catch (Exception ex) when (ex is not OpenDFeCommunicationException)
        {
            throw new OpenDFeCommunicationException("Erro no Execute HttpContent => " + ex.Message, ex);
        }
    }

    protected virtual string Authentication() => "";

    protected virtual bool ValidarCertificadoServidor() => true;

    /// <summary>
    /// Salvar o arquivo xml no disco de acordo com as propriedades.
    /// </summary>
    /// <param name="conteudoArquivo"></param>
    /// <param name="nomeArquivo"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected virtual void GravarEnvio(string conteudoArquivo, string nomeArquivo)
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
            DisposeManaged();

        DisposeUnmanaged();
        IsDisposed = true;
    }

    #endregion Methods
}
