// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rodolfo Duarte
// Created          : 05-15-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="SaoPauloServiceClient.cs" company="OpenAC .Net">
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

using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class GIAPClient : NFSeHttpServiceClient, IServiceClient
{
    #region Constructors

    public GIAPClient(ProviderGIAP provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
    {
        AuthenticationScheme = AuthScheme.Custom;
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        return Execute(msg);
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        return Execute(msg);
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        return Execute(msg);
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    protected override void CustomAuthentication(HttpRequestHeaders requestHeaders)
    {
        requestHeaders.Add("Authorization", $"{Provider.Configuracoes.PrestadorPadrao.InscricaoMunicipal.ZeroFill(6)}-{(
            !string.IsNullOrWhiteSpace(Provider.Configuracoes.WebServices.ChaveAcesso) ? Provider.Configuracoes.WebServices.ChaveAcesso :
            !string.IsNullOrWhiteSpace(Provider.Configuracoes.WebServices.ChavePrivada) ? Provider.Configuracoes.WebServices.ChavePrivada :
            Provider.Configuracoes.WebServices.Usuario
        )}");
    }

    private string Execute(string message)
    {
        var content = new StringContent(message);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

        ExecutePost(content);

        return EnvelopeRetorno;
    }

    //public string TratarRetorno(XElement xmlDocument, string[] responseTag)
    //{
    //    var element = xmlDocument.ElementAnyNs("Fault");
    //    if (element != null)
    //    {
    //        var exMessage = $"{element.ElementAnyNs("Code")?.ElementAnyNs("Value")?.GetValue<string>()} - " +
    //                        $"{element.ElementAnyNs("Reason")?.ElementAnyNs("Text")?.GetValue<string>()}";

    //        throw new OpenDFeCommunicationException(exMessage);
    //    }

    //    return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("nfeResposta").Value;
    //}

    #endregion Methods
}