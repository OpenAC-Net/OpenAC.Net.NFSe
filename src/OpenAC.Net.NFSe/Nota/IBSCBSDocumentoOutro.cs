using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

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
