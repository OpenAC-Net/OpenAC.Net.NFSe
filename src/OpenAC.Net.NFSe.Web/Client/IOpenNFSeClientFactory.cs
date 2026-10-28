using OpenAC.Net.NFSe.Web.Common;

namespace OpenAC.Net.NFSe.Web.Client;

/// <summary>Cria clientes independentes por empresa. O consumidor deve descartá-los após o uso.</summary>
public interface IOpenNFSeClientFactory
{
    /// <summary>Obtém configuração e certificado vigentes para a empresa informada.</summary>
    ValueTask<IOpenNFSeClient> CreateAsync(string tenantId = NFSeTenant.Padrao,
        CancellationToken cancellationToken = default);
}
