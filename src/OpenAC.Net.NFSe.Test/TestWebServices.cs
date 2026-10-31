using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileProviders;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Web.Service;
using OpenAC.Net.NFSe.Storage;
using OpenAC.Net.NFSe.Web.Accessor;
using OpenAC.Net.NFSe.Web.Client;
using OpenAC.Net.NFSe.Web.Exception;
using OpenAC.Net.NFSe.Web.Provider;

namespace OpenAC.Net.NFSe.Test;

public class TestWebServices
{
    private static readonly byte[] Certificate = CreateCertificate();

    [Test]
    public async Task ScopedRegistrationUsesIndependentConfigurationsAndRefusesInteractiveCertificates()
    {
        var services = Services();
        services.AddOpenNFSeWeb(Configure);
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        using var first = provider.CreateScope();
        using var second = provider.CreateScope();
        var firstClient = first.ServiceProvider.GetRequiredService<IOpenNFSeClient>();
        var secondClient = second.ServiceProvider.GetRequiredService<IOpenNFSeClient>();
        await Assert.That(first.ServiceProvider.GetRequiredService<IOpenNFSeClient>()).IsSameReferenceAs(firstClient);
        await Assert.That(secondClient.Configuracoes).IsNotSameReferenceAs(firstClient.Configuracoes);
        await Assert.That(secondClient.NotasServico).IsNotSameReferenceAs(firstClient.NotasServico);
        var invalidServices = Services();
        invalidServices.AddOpenNFSeWeb(config => config.Certificados.Certificado = "serial-without-a-file");
        using var invalidProvider = invalidServices.BuildServiceProvider();
        using var scope = invalidProvider.CreateScope();
        await Assert.That(() => scope.ServiceProvider.GetRequiredService<IOpenNFSeClient>()).ThrowsExactly<InvalidOperationException>();
    }

