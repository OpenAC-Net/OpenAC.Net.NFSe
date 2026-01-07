using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSValoresUF : GenericClone<IBSCBSValoresUF>
{
    public decimal PercentualIBSUF { get; set; }

    public decimal PercentualReducaoAliquotaUF { get; set; }

    public decimal PercentualAliquotaEfetivaUF { get; set; }
}