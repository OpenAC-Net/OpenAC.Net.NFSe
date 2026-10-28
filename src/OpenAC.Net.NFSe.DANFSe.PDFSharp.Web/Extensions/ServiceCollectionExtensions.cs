using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Web;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Web.Configuration;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Web.Service;
using OpenAC.Net.NFSe.Web;
using OpenAC.Net.NFSe.Web.Provider;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Extensões para registrar a geração Web do DANFSe no container de serviços.</summary>
public static class ServiceCollectionExtensions
{

    /// <summary>Registra o gerador Web do DANFSe com as configurações padrão.</summary>
    /// <returns>A própria coleção, para encadeamento das configurações.</returns>
    public static IServiceCollection AddOpenDANFSeWeb(this IServiceCollection services) =>
        services.AddOpenDANFSeWeb(_ => { });

    /// <summary>Registra e configura o gerador Web do DANFSe.</summary>
    /// <param name="services">Coleção de serviços da aplicação.</param>
    /// <param name="configure">Ação usada para configurar a geração e a resposta HTTP.</param>
    /// <returns>A própria coleção, para encadeamento das configurações.</returns>
    public static IServiceCollection AddOpenDANFSeWeb(this IServiceCollection services, Action<DANFSeWebOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);
        services.TryAddSingleton<INFSeConfigurationProvider<DANFSeWebOptions>>(
            new DefaultNFSeConfigurationProvider<DANFSeWebOptions>(() =>
            {
                var configuration = new DANFSeWebOptions();
                configure(configuration);
                return configuration;
            }));
        services.TryAddSingleton<IOpenDANFSeWeb, OpenDANFSeWeb>();
        return services;
    }

    /// <summary>Registra o gerador usando um provider de configuração multiempresa.</summary>
    /// <typeparam name="TConfigurationProvider">Provider das configurações de impressão por empresa.</typeparam>
    /// <returns>A própria coleção, para encadeamento das configurações.</returns>
    public static IServiceCollection AddOpenDANFSeWeb<TConfigurationProvider>(this IServiceCollection services)
        where TConfigurationProvider : class, INFSeConfigurationProvider<DANFSeWebOptions>
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddSingleton<INFSeConfigurationProvider<DANFSeWebOptions>,
            TConfigurationProvider>();
        services.TryAddSingleton<IOpenDANFSeWeb, OpenDANFSeWeb>();
        return services;
    }

}
