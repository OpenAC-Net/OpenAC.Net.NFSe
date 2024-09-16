// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-16-2018
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="CoplanServiceClient.cs" company="OpenAC .Net">
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

using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class CoplanServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Fields

    private bool expect100Continue;

    #endregion Fields

    #region Constructors

    public CoplanServiceClient(ProviderCoplan provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider,
        tipoUrl, certificado, SoapVersion.Soap11)
    {
        expect100Continue = ServicePointManager.Expect100Continue;
        ServicePointManager.Expect100Continue = false;
    }

    public CoplanServiceClient(ProviderCoplan provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
        expect100Continue = ServicePointManager.Expect100Continue;
        ServicePointManager.Expect100Continue = false;
    }

    ~CoplanServiceClient()
    {
        ServicePointManager.Expect100Continue = expect100Continue;
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse_web_service.RECEPCIONARLOTERPS xmlns=\"Tributario\">");
        message.Append("<Recepcionarloterpsrequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</Recepcionarloterpsrequest>");
        message.Append("</nfse_web_service.RECEPCIONARLOTERPS>");

        return Execute("RECEPCIONARLOTERPS", message.ToString(), "nfse_web_service.RECEPCIONARLOTERPSResponse",
            "Recepcionarloterpsresponse");
    }

    public string EnviarSincrono(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse_web_service.RECEPCIONARLOTERPSSINCRONO xmlns=\"Tributario\">");
        message.Append("<Recepcionarloterpssincronorequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</Recepcionarloterpssincronorequest>");
        message.Append("</nfse_web_service.RECEPCIONARLOTERPSSINCRONO>");

        return Execute("RECEPCIONARLOTERPSSINCRONO", message.ToString(),
            "nfse_web_service.RECEPCIONARLOTERPSSINCRONOResponse", "Recepcionarloterpssincronoresponse");
    }

    public string ConsultarSituacao(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarLoteRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse_web_service.CONSULTARLOTERPS xmlns=\"Tributario\">");
        message.Append("<Consultarloterpsrequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</Consultarloterpsrequest>");
        message.Append("</nfse_web_service.CONSULTARLOTERPS>");

        return Execute("CONSULTARLOTERPS", message.ToString(), "nfse_web_service.CONSULTARLOTERPSResponse",
            "Consultarloterpsresponse");
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarNFSeRps(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse_web_service.CONSULTARNFSEPORRPS xmlns=\"Tributario\">");
        message.Append("<Consultarnfseporrpsrequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</Consultarnfseporrpsrequest>");
        message.Append("</nfse_web_service.CONSULTARNFSEPORRPS>");

        return Execute("CONSULTARNFSEPORRPS", message.ToString(), "nfse_web_service.CONSULTARNFSEPORRPSResponse",
            "Consultarnfseporrpsresponse");
    }

    public string ConsultarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse_web_service.CONSULTARNFSESERVICOPRESTADO xmlns=\"Tributario\">");
        message.Append("<Consultarnfseservicoprestadorequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</Consultarnfseservicoprestadorequest>");
        message.Append("</nfse_web_service.CONSULTARNFSESERVICOPRESTADO>");

        return Execute("CONSULTARNFSESERVICOPRESTADO", message.ToString(),
            "nfse_web_service.CONSULTARNFSESERVICOPRESTADOResponse", "Consultarnfseservicoprestadoresponse");
    }

    public string CancelarNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse_web_service.CANCELARNFSE xmlns=\"Tributario\">");
        message.Append("<Cancelarnfserequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</Cancelarnfserequest>");
        message.Append("</nfse_web_service.CANCELARNFSE>");

        return Execute("CANCELARNFSE", message.ToString(), "nfse_web_service.CANCELARNFSEResponse",
            "Cancelarnfseresponse");
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse_web_service.SUBSTITUIRNFSE xmlns=\"Tributario\">");
        message.Append("<Substituirnfserequest>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(cabec);
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(msg);
        message.Append("</nfseDadosMsg>");
        message.Append("</Substituirnfserequest>");
        message.Append("</nfse_web_service.SUBSTITUIRNFSE>");

        return Execute("SUBSTITUIRNFSE", message.ToString(), "nfse_web_service.SUBSTITUIRNFSEResponse",
            "Substituirnfseresponse");
    }

    private string Execute(string action, string message, params string[] responseTag)
    {
        return base.Execute($"Tributarioaction/ANFSE_WEB_SERVICE.{action}", message, "", responseTag, []);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        var element = xmlDocument.ElementAnyNs(responseTag[0])?.ElementAnyNs("Fault");
        if (element != null)
        {
            var exMessage =
                $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
            throw new OpenDFeCommunicationException(exMessage);
        }

        return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs(responseTag[1]).ElementAnyNs("outputXML").Value;
    }

    #endregion Methods
}