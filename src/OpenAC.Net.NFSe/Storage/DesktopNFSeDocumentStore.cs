using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Storage;

/// <summary>Armazena documentos desktop usando os caminhos configurados e criação atômica.</summary>
public sealed class DesktopNFSeDocumentStore : INFSeDocumentStore
{
    /// <summary>Instância compartilhada.</summary>
    public static DesktopNFSeDocumentStore Instancia { get; } = new();

    /// <inheritdoc />
    public Task SaveAsync(NFSeDocument document, ConfigNFSe configuration, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var taxId = document.DocumentoContribuinte ?? "";
        var directory = document.Tipo switch
        {
            NFSeTipoDocumento.Rps => configuration.Arquivos.GetPathRps(document.Data, taxId),
            NFSeTipoDocumento.NFSe => configuration.Arquivos.GetPathNFSe(document.Data, taxId),
            NFSeTipoDocumento.SolicitacaoSoap or NFSeTipoDocumento.RespostaSoap => configuration.Arquivos.GetPathSoap(document.Data, taxId),
            _ => configuration.Arquivos.GetPathLote(document.Data, taxId)
        };
        return SaveToDirectoryAsync(document, directory, cancellationToken);
    }

    /// <summary>Grava em diretório explícito sem sobrescrever documentos existentes.</summary>
    public static async Task SaveToDirectoryAsync(NFSeDocument document, string directory,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Directory.CreateDirectory(directory);
        var filename = Path.GetFileName(document.NomeArquivo);
        var name = Path.GetFileNameWithoutExtension(filename);
        var extension = Path.GetExtension(filename);
        for (var index = 0; ; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var path = Path.Combine(directory, index == 0 ? filename : $"{name}_{index}{extension}");
            FileStream stream;
            try { stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read, 4096, true); }
            catch (IOException) when (!document.PreservarNomeArquivo && File.Exists(path)) { continue; }
            using (stream)
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
            {
                await writer.WriteAsync(document.Conteudo).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
            }
            return;
        }
    }
}
