// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira/Transis
// Created          : 02-13-2023
//
// Last Modified By : Felipe Silveira/Transis
// Last Modified On : 02-13-2023
// ***********************************************************************
// <copyright file="ProviderSigiss.cs" company="OpenAC .Net">
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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class SigISS103ServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public SigISS103ServiceClient(ProviderSigISS103 provider, TipoUrl tipoUrl) : base(provider, tipoUrl, null, SoapVersion.Soap11)
    {
        CharSet = OpenEncoding.ISO88591;
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var request = new StringBuilder();
        request.Append("<ws:GerarNfse>");
        request.Append("<xml>");
        request.AppendCData(msg);
        request.Append("</xml>");
        request.Append("</ws:GerarNfse>");

        return Execute(GetUrlWsProvedor + "#GerarNfse", request.ToString(), "", ["GerarNfseResponse", "RetornoNfse"], ["xmlns:ws=\"" + GetUrlWsProvedor + "\""]);
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
        var message = new StringBuilder();
        message.Append("<ws:ConsultarLoteRps>");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</ws:ConsultarLoteRps>");
        return Execute("ConsultarLoteRpsEnvio", message.ToString(), "",["ConsultarLoteRpsResponse", "ConsultarLoteRpsResult"], ["xmlns:ws=\"" + GetUrlWsProvedor + "\""]);
    }

    private string GetUrlWsProvedor => Provider.GetUrl(TipoUrl.ConsultarLoteRps)?.Replace("?wsdl", "").Replace("https://", "https://abrasf").Replace("/abrasf/ws", "/ws");

    public string ConsultarNFSe(string cabec, string msg) => throw new NotImplementedException();

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ws:ConsultarNfsePorRps>");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</ws:ConsultarNfsePorRps>");
        return Execute("ConsultarNfsePorRps", message.ToString(), "", ["ConsultarNfsePorRpsResponse", "ConsultarNfsePorRpsResult"], ["xmlns:ws=\"" + GetUrlWsProvedor + "\""]);
    }

    public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException();

    public string EnviarSincrono(string cabec, string msg) => throw new NotImplementedException();

    public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException();

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
    protected override bool ValidarCertificadoServidor() => false;

    #endregion Methods
}