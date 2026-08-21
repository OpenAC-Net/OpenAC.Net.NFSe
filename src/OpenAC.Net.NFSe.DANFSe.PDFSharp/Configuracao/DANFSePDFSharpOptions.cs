// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.DANFSe.PDFSharp
// Author           : RFTD / OpenAC.Net Team
// Created          : 2026-08-16
// ***********************************************************************
// <copyright file="DANFSePDFSharpOptions.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.IO;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.DANFSe.PDFSharp.Configuracao;

/// <summary>
/// Opções e configurações de impressão do DANFSe em PDF (PDFsharp).
/// </summary>
public sealed class DANFSePDFSharpOptions : DANFSeOptions<FiltroDFeReport>
{
    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância de <see cref="DANFSePDFSharpOptions"/> com as configurações padrão.
    /// </summary>
    public DANFSePDFSharpOptions() : base(ConfigNFSe.Default)
    {
    }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="DANFSePDFSharpOptions"/> associada à configuração da NFS-e.
    /// </summary>
    /// <param name="configuracoes">Instância de configuração do componente NFS-e.</param>
    public DANFSePDFSharpOptions(ConfigNFSe configuracoes) : base(configuracoes)
    {
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Instância estática padrão com configurações pré-definidas.
    /// </summary>
    public static DANFSePDFSharpOptions Default { get; } = new(ConfigNFSe.Default);

    /// <summary>
    /// Logotipo da Prefeitura / Município em bytes (PNG/JPG).
    /// </summary>
    public byte[]? LogoPrefeituraBytes { get; set; }

    /// <summary>
    /// Caminho para o arquivo do logotipo da Prefeitura.
    /// </summary>
    public string? LogoPrefeituraPath
    {
        get => null;
        set
        {
            if (!string.IsNullOrEmpty(value) && File.Exists(value))
                LogoPrefeituraBytes = File.ReadAllBytes(value);
        }
    }

    /// <summary>
    /// Logotipo do Prestador em bytes (PNG/JPG).
    /// </summary>
    public byte[]? LogoPrestadorBytes { get; set; }

    /// <summary>
    /// Caminho para o arquivo do logotipo do Prestador.
    /// </summary>
    public string? LogoPrestadorPath
    {
        get => null;
        set
        {
            if (!string.IsNullOrEmpty(value) && File.Exists(value))
                LogoPrestadorBytes = File.ReadAllBytes(value);
        }
    }

    /// <summary>
    /// Texto da primeira linha do cabeçalho da prefeitura (Padrão: "PREFEITURA MUNICIPAL").
    /// </summary>
    public string CabecalhoLinha1 { get; set; } = "PREFEITURA MUNICIPAL";

    /// <summary>
    /// Texto da segunda linha do cabeçalho da prefeitura (Padrão: "SECRETARIA MUNICIPAL DE FINANÇAS").
    /// </summary>
    public string CabecalhoLinha2 { get; set; } = "SECRETARIA MUNICIPAL DE FINANÇAS";

    /// <summary>
    /// Indica se deve gerar e exibir o QR-Code de consulta da NFS-e.
    /// </summary>
    public bool ExibirQRCode { get; set; } = true;

    /// <summary>
    /// Indica se a nota está cancelada (adiciona marca d'água de NOTA CANCELADA).
    /// </summary>
    public bool Cancelada { get; set; } = false;

    /// <summary>
    /// Indica se a emissão é em ambiente de homologação (adiciona marca d'água de AMBIENTE DE HOMOLOGAÇÃO / SEM VALOR FISCAL).
    /// </summary>
    public bool Homologacao { get; set; } = false;

    /// <summary>
    /// Caractere ou sequência personalizada utilizada para quebra de linhas na discriminação de serviços.
    /// </summary>
    public string QuebraDeLinha { get; set; } = string.Empty;

    /// <summary>
    /// Mensagem personalizada de rodapé (pode utilizar pipe '|' para separar [Esquerda|Centro|Direita]).
    /// </summary>
    public string MensagemRodape { get; set; } = string.Empty;

    /// <summary>
    /// Margem vertical em milímetros (Padrão: 8.0 mm).
    /// </summary>
    public double MargemVerticalMm { get; set; } = 8.0;

    /// <summary>
    /// Margem horizontal em milímetros (Padrão: 8.0 mm).
    /// </summary>
    public double MargemHorizontalMm { get; set; } = 8.0;

    /// <summary>
    /// Configurações de segurança e criptografia por senha do PDF.
    /// </summary>
    public DANFSeSegurancaConfig Seguranca { get; set; } = new();

    #endregion Properties
}
