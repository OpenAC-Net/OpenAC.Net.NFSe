using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Providers;
using OpenAC.Net.NFSe.Storage;

namespace OpenAC.Net.NFSe.Test;

public class TestCommunication
{
    [Test]
    public async Task DefaultClientsHaveIndependentConfiguration()
    {
        using var first = new OpenNFSe();
        using var second = new OpenNFSe();
        first.Configuracoes.PrestadorPadrao.CpfCnpj = "11111111111111";
        await Assert.That(second.Configuracoes).IsNotSameReferenceAs(first.Configuracoes);
        await Assert.That(ConfigNFSe.Default).IsNotSameReferenceAs(ConfigNFSe.Default);
        await Assert.That(second.Configuracoes.PrestadorPadrao.CpfCnpj).IsNotEqualTo(first.Configuracoes.PrestadorPadrao.CpfCnpj);
    }

    [Test]
    [Arguments("UTF-8", false)]
    [Arguments("ISO-8859-1", false)]
    [Arguments("ISO-8859-1", true)]
    public async Task HttpPreservesAuthenticationEncodingAndErrorBodies(string declaredCharset, bool latin1)
    {
        var bytes = (latin1 ? Encoding.Latin1 : Encoding.UTF8).GetBytes("Serviço não autorizado");
        var responseContent = new TrackedContent(bytes);
        responseContent.Headers.ContentType = new("text/xml") { CharSet = declaredCharset };
        var transport = new CallbackTransport(async (request, _) =>
        {
            await Assert.That(request.Headers.Authorization?.Scheme).IsEqualTo("Basic");
            await Assert.That(request.Headers.Authorization?.Parameter).IsEqualTo(Convert.ToBase64String(Encoding.UTF8.GetBytes("user:password")));
            await Assert.That(request.Headers.ExpectContinue).IsFalse();
            return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = responseContent };
        });
        var configuration = CreateConfiguration();
        using var component = new OpenNFSe(configuration, transport, new NullStore());
        using var provider = ProviderManager.GetProvider(configuration);
        using var client = new ProbeClient(provider);
        client.Post();
        await Assert.That(client.EnvelopeRetorno).IsEqualTo("Serviço não autorizado");
        await Assert.That(client.LastStatus).IsEqualTo(HttpStatusCode.InternalServerError);
        await Assert.That(responseContent.Disposed).IsTrue();
    }

    [Test]
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
        await Assert.That(async () => { await pending.WaitAsync(TimeSpan.FromSeconds(10)); }).Throws<OperationCanceledException>();
    }

    [Test]
    public async Task ConcurrentFileNamesPreserveAllDocuments()
    {
        var directory = Path.Combine(Path.GetTempPath(), "nfse-store-" + Guid.NewGuid().ToString("N"));
        try
        {
            var document = new NFSeDocument(NFSeTipoDocumento.NFSe, "documento", "NFSe.xml", "", DateTime.Today);
            await Task.WhenAll(Enumerable.Range(0, 12).Select(_ => DesktopNFSeDocumentStore.SaveToDirectoryAsync(document, directory)));
            await Assert.That(Directory.GetFiles(directory).Length).IsEqualTo(12);
            foreach (var file in Directory.GetFiles(directory))
            {
                await Assert.That(File.ReadAllText(file)).IsEqualTo("documento");
            }
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Test]
    public async Task InvalidStorageDestinationDoesNotLoopForever()
    {
        var filename = Path.GetTempFileName();
        try
        {
            var document = new NFSeDocument(NFSeTipoDocumento.NFSe, "xml", "NFSe.xml", "", DateTime.Today);
            await Assert.That(() => DesktopNFSeDocumentStore.SaveToDirectoryAsync(document, filename)).ThrowsExactly<IOException>();
        }
        finally { File.Delete(filename); }
    }

    [Test]
    public async Task ReadingCatalogDoesNotChangeLiveMunicipalities()
    {
        var before = ProviderManager.Municipios.ToArray();
        using var stream = new MemoryStream();
        ProviderManager.WriteCatalog(stream, [before[0]]);
        stream.Position = 0;
        var loaded = ProviderManager.ReadCatalog(stream);
        await Assert.That(loaded).HasSingleItem();
        await Assert.That(loaded[0].Codigo).IsEqualTo(before[0].Codigo);
        await Assert.That(ProviderManager.Municipios.ToArray()).IsEquivalentTo(before, TUnit.Assertions.Enums.CollectionOrdering.Matching);
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
