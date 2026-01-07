using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalTribRegular : GenericClone<IBSCBSTotalTribRegular>
{
    public decimal PercentualAliquotaEfetivaRegIBSUF { get; set; }

    public decimal ValorTributacaoRegIBSUF { get; set; }

    public decimal PercentualAliquotaEfetivaRegIBSMun { get; set; }

    public decimal ValorTributacaoRegIBSMun { get; set; }

    public decimal PercentualAliquotaEfetivaRegCBS { get; set; }

    public decimal ValorTributacaoRegCBS { get; set; }
}