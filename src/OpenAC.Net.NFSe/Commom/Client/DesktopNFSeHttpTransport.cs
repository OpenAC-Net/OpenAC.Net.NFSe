using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.DFe.Core.Http;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Commom.Client;

/// <summary>Adapta o contrato dos provedores municipais ao transporte compartilhado do Core.</summary>
internal sealed class DesktopNFSeHttpTransport : INFSeHttpTransport
{
    internal static DesktopNFSeHttpTransport Instancia { get; } = new();

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ConfigNFSe configuration,
        X509Certificate2? certificate, bool validateServerCertificate, CancellationToken cancellationToken = default) =>
        configuration.HttpTransport.SendAsync(request,
            new DFeHttpTransportOptions(configuration.WebServices, certificate, validateServerCertificate)
                { MargemValidadeCertificado = configuration.Certificados.MargemValidadeCertificado },
            cancellationToken);
}