using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Storage;
using OpenAC.Net.NFSe.Web.Common;
using OpenAC.Net.NFSe.Web.Configuration;
using OpenAC.Net.NFSe.Web.Provider;
using OpenAC.Net.NFSe.Web.Transport;

namespace OpenAC.Net.NFSe.Web.Client;

internal sealed class OpenNFSeClientFactory(INFSeConfigurationProvider<ConfigNFSe> configurations,
    INFSeCertificateProvider certificates, INFSeHttpTransport transport, INFSeDocumentStore store,
    ILoggerFactory logs, IOptions<OpenNFSeWebOptions> options, NFSeHttpClientRegistry registry) : IOpenNFSeClientFactory
{
    public async ValueTask<IOpenNFSeClient> CreateAsync(string tenantId = NFSeTenant.Padrao,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        var configuration = await configurations.GetConfigurationAsync(tenantId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("O provider não retornou a configuração da empresa.");
        await certificates.ConfigureAsync(tenantId, configuration.Certificados, cancellationToken).ConfigureAwait(false);
        ValidateCertificate(configuration, options.Value);
        registry.Bind(configuration, tenantId);
        return new OpenNFSeWebClient(configuration, transport, store, logs.CreateLogger<OpenNFSeWebClient>(), options, tenantId);
    }

    internal static void ValidateCertificate(ConfigNFSe configuration, OpenNFSeWebOptions options)
    {
        using var certificate = NFSeWebCertificateResolver.Resolve(configuration.Certificados);
        if (!certificate.HasPrivateKey || certificate.NotBefore > DateTime.Now ||
            certificate.NotAfter <= DateTime.Now.Add(options.MargemExpiracaoCertificado))
            throw new InvalidOperationException("O certificado deve possuir chave privada e estar dentro da validade exigida.");
    }
}
