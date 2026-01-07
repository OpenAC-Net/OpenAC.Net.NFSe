using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSDocumentoDFe : GenericClone<IBSCBSDocumentoDFe>
{
    /// <summary>
    /// Documento fiscal a que se refere a chave DFe que seja um dos documentos do Repositorio Nacional.
    /// </summary>
    public string? TipoChaveDFe { get; set; }

    /// <summary>
    /// Descricao da DF-e a que se refere a chave DFe que seja um dos documentos do Repositorio Nacional.
    /// Deve ser preenchido apenas quando "tipoChaveDFe = 9 (Outro)".
    /// </summary>
    public string? DescricaoTipoChaveDFe { get; set; }

    /// <summary>
    /// Chave do Documento Fiscal eletronico do repositorio nacional referenciado para operacoes ja tributadas.
    /// </summary>
    public string? ChaveDFe { get; set; }
}
