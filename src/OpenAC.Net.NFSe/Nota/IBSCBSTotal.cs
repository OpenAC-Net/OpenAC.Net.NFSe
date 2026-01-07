using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotal : GenericClone<IBSCBSTotal>
{
    public IBSCBSTotal()
    {
        Valores = new IBSCBSValores();
        Totalizadores = new IBSCBSTotalCIBS();
    }

    public string? DescricaoLocalidadeIncidencia { get; set; }

    public string? DescricaoClassificacaoTributaria { get; set; }

    public IBSCBSValores Valores { get; set; }

    public IBSCBSTotalCIBS Totalizadores { get; set; }
}