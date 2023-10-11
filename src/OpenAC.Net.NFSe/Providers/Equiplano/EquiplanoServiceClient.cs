// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : angelomachado
// Created          : 01-23-2020
//
// Last Modified By : angelomachado
// Last Modified On : 07-11-2020
// ***********************************************************************
// <copyright file="EquiplanoServiceClient.cs" company="OpenAC .Net">
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

internal sealed class EquiplanoServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public EquiplanoServiceClient(ProviderEquiplano provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var request = new StringBuilder();
        request.Append("<esRecepcionarLoteRps xmlns=\"http://services.enfsws.es\">");
        request.Append("<nrVersaoXml>1</nrVersaoXml>");
        request.Append("<xml>");
        request.AppendEnvio(msg);
        request.Append("</xml>");
        request.Append("</esRecepcionarLoteRps>");

        return Execute("esRecepcionarLoteRps", request.ToString(), "esRecepcionarLoteRpsResponse");
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        var request = new StringBuilder();
        request.Append("<esConsultarSituacaoLoteRps xmlns=\"http://services.enfsws.es\">");
        request.Append("<nrVersaoXml>1</nrVersaoXml>");
        request.Append("<xml>");
        request.AppendEnvio(msg);
        request.Append("</xml>");
        request.Append("</esConsultarSituacaoLoteRps>");

        return Execute("esConsultarSituacaoLoteRps", request.ToString(), "esConsultarSituacaoLoteRpsResponse");
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var request = new StringBuilder();
        request.Append("<esConsultarLoteNfse xmlns=\"http://services.enfsws.es\">");
        request.Append("<nrVersaoXml>1</nrVersaoXml>");
        request.Append("<xml>");
        request.AppendEnvio(msg);
        request.Append("</xml>");
        request.Append("</esConsultarLoteNfse>");

        return Execute("esConsultarLoteRps", request.ToString(), "esConsultarLoteRpsResponse");
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var request = new StringBuilder();
        request.Append("<esConsultarNfsePorRps xmlns=\"http://services.enfsws.es\">");
        request.Append("<nrVersaoXml>1</nrVersaoXml>");
        request.Append("<xml>");
        request.AppendEnvio(msg);
        request.Append("</xml>");
        request.Append("</esConsultarNfsePorRps>");

        return Execute("esConsultarNfsePorRps", request.ToString(), "esConsultarNfsePorRpsResponse");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var request = new StringBuilder();
        request.Append("<esConsultarNfse xmlns=\"http://services.enfsws.es\">");
        request.Append("<nrVersaoXml>1</nrVersaoXml>");
        request.Append("<xml>");
        request.AppendEnvio(msg);
        request.Append("</xml>");
        request.Append("</esConsultarNfse>");

        return Execute("esConsultarNfse", request.ToString(), "esConsultarNfseResponse");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var request = new StringBuilder();
        request.Append("<esCancelarNfse xmlns=\"http://services.enfsws.es\">");
        request.Append("<nrVersaoXml>1</nrVersaoXml>");
        request.Append("<xml>");
        request.AppendEnvio(msg);
        request.Append("</xml>");
        request.Append("</esCancelarNfse>");

        return Execute("esCancelarNfse", request.ToString(), "esCancelarNfseResponse");
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
        return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("return").Value.ToString();
    }

    #endregion Methods
}