using System;
using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe;

public sealed partial class OpenNFSe
{
    private readonly SemaphoreSlim operationGate = new(1, 1);

    // Os provedores municipais expõem uma API síncrona. Executamos fora da thread de UI,
    // propagando o cancelamento até a requisição HTTP e aguardando a conclusão real do provedor.
    private async Task<T> ExecuteAsync<T>(Func<T> action, CancellationToken cancellationToken)
    {
        await operationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await Task.Run(() =>
            {
                Configuracoes.OperationToken.Value = cancellationToken;
                try { cancellationToken.ThrowIfCancellationRequested(); return action(); }
                finally { Configuracoes.OperationToken.Value = default; }
            }, cancellationToken).ConfigureAwait(false);
        }
        finally { operationGate.Release(); }
    }

    /// <inheritdoc />
    public Task<RetornoEnviar> EnviarAsync(int lote, bool sincrono = false, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => Enviar(lote, sincrono), cancellationToken);

    /// <inheritdoc />
    public Task<RetornoConsultarSituacao> ConsultarSituacaoAsync(int lote, string protocolo = "", CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => ConsultarSituacao(lote, protocolo), cancellationToken);

    /// <inheritdoc />
    public Task<RetornoConsultarLoteRps> ConsultarLoteRpsAsync(int lote, string protocolo, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => ConsultarLoteRps(lote, protocolo), cancellationToken);

    /// <inheritdoc />
    public Task<RetornoConsultarSequencialRps> ConsultarSequencialRpsAsync(string serie, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => ConsultarSequencialRps(serie), cancellationToken);

    /// <inheritdoc />
    public Task<RetornoConsultarNFSeRps> ConsultaNFSeRpsAsync(int numero, string serie, TipoRps tipo, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => ConsultaNFSeRps(numero, serie, tipo), cancellationToken);

    /// <inheritdoc />
    public Task<RetornoConsultarNFSeRps> ConsultaNFSeRpsAsync(int numero, string serie, TipoRps tipo, int mesCompetencia, int anoCompetencia, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => ConsultaNFSeRps(numero, serie, tipo, mesCompetencia, anoCompetencia), cancellationToken);

    /// <inheritdoc />
    public Task<RetornoConsultarNFSe> ConsultaNFSeAsync(int numeroNfse, int pagina = 0, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => ConsultaNFSe(numeroNfse, pagina), cancellationToken);

    /// <inheritdoc />
    public Task<RetornoConsultarNFSe> ConsultaNFSeAsync(int numeroNfse, string serieNfse, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => ConsultaNFSe(numeroNfse, serieNfse), cancellationToken);

    /// <inheritdoc />
    public Task<RetornoConsultarNFSe> ConsultaNFSePeriodoAsync(DateTime inicio, DateTime fim, int pagina = 0, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => ConsultaNFSePeriodo(inicio, fim, pagina), cancellationToken);

    /// <inheritdoc />
    public Task<RetornoConsultarNFSe> ConsultaNFSeTomadorAsync(string cnpjTomador, string imTomador, int pagina = 0, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => ConsultaNFSeTomador(cnpjTomador, imTomador, pagina), cancellationToken);

    /// <inheritdoc />
    public Task<RetornoConsultarNFSe> ConsultaNFSeIntermediarioAsync(string nomeInter, string cnpjInter, string imInter, int pagina = 0, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => ConsultaNFSeIntermediario(nomeInter, cnpjInter, imInter, pagina), cancellationToken);

    /// <inheritdoc />
    public Task<RetornoCancelar> CancelarNFSeAsync(string codigoCancelamento, string numeroNFSe, string motivo, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => CancelarNFSe(codigoCancelamento, numeroNFSe, motivo), cancellationToken);

    /// <inheritdoc />
    public Task<RetornoCancelar> CancelarNFSeAsync(string codigoCancelamento, string numeroNFSe, string serie, decimal valor, string motivo, string? codigoVerificacao = null, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => CancelarNFSe(codigoCancelamento, numeroNFSe, serie, valor, motivo, codigoVerificacao), cancellationToken);

    /// <inheritdoc />
    public Task<RetornoCancelarNFSeLote> CancelarNFSeLoteAsync(int lote, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => CancelarNFSeLote(lote), cancellationToken);

    /// <inheritdoc />
    public Task<RetornoSubstituirNFSe> SubstituirNFSeAsync(string codigoCancelamento, string numeroNFSe, string motivo, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => SubstituirNFSe(codigoCancelamento, numeroNFSe, motivo), cancellationToken);
}
