// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-16-2018
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="ISSeServiceClient.cs" company="OpenAC .Net">
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
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ISSeServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public ISSeServiceClient(ProviderISSe provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
    {
    }

    public ISSeServiceClient(ProviderISSe provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<v2:EnviarLoteRps soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
        message.Append("<xml xsi:type=\"xsd:string\">");
        message.AppendEnvio(msg);
        message.Append("</xml>");
        message.Append("</v2:EnviarLoteRps>");

        return Execute("EnviarLoteRps", message.ToString(), "EnviarLoteRpsResponse");
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<v2:EnviarLoteRpsSincrono soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
        message.Append("<xml xsi:type=\"xsd:string\">");
        message.AppendEnvio(msg);
        message.Append("</xml>");
        message.Append("</v2:EnviarLoteRpsSincrono>");

        return Execute("EnviarLoteRpsSincrono", message.ToString(), "EnviarLoteRpsSincronoResponse");
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<v2:ConsultarLoteRps soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
        message.Append("<xml xsi:type=\"xsd:string\">");
        message.AppendEnvio(msg);
        message.Append("</xml>");
        message.Append("</v2:ConsultarLoteRps>");

        return Execute("ConsultarLoteRps", message.ToString(), "ConsultarLoteRpsResponse");
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<v2:ConsultarNfseRps soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
        message.Append("<xml xsi:type=\"xsd:string\">");
        message.AppendEnvio(msg);
        message.Append("</xml>");
        message.Append("</v2:ConsultarNfseRps>");

        return Execute("ConsultarNfseRps", message.ToString(), "ConsultarNfseRpsResponse");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<v2:ConsultarNfseFaixa soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
        message.Append("<xml xsi:type=\"xsd:string\">");
        message.AppendEnvio(msg);
        message.Append("</xml>");
        message.Append("</v2:ConsultarNfseFaixa>");

        return Execute("ConsultarNfseFaixa", message.ToString(), "ConsultarNfseFaixaResponse");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<v2:CancelarNfse soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
        message.Append("<xml xsi:type=\"xsd:string\">");
        message.AppendEnvio(msg);
        message.Append("</xml>");
        message.Append("</v2:CancelarNfse>");

        return Execute("CancelarNfse", message.ToString(), "CancelarNfseResponse");
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<v2:SubstituirNfse soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
        message.Append("<xml xsi:type=\"xsd:string\">");
        message.AppendEnvio(msg);
        message.Append("</xml>");
        message.Append("</v2:SubstituirNfse>");

        return Execute("SubstituirNfse", message.ToString(), "SubstituirNfseResponse");
    }

    private string Execute(string action, string message, params string[] responseTag)
    {
        var baseUrl = new Uri(Url).GetLeftPart(UriPartial.Authority);
        var soapNs = $"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                     $"xmlns:v2=\"{baseUrl}/v2.01\"";

        return Execute($"https://nfse-ws.hom-ecity.maringa.pr.gov.br/v2.01#{action}", message, "", responseTag, [soapNs]);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element == null)
            return xmlDocument.ElementAnyNs("return")?.Value;

        var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
        throw new OpenDFeCommunicationException(exMessage);
    }

    #endregion Methods
}