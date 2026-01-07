using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSValoresMun : GenericClone<IBSCBSValoresMun>
{
    /// <summary>
    /// Aliquota do Municipio para IBS da localidade de incidencia parametrizada no sistema.
    /// </summary>
    public decimal PercentualIBSMun { get; set; }

    /// <summary>
    /// Percentual de reducao de aliquota municipal.
    /// </summary>
    public decimal PercentualReducaoAliquotaMun { get; set; }

    /// <summary>
    /// pAliqEfetMun = pIBSMun x (1 - pRedAliqMun) x (1 - pRedutor).
    /// Se pRedAliqMun nao for informado na DPS, entao pAliqEfetMun e a propria pIBSMun.
    /// </summary>
    public decimal PercentualAliquotaEfetivaMun { get; set; }
}
