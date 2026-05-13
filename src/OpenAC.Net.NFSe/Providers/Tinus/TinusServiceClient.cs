// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : OpenAC .Net
// Created          : 08-05-2025
//
// Last Modified By : OpenAC .Net
// Last Modified On : 08-05-2025
// ***********************************************************************
// <copyright file="TinusServiceClient.cs" company="OpenAC .Net">
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
using System.Linq;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class TinusServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public TinusServiceClient(ProviderTinus provider, TipoUrl tipoUrl)
        : base(provider, tipoUrl, provider.Certificado, SoapVersion.Soap11) { }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        return Execute("http://nfse.abrasf.org.br/WSNFSE203.Service2.nfseSOAP.EnviarLoteRpsEnvio",
            StripXmlDeclaration(msg), "EnviarLoteRpsEnvioResponse", "EnviarLoteRpsResposta");
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        return Execute("http://nfse.abrasf.org.br/WSNFSE203.Service2.nfseSOAP.EnviarLoteRpsSincronoEnvio",
            StripXmlDeclaration(msg), "EnviarLoteRpsSincronoEnvioResponse", "EnviarLoteRpsSincronoResposta");
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        throw new NotImplementedException("Operação ConsultarSituacao não suportada pelo provedor Tinus.");
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        return Execute("http://nfse.abrasf.org.br/WSNFSE203.Service2.nfseSOAP.ConsultarLoteRpsEnvio",
            StripXmlDeclaration(msg), "ConsultarLoteRpsEnvioResponse", "ConsultarLoteRpsResposta");
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException("Operação ConsultarSequencialRps não suportada pelo provedor Tinus.");
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        return Execute("http://nfse.abrasf.org.br/WSNFSE203.Service2.nfseSOAP.ConsultarNfseRpsEnvio",
            StripXmlDeclaration(msg), "ConsultarNfseRpsEnvioResponse", "ConsultarNfseRpsResposta");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        throw new NotImplementedException("Operação ConsultarNFSe não suportada pelo provedor Tinus.");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        return Execute("http://nfse.abrasf.org.br/WSNFSE203.Service2.nfseSOAP.CancelarNfseEnvio",
            StripXmlDeclaration(msg), "CancelarNfseEnvioResponse", "CancelarNfseResposta");
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new NotImplementedException("Operação CancelarNFSeLote não suportada pelo provedor Tinus.");
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        throw new NotImplementedException("Operação SubstituirNFSe não suportada pelo provedor Tinus.");
    }

    private string Execute(string soapAction, string message, params string[] responseTags)
    {
        return Execute(soapAction, message, "", responseTags, ["xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\""]);
    }

    /// <summary>
    /// Remove a declaração XML (<?xml ...?>) do início do conteúdo, se existir,
    /// pois não é válida dentro de um SOAP body.
    /// </summary>
    private static string StripXmlDeclaration(string xml)
    {
        if (!xml.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase)) return xml;
        var idx = xml.IndexOf("?>", StringComparison.Ordinal);
        return idx >= 0 ? xml.Substring(idx + 2).TrimStart() : xml;
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        XElement? responseElement = null;
        foreach (var tag in responseTag)
        {
            responseElement = xmlDocument.ElementAnyNs(tag);
            if (responseElement != null) break;
        }
        var outputXml = responseElement?.ElementAnyNs("outputXML");
        if (outputXml != null)
            return outputXml.Value;

        if (responseElement != null)
            return NormalizeElement(responseElement).ToString();

        return xmlDocument.ToString();
    }

    /// <summary>
    /// Reconstrói recursivamente um XElement usando apenas os nomes locais dos elementos
    /// e atributos, removendo todos os prefixos de namespace e declarações xmlns.
    /// </summary>
    private static XElement NormalizeElement(XElement element)
    {
        return new XElement(
            element.Name.LocalName,
            element.Attributes()
                   .Where(a => !a.IsNamespaceDeclaration)
                   .Select(a => new XAttribute(a.Name.LocalName, a.Value)),
            element.Nodes().Select(n => n is XElement child ? NormalizeElement(child) : n)
        );
    }

    #endregion Methods
}
