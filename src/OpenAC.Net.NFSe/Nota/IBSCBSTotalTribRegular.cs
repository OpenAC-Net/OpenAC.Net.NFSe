using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalTribRegular : GenericClone<IBSCBSTotalTribRegular>
{
    /// <summary>
    /// Aliquota efetiva de tributacao regular do IBS estadual.
    /// </summary>
    public decimal PercentualAliquotaEfetivaRegIBSUF { get; set; }

    /// <summary>
    /// Valor da tributacao regular do IBS estadual.
    /// </summary>
    /// <remarks>
    /// vTribRegIBSUF = vBC x pAliqEfeRegIBSUF
    /// </remarks>
    public decimal ValorTributacaoRegIBSUF { get; set; }

    /// <summary>
    /// Aliquota efetiva de tributacao regular do IBS municipal.
    /// </summary>
    public decimal PercentualAliquotaEfetivaRegIBSMun { get; set; }

    /// <summary>
    /// Valor da tributacao regular do IBS municipal.
    /// </summary>
    /// <remarks>
    /// vTribRegIBSMun = vBC x pAliqEfeRegIBSMun
    /// </remarks>
    public decimal ValorTributacaoRegIBSMun { get; set; }

    /// <summary>
    /// Aliquota efetiva de tributacao regular da CBS.
    /// </summary>
    public decimal PercentualAliquotaEfetivaRegCBS { get; set; }

    /// <summary>
    /// Valor da tributacao regular da CBS.
    /// </summary>
    /// <remarks>
    /// vTribRegCBS = vBC x pAliqEfeRegCBS
    /// </remarks>
    public decimal ValorTributacaoRegCBS { get; set; }
}
