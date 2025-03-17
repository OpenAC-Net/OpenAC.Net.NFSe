using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class Fintel204ServiceClient : NFSeSoapServiceClient, IServiceClient
{
    public Fintel204ServiceClient(ProviderBase provider, TipoUrl tipoUrl, X509Certificate2? certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
    {
    }

    private string Execute(string soapAction, string message, string[] responseTag)
    {
        return Execute(soapAction, message, "", responseTag, ["xmlns:nfse=\"http://nfse.abrasf.org.br\""]);
    }

    public string CancelarNFSe(string? cabec, string msg)
    {
        var sb = new StringBuilder();
        sb.Append("<nfse:CancelarNfse>");
        sb.Append($"<nfseCabecMsg>{cabec}</nfseCabecMsg>");
        sb.Append($"<nfseDadosMsg>{msg}</nfseDadosMsg>");
        sb.Append("</nfse:CancelarNfse>");
        return this.Execute("http://nfse.abrasf.org.br/CancelarNfse", sb.ToString(), ["CancelarNfseResult", "CancelarNfseResposta"]);
    }

    public string CancelarNFSeLote(string? cabec, string msg)
    {
        throw new System.NotImplementedException();
    }

    public string ConsultarLoteRps(string? cabec, string msg)
    {
        var sb = new StringBuilder();
        sb.Append("<nfse:ConsultarLoteRps>");
        sb.Append($"<nfseCabecMsg>{cabec}</nfseCabecMsg>");
        sb.Append($"<nfseDadosMsg>{msg}</nfseDadosMsg>");
        sb.Append("</nfse:ConsultarLoteRps>");
        return this.Execute("http://nfse.abrasf.org.br/ConsultarLoteRps", sb.ToString(), ["ConsultarLoteRpsResult", "ConsultarLoteRpsResposta"]);
    }

    public string ConsultarNFSe(string? cabec, string msg)
    {
        var sb = new StringBuilder();
        sb.Append("<nfse:ConsultarNfseFaixa>");
        sb.Append($"<nfse:nfseCabecMsg>{cabec}</nfse:nfseCabecMsg>");
        sb.Append($"<nfse:nfseDadosMsg>{msg}</nfse:nfseDadosMsg>");
        sb.Append("</nfse:ConsultarNfseFaixa>");
        return this.Execute("http://nfse.abrasf.org.br/ConsultarNfseFaixa", sb.ToString(), ["ConsultarNfseFaixaResult", "ConsultarNfseFaixaResposta"]);
    }

    private string EmpacotaXml(string conteudo)
    {
        return string.Concat("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", conteudo);
    }

    public string ConsultarNFSeRps(string? cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:ConsultarNfsePorRps>");

        message.Append("<nfseCabecMsg>");
        message.AppendCData(EmpacotaXml(cabec));
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(EmpacotaXml(msg));
        message.Append("</nfseDadosMsg>");

        message.Append("</nfse:ConsultarNfsePorRps>");
        return this.Execute("http://nfse.abrasf.org.br/ConsultarNfsePorRps", message.ToString(), ["ConsultarNfsePorRpsResult", "ConsultarNfseRpsResposta"]);
    }

    public string ConsultarSequencialRps(string? cabec, string msg)
    {
        throw new System.NotImplementedException();
    }

    public string ConsultarSituacao(string? cabec, string msg)
    {
        throw new System.NotImplementedException();
    }

    public string Enviar(string? cabec, string msg)
    {
        var sb = new StringBuilder();
        sb.Append("<nfse:RecepcionarLoteRps>");
        sb.Append($"<nfseCabecMsg>{cabec}</nfseCabecMsg>");
        sb.Append($"<nfseDadosMsg>{msg}</nfseDadosMsg>");
        sb.Append("</nfse:RecepcionarLoteRps>");
        return this.Execute("http://nfse.abrasf.org.br/RecepcionarLoteRps", sb.ToString(), ["RecepcionarLoteRpsResult", "EnviarLoteRpsResposta"]);
    }

    public string EnviarSincrono(string? cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:RecepcionarLoteRpsSincrono>");

        message.Append("<nfseCabecMsg>");
        message.AppendCData(EmpacotaXml(cabec));
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(EmpacotaXml(msg));
        message.Append("</nfseDadosMsg>");

        message.Append("</nfse:RecepcionarLoteRpsSincrono>");
        return this.Execute("http://nfse.abrasf.org.br/RecepcionarLoteRpsSincrono", message.ToString(), ["RecepcionarLoteRpsSincronoResult", "EnviarLoteRpsSincronoResposta", "RecepcionarLoteRpsSincronoResponse"]);
    }

    public string SubstituirNFSe(string? cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:SubstituirNfse>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(EmpacotaXml(cabec));
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(EmpacotaXml(msg));
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:SubstituirNfse>");
        return this.Execute("http://nfse.abrasf.org.br/SubstituirNfse", message.ToString(), ["SubstituirNfseResult", "SubstituirNfseResposta"]);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("outputXML").Value;
        //return xmlDocument.ElementAnyNs(responseTag[0]).ToString();
    }
}