using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using OpenAC.Net.DFe.Core.Http;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Web.Transport;

internal sealed class NFSeHttpTransport(
    IHttpClientFactory factory,
    NFSeHttpClientRegistry registry,
    ILogger<NFSeHttpTransport> logger) : DFeHttpTransportBase, INFSeHttpTransport
{
    private static readonly HttpRequestOptionsKey<string> Tenant = new("OpenAC.Net.NFSe.Tenant");

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ConfigNFSe configuration,
        X509Certificate2? certificate, bool validateServerCertificate, CancellationToken cancellationToken = default)
    {
        if (certificate != null) configuration.Certificados.ValidarCertificado(certificate);
        request.Options.Set(Tenant, registry.GetTenant(configuration));
        return base.SendAsync(request,
            new DFeHttpTransportOptions(configuration.WebServices, certificate, validateServerCertificate)
                { MargemValidadeCertificado = configuration.Certificados.MargemValidadeCertificado },
            cancellationToken);
    }

    protected override async Task<HttpResponseMessage> SendCoreAsync(HttpRequestMessage request,
        DFeHttpTransportOptions options,
        CancellationToken cancellationToken)
    {
        var endpoint = request.RequestUri ?? throw new InvalidOperationException("A requisição não possui endpoint.");
        logger.LogDebug("Enviando {Method} ao host {Host}", request.Method, endpoint.Authority);
        request.Options.TryGetValue(Tenant, out var tenant);
        var name = registry.Register(tenant ?? "default", options, endpoint);
        using var client = factory.CreateClient(name);
        return await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ConfigureAwait(false);
    }
}