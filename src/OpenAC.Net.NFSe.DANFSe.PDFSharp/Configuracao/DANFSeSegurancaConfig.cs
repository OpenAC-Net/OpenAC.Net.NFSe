// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.DANFSe.PDFSharp
// Author           : RFTD / OpenAC.Net Team
// Created          : 2026-08-16
// ***********************************************************************
// <copyright file="DANFSeSegurancaConfig.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace OpenAC.Net.NFSe.DANFSe.PDFSharp.Configuracao;

/// <summary>
/// Opções de segurança e criptografia com senha para o PDF da NFS-e.
/// </summary>
public sealed class DANFSeSegurancaConfig
{
    #region Properties

    /// <summary>
    /// Senha de usuário exigida para abrir e visualizar o documento PDF.
    /// Se informada, o PDF será protegido e solicitará essa senha ao ser aberto.
    /// </summary>
    public string? SenhaUsuario { get; set; }

    /// <summary>
    /// Senha do proprietário / administrador (necessária para alterar permissões e senhas do PDF).
    /// </summary>
    public string? SenhaProprietario { get; set; }

    /// <summary>
    /// Permite ao usuário imprimir o documento. (Padrão: true)
    /// </summary>
    public bool PermitirImpressao { get; set; } = true;

    /// <summary>
    /// Permite impressão em alta resolução / qualidade. (Padrão: true)
    /// </summary>
    public bool PermitirImpressaoAltaQualidade { get; set; } = true;

    /// <summary>
    /// Permite modificar o conteúdo do documento PDF. (Padrão: false)
    /// </summary>
    public bool PermitirModificacao { get; set; } = false;

    /// <summary>
    /// Permite copiar ou extrair texto e gráficos do PDF. (Padrão: true)
    /// </summary>
    public bool PermitirCopiarConteudo { get; set; } = true;

    /// <summary>
    /// Permite adicionar ou modificar anotações e comentários. (Padrão: true)
    /// </summary>
    public bool PermitirAnotacoes { get; set; } = true;

    /// <summary>
    /// Permite preencher formulários interativos. (Padrão: true)
    /// </summary>
    public bool PermitirPreenchimentoFormularios { get; set; } = true;

    /// <summary>
    /// Permite extração de conteúdo para acessibilidade (leitores de tela). (Padrão: true)
    /// </summary>
    public bool PermitirAcessibilidade { get; set; } = true;

    /// <summary>
    /// Permite montar ou mesclar o documento. (Padrão: false)
    /// </summary>
    public bool PermitirMontarDocumento { get; set; } = false;

    /// <summary>
    /// Retorna verdadeiro se alguma senha foi definida para encriptar o documento.
    /// </summary>
    public bool TemCriptografia => !string.IsNullOrEmpty(SenhaUsuario) || !string.IsNullOrEmpty(SenhaProprietario);

    #endregion Properties
}
