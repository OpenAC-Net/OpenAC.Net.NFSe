using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Configuracao;

namespace OpenAC.Net.NFSe.DANFSe.PDFSharp.Web.Configuration;

/// <summary>Configura a geração e a resposta HTTP do DANFSe em aplicações ASP.NET Core.</summary>
public sealed class DANFSeWebOptions
{
    /// <summary>Obtém as configurações visuais e de segurança aplicadas ao PDF.</summary>
    public DANFSePDFSharpOptions Configuracoes { get; } = new();

    /// <summary>Obtém ou define a função responsável por gerar o nome do arquivo de uma NFS-e.</summary>
    public Func<NotaServico, string> NomeArquivo { get; set; } =
        nota => $"DANFSe-{nota.IdentificacaoNFSe.Numero}.pdf";

    /// <summary>Obtém ou define o nome usado quando várias NFS-e são reunidas no mesmo PDF.</summary>
    public string NomeArquivoLote { get; set; } = "DANFSe-Lote.pdf";

    /// <summary>Obtém ou define se o navegador deve sugerir download em vez de exibir o PDF.</summary>
    public bool ForcarDownload { get; set; }
}
