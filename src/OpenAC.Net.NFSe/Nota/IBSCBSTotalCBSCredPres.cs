using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalCBSCredPres : GenericClone<IBSCBSTotalCBSCredPres>
{
    public decimal PercentualCreditoPresumido { get; set; }

    public decimal ValorCreditoPresumido { get; set; }
}