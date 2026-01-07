using System;
using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSDocumento : GenericClone<IBSCBSDocumento>
{
    public IBSCBSDocumentoDFe? DocumentoDFeNacional { get; set; }

    public IBSCBSDocumentoFiscalOutro? DocumentoFiscalOutro { get; set; }

    public IBSCBSDocumentoOutro? DocumentoOutro { get; set; }

    public IBSCBSDocumentoFornecedor? Fornecedor { get; set; }

    public DateTime DataEmissaoDocumento { get; set; }

    public DateTime DataCompetenciaDocumento { get; set; }

    public string? TipoReeRepRes { get; set; }

    public string? DescricaoTipoReeRepRes { get; set; }

    public decimal ValorReeRepRes { get; set; }
}