using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class FintelISS204ServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public FintelISS204ServiceClient(ProviderBase provider, TipoUrl tipoUrl, X509Certificate2? certificado) : base(provider, tipoUrl, certificado, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    private string Execute(string soapAction, string message, string[] responseTag)
    {
        return Execute(soapAction, message, "", responseTag, ["xmlns:nfse=\"http://nfse.abrasf.org.br\""]);
    }

    public string CancelarNFSe(string? cabec, string msg)
    {
        var message = new StringBuilder();
        message.Append("<nfse:CancelarNfse>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(EmpacotaXml(cabec));
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(EmpacotaXml(msg));
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:CancelarNfse>");
        return Execute("http://nfse.abrasf.org.br/CancelarNfse", message.ToString(), ["CancelarNfseResponse"]);
    }

    public string CancelarNFSeLote(string? cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarLoteRps(string? cabec, string msg)
    {
        // metodo nao testado
        var message = new StringBuilder();
        message.Append("<nfse:ConsultarLoteRps>");

        message.Append("<nfseCabecMsg>");
        message.AppendCData(EmpacotaXml(cabec));
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(EmpacotaXml(msg));
        message.Append("</nfseDadosMsg>");

        message.Append("</nfse:ConsultarLoteRps>");
        return Execute("http://nfse.abrasf.org.br/ConsultarLoteRps", message.ToString(), ["ConsultarLoteRpsResponse"]);
    }

    public string ConsultarNFSe(string? cabec, string msg)
    {
        // metodo nao testado
        var message = new StringBuilder();
        message.Append("<nfse:ConsultarNfseFaixa>");

        message.Append("<nfseCabecMsg>");
        message.AppendCData(EmpacotaXml(cabec));
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(EmpacotaXml(msg));
        message.Append("</nfseDadosMsg>");

        message.Append("</nfse:ConsultarNfseFaixa>");
        return Execute("http://nfse.abrasf.org.br/ConsultarNfseFaixa", message.ToString(), ["ConsultarNfseFaixaResponse"]);
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
        return Execute("http://nfse.abrasf.org.br/ConsultarNfsePorRps", message.ToString(), ["ConsultarNfsePorRpsResponse"]);
    }

    public string ConsultarSequencialRps(string? cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string ConsultarSituacao(string? cabec, string msg)
    {
        throw new NotImplementedException();
    }

    public string Enviar(string? cabec, string msg)
    {
        // metodo nao testado
        var message = new StringBuilder();
        message.Append("<nfse:RecepcionarLoteRps>");

        message.Append("<nfseCabecMsg>");
        message.AppendCData(EmpacotaXml(cabec));
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(EmpacotaXml(msg));
        message.Append("</nfseDadosMsg>");

        message.Append("</nfse:RecepcionarLoteRps>");
        return Execute("http://nfse.abrasf.org.br/RecepcionarLoteRps", message.ToString(), ["RecepcionarLoteRpsResponse"]);
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
        return Execute("http://nfse.abrasf.org.br/RecepcionarLoteRpsSincrono", message.ToString(), ["RecepcionarLoteRpsSincronoResponse"]);
    }

    public string SubstituirNFSe(string? cabec, string msg)
    {
        // m�todo n�o testado
        var message = new StringBuilder();
        message.Append("<nfse:SubstituirNfse>");
        message.Append("<nfseCabecMsg>");
        message.AppendCData(EmpacotaXml(cabec));
        message.Append("</nfseCabecMsg>");
        message.Append("<nfseDadosMsg>");
        message.AppendCData(EmpacotaXml(msg));
        message.Append("</nfseDadosMsg>");
        message.Append("</nfse:SubstituirNfse>");
        return Execute("http://nfse.abrasf.org.br/SubstituirNfse", message.ToString(), ["SubstituirNfseResponse"]);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("outputXML").Value;
    }

    #endregion Methods
}