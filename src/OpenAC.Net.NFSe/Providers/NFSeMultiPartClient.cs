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

using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

namespace OpenAC.Net.NFSe.Providers;

public abstract class NFSeMultiPartClient : NFSeHttpServiceClient
{
    #region Constructors

    protected NFSeMultiPartClient(ProviderBase provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
    {
    }

    protected NFSeMultiPartClient(ProviderBase provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado)
    {
    }

    #endregion Constructors

    #region Methods

    protected string Upload(string message)
    {
        var url = Url;

        try
        {
            EnvelopeEnvio = message;

            var UsuarioWeb = Provider.Configuracoes.WebServices.Usuario.Trim();
            Guard.Against<OpenDFeException>(UsuarioWeb.IsEmpty(), "O provedor necessita que a propriedade: Configuracoes.WebServices.Usuario seja informada.");

            var SenhaWeb = Provider.Configuracoes.WebServices.Senha.Trim();
            Guard.Against<OpenDFeException>(SenhaWeb.IsEmpty(), "O provedor necessita que a propriedade: Configuracoes.WebServices.Senha seja informada.");

            var form = new MultipartFormDataContent
            {
                { new StringContent(UsuarioWeb), "login" },
                { new StringContent(SenhaWeb), "senha" },
                { new ByteArrayContent(Charset.GetBytes(EnvelopeEnvio)), "file", $"{DateTime.Now:yyyyMMddssfff}_{PrefixoEnvio}_envio.xml" }
            };

            form.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml;charset=" + Charset.WebName);

            Execute(form, HttpMethod.Post);
            return EnvelopeRetorno;
        }
        finally
        {
            Url = url;
        }
    }

    #endregion Methods
}