// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 03-29-2023
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 03-29-2023
//
// ***********************************************************************
// <copyright file="IPM2ServiceClient.cs" company="OpenAC .Net">
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
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.Core;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Providers;

/// <summary>
/// Cliente de serviço para o provedor IPM 1.00, responsável por operações de envio e consulta de NFSe.
/// </summary>
public class IPM100ServiceClient : NFSeMultiPartClient, IServiceClient
{
    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="IPM100ServiceClient"/>.
    /// </summary>
    /// <param name="provider">Instância do provedor.</param>
    /// <param name="tipoUrl">Tipo de URL a ser utilizado.</param>
    public IPM100ServiceClient(ProviderBase provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
    {
        AuthenticationScheme = AuthScheme.Basic;
        UseFormAuth = false;
        FileNameForm = "f1";
    }

    #endregion Constructors

    #region Methods

    /// <inheritdoc />
    public string EnviarSincrono(string? cabec, string msg) => Upload(msg);

    /// <inheritdoc />
    public string ConsultarLoteRps(string? cabec, string msg) => Upload(msg);
    
    /// <inheritdoc />
    public string ConsultarNFSe(string? cabec, string msg) => Upload(msg);
    
    /// <inheritdoc />
    public string ConsultarNFSeRps(string? cabec, string msg) => throw new NotImplementedException();

    /// <inheritdoc />
    public string CancelarNFSe(string? cabec, string msg) => throw new NotImplementedException();

    /// <inheritdoc />
    public string Enviar(string? cabec, string msg) => throw new NotImplementedException();

    /// <inheritdoc />
    public string ConsultarSituacao(string? cabec, string msg) => throw new NotImplementedException();

    /// <inheritdoc />
    public string ConsultarSequencialRps(string? cabec, string msg) => throw new NotImplementedException();

    /// <inheritdoc />
    public string CancelarNFSeLote(string? cabec, string msg) => throw new NotImplementedException();

    /// <inheritdoc />
    public string SubstituirNFSe(string? cabec, string msg) => throw new NotImplementedException();
    
    /// <inheritdoc />
    protected override string Authentication() => (Provider.Configuracoes.WebServices.Usuario.Trim() + ":" + Provider.Configuracoes.WebServices.Senha.Trim()).Base64Encode();

    #endregion Methods
}