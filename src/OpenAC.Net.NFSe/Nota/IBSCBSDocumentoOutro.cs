using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;/// <summary>
/// Documentos diversos de referência para apuração de tributos IBS e CBS.
/// </summary>


public sealed class IBSCBSDocumentoOutro : GenericClone<IBSCBSDocumentoOutro>
{
    /// <summary>
    /// Numero do documento nao fiscal.
    /// </summary>
    public string? NumeroDocumento { get; set; }

    /// <summary>
    /// Descricao do documento nao fiscal.
    /// </summary>
    public string? DescricaoDocumento { get; set; }
}
