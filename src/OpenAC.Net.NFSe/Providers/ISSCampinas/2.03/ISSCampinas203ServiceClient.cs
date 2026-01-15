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

using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ISSCampinas203ServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Fields


    #endregion Fields

    #region Constructors

    public ISSCampinas203ServiceClient(ProviderISSCampinas203 provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        return Execute("https://novanfse.campinas.sp.gov.br/notafiscal-abrasfv203-ws/NotaFiscalSoap/RecepcionarLoteRps", msg.ToString(), "RecepcionarLoteRpsResponse");
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<ConsultarSituacaoLoteRps>");
        message.Append(msg);
        message.Append("</ConsultarSituacaoLoteRps>");

        return Execute("https://novanfse.campinas.sp.gov.br/notafiscal-abrasfv203-ws/NotaFiscalSoap/ConsultarSituacaoLoteRps",
            message.ToString(), "ConsultarSituacaoLoteRpsResult");
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
       
        var message = new StringBuilder();
        message.Append("<tns:ConsultarLoteRps>");
        message.Append(msg);
        message.Append("</tns:ConsultarLoteRps>");

        return Execute("", message.ToString(), "ConsultarLoteRpsResponse");
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<tns:ConsultarNfsePorRps>");
        message.Append(msg);
        message.Append("</tns:ConsultarNfsePorRps>");

        return Execute("", message.ToString(), "ConsultarNfsePorRpsResponse");
    }

    public string ConsultarNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<tns:CancelarNfse>");
        message.Append(msg);
        message.Append("</tns:CancelarNfse>");

        return Execute("", message.ToString(), "CancelarNfseResponse");
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<tns:RecepcionarLoteRpsSincrono>");
        message.Append(msg);
        message.Append("</tns:RecepcionarLoteRpsSincrono>");

        return Execute("", message.ToString(), "RecepcionarLoteRpsSincronoResponse");
      
    }

    
    public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");


    private string Execute(string soapAction, string message, string responseTag)
    {
        return Execute(soapAction, message, "", [responseTag], ["xmlns:tns= \"http://nfse.abrasf.org.br\""]);
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
        return reader.ReadInnerXml();
    }

    #endregion Methods
}