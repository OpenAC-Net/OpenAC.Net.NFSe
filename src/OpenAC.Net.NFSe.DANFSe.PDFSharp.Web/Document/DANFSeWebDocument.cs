namespace OpenAC.Net.NFSe.DANFSe.PDFSharp.Web.Document;

/// <summary>Representa um DANFSe pronto para ser devolvido por uma aplicação Web.</summary>
public sealed class DANFSeWebDocument
{
    /// <summary>Tipo MIME de documentos PDF.</summary>
    public const string TipoConteudoPdf = "application/pdf";

    /// <summary>Inicializa um documento PDF gerado.</summary>
    /// <param name="conteudo">Conteúdo binário do PDF.</param>
    /// <param name="nomeArquivo">Nome sugerido para o arquivo.</param>
    public DANFSeWebDocument(byte[] conteudo, string nomeArquivo)
    {
        Conteudo = conteudo ?? throw new ArgumentNullException(nameof(conteudo));
        NomeArquivo = string.IsNullOrWhiteSpace(nomeArquivo)
            ? throw new ArgumentException("O nome do arquivo deve ser informado.", nameof(nomeArquivo))
            : nomeArquivo;
    }

    /// <summary>Obtém o conteúdo binário do PDF.</summary>
    public byte[] Conteudo { get; }

    /// <summary>Obtém o nome sugerido para o arquivo.</summary>
    public string NomeArquivo { get; }

    /// <summary>Obtém o tipo MIME do documento.</summary>
    public string TipoConteudo => TipoConteudoPdf;

    /// <summary>Cria uma stream somente leitura posicionada no início do PDF.</summary>
    /// <returns>Stream que permite ler o conteúdo do documento.</returns>
    public Stream AbrirLeitura() => new MemoryStream(Conteudo, writable: false);
}
