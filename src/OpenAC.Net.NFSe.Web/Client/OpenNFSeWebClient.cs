using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom.Client;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.Storage;
using OpenAC.Net.NFSe.Web.Common;
using OpenAC.Net.NFSe.Web.Configuration;
using OpenAC.Net.NFSe.Web.Exception;
using OpenAC.Net.NFSe.Web.Provider;

namespace OpenAC.Net.NFSe.Web.Client;

internal sealed class OpenNFSeWebClient : IOpenNFSeClient
{
    private static readonly ActivitySource Activities = new("OpenAC.Net.NFSe.Web");
    private static readonly Meter Meter = new("OpenAC.Net.NFSe.Web");
    private static readonly Counter<long> Operations = Meter.CreateCounter<long>("openac.nfse.operations");
    private static readonly Histogram<double> Duration = Meter.CreateHistogram<double>("openac.nfse.operation.duration", "ms");
    private readonly OpenNFSe client;
    private readonly ILogger<OpenNFSeWebClient> logger;
    private readonly OpenNFSeWebOptions options;
    private readonly string tenantId;
    public ConfigNFSe Configuracoes => client.Configuracoes;
    public NotaServicoCollection NotasServico => client.NotasServico;

    public OpenNFSeWebClient(ConfigNFSe configuration, INFSeHttpTransport transport, INFSeDocumentStore store,
        ILogger<OpenNFSeWebClient> logger, IOptions<OpenNFSeWebOptions> options, string tenantId = NFSeTenant.Padrao)
    {
        client = new OpenNFSe(configuration, transport, store, () => NFSeWebCertificateResolver.Resolve(configuration.Certificados));
        this.logger = logger;
        this.options = options.Value;
        this.tenantId = tenantId;
    }

    private async Task<T> ExecuteAsync<T>(string operation, CancellationToken cancellationToken,
        Func<CancellationToken, Task<T>> action)
    {
        using var activity = Activities.StartActivity(operation, ActivityKind.Client);
        activity?.SetTag("nfse.operation", operation);
        activity?.SetTag("nfse.tenant.id", tenantId);
        using var scope = logger.BeginScope(new Dictionary<string, object?>
        {
            ["NFSe.Operation"] = operation, ["NFSe.TenantId"] = tenantId,
            ["NFSe.Municipio"] = Configuracoes.WebServices.CodigoMunicipio
        });
        var started = Stopwatch.GetTimestamp();
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(options.TimeoutsPorOperacao.TryGetValue(operation, out var duration) ? duration : options.TimeoutOperacao);
        var outcome = "success";
        try
        {
            var result = await action(timeout.Token).ConfigureAwait(false);
            if (result is RetornoWebservice response && !response.Sucesso)
            {
                outcome = "fiscal_error";
                activity?.SetStatus(ActivityStatusCode.Error, outcome);
            }
            else activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (OperationCanceledException)
        {
            outcome = "cancelled";
            activity?.SetStatus(ActivityStatusCode.Error, outcome);
            logger.LogWarning("Operação {Operation} cancelada ou expirada", operation);
            throw;
        }
        catch (System.Exception exception) when (exception is OpenDFeCommunicationException or HttpRequestException
            or Polly.CircuitBreaker.BrokenCircuitException or Polly.Timeout.TimeoutRejectedException)
        {
            outcome = "technical_error";
            activity?.SetStatus(ActivityStatusCode.Error, outcome);
            logger.LogError("Falha técnica na operação {Operation}: {ExceptionType}", operation, exception.GetType().Name);
            throw new NFSeTechnicalException(operation, $"Falha técnica na operação '{operation}'.", exception);
        }
        catch (System.Exception)
        {
            outcome = "error";
            activity?.SetStatus(ActivityStatusCode.Error, outcome);
            throw;
        }
        finally
        {
            Operations.Add(1, new KeyValuePair<string, object?>("operation", operation), new("outcome", outcome));
            Duration.Record(Stopwatch.GetElapsedTime(started).TotalMilliseconds, new KeyValuePair<string, object?>("operation", operation));
        }
    }
    public void Dispose() => client.Dispose();

    public Task<RetornoEnviar> EnviarAsync(int lote, bool sincrono = false, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(EnviarAsync), cancellationToken, ct => client.EnviarAsync(lote, sincrono, ct));

    public Task<RetornoConsultarSituacao> ConsultarSituacaoAsync(int lote, string protocolo = "", CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultarSituacaoAsync), cancellationToken, ct => client.ConsultarSituacaoAsync(lote, protocolo, ct));

