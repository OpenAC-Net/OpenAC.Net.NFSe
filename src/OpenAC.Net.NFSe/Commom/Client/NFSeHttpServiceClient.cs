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
//	     		Copyright (c) 2014 - 2024 Projeto OpenAC .Net
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Extensions;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Commom.Client;

/// <summary>
/// Classe base abstrata para clientes de serviço HTTP utilizados na comunicação com provedores de NFSe.
/// Responsável por gerenciar requisições HTTP, autenticação, manipulação de certificados digitais,
/// gravação de arquivos de envio e retorno, além de permitir customizações para validação de certificados
/// e autenticação.
/// </summary>
/// <remarks>
/// Deve ser herdada por classes concretas que implementam a comunicação HTTP específica de cada provedor de NFSe,
/// podendo sobrescrever métodos para autenticação e validação de certificados conforme necessário.
/// </remarks>
public abstract class NFSeHttpServiceClient : IDisposable
{
    #region Inner Types

    /// <summary>
    /// Esquemas de autenticação suportados para requisições HTTP.
    /// </summary>
    public enum AuthScheme
    {
        /// <summary>
        /// Sem autenticação.
        /// </summary>
        [DFeEnum("None")]
        None,

        /// <summary>
        /// Autenticação Basic.
        /// </summary>
        [DFeEnum("Basic")]
        Basic,

        /// <summary>
        /// Autenticação Bearer (Token).
        /// </summary>
        [DFeEnum("Bearer")]
        Bearer,

        [DFeEnum("Custom")]
        Custom
    }

    #endregion Inner Types

    #region Constructors

