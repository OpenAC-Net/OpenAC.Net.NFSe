using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSDocumentoFiscalOutro : GenericClone<IBSCBSDocumentoFiscalOutro>
{
    /// <summary>
    /// Codigo do municipio emissor do documento fiscal que nao se encontra no repositorio nacional.
    /// </summary>
    public string? CodigoMunicipioDocumentoFiscal { get; set; }

    /// <summary>
    /// Numero do documento fiscal que nao se encontra no repositorio nacional.
    /// </summary>
    public string? NumeroDocumentoFiscal { get; set; }

    /// <summary>
    /// Descricao do documento fiscal.
    /// </summary>
    public string? DescricaoDocumentoFiscal { get; set; }
}
