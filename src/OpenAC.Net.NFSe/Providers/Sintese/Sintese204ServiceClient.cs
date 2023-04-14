// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 18-04-2022
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 18-04-2022
// ***********************************************************************
// <copyright file="SinteseServiceClient.cs" company="OpenAC .Net">
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
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class Sintese204ServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public Sintese204ServiceClient(ProviderSintese204 provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
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
        message.Append("<nfse:EnviarLoteRpsSincronoEnvio>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(EmpacotaXml(cabec));
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(EmpacotaXml(msg));
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:EnviarLoteRpsSincronoEnvio>");

        return Execute("http://nfsews.sintesetecnologia.com.br/RecepcionarLoteRpsSincrono", message.ToString(), "RecepcionarLoteRpsSincronoResponse ");
    }

    public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string ConsultarLoteRps(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:ConsultarNfsePorRpsRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(EmpacotaXml(cabec));
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(EmpacotaXml(msg));
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:ConsultarNfsePorRpsRequest>");

        return Execute("consultarNfsePorRps", message.ToString(), "consultarNfsePorRpsResponse");
    }

    public string ConsultarNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string CancelarNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    private string Execute(string soapAction, string message, string responseTag)
    {
        return Execute(soapAction, message, "", responseTag, "xmlns:nfse=\"http://nfse.abrasf.org.br\"");
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element == null)
        {
            element = responseTag.Aggregate(xmlDocument, (current, tag) => current.ElementAnyNs(tag));
            if (element == null)
                return xmlDocument.ToString();

            return element.ToString();
        }

        var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
        throw new OpenDFeCommunicationException(exMessage);
    }

    #endregion Methods
}