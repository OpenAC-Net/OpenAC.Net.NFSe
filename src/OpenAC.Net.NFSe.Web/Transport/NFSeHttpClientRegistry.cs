using OpenAC.Net.DFe.Core.Http;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Web.Accessor;
using OpenAC.Net.NFSe.Web.Common;

namespace OpenAC.Net.NFSe.Web.Transport;

internal sealed record NFSeHttpClientRegistration(
    X509Certificate2? Certificate,
    SslProtocols Protocols,
    string Proxy,
    bool ValidateServerCertificate,
    int Tentativas,
    TimeSpan IntervaloTentativas,
    DFeHttpTransportOptions Options);

internal sealed class NFSeHttpClientRegistry : INFSeTenantContextAccessor, IDisposable
{
    private readonly ConcurrentDictionary<string, NFSeHttpClientRegistration> registrations = new();
    private readonly ConditionalWeakTable<ConfigNFSe, TenantIdentity> tenants = new();

    public void Bind(ConfigNFSe configuration, string tenantId) =>
        tenants.AddOrUpdate(configuration, new TenantIdentity(tenantId));

    public string GetTenant(ConfigNFSe configuration) =>
        tenants.TryGetValue(configuration, out var tenant) ? tenant.Value : NFSeTenant.Padrao;

    public string Register(string tenant, DFeHttpTransportOptions options, Uri endpoint)
    {
        var identity = tenant + "|" + options.GetPoolKey(endpoint);
        var name = "OpenAC.Net.NFSe:" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity)));
        if (!registrations.ContainsKey(name))
        {
            var owned = options.Certificate == null ? null : new X509Certificate2(options.Certificate);
            if (!registrations.TryAdd(name,
                    new(owned, options.Protocols, options.Proxy, options.ValidateServerCertificate, options.Attempts,
                        options.RetryDelay, options)))
                owned?.Dispose();
        }

        return name;
    }

    public bool TryGet(string name, out NFSeHttpClientRegistration registration) =>
        registrations.TryGetValue(name, out registration!);

    public void Dispose()
    {
        foreach (var entry in registrations.Values) entry.Certificate?.Dispose();
        registrations.Clear();
    }

    private sealed record TenantIdentity(string Value);
}

internal sealed class NFSeHttpClientOptions(NFSeHttpClientRegistry registry)
    : IConfigureNamedOptions<HttpClientFactoryOptions>
{
    public void Configure(HttpClientFactoryOptions value)
    {
    }

    public void Configure(string? name, HttpClientFactoryOptions value)
    {
        if (name == null || !registry.TryGet(name, out var registration)) return;
        value.HandlerLifetime = registration.Options.TempoVidaHandler;
        value.HttpClientActions.Add(client => client.Timeout = Timeout.InfiniteTimeSpan);
        value.HttpMessageHandlerBuilderActions.Add(builder =>
        {
            var certificate = registration.Certificate == null ? null : new X509Certificate2(registration.Certificate);
            var ssl = new SslClientAuthenticationOptions { EnabledSslProtocols = registration.Protocols };
            if (certificate != null) ssl.ClientCertificates = new X509CertificateCollection { certificate };
            if (!registration.ValidateServerCertificate) ssl.RemoteCertificateValidationCallback = (_, _, _, _) => true;
            builder.PrimaryHandler = new CertificateHandler(certificate, new SocketsHttpHandler
            {
                UseCookies = registration.Options.UseCookies,
                AllowAutoRedirect = registration.Options.AllowAutoRedirect,
                AutomaticDecompression = registration.Options.AutomaticDecompression,
                PooledConnectionLifetime = registration.Options.TempoVidaConexaoPool,
                Proxy = string.IsNullOrWhiteSpace(registration.Proxy) ? null : new WebProxy(registration.Proxy, true),
                SslOptions = ssl
            });
            builder.AdditionalHandlers.Add(
                new ResilienceHandler(DFeHttpResilience.CreatePipeline(registration.Options)));
        });
    }

    private sealed class CertificateHandler(X509Certificate2? certificate, HttpMessageHandler inner)
        : DelegatingHandler(inner)
    {
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) certificate?.Dispose();
        }
    }
}