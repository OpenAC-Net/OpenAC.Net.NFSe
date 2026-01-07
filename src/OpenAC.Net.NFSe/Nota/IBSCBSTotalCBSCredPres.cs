using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalCBSCredPres : GenericClone<IBSCBSTotalCBSCredPres>
{
    /// <summary>
    /// Aliquota do credito presumido para a CBS.
    /// </summary>
    public decimal PercentualCreditoPresumido { get; set; }

    /// <summary>
    /// Valor do Credito Presumido da CBS.
    /// </summary>
    /// <remarks>
    /// vCredPresCBS = vBC x pCredPresCBS
    /// </remarks>
    public decimal ValorCreditoPresumido { get; set; }
}
