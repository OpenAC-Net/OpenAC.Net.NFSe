using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class InfoValoresIBSCBS : GenericClone<InfoValoresIBSCBS>
{
    public InfoValoresIBSCBS()
    {
        Tributos = new InfoTributosIBSCBS();
    }

    /// <summary>
    /// Grupo de informacoes relativas a valores incluidos neste documento e recebidos por motivo de operacoes de terceiros,
    /// objeto de reembolso, repasse ou ressarcimento pelo recebedor, ja tributados e aqui referenciados.
    /// </summary>
    public InfoReeRepRes? ReembolsoRepasseRessarcimento { get; set; }

    /// <summary>
    /// Grupo de informacoes relacionados aos tributos IBS e CBS.
    /// </summary>
    public InfoTributosIBSCBS Tributos { get; set; }

    /// <summary>
    /// Codigo IBGE da localidade de incidencia do IBS/CBS (local da operacao).
    /// </summary>
    public string? CodigoLocalidadeIncidencia { get; set; }

    /// <summary>
    /// Percentual de reducao de aliquota em compra governamental.
    /// </summary>
    public decimal PercentualRedutor { get; set; }

    /// <summary>
    /// Valor da base de calculo do ISSQN (R$) = Valor do Servico - Desconto Incondicionado - Deducoes/Reducoes - Beneficio Municipal.
    /// </summary>
    /// <remarks>
    /// vBC = vServ - descIncond - (vDR ou vCalcDR + vCalcReeRepRes) - (vRedBCBM ou vCalcBM)
    /// </remarks>
    public decimal ValorBaseCalculo { get; set; }
}
