using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalCIBS : GenericClone<IBSCBSTotalCIBS>
{
    public IBSCBSTotalCIBS()
    {
        IBS = new IBSCBSTotalIBS();
        CBS = new IBSCBSTotalCBS();
    }

    public decimal ValorTotalNF { get; set; }

    public IBSCBSTotalIBS IBS { get; set; }

    public IBSCBSTotalCBS CBS { get; set; }

    public IBSCBSTotalTribRegular? TributacaoRegular { get; set; }

    public IBSCBSTotalTribCompraGov? TributacaoCompraGov { get; set; }
}