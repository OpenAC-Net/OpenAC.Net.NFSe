using System.Security.Cryptography;
using System.Text;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Storage;
using OpenAC.Net.NFSe.Web.Accessor;

namespace OpenAC.Net.NFSe.Web.Store;

/// <summary>Preserva os diretórios fiscais configurados e acrescenta a identidade da empresa.</summary>
public sealed class FileSystemNFSeDocumentStore(INFSeTenantContextAccessor tenants) : INFSeDocumentStore
{
    /// <inheritdoc />
    public Task SaveAsync(NFSeDocument document, ConfigNFSe configuration,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var fiscal = document.Tipo is NFSeTipoDocumento.NFSe or NFSeTipoDocumento.Rps;
        if (fiscal ? !configuration.Arquivos.Salvar : !configuration.WebServices.Salvar) return Task.CompletedTask;
        var tenant = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(tenants.GetTenant(configuration))));
        var source = configuration.Arquivos;
        var isolated = new ConfigNFSe();
        isolated.DocumentStore = configuration.DocumentStore;
        var files = isolated.Arquivos;
        files.PathSalvar = Scope(source.PathSalvar, source.PathSalvar, tenant);
        files.PathRps = Scope(source.PathRps, source.PathSalvar, tenant);
        files.PathNFSe = Scope(source.PathNFSe, source.PathSalvar, tenant);
        files.PathLote = Scope(source.PathLote, source.PathSalvar, tenant);
        files.AdicionarLiteral = source.AdicionarLiteral;
        files.SepararPorCNPJ = source.SepararPorCNPJ;
        files.SepararPorModelo = source.SepararPorModelo;
        files.SepararPorAno = source.SepararPorAno;
        files.SepararPorMes = source.SepararPorMes;
        files.SepararPorDia = source.SepararPorDia;
        lock (source.OrdenacaoPath) files.OrdenacaoPath.AddRange(source.OrdenacaoPath);
        return DesktopNFSeDocumentStore.Instancia.SaveAsync(document, isolated, cancellationToken);
    }

    private static string Scope(string path, string fallback, string tenant) =>
        Path.Combine(Path.GetFullPath(string.IsNullOrWhiteSpace(path) ? fallback : path), tenant);
}