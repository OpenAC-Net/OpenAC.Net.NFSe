using System;
using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSDocumento : GenericClone<IBSCBSDocumento>
{
    /// <summary>
    /// Grupo de informacoes de documentos fiscais eletronicos que se encontram no repositorio nacional.
    /// </summary>
    public IBSCBSDocumentoDFe? DocumentoDFeNacional { get; set; }

    /// <summary>
    /// Grupo de informacoes de documentos fiscais, eletronicos ou nao, que nao se encontram no repositorio nacional.
    /// </summary>
    public IBSCBSDocumentoFiscalOutro? DocumentoFiscalOutro { get; set; }

    /// <summary>
    /// Grupo de informacoes de documento nao fiscal.
    /// </summary>
    public IBSCBSDocumentoOutro? DocumentoOutro { get; set; }

    /// <summary>
    /// Grupo de informacoes do fornecedor do documento referenciado.
    /// </summary>
    public IBSCBSDocumentoFornecedor? Fornecedor { get; set; }

    /// <summary>
    /// Data da emissao do documento dedutivel (AAAA-MM-DD).
    /// </summary>
    public DateTime DataEmissaoDocumento { get; set; }

    /// <summary>
    /// Data da competencia do documento dedutivel (AAAA-MM-DD).
    /// </summary>
    public DateTime DataCompetenciaDocumento { get; set; }

    /// <summary>
    /// Tipo de valor incluido neste documento, recebido por motivo de operacoes de terceiros,
    /// objeto de reembolso, repasse ou ressarcimento pelo recebedor, ja tributados e aqui referenciados.
    /// </summary>
    public string? TipoReeRepRes { get; set; }

    /// <summary>
    /// Descricao do reembolso ou ressarcimento quando a opcao for "99 - Outros reembolsos ou ressarcimentos recebidos
    /// por valores pagos relativos a operacoes por conta e ordem de terceiro".
    /// </summary>
    public string? DescricaoTipoReeRepRes { get; set; }

    /// <summary>
    /// Valor monetario utilizado para nao inclusao na base de calculo do ISS e do IBS e da CBS da NFS-e que esta sendo emitida (R$).
    /// </summary>
    public decimal ValorReeRepRes { get; set; }
}
