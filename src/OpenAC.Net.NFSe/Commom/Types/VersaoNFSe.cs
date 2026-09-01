using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Commom.Types;

/// <summary>
/// Versões do padrão de leiaute e schemas de NFSe suportadas.
/// </summary>
public enum VersaoNFSe
{
    /// <summary>
    /// Versão 1.00.
    /// </summary>
    [DFeEnum("1.00")]
    ve100,

    /// <summary>
    /// Versão 1.01.
    /// </summary>
    [DFeEnum("1.01")]
    ve101,

    /// <summary>
    /// Versão 1.02.
    /// </summary>
    [DFeEnum("1.02")]
    ve102,

    /// <summary>
    /// Versão 1.03.
    /// </summary>
    [DFeEnum("1.03")]
    ve103,

    /// <summary>
    /// Versão 2.00.
    /// </summary>
    [DFeEnum("2.00")]
    ve200,

    /// <summary>
    /// Versão 2.01.
    /// </summary>
    [DFeEnum("2.01")]
    ve201,

    /// <summary>
    /// Versão 2.02.
    /// </summary>
    [DFeEnum("2.02")]
    ve202,

    /// <summary>
    /// Versão 2.03.
    /// </summary>
    [DFeEnum("2.03")]
    ve203,

    /// <summary>
    /// Versão 2.04.
    /// </summary>
    [DFeEnum("2.04")]
    ve204
}
