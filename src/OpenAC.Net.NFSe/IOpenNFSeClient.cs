using System;
using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe;

/// <summary>Operações municipais com cancelamento; cada cliente mantém sua configuração e notas.</summary>
public interface IOpenNFSeClient : IDisposable
{
    /// <summary>Configuração exclusiva do cliente.</summary>
    ConfigNFSe Configuracoes { get; }
    /// <summary>Documentos da operação.</summary>
    NotaServicoCollection NotasServico { get; }
    /// <summary>Executa Enviar com suporte a cancelamento.</summary>
    Task<RetornoEnviar> EnviarAsync(int lote, bool sincrono = false, CancellationToken cancellationToken = default);
    /// <summary>Executa ConsultarSituacao com suporte a cancelamento.</summary>
    Task<RetornoConsultarSituacao> ConsultarSituacaoAsync(int lote, string protocolo = "", CancellationToken cancellationToken = default);
    /// <summary>Executa ConsultarLoteRps com suporte a cancelamento.</summary>
    Task<RetornoConsultarLoteRps> ConsultarLoteRpsAsync(int lote, string protocolo, CancellationToken cancellationToken = default);
    /// <summary>Executa ConsultarSequencialRps com suporte a cancelamento.</summary>
    Task<RetornoConsultarSequencialRps> ConsultarSequencialRpsAsync(string serie, CancellationToken cancellationToken = default);
    /// <summary>Executa ConsultaNFSeRps com suporte a cancelamento.</summary>
    Task<RetornoConsultarNFSeRps> ConsultaNFSeRpsAsync(int numero, string serie, TipoRps tipo, CancellationToken cancellationToken = default);
    /// <summary>Executa ConsultaNFSeRps com suporte a cancelamento.</summary>
    Task<RetornoConsultarNFSeRps> ConsultaNFSeRpsAsync(int numero, string serie, TipoRps tipo, int mesCompetencia, int anoCompetencia, CancellationToken cancellationToken = default);
    /// <summary>Executa ConsultaNFSe com suporte a cancelamento.</summary>
    Task<RetornoConsultarNFSe> ConsultaNFSeAsync(int numeroNfse, int pagina = 0, CancellationToken cancellationToken = default);
    /// <summary>Executa ConsultaNFSe com suporte a cancelamento.</summary>
    Task<RetornoConsultarNFSe> ConsultaNFSeAsync(int numeroNfse, string serieNfse, CancellationToken cancellationToken = default);
    /// <summary>Executa ConsultaNFSePeriodo com suporte a cancelamento.</summary>
    Task<RetornoConsultarNFSe> ConsultaNFSePeriodoAsync(DateTime inicio, DateTime fim, int pagina = 0, CancellationToken cancellationToken = default);
    /// <summary>Executa ConsultaNFSeTomador com suporte a cancelamento.</summary>
    Task<RetornoConsultarNFSe> ConsultaNFSeTomadorAsync(string cnpjTomador, string imTomador, int pagina = 0, CancellationToken cancellationToken = default);
    /// <summary>Executa ConsultaNFSeIntermediario com suporte a cancelamento.</summary>
    Task<RetornoConsultarNFSe> ConsultaNFSeIntermediarioAsync(string nomeInter, string cnpjInter, string imInter, int pagina = 0, CancellationToken cancellationToken = default);
    /// <summary>Executa CancelarNFSe com suporte a cancelamento.</summary>
    Task<RetornoCancelar> CancelarNFSeAsync(string codigoCancelamento, string numeroNFSe, string motivo, CancellationToken cancellationToken = default);
    /// <summary>Executa CancelarNFSe com suporte a cancelamento.</summary>
    Task<RetornoCancelar> CancelarNFSeAsync(string codigoCancelamento, string numeroNFSe, string serie, decimal valor, string motivo, string? codigoVerificacao = null, CancellationToken cancellationToken = default);
    /// <summary>Executa CancelarNFSeLote com suporte a cancelamento.</summary>
    Task<RetornoCancelarNFSeLote> CancelarNFSeLoteAsync(int lote, CancellationToken cancellationToken = default);
    /// <summary>Executa SubstituirNFSe com suporte a cancelamento.</summary>
    Task<RetornoSubstituirNFSe> SubstituirNFSeAsync(string codigoCancelamento, string numeroNFSe, string motivo, CancellationToken cancellationToken = default);
}
