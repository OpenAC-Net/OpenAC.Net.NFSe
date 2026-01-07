using System.Collections.Generic;
using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class InfoIBSCBS : GenericClone<InfoIBSCBS>
{
    public InfoIBSCBS()
    {
        ReferenciasNFSe = new List<string>();
        Valores = new InfoValoresIBSCBS();
    }

    public string? FinalidadeNFSe { get; set; }

    public string? IndicadorFinal { get; set; }

    public string? CodigoIndicadorOperacao { get; set; }

    public string? TipoOperacao { get; set; }

    public List<string> ReferenciasNFSe { get; }

    public string? TipoEnteGov { get; set; }

    public string? IndicadorDestinatario { get; set; }

    public InfoValoresIBSCBS Valores { get; set; }
}
