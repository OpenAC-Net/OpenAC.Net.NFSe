using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalIBSCredPres : GenericClone<IBSCBSTotalIBSCredPres>
{
    public decimal PercentualCreditoPresumido { get; set; }

    public decimal ValorCreditoPresumido { get; set; }
}