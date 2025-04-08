// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Flávio Vodzinski
// Created          : 04-24-2024
//
// Last Modified By : Rafael Dias
// Last Modified On : 08-15-2024
// ***********************************************************************
// <copyright file="MegasoftServiceClient.cs" company="OpenAC .Net">
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
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Providers;

internal class MegasoftServiceCliente : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public MegasoftServiceCliente(ProviderMegasoft provider, TipoUrl tipoUrl) : base(provider, tipoUrl,
        SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string CancelarNFSe(string cabec, string msg) => throw new NotImplementedException();

    public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException();

    public string ConsultarLoteRps(string cabec, string msg) => throw new NotImplementedException();

    public string ConsultarNFSe(string cabec, string msg) => throw new NotImplementedException();

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ws:ConsultarNfsePorRpsRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</ws:ConsultarNfsePorRpsRequest>");

        return Execute("http://ws.megasoftarrecadanet.com.br/ConsultarNfsePorRps", message.ToString(), "",
            ["ConsultarNfsePorRpsResponse"], ["xmlns:ws=\"http://ws.megasoftarrecadanet.com.br\""]);
    }

    public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException();

    public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException();

    public string Enviar(string cabec, string msg) => throw new NotImplementedException();

    public string EnviarSincrono(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ws:GerarNfseRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</ws:GerarNfseRequest>");

        return Execute("http://ws.megasoftarrecadanet.com.br/GerarNfse", message.ToString(), "", ["GerarNfseResponse"],
            ["xmlns:ws=\"http://ws.megasoftarrecadanet.com.br\""]);
    }

    public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException();

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element == null) return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("outputXML").Value;

        var exMessage =
            $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
        throw new OpenDFeCommunicationException(exMessage);
    }

    #endregion Methods
}