// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Flávio Vodzinski
// Created          : 04-24-2024
//
// Last Modified By : Rafael Dias
// Last Modified On : 08-15-2024
// ***********************************************************************
// <copyright file="AgiliServiceClient.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2024 Projeto OpenAC .Net
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

internal class AgiliServiceClient : NFSeHttpServiceClient, IServiceClient
{
    #region Constructors

    public AgiliServiceClient(ProviderAgili provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg) => Execute(msg);

    public string EnviarSincrono(string cabec, string msg) => Execute(msg);

    public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException();

    public string ConsultarLoteRps(string cabec, string msg) => Execute(msg);

    public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException();

    public string ConsultarNFSeRps(string cabec, string msg) => Execute(msg);

    public string ConsultarNFSe(string cabec, string msg) => Execute(msg);

    public string CancelarNFSe(string cabec, string msg) => Execute(msg);

    public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException();

    public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException();

    private string Execute(string msg)
    {
        Execute(new StringContent(msg, Charset, HttpContentType.ApplicationXml), HttpMethod.Post);
        return EnvelopeRetorno;
    }

    #endregion Methods
}