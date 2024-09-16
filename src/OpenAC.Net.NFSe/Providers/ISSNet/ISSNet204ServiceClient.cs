// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-22-2018
//
// Last Modified By : Leandro Rossi (rossism.com.br)
// Last Modified On : 14-04-2023
// ***********************************************************************
// <copyright file="ISSNetServiceClient.cs" company="OpenAC .Net">
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

using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ISSNet204ServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public ISSNet204ServiceClient(ProviderISSNet204 provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    public ISSNet204ServiceClient(ProviderISSNet204 provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:RecepcionarLoteRps>");
        message.Append("<nfseCabecMsg>");
        message.Append(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.Append(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:RecepcionarLoteRps>");

        return Execute("http://nfse.abrasf.org.br/RecepcionarLoteRps", message.ToString(), "RecepcionarLoteRpsResponse");
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:RecepcionarLoteRpsSincrono>");
        message.Append("<nfseCabecMsg>");
        message.Append(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.Append(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:RecepcionarLoteRpsSincrono>");

        return Execute("http://nfse.abrasf.org.br/RecepcionarLoteRpsSincrono", message.ToString(), "RecepcionarLoteRpsSincronoResponse");
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        throw new System.NotImplementedException();
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:ConsultarLoteRps>");
        message.Append("<nfseCabecMsg>");
        message.Append(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.Append(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:ConsultarLoteRps>");

        return Execute("http://nfse.abrasf.org.br/ConsultarLoteRps", message.ToString(), "ConsultarLoteRpsResponse");
    }

    public string ConsultarSequencialRps(string cabec, string msg) => throw new System.NotImplementedException();

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:ConsultarNfsePorRps>");
        message.Append("<nfseCabecMsg>");
        message.Append(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.Append(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:ConsultarNfsePorRps>");

        return Execute("http://nfse.abrasf.org.br/ConsultarNfsePorRps", message.ToString(), "ConsultarNfsePorRpsResponse");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:ConsultarNfseServicoPrestado>");
        message.Append("<nfseCabecMsg>");
        message.Append(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.Append(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:ConsultarNfseServicoPrestado>");

        return Execute("http://nfse.abrasf.org.br/ConsultarNfseServicoPrestado", message.ToString(), "ConsultarNfseServicoPrestadoResponse");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:CancelarNfse>");
        message.Append("<nfseCabecMsg>");
        message.Append(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.Append(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:CancelarNfse>");

        return Execute("http://nfse.abrasf.org.br/CancelarNfse", message.ToString(), "CancelarNfseResponse");
    }

    public string CancelarNFSeLote(string cabec, string msg) => throw new System.NotImplementedException();

    public string SubstituirNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:SubstituirNfse>");
        message.Append("<nfseCabecMsg>");
        message.Append(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.Append(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:SubstituirNfse>");

        return Execute("http://nfse.abrasf.org.br/SubstituirNfse", message.ToString(), "SubstituirNfseResponse");
    }

    private string Execute(string soapAction, string message, string responseTag)
    {
        return Execute(soapAction, message, "", [responseTag], ["xmlns:nfse=\"http://nfse.abrasf.org.br\""]);
    }

    protected override bool ValidarCertificadoServidor()
    {
        return Provider.Configuracoes.WebServices.Ambiente != DFeTipoAmbiente.Homologacao;
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
