// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : danilobreda
// Created          : 07-10-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 10-10-2020
// ***********************************************************************
// <copyright file="ProviderSigiss.cs" company="OpenAC .Net">
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
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class SigISS100ServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public SigISS100ServiceClient(ProviderSigISS100 provider, TipoUrl tipoUrl) : base(provider, tipoUrl, null, SoapVersion.Soap11)
    {
        CharSet = OpenEncoding.ISO88591;
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var request = new StringBuilder();
        request.Append("<urn:GerarNota soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
        request.Append(msg);
        request.Append("</urn:GerarNota>");

        return Execute("urn:sigiss_ws#GerarNota", request.ToString(), "", ["GerarNotaResponse", "RetornoNota"], ["xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "xmlns:urn=\"urn: sigiss_ws\""]);
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        var request = new StringBuilder();
        request.Append("<urn:ConsultarNotaValida soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
        request.Append("<DadosConsultaNota xsi:type=\"urn:tcDadosConsultaNota\">");
        request.Append(msg);
        request.Append("</DadosConsultaNota>");
        request.Append("</urn:ConsultarNotaValida>");

        return Execute("urn:sigiss_ws#ConsultarNotaValida", request.ToString(), "", ["GerarNotaResponse", "RetornoNota"], ["xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "xmlns:urn=\"urn: sigiss_ws\""]);
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var request = new StringBuilder();
        request.Append("<urn:CancelarNota soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
        request.Append(msg);
        request.Append("</urn:CancelarNota>");

        return Execute("urn:sigiss_ws#CancelarNota", request.ToString(), "", ["CancelarNotaResponse", "RetornoNota"], ["xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "xmlns:urn=\"urn: sigiss_ws\""]);
    }

    public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException();

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var url = Provider.GetUrl(TipoUrl.ConsultarLoteRps)?.Replace("?wsdl", "")
            .Replace("https://", "https://abrasf").Replace("/abrasf/ws", "/ws");
        var message = new StringBuilder();
        message.Append("<ws:ConsultarLoteRps>");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</ws:ConsultarLoteRps>");
        return Execute("ConsultarLoteRpsEnvio", message.ToString(), "", ["ConsultarLoteRpsResponse", "ConsultarLoteRpsResult"], ["xmlns:ws=\"" + url + "\""]);
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var request = new StringBuilder();
        request.Append(msg);
        return Execute("ConsultarNotaPrestador", request.ToString(), "", ["ConsultarNotaPrestadorResponse"], ["xmlns:urn=\"urn:sigiss_ws\""]);
    }

    public string ConsultarNFSeRps(string cabec, string msg) => throw new NotImplementedException();

    public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException();

    public string EnviarSincrono(string cabec, string msg) => throw new NotImplementedException();

    public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException();

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        //verifica se o retorno tem os elementos corretos senão da erro.
        var element = xmlDocument.ElementAnyNs(responseTag[0]) ?? throw new OpenDFeCommunicationException($"Primeiro Elemento ({responseTag[0]}) do xml não encontrado");
        if (responseTag.Length > 1)
        {
            _ = element.ElementAnyNs(responseTag[1]) ?? throw new OpenDFeCommunicationException($"Dados ({responseTag[1]}) do xml não encontrado");
        }

        return element.ToString();
    }

    protected override bool ValidarCertificadoServidor() => false;

    #endregion Methods
}