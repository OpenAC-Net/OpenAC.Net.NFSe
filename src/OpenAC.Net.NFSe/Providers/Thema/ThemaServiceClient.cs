// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 12-26-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 23-01-2020
// ***********************************************************************
// <copyright file="ThemaServiceClient.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2021 Projeto OpenAC .Net
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
using OpenAC.Net.DFe.Core.Common;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers.Thema;

internal sealed class ThemaServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public ThemaServiceClient(ProviderThema provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    public ThemaServiceClient(ProviderThema provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        StringBuilder message = new StringBuilder();
        message.Append("<recepcionarLoteRps xmlns=\"http://server.nfse.thema.inf.br\">");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</recepcionarLoteRps>");

        return Execute("recepcionarLoteRps", $"{message}", "recepcionarLoteRpsResponse");
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        StringBuilder message = new StringBuilder();
        message.Append("<consultarSituacaoLoteRps xmlns=\"http://server.nfse.thema.inf.br\">");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</consultarSituacaoLoteRps>");

        return Execute("consultarSituacaoLoteRps", $"{message}", "consultarSituacaoLoteRpsResponse");
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        StringBuilder message = new StringBuilder();
        message.Append("<consultarLoteRps xmlns=\"http://server.nfse.thema.inf.br\">");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</consultarLoteRps>");

        return Execute("consultarLoteRps", $"{message}", "consultarLoteRpsResponse");
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        StringBuilder message = new StringBuilder();
        message.Append("<consultarNfsePorRps xmlns=\"http://server.nfse.thema.inf.br\">");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</consultarNfsePorRps>");

        return Execute("consultarNfsePorRps", $"{message}", "consultarNfsePorRpsResponse");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        StringBuilder message = new StringBuilder();
        message.Append("<consultarNfse xmlns=\"http://server.nfse.thema.inf.br\">");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</consultarNfse>");

        return Execute("consultarNfse", $"{message}", "consultarNfseResponse");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        StringBuilder message = new StringBuilder();
        message.Append("<cancelarNfse xmlns=\"http://server.nfse.thema.inf.br\">");
        message.Append("<xml>");
        message.AppendCData(msg);
        message.Append("</xml>");
        message.Append("</cancelarNfse>");

        return Execute("cancelarNfse", $"{message}", "cancelarNfseResponse");
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
        return Execute(soapAction, message, "", responseTag, "xmlns:e=\"http://www.e-nfs.com.br\"");
    }

    protected override bool ValidarCertificadoServidor()
    {
        return Provider.Configuracoes.WebServices.Ambiente != DFeTipoAmbiente.Homologacao;
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        XElement element = xmlDocument.ElementAnyNs("Fault");
        if (element == null)
        {
            return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("return").Value;
        }

        string exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
        throw new OpenDFeCommunicationException(exMessage);
    }

    #endregion Methods
}