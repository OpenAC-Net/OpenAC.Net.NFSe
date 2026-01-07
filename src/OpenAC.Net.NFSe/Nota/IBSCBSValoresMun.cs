using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSValoresMun : GenericClone<IBSCBSValoresMun>
{
    public decimal PercentualIBSMun { get; set; }

    public decimal PercentualReducaoAliquotaMun { get; set; }

    public decimal PercentualAliquotaEfetivaMun { get; set; }
}