using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSDocumentoOutro : GenericClone<IBSCBSDocumentoOutro>
{
    public string? NumeroDocumento { get; set; }

    public string? DescricaoDocumento { get; set; }
}