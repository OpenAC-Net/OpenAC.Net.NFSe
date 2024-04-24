using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers.Agili
{
    internal class AgiliServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        public AgiliServiceClient(ProviderAgili provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
        {
        }

        public AgiliServiceClient(ProviderAgili provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
        {
        }

        public string Enviar(string cabec, string msg)
        {
            Execute(new StringContent(msg, CharSet, "application/xml"), HttpMethod.Post);

            return EnvelopeRetorno;
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            Execute(new StringContent(msg, CharSet, "application/xml"), HttpMethod.Post);

            return EnvelopeRetorno;
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            return Execute(msg.Replace("ConsultarSituacaoLoteRpsEnvio", "e:ConsultarSituacaoLoteRpsEnvio"));
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarLoteRpsRequest xmlns=\"http://www.agili.com.br/nfse_v_1.00.xsd\">");
            message.Append("<inputXML>");
            message.AppendEnvio(msg);
            message.Append("</inputXML>");
            message.Append("</ConsultarLoteRpsRequest>");

            return Execute("http://nfse.abrasf.org.br/ConsultarLoteRps", message.ToString(), "", "ConsultarLoteRps", "xmlns:tns=\"http://www.agili.com.br/nfse_v_1.00.xsd\"");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            Execute(new StringContent(msg, CharSet, "application/xml"), HttpMethod.Post);

            return EnvelopeRetorno;
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            Execute(new StringContent(msg, CharSet, "application/xml"), HttpMethod.Post);

            return EnvelopeRetorno;
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            Execute(new StringContent(msg, CharSet, "application/xml"), HttpMethod.Post);

            return EnvelopeRetorno;
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
            return Execute("", message, "", "", "xmlns:e=\"http://www.agili.com.br/nfse_v_1.00.xsd\"");
        }

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            throw new NotImplementedException();
        }
    }
}
