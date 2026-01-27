using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalCBS : GenericClone<IBSCBSTotalCBS>
{
    /// <summary>
    /// Total do Diferimento CBS.
    /// </summary>
    /// <remarks>
    /// vDifCBS = vCBS x pDifCBS
    /// </remarks>
    public decimal ValorDiferimento { get; set; }

    /// <summary>
    /// Total valor da CBS da Uniao.
    /// </summary>
    /// <remarks>
    /// vCBS = vBC x (pCBS ou pAliqEfetCBS)
    /// </remarks>
    public decimal ValorCBS { get; set; }

    /// <summary>
    /// Grupo de valores referentes ao credito presumido para CBS.
    /// </summary>
    public IBSCBSTotalCBSCredPres? CreditoPresumido { get; set; }
}