    public Task<RetornoConsultarLoteRps> ConsultarLoteRpsAsync(int lote, string protocolo, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultarLoteRpsAsync), cancellationToken, ct => client.ConsultarLoteRpsAsync(lote, protocolo, ct));

    public Task<RetornoConsultarSequencialRps> ConsultarSequencialRpsAsync(string serie, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultarSequencialRpsAsync), cancellationToken, ct => client.ConsultarSequencialRpsAsync(serie, ct));

    public Task<RetornoConsultarNFSeRps> ConsultaNFSeRpsAsync(int numero, string serie, TipoRps tipo, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultaNFSeRpsAsync), cancellationToken, ct => client.ConsultaNFSeRpsAsync(numero, serie, tipo, ct));

    public Task<RetornoConsultarNFSeRps> ConsultaNFSeRpsAsync(int numero, string serie, TipoRps tipo, int mesCompetencia, int anoCompetencia, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultaNFSeRpsAsync), cancellationToken, ct => client.ConsultaNFSeRpsAsync(numero, serie, tipo, mesCompetencia, anoCompetencia, ct));

    public Task<RetornoConsultarNFSe> ConsultaNFSeAsync(int numeroNfse, int pagina = 0, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultaNFSeAsync), cancellationToken, ct => client.ConsultaNFSeAsync(numeroNfse, pagina, ct));

    public Task<RetornoConsultarNFSe> ConsultaNFSeAsync(int numeroNfse, string serieNfse, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultaNFSeAsync), cancellationToken, ct => client.ConsultaNFSeAsync(numeroNfse, serieNfse, ct));

    public Task<RetornoConsultarNFSe> ConsultaNFSePeriodoAsync(DateTime inicio, DateTime fim, int pagina = 0, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultaNFSePeriodoAsync), cancellationToken, ct => client.ConsultaNFSePeriodoAsync(inicio, fim, pagina, ct));

    public Task<RetornoConsultarNFSe> ConsultaNFSeTomadorAsync(string cnpjTomador, string imTomador, int pagina = 0, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultaNFSeTomadorAsync), cancellationToken, ct => client.ConsultaNFSeTomadorAsync(cnpjTomador, imTomador, pagina, ct));

    public Task<RetornoConsultarNFSe> ConsultaNFSeIntermediarioAsync(string nomeInter, string cnpjInter, string imInter, int pagina = 0, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultaNFSeIntermediarioAsync), cancellationToken, ct => client.ConsultaNFSeIntermediarioAsync(nomeInter, cnpjInter, imInter, pagina, ct));

    public Task<RetornoCancelar> CancelarNFSeAsync(string codigoCancelamento, string numeroNFSe, string motivo, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(CancelarNFSeAsync), cancellationToken, ct => client.CancelarNFSeAsync(codigoCancelamento, numeroNFSe, motivo, ct));

    public Task<RetornoCancelar> CancelarNFSeAsync(string codigoCancelamento, string numeroNFSe, string serie, decimal valor, string motivo, string? codigoVerificacao = null, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(CancelarNFSeAsync), cancellationToken, ct => client.CancelarNFSeAsync(codigoCancelamento, numeroNFSe, serie, valor, motivo, codigoVerificacao, ct));

    public Task<RetornoCancelarNFSeLote> CancelarNFSeLoteAsync(int lote, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(CancelarNFSeLoteAsync), cancellationToken, ct => client.CancelarNFSeLoteAsync(lote, ct));

    public Task<RetornoSubstituirNFSe> SubstituirNFSeAsync(string codigoCancelamento, string numeroNFSe, string motivo, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(SubstituirNFSeAsync), cancellationToken, ct => client.SubstituirNFSeAsync(codigoCancelamento, numeroNFSe, motivo, ct));
}
