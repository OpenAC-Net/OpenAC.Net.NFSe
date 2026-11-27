#if NETFRAMEWORK
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
#endif
using OpenAC.Net.DFe.Core.Http;

namespace OpenAC.Net.NFSe.Commom.Client;

internal sealed class DesktopNFSeHttpTransport : DFeHttpTransport
{
    internal static DesktopNFSeHttpTransport Instancia { get; } = new();

    protected override bool ManageRetries => false;

#if NETFRAMEWORK
    protected override async Task<HttpResponseMessage> SendCoreAsync(HttpRequestMessage request,
        DFeHttpTransportOptions options, CancellationToken cancellationToken)
    {
        try
        {
            return await base.SendCoreAsync(request, options, cancellationToken).ConfigureAwait(false);
        }
        catch (TargetInvocationException ex) when (ex.InnerException is PlatformNotSupportedException)
        {
            // A System.Net.Http 4.2.0.0 (NuGet) não suporta SslProtocols no handler.
            cancellationToken.ThrowIfCancellationRequested();
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)options.Protocols;

            using var handler = new HttpClientHandler
            {
                UseCookies = options.UseCookies,
                AllowAutoRedirect = options.AllowAutoRedirect,
                AutomaticDecompression = options.AutomaticDecompression
            };
            if (!options.ValidateServerCertificate)
                handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
            if (options.Certificate != null) handler.ClientCertificates.Add(options.Certificate);
            if (!string.IsNullOrWhiteSpace(options.Proxy)) handler.Proxy = new WebProxy(options.Proxy, true);

            using var client = new HttpClient(handler, disposeHandler: false) { Timeout = Timeout.InfiniteTimeSpan };
            return await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
                .ConfigureAwait(false);
        }
    }
#endif
}
