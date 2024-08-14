// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Adriano Trentim
// Created          : 01-13-2024
//
// Last Modified By : Adriano Trentim
// Last Modified On : 01-13-2024
// ***********************************************************************
// <copyright file="ISSIntegraServiceClient.cs" company="OpenAC .Net">
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
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ISSIntegraServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    private string[] _namespaces = ["xmlns:api=\"https://www.issintegra.com.br/webservices/abrasf/wsdl\"", "xmlns:e=\"http://www.abrasf.org.br/nfse.xsd\""
    ];

    public ISSIntegraServiceClient(ProviderISSIntegra provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<api:RecepcionarLoteRps>");
        message.Append(msg.Replace("EnviarLoteRpsEnvio", "e:EnviarLoteRpsEnvio"));
        message.Append("</api:RecepcionarLoteRps>");

        return Execute("RecepcionarLoteRps", message.ToString(), "RecepcionarLoteRpsResult", _namespaces);
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<api:ConsultarSituacaoLoteRps>");
        message.Append(msg.Replace("ConsultarSituacaoLoteRpsEnvio", "e:ConsultarSituacaoLoteRpsEnvio"));
        message.Append("</api:ConsultarSituacaoLoteRps>");

        return Execute("ConsultarSituacaoLoteRps", message.ToString(), "ConsultarSituacaoLoteRpsResult", _namespaces);
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<api:ConsultarSituacaoLoteRps>");
        message.Append(msg.Replace("ConsultarLoteRpsEnvio", "e:ConsultarLoteRpsEnvio"));
        message.Append("</api:ConsultarSituacaoLoteRps>");

        return Execute("ConsultarSituacaoLoteRps", message.ToString(), "ConsultarLoteRpsResult", _namespaces);
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<api:ConsultarNfsePorRps>");
        message.Append(msg.Replace("ConsultarNfsePorRpsEnvio", "e:ConsultarNfsePorRpsEnvio"));
        message.Append("</api:ConsultarNfsePorRps>");

        return Execute("ConsultarNfsePorRps", message.ToString(), "ConsultarNfsePorRpsResult", _namespaces);
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<api:ConsultarNfse>");
        message.Append(msg.Replace("ConsultarNfseEnvio", "e:ConsultarNfseEnvio"));
        message.Append("</api:ConsultarNfse>");

        return Execute("ConsultarNfse", message.ToString(), "ConsultarNfseResult", _namespaces);
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<api:CancelarNfse>");
        message.Append(msg.Replace("CancelarNfseEnvio", "e:CancelarNfseEnvio"));
        message.Append("</api:CancelarNfse>");

        return Execute("CancelarNfse", message.ToString(), "CancelarNfseResult", _namespaces);
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    private string Execute(string soapAction, string message, string responseTag, string[] namespaces) =>
        Execute(soapAction, message, "", [responseTag], namespaces);

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element == null) return xmlDocument.FirstNode?.ToString(); 

        var exMessage = $"{element.ElementAnyNs("Code").GetValue<string>()} - {element.ElementAnyNs("Reason").GetValue<string>()}";
        throw new OpenDFeCommunicationException(exMessage);
    }

    #endregion Methods
}