// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-22-2018
//
// Last Modified By : Leandro Rossi (rossism.com.br)
// Last Modified On : 14-04-2023
// ***********************************************************************
// <copyright file="ISSNetServiceClient.cs" company="OpenAC .Net">
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
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ISSPortoVelhoServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public ISSPortoVelhoServiceClient(ProviderISSPortoVelho provider, TipoUrl tipoUrl) : base(provider, tipoUrl,
        SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:CancelarNfseRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:CancelarNfseRequest>");
        return Execute("CancelarNfseEnvio", message.ToString(), "CancelarNfseResponse");
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:ConsultarLoteRpsRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:ConsultarLoteRpsRequest>");
        return Execute("ConsultarLoteRpsEnvio", message.ToString(), "ConsultarLoteRpsResponse");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:ConsultarNfseServicoPrestadoRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:ConsultarNfseServicoPrestadoRequest>");
        return Execute("ConsultarNfseServicoPrestadoEnvio", message.ToString(), "ConsultarNfseServicoPrestadoResponse");
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:ConsultarNfsePorRpsRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:ConsultarNfsePorRpsRequest>");
        return Execute("ConsultarNfseRpsEnvio", message.ToString(), "ConsultarNfsePorRpsResponse");
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:RecepcionarLoteRpsRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:RecepcionarLoteRpsRequest>");
        return Execute("EnviarLoteRpsEnvio", message.ToString(), "RecepcionarLoteRpsResponse");
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:RecepcionarLoteRpsSincronoRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:RecepcionarLoteRpsSincronoRequest>");
        return Execute("EnviarLoteRpsSincronoEnvio", message.ToString(), "RecepcionarLoteRpsSincronoResponse");
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:SubstituirNfseRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:SubstituirNfseRequest>");
        return Execute("SubstituirNfseEnvio", message.ToString(), "SubstituirNfseResponse");
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element != null)
        {
            var exMessage =
                $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
            throw new OpenDFeCommunicationException(exMessage);
        }

        return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("outputXML").Value;
    }

    private string Execute(string soapAction, string message, string responseTag)
    {
        return Execute(soapAction, message, "", [responseTag], ["xmlns:nfse=\"http://nfse.abrasf.org.br\""]);
    }

    #endregion Methods
}