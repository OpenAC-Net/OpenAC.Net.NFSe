using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Commom.Model;

/// <summary>
/// Define os metadados de um parâmetro de configuração customizado aceito por um provedor de NFSe.
/// </summary>
public sealed class ParametroProvider
{
    /// <summary>
    /// Nome identificador do parâmetro.
    /// </summary>
    public string Nome { get; set; } = "";
    
    /// <summary>
    /// Descrição explicativa da finalidade do parâmetro.
    /// </summary>
    public string Descricao { get; set; } = "";

    /// <summary>
    /// Tipo de dado esperado para o parâmetro.
    /// </summary>
    public TipoParametro Tipo { get; set; } = TipoParametro.Text;

    /// <summary>
    /// Indica se o preenchimento deste parâmetro é obrigatório para o provedor.
    /// </summary>
    public bool Obrigatoria { get; set; }

    /// <summary>
    /// Lista de versões do padrão NFSe afetadas por este parâmetro.
    /// </summary>
    public VersaoNFSe[] VersoesAfetadas { get; set; } = [];
}
