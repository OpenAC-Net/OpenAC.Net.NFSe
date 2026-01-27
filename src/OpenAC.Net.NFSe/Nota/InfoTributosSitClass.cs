using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class InfoTributosSitClass : GenericClone<InfoTributosSitClass>
{
    /// <summary>
    /// Codigo de Situacao Tributaria do IBS e da CBS.
    /// </summary>
    public string? CodigoSituacaoTributaria { get; set; }

    /// <summary>
    /// Codigo de Classificacao Tributaria do IBS e da CBS.
    /// </summary>
    public string? CodigoClassificacaoTributaria { get; set; }
}
