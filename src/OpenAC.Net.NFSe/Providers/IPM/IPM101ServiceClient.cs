// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 30-05-2022
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 27-08-2022
//
// ***********************************************************************
// <copyright file="IPMServiceClient.cs" company="OpenAC .Net">
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

using OpenAC.Net.DFe.Core;
using System;
using System.Net;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

public sealed class IPM101ServiceClient : NFSeMultiPartClient, IServiceClient
{
    #region Constructors

    public IPM101ServiceClient(ProviderBase provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
    {
        Charset = OpenEncoding.CP1252;
        FileNameForm = "filename";
        UseFormAuth = false;
        AuthenticationScheme  = AuthScheme.Basic;
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg) => throw new NotImplementedException();

    public string EnviarSincrono(string cabec, string msg) => Upload(msg, sendFormat: SendFormat.Binary);

    public string GerarNfse(string cabec, string msg) => throw new NotImplementedException();

    public string ConsultarLoteRps(string cabec, string msg) => Upload(msg, sendFormat: SendFormat.Binary);
    
    public string CancelarNFSe(string cabec, string msg) => Upload(msg, sendFormat: SendFormat.Binary);

    public string ConsultarNFSeRps(string cabec, string msg) => throw new NotImplementedException();

    public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException();

    public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException();

    public string ConsultarNFSe(string cabec, string msg) => throw new NotImplementedException();

    public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException();

    public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException();

    protected override string Authentication()
    {
        var usuarioWeb = Provider.Configuracoes.WebServices.Usuario.Trim();
        Guard.Against<OpenDFeException>(usuarioWeb.IsEmpty(), "O provedor necessita que a propriedade: Configuracoes.WebServices.Usuario seja informada.");

        var senhaWeb = Provider.Configuracoes.WebServices.Senha.Trim();
        Guard.Against<OpenDFeException>(senhaWeb.IsEmpty(), "O provedor necessita que a propriedade: Configuracoes.WebServices.Senha seja informada.");
        
        return (usuarioWeb + ":" + senhaWeb.HtmlEncode()).Base64Encode();
    }

    #endregion Methods
}

