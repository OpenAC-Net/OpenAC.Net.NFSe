using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotal : GenericClone<IBSCBSTotal>
{
    public IBSCBSTotal()
    {
        Valores = new IBSCBSValores();
        Totalizadores = new IBSCBSTotalCIBS();
    }

    /// <summary>
    /// Nome da localidade de incidencia do IBS/CBS.
    /// </summary>
    public string? DescricaoLocalidadeIncidencia { get; set; }

    /// <summary>
    /// Descricao do Codigo de Classificacao Tributaria do IBS/CBS.
    /// </summary>
    public string? DescricaoClassificacaoTributaria { get; set; }

    /// <summary>
    /// Grupo de valores brutos referentes ao IBS/CBS.
    /// </summary>
    public IBSCBSValores Valores { get; set; }

    /// <summary>
    /// Grupo de totalizadores.
    /// </summary>
    public IBSCBSTotalCIBS Totalizadores { get; set; }
}
