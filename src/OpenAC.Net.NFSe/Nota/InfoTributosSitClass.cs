using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class InfoTributosSitClass : GenericClone<InfoTributosSitClass>
{
    public string? CodigoSituacaoTributaria { get; set; }

    public string? CodigoClassificacaoTributaria { get; set; }
}