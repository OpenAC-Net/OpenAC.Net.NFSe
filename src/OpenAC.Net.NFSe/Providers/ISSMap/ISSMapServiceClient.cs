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
using System.Net.Http;
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

    /// <summary>Envia a Carta de Cancelamento (POST).</summary>
    public string CancelarNFSe(string? cabec, string msg) => PostXml(msg);

    public string CancelarNFSeLote(string? cabec, string msg) => throw new NotImplementedException();

    public string SubstituirNFSe(string? cabec, string msg) => throw new NotImplementedException();

    private string PostXml(string msg)
    {
        EnvelopeEnvio = msg;
        Execute(new StringContent(msg, Charset, HttpContentType.ApplicationXml), HttpMethod.Post);
        return EnvelopeRetorno;
    }

    #endregion Methods
}
