 

#region

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core; 

#endregion

namespace OpenAC.Net.NFSe.Providers.Metropolisweb
{
    internal sealed class MetropolisWebAbrasfClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Construtor

        public MetropolisWebAbrasfClient(ProviderMetropolisWebAbrasf provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
        {
        }

        #endregion

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            if (xmlDocument == null) return "";
            var output = xmlDocument.Descendants("outputXML").FirstOrDefault();
            return output?.Value;
        }

         protected override string Execute(string soapAction, string message, string soapHeader, string[] responseTag, params string[] soapNamespaces)
        { 
            return base.Execute(soapAction, message, soapHeader, responseTag, "xmlns:end=\"http://endpoint.nfse.ws.webservicenfse.edza.com.br/\"");
        }

        #region Implementacoes

        public string Enviar(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<end:RecepcionarLoteRps>");
            message.Append("<RecepcionarLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</RecepcionarLoteRpsRequest>");
            message.Append("</end:RecepcionarLoteRps>");
            return Execute("http://endpoint.nfse.ws.webservicenfse.edza.com.br/NfseEndpoint/RecepcionarLoteRpsRequest", message.ToString(), "RecepcionarLoteRpsResponse");
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            throw new NotImplementedException("Serviço não disponível por este provedor");
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<end:ConsultarSituacaoLoteRps>");
            message.Append("<ConsultarSituacaoLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ConsultarSituacaoLoteRpsRequest>");
            message.Append("</end:ConsultarSituacaoLoteRps>");
            return Execute("", message.ToString(), "");
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<end:ConsultarLoteRps>");
            message.Append("<ConsultarLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ConsultarLoteRpsRequest>");
            message.Append("</end:ConsultarLoteRps>");
            return Execute("", message.ToString(), "");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException("Serviço não disponível por este provedor");
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<end:ConsultarNfsePorRps>");
            message.Append("<ConsultarNfsePorRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ConsultarNfsePorRpsRequest>");
            message.Append("</end:ConsultarNfsePorRps>");
            return Execute("", message.ToString(), "");
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<end:ConsultarNfse>");
            message.Append("<ConsultarNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ConsultarNfseRequest>");
            message.Append("</end:ConsultarNfse>");
            return Execute("", message.ToString(), "");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<end:CancelarNfse>");
            message.Append("<CancelarNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</CancelarNfseRequest>");
            message.Append("</end:CancelarNfse>");
            return Execute("", message.ToString(), "");
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException("Serviço não disponível por este provedor");
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new NotImplementedException("Serviço não disponível por este provedor");
        }

        #endregion
    }
}
