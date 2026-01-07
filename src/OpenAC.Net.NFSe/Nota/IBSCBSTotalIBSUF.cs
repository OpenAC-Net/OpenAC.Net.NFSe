using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalIBSUF : GenericClone<IBSCBSTotalIBSUF>
{
    /// <summary>
    /// Total do diferimento do IBS estadual.
    /// </summary>
    /// <remarks>
    /// vDifUF = vIBSUF x pDifUF
    /// </remarks>
    public decimal ValorDiferimento { get; set; }

    /// <summary>
    /// Total valor do IBS estadual.
    /// </summary>
    /// <remarks>
    /// vIBSUF = vBC x (pIBSUF ou pAliqEfetUF)
    /// </remarks>
    public decimal ValorIBSUF { get; set; }
}
