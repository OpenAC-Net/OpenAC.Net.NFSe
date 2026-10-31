using System.IO;
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
    public Task SaveAsync(NFSeDocument document, ConfigNFSe configuration,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var taxId = document.DocumentoContribuinte ?? "";
        var directory = document.Tipo switch
        {
            NFSeTipoDocumento.Rps => configuration.Arquivos.GetPathRps(document.Data, taxId),
            NFSeTipoDocumento.NFSe => configuration.Arquivos.GetPathNFSe(document.Data, taxId),
            NFSeTipoDocumento.SolicitacaoSoap or NFSeTipoDocumento.RespostaSoap => configuration.Arquivos.GetPathSoap(
                document.Data, taxId),
            _ => configuration.Arquivos.GetPathLote(document.Data, taxId)
        };
        return SaveToDirectoryAsync(document, directory, cancellationToken);
    }

    /// <summary>Grava em diretório explícito sem sobrescrever documentos existentes.</summary>
    public static Task SaveToDirectoryAsync(NFSeDocument document, string directory,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var policy = document.PreservarNomeArquivo
            ? OpenAC.Net.DFe.Core.Storage.DFeArquivoExiste.LancarErro
            : OpenAC.Net.DFe.Core.Storage.DFeArquivoExiste.AdicionarSufixo;
        var destination = new OpenAC.Net.DFe.Core.Storage.DFeDocument(
            Path.Combine(directory, Path.GetFileName(document.NomeArquivo)), document.Conteudo, policy);
        return OpenAC.Net.DFe.Core.Storage.DFeFileSystemDocumentStore.Instance.SaveAsync(destination,
            cancellationToken);
    }
}