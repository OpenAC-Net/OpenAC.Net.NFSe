using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers.ISSRecife
{
    internal class ISSRecifeServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public ISSRecifeServiceClient(ProviderISSRecife provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
        {
        }

        public ISSRecifeServiceClient(ProviderISSRecife provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<RecepcionarLoteRpsRequest xmlns=\"http://nfse.recife.pe.gov.br/\">");
            message.Append("<inputXML>");
            message.AppendEnvio(msg);
            message.Append("</inputXML>");
            message.Append("</RecepcionarLoteRpsRequest>");

            return Execute("http://nfse.recife.pe.gov.br/RecepcionarLoteRps", message.ToString(), "", "RecepcionarLoteRps", "xmlns:tns=\"http://nfse.recife.pe.gov.br/\"");
        }

        protected override string Execute(string soapAction, string message, string soapHeader, string[] responseTag, params string[] soapNamespaces)
        {            
            var envelope = new StringBuilder();
            envelope.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");

            envelope.Append("<soap:Header/>");
            envelope.Append("<soap:Body>");
            envelope.Append(message);
            envelope.Append("</soap:Body>");
            envelope.Append("</soap:Envelope>");
            EnvelopeEnvio = envelope.ToString();


            StringContent content;
            switch (MessageVersion)
            {
                case SoapVersion.Soap11:
                    content = new StringContent(EnvelopeEnvio, CharSet, "text/xml");
                    if (Provider.Name != NFSeProvider.Sigep.ToString())
                        content.Headers.Add("SOAPAction", $"\"{soapAction}\"");
                    break;

                case SoapVersion.Soap12:
                    content = new StringContent(EnvelopeEnvio, CharSet, "application/soap+xml");
                    content.Headers.ContentType?.Parameters.Add(new NameValueHeaderValue("action", $"\"{soapAction}\""));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            Execute(content, HttpMethod.Post);
            
            var xmlRet = XDocument.Parse(EnvelopeRetorno);            
            var el = xmlRet.Descendants().FirstOrDefault(e => e.Name.LocalName == "outputXML");

            return el.Value;
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            return Execute(msg.Replace("ConsultarSituacaoLoteRpsEnvio", "e:ConsultarSituacaoLoteRpsEnvio"));
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarLoteRpsRequest xmlns=\"http://nfse.recife.pe.gov.br/\">");
            message.Append("<inputXML>");
            message.AppendEnvio(msg);
            message.Append("</inputXML>");
            message.Append("</ConsultarLoteRpsRequest>");

            return Execute("http://nfse.recife.pe.gov.br/ConsultarLoteRps", message.ToString(), "", "ConsultarLoteRps", "xmlns:tns=\"http://nfse.recife.pe.gov.br/\"");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            return Execute(msg.Replace("ConsultarNfsePorRpsEnvio", "e:ConsultarNfsePorRpsEnvio"));
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            return Execute(msg.Replace("ConsultarNfseEnvio", "e:ConsultarNfseEnvio"));
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<CancelarNfseRequest xmlns=\"http://nfse.recife.pe.gov.br/\">");
            message.Append("<inputXML>");
            message.AppendEnvio(msg);
            message.Append("</inputXML>");
            message.Append("</CancelarNfseRequest>");

            return Execute("http://nfse.recife.pe.gov.br/CancelarNfse", message.ToString(), "", "CancelarNfse", "xmlns:tns=\"http://nfse.recife.pe.gov.br/\"");
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        private string Execute(string message)
        {
            return Execute("", message, "", "", "xmlns:e=\"http://www.abrasf.org.br/ABRASF/arquivos/nfse.xsd\"");
        }

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}
