using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSTotalCIBS : GenericClone<IBSCBSTotalCIBS>
{
    public IBSCBSTotalCIBS()
    {
        IBS = new IBSCBSTotalIBS();
        CBS = new IBSCBSTotalCBS();
    }

    /// <summary>
    /// Valor total da NF considerando os impostos por fora: IBS e CBS.
    /// </summary>
    /// <remarks>
    /// O IBS e a CBS sao por fora, por isso seus valores devem ser adicionados ao valor total da NF.
    /// vTotNF = vLiq (em 2026)
    /// vTotNF = vLiq + vCBS + vIBSTot (a partir de 2027)
    /// </remarks>
    public decimal ValorTotalNF { get; set; }

    /// <summary>
    /// Grupo de valores referentes ao IBS.
    /// </summary>
    public IBSCBSTotalIBS IBS { get; set; }

    /// <summary>
    /// Grupo de valores referentes a CBS.
    /// </summary>
    public IBSCBSTotalCBS CBS { get; set; }

    /// <summary>
    /// Grupo de informacoes de tributacao regular.
    /// </summary>
    public IBSCBSTotalTribRegular? TributacaoRegular { get; set; }

    /// <summary>
    /// Grupo de informacoes da composicao do valor do IBS e da CBS em compras governamentais.
    /// </summary>
    public IBSCBSTotalTribCompraGov? TributacaoCompraGov { get; set; }
}
