using System;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ConamServiceClient : NFSeSoapServiceClient, IServiceClient
{
    #region Constructors

    public ConamServiceClient(ProviderConam provider, TipoUrl tipoUrl) : base(provider, tipoUrl, SoapVersion.Soap11)
    {
    }

    #endregion Constructors

    #region Methods

    public string Enviar(string cabecalho, string dados)
    {
        var message = new StringBuilder();
        message.Append("<ws_nfe.PROCESSARPS>");
        message.Append("<Sdt_processarpsin xmlns=\"NFe\">");
        message.Append("<Login>");
        message.Append($"<CodigoUsuario>{Provider.Configuracoes.WebServices.Usuario}</CodigoUsuario>");
        message.Append($"<CodigoContribuinte>{Provider.Configuracoes.WebServices.Senha}</CodigoContribuinte>");
        message.Append("</Login>");
        message.Append(dados);
        message.Append("</Sdt_processarpsin>");
        message.Append("</ws_nfe.PROCESSARPS>");

        return Execute(message.ToString(), "ws_nfe.PROCESSARPSResponse");
    }

    public string EnviarSincrono(string cabecalho, string dados)
    {
        throw new NotImplementedException();
    }

    public string ConsultarSituacao(string cabecalho, string dados)
    {
        var message = new StringBuilder();
        message.Append("<ws_nfe.CONSULTAPROTOCOLO>");
        message.Append("<Sdt_consultaprotocoloin xmlns=\"NFe\">");
        message.Append(dados);
        message.Append("<Login>");
        message.Append($"<CodigoUsuario>{Provider.Configuracoes.WebServices.Usuario}</CodigoUsuario>");
        message.Append($"<CodigoContribuinte>{Provider.Configuracoes.WebServices.Senha}</CodigoContribuinte>");
        message.Append("</Login>");
        message.Append("</Sdt_consultaprotocoloin>");
        message.Append("</ws_nfe.CONSULTAPROTOCOLO>");

        return Execute(message.ToString(), "ws_nfe.CONSULTAPROTOCOLOResponse");
    }

    public string ConsultarLoteRps(string cabecalho, string dados)
    {
        var message = new StringBuilder();
        message.Append("<ws_nfe.CONSULTANOTASPROTOCOLO>");
        message.Append("<Sdt_consultanotasprotocoloin xmlns=\"NFe\">");
        message.Append(dados);
        message.Append("<Login>");
        message.Append($"<CodigoUsuario>{Provider.Configuracoes.WebServices.Usuario}</CodigoUsuario>");
        message.Append($"<CodigoContribuinte>{Provider.Configuracoes.WebServices.Senha}</CodigoContribuinte>");
        message.Append("</Login>");
        message.Append("</Sdt_consultanotasprotocoloin>");
        message.Append("</ws_nfe.CONSULTANOTASPROTOCOLO>");

        return Execute(message.ToString(), "ws_nfe.CONSULTANOTASPROTOCOLOResponse");
    }

    public string ConsultarSequencialRps(string cabec, string msg)
    {
        throw new System.NotImplementedException();
    }

    public string ConsultarNFSeRps(string cabecalho, string dados)
    {
        var xmlEnvio = new StringBuilder();
        xmlEnvio.Append("<SDT_IMPRESSAO_IN xmlns=\"NFe\">");
        xmlEnvio.Append("<Login>");
        xmlEnvio.Append($"<CodigoUsuario>{Provider.Configuracoes.WebServices.Usuario}</CodigoUsuario>");
        xmlEnvio.Append($"<CodigoContribuinte>{Provider.Configuracoes.WebServices.Senha}</CodigoContribuinte>");
        xmlEnvio.Append($"<Versao>2.00</Versao>");
        xmlEnvio.Append("</Login>");
        xmlEnvio.Append("<Nota>");
        xmlEnvio.Append(dados);
        xmlEnvio.Append("</Nota>");
        xmlEnvio.Append("</SDT_IMPRESSAO_IN>");

        var message = new StringBuilder();
        message.Append("<ws_nfe.IMPRESSAOLINKNFSE>");
        message.Append("<Xml_entrada>");
        message.AppendEnvio(xmlEnvio.ToString());
        message.Append("</Xml_entrada>");
        message.Append("</ws_nfe.IMPRESSAOLINKNFSE>");

        return Execute(message.ToString(), "ws_nfe.IMPRESSAOLINKNFSEResponse");
    }

    public string ConsultarNFSe(string cabecalho, string dados)
    {
        var xmlEnvio = new StringBuilder();
        xmlEnvio.Append("<SDT_IMPRESSAO_IN xmlns=\"NFe\">");
        xmlEnvio.Append("<Login>");
        xmlEnvio.Append($"<CodigoUsuario>{Provider.Configuracoes.WebServices.Usuario}</CodigoUsuario>");
        xmlEnvio.Append($"<CodigoContribuinte>{Provider.Configuracoes.WebServices.Senha}</CodigoContribuinte>");
        xmlEnvio.Append($"<Versao>2.00</Versao>");
        xmlEnvio.Append("</Login>");
        xmlEnvio.Append("<Nota>");
        xmlEnvio.Append(dados);
        xmlEnvio.Append("</Nota>");
        xmlEnvio.Append("</SDT_IMPRESSAO_IN>");

        var message = new StringBuilder();
        message.Append("<ws_nfe.IMPRESSAOLINKNFSE>");
        message.Append("<Xml_entrada>");
        message.AppendEnvio(xmlEnvio.ToString());
        message.Append("</Xml_entrada>");
        message.Append("</ws_nfe.IMPRESSAOLINKNFSE>");

        return Execute(message.ToString(), "ws_nfe.IMPRESSAOLINKNFSEResponse");
    }

    public string CancelarNFSe(string cabecalho, string dados)
    {
        var message = new StringBuilder();
        message.Append("<ws_nfe.CANCELANOTAELETRONICA>");
        message.Append("<Sdt_cancelanfe xmlns=\"NFe\">");
        message.Append("<Login>");
        message.Append($"<CodigoUsuario>{Provider.Configuracoes.WebServices.Usuario}</CodigoUsuario>");
        message.Append($"<CodigoContribuinte>{Provider.Configuracoes.WebServices.Senha}</CodigoContribuinte>");
        message.Append("</Login>");
        message.Append("<Nota>");
        message.Append(dados);
        message.Append("</Nota>");
        message.Append("</Sdt_cancelanfe>");
        message.Append("</ws_nfe.CANCELANOTAELETRONICA>");

        return Execute(message.ToString(), "ws_nfe.CANCELANOTAELETRONICAResponse");
    }

    public string CancelarNFSeLote(string cabec, string msg)
    {
        throw new System.NotImplementedException();
    }

    public string SubstituirNFSe(string cabec, string msg)
    {
        throw new System.NotImplementedException();
    }

    private string Execute(string message, string responseTag)
    {
        var result = ValidarUsernamePassword();
        if (!result) throw new DFe.Core.OpenDFeCommunicationException("Faltou informar username e/ou password");

        return Execute("", message, "", [responseTag], []);
    }

    private bool ValidarUsernamePassword()
    {
        return !string.IsNullOrEmpty(Provider.Configuracoes.WebServices.Usuario) &&
               !string.IsNullOrEmpty(Provider.Configuracoes.WebServices.Senha);
    }

    protected override string TratarRetorno(XElement xmlDocument, string[] responseTag)
    {
        return xmlDocument.ElementAnyNs(responseTag[0]).ToString();
    }

    protected override bool ValidarCertificadoServidor()
    {
        return false;
    }

    #endregion Methods
}