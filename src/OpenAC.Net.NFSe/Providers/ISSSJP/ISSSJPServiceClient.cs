// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Dheizon Gonçalves
// Created          : 29-05-2023
//
// Last Modified By : Dheizon Gonçalves
// Last Modified On : 29-05-2023
// ***********************************************************************
// <copyright file="BHISSServiceClient.cs" company="OpenAC .Net">
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


using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using System;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ISSSJPServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public ISSSJPServiceClient(ProviderISSSJP provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabecalho, string dados)
    {
        var message = new StringBuilder();

        message.Append("<nfe:RecepcionarLoteRpsV3>");
        message.Append("<arg0>");
        message.AppendCData(cabecalho);
        message.Append("</arg0>");
        message.Append("<arg1>");
        message.AppendCData(dados);
        message.Append("</arg1>");
        message.Append("</nfe:RecepcionarLoteRpsV3>");

        return Execute("", message.ToString(), "", [], ["xmlns:nfe=\"http://nfe.sjp.pr.gov.br\""]);            
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();

        message.Append("<nfe:ConsultarLoteRpsV3>");
        message.Append("<arg0>");
        message.AppendCData(cabec);
        message.Append("</arg0>");
        message.Append("<arg1>");
        message.AppendCData(msg);
        message.Append("</arg1>");
        message.Append("</nfe:ConsultarLoteRpsV3>");

        return Execute("", message.ToString(), "", [], ["xmlns:nfe=\"http://nfe.sjp.pr.gov.br\""]);
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();

        message.Append("<nfe:ConsultarNfseV3>");
        message.Append("<arg0>");
        message.AppendCData(cabec);
        message.Append("</arg0>");
        message.Append("<arg1>");
        message.AppendCData(msg);
        message.Append("</arg1>");
        message.Append("</nfe:ConsultarNfseV3>");

        return Execute("", message.ToString(), "", [], ["xmlns:nfe=\"http://nfe.sjp.pr.gov.br\""]);
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();

        message.Append("<nfe:ConsultarNfsePorRpsV3>");
        message.Append("<arg0>");
        message.AppendCData(cabec);
        message.Append("</arg0>");
        message.Append("<arg1>");
        message.AppendCData(msg);
        message.Append("</arg1>");
        message.Append("</nfe:ConsultarNfsePorRpsV3>");

        return Execute("", message.ToString(), "", [], ["xmlns:nfe=\"http://nfe.sjp.pr.gov.br\""]);
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        var message = new StringBuilder();

        message.Append("<nfe:ConsultarSituacaoLoteRpsV3>");
        message.Append("<arg0>");
        message.AppendCData(cabec);
        message.Append("</arg0>");
        message.Append("<arg1>");
        message.AppendCData(msg);
        message.Append("</arg1>");
        message.Append("</nfe:ConsultarSituacaoLoteRpsV3>");

        return Execute("", message.ToString(), "", [], ["xmlns:nfe=\"http://nfe.sjp.pr.gov.br\""]);
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element == null) return xmlDocument.ToString();

        var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
        throw new OpenDFeCommunicationException(exMessage);
    }

    #endregion
}