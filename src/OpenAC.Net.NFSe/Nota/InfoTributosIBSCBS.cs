using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class InfoTributosIBSCBS : GenericClone<InfoTributosIBSCBS>
{
    public InfoTributosIBSCBS()
    {
        SituacaoClassificacao = new InfoTributosSitClass();
    }

    /// <summary>
    /// Grupo de informacoes relacionadas ao IBS e a CBS.
    /// </summary>
    public InfoTributosSitClass SituacaoClassificacao { get; set; }
}
