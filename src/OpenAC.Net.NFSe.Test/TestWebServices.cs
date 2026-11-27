using OpenAC.Net.DFe.Core.Storage;
using OpenAC.Net.DFe.Core.Http;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Web.Service;
using OpenAC.Net.NFSe.Storage;
using OpenAC.Net.NFSe.Web.Accessor;
using OpenAC.Net.NFSe.Web.Client;
using OpenAC.Net.NFSe.Web.Exception;
using OpenAC.Net.NFSe.Web.Provider;
using Xunit;

namespace OpenAC.Net.NFSe.Test;

public class TestWebServices
{
    private static readonly byte[] Certificate = CreateCertificate();

    [Fact]
    public void ScopedRegistrationUsesIndependentConfigurationsAndRefusesInteractiveCertificates()
    {
        var services = Services();
        services.AddOpenNFSeWeb(Configure);
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        Assert.IsType<DFeHttpTransport>(provider.GetRequiredService<IDFeHttpTransport>());
        using var first = provider.CreateScope();
        using var second = provider.CreateScope();
        var firstClient = first.ServiceProvider.GetRequiredService<IOpenNFSeClient>();
        var secondClient = second.ServiceProvider.GetRequiredService<IOpenNFSeClient>();
        Assert.Same(firstClient, first.ServiceProvider.GetRequiredService<IOpenNFSeClient>());
        Assert.NotSame(firstClient.Configuracoes, secondClient.Configuracoes);
        Assert.NotSame(firstClient.NotasServico, secondClient.NotasServico);
        var invalidServices = Services();
        invalidServices.AddOpenNFSeWeb(config => config.Certificados.Certificado = "serial-without-a-file");
        using var invalidProvider = invalidServices.BuildServiceProvider();
        using var scope = invalidProvider.CreateScope();
        Assert.Throws<InvalidOperationException>(() => scope.ServiceProvider.GetRequiredService<IOpenNFSeClient>());
    }

    [Fact]
    public async Task FactoryPassesIsolatedTenantPartitionsToTransport()
    {
        var transport = new RecordingTransport();
        var services = Services();
        services.AddSingleton<IDFeHttpTransport>(transport);
        services.AddOpenNFSeWebMultiTenant<TenantConfigurations, TenantCertificates>();
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        var factory = provider.GetRequiredService<IOpenNFSeClientFactory>();
        using var first = await factory.CreateAsync("tenant-a");
        using var second = await factory.CreateAsync("tenant-b");
        using var third = await factory.CreateAsync("tenant-a");
        Assert.NotSame(first.Configuracoes, third.Configuracoes);
        first.NotasServico.AddNew();
        Assert.Empty(second.NotasServico);
        var accessor = provider.GetRequiredService<INFSeTenantContextAccessor>();
        Assert.Equal("tenant-a", accessor.GetTenant(first.Configuracoes));
        Assert.Equal("tenant-b", accessor.GetTenant(second.Configuracoes));
        await SendAsync(first.Configuracoes, HttpMethod.Get);
        await SendAsync(second.Configuracoes, HttpMethod.Get);
        await SendAsync(third.Configuracoes, HttpMethod.Get);
        Assert.Equal(transport.PoolKeys[0], transport.PoolKeys[2]);
        Assert.NotEqual(transport.PoolKeys[0], transport.PoolKeys[1]);
    }

    [Fact]
    public async Task CertificateRotationAndConnectionSettingsChangeThePoolKey()
    {
        var transport = new RecordingTransport();
        var services = Services(); services.AddSingleton<IDFeHttpTransport>(transport);
        services.AddOpenNFSeWeb(Configure);
        using var provider = services.BuildServiceProvider();
        using var client = await provider.GetRequiredService<IOpenNFSeClientFactory>().CreateAsync();
        await SendAsync(client.Configuracoes, HttpMethod.Get);
        client.Configuracoes.Certificados.CertificadoBytes = CreateCertificate();
        await SendAsync(client.Configuracoes, HttpMethod.Get);
        client.Configuracoes.WebServices.Proxy = "http://proxy.invalid:8080";
        await SendAsync(client.Configuracoes, HttpMethod.Get);
        client.Configuracoes.WebServices.Protocolos = SecurityProtocolType.Tls12;
        await SendAsync(client.Configuracoes, HttpMethod.Get);
        Assert.Equal(4, transport.PoolKeys.Distinct().Count());
    }

