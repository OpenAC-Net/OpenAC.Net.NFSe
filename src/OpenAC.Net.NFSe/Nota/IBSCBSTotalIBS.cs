using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalIBS : GenericClone<IBSCBSTotalIBS>
{
    public IBSCBSTotalIBS()
    {
        TotalIBSUF = new IBSCBSTotalIBSUF();
        TotalIBSMun = new IBSCBSTotalIBSMun();
    }

    /// <summary>
    /// Valor total do IBS.
    /// </summary>
    /// <remarks>
    /// vIBSTot = vIBSUF + vIBSMun
    /// </remarks>
    public decimal ValorIBSTotal { get; set; }

    /// <summary>
    /// Grupo de valores referentes ao credito presumido para IBS.
    /// </summary>
    public IBSCBSTotalIBSCredPres? CreditoPresumido { get; set; }

    /// <summary>
    /// Grupo de valores referentes ao IBS Estadual.
    /// </summary>
    public IBSCBSTotalIBSUF TotalIBSUF { get; set; }

    /// <summary>
    /// Grupo de valores referentes ao IBS Municipal.
    /// </summary>
    public IBSCBSTotalIBSMun TotalIBSMun { get; set; }
}
