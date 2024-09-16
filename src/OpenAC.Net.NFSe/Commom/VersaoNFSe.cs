using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Commom;

public enum VersaoNFSe
{
    [DFeEnum("1.00")]
    ve100,

    [DFeEnum("1.01")]
    ve101,

    [DFeEnum("1.03")]
    ve103,

    [DFeEnum("2.00")]
    ve200,

    [DFeEnum("2.01")]
    ve201,

    [DFeEnum("2.02")]
    ve202,

    [DFeEnum("2.03")]
    ve203,

    [DFeEnum("2.04")]
    ve204
}