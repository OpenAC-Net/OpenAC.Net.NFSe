// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 07-28-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="GinfesServiceClient.cs" company="OpenAC .Net">
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
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class GinfesServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public GinfesServiceClient(ProviderGinfes provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabecalho, string dados)
    {
        var message = new StringBuilder();
        message.Append("<gin:RecepcionarLoteRpsV3>");
        message.Append("<arg0>");
        message.AppendEnvio(cabecalho);
        message.Append("</arg0>");
        message.Append("<arg1>");
        message.AppendEnvio(dados);
        message.Append("</arg1>");
        message.Append("</gin:RecepcionarLoteRpsV3>");

        return Execute(message.ToString(), "RecepcionarLoteRpsV3Response");
    }

    public string EnviarSincrono(string cabec, string msg) =>
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string GerarNfse(string cabec, string msg) =>
        throw new NotImplementedException("Função não suportada neste Provedor!");

    public string ConsultarSituacao(string cabecalho, string dados)
    {
        var message = new StringBuilder();
        message.Append("<gin:ConsultarSituacaoLoteRpsV3>");
        message.Append("<arg0>");
        message.AppendEnvio(cabecalho);
        message.Append("</arg0>");
        message.Append("<arg1>");
        message.AppendEnvio(dados);
        message.Append("</arg1>");
        message.Append("</gin:ConsultarSituacaoLoteRpsV3>");

        return Execute(message.ToString(), "ConsultarSituacaoLoteRpsV3Response");
    }

    public string ConsultarLoteRps(string cabecalho, string dados)
    {
        var message = new StringBuilder();
        message.Append("<gin:ConsultarLoteRpsV3>");
        message.Append("<arg0>");
        message.AppendEnvio(cabecalho);
        message.Append("</arg0>");
        message.Append("<arg1>");
        message.AppendEnvio(dados);
        message.Append("</arg1>");
        message.Append("</gin:ConsultarLoteRpsV3>");

        return Execute(message.ToString(), "ConsultarLoteRpsV3Response");
    }

    public string ConsultarSequencialRps(string cabec, string msg) =>
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string ConsultarNFSeRps(string cabecalho, string dados)
    {
        var message = new StringBuilder();
        message.Append("<gin:ConsultarNfsePorRpsV3>");
        message.Append("<arg0>");
        message.AppendEnvio(cabecalho);
        message.Append("</arg0>");
        message.Append("<arg1>");
        message.AppendEnvio(dados);
        message.Append("</arg1>");
        message.Append("</gin:ConsultarNfsePorRpsV3>");

        return Execute(message.ToString(), "ConsultarNfsePorRpsV3Response");
    }

    public string ConsultarNFSe(string cabecalho, string dados)
    {
        var message = new StringBuilder();
        message.Append("<gin:ConsultarNfseV3>");
        message.Append("<arg0>");
        message.AppendEnvio(cabecalho);
        message.Append("</arg0>");
        message.Append("<arg1>");
        message.AppendEnvio(dados);
        message.Append("</arg1>");
        message.Append("</gin:ConsultarNfseV3>");

        return Execute(message.ToString(), "ConsultarNfseV3Response");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<gin:CancelarNfseV3>");
        message.Append("<arg0>");
        message.AppendEnvio(cabec);
        message.Append("</arg0>");
        message.Append("<arg1>");
        message.AppendEnvio(msg);
        message.Append("</arg1>");
        message.Append("</gin:CancelarNfseV3>");

        return Execute(message.ToString(), "CancelarNfseV3Response");
    }

    public string CancelarNFSeLote(string cabec, string msg) =>
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string SubstituirNFSe(string cabec, string msg) =>
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    private string Execute(string message, string responseTag)
    {
        var ns = Provider.Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Homologacao
            ? "xmlns:gin=\"http://homologacao.ginfes.com.br\""
            : "xmlns:gin=\"http://producao.ginfes.com.br\"";

        return Execute("", message, "", [responseTag], [ns]);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag) =>
        xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("return").Value;

    #endregion Methods
}