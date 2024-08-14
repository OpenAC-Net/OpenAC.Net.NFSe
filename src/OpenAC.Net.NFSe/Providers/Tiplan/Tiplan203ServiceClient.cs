// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Diego Martins
// Created          : 08-29-2021
//
// Last Modified By : Diego Martins
// Last Modified On : 23-08-2022
// ***********************************************************************
// <copyright file="Tiplan203ServiceClient.cs" company="OpenAC .Net">
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

namespace OpenAC.Net.NFSe.Providers;

internal sealed class Tiplan203ServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public Tiplan203ServiceClient(ProviderTiplan203 provider, TipoUrl tipoUrl) : base(provider, tipoUrl, provider.Certificado, SoapVersion.Soap11) { }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<RecepcionarLoteRpsRequest xmlns=\"http://nfse.abrasf.org.br/\">");
        message.Append("<nfseCabecMsg xmlns=\"\">");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg xmlns=\"\">");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</RecepcionarLoteRpsRequest>");

        return Execute("http://nfse.abrasf.org.br/RecepcionarLoteRps", message.ToString(), "RecepcionarLoteRpsResponse");
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<RecepcionarLoteRpsSincronoRequest xmlns=\"http://nfse.abrasf.org.br/\">");
        message.Append("<nfseCabecMsg xmlns=\"\">");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg xmlns=\"\">");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</RecepcionarLoteRpsSincronoRequest>");
        return Execute("http://nfse.abrasf.org.br/RecepcionarLoteRpsSincrono", message.ToString(), "RecepcionarLoteRpsSincronoResponse");
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ConsultarSituacaoLoteRpsRequest xmlns=\"http://nfse.abrasf.org.br/\">");
        message.Append("<nfseCabecMsg xmlns=\"\">");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg xmlns=\"\">");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</ConsultarSituacaoLoteRpsRequest>");

        return Execute("http://nfse.abrasf.org.br/ConsultarSituacaoLoteRps", message.ToString(), "ConsultarSituacaoLoteRpsResponse");
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ConsultarLoteRpsRequest xmlns=\"http://nfse.abrasf.org.br/\">");
        message.Append("<nfseCabecMsg xmlns=\"\">");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg xmlns=\"\">");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</ConsultarLoteRpsRequest>");

        return Execute("http://nfse.abrasf.org.br/ConsultarLoteRps", message.ToString(), "ConsultarLoteRpsResponse");
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ConsultarNfsePorRpsRequest xmlns=\"http://nfse.abrasf.org.br/\">");
        message.Append("<nfseCabecMsg xmlns=\"\">");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg xmlns=\"\">");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</ConsultarNfsePorRpsRequest>");

        return Execute("http://nfse.abrasf.org.br/ConsultarNfsePorRps", message.ToString(), "ConsultarNfsePorRpsResponse");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ConsultarNfseServicoPrestadoRequest xmlns=\"http://nfse.abrasf.org.br/\">");
        message.Append("<nfseCabecMsg xmlns=\"\">");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg xmlns=\"\">");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</ConsultarNfseServicoPrestadoRequest>");

        return Execute("http://nfse.abrasf.org.br/ConsultarNfseServicoPrestado", message.ToString(), "ConsultarNfseServicoPrestadoResponse");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<CancelarNfseRequest xmlns=\"http://nfse.abrasf.org.br/\">");
        message.Append("<nfseCabecMsg xmlns=\"\">");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg xmlns=\"\">");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</CancelarNfseRequest>");

        return Execute("http://nfse.abrasf.org.br/CancelarNfse", message.ToString(), "CancelarNfseResponse");
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<SubstituirNfseRequest xmlns=\"http://nfse.abrasf.org.br/\">");
        message.Append("<nfseCabecMsg xmlns=\"\">");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg xmlns=\"\">");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</SubstituirNfseRequest>");

        return Execute("http://nfse.abrasf.org.br/SubstituirNfse", message.ToString(), "SubstituirNfseResponse");
    }

    private string Execute(string soapAction, string message, string responseTag)
    {
        return Execute(soapAction, message, "", [responseTag], ["xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\""]);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("outputXML").Value;
    }

    #endregion Methods
}