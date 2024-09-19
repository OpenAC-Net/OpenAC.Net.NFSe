using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.DANFSe.QuestPdf;

public sealed class QuestPdfDANFSeOptions : DANFSeOptions<FiltroDFeReport>
{
    #region Constructors

    public QuestPdfDANFSeOptions(ConfigNFSe configuracoes) : base(configuracoes)
    {
    }

    #endregion Constructors
}