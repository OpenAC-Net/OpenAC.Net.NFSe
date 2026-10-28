using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Providers;
using OpenAC.Net.NFSe.Storage;
using Xunit;

namespace OpenAC.Net.NFSe.Test;

public class TestCommunication
{
    [Fact]
    public void DefaultClientsHaveIndependentConfiguration()
    {
        using var first = new OpenNFSe();
        using var second = new OpenNFSe();
        first.Configuracoes.PrestadorPadrao.CpfCnpj = "11111111111111";
        Assert.NotSame(first.Configuracoes, second.Configuracoes);
        Assert.NotSame(ConfigNFSe.Default, ConfigNFSe.Default);
        Assert.NotEqual(first.Configuracoes.PrestadorPadrao.CpfCnpj, second.Configuracoes.PrestadorPadrao.CpfCnpj);
    }

    [Theory]
    [InlineData("UTF-8", false)]
    [InlineData("ISO-8859-1", false)]
    [InlineData("ISO-8859-1", true)]
    public void HttpPreservesAuthenticationEncodingAndErrorBodies(string declaredCharset, bool latin1)
    {
        var bytes = (latin1 ? Encoding.Latin1 : Encoding.UTF8).GetBytes("Serviço não autorizado");
        var responseContent = new TrackedContent(bytes);
        responseContent.Headers.ContentType = new("text/xml") { CharSet = declaredCharset };
        var transport = new CallbackTransport((request, _) =>
        {
            Assert.Equal("Basic", request.Headers.Authorization?.Scheme);
            Assert.Equal(Convert.ToBase64String(Encoding.UTF8.GetBytes("user:password")), request.Headers.Authorization?.Parameter);
            Assert.False(request.Headers.ExpectContinue);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = responseContent });
        });
        var configuration = CreateConfiguration();
        using var component = new OpenNFSe(configuration, transport, new NullStore());
        using var provider = ProviderManager.GetProvider(configuration);
        using var client = new ProbeClient(provider);
        client.Post();
        Assert.Equal("Serviço não autorizado", client.EnvelopeRetorno);
        Assert.Equal(HttpStatusCode.InternalServerError, client.LastStatus);
        Assert.True(responseContent.Disposed);
    }

    [Fact]
    public async Task CancellationReachesHttpThroughMunicipalProvider()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var transport = new CallbackTransport(async (_, token) =>
        {
            started.SetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, token);
            throw new InvalidOperationException();
        });
        using var component = new OpenNFSe(CreateConfiguration(), transport, new NullStore());
        using var cancellation = new CancellationTokenSource();
        var pending = component.ConsultaNFSeAsync(1, cancellationToken: cancellation.Token);
        await started.Task.WaitAsync(TimeSpan.FromSeconds(10));
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => pending.WaitAsync(TimeSpan.FromSeconds(10)));
    }

    [Fact]
    public async Task ConcurrentFileNamesPreserveAllDocuments()
    {
        var directory = Path.Combine(Path.GetTempPath(), "nfse-store-" + Guid.NewGuid().ToString("N"));
        try
        {
            var document = new NFSeDocument(NFSeTipoDocumento.NFSe, "documento", "NFSe.xml", "", DateTime.Today);
            await Task.WhenAll(Enumerable.Range(0, 12).Select(_ => DesktopNFSeDocumentStore.SaveToDirectoryAsync(document, directory)));
            Assert.Equal(12, Directory.GetFiles(directory).Length);
            Assert.All(Directory.GetFiles(directory), file => Assert.Equal("documento", File.ReadAllText(file)));
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Fact]
    public async Task InvalidStorageDestinationDoesNotLoopForever()
    {
        var filename = Path.GetTempFileName();
        try
        {
            var document = new NFSeDocument(NFSeTipoDocumento.NFSe, "xml", "NFSe.xml", "", DateTime.Today);
            await Assert.ThrowsAsync<IOException>(() => DesktopNFSeDocumentStore.SaveToDirectoryAsync(document, filename));
        }
        finally { File.Delete(filename); }
    }

    [Fact]
    public void ReadingCatalogDoesNotChangeLiveMunicipalities()
    {
        var before = ProviderManager.Municipios.ToArray();
        using var stream = new MemoryStream();
        ProviderManager.WriteCatalog(stream, [before[0]]);
        stream.Position = 0;
        var loaded = ProviderManager.ReadCatalog(stream);
        Assert.Single(loaded);
        Assert.Equal(before[0].Codigo, loaded[0].Codigo);
        Assert.Equal(before, ProviderManager.Municipios.ToArray());
    }

    internal static ConfigNFSe CreateConfiguration()
    {
        var configuration = new ConfigNFSe();
        configuration.WebServices.CodigoMunicipio = 3529005;
        configuration.PrestadorPadrao.CpfCnpj = "03514896000115";
        configuration.Geral.Salvar = configuration.Arquivos.Salvar = configuration.WebServices.Salvar = false;
        return configuration;
    }

    private sealed class ProbeClient(ProviderBase provider) : NFSeHttpServiceClient(provider, TipoUrl.ConsultarNFSe, null)
    {
        public HttpStatusCode? LastStatus => StatusCode;
        public void Post() { AuthenticationScheme = AuthScheme.Basic; ExecutePost(new StringContent("<xml/>")); }
        protected override string Authentication() => Convert.ToBase64String(Encoding.UTF8.GetBytes("user:password"));
    }
    internal sealed class CallbackTransport(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> callback) : INFSeHttpTransport
    {
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ConfigNFSe configuration,
            X509Certificate2? certificate, bool validateServerCertificate, CancellationToken cancellationToken = default) => callback(request, cancellationToken);
    }
    internal sealed class NullStore : INFSeDocumentStore
    {
        public Task SaveAsync(NFSeDocument document, ConfigNFSe configuration, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
    private sealed class TrackedContent(byte[] bytes) : ByteArrayContent(bytes)
    {
        public bool Disposed { get; private set; }
        protected override void Dispose(bool disposing) { Disposed = true; base.Dispose(disposing); }
    }
}
