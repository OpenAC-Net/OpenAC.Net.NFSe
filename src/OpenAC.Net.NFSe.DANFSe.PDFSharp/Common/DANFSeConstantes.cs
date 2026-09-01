// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.DANFSe.PDFSharp
// Author           : RFTD / OpenAC.Net Team
// Created          : 2026-08-16
// ***********************************************************************
// <copyright file="DANFSeConstantes.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace OpenAC.Net.NFSe.DANFSe.PDFSharp.Common;

/// <summary>
/// Constantes de medidas, fontes e mensagens de layout do DANFSe A4 Retrato.
/// </summary>
public static class DANFSeConstantes
{
    #region Dimensões em Milímetros (A4)

    /// <summary>Largura da página A4 em milímetros (210.0 mm).</summary>
    public const double PaginaLarguraMm = 210.0;

    /// <summary>Altura da página A4 em milímetros (297.0 mm).</summary>
    public const double PaginaAlturaMm = 297.0;

    /// <summary>Margem padrão do documento em milímetros (8.0 mm).</summary>
    public const double MargemPadraoMm = 8.0;

    /// <summary>Altura padrão do bloco de cabeçalho em milímetros (26.0 mm).</summary>
    public const double AlturaCabecalhoMm = 26.0;

    /// <summary>Altura padrão do bloco do prestador em milímetros (25.0 mm).</summary>
    public const double AlturaPrestadorMm = 25.0;

    /// <summary>Altura padrão do bloco do tomador em milímetros (25.0 mm).</summary>
    public const double AlturaTomadorMm = 25.0;

    /// <summary>Altura da linha do cabeçalho da tabela de itens em milímetros (4.0 mm).</summary>
    public const double AlturaLinhaCabecalhoItemMm = 4.0;

    /// <summary>Altura padrão do bloco de rodapé em milímetros (5.0 mm).</summary>
    public const double AlturaRodapeMm = 5.0;

    /// <summary>Tamanho padrão do QR Code em milímetros (20.0 mm).</summary>
    public const double QrCodeTamanhoMm = 20.0;

    #endregion Dimensões em Milímetros (A4)

    #region Fontes e Tamanhos em Pontos

    /// <summary>Família de fonte padrão do documento.</summary>
    public const string FontePadrao = "LiberationSans";

    /// <summary>Tamanho em pontos do título do documento.</summary>
    public const double FonteTituloDocPt = 10.0;

    /// <summary>Tamanho em pontos do título dos blocos.</summary>
    public const double FonteTituloBlocoPt = 8.0;

    /// <summary>Tamanho em pontos dos rótulos (labels) dos campos.</summary>
    public const double FonteLabelCampoPt = 6.0;

    /// <summary>Tamanho em pontos do conteúdo padrão.</summary>
    public const double FonteConteudoPt = 8.0;

    /// <summary>Tamanho em pontos do conteúdo em negrito.</summary>
    public const double FonteConteudoNegritoPt = 9.0;

    /// <summary>Tamanho em pontos para valores em destaque.</summary>
    public const double FonteValorDestaquePt = 10.0;

    /// <summary>Tamanho em pontos para o valor total da nota.</summary>
    public const double FonteValorTotalNotaPt = 12.0;

    /// <summary>Tamanho em pontos das informações de rodapé.</summary>
    public const double FonteRodapePt = 6.0;

    /// <summary>Tamanho em pontos da marca d'água.</summary>
    public const double FonteMarcaDaguaPt = 42.0;

    #endregion Fontes e Tamanhos em Pontos

    #region Mensagens Padrão

    /// <summary>Mensagem padrão da 1ª linha do cabeçalho.</summary>
    public const string MsgCabecalhoLinha1Padrao = "PREFEITURA MUNICIPAL";

    /// <summary>Mensagem padrão da 2ª linha do cabeçalho.</summary>
    public const string MsgCabecalhoLinha2Padrao = "SECRETARIA MUNICIPAL DE FINANÇAS";

    /// <summary>Título principal do documento DANFSe.</summary>
    public const string MsgTituloDanfse = "NOTA FISCAL DE SERVIÇOS ELETRÔNICA - NFSe";

    /// <summary>Mensagem de marca d'água para documentos sem valor fiscal.</summary>
    public const string MsgSemValorFiscal = "SEM VALOR FISCAL";

    /// <summary>Mensagem de marca d'água para notas canceladas.</summary>
    public const string MsgCancelada = "NOTA CANCELADA";

    /// <summary>Mensagem de marca d'água para notas emitidas em ambiente de homologação.</summary>
    public const string MsgHomologacao = "AMBIENTE DE HOMOLOGAÇÃO";

    #endregion Mensagens Padrão
}
