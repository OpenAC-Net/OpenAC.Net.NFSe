// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 10-08-2014
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="DSFServiceClient.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2022 Projeto OpenAC .Net
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
using OpenAC.Net.DFe.Core;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class DSFServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructor

        public DSFServiceClient(ProviderBase provider, TipoUrl tipoUrl) : base(provider, tipoUrl, null, SoapVersion.Soap11)
        {
        }

        #endregion Constructor

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            var servico = EhHomologacao ? "testeEnviar" : "enviar";

            var message = new StringBuilder();
            message.Append($"<proc:{servico}>");
            message.Append("<mensagemXml>");
            message.AppendCData(msg);
            message.Append("</mensagemXml>");
            message.Append($"</proc:{servico}>");

            var response = EhHomologacao ? "testeEnviarResponse" : "enviarResponse";
            var responseReturn = EhHomologacao ? "testeEnviarReturn" : "enviarReturn";

            return Execute(message.ToString(), response, responseReturn);
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            if (EhHomologacao) throw new NotImplementedException();

            var message = new StringBuilder();
            message.Append("<proc:enviarSincrono>");
            message.Append("<mensagemXml>");
            message.AppendEnvio(msg);
            message.Append("</mensagemXml>");
            message.Append("</proc:enviarSincrono>");

            return Execute(message.ToString(), "enviarSincronoResponse", "enviarSincronoReturn");
        }

        public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException();

        public string ConsultarLoteRps(string cabec, string msg)
        {
            if (EhHomologacao) throw new NotImplementedException();

            var message = new StringBuilder();
            message.Append("<proc:consultarLote>");
            message.Append("<mensagemXml>");
            message.AppendEnvio(msg);
            message.Append("</mensagemXml>");
            message.Append("</proc:consultarLote>");

            return Execute(message.ToString(), "consultarLoteResponse", "consultarLoteReturn");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            if (EhHomologacao) throw new NotImplementedException();

            var message = new StringBuilder();
            message.Append("<proc:consultarSequencialRps>");
            message.Append("<mensagemXml>");
            message.AppendEnvio(msg);
            message.Append("</mensagemXml>");
            message.Append("</proc:consultarSequencialRps>");

            return Execute(message.ToString(), "consultarSequencialRpsResponse", "consultarSequencialRpsReturn");
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            if (EhHomologacao) throw new NotImplementedException();

            var message = new StringBuilder();
            message.Append("<proc:consultarNFSeRps>");
            message.Append("<mensagemXml>");
            message.AppendEnvio(msg);
            message.Append("</mensagemXml>");
            message.Append("</proc:consultarNFSeRps>");

            return Execute(message.ToString(), "consultarNFSeRpsResponse", "consultarNFSeRpsReturn");
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            if (EhHomologacao) throw new NotImplementedException();

            var message = new StringBuilder();
            message.Append("<proc:consultarNota>");
            message.Append("<mensagemXml>");
            message.AppendEnvio(msg);
            message.Append("</mensagemXml>");
            message.Append("</proc:consultarNota>");

            return Execute(message.ToString(), "consultarNotaResponse", "consultarNotaReturn");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            if (EhHomologacao) throw new NotImplementedException();

            var message = new StringBuilder();
            message.Append("<proc:cancelar>");
            message.Append("<mensagemXml>");
            message.AppendEnvio(msg);
            message.Append("</mensagemXml>");
            message.Append("</proc:cancelar>");

            return Execute(message.ToString(), "cancelarResponse", "cancelarReturn");
        }

        public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException();

        public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException();

        private string Execute(string message, params string[] reponseTags) => Execute("", message, reponseTags, "xmlns:proc=\"http://proces.wsnfe2.dsfnet.com.br\"");

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element == null) return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs(responseTag[1]).Value;

            var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
            throw new OpenDFeCommunicationException(exMessage);
        }

        #endregion Methods
    }
}