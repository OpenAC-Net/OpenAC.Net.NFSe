// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Fabio Pimenta Correa
// Created          : 21-08-2026
//
// Last Modified By : Fabio Pimenta Correa
// Last Modified On : 21-08-2026
// ***********************************************************************
// <copyright file="SimplISSv2ServiceClient.cs" company="OpenAC .Net">
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
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Nota;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ISSSalvadorServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Fields


    #endregion Fields

    #region Constructors

    public ISSSalvadorServiceClient(ProviderISSSalvador provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {

    }

    #endregion Constructors

    #region Methods

    private string GetUrlWsProvedor => Provider.GetUrl(TipoUrl.ConsultarLoteRps)?.Replace("?wsdl", "");

    public string Enviar(string cabec, string msg)
    {
             var message = new StringBuilder();
        message.Append("<tns:EnviarLoteRPS>");
        message.Append("<tns:loteXML>");
        message.AppendCData(msg);
        message.Append("</tns:loteXML>");
        message.Append("</tns:EnviarLoteRPS>");
        return Execute("http://tempuri.org/IEnvioLoteRPS/EnviarLoteRPS", message.ToString(), ["EnviarLoteRPSResponse", "EnviarLoteRPSResult"]);
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<tns:ConsultarSituacaoLoteRPS>");
        message.Append("<tns:loteXML>");
        message.AppendCData(msg);
        message.Append("</tns:loteXML>");
        message.Append("</tns:ConsultarSituacaoLoteRPS>");

        return Execute("http://tempuri.org/IConsultaSituacaoLoteRPS/ConsultarSituacaoLoteRPS",
            message.ToString(), ["ConsultarSituacaoLoteRPSResponse", "ConsultarSituacaoLoteRPSResult"]);
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {

        var message = new StringBuilder();
        message.Append("<tns:ConsultarLoteRPS>");
        message.Append("<tns:loteXML>");
        message.AppendCData(msg);
        message.Append("</tns:loteXML>");
        message.Append("</tns:ConsultarLoteRPS>");

        return Execute("http://tempuri.org/IConsultaLoteRPS/ConsultarLoteRPS",
            message.ToString(), ["ConsultarLoteRPSResponse", "ConsultarLoteRPSResult"]);
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<tns:ConsultarNfseRPS>");
        message.Append("<tns:consultaxml>");
        message.AppendCData(msg);
        message.Append("</tns:consultaxml>");
        message.Append("</tns:ConsultarNfseRPS>");

        return Execute("http://tempuri.org/IConsultaNfseRPS/ConsultarNfseRPS", message.ToString(), ["ConsultarNfseRPSResponse", "ConsultarNfseRPSResult"]);
    }

    public string ConsultarNFSe(string cabec, string msg) 
    {
        var message = new StringBuilder();
        message.Append("<tns:ConsultarNfse>");
        message.Append("<tns:consultaxml>");
        message.AppendCData(msg);
        message.Append("</tns:consultaxml>");
        message.Append("</tns:ConsultarNfse>");

        return Execute("http://tempuri.org/IConsultaNfse/ConsultarNfse", message.ToString(), ["ConsultarNfseResponse", "ConsultarNfseResult"]);

    }



    public string CancelarNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    public string EnviarSincrono(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");


    private string Execute(string soapAction, string message, string[] responseTag)
    {
        return Execute(soapAction, message, "", responseTag, ["xmlns:tns=\"http://tempuri.org/\""]);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element != null)
            throw new OpenDFeCommunicationException(element.ElementAnyNs("Reason").GetValue<string>());

        return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs(responseTag[1]).Value;
    }


    #endregion Methods
}