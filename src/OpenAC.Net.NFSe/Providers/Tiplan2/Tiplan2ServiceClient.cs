// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 03-27-2023
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 03-27-2023
// ***********************************************************************
// <copyright file="Tiplan2ServiceClient.cs" company="OpenAC .Net">
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class Tiplan2ServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public Tiplan2ServiceClient(ProviderTiplan2 provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    private string EmpacotaXml(string conteudo)
    {
        return string.Concat("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", conteudo);
    }

    public string Enviar(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string EnviarSincrono(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<RecepcionarLoteRpsSincronoRequest xmlns=\"http://nfse.abrasf.org.br\">");
        message.Append("<nfseCabecMsg xmlns=\"\">");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg xmlns=\"\">");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</RecepcionarLoteRpsSincronoRequest>");

        return Execute("http://nfse.abrasf.org.br/RecepcionarLoteRpsSincrono", message.ToString(), "RecepcionarLoteRpsSincronoResponse ");
    }

    public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string ConsultarLoteRps(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ConsultarNfsePorRpsRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</ConsultarNfsePorRpsRequest>");

        return Execute("consultarNfsePorRps", message.ToString(), "consultarNfsePorRpsResponse");
    }

    public string ConsultarNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string CancelarNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    private string Execute(string soapAction, string message, string responseTag)
    {
        return Execute(soapAction, message, "", responseTag, "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"");
    }

    protected override string Execute(string soapAction, string message, string soapHeader, string[] responseTag, params string[] soapNamespaces)
    {
        var contentType = $"text/xml; charset={CharSet}";
        var headers = new Dictionary<string, string>() { { "SOAPAction", soapAction } };

        var envelope = new StringBuilder();
        envelope.Append("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"");

        envelope.Append(soapNamespaces.Aggregate("", (atual, next) => atual + $" {next}", namespaces => namespaces + ">"));
        envelope.Append("<soap:Body>");
        envelope.Append(message);
        envelope.Append("</soap:Body>");
        envelope.Append("</soap:Envelope>");
        EnvelopeEnvio = envelope.ToString();

        Execute(new StringContent(EnvelopeEnvio, Encoding.UTF8, contentType), HttpMethod.Post, headers);

        if (!EnvelopeRetorno.IsValidXml())
            throw new OpenDFeCommunicationException("Erro ao processar o xml do envelope SOAP => " + EnvelopeRetorno);

        var xmlDocument = XDocument.Parse(EnvelopeRetorno);
        var body = xmlDocument.ElementAnyNs("Envelope").ElementAnyNs("Body");
        var retorno = TratarRetorno(body, responseTag);
        if (retorno.IsValidXml()) return retorno;

        if (retorno != null)
            throw new OpenDFeCommunicationException(retorno);
        else
            throw new OpenDFeCommunicationException(EnvelopeRetorno);
    }

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

    #endregion Methods
}