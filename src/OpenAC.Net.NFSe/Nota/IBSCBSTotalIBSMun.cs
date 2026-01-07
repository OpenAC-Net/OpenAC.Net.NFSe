using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalIBSMun : GenericClone<IBSCBSTotalIBSMun>
{
    /// <summary>
    /// Total do diferimento do IBS municipal.
    /// </summary>
    /// <remarks>
    /// vDifMun = vIBSMun x pDifMun
    /// </remarks>
    public decimal ValorDiferimento { get; set; }

    /// <summary>
    /// Total valor do IBS municipal.
    /// </summary>
    /// <remarks>
    /// vIBSMun = vBC x (pIBSMun ou pAliqEfetMun)
    /// </remarks>
    public decimal ValorIBSMun { get; set; }
}
