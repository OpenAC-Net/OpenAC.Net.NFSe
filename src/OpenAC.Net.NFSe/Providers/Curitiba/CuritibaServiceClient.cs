using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;

namespace OpenAC.Net.NFSe.Providers.Curitiba
{
    internal sealed class CuritibaServiceClient : NFSeSOAP11ServiceClient, IServiceClient
    {
        #region Constructors

        public CuritibaServiceClient(ProviderCuritiba provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        public CuritibaServiceClient(ProviderCuritiba provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado)
        {
        }

        #endregion Constructors

        #region Methods

        public string CancelarNFSe(string cabec, string msg)
        {
            return Execute("http://www.e-governeapps2.com.br/CancelarNfse", msg.ToString(), "CancelarNfseResponse");
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            return Execute("http://www.e-governeapps2.com.br/ConsultarLoteRps", msg.ToString(), "ConsultarLoteRpsResponse");
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            return Execute("http://www.e-governeapps2.com.br/ConsultarNfse", msg.ToString(), "ConsultarNfseResponse");
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            return Execute("http://www.e-governeapps2.com.br/ConsultarNfsePorRps", msg.ToString(), "ConsultarNfsePorRpsResponse");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            return Execute("http://www.e-governeapps2.com.br/ConsultarSituacaoLoteRps", msg.ToString(), "ConsultarSituacaoLoteRpsResult");
        }

        public string Enviar(string cabec, string msg)
        {
            return Execute("http://www.e-governeapps2.com.br/RecepcionarLoteRps", msg.ToString(), "RecepcionarLoteRpsResponse");
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        private string Execute(string action, string message, string responseTag)
        {
            return Execute(action, message, responseTag, new string[0]);
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element == null) return xmlDocument.ToString();

            var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
            throw new OpenDFeCommunicationException(exMessage);
        }

        #endregion Methods
    }
}