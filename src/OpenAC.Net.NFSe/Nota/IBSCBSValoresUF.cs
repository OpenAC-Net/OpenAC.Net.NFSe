using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSValoresUF : GenericClone<IBSCBSValoresUF>
{
    /// <summary>
    /// Aliquota da UF para IBS da localidade de incidencia parametrizada no sistema.
    /// </summary>
    public decimal PercentualIBSUF { get; set; }

    /// <summary>
    /// Percentual de reducao de aliquota estadual.
    /// </summary>
    public decimal PercentualReducaoAliquotaUF { get; set; }

    /// <summary>
    /// pAliqEfetUF = pIBSUF x (1 - pRedAliqUF) x (1 - pRedutor).
    /// Se pRedAliqUF nao for informado na DPS, entao pAliqEfetUF e a propria pIBSUF.
    /// </summary>
    public decimal PercentualAliquotaEfetivaUF { get; set; }
}
