using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Web.Transport;

internal sealed class NFSeHttpTransport(IHttpClientFactory factory, NFSeHttpClientRegistry registry,
    ILogger<NFSeHttpTransport> logger) : INFSeHttpTransport
{
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ConfigNFSe configuration,
        X509Certificate2? certificate, bool validateServerCertificate, CancellationToken cancellationToken = default)
    {
        var endpoint = request.RequestUri ?? throw new InvalidOperationException("A requisição não possui endpoint.");
        logger.LogDebug("Enviando {Method} ao host {Host}", request.Method, endpoint.Authority);
        var name = registry.Register(configuration, certificate, validateServerCertificate, endpoint);
        using var client = factory.CreateClient(name);
        return await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ConfigureAwait(false);
    }
}
