using System;
using System.IO;
using OpenAC.Net.DFe.Core.Storage;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Storage;

/// <summary>Documento com caminho fiscal e metadados usados pelo host para isolar empresas.</summary>
public sealed class NFSeDocument : IDFeDocument
{
    /// <summary>Cria um documento preservando as regras de arquivos do componente.</summary>
    public NFSeDocument(ConfigNFSe configuracao, string categoria, string conteudo, string nomeArquivo,
        string? documentoContribuinte, DateTime data, DFeArquivoExiste arquivoExiste = DFeArquivoExiste.AdicionarSufixo)
    {
        Configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
        if (string.IsNullOrWhiteSpace(categoria) || categoria == "." || categoria == ".." ||
            categoria.IndexOfAny(['/', '\\', ':']) >= 0)
            throw new ArgumentException("Informe uma categoria válida, sem diretórios.", nameof(categoria));
        // O armazenamento anterior usava apenas o nome, descartando diretórios informados.
        NomeArquivo = Path.GetFileName(nomeArquivo);
        if (string.IsNullOrWhiteSpace(NomeArquivo) || NomeArquivo == "." || NomeArquivo == "..")
            throw new ArgumentException("Informe o nome do arquivo.", nameof(nomeArquivo));
        if (!Enum.IsDefined(typeof(DFeArquivoExiste), arquivoExiste))
            throw new ArgumentOutOfRangeException(nameof(arquivoExiste));
        Categoria = categoria;
        Conteudo = conteudo ?? throw new ArgumentNullException(nameof(conteudo));
        DocumentoContribuinte = documentoContribuinte;
        Data = data;
        ArquivoExiste = arquivoExiste;
    }
    /// <summary>Configuração associada à empresa e às regras de arquivos.</summary>
    public ConfigNFSe Configuracao { get; }
    /// <summary>Categoria fiscal ou técnica.</summary>
    public string Categoria { get; }
    /// <summary>Nome do arquivo, incluindo extensão.</summary>
    public string NomeArquivo { get; }
    /// <summary>CPF ou CNPJ usado nos diretórios.</summary>
    public string? DocumentoContribuinte { get; }
    /// <summary>Data de referência.</summary>
    public DateTime Data { get; }
    /// <inheritdoc />
    public string Caminho => Path.GetFullPath(Path.Combine(
        Configuracao.Arquivos.GetDocumentPath(Categoria, DocumentoContribuinte, Data), NomeArquivo));
    /// <inheritdoc />
    public string Conteudo { get; }
    /// <inheritdoc />
    public DFeArquivoExiste ArquivoExiste { get; }
}
