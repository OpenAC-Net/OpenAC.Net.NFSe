using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Commom.Model;

public sealed class ParametroProvider
{
    public string Nome { get; set; } = "";
    
    public string Descricao { get; set; } = "";

    public TipoParametro Tipo { get; set; } = TipoParametro.Text;

    public bool Obrigatoria { get; set; }

    public VersaoNFSe[] VersoesAfetadas { get; set; } = [];
}