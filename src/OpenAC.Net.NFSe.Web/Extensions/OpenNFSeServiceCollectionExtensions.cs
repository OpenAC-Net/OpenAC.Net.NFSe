using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using OpenAC.Net.NFSe;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Storage;
using OpenAC.Net.NFSe.Web.Accessor;
using OpenAC.Net.NFSe.Web.Client;
using OpenAC.Net.NFSe.Web.Common;
using OpenAC.Net.NFSe.Web.Configuration;
using OpenAC.Net.NFSe.Web.Provider;
using OpenAC.Net.NFSe.Web.Store;
using OpenAC.Net.NFSe.Web.Transport;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Registra a NFS-e municipal para aplicações ASP.NET Core.</summary>
public static class OpenNFSeServiceCollectionExtensions
{
    /// <summary>Registra um cliente por escopo e uma fábrica, com configuração nova por cliente.</summary>
    public static IServiceCollection AddOpenNFSeWeb(this IServiceCollection services, Action<ConfigNFSe> configure,
        Action<OpenNFSeWebOptions>? configureInfrastructure = null)
    {
        ArgumentNullException.ThrowIfNull(configure);
        AddInfrastructure(services, configureInfrastructure);
        services.TryAddSingleton<INFSeConfigurationProvider<ConfigNFSe>>(new DefaultNFSeConfigurationProvider<ConfigNFSe>(() =>
        {
            var configuration = new ConfigNFSe();
            configure(configuration);
            return configuration;
        }));
        services.TryAddSingleton<INFSeCertificateProvider, ConfiguredNFSeCertificateProvider>();
        services.AddScoped<IOpenNFSeClient>(provider =>
        {
            var configuration = new ConfigNFSe();
            configure(configuration);
            OpenNFSeClientFactory.ValidateCertificate(configuration, provider.GetRequiredService<IOptions<OpenNFSeWebOptions>>().Value);
            provider.GetRequiredService<NFSeHttpClientRegistry>().Bind(configuration, NFSeTenant.Padrao);
            return ActivatorUtilities.CreateInstance<OpenNFSeWebClient>(provider, configuration);
        });
        return services;
    }

    /// <summary>Registra providers de configuração e certificado por empresa.</summary>
    public static IServiceCollection AddOpenNFSeWebMultiTenant<TConfigurationProvider, TCertificateProvider>(
        this IServiceCollection services, Action<OpenNFSeWebOptions>? configureInfrastructure = null)
        where TConfigurationProvider : class, INFSeConfigurationProvider<ConfigNFSe>
        where TCertificateProvider : class, INFSeCertificateProvider
    {
        AddInfrastructure(services, configureInfrastructure);
        services.TryAddSingleton<INFSeConfigurationProvider<ConfigNFSe>, TConfigurationProvider>();
        services.TryAddSingleton<INFSeCertificateProvider, TCertificateProvider>();
        return services;
    }

    private static void AddInfrastructure(IServiceCollection services, Action<OpenNFSeWebOptions>? configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddOptions<OpenNFSeWebOptions>().Configure(configure ?? (_ => { }))
            .Validate(options => options.TimeoutOperacao > TimeSpan.Zero && options.TimeoutTentativa > TimeSpan.Zero
                && options.TempoVidaConexaoPool > TimeSpan.Zero && options.TempoVidaHandler >= TimeSpan.FromSeconds(1)
                && options.MargemExpiracaoCertificado >= TimeSpan.Zero
                && options.TimeoutsPorOperacao.Values.All(timeout => timeout > TimeSpan.Zero), "Os tempos configurados são inválidos.")
            .ValidateOnStart();
        services.AddHttpClient();
        services.TryAddSingleton<NFSeHttpClientRegistry>();
        services.TryAddSingleton<INFSeTenantContextAccessor>(provider => provider.GetRequiredService<NFSeHttpClientRegistry>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<HttpClientFactoryOptions>, NFSeHttpClientOptions>());
        services.TryAddSingleton<INFSeHttpTransport, NFSeHttpTransport>();
        services.TryAddSingleton<INFSeDocumentStore, FileSystemNFSeDocumentStore>();
        services.TryAddSingleton<IOpenNFSeClientFactory, OpenNFSeClientFactory>();
    }
}
