using System;

namespace OpenAC.Net.NFSe.Storage;

/// <summary>Documento produzido durante uma operação de NFS-e.</summary>
public sealed class NFSeDocument
{
    /// <summary>Inicializa um documento produzido durante uma operação de NFS-e.</summary>
    /// <param name="tipo">Classificação do payload ou documento fiscal.</param>
    /// <param name="conteudo">Conteúdo textual que será persistido.</param>
    /// <param name="nomeArquivo">Nome sugerido para o arquivo, incluindo sua extensão.</param>
    /// <param name="documentoContribuinte">CPF ou CNPJ usado na organização dos diretórios.</param>
    /// <param name="data">Data de referência usada na organização dos diretórios.</param>
    /// <param name="preservarNomeArquivo">Indica se uma colisão de nome deve impedir a criação de um arquivo com sufixo.</param>
    public NFSeDocument(NFSeTipoDocumento tipo, string conteudo, string nomeArquivo,
        string? documentoContribuinte, DateTime data, bool preservarNomeArquivo = false)
    {
        Tipo = tipo;
        Conteudo = conteudo;
        NomeArquivo = nomeArquivo;
        DocumentoContribuinte = documentoContribuinte;
        Data = data;
        PreservarNomeArquivo = preservarNomeArquivo;
    }

    /// <summary>Tipo do documento ou payload.</summary>
    public NFSeTipoDocumento Tipo { get; }

    /// <summary>Conteúdo que será armazenado.</summary>
    public string Conteudo { get; }

    /// <summary>Obtém o nome sugerido para o arquivo, incluindo sua extensão.</summary>
    public string NomeArquivo { get; }

    /// <summary>CPF ou CNPJ usado na organização dos arquivos.</summary>
    public string? DocumentoContribuinte { get; }

    /// <summary>Data usada na organização dos arquivos.</summary>
    public DateTime Data { get; }

    /// <summary>Obtém um valor que indica se o nome informado deve ser preservado em caso de colisão.</summary>
    public bool PreservarNomeArquivo { get; }
}
