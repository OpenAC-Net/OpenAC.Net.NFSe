using OpenAC.Net.DFe.Core;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using OpenAC.Net.Core.Extensions;
using System.Xml.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.ServiceModel.Channels;

namespace OpenAC.Net.NFSe.Providers.GISS
{
    internal class GISSServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public GISSServiceClient(ProviderGISS provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
        {
        }

        public GISSServiceClient(ProviderGISS provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfse:RecepcionarLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData("<cabecalho versao=\"2.00\"><versaoDados>2.00</versaoDados></cabecalho>");
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");

            message.Append("</nfse:RecepcionarLoteRpsRequest>");

            return Execute("RecepcionarLoteRpsRequest", message.ToString(), "EnviarLoteRpsResposta");
        }

        public string EnviarSincrono(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfse:ConsultarLoteRps>");
            message.Append("<ConsultarLoteRpsEnvio>");
            message.AppendCData(msg);
            message.Append("</ConsultarLoteRpsEnvio>");
            message.Append("</nfse:ConsultarLoteRps>");

            return Execute("ConsultarLoteRps", message.ToString(), "ConsultarLoteRpsResposta");
        }

        public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfse:ConsultarNfseRpsEnvio>");
            message.Append("<ConsultarNfseRpsEnvio>");
            message.AppendCData(msg);
            message.Append("</ConsultarNfseRpsEnvio>");
            message.Append("</nfse:consultarNfseRps>");

            return Execute("ConsultarNfseRpsEnvio", message.ToString(), "ConsultarNfseRpsEnvioResponse");
        }

        public string ConsultarNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<nfse:cancelarNfse>");
            message.Append("<CancelarNfseEnvio>");
            message.AppendCData(msg);
            message.Append("</CancelarNfseEnvio>");
            message.Append("</nfse:cancelarNfse>");

            return Execute("cancelarNfse", message.ToString(), "CancelarNfseResposta");
        }

        public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        private string Execute(string soapAction, string message, string responseTag)
        {
            return Execute(soapAction, message, "", responseTag, "xmlns:nfse=\"http://nfse.abrasf.org.br\"");
        }

        public bool ValidarUsernamePassword()
        {
            return !string.IsNullOrEmpty(Provider.Configuracoes.WebServices.Usuario) && !string.IsNullOrEmpty(Provider.Configuracoes.WebServices.Senha);
        }

        protected override string TratarRetorno(XElement xElement, string[] responseTag)
        {
            var reader = xElement.ElementAnyNs(responseTag[0]).CreateReader();
            reader.MoveToContent();
            var xml = reader.ReadInnerXml().Replace("ns2:", string.Empty);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlDocument xmlMensagem = new XmlDocument();
            xmlMensagem.LoadXml(xmlDoc.LastChild.InnerText);
            var mensagem = xmlMensagem.GetElementsByTagName("Mensagem");
            if (mensagem.Count == 0)
                return xElement.ToString();
            else
                return mensagem[0].InnerText;
        }

        #endregion Methods
    }
}
