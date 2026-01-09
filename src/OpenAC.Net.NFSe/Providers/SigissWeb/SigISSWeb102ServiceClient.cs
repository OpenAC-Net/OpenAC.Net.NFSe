// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Diego Martins
// Created          : 08-30-2021
//
// ***********************************************************************
// <copyright file="ProviderBase.cs" company="OpenAC .Net">
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Providers;

public class SigISSWeb102ServiceClient : NFSeRestServiceClient, IServiceClient
{
    private string? authToken;

    #region Constructors

    public SigISSWeb102ServiceClient(ProviderBase provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
    {
        AuthenticationScheme = AuthScheme.Custom;
    }

    #endregion Constructors

    #region Methods

    protected override void CustomAuthentication(HttpRequestHeaders requestHeaders)
    {
        var token = GetAuthToken();
        if (token.IsEmpty()) return;

        requestHeaders.Add("AUTHORIZATION", token);
    }

    public string EnviarSincrono(string cabec, string msg) => Post("/nfes", msg, HttpContentType.ApplicationXml);

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var xml = XDocument.Parse(msg);
        var numerorps = xml.Root?.ElementAnyNs("NumeroRPS")?.GetValue<string>();
        var serierps = xml.Root?.ElementAnyNs("SerieRPS")?.GetValue<string>();
        return Get($"/nfes/pegaxml/{numerorps}/serierps/{serierps}");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var xml = XDocument.Parse(msg);
        var numeronf = xml.Root?.ElementAnyNs("NumeroNFSe")?.GetValue<string>();
        var serie = xml.Root?.ElementAnyNs("SerieNFSe")?.GetValue<string>();
        var motivo = xml.Root?.ElementAnyNs("Motivo")?.GetValue<string>();
        return Get($"/nfes/cancela/{numeronf}/serie/{serie}/motivo/{motivo}");
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
        var url = Url;
        var authScheme = AuthenticationScheme;

        try
        {
            AuthenticationScheme = AuthScheme.None;
            Url = Provider.GetUrl(TipoUrl.Autenticacao);
            SetAction("/login");

            var login = Provider.Configuracoes.WebServices.Usuario?.Trim() ?? string.Empty;
            var senha = Provider.Configuracoes.WebServices.Senha?.Trim() ?? string.Empty;

            EnvelopeEnvio = "{ \"login\": \"" + login + "\"  , \"senha\":\"" + senha + "\"}";
            Execute(new StringContent(EnvelopeEnvio, Charset, "application/json"), HttpMethod.Post);
            return EnvelopeRetorno;
        }
        finally
        {
            AuthenticationScheme = authScheme;
            Url = url;
        }
    }

    private string GetAuthToken()
    {
        if (!authToken.IsEmpty()) return authToken;

        authToken = ExtractToken(Authentication());
        return authToken;
    }

    private static string ExtractToken(string response)
    {
        if (response.IsEmpty()) return string.Empty;

        var trimmed = response.Trim();
        if (trimmed.StartsWith("{") && trimmed.EndsWith("}"))
        {
            var match = Regex.Match(trimmed, "\"(token|access_token)\"\\s*:\\s*\"(?<token>[^\"]+)\"",
                RegexOptions.IgnoreCase);
            if (match.Success) return match.Groups["token"].Value;
        }

        return trimmed.Trim('"');
    }

    #endregion Methods
}
