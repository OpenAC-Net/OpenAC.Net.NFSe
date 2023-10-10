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

namespace OpenAC.Net.NFSe.Providers.Sigep
{
    internal class SigepServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public SigepServiceClient(ProviderSigep provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
        {
        }

        public SigepServiceClient(ProviderSigep provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:enviarLoteRpsSincrono>");
            message.Append("<EnviarLoteRpsSincronoEnvio>");
            message.AppendCData(msg);
            message.Append("</EnviarLoteRpsSincronoEnvio>");

            message.Append("</ws:enviarLoteRpsSincrono>");

            return Execute("enviarLoteRpsSincrono", message.ToString(), "enviarLoteRpsSincronoResponse");
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:gerarNfse>");
            message.Append("<GerarNfseEnvio>");
            message.AppendCData(msg);
            message.Append("</GerarNfseEnvio>");
            message.Append("</ws:gerarNfse>");

            return Execute("gerarNfse", message.ToString(), "gerarNfseResponse");
        }

        public string ConsultarSituacao(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:consultarLoteRps>");
            message.Append("<consultarLoteRps>");
            message.AppendCData(msg);
            message.Append("</consultarLoteRps>");
            message.Append("</ws:consultarLoteRps>");

            return Execute("consultarLoteRps", message.ToString(), "consultarLoteRpsResponse");
        }

        public string ConsultarSequencialRps(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarNfseRpsEnvio>");
            message.Append("<ConsultarNfseRpsEnvio>");
            message.AppendCData(msg);
            message.Append("</ConsultarNfseRpsEnvio>");
            message.Append("</ws:consultarNfseRps>");

            return Execute("ConsultarNfseRpsEnvio", message.ToString(), "ConsultarNfseRpsEnvioResponse");
        }

        public string ConsultarNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:cancelarNfse>");
            message.Append("<CancelarNfseEnvio>");
            message.AppendCData(msg);
            message.Append("</CancelarNfseEnvio>");
            message.Append("</ws:cancelarNfse>");

            return Execute("cancelarNfse", message.ToString(), "cancelarNfseResponse");
        }

        public string CancelarNFSeLote(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        public string SubstituirNFSe(string cabec, string msg) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        private string Execute(string soapAction, string message, string responseTag)
        {
            var result = ValidarUsernamePassword();
            if (!result) throw new OpenDFeCommunicationException("Faltou informar username e/ou password");

            return Execute(soapAction, message, "", responseTag, "xmlns:ws=\"http://ws.integration.pm.bsit.com.br/\"");
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
