using System.Collections.Generic;
using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class InfoIBSCBS : GenericClone<InfoIBSCBS>
{
    public InfoIBSCBS()
    {
        ReferenciasNFSe = new List<string>();
        Valores = new InfoValoresIBSCBS();
    }

    /// <summary>
    /// Indicador da finalidade da emissao de NFS-e.
    /// </summary>
    public string? FinalidadeNFSe { get; set; }

    /// <summary>
    /// Indica operacao de uso ou consumo pessoal (art. 57).
    /// </summary>
    public string? IndicadorFinal { get; set; }

    /// <summary>
    /// Codigo indicador da operacao de fornecimento, conforme tabela "codigo indicador de operacao".
    /// </summary>
    public string? CodigoIndicadorOperacao { get; set; }

    /// <summary>
    /// Codigo indicador da operacao com entes governamentais, conforme situacoes previstas na LC 214/2025.
    /// </summary>
    public string? TipoOperacao { get; set; }

    /// <summary>
    /// Referencias de NFS-e.
    /// </summary>
    public List<string> ReferenciasNFSe { get; }

    /// <summary>
    /// Tipo de ente governamental.
    /// </summary>
    public string? TipoEnteGov { get; set; }

    /// <summary>
    /// A respeito do destinatario dos servicos.
    /// </summary>
    public string? IndicadorDestinatario { get; set; }

    /// <summary>
    /// Grupo de informacoes relativas aos valores do servico prestado para IBS e CBS.
    /// </summary>
    public InfoValoresIBSCBS Valores { get; set; }
}
