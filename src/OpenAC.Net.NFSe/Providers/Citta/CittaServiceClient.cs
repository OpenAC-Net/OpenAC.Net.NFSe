// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 26-08-2021
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 26-08-2021
// ***********************************************************************
// <copyright file="CittaServiceClient.cs" company="OpenAC .Net">
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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class CittaServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public CittaServiceClient(ProviderCitta provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
    {
    }

    public CittaServiceClient(ProviderCitta provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        return Execute("http://nfse.abrasf.org.br/RecepcionarLoteRpsSincrono", msg, "RecepcionarLoteRpsSincronoResposta");
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        return Execute("http://nfse.abrasf.org.br/ConsultarLoteRps", msg, "ConsultarLoteRpsResposta");
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        return Execute("http://nfse.abrasf.org.br/ConsultarNfsePorRps", msg, "ConsultarNfsePorRpsResposta");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:ConsultarNfsePorFaixaEnvio soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
        message.Append(msg);
        message.Append("</nfse:ConsultarNfsePorFaixaEnvio>");

        return Execute("http://nfse.abrasf.org.br/ConsultarNfsePorFaixa", message.ToString(), "ConsultarNfseFaixaResponse");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:CancelarNfseEnvio soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
        message.Append(msg);
        message.Append("</nfse:CancelarNfseEnvio>");

        return Execute("http://nfse.abrasf.org.br/CancelarNfse", message.ToString(), "CancelarNfseResponse");
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:SubstituirNfseEnvio soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
        message.Append(msg);
        message.Append("</nfse:SubstituirNfseEnvio>");

        return Execute("http://nfse.abrasf.org.br/SubstituirNfse", message.ToString(), "SubstituirNfseResponse");
    }

    private string Execute(string action, string message, params string[] responseTag)
    {
        var ns = "xmlns:nfse=\"http://nfse.abrasf.org.br\" xmlns:nfs=\"http://localhost:8080/nfse/services/nfseSOAP?wsdl\"";
        if (action == "http://nfse.abrasf.org.br/RecepcionarLoteRpsSincrono") ns += " xmlns:nfse1=\"http://nfse.citta.com.br\"";

        return Execute(action, message, responseTag, ns);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        return xmlDocument.ElementAnyNs(responseTag[0]).ToString();
    }

    #endregion Methods
}