    [Test]
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
        await Assert.That(third.Configuracoes).IsNotSameReferenceAs(first.Configuracoes);
        first.NotasServico.AddNew();
        await Assert.That(second.NotasServico).IsEmpty();
        var accessor = provider.GetRequiredService<INFSeTenantContextAccessor>();
        await Assert.That(accessor.GetTenant(first.Configuracoes)).IsEqualTo("tenant-a");
        await Assert.That(accessor.GetTenant(second.Configuracoes)).IsEqualTo("tenant-b");
        await SendAsync(provider, first.Configuracoes, HttpMethod.Get);
        await SendAsync(provider, second.Configuracoes, HttpMethod.Get);
        await SendAsync(provider, third.Configuracoes, HttpMethod.Get);
        await Assert.That(filter.Names.Distinct().Count()).IsEqualTo(2);
        foreach (var name in filter.Names)
        {
            await Assert.That(name).DoesNotContain("tenant-a");
            await Assert.That(name).DoesNotContain("tenant-b");
        }
    }

    [Test]
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
        client.Configuracoes.WebServices.TempoVidaHandler = TimeSpan.FromMinutes(7);
        client.Configuracoes.WebServices.TempoVidaConexaoPool = TimeSpan.FromMinutes(3);
        await SendAsync(provider, client.Configuracoes, HttpMethod.Get);
        await Assert.That(filter.Names.Distinct().Count()).IsEqualTo(5);
        var options = provider.GetRequiredService<IOptionsMonitor<HttpClientFactoryOptions>>();
        await Assert.That(filter.Names.Select(name => options.Get(name).HandlerLifetime)).Contains(TimeSpan.FromMinutes(7));
    }

    [Test]
    public async Task FactoryRejectsExpiredCertificate()
    {
        using var key = RSA.Create(2048);
        var request = new CertificateRequest("CN=Expired NFSe", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow.AddDays(-1));
        var services = Services();
        services.AddOpenNFSeWeb(config => { Configure(config); config.Certificados.CertificadoBytes = certificate.Export(X509ContentType.Pfx); });
        using var provider = services.BuildServiceProvider();
        await Assert.That(async () => await provider.GetRequiredService<IOpenNFSeClientFactory>().CreateAsync()).ThrowsExactly<InvalidOperationException>();
    }

    [Test]
    public async Task RetryIsLimitedToSafeHttpMethods()
    {
        var filter = new StubFilter { StatusCode = HttpStatusCode.ServiceUnavailable };
        var services = Services(); services.AddSingleton<IHttpMessageHandlerBuilderFilter>(filter);
        services.AddOpenNFSeWeb(config => { Configure(config); config.WebServices.Tentativas = 2; config.WebServices.IntervaloTentativas = 0; });
        using var provider = services.BuildServiceProvider();
        using var client = await provider.GetRequiredService<IOpenNFSeClientFactory>().CreateAsync();
        await SendAsync(provider, client.Configuracoes, HttpMethod.Get);
        await Assert.That(filter.Calls).IsEqualTo(2);
        await SendAsync(provider, client.Configuracoes, HttpMethod.Post);
        await Assert.That(filter.Calls).IsEqualTo(3);
    }

    [Test]
    public async Task WebOperationTimeoutCancelsUnderlyingRequest()
    {
        var services = Services();
        services.AddSingleton<INFSeHttpTransport>(new TestCommunication.CallbackTransport(async (_, token) =>
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, token); throw new InvalidOperationException();
        }));
        services.AddOpenNFSeWeb(config => { Configure(config); config.WebServices.TimeoutOperacao = TimeSpan.FromMilliseconds(100); });
        using var provider = services.BuildServiceProvider();
        using var client = await provider.GetRequiredService<IOpenNFSeClientFactory>().CreateAsync();
        await Assert.That(async () => { await client.ConsultaNFSeAsync(1).WaitAsync(TimeSpan.FromSeconds(10)); }).Throws<OperationCanceledException>();
    }

    [Test]
    public async Task TechnicalTransportFailureIsNotReturnedAsFiscalRejection()
    {
        var services = Services();
        services.AddSingleton<INFSeHttpTransport>(new TestCommunication.CallbackTransport((_, _) => throw new HttpRequestException("offline")));
        services.AddOpenNFSeWeb(Configure);
        using var provider = services.BuildServiceProvider();
        using var client = await provider.GetRequiredService<IOpenNFSeClientFactory>().CreateAsync();
        var exception = await Assert.That(async () => { await client.ConsultaNFSeAsync(1); }).ThrowsExactly<NFSeTechnicalException>();
        await Assert.That(exception!.Operacao).IsEqualTo(nameof(IOpenNFSeClient.ConsultaNFSeAsync));
    }

    [Test]
    public async Task PersistenceIsOptInAndTenantDirectoriesAreSeparated()
    {
        var root = Path.Combine(Path.GetTempPath(), "nfse-web-" + Guid.NewGuid().ToString("N"));
        try
        {
            foreach (var enabled in new[] { false, true })
            {
                var services = Services(root);
                services.AddSingleton<INFSeConfigurationProvider<ConfigNFSe>>(new TenantConfigurations(config =>
                {
                    config.Arquivos.Salvar = enabled;
                    config.Arquivos.PathNFSe = root;
                }));
                services.AddOpenNFSeWebMultiTenant<TenantConfigurations, TenantCertificates>();
                using var provider = services.BuildServiceProvider();
                var factory = provider.GetRequiredService<IOpenNFSeClientFactory>();
                using var first = await factory.CreateAsync("tenant-a"); using var second = await factory.CreateAsync("tenant-b");
                var store = provider.GetRequiredService<INFSeDocumentStore>();
                var document = new NFSeDocument(NFSeTipoDocumento.NFSe, "xml", "../../NFSe.xml", "", DateTime.Today);
                await store.SaveAsync(document, first.Configuracoes); await store.SaveAsync(document, second.Configuracoes);
                if (!enabled) await Assert.That(Directory.Exists(root)).IsFalse();
                else
                {
                    await Assert.That(Directory.GetFiles(root, "*.xml", SearchOption.AllDirectories).Length).IsEqualTo(2);
                    await Assert.That(Directory.GetDirectories(root).Length).IsEqualTo(2);
                }
            }
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    [Test]
    public async Task PdfWebReturnsPdfAndHttpResultWithoutCreatingFiles()
    {
        var services = Services(); services.AddOpenDANFSeWeb(options => options.ForcarDownload = true);
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });
        var generator = provider.GetRequiredService<IOpenDANFSeWeb>();
        using var component = new OpenNFSe();
        var note = component.NotasServico.AddNew(); note.IdentificacaoNFSe.Numero = "123";
        note.IdentificacaoNFSe.DataEmissao = DateTime.Today; note.Prestador.RazaoSocial = "Empresa de teste";
        var document = await generator.GerarAsync(note);
        await Assert.That(document.NomeArquivo).IsEqualTo("DANFSe-123.pdf");
        await Assert.That(document.TipoConteudo).IsEqualTo("application/pdf");
        await Assert.That(System.Text.Encoding.ASCII.GetString(document.Conteudo, 0, 8)).StartsWith("%PDF-");
        var result = await generator.GerarResultadoAsync(note);
        var file = await Assert.That(result).IsAssignableTo<Microsoft.AspNetCore.Http.IFileHttpResult>();
        await Assert.That(file!.FileDownloadName).IsEqualTo(document.NomeArquivo);
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
        using var certificate = X509CertificateLoader.LoadPkcs12(config.Certificados.CertificadoBytes, null, X509KeyStorageFlags.EphemeralKeySet);
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
    public sealed class TenantConfigurations(Action<ConfigNFSe>? extra = null) : INFSeConfigurationProvider<ConfigNFSe>
    {
        public ValueTask<ConfigNFSe> GetConfigurationAsync(string tenantId = "default", CancellationToken cancellationToken = default)
        { cancellationToken.ThrowIfCancellationRequested(); var config = new ConfigNFSe(); Configure(config); extra?.Invoke(config); return ValueTask.FromResult(config); }
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