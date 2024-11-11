// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 29-01-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 29-01-2020
// ***********************************************************************
// <copyright file="FiorilliServiceClient.cs" company="OpenAC .Net">
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
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class Fiorilli201ServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public Fiorilli201ServiceClient(ProviderFiorilli201 provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
    {
    }

    public Fiorilli201ServiceClient(ProviderFiorilli201 provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ws:recepcionarLoteRps>");
        message.Append(msg);
        message.Append("<username>");
        message.Append(Provider.Configuracoes.WebServices.Usuario);
        message.Append("</username>");
        message.Append("<password>");
        message.Append(Provider.Configuracoes.WebServices.Senha);
        message.Append("</password>");
        message.Append("</ws:recepcionarLoteRps>");

        return Execute("recepcionarLoteRps", message.ToString(), "recepcionarLoteRpsResponse");
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ws:recepcionarLoteRpsSincrono>");
        message.Append(msg);
        message.Append("<username>");
        message.Append(Provider.Configuracoes.WebServices.Usuario);
        message.Append("</username>");
        message.Append("<password>");
        message.Append(Provider.Configuracoes.WebServices.Senha);
        message.Append("</password>");
        message.Append("</ws:recepcionarLoteRpsSincrono>");

        return Execute("recepcionarLoteRpsSincrono", message.ToString(), "recepcionarLoteRpsSincronoResponse");
    }

    public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ws:consultarLoteRps>");
        message.Append(msg);
        message.Append("<username>");
        message.Append(Provider.Configuracoes.WebServices.Usuario);
        message.Append("</username>");
        message.Append("<password>");
        message.Append(Provider.Configuracoes.WebServices.Senha);
        message.Append("</password>");
        message.Append("</ws:consultarLoteRps>");

        return Execute("consultarLoteRps", message.ToString(), "consultarLoteRpsResponse");
    }

    public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ws:consultarNfsePorRps>");
        message.Append(msg);
        message.Append("<username>");
        message.Append(Provider.Configuracoes.WebServices.Usuario);
        message.Append("</username>");
        message.Append("<password>");
        message.Append(Provider.Configuracoes.WebServices.Senha);
        message.Append("</password>");
        message.Append("</ws:consultarNfsePorRps>");

        return Execute("consultarNfsePorRps", message.ToString(), "consultarNfsePorRpsResponse");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ws:consultarNfseServicoPrestado>");
        message.Append(msg);
        message.Append("<username>");
        message.Append(Provider.Configuracoes.WebServices.Usuario);
        message.Append("</username>");
        message.Append("<password>");
        message.Append(Provider.Configuracoes.WebServices.Senha);
        message.Append("</password>");
        message.Append("</ws:consultarNfseServicoPrestado>");

        return Execute("consultarNfseServicoPrestado", message.ToString(), "consultarNfseServicoPrestadoResponse");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        // Dados de homologação
        // CNPJ=01001001000113, IM:15000, Login=01001001000113, Senha=123456;
        var message = new StringBuilder();
        message.Append("<ws:cancelarNfse>");
        message.Append(msg);
        message.Append("<username>");
        message.Append(Provider.Configuracoes.WebServices.Usuario);
        message.Append("</username>");
        message.Append("<password>");
        message.Append(Provider.Configuracoes.WebServices.Senha);
        message.Append("</password>");
        message.Append("</ws:cancelarNfse>");

        return Execute("cancelarNfse", message.ToString(), "cancelarNfseResponse");
    }

    public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string SubstituirNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ws:substituirNfse>");
        message.Append(msg);
        message.Append("<username>");
        message.Append(Provider.Configuracoes.WebServices.Usuario);
        message.Append("</username>");
        message.Append("<password>");
        message.Append(Provider.Configuracoes.WebServices.Senha);
        message.Append("</password>");
        message.Append("</ws:substituirNfse>");

        return Execute("substituirNfse", message.ToString(), "substituirNfseResponse");
    }

    private string Execute(string soapAction, string message, string responseTag)
    {
        var result = ValidarUsernamePassword();
        if (!result) throw new OpenDFeCommunicationException("Faltou informar username e/ou password");

        return Execute(soapAction, message, "", [responseTag], ["xmlns:ws=\"http://ws.issweb.fiorilli.com.br/\""]);
    }

    public bool ValidarUsernamePassword()
    {
        return !string.IsNullOrEmpty(Provider.Configuracoes.WebServices.Usuario) && !string.IsNullOrEmpty(Provider.Configuracoes.WebServices.Senha);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element != null)
        {
            var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
            throw new OpenDFeCommunicationException(exMessage);
        }

        var reader = xmlDocument.ElementAnyNs(responseTag[0]).CreateReader();
        reader.MoveToContent();
        return reader.ReadInnerXml().Replace("ns2:", string.Empty);
    }

    #endregion Methods
}