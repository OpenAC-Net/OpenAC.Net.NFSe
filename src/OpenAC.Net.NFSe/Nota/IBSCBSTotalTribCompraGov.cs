using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalTribCompraGov : GenericClone<IBSCBSTotalTribCompraGov>
{
    /// <summary>
    /// Aliquota do IBS de competencia do Estado.
    /// </summary>
    public decimal PercentualIBSUF { get; set; }

    /// <summary>
    /// Valor do Tributo do IBS da UF calculado.
    /// </summary>
    public decimal ValorIBSUF { get; set; }

    /// <summary>
    /// Aliquota do IBS de competencia do Municipio.
    /// </summary>
    public decimal PercentualIBSMun { get; set; }

    /// <summary>
    /// Valor do Tributo do IBS do Municipio calculado.
    /// </summary>
    public decimal ValorIBSMun { get; set; }

    /// <summary>
    /// Aliquota da CBS.
    /// </summary>
    public decimal PercentualCBS { get; set; }

    /// <summary>
    /// Valor do Tributo da CBS calculado.
    /// </summary>
    public decimal ValorCBS { get; set; }
}
