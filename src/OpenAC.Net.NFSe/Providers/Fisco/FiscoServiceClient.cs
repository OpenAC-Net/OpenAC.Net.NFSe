// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 01-11-2023
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 02-27-2023
// ***********************************************************************
// <copyright file="FiscoServiceClient.cs" company="OpenAC .Net">
//		        	   The MIT License (MIT)
//	     		Copyright (c) 2014 - 2023 Projeto OpenAC .Net
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
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class FiscoServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public FiscoServiceClient(ProviderFisco provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        throw new NotImplementedException("Função não implementada");
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ser:recepcionarLoteRpsSincrono>");
        message.Append("<ser:xml>");
        message.AppendCData(msg);
        message.Append("</ser:xml>");
        message.Append("</ser:recepcionarLoteRpsSincrono>");

        var soapAction = GetUrlWsProvedor + "/recepcionarLoteRpsSincrono";
        return Execute(soapAction, message.ToString(), "", ["RecepcionarLoteRpsSincronoResponse"], GetNamespaceSer);
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        throw new NotImplementedException("Função não implementada");
    }

    private string GetUrlWsProvedor => Provider.GetUrl(TipoUrl.ConsultarLoteRps)?.Replace("?wsdl", "");

    private string[] GetNamespaceSer => ["xmlns:ser=\"" + GetUrlWsProvedor + "\""];

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ser:consultarLoteRps>");
        message.Append("<ser:xml>");
        message.AppendCData(msg);
        message.Append("</ser:xml>");
        message.Append("</ser:consultarLoteRps>");

        var soapAction = GetUrlWsProvedor + "/consultarLoteRps";
        return Execute(soapAction, message.ToString(), "", ["consultarLoteRpsResult"], GetNamespaceSer);
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        throw new NotImplementedException("Função não implementada");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        throw new NotImplementedException("Função não implementada");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        throw new NotImplementedException("Função não implementada");
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element == null)
        {
            element = responseTag.Aggregate(xmlDocument, (current, tag) => current.ElementAnyNs(tag));
            return element == null ? xmlDocument.ToString() : element.ToString();
        }

        var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
        throw new OpenDFeCommunicationException(exMessage);
    }


    #endregion Methods
}