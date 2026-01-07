using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSDocumentoFiscalOutro : GenericClone<IBSCBSDocumentoFiscalOutro>
{
    public string? CodigoMunicipioDocumentoFiscal { get; set; }

    public string? NumeroDocumentoFiscal { get; set; }

    public string? DescricaoDocumentoFiscal { get; set; }
}