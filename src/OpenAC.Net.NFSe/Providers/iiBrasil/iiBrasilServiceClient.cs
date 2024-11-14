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
using System.ServiceModel.Channels;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class iiBrasilServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public iiBrasilServiceClient(ProvideriiBrasil provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    public string GerarNfse(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<e:GerarNfseRequest>");
        message.Append("<nfseCabecMsg>");
        message.Append(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.Append(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</e:GerarNfseRequest>");

        return Execute("http://nfse.abrasf.org.br/GerarNfse", message.ToString(), "GerarNfseResponse");
    }

    public string ConsultarSituacao(string cabecalho, string dados)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    public string ConsultarSequencialRps(string cabec, string msg) =>
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string ConsultarLoteRps(string cabec, string msg)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<e:ConsultarNfsePorRpsRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</e:ConsultarNfsePorRpsRequest>");

        return Execute("http://nfse.abrasf.org.br/ConsultarNfsePorRps", message.ToString(), "ConsultarNfsePorRpsResponse");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<e:CancelarNfseRequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</e:CancelarNfseRequest>");

        return Execute("http://nfse.abrasf.org.br/CancelarNfse", message.ToString(), "CancelarNfseResponse");
    }

    public string CancelarNFSeLote(string cabec, string msg) =>
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string SubstituirNFSe(string cabec, string msg) =>
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    private string Execute(string soapAction, string message, string responseTag)
    {
        return Execute(soapAction, message, "", [responseTag], ["xmlns:e=\"http://nfse.abrasf.org.br\""]);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs("Fault");
        if (element == null) return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("outputXML").Value;

        var exMessage =
            $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
        throw new OpenDFeCommunicationException(exMessage);
    }

    #endregion Methods
}