    /// <summary>
    ///
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="tipoUrl"></param>
    protected NFSeHttpServiceClient(ProviderBase provider, TipoUrl tipoUrl) : this(provider, tipoUrl,
        provider.Certificado)
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="tipoUrl"></param>
    /// <param name="certificado"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected NFSeHttpServiceClient(ProviderBase provider, TipoUrl tipoUrl, X509Certificate2? certificado)
    {
        Certificado = certificado;
        Url = provider.GetUrl(tipoUrl).Replace("?wsdl", "") ?? throw new OpenDFeException("Url não encontrada.");
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

            case TipoUrl.CancelarNFSeLote:
                PrefixoEnvio = "canc-lote-nfse";
                PrefixoResposta = "canc-lote-nfse";
                break;
            case TipoUrl.Autenticacao:
                PrefixoEnvio = "aut-nfse";
                PrefixoResposta = "aut-nfse";
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(tipoUrl), tipoUrl, null);
        }
    }

    ~NFSeHttpServiceClient() => Dispose(false);

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Prefixo utilizado para gerar o nome do arquivo de envio.
    /// </summary>
    public string PrefixoEnvio { get; }

    /// <summary>
    /// Prefixo utilizado para gerar o nome do arquivo de resposta.
    /// </summary>
    public string PrefixoResposta { get; }

    /// <summary>
    /// Envelope XML que será enviado na requisição HTTP.
    /// </summary>
    public string EnvelopeEnvio { get; protected set; } = "";

    /// <summary>
    /// Envelope XML que será recebido na resposta da requisição HTTP.
    /// </summary>
    public string EnvelopeRetorno { get; protected set; } = "";

    /// <summary>
    /// Instância do provedor de NFSe associado a este cliente HTTP.
    /// </summary>
    public ProviderBase Provider { get; set; }

    /// <summary>
    /// Indica se o ambiente configurado para o provedor é de homologação.
    /// </summary>
    public bool EhHomologacao => Provider.Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Homologacao;

    /// <summary>
    /// URL do serviço HTTP utilizado para comunicação com o provedor de NFSe.
    /// </summary>
    protected string Url { get; set; }

    /// <summary>
    /// Certificado digital utilizado para autenticação do cliente HTTP, se necessário.
    /// </summary>
    protected X509Certificate2? Certificado { get; set; }

    /// <summary>
    /// Indica se o objeto já foi descartado (disposed).
    /// </summary>
    protected bool IsDisposed { get; private set; }

    /// <summary>
    /// Esquema de autenticação utilizado nas requisições HTTP.
    /// </summary>
    protected AuthScheme AuthenticationScheme { get; set; } = AuthScheme.None;

    /// <summary>
    /// Define o encoding (conjunto de caracteres) utilizado nas requisições HTTP.
    /// O padrão é UTF-8, mas pode ser alterado conforme a necessidade do provedor ou integração.
    /// </summary>
    protected Encoding Charset { get; set; } = Encoding.UTF8;

    #endregion Properties

    #region Methods

    /// <summary>
    /// Executa uma requisição HTTP GET utilizando as configurações do cliente.
    /// </summary>
    protected void ExecuteGet()
    {
        Execute(null, HttpMethod.Get);
    }

    /// <summary>
    /// Executa uma requisição HTTP POST utilizando as configurações do cliente.
    /// </summary>
    /// <param name="content">Conteúdo HTTP a ser enviado na requisição POST.</param>
    protected void ExecutePost(HttpContent content)
    {
        Execute(content, HttpMethod.Post);
    }

    /// <summary>
    /// Executa uma requisição HTTP utilizando o método e conteúdo especificados.
    /// </summary>
    /// <param name="content">Conteúdo HTTP a ser enviado na requisição (pode ser nulo para métodos que não enviam corpo).</param>
    /// <param name="method">Método HTTP a ser utilizado (GET, POST, etc).</param>
    protected void Execute(HttpContent? content, HttpMethod method)
    {
        try
        {
            if (content == null && method == HttpMethod.Post) throw new ArgumentNullException(nameof(content));

            if (!EnvelopeEnvio.IsEmpty())
                GravarEnvio(EnvelopeEnvio, $"{DateTime.Now:yyyyMMddssfff}_{PrefixoEnvio}_envio.xml");

            var handler = new HttpClientHandler();

            if (!ValidarCertificadoServidor())
                handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            handler.SslProtocols = (SslProtocols)Provider.Configuracoes.WebServices.Protocolos;

            if (Certificado != null)
                handler.ClientCertificates.Add(Certificado);

            if (!string.IsNullOrWhiteSpace(Provider.Configuracoes.WebServices.Proxy))
            {
                var webProxy = new WebProxy(Provider.Configuracoes.WebServices.Proxy, true);
                handler.Proxy = webProxy;
            }

            var client = new HttpClient(handler);

            if (Provider.TimeOut.HasValue)
                client.Timeout = Provider.TimeOut.Value;

            var request = new HttpRequestMessage(method, Url);

            var assemblyName = GetType().Assembly.GetName();
            var productValue = new ProductInfoHeaderValue(assemblyName.Name!, assemblyName.Version!.ToString());
            var commentValue = new ProductInfoHeaderValue("(+https://github.com/OpenAC-Net/OpenAC.Net.NFSe)");

            request.Headers.UserAgent.Add(productValue);
            request.Headers.UserAgent.Add(commentValue);

            switch (AuthenticationScheme)
            {
                case AuthScheme.Basic or AuthScheme.Bearer:
                    request.Headers.Authorization = new AuthenticationHeaderValue(AuthenticationScheme.GetDFeValue(), Authentication());
                    break;
                case AuthScheme.Custom:
                    CustomAuthentication(request.Headers);
                    break;
            }

            if (content != null)
                request.Content = content;

            var response = client.SendAsync(request).GetAwaiter().GetResult();
            EnvelopeRetorno = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            GravarEnvio(EnvelopeRetorno, $"{DateTime.Now:yyyyMMddssfff}_{PrefixoResposta}_retorno.xml");
            client.Dispose();
        }
        catch (Exception ex) when (ex is not OpenDFeCommunicationException)
        {
            throw new OpenDFeCommunicationException("Erro no Execute HttpContent => " + ex.Message, ex);
        }
    }

    /// <summary>
    /// Retorna a string de autenticação para ser utilizada no cabeçalho da requisição HTTP.
    /// Pode ser sobrescrito em classes derivadas para fornecer o token ou credencial apropriada,
    /// dependendo do esquema de autenticação configurado.
    /// </summary>
    /// <returns>String de autenticação ou vazia se não houver autenticação.</returns>
    protected virtual string Authentication() => "";

    /// <summary>
    /// Rotina customizada para Autenticação.
    /// Deve ser sobrescrita no caso de autenticação personalizada.
    /// </summary>
    protected virtual void CustomAuthentication(HttpRequestHeaders requestHeaders) {  }

    /// <summary>
    /// Valida o certificado do servidor durante a comunicação HTTP.
    /// Pode ser sobrescrito para implementar validação personalizada do certificado.
    /// Retorna <c>true</c> para validar normalmente, ou <c>false</c> para ignorar a validação.
    /// </summary>
    /// <returns><c>true</c> se a validação do certificado do servidor deve ser realizada; caso contrário, <c>false</c>.</returns>
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

        var path = Provider.Configuracoes.Arquivos.GetPathSoap(DateTime.Now,
            Provider.Configuracoes.PrestadorPadrao.CpfCnpj);
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