using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class InfoValoresIBSCBS : GenericClone<InfoValoresIBSCBS>
{
    public InfoValoresIBSCBS()
    {
        Tributos = new InfoTributosIBSCBS();
    }

    public InfoReeRepRes? ReembolsoRepasseRessarcimento { get; set; }

    public InfoTributosIBSCBS Tributos { get; set; }

    public string? CodigoLocalidadeIncidencia { get; set; }

    public decimal PercentualRedutor { get; set; }

    public decimal ValorBaseCalculo { get; set; }
}