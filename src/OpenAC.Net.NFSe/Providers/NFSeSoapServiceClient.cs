// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 09-03-2022
//
// Last Modified By : Rafael Dias
// Last Modified On : 09-03-2022
// ***********************************************************************
// <copyright file="NFSeSoapServiceClient.cs" company="OpenAC .Net">
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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers;

public abstract class NFSeSoapServiceClient : NFSeHttpServiceClient
{
    #region Inner Types

    public enum SoapVersion
    {
        Soap11,
        Soap12,
    }

    #endregion Inner Types

    #region Constructors

    /// <summary>
    ///
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="tipoUrl"></param>
    protected NFSeSoapServiceClient(ProviderBase provider, TipoUrl tipoUrl, SoapVersion message) : base(provider, tipoUrl, provider.Certificado)
    {
        Guard.Against<ArgumentException>(!Enum.IsDefined(typeof(SoapVersion), message), "Versão Soap não definida.");

        MessageVersion = message;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="tipoUrl"></param>
    /// <param name="certificado"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected NFSeSoapServiceClient(ProviderBase provider, TipoUrl tipoUrl, X509Certificate2 certificado, SoapVersion message) : base(provider, tipoUrl, certificado)
    {
        Guard.Against<ArgumentException>(!Enum.IsDefined(typeof(SoapVersion), message), "Versão Soap não definida.");

        MessageVersion = message;
    }

    #endregion Constructors

    #region Properties

    protected SoapVersion MessageVersion { get; }

    protected Encoding CharSet { get; set; } = Encoding.UTF8;

    #endregion Properties

    #region Methods

    protected virtual string Execute(string soapAction, string message, string responseTag, params string[] soapNamespaces)
    {
        return Execute(soapAction, message, string.Empty, new[] { responseTag }, soapNamespaces);
    }

    protected virtual string Execute(string soapAction, string message, string[] responseTag, params string[] soapNamespaces)
    {
        return Execute(soapAction, message, string.Empty, responseTag, soapNamespaces);
    }

    protected virtual string Execute(string soapAction, string message, string soapHeader, string responseTag, params string[] soapNamespaces)
    {
        return Execute(soapAction, message, soapHeader, new[] { responseTag }, soapNamespaces);
    }

    protected virtual string Execute(string soapAction, string message, string soapHeader, string[] responseTag, params string[] soapNamespaces)
    {
        var envelope = new StringBuilder();
        switch (MessageVersion)
        {
            case SoapVersion.Soap11:
                envelope.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\"");
                break;

            case SoapVersion.Soap12:
                envelope.Append("<soapenv:Envelope xmlns:soapenv=\"http://www.w3.org/2003/05/soap-envelope\"");
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        envelope.Append(soapNamespaces.Aggregate("", (atual, next) => atual + $" {next}", namespaces => namespaces + ">"));
        envelope.Append(soapHeader.IsEmpty() ? "<soapenv:Header/>" : $"<soapenv:Header>{soapHeader}</soapenv:Header>");
        envelope.Append("<soapenv:Body>");
        envelope.Append(message);
        envelope.Append("</soapenv:Body>");
        envelope.Append("</soapenv:Envelope>");
        EnvelopeEnvio = envelope.ToString();

        StringContent content;
        switch (MessageVersion)
        {
            case SoapVersion.Soap11:
                content = new StringContent(EnvelopeEnvio, CharSet, "text/xml");
                if (Provider.Name != NFSeProvider.Sigep.ToString())
                    content.Headers.Add("SOAPAction", $"\"{soapAction}\"");
                break;

            case SoapVersion.Soap12:
                content = new StringContent(EnvelopeEnvio, CharSet, "application/soap+xml");
                content.Headers.ContentType?.Parameters.Add(new NameValueHeaderValue("action", $"\"{soapAction}\""));
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
        
        Execute(content, HttpMethod.Post);

        if (!EnvelopeRetorno.IsValidXml())
            throw new OpenDFeCommunicationException("Erro ao processar o xml do envelope SOAP => " + EnvelopeRetorno);

        var xmlDocument = XDocument.Parse(EnvelopeRetorno);
        var body = xmlDocument.ElementAnyNs("Envelope").ElementAnyNs("Body");
        var retorno = TratarRetorno(body, responseTag);
        if (retorno.IsValidXml()) return retorno;

        if (retorno != null)
            throw new OpenDFeCommunicationException("Erro ao processar o retorno(1) => " + retorno);
        
        throw new OpenDFeCommunicationException("Erro ao processar o retorno(2) => " + EnvelopeRetorno);
    }

    protected abstract string TratarRetorno(XElement xmlDocument, string[] responseTag);

    #endregion Methods
}