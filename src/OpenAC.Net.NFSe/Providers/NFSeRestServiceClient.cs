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
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using OpenAC.Net.Core.Extensions;

namespace OpenAC.Net.NFSe.Providers;

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

    #region Methods

    protected string Get(string action)
    {
        var url = Url;

        try
        {
            SetAction(action);
            EnvelopeEnvio = string.Empty;
            Execute(null, HttpMethod.Get);
            return EnvelopeRetorno;
        }
        finally
        {
            Url = url;
        }
    }

    protected string Post(string action, string message, string contentyType = "application/json")
    {
        var url = Url;

        try
        {
            SetAction(action);

            EnvelopeEnvio = message;
            Execute(new StringContent(message, Charset, contentyType), HttpMethod.Post);
            return EnvelopeRetorno;
        }
        finally
        {
            Url = url;
        }
    }

    protected string Upload(string action, string message)
    {
        var url = Url;

        try
        {
            SetAction(action);

            EnvelopeEnvio = message;

            var fileName = $"{DateTime.Now:yyyyMMddssfff}_{PrefixoEnvio}_envio.xml";
            GravarEnvio(EnvelopeEnvio, fileName);

            var requestContent = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(Charset.GetBytes(EnvelopeEnvio));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");

            requestContent.Add(fileContent, "file", fileName);

            Execute(requestContent, HttpMethod.Post);
            return EnvelopeRetorno;
        }
        finally
        {
            Url = url;
        }
    }

    protected void SetAction(string action)
    {
        Url ??= "";
        Url = !Url.EndsWith("/") ? $"{Url}/{action}" : $"{Url}{action}";
    }

    #endregion Methods
}