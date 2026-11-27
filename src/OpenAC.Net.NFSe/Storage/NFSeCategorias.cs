namespace OpenAC.Net.NFSe.Storage;

/// <summary>Categorias usadas na organização histórica dos arquivos da NFS-e.</summary>
public static class NFSeCategorias
{
    /// <summary>Payload técnico enviado.</summary>
    public const string SolicitacaoTecnica = "SolicitacaoTecnica";

    /// <summary>Payload técnico recebido.</summary>
    public const string RespostaTecnica = "RespostaTecnica";

    /// <summary>RPS enviado para emissão.</summary>
    public const string Rps = "Rps";

    /// <summary>Envelope SOAP enviado.</summary>
    public const string SolicitacaoSoap = "SolicitacaoSoap";

    /// <summary>Envelope SOAP recebido.</summary>
    public const string RespostaSoap = "RespostaSoap";

    /// <summary>Nota fiscal autorizada.</summary>
    public const string NFSe = "NFSe";
}