// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira/Transis
// Created          : 16-03-2023
//
// Last Modified By : Felipe Silveira/Transis
// Last Modified On : 16-03-2023
// ***********************************************************************
// <copyright file="SimplISSv2ServiceClient.cs" company="OpenAC .Net">
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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class SimplISS203ServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Fields

    private XNamespace tc = "http://www.sistema.com.br/Nfse/arquivos/nfse_3.xsd";

    #endregion Fields

    #region Constructors

    public SimplISS203ServiceClient(ProviderSimplISS203 provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<sis:RecepcionarLoteRps>");
        message.Append(msg);
        message.Append("</sis:RecepcionarLoteRps>");

        return Execute("http://www.sistema.com.br/Sistema.Ws.Nfse/INfseService/RecepcionarLoteRps", message.ToString(), "RecepcionarLoteRpsResult");
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<sis:ConsultarSituacaoLoteRps>");
        message.Append(AjustarMensagem(msg));
        message.Append("</sis:ConsultarSituacaoLoteRps>");

        return Execute("http://www.sistema.com.br/Sistema.Ws.Nfse/INfseService/ConsultarSituacaoLoteRps",
            message.ToString(), "ConsultarSituacaoLoteRpsResult");
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<sis:ConsultarLoteRps>");
        message.Append(AjustarMensagem(msg));
        message.Append("</sis:ConsultarLoteRps>");

        return Execute("http://www.sistema.com.br/Sistema.Ws.Nfse/INfseService/ConsultarLoteRps",
            message.ToString(), "ConsultarLoteRpsResult");
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<sis:ConsultarNfseRps>");
        message.Append(msg);
        message.Append("</sis:ConsultarNfseRps>");

        return Execute("http://www.sistema.com.br/Sistema.Ws.Nfse/INfseService/ConsultarNfseRps",
            message.ToString(), "ConsultarNfsePorRpsResult");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<sis:ConsultarNfseRps>");
        message.Append(msg);
        message.Append("</sis:ConsultarNfseRps>");

        return Execute("http://www.sistema.com.br/Sistema.Ws.Nfse/INfseService/ConsultarNfse",
            message.ToString(), "ConsultarNfseResult");
    }

    public string CancelarNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string EnviarSincrono(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<sis:GerarNfse>");
        message.Append(msg);
        message.Append("</sis:GerarNfse>");

        return Execute("http://www.sistema.com.br/Sistema.Ws.Nfse/INfseService/GerarNfse", message.ToString(), "GerarNfseResult");
    }

    public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    private string Execute(string soapAction, string message, string responseTag)
    {
        return Execute(soapAction, message, "", [responseTag], ["xmlns:sis=\"http://www.sistema.com.br/Sistema.Ws.Nfse\"",
            "xmlns:sis1=\"http://www.sistema.com.br/Sistema.Ws.Nfse.Cn\""]);
    }

    public string AjustarMensagem(string msg, params string[] tags)
    {
        var document = XDocument.Parse(msg);
        var element = tags.Aggregate(document.Root, (current, tag) => current.ElementAnyNs(tag));

        element.AddAttribute(new XAttribute(XNamespace.Xmlns + "nfse", tc));
        NFSeUtil.ApplyNamespace(element, tc);

        var result = document.AsString(false, false);
        var tagName = document.Root?.Name.LocalName ?? "";

        return result.Contains($"nfse:{tagName}") ? result.Replace($"nfse:{tagName}", $"sis:{tagName}") :
            result.Replace(tagName, $"sis:{tagName}");
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