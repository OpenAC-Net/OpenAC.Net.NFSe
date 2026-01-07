using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalIBSMun : GenericClone<IBSCBSTotalIBSMun>
{
    public decimal ValorDiferimento { get; set; }

    public decimal ValorIBSMun { get; set; }
}