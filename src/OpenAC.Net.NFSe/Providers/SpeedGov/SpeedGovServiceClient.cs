// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 07-30-2021
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 07-30-2021
// ***********************************************************************
// <copyright file="SpeedGovServiceClient.cs" company="OpenAC .Net">
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
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class SpeedGovServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public SpeedGovServiceClient(ProviderSpeedGov provider, TipoUrl tipoUrl) : base(provider, tipoUrl, null, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:RecepcionarLoteRps>");
        message.Append("<!--Optional:-->");
        message.Append("<header>");
        message.AppendCData("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + cabec);
        message.Append("</header>");
        message.Append("<!--Optional:-->");
        message.Append("<parameters>");
        message.AppendCData("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + msg);
        message.Append("</parameters>");
        message.Append("</nfse:RecepcionarLoteRps>");

        return Execute("*", "RecepcionarLoteRpsResponse", message.ToString());
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string GerarNfse(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:ConsultarLoteRps>");
        message.Append("<header>");
        message.Append(cabec);
        message.Append("</header>");
        message.Append("<parameters>");
        message.Append(msg);
        message.Append("</parameters>");
        message.Append("</nfse:ConsultarLoteRps>");

        return Execute("*", "ConsultarLoteRpsResponse", message.ToString());
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:ConsultarNfsePorRps>");
        message.Append("<header>");
        message.Append(cabec);
        message.Append("</header>");
        message.Append("<parameters>");
        message.Append(msg);
        message.Append("</parameters>");
        message.Append("</nfse:ConsultarNfsePorRps>");

        return Execute("*", "ConsultarNfsePorRpsResponse", message.ToString());
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:ConsultarNfse>");
        message.Append("<header>");
        message.Append(cabec);
        message.Append("</header>");
        message.Append("<parameters>");
        message.Append(msg);
        message.Append("</parameters>");
        message.Append("</nfse:ConsultarNfse>");

        return Execute("*", "ConsultarNfseServicoPrestadoResponse", message.ToString());
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:CancelarNfse>");
        message.Append("<header>");
        message.Append(cabec);
        message.Append("</header>");
        message.Append("<parameters>");
        message.Append(msg);
        message.Append("</parameters>");
        message.Append("</nfse:CancelarNfse>");

        return Execute("*", "CancelarNfseResponse", message.ToString());
    }

    public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException();

    public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException();

    private string Execute(string soapAction, string responseTag, string message)
    {
        return Execute(soapAction, message, "", [responseTag], ["xmlns:nfse=\"http://www.abrasf.org.br/ABRASF/arquivos/nfse.xsd\""]);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element == null) return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("return").Value;

        var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
        throw new OpenDFeCommunicationException(exMessage);
    }

    #endregion Methods
}