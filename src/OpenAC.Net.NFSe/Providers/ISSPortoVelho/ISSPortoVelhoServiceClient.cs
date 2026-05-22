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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;
using System.Linq;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ISSPortoVelhoServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public ISSPortoVelhoServiceClient(ProviderISSPortoVelho provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();

        message.Append("<nfse:GerarNfse xmlns:nfse=\"http://nfse.abrasf.org.br\">");
        message.Append("<nfse:GerarNfseRequest>");

        message.Append("<nfseCabecMsg><![CDATA[");
        message.Append(cabec);
        message.Append("]]></nfseCabecMsg>");

        message.Append("<nfseDadosMsg><![CDATA[");
        message.Append(msg);
        message.Append("]]></nfseDadosMsg>");

        message.Append("</nfse:GerarNfseRequest>");
        message.Append("</nfse:GerarNfse>");

        return Execute("GerarNfse", new[] { "GerarNfseResponse", "outputXML" }, message.ToString());
    }

    public string EnviarSincrono(string cabec, string msg)
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

        message.Append("<nfse:ConsultarLoteRps xmlns:nfse=\"http://nfse.abrasf.org.br\">");
        message.Append("<nfse:ConsultarLoteRpsRequest>");

        message.Append("<nfseCabecMsg><![CDATA[");
        message.Append(cabec);
        message.Append("]]></nfseCabecMsg>");

        message.Append("<nfseDadosMsg><![CDATA[");
        message.Append(msg);
        message.Append("]]></nfseDadosMsg>");

        message.Append("</nfse:ConsultarLoteRpsRequest>");
        message.Append("</nfse:ConsultarLoteRps>");

        return Execute("ConsultarLoteRps", new[] { "ConsultarLoteRpsResponse", "outputXML" }, message.ToString());
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();

        message.Append("<nfse:ConsultarNfsePorRps xmlns:nfse=\"http://nfse.abrasf.org.br\">");
        message.Append("<nfse:ConsultarNfsePorRpsRequest>");

        message.Append("<nfseCabecMsg><![CDATA[");
        message.Append(cabec);
        message.Append("]]></nfseCabecMsg>");

        message.Append("<nfseDadosMsg><![CDATA[");
        message.Append(msg);
        message.Append("]]></nfseDadosMsg>");

        message.Append("</nfse:ConsultarNfsePorRpsRequest>");
        message.Append("</nfse:ConsultarNfsePorRps>");

        return Execute("ConsultarNfsePorRps", new[] { "ConsultarNfsePorRpsResponse", "outputXML" }, message.ToString());
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();

        message.Append("<nfse:CancelarNfse xmlns:nfse=\"http://nfse.abrasf.org.br\">");
        message.Append("<nfse:CancelarNfseRequest>");

        message.Append("<nfseCabecMsg><![CDATA[");
        message.Append(cabec);
        message.Append("]]></nfseCabecMsg>");

        message.Append("<nfseDadosMsg><![CDATA[");
        message.Append(msg);
        message.Append("]]></nfseDadosMsg>");

        message.Append("</nfse:CancelarNfseRequest>");
        message.Append("</nfse:CancelarNfse>");

        return Execute("CancelarNfse", new[] { "CancelarNfseResponse", "outputXML" },message.ToString());
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    private string Execute(string soapAction, string[] responseTag, string message)
    {
        return Execute(soapAction, message, "", responseTag, ["xmlns:ws=\"http://nfse.abrasf.org.br\""]);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var fault = xmlDocument.ElementAnyNs("Fault");
        if (fault != null)
        {
            var code = fault.ElementAnyNs("faultcode")?.Value;
            var message = fault.ElementAnyNs("faultstring")?.Value;
            throw new OpenDFeCommunicationException($"{code} - {message}");
        }

        var outputXml = xmlDocument
            .Descendants()
            .FirstOrDefault(x => x.Name.LocalName == "outputXML");

        if (outputXml == null)
            throw new OpenDFeCommunicationException("Retorno da NFSe não contém a tag <outputXML>.");

        return outputXml.Value;
    }

    #endregion Methods
}