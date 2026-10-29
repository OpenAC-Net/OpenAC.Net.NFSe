using OpenAC.Net.DFe.Core.Http;
using Microsoft.Extensions.Logging;
using OpenAC.Net.NFSe.Web.Common;

namespace OpenAC.Net.NFSe.Web.Transport;

internal sealed class NFSeHttpTransport(IHttpClientFactory factory, NFSeHttpClientRegistry registry,
    ILogger<NFSeHttpTransport> logger) : DFeHttpTransportBase
{
    internal static readonly HttpRequestOptionsKey<string> TenantKey = new("OpenAC.Net.NFSe.Tenant");
    protected override bool ManageRetries => false;

    protected override async Task<HttpResponseMessage> SendCoreAsync(HttpRequestMessage request,
        DFeHttpTransportOptions options, CancellationToken cancellationToken)
    {
        var tenant = request.Options.TryGetValue(TenantKey, out var identity) ? identity : NFSeTenant.Padrao;
        var endpoint = request.RequestUri ?? throw new InvalidOperationException("A requisição não possui endpoint.");
        logger.LogDebug("Enviando {Method} ao host {Host}", request.Method, endpoint.Authority);
        var name = registry.Register(tenant, options, endpoint);
        using var client = factory.CreateClient(name);
        return await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
    }
}

internal sealed class NFSeTenantHttpTransport(IDFeHttpTransport inner, string tenantId) : IDFeHttpTransport
{
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, DFeHttpTransportOptions options,
        CancellationToken cancellationToken = default)
    {
        request.Options.Set(NFSeHttpTransport.TenantKey, tenantId);
        return inner.SendAsync(request, options, cancellationToken);
    }
}
