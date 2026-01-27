using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSValoresFed : GenericClone<IBSCBSValoresFed>
{
    /// <summary>
    /// Aliquota da Uniao para CBS parametrizada no sistema.
    /// </summary>
    public decimal PercentualCBS { get; set; }

    /// <summary>
    /// Percentual de reducao de aliquota da CBS.
    /// </summary>
    public decimal PercentualReducaoAliquotaCBS { get; set; }

    /// <summary>
    /// pAliqEfetCBS = pCBS x (1 - pRedAliqCBS) x (1 - pRedutor).
    /// Se pRedAliqCBS nao for informado na DPS, entao pAliqEfetCBS e a propria pCBS.
    /// </summary>
    public decimal PercentualAliquotaEfetivaCBS { get; set; }
}