    [Fact]
    public async Task FactoryRejectsExpiredCertificate()
    {
        using var key = RSA.Create(2048);
        var request = new CertificateRequest("CN=Expired NFSe", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow.AddDays(-1));
        var services = Services();
        services.AddOpenNFSeWeb(config => { Configure(config); config.Certificados.CertificadoBytes = certificate.Export(X509ContentType.Pfx); });
        using var provider = services.BuildServiceProvider();
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await provider.GetRequiredService<IOpenNFSeClientFactory>().CreateAsync());
    }

    [Fact]
    public async Task RetryIsLimitedToSafeHttpMethods()
    {
        var calls = 0;
        await using var endpoint = await StartEndpointAsync(context =>
        {
            Interlocked.Increment(ref calls);
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return Task.CompletedTask;
        });
        var services = Services();
        services.AddOpenNFSeWeb(config => { Configure(config); config.WebServices.Tentativas = 2; config.WebServices.IntervaloTentativas = 0; });
        using var provider = services.BuildServiceProvider();
        using var client = await provider.GetRequiredService<IOpenNFSeClientFactory>().CreateAsync();
        var address = endpoint.Urls.Single();
        await SendAsync(client.Configuracoes, HttpMethod.Get, address);
        Assert.Equal(2, Volatile.Read(ref calls));
        await SendAsync(client.Configuracoes, HttpMethod.Post, address);
        Assert.Equal(3, Volatile.Read(ref calls));
    }

    [Fact]
    public async Task WebOperationTimeoutCancelsUnderlyingRequest()
    {
        var services = Services();
        services.AddSingleton<IDFeHttpTransport>(new TestCommunication.CallbackTransport(async (_, token) =>
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, token); throw new InvalidOperationException();
        }));
        services.AddOpenNFSeWeb(Configure, options => options.TimeoutOperacao = TimeSpan.FromMilliseconds(100));
        using var provider = services.BuildServiceProvider();
        using var client = await provider.GetRequiredService<IOpenNFSeClientFactory>().CreateAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.ConsultaNFSeAsync(1).WaitAsync(TimeSpan.FromSeconds(10)));
    }

    [Fact]
    public async Task TechnicalTransportFailureIsNotReturnedAsFiscalRejection()
    {
        var services = Services();
        services.AddSingleton<IDFeHttpTransport>(new TestCommunication.CallbackTransport((_, _) => throw new HttpRequestException("offline")));
        services.AddOpenNFSeWeb(Configure);
        using var provider = services.BuildServiceProvider();
        using var client = await provider.GetRequiredService<IOpenNFSeClientFactory>().CreateAsync();
        var exception = await Assert.ThrowsAsync<NFSeTechnicalException>(() => client.ConsultaNFSeAsync(1));
        Assert.Equal(nameof(IOpenNFSeClient.ConsultaNFSeAsync), exception.Operacao);
    }

    [Fact]
    public async Task PersistenceIsOptInAndTenantDirectoriesAreSeparated()
    {
        var root = Path.Combine(Path.GetTempPath(), "nfse-web-" + Guid.NewGuid().ToString("N"));
        try
        {
            foreach (var enabled in new[] { false, true })
            {
                var services = Services(root);
                services.AddOpenNFSeWebMultiTenant<TenantConfigurations, TenantCertificates>(options => options.PersistenciaHabilitada = enabled);
                using var provider = services.BuildServiceProvider();
                var factory = provider.GetRequiredService<IOpenNFSeClientFactory>();
                using var first = await factory.CreateAsync("tenant-a"); using var second = await factory.CreateAsync("tenant-b");
                var store = provider.GetRequiredService<IDFeDocumentStore>();
                await store.SaveAsync(new NFSeDocument(first.Configuracoes, NFSeCategorias.NFSe, "xml", "../../NFSe.xml", "", DateTime.Today));
                await store.SaveAsync(new NFSeDocument(second.Configuracoes, NFSeCategorias.NFSe, "xml", "../../NFSe.xml", "", DateTime.Today));
                if (!enabled) Assert.False(Directory.Exists(root));
                else
                {
                    Assert.Equal(2, Directory.GetFiles(root, "*.xml", SearchOption.AllDirectories).Length);
                    Assert.Equal(2, Directory.GetDirectories(Path.Combine(root, "App_Data", "NFSe", "Tenants")).Length);
                }
            }
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    [Fact]
    public async Task PdfWebReturnsPdfAndHttpResultWithoutCreatingFiles()
    {
        var services = Services(); services.AddOpenDANFSeWeb(options => options.ForcarDownload = true);
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });
        var generator = provider.GetRequiredService<IOpenDANFSeWeb>();
        using var component = new OpenNFSe();
        var note = component.NotasServico.AddNew(); note.IdentificacaoNFSe.Numero = "123";
        note.IdentificacaoNFSe.DataEmissao = DateTime.Today; note.Prestador.RazaoSocial = "Empresa de teste";
        var document = await generator.GerarAsync(note);
        Assert.Equal("DANFSe-123.pdf", document.NomeArquivo);
        Assert.Equal("application/pdf", document.TipoConteudo);
        Assert.StartsWith("%PDF-", System.Text.Encoding.ASCII.GetString(document.Conteudo, 0, 8));
        var result = await generator.GerarResultadoAsync(note);
        var file = Assert.IsAssignableFrom<Microsoft.AspNetCore.Http.IFileHttpResult>(result);
        Assert.Equal(document.NomeArquivo, file.FileDownloadName);
    }

    [Fact]
    public async Task WebTimeoutIncludesResponseBodyRead()
    {
        await using var endpoint = await StartEndpointAsync(async context =>
        {
            await context.Response.StartAsync(context.RequestAborted);
            await Task.Delay(Timeout.InfiniteTimeSpan, context.RequestAborted);
        });
        var services = Services();
        services.AddOpenNFSeWeb(config =>
        {
            Configure(config);
            config.WebServices.TimeOut = TimeSpan.FromSeconds(1);
        });
        using var provider = services.BuildServiceProvider();
        using var client = await provider.GetRequiredService<IOpenNFSeClientFactory>().CreateAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            SendAsync(client.Configuracoes, HttpMethod.Get, endpoint.Urls.Single()));
    }

    private static async Task<WebApplication> StartEndpointAsync(RequestDelegate handler)
    {
        var endpoint = WebApplication.CreateSlimBuilder().Build();
        endpoint.Urls.Add("http://127.0.0.1:0");
        endpoint.Run(handler);
        await endpoint.StartAsync();
        return endpoint;
    }

    private static ServiceCollection Services(string? root = null)
    {
        var services = new ServiceCollection(); services.AddLogging();
        services.AddSingleton<IHostEnvironment>(new TestEnvironment { ContentRootPath = root ?? Path.GetTempPath() });
        return services;
    }
    private static void Configure(ConfigNFSe config)
    {
        config.WebServices.CodigoMunicipio = 3529005;
        config.PrestadorPadrao.CpfCnpj = "03514896000115";
        config.Certificados.CertificadoBytes = Certificate;
        config.Geral.Salvar = config.Arquivos.Salvar = config.WebServices.Salvar = false;
    }
    private static async Task SendAsync(ConfigNFSe config, HttpMethod method, string address = "https://example.invalid/nfse")
    {
        using var certificate = new X509Certificate2(config.Certificados.CertificadoBytes, (string?)null, X509KeyStorageFlags.EphemeralKeySet);
        using var request = new HttpRequestMessage(method, address);
        if (method == HttpMethod.Post) request.Content = new StringContent("<xml/>");
        using var response = await config.HttpTransport.SendAsync(request, new DFeHttpTransportOptions(config.WebServices, certificate));
    }
    private static byte[] CreateCertificate()
    {
        using var key = RSA.Create(2048);
        var request = new CertificateRequest("CN=NFSe Test", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddDays(2));
        return certificate.Export(X509ContentType.Pfx);
    }
    public sealed class TenantConfigurations : INFSeConfigurationProvider<ConfigNFSe>
    {
        public ValueTask<ConfigNFSe> GetConfigurationAsync(string tenantId = "default", CancellationToken cancellationToken = default)
        { cancellationToken.ThrowIfCancellationRequested(); var config = new ConfigNFSe(); Configure(config); return ValueTask.FromResult(config); }
    }
    public sealed class TenantCertificates : INFSeCertificateProvider
    {
        public ValueTask ConfigureAsync(string tenantId, ConfigCertificadosNFSe certificateConfiguration, CancellationToken cancellationToken = default)
        { cancellationToken.ThrowIfCancellationRequested(); certificateConfiguration.CertificadoBytes = Certificate; return ValueTask.CompletedTask; }
    }
    private sealed class TestEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Testing";
        public string ApplicationName { get; set; } = "NFSe.Test";
        public string ContentRootPath { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
    private sealed class RecordingTransport : IDFeHttpTransport
    {
        public List<string> PoolKeys { get; } = [];

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, DFeHttpTransportOptions options,
            CancellationToken cancellationToken = default)
        {
            PoolKeys.Add(options.GetPoolKey(request.RequestUri!));
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("test") });
        }
    }
}
