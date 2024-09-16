// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 08-14-2024
//
// Last Modified By : Rafael Dias
// Last Modified On : 08-14-2024
// ***********************************************************************
// <copyright file="PronimServiceClient.cs" company="OpenAC .Net">
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
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class PronimServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public PronimServiceClient(ProviderPronim provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
    {
    }

    public PronimServiceClient(ProviderPronim provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<tem:RecepcionarLoteRps>");
        message.Append("<tem:xmlEnvio>");
        message.AppendCData(msg);
        message.Append("</tem:xmlEnvio>");
        message.Append("</tem:RecepcionarLoteRps>");

        return Execute("INFSEGeracao/RecepcionarLoteRps", cabec, message.ToString(),
            ["RecepcionarLoteRpsResponse", "RecepcionarLoteRpsResult"]);
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<tem:GerarNfse>");
        message.Append("<tem:xmlEnvio>");
        message.AppendCData(msg);
        message.Append("</tem:xmlEnvio>");
        message.Append("</tem:GerarNfse>");

        return Execute("INFSEGeracao/GerarNfse", cabec, message.ToString(), 
            ["GerarNfseResponse", "GerarNfseResponseResult"]);
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<tem:ConsultarSituacaoLoteRps>");
        message.Append("<tem:xmlEnvio>");
        message.AppendCData(msg);
        message.Append("</tem:xmlEnvio>");
        message.Append("</tem:ConsultarSituacaoLoteRps>");

        return Execute("INFSEConsultas/ConsultarSituacaoLoteRps", cabec, message.ToString(), 
            ["ConsultarSituacaoLoteRpsResponse", "ConsultarSituacaoLoteRpsResult"]);
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<tem:ConsultarLoteRps>");
        message.Append("<tem:xmlEnvio>");
        message.AppendCData(msg);
        message.Append("</tem:xmlEnvio>");
        message.Append("</tem:ConsultarLoteRps>");

        return Execute("INFSEConsultas/ConsultarLoteRps", cabec, message.ToString(), 
            ["ConsultarLoteRpsResponse", "ConsultarLoteRpsResult"]);
    }

    public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException();

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<tem:ConsultarNfsePorRps>");
        message.Append("<tem:xmlEnvio>");
        message.AppendCData(msg);
        message.Append("</tem:xmlEnvio>");
        message.Append("</tem:ConsultarNfsePorRps>");

        return Execute("INFSEConsultas/ConsultarNfsePorRps", cabec, message.ToString(), 
            ["ConsultarNfsePorRpsResponse", "ConsultarNfsePorRpsResult"]);
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<tem:ConsultarNfseServicoPrestado>");
        message.Append("<tem:xmlEnvio>");
        message.AppendCData(msg);
        message.Append("</tem:xmlEnvio>");
        message.Append("</tem:ConsultarNfseServicoPrestado>");

        return Execute("INFSEConsultas/ConsultarNfseServicoPrestado", cabec, message.ToString(), 
            ["ConsultarNfseServicoPrestadoResponse", "ConsultarNfseServicoPrestadoResult"]);
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<tem:CancelarNfse>");
        message.Append("<tem:xmlEnvio>");
        message.AppendCData(msg);
        message.Append("</tem:xmlEnvio>");
        message.Append("</tem:CancelarNfse>");

        return Execute("INFSEGeracao/CancelarNfse", cabec, message.ToString(), 
            ["CancelarNfseResponse", "CancelarNfseResult"]);
    }

    public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException();

    public string SubstituirNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<tem:SubstituirNfse>");
        message.Append("<tem:xmlEnvio>");
        message.AppendCData(msg);
        message.Append("</tem:xmlEnvio>");
        message.Append("</tem:SubstituirNfse>");

        return Execute("INFSEGeracao/SubstituirNfse", cabec, message.ToString(), 
            ["SubstituirNfseResponse", "SubstituirNfseResult"]);
    }

    private string Execute(string soapAction, string header, string message, string[] responseTag)
    {
        return Execute("http://tempuri.org/" + soapAction, message, 
            header, responseTag, ["xmlns:tem=\"http://tempuri.org/\""]);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element != null)
        {
            var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
            throw new OpenDFeCommunicationException(exMessage);
        }

        var reader = responseTag.Aggregate(xmlDocument, (current, tag) => current.ElementAnyNs(tag)).CreateReader();
        reader.MoveToContent();
        return reader.ReadInnerXml().HtmlDecode();
    }

    #endregion Methods
}