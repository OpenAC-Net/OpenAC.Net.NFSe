// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rodolfo Duarte
// Created          : 05-15-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="SaoPauloServiceClient.cs" company="OpenAC .Net">
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

namespace OpenAC.Net.NFSe.Providers;

internal sealed class SaoPauloServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public SaoPauloServiceClient(ProviderSaoPaulo provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap12)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        string tag, response, soapAction;
        if (EhHomologacao)
        {
            tag = "TesteEnvioLoteRPSRequest";
            response = "TesteEnvioLoteRPSResponse";
            soapAction = "http://www.prefeitura.sp.gov.br/nfe/ws/testeEnvioLoteRPS";
        }
        else
        {
            tag = "EnvioLoteRPSRequest";
            response = "EnvioLoteRPSResponse";
            soapAction = "http://www.prefeitura.sp.gov.br/nfe/ws/envioLoteRPS";
        }

        var message = new StringBuilder();
        message.Append($"<nfe:{tag}>");
        message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
        message.Append("<nfe:MensagemXML>");
        message.AppendCData(msg);
        message.Append("</nfe:MensagemXML>");
        message.Append($"</nfe:{tag}>");

        return Execute(soapAction, message.ToString(), response);
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfe:EnvioRPSRequest>");
        message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
        message.Append("<nfe:MensagemXML>");
        message.AppendCData(msg);
        message.Append("</nfe:MensagemXML>");
        message.Append("</nfe:EnvioRPSRequest>");

        return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/envioRPS", message.ToString(), "EnvioRPSResponse");
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfe:ConsultaInformacoesLoteRequest>");
        message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
        message.Append("<nfe:MensagemXML>");
        message.AppendCData(msg);
        message.Append("</nfe:MensagemXML>");
        message.Append("</nfe:ConsultaInformacoesLoteRequest>");

        return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaInformacoesLote", message.ToString(), "ConsultaInformacoesLoteResponse");
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfe:ConsultaLoteRequest>");
        message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
        message.Append("<nfe:MensagemXML>");
        message.AppendCData(msg);
        message.Append("</nfe:MensagemXML>");
        message.Append("</nfe:ConsultaLoteRequest>");

        return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaLote", message.ToString(), "ConsultaLoteResponse");
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        return ConsultarNFSe(cabec, msg);
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfe:ConsultaNFeRequest>");
        message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
        message.Append("<nfe:MensagemXML>");
        message.AppendCData(msg);
        message.Append("</nfe:MensagemXML>");
        message.Append("</nfe:ConsultaNFeRequest>");

        return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFe", message.ToString(), "ConsultaNFeResponse");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfe:CancelamentoNFeRequest>");
        message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
        message.Append("<nfe:MensagemXML>");
        message.AppendCData(msg);
        message.Append("</nfe:MensagemXML>");
        message.Append("</nfe:CancelamentoNFeRequest>");

        return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/cancelamentoNFe", message.ToString(), "CancelamentoNFeResponse");
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    private string Execute(string soapAction, string message, string responseTag)
    {
        return Execute(soapAction, message, "", responseTag, "xmlns:nfe=\"http://www.prefeitura.sp.gov.br/nfe\"");
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element != null)
        {
            var exMessage = $"{element.ElementAnyNs("Code")?.ElementAnyNs("Value")?.GetValue<string>()} - " +
                            $"{element.ElementAnyNs("Reason")?.ElementAnyNs("Text")?.GetValue<string>()}";
            throw new OpenDFeCommunicationException(exMessage);
        }

        return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("RetornoXML").Value;
    }

    #endregion Methods
}