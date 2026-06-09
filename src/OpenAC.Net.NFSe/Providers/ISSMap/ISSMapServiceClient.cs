// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : danilobreda
// Created          : 22-05-2026
//
// Last Modified By : danilobreda
// Last Modified On : 22-05-2026
// ***********************************************************************
// <copyright file="ISSMapServiceClient.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2026 Projeto OpenAC .Net
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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Providers;

/// <summary>
/// Cliente de comunicação REST com o WebService do provedor ISSMap (Gemmap Informática).
/// O ISSMap utiliza o padrão RESTFul sobre HTTPS, trafegando XML em <c>application/xml</c>.
/// </summary>
internal sealed class ISSMapServiceClient : NFSeHttpServiceClient, IServiceClient
{
    #region Constructors

    // O ISSMap não utiliza certificado digital (autenticação por chave AES simétrica).
    // Passa-se null explicitamente para não acionar o carregamento de certificado do OpenAC.
    public ISSMapServiceClient(ProviderISSMap provider, TipoUrl tipoUrl) : base(provider, tipoUrl, null)
    {
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Nome do arquivo informado pelo provedor no cabeçalho <c>Content-Disposition</c> da última
    /// resposta binária (ex.: <c>0000245.pdf</c>). Vazio quando não informado.
    /// </summary>
    public string NomeArquivo { get; private set; } = "";

    #endregion Properties

    #region Methods

    /// <summary>Envia o RPS (POST). A URL já contém o código da cidade do IssMap.</summary>
    public string Enviar(string? cabec, string msg) => PostXml(msg);

    public string EnviarSincrono(string? cabec, string msg) => PostXml(msg);

    public string ConsultarSituacao(string? cabec, string msg) => throw new NotImplementedException();

    public string ConsultarLoteRps(string? cabec, string msg) => throw new NotImplementedException();

    public string ConsultarSequencialRps(string? cabec, string msg) => throw new NotImplementedException();

    public string ConsultarNFSeRps(string? cabec, string msg) => throw new NotImplementedException();

    /// <summary>
    /// Consulta o RPS/NFSe (GET). O <paramref name="msg"/> é o sufixo de path
    /// (<c>/{docPrestador}/{docTomador}/{numeroRps}</c>) que complementa a URL base.
    /// </summary>
    public string ConsultarNFSe(string? cabec, string msg)
    {
        if (!string.IsNullOrEmpty(msg))
            Url = Url.TrimEnd('/') + msg;

        ExecuteGet();
        return EnvelopeRetorno;
    }

    /// <summary>
    /// Consulta a versão digital (PDF) do RPS/NFSe (GET). O <paramref name="msg"/> é o sufixo de
    /// path (<c>/{docPrestador}/{numeroRps}</c>) que complementa a URL base de QRCode.
    /// Retorna o conteúdo binário do PDF.
    /// </summary>
    public byte[] ConsultarNFSePdf(string msg)
    {
        if (!string.IsNullOrEmpty(msg))
            Url = Url.TrimEnd('/') + msg;

        return ExecuteGetBytes();
    }

    /// <summary>Envia a Carta de Cancelamento (POST).</summary>
    public string CancelarNFSe(string? cabec, string msg) => PostXml(msg);

    public string CancelarNFSeLote(string? cabec, string msg) => throw new NotImplementedException();

    public string SubstituirNFSe(string? cabec, string msg) => throw new NotImplementedException();

    /// <summary>
    /// Executa um GET HTTP cuja resposta é binária (ex.: PDF) e devolve os bytes recebidos.
    /// Reproduz a configuração de proxy/timeout/certificado de <see cref="NFSeHttpServiceClient.Execute"/>,
    /// porém lendo o corpo como <c>byte[]</c> (a leitura como string corromperia o binário). Também
    /// captura o nome do arquivo do cabeçalho <c>Content-Disposition</c> em <see cref="NomeArquivo"/>.
    /// </summary>
    private byte[] ExecuteGetBytes()
    {
        try
        {
            // O serviço de QRCode do ISSMap responde 303 redirecionando para uma URL HTTP (downgrade
            // de https para http). O HttpClient não segue automaticamente esse tipo de redirecionamento
            // por segurança, então o tratamento é feito manualmente (limitado a poucos saltos).
            var handler = new HttpClientHandler { AllowAutoRedirect = false };

            if (!ValidarCertificadoServidor())
                handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            if (Certificado != null)
                handler.ClientCertificates.Add(Certificado);

            if (!string.IsNullOrWhiteSpace(Provider.Configuracoes.WebServices.Proxy))
                handler.Proxy = new WebProxy(Provider.Configuracoes.WebServices.Proxy, true);

            using var client = new HttpClient(handler);

            if (Provider.TimeOut.HasValue)
                client.Timeout = Provider.TimeOut.Value;

            var assemblyName = GetType().Assembly.GetName();
            var urlAtual = new Uri(Url);
            HttpResponseMessage response;

            const int maxRedirecionamentos = 5;
            var saltos = 0;
            while (true)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, urlAtual);
                request.Headers.UserAgent.Add(new ProductInfoHeaderValue(assemblyName.Name!, assemblyName.Version!.ToString()));
                request.Headers.UserAgent.Add(new ProductInfoHeaderValue("(+https://github.com/OpenAC-Net/OpenAC.Net.NFSe)"));

                response = client.SendAsync(request).GetAwaiter().GetResult();

                var ehRedirect = (int)response.StatusCode is >= 300 and < 400;
                if (!ehRedirect || response.Headers.Location == null || ++saltos > maxRedirecionamentos)
                    break;

                urlAtual = response.Headers.Location.IsAbsoluteUri
                    ? response.Headers.Location
                    : new Uri(urlAtual, response.Headers.Location);
                response.Dispose();
            }

            response.EnsureSuccessStatusCode();

            NomeArquivo = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "";
            var bytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

            // EnvelopeRetorno é usado para diagnóstico/log; mantém apenas um resumo do binário.
            EnvelopeRetorno = $"[{response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream"}] {NomeArquivo} ({bytes.Length} bytes)";
            response.Dispose();
            return bytes;
        }
        catch (Exception ex) when (ex is not OpenDFeCommunicationException)
        {
            throw new OpenDFeCommunicationException("Erro no ExecuteGetBytes HttpContent => " + ex.Message, ex);
        }
    }

    private string PostXml(string msg)
    {
        EnvelopeEnvio = msg;

        // O WebService do ISSMap espera o header Content-Type EXATAMENTE como "application/xml".
        // O StringContent(msg, encoding, mediaType) do .NET sempre acrescenta "; charset=utf-8",
        // e o serviço, ao não reconhecer o content-type, ignora o corpo e responde 201 (campo Key
        // inválida). Por isso montamos o header manualmente, sem o parâmetro charset.
        var content = new StringContent(msg, Charset);
        content.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.ApplicationXml);

        Execute(content, HttpMethod.Post);
        return EnvelopeRetorno;
    }

    #endregion Methods
}
