using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalCBS : GenericClone<IBSCBSTotalCBS>
{
    public decimal ValorDiferimento { get; set; }

    public decimal ValorCBS { get; set; }

    public IBSCBSTotalCBSCredPres? CreditoPresumido { get; set; }
}