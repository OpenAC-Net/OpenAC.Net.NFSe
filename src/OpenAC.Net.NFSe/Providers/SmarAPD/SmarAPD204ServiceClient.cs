// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 21-01-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 23-01-2020
// ***********************************************************************
// <copyright file="SmarAPDABRASFServiceClient.cs" company="OpenAC .Net">
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

using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class SmarAPD204ServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Fields

    /// <summary>
    /// Inicializado com 1 para manter o mesmo funcionamento do código anterior
    /// Também para ter uma lógica de configuração semelhante ao projeto ACBr
    /// </summary>
    private int _subVersao = 1;

    #endregion

    #region Constructors

    public SmarAPD204ServiceClient(ProviderSmarAPD204 provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
    {
        if (provider.Municipio.Parametros.TryGetValue("SubVersao", out string? value))
            int.TryParse(value, out this._subVersao);
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();
        string tagResponse;
        if (this._subVersao == 1)
        {
            tagResponse = "recepcionarLoteRpsResponse";
            message.Append("<nfse:recepcionarLoteRps>");
            message.Append("<xml>");
            message.AppendCData(msg);
            message.Append("</xml>");
            message.Append("</nfse:recepcionarLoteRps>");
        }
        else
        {
            tagResponse = "RecepcionarLoteRpsResponse";
            message.Append("<nfse:RecepcionarLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</nfse:RecepcionarLoteRpsRequest>");
        }

        return Execute("http://nfse.abrasf.org.br/RecepcionarLoteRps", message.ToString(), tagResponse);
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        var message = new StringBuilder();
        string tagResponse;
        if (this._subVersao == 1)
        {
            tagResponse = "recepcionarLoteRpsSincronoResponse";
            message.Append("<nfse:recepcionarLoteRpsSincrono>");
            message.Append("<xml>");
            message.AppendCData(msg);
            message.Append("</xml>");
            message.Append("</nfse:recepcionarLoteRpsSincrono>");
        }
        else
        {
            tagResponse = "RecepcionarLoteRpsSincronoResponse";
            message.Append("<nfse:RecepcionarLoteRpsSincronoRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</nfse:RecepcionarLoteRpsSincronoRequest>");
        }


        return Execute("http://nfse.abrasf.org.br/RecepcionarLoteRpsSincrono", message.ToString(), tagResponse);
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        throw new System.NotImplementedException();
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        string tagResponse;
        if (this._subVersao == 1)
        {
            tagResponse = "consultarLoteRpsResponse";
            message.Append("<nfse:consultarLoteRps>");
            message.Append("<xml>");
            message.AppendCData(msg);
            message.Append("</xml>");
            message.Append("</nfse:consultarLoteRps>");
        }
        else
        {
            tagResponse = "ConsultarLoteRpsResponse";
            message.Append("<nfse:ConsultarLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</nfse:ConsultarLoteRpsRequest>");
        }

        return Execute("http://nfse.abrasf.org.br/ConsultarLoteRps", message.ToString(), tagResponse);
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new System.NotImplementedException();
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        string tagResponse;
        if (this._subVersao == 1)
        {
            tagResponse = "consultarNfsePorRpsResponse";
            message.Append("<nfse:consultarNfsePorRps>");
            message.Append("<xml>");
            message.AppendCData(msg);
            message.Append("</xml>");
            message.Append("</nfse:consultarNfsePorRps>");
        }
        else
        {
            tagResponse = "ConsultarNfsePorRpsResponse";
            message.Append("<nfse:ConsultarNfsePorRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</nfse:ConsultarNfsePorRpsRequest>");
        }

        return Execute("http://nfse.abrasf.org.br/ConsultarNfsePorRps", message.ToString(), tagResponse);
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        string tagResponse;
        if (this._subVersao == 1)
        {
            tagResponse = "consultarNfsePorRpsResponse";
            message.Append("<nfse:consultarNfseServicoPrestado>");
            message.Append("<xml>");
            message.AppendCData(msg);
            message.Append("</xml>");
            message.Append("</nfse:consultarNfseServicoPrestado>");
        }
        else
        {
            tagResponse = "ConsultarNfsePorRpsResponse";
            message.Append("<nfse:ConsultarNfseServicoPrestadoRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</nfse:ConsultarNfseServicoPrestadoRequest>");
        }

        return Execute("http://nfse.abrasf.org.br/ConsultarNfseServicoPrestado", message.ToString(), tagResponse);
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        string tagResponse;
        if (this._subVersao == 1)
        {
            tagResponse = "cancelarNfseResponse";
            message.Append("<nfse:cancelarNfse>");
            message.Append("<xml>");
            message.AppendCData(msg);
            message.Append("</xml>");
            message.Append("</nfse:cancelarNfse>");
        }
        else
        {
            tagResponse = "CancelarNfseResponse";
            message.Append("<nfse:CancelarNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</nfse:CancelarNfseRequest>");
        }

        return Execute("http://nfse.abrasf.org.br/CancelarNfse", message.ToString(), tagResponse);
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new System.NotImplementedException();
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        string tagResponse;
        if (this._subVersao == 1)
        {
            tagResponse = "substituirNfseResponse";
            message.Append("<nfse:substituirNfse>");
            message.Append("<xml>");
            message.AppendCData(msg);
            message.Append("</xml>");
            message.Append("</nfse:substituirNfse>");
        }
        else
        {
            tagResponse = "SubstituirNfseResponse";
            message.Append("<nfse:SubstituirNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</nfse:SubstituirNfseRequest>");
        }

        return Execute("http://nfse.abrasf.org.br/SubstituirNfse", message.ToString(), tagResponse);
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

        if (this._subVersao == 1)
            return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("return").Value;
        else

            return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("outputXML").Value;
    }

    #endregion Methods
}