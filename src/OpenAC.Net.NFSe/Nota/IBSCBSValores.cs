using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSValores : GenericClone<IBSCBSValores>
{
    public IBSCBSValores()
    {
        UF = new IBSCBSValoresUF();
        Municipio = new IBSCBSValoresMun();
        Federal = new IBSCBSValoresFed();
    }

    /// <summary>
    /// Valor monetario total relativo ao fornecimento proprio de bens materiais ou relacionados a operacoes de terceiros,
    /// objeto de reembolso, repasse ou ressarcimento pelo recebedor, ja tributados e aqui referenciados e que nao integram
    /// a base de calculo do ISSQN, do IBS e da CBS.
    /// </summary>
    public decimal ValorCalculadoReeRepRes { get; set; }

    /// <summary>
    /// Grupo de informacoes relativas aos valores do IBS Estadual.
    /// </summary>
    public IBSCBSValoresUF UF { get; set; }

    /// <summary>
    /// Grupo de informacoes relativas aos valores do IBS Municipal.
    /// </summary>
    public IBSCBSValoresMun Municipio { get; set; }

    /// <summary>
    /// Grupo de informacoes relativas aos valores da CBS.
    /// </summary>
    public IBSCBSValoresFed Federal { get; set; }
}
