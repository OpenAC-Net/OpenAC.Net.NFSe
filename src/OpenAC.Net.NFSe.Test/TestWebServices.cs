using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Web.Service;
using OpenAC.Net.NFSe.Storage;
using OpenAC.Net.NFSe.Web.Accessor;
using OpenAC.Net.NFSe.Web.Client;
using OpenAC.Net.NFSe.Web.Configuration;
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
    public async Task FactoryIsolatesTenantStateAndHttpHandlers()
    {
        var filter = new StubFilter();
        var services = Services();
        services.AddSingleton<IHttpMessageHandlerBuilderFilter>(filter);
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
        await SendAsync(provider, first.Configuracoes, HttpMethod.Get);
        await SendAsync(provider, second.Configuracoes, HttpMethod.Get);
        await SendAsync(provider, third.Configuracoes, HttpMethod.Get);
        Assert.Equal(2, filter.Names.Distinct().Count());
        Assert.All(filter.Names, name => { Assert.DoesNotContain("tenant-a", name); Assert.DoesNotContain("tenant-b", name); });
    }

    [Fact]
    public async Task CertificateRotationAndConnectionSettingsCreateIndependentHandlers()
    {
        var filter = new StubFilter();
        var services = Services(); services.AddSingleton<IHttpMessageHandlerBuilderFilter>(filter);
        services.AddOpenNFSeWeb(Configure);
        using var provider = services.BuildServiceProvider();
        using var client = await provider.GetRequiredService<IOpenNFSeClientFactory>().CreateAsync();
        await SendAsync(provider, client.Configuracoes, HttpMethod.Get);
        client.Configuracoes.Certificados.CertificadoBytes = CreateCertificate();
        await SendAsync(provider, client.Configuracoes, HttpMethod.Get);
        client.Configuracoes.WebServices.Proxy = "http://proxy.invalid:8080";
        await SendAsync(provider, client.Configuracoes, HttpMethod.Get);
        client.Configuracoes.WebServices.Protocolos = SecurityProtocolType.Tls12;
        await SendAsync(provider, client.Configuracoes, HttpMethod.Get);
        Assert.Equal(4, filter.Names.Distinct().Count());
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
        var filter = new StubFilter { StatusCode = HttpStatusCode.ServiceUnavailable };
        var services = Services(); services.AddSingleton<IHttpMessageHandlerBuilderFilter>(filter);
        services.AddOpenNFSeWeb(config => { Configure(config); config.WebServices.Tentativas = 2; config.WebServices.IntervaloTentativas = 0; });
        using var provider = services.BuildServiceProvider();
        using var client = await provider.GetRequiredService<IOpenNFSeClientFactory>().CreateAsync();
        await SendAsync(provider, client.Configuracoes, HttpMethod.Get);
        Assert.Equal(2, filter.Calls);
        await SendAsync(provider, client.Configuracoes, HttpMethod.Post);
        Assert.Equal(3, filter.Calls);
    }

    [Fact]
    public async Task WebOperationTimeoutCancelsUnderlyingRequest()
    {
        var services = Services();
        services.AddSingleton<INFSeHttpTransport>(new TestCommunication.CallbackTransport(async (_, token) =>
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
        services.AddSingleton<INFSeHttpTransport>(new TestCommunication.CallbackTransport((_, _) => throw new HttpRequestException("offline")));
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
                var store = provider.GetRequiredService<INFSeDocumentStore>();
                var document = new NFSeDocument(NFSeTipoDocumento.NFSe, "xml", "../../NFSe.xml", "", DateTime.Today);
                await store.SaveAsync(document, first.Configuracoes); await store.SaveAsync(document, second.Configuracoes);
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
    private static async Task SendAsync(IServiceProvider provider, ConfigNFSe config, HttpMethod method)
    {
        using var certificate = new X509Certificate2(config.Certificados.CertificadoBytes, (string?)null, X509KeyStorageFlags.EphemeralKeySet);
        using var request = new HttpRequestMessage(method, "https://example.invalid/nfse");
        if (method == HttpMethod.Post) request.Content = new StringContent("<xml/>");
        using var response = await provider.GetRequiredService<INFSeHttpTransport>().SendAsync(request, config, certificate, true);
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
    private sealed class StubFilter : IHttpMessageHandlerBuilderFilter
    {
        public ConcurrentBag<string> Names { get; } = [];
        public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;
        private int calls;
        public int Calls => calls;
        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next) => builder =>
        {
            next(builder); Names.Add(builder.Name!); builder.PrimaryHandler.Dispose(); builder.PrimaryHandler = new StubHandler(this);
        };
        private sealed class StubHandler(StubFilter owner) : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            { Interlocked.Increment(ref owner.calls); return Task.FromResult(new HttpResponseMessage(owner.StatusCode) { Content = new StringContent("test") }); }
        }
    }
}
