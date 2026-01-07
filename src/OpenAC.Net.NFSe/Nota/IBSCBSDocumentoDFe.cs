using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSDocumentoDFe : GenericClone<IBSCBSDocumentoDFe>
{
    public string? TipoChaveDFe { get; set; }

    public string? DescricaoTipoChaveDFe { get; set; }

    public string? ChaveDFe { get; set; }
}