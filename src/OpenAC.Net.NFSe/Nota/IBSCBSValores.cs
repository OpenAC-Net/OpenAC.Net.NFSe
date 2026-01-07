using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSValores : GenericClone<IBSCBSValores>
{
    public IBSCBSValores()
    {
        UF = new IBSCBSValoresUF();
        Municipio = new IBSCBSValoresMun();
        Federal = new IBSCBSValoresFed();
    }

    public decimal ValorCalculadoReeRepRes { get; set; }

    public IBSCBSValoresUF UF { get; set; }

    public IBSCBSValoresMun Municipio { get; set; }

    public IBSCBSValoresFed Federal { get; set; }
}