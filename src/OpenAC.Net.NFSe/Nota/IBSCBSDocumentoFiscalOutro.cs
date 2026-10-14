using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;/// <summary>
/// Outro documento fiscal de suporte à apuração de IBS e CBS.
/// </summary>


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
