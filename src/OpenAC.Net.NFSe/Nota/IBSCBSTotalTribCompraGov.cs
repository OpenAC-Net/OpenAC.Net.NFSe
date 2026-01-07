using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalTribCompraGov : GenericClone<IBSCBSTotalTribCompraGov>
{
    public decimal PercentualIBSUF { get; set; }

    public decimal ValorIBSUF { get; set; }

    public decimal PercentualIBSMun { get; set; }

    public decimal ValorIBSMun { get; set; }

    public decimal PercentualCBS { get; set; }

    public decimal ValorCBS { get; set; }
}