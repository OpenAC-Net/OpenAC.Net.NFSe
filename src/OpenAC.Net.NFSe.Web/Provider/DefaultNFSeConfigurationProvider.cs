using OpenAC.Net.NFSe.Web.Common;

namespace OpenAC.Net.NFSe.Web.Provider;

/// <summary>Fornece configurações para aplicações que operam com uma única empresa.</summary>
/// <typeparam name="TConfiguration">Tipo da configuração fornecida.</typeparam>
public sealed class DefaultNFSeConfigurationProvider<TConfiguration>
    : INFSeConfigurationProvider<TConfiguration>
{
    private readonly Func<TConfiguration> configurationFactory;

    /// <summary>Inicializa o provider com a função que cria cada configuração.</summary>
    /// <param name="configurationFactory">Função responsável por criar uma configuração independente.</param>
    public DefaultNFSeConfigurationProvider(Func<TConfiguration> configurationFactory)
    {
        this.configurationFactory = configurationFactory ??
            throw new ArgumentNullException(nameof(configurationFactory));
    }

    /// <inheritdoc />
    public ValueTask<TConfiguration> GetConfigurationAsync(string tenantId = NFSeTenant.Padrao,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(configurationFactory());
    }
}
