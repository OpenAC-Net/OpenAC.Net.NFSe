using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;/// <summary>
/// Informações consolidadas de tributos incidentes de IBS e CBS.
/// </summary>


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
