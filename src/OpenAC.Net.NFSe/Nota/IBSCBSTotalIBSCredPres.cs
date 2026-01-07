using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalIBSCredPres : GenericClone<IBSCBSTotalIBSCredPres>
{
    /// <summary>
    /// Aliquota do credito presumido para o IBS.
    /// </summary>
    public decimal PercentualCreditoPresumido { get; set; }

    /// <summary>
    /// Valor do Credito Presumido para o IBS.
    /// </summary>
    /// <remarks>
    /// vCredPresIBS = vBC x pCredPresIBS
    /// </remarks>
    public decimal ValorCreditoPresumido { get; set; }
}
