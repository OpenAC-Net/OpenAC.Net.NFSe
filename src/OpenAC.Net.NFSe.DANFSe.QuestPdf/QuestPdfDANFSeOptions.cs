using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.DANFSe.QuestPdf;

/// <summary>
/// 
/// </summary>
public sealed class QuestPdfDANFSeOptions : DANFSeOptions<FiltroDFeReport>
{
    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuracoes"></param>
    public QuestPdfDANFSeOptions(ConfigNFSe configuracoes) : base(configuracoes)
    {
    }

    #endregion Constructors
}