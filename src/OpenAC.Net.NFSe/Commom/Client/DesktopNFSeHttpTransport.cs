using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Commom.Client;

/// <summary>Reutiliza conexões desktop por certificado, TLS, proxy e validação do servidor.</summary>
internal sealed class DesktopNFSeHttpTransport : INFSeHttpTransport
{
    internal static DesktopNFSeHttpTransport Instancia { get; } = new();
    private readonly object sync = new();
    private readonly Dictionary<string, Entry> clients = new();

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ConfigNFSe configuration,
        X509Certificate2? certificate, bool validateServerCertificate, CancellationToken cancellationToken = default)
    {
        var protocols = (SslProtocols)configuration.WebServices.Protocolos;
        var proxy = configuration.WebServices.Proxy ?? "";
        var key = $"{certificate?.Thumbprint}:{(int)protocols}:{validateServerCertificate}:{proxy}";
        Entry entry;
        lock (sync)
        {
            var expired = new List<string>();
            foreach (var pair in clients)
            {
                if (pair.Value.Expires > DateTime.UtcNow) continue;
                pair.Value.Retired = true;
                if (pair.Value.Active == 0) pair.Value.Dispose();
                expired.Add(pair.Key);
            }
            foreach (var name in expired) clients.Remove(name);
            if (!clients.TryGetValue(key, out entry!))
            {
                var ownedCertificate = certificate == null ? null : new X509Certificate2(certificate);
                var handler = new HttpClientHandler { UseCookies = false };
#if NET6_0_OR_GREATER || NETSTANDARD2_0
                handler.SslProtocols = protocols;
#elif NETFRAMEWORK
                // O setter não existe nas referências antigas, mas está disponível em runtimes recentes.
                typeof(HttpClientHandler).GetProperty("SslProtocols")?.SetValue(handler, protocols);
#endif
                if (!validateServerCertificate)
                    handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                if (ownedCertificate != null) handler.ClientCertificates.Add(ownedCertificate);
                if (!string.IsNullOrWhiteSpace(proxy)) handler.Proxy = new WebProxy(proxy, true);
                entry = new Entry(new HttpClient(handler) { Timeout = Timeout.InfiniteTimeSpan }, ownedCertificate);
                clients.Add(key, entry);
            }
            entry.Active++;
        }
        try
        {
            // Bufferiza o corpo antes de devolver o empréstimo ao pool e encerrar o timeout.
            return await entry.Client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            lock (sync)
            {
                entry.Active--;
                if (entry.Retired && entry.Active == 0) entry.Dispose();
            }
        }
    }

    private sealed class Entry : IDisposable
    {
        internal Entry(HttpClient client, X509Certificate2? certificate)
        {
            Client = client;
            Certificate = certificate;
            Expires = DateTime.UtcNow.AddMinutes(10);
            if (certificate != null && certificate.NotAfter.ToUniversalTime() < Expires)
                Expires = certificate.NotAfter.ToUniversalTime();
        }
        internal HttpClient Client { get; }
        private X509Certificate2? Certificate { get; }
        internal DateTime Expires { get; }
        internal int Active { get; set; }
        internal bool Retired { get; set; }
        public void Dispose() { Client.Dispose(); Certificate?.Dispose(); }
    }
}
