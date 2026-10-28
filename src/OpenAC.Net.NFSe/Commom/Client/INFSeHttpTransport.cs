using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Commom.Client;

/// <summary>Transporte substituível para todos os provedores HTTP, REST, SOAP e multipart.</summary>
public interface INFSeHttpTransport
{
    /// <summary>Envia uma requisição. O chamador é responsável pelo descarte da resposta.</summary>
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ConfigNFSe configuration,
        X509Certificate2? certificate, bool validateServerCertificate,
        CancellationToken cancellationToken = default);
}
