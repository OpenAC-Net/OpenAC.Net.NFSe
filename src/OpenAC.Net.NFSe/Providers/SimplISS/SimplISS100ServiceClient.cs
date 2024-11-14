// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 12-26-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 23-01-2020
// ***********************************************************************
// <copyright file="SimplISSServiceClient.cs" company="OpenAC .Net">
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
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class SimplISS100ServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Fields

    private XNamespace tc = "http://www.sistema.com.br/Nfse/arquivos/nfse_3.xsd";

    #endregion Fields

    #region Constructors

    public SimplISS100ServiceClient(ProviderSimplISS100 provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<sis:RecepcionarLoteRps>");
        message.Append(AjustarMensagem(msg, "LoteRps"));
        message.Append(DadosUsuario());
        message.Append("</sis:RecepcionarLoteRps>");

        return Execute("http://www.sistema.com.br/Sistema.Ws.Nfse/INfseService/RecepcionarLoteRps", message.ToString(), "RecepcionarLoteRpsResult");
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<sis:ConsultarSituacaoLoteRps>");
        message.Append(AjustarMensagem(msg));
        message.Append(DadosUsuario());
        message.Append("</sis:ConsultarSituacaoLoteRps>");

        return Execute("http://www.sistema.com.br/Sistema.Ws.Nfse/INfseService/ConsultarSituacaoLoteRps",
            message.ToString(), "ConsultarSituacaoLoteRpsResult");
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<sis:ConsultarLoteRps>");
        message.Append(AjustarMensagem(msg));
        message.Append(DadosUsuario());
        message.Append("</sis:ConsultarLoteRps>");

        return Execute("http://www.sistema.com.br/Sistema.Ws.Nfse/INfseService/ConsultarLoteRps",
            message.ToString(), "ConsultarLoteRpsResult");
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<sis:ConsultarNfsePorRps>");
        message.Append(AjustarMensagem(msg));
        message.Append(DadosUsuario());
        message.Append("</sis:ConsultarNfsePorRps>");

        return Execute("http://www.sistema.com.br/Sistema.Ws.Nfse/INfseService/ConsultarNfsePorRps",
            message.ToString(), "ConsultarNfsePorRpsResult");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<sis:ConsultarNfse>");
        message.Append(AjustarMensagem(msg));
        message.Append(DadosUsuario());
        message.Append("</sis:ConsultarNfse>");

        return Execute("http://www.sistema.com.br/Sistema.Ws.Nfse/INfseService/ConsultarNfse",
            message.ToString(), "ConsultarNfseResult");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<sis:CancelarNfse>");
        message.Append(AjustarMensagem(msg));
        message.Append(DadosUsuario());
        message.Append("</sis:CancelarNfse>");

        return Execute("http://www.sistema.com.br/Sistema.Ws.Nfse/INfseService/CancelarNfse",
            message.ToString(), "CancelarNfseResult");
    }

    public string EnviarSincrono(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string GerarNfse(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    private string Execute(string soapAction, string message, string responseTag)
    {
        Guard.Against<OpenDFeCommunicationException>(ValidarUsernamePassword(), "Faltou informar username e/ou password");

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

    private bool ValidarUsernamePassword() => Provider.Configuracoes.WebServices.Usuario.IsEmpty() && Provider.Configuracoes.WebServices.Senha.IsEmpty();

    private string DadosUsuario()
    {
        return $"<sis:pParam><sis1:P1>{Provider.Configuracoes.WebServices.Usuario.OnlyNumbers()}</sis1:P1>" +
               $"<sis1:P2>{Provider.Configuracoes.WebServices.Senha.HtmlEncode()}</sis1:P2></sis:pParam>";
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element == null)
        {
            element = responseTag.Aggregate(xmlDocument.Elements().First(), (current, tag) => current.ElementAnyNs(tag));
            return element.ToString();
        }

        var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
        throw new OpenDFeCommunicationException(exMessage);
    }

    #endregion Methods
}