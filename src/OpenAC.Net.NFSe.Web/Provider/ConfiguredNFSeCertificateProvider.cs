using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Web.Provider;

internal sealed class ConfiguredNFSeCertificateProvider : INFSeCertificateProvider
{
    public ValueTask ConfigureAsync(string tenantId, ConfigCertificadosNFSe certificateConfiguration,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.CompletedTask;
    }
}
