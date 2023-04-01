// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Valnei Filho v_marinpietri@yahoo.com.br
// Created          : 24-07-2022
//
// Last Modified By : Rafael Dias
// Last Modified On : 26-08-2022
// ***********************************************************************
// <copyright file="MetropolisWebClient.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2022 Projeto OpenAC .Net
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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class MetropolisWebClient : NFSeSoapServiceClient, IServiceClient
{
    #region Construtor

    public MetropolisWebClient(ProviderMetropolisWeb provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
    {
    }

    #endregion Construtor

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<end:RecepcionarLoteRps>");
        message.Append("<RecepcionarLoteRpsRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</RecepcionarLoteRpsRequest>");
        message.Append("</end:RecepcionarLoteRps>");
        return Execute(message.ToString());
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<end:ConsultarSituacaoLoteRps>");
        message.Append("<ConsultarSituacaoLoteRpsRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</ConsultarSituacaoLoteRpsRequest>");
        message.Append("</end:ConsultarSituacaoLoteRps>");
        return Execute(message.ToString());
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<end:ConsultarLoteRps>");
        message.Append("<ConsultarLoteRpsRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</ConsultarLoteRpsRequest>");
        message.Append("</end:ConsultarLoteRps>");
        return Execute(message.ToString());
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<end:ConsultarNfsePorRps>");
        message.Append("<ConsultarNfsePorRpsRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</ConsultarNfsePorRpsRequest>");
        message.Append("</end:ConsultarNfsePorRps>");
        return Execute(message.ToString());
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<end:ConsultarNfse>");
        message.Append("<ConsultarNfseRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</ConsultarNfseRequest>");
        message.Append("</end:ConsultarNfse>");
        return Execute(message.ToString());
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<end:CancelarNfse>");
        message.Append("<CancelarNfseRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</CancelarNfseRequest>");
        message.Append("</end:CancelarNfse>");
        return Execute(message.ToString());
    }

    public string EnviarSincrono(string cabec, string msg) => throw new NotImplementedException("Serviço não disponível por este provedor");

    public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException("Serviço não disponível por este provedor");

    public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException("Serviço não disponível por este provedor");

    public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException("Serviço não disponível por este provedor");

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        if (xmlDocument == null) return "";
        var output = xmlDocument.Descendants("outputXML").FirstOrDefault();
        return output?.Value;
    }

    private string Execute(string message)
    {
        return Execute("", message, "", "", "xmlns:end=\"http://endpoint.nfse.ws.webservicenfse.edza.com.br/\"");
    }

    #endregion Methods
}