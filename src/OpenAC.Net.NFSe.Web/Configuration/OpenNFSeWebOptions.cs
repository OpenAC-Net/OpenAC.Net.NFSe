namespace OpenAC.Net.NFSe.Web.Configuration;

/// <summary>Configura transporte HTTP, resiliência, certificados e persistência da integração ASP.NET Core.</summary>
public sealed class OpenNFSeWebOptions
{
    /// <summary>Habilita persistência de payloads e documentos. Padrão: desabilitada.</summary>
    public bool PersistenciaHabilitada { get; set; }

    /// <summary>Obtém ou define o diretório relativo ao <c>ContentRootPath</c> usado para os documentos.</summary>
    public string CaminhoDocumentos { get; set; } = "App_Data/NFSe";
    /// <summary>Obtém ou define o tempo máximo total de uma operação. O padrão é 100 segundos.</summary>
    public TimeSpan TimeoutOperacao { get; set; } = TimeSpan.FromSeconds(100);

    /// <summary>Obtém ou define o tempo máximo que uma conexão permanece no pool antes de ser renovada.</summary>
    public TimeSpan TempoVidaConexaoPool { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>Obtém ou define o tempo de vida do handler gerenciado pelo <c>IHttpClientFactory</c>.</summary>
    public TimeSpan TempoVidaHandler { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>Obtém ou define o tempo máximo de cada tentativa individual.</summary>
    public TimeSpan TimeoutTentativa { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>Obtém ou define se o circuit breaker isolado por endpoint está habilitado.</summary>
    public bool CircuitBreakerHabilitado { get; set; } = true;

    /// <summary>Obtém os timeouts específicos indexados pelo nome do método do cliente.</summary>
    public IDictionary<string, TimeSpan> TimeoutsPorOperacao { get; } = new Dictionary<string, TimeSpan>();

    /// <summary>Obtém ou define a antecedência mínima exigida para aceitar um certificado.</summary>
    public TimeSpan MargemExpiracaoCertificado { get; set; } = TimeSpan.FromMinutes(5);
}
