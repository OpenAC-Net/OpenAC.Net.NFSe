// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 19-08-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 19-08-2020
// ***********************************************************************
// <copyright file="DBSellerServiceClient.cs" company="OpenAC .Net">
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
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class DBSellerServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public DBSellerServiceClient(ProviderDBSeller provider, TipoUrl tipoUrl) : base(provider, tipoUrl,
        SoapVersion.Soap11)
    {
    }

    public DBSellerServiceClient(ProviderDBSeller provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(
        provider, tipoUrl, certificado, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<e:RecepcionarLoteRps>");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</e:RecepcionarLoteRps>");

        return Execute("*", message.ToString(), "RecepcionarLoteRpsResponse");
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string GerarNfse(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<e:ConsultarSituacaoLoteRps>");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</e:ConsultarSituacaoLoteRps>");

        return Execute("*", message.ToString(), "ConsultarSituacaoLoteRpsResponse");
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<e:ConsultarLoteRps>");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</e:ConsultarLoteRps>");

        return Execute("*", message.ToString(), "ConsultarLoteRpsResponse");
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<e:ConsultarNfsePorRps>");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</e:ConsultarNfsePorRps>");

        return Execute("*", message.ToString(), "ConsultarNfsePorRpsResponse");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<e:ConsultarNfse>");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</e:ConsultarNfse>");

        return Execute("*", message.ToString(), "ConsultarNfseResponse");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<e:CancelarNfse>");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</e:CancelarNfse>");

        return Execute("*", message.ToString(), "CancelarNfseResponse");
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    private string Execute(string soapAction, string message, string responseTag)
    {
        var baseUrl = new Uri(Url).GetLeftPart(UriPartial.Authority);
        var soapNs = EhHomologacao
            ? $"xmlns:e=\"{baseUrl}/webservice/index/homologacao\""
            : $"xmlns:e=\"{baseUrl}/webservice/index/producao\"";

        return Execute(soapAction, message, "", [responseTag], [soapNs]);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element == null) return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("return").Value;

        var exMessage =
            $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
        throw new OpenDFeCommunicationException(exMessage);
    }

    #endregion Methods
}