using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalIBS : GenericClone<IBSCBSTotalIBS>
{
    public IBSCBSTotalIBS()
    {
        TotalIBSUF = new IBSCBSTotalIBSUF();
        TotalIBSMun = new IBSCBSTotalIBSMun();
    }

    public decimal ValorIBSTotal { get; set; }

    public IBSCBSTotalIBSCredPres? CreditoPresumido { get; set; }

    public IBSCBSTotalIBSUF TotalIBSUF { get; set; }

    public IBSCBSTotalIBSMun TotalIBSMun { get; set; }
}