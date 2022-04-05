// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Shared
// Author           : Rafael Dias
// Created          : 06-02-2018
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-02-2018
// ***********************************************************************
// <copyright file="FissLexServiceClient.cs" company="OpenAC .Net">
//		        	   The MIT License (MIT)
//	     		Copyright (c) 2014 - 2021 Projeto OpenAC .Net
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
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class FissLexServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public FissLexServiceClient(ProviderFissLex provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            return Execute("FISS-LEXaction/AWS_RECEPCIONARLOTERPS.Execute", msg,
                   new[] { "WS_RecepcionarLoteRps.ExecuteResponse", "Enviarloterpsresposta" });
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            return Execute("FISS-LEXaction/AWS_CONSULTARSITUACAOLOTERPS.Execute", msg,
                   new[] { "WS_ConsultarSituacaoLoteRps.ExecuteResponse" });
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            return Execute("FISS-LEXaction/AWS_CONSULTALOTERPS.Execute", msg, new string[0]);
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            return Execute("FISS-LEXaction/AWS_CONSULTANFSEPORRPS.Execute", msg, new string[0]);
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            return Execute("FISS-LEXaction/AWS_CONSULTANFSE.Execute", msg, new string[0]);
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            return Execute("FISS-LEXaction/AWS_CANCELARNFSE.Execute", msg,
                   new[] { "WS_CancelarNfse.ExecuteResponse", "Cancelarnfseresposta" });
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            var element = responseTag.Aggregate(xmlDocument, (current, tag) => current.ElementAnyNs(tag));
            return element.ToString();
        }

        #endregion Methods
    }
}