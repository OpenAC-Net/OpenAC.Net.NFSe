using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Providers.Barueri
{
    internal sealed class BarueriServiceClient : NFSeSoapServiceClient, IServiceClient
    {
        #region Constructors

        public BarueriServiceClient(ProviderBarueri provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
        {
        }

        #endregion Constructors

        #region Methods

        private string Execute(string soapAction, string message, string responseTag)
        {
            return Execute(soapAction, message, string.Empty, new[] { responseTag }, new[] { "xmlns:wsrps=\"http://www.barueri.sp.gov.br/nfe\"" });
        }

        public string Enviar(string? cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<NFeLoteEnviarArquivo xmlns=\"http://www.barueri.sp.gov.br/nfe\">");
            message.Append("<VersaoSchema>1</VersaoSchema>");
            message.Append($"<MensagemXML><![CDATA[{msg}]]></MensagemXML>");
            message.Append("</NFeLoteEnviarArquivo>");

            var strMessage = message.ToString();

            return Execute("http://www.barueri.sp.gov.br/nfe/NFeLoteEnviarArquivo", strMessage, "NFeLoteEnviarArquivoResponse");
        }

        public string EnviarSincrono(string? cabec, string msg)
        {
            return Enviar(cabec, msg);
        }

        public string ConsultarLoteRps(string? cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<NFeLoteStatusArquivo xmlns=\"http://www.barueri.sp.gov.br/nfe\">");
            message.Append("<VersaoSchema>1</VersaoSchema>");
            message.Append("<MensagemXML><![CDATA[");
            message.Append(msg);
            message.Append("]]></MensagemXML>");
            message.Append("</NFeLoteStatusArquivo>");

            return Execute("http://www.barueri.sp.gov.br/nfe/NFeLoteStatusArquivo", message.ToString(), "NFeLoteStatusArquivoResponse");
        }

        public string ConsultarSituacao(string? cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<NFeLoteStatusArquivo xmlns=\"http://www.barueri.sp.gov.br/nfe\">");
            message.Append("<VersaoSchema>1</VersaoSchema>");
            message.Append("<MensagemXML><![CDATA[");
            message.Append(msg);
            message.Append("]]></MensagemXML>");
            message.Append("</NFeLoteStatusArquivo>");

            return Execute("http://www.barueri.sp.gov.br/nfe/NFeLoteStatusArquivo", message.ToString(), "NFeLoteStatusArquivoResponse");
        }

        public string ConsultarNFSeRps(string? cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<NFeLoteBaixarArquivo xmlns=\"http://www.barueri.sp.gov.br/nfe\">");
            message.Append("<VersaoSchema>1</VersaoSchema>");
            message.Append("<MensagemXML><![CDATA[");
            message.Append(msg);
            message.Append("]]></MensagemXML>");
            message.Append("</NFeLoteBaixarArquivo>");

            return Execute("http://www.barueri.sp.gov.br/nfe/NFeLoteBaixarArquivo", message.ToString(), "NFeLoteBaixarArquivoResponse");
        }

        public string ConsultarNFSe(string? cabec, string msg)
        {
            return ConsultarNFSeRps(cabec, msg);
        }

        public string CancelarNFSe(string? cabec, string msg)
        {
            throw new System.NotImplementedException("Barueri não possui método de cancelamento no web service. O cancelamento deve ser feito através do portal.");
        }

        public string CancelarNFSeLote(string? cabec, string msg)
        {
            throw new System.NotImplementedException("Barueri não suporta cancelamento em lote.");
        }

        public string SubstituirNFSe(string? cabec, string msg)
        {
            throw new System.NotImplementedException("Barueri não suporta substituição de NFSe.");
        }

        protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs(responseTag[0]);
            var retorno = element?.ElementAnyNs("return");
            return retorno?.Value ?? xmlDocument.ToString();
        }

        public string ConsultarSequencialRps(string? cabec, string msg)
        {
            throw new System.NotImplementedException();
        }

        #endregion Methods
    }
}
