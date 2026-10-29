using OpenAC.Net.DFe.Core.Storage;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenAC.Net.NFSe.Storage;
using OpenAC.Net.NFSe.Web.Accessor;
using OpenAC.Net.NFSe.Web.Configuration;

namespace OpenAC.Net.NFSe.Web.Store;

/// <summary>Persistência opcional relativa à raiz do host, isolada por empresa e categoria.</summary>
public sealed class FileSystemNFSeDocumentStore(
    IHostEnvironment environment,
    IOptions<OpenNFSeWebOptions> options,
    INFSeTenantContextAccessor tenants) : IDFeDocumentStore
{
    /// <inheritdoc />
    public Task SaveAsync(IDFeDocument document, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!options.Value.PersistenciaHabilitada) return Task.CompletedTask;
        if (document is not NFSeDocument nfse)
            throw new ArgumentException("O armazenamento web requer os metadados de NFSeDocument.", nameof(document));
        var tenant = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(tenants.GetTenant(nfse.Configuracao))));
        var directory = Path.Combine(environment.ContentRootPath, options.Value.CaminhoDocumentos, "Tenants", tenant,
            nfse.Categoria, nfse.Data.ToString("yyyyMM"));
        return DFeFileSystemDocumentStore.Instance.SaveAsync(new DFeDocument(
                Path.GetFullPath(Path.Combine(directory, nfse.NomeArquivo)), nfse.Conteudo, nfse.ArquivoExiste),
            cancellationToken);
    }
}