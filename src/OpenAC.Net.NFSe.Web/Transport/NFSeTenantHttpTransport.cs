using System.Net;
using OpenAC.Net.DFe.Core.Http;

namespace OpenAC.Net.NFSe.Web.Transport;

internal sealed class NFSeTenantHttpTransport(IDFeHttpTransport inner, string partition,
    TimeSpan certificateMargin) : IDFeHttpTransport
{
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, DFeHttpTransportOptions options,
        CancellationToken cancellationToken = default)
    {
        options.TenantId = partition;
        options.MargemValidadeCertificado = certificateMargin;
        options.UseCookies = false;
        options.AutomaticDecompression = DecompressionMethods.All;
        return inner.SendAsync(request, options, cancellationToken);
    }
}
