namespace OpenAC.Net.NFSe.Web.Configuration;

/// <summary>Limites específicos dos métodos Web. As políticas comuns pertencem à configuração do Core.</summary>
public sealed class OpenNFSeWebOptions
{
    /// <summary>Timeouts específicos por nome de operação.</summary>
    public IDictionary<string, TimeSpan> TimeoutsPorOperacao { get; } = new Dictionary<string, TimeSpan>();
}
