using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Storage;
using OpenAC.Net.NFSe.Web.Accessor;
using OpenAC.Net.NFSe.Web.Configuration;

namespace OpenAC.Net.NFSe.Web.Store;

/// <summary>Persistência opcional relativa à raiz do host, isolada por empresa e categoria.</summary>
public sealed class FileSystemNFSeDocumentStore(IHostEnvironment environment,
    IOptions<OpenNFSeWebOptions> options, INFSeTenantContextAccessor tenants) : INFSeDocumentStore
{
    /// <inheritdoc />
    public Task SaveAsync(NFSeDocument document, ConfigNFSe configuration, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!options.Value.PersistenciaHabilitada) return Task.CompletedTask;
        var tenant = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(tenants.GetTenant(configuration))));
        var directory = Path.Combine(environment.ContentRootPath, options.Value.CaminhoDocumentos, "Tenants", tenant,
            document.Tipo.ToString(), document.Data.ToString("yyyyMM"));
        return DesktopNFSeDocumentStore.SaveToDirectoryAsync(document, directory, cancellationToken);
    }
}
