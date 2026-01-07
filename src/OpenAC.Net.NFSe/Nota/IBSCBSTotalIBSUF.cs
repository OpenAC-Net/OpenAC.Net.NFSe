using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalIBSUF : GenericClone<IBSCBSTotalIBSUF>
{
    public decimal ValorDiferimento { get; set; }

    public decimal ValorIBSUF { get; set; }
}