using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class InfoTributosIBSCBS : GenericClone<InfoTributosIBSCBS>
{
    public InfoTributosIBSCBS()
    {
        SituacaoClassificacao = new InfoTributosSitClass();
    }

    public InfoTributosSitClass SituacaoClassificacao { get; set; }
}