// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 12-26-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 23-01-2020
// ***********************************************************************
// <copyright file="AbacoServiceClient.cs" company="OpenAC .Net">
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
using OpenAC.Net.DFe.Core.Common;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class AbacoServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public AbacoServiceClient(ProviderAbaco provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:RecepcionarLoteRPS.Execute>");
            message.Append("<e:Nfsecabecmsg>");
            message.AppendCData(cabec);
            message.Append("</e:Nfsecabecmsg>");
            message.Append("<e:Nfsedadosmsg>");
            message.AppendCData(msg);
            message.Append("</e:Nfsedadosmsg>");
            message.Append("</e:RecepcionarLoteRPS.Execute>");

            return Execute("http://www.e-nfs.com.braction/ARECEPCIONARLOTERPS.Execute", message.ToString(), "RecepcionarLoteRPS.ExecuteResponse");
        }

        public string EnviarSincrono(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string ConsultarSituacao(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:ConsultarSituacaoLoteRPS.Execute>");
            message.Append("<e:Nfsecabecmsg>");
            message.AppendCData(cabec);
            message.Append("</e:Nfsecabecmsg>");
            message.Append("<e:Nfsedadosmsg>");
            message.AppendCData(msg);
            message.Append("</e:Nfsedadosmsg>");
            message.Append("</e:ConsultarSituacaoLoteRPS.Execute>");

            return Execute("http://www.e-nfs.com.braction/ACONSULTARSITUACAOLOTERPS.Execute", message.ToString(), "ConsultarSituacaoLoteRPS.ExecuteResponse");
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:ConsultarLoteRps.Execute>");
            message.Append("<e:Nfsecabecmsg>");
            message.AppendCData(cabec);
            message.Append("</e:Nfsecabecmsg>");
            message.Append("<e:Nfsedadosmsg>");
            message.AppendCData(msg);
            message.Append("</e:Nfsedadosmsg>");
            message.Append("</e:ConsultarLoteRps.Execute>");

            return Execute("http://www.e-nfs.com.braction/ACONSULTARLOTERPS.Execute", message.ToString(), "ConsultarLoteRps.ExecuteResponse");
        }

        public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:ConsultarNfsePorRps.Execute>");
            message.Append("<e:Nfsecabecmsg>");
            message.AppendCData(cabec);
            message.Append("</e:Nfsecabecmsg>");
            message.Append("<e:Nfsedadosmsg>");
            message.AppendCData(msg);
            message.Append("</e:Nfsedadosmsg>");
            message.Append("</e:ConsultarNfsePorRps.Execute>");

            return Execute("http://www.e-nfs.com.braction/ACONSULTARNFSEPORRPS.Execute", message.ToString(), "ConsultarNfsePorRps.ExecuteResponse");
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:ConsultarNfse.Execute>");
            message.Append("<e:Nfsecabecmsg>");
            message.AppendCData(cabec);
            message.Append("</e:Nfsecabecmsg>");
            message.Append("<e:Nfsedadosmsg>");
            message.AppendCData(msg);
            message.Append("</e:Nfsedadosmsg>");
            message.Append("</e:ConsultarNfse.Execute>");

            return Execute("http://www.e-nfs.com.braction/ACONSULTARNFSE.Execute", message.ToString(), "ConsultarNfse.ExecuteResponse");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:CancelarNfse.Execute>");
            message.Append("<e:Nfsecabecmsg>");
            message.AppendCData(cabec);
            message.Append("</e:Nfsecabecmsg>");
            message.Append("<e:Nfsedadosmsg>");
            message.AppendCData(msg);
            message.Append("</e:Nfsedadosmsg>");
            message.Append("</e:CancelarNfse.Execute>");

            return Execute("http://www.e-nfs.com.braction/ACANCELARNFSE.Execute", message.ToString(), "CancelarNfse.ExecuteResponse");
        }

        public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        private string Execute(string soapAction, string message, string responseTag)
        {
            return Execute(soapAction, message, "", responseTag, "xmlns:e=\"http://www.e-nfs.com.br\"");
        }

        protected override bool ValidarCertificadoServidor()
        {
            return Provider.Configuracoes.WebServices.Ambiente != DFeTipoAmbiente.Homologacao;
        }

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element == null) return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("Outputxml").Value;

            var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
            throw new OpenDFeCommunicationException(exMessage);
        }

        #endregion Methods
    }
}