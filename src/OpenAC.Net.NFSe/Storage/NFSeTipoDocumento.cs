namespace OpenAC.Net.NFSe.Storage;

/// <summary>Classifica separadamente documentos fiscais e payloads técnicos.</summary>
public enum NFSeTipoDocumento
{
    /// <summary>Payload técnico enviado ao webservice.</summary>
    SolicitacaoTecnica,

    /// <summary>Payload técnico recebido do webservice.</summary>
    RespostaTecnica,

    /// <summary>Declaração de Prestação de Serviços enviada para emissão.</summary>
    Rps,

    /// <summary>Envelope SOAP enviado.</summary>
    SolicitacaoSoap,

    /// <summary>Envelope SOAP recebido.</summary>
    RespostaSoap,

    /// <summary>Nota Fiscal de Serviço eletrônica autorizada.</summary>
    NFSe
}
