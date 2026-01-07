using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSValoresFed : GenericClone<IBSCBSValoresFed>
{
    public decimal PercentualCBS { get; set; }

    public decimal PercentualReducaoAliquotaCBS { get; set; }

    public decimal PercentualAliquotaEfetivaCBS { get; set; }
}