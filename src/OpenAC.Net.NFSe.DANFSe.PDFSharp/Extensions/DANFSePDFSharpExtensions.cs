// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.DANFSe.PDFSharp
// Author           : RFTD / OpenAC.Net Team
// Created          : 2026-08-16
// ***********************************************************************
// <copyright file="DANFSePDFSharpExtensions.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.IO;
using System.Linq;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Configuracao;

namespace OpenAC.Net.NFSe.DANFSe.PDFSharp.Extensions;

/// <summary>
/// Métodos de extensão para facilitar a impressão e exportação em PDF diretamente a partir do objeto <see cref="OpenNFSe"/>.
/// </summary>
public static class DANFSePDFSharpExtensions
{
    /// <summary>
    /// Imprime ou visualiza o DANFSe em PDF.
    /// </summary>
    /// <param name="nfse">Instância do componente OpenNFSe.</param>
    /// <param name="options">Ação para customização das opções de impressão.</param>
    public static void Imprimir(this OpenNFSe nfse, Action<DANFSePDFSharpOptions>? options = null)
    {
        var danfse = new OpenDANFSePDFSharp(nfse.Configuracoes);
        options?.Invoke(danfse.Configuracoes);
        danfse.Imprimir(nfse.NotasServico.ToArray());
    }

    /// <summary>
    /// Exporta o DANFSe para o arquivo PDF configurado nas opções do componente.
    /// </summary>
    /// <param name="nfse">Instância do componente OpenNFSe.</param>
    /// <param name="options">Ação para customização das opções de impressão.</param>
    public static void ImprimirPDF(this OpenNFSe nfse, Action<DANFSePDFSharpOptions>? options = null)
    {
        var danfse = new OpenDANFSePDFSharp(nfse.Configuracoes);
        options?.Invoke(danfse.Configuracoes);
        danfse.ImprimirPDF(nfse.NotasServico.ToArray());
    }

    /// <summary>
    /// Exporta o DANFSe para o arquivo informado no parâmetro.
    /// </summary>
    /// <param name="nfse">Instância do componente OpenNFSe.</param>
    /// <param name="caminhoArquivo">Caminho do arquivo de destino.</param>
    /// <param name="options">Ação para customização das opções de impressão.</param>
    public static void ImprimirPDF(this OpenNFSe nfse, string caminhoArquivo, Action<DANFSePDFSharpOptions>? options = null)
    {
        var danfse = new OpenDANFSePDFSharp(nfse.Configuracoes);
        options?.Invoke(danfse.Configuracoes);
        danfse.ImprimirPDF(nfse.NotasServico.ToArray(), caminhoArquivo);
    }

    /// <summary>
    /// Exporta o DANFSe para a Stream informada.
    /// </summary>
    /// <param name="nfse">Instância do componente OpenNFSe.</param>
    /// <param name="aStream">Stream de destino.</param>
    /// <param name="options">Ação para customização das opções de impressão.</param>
    public static void ImprimirPDF(this OpenNFSe nfse, Stream aStream, Action<DANFSePDFSharpOptions>? options = null)
    {
        var danfse = new OpenDANFSePDFSharp(nfse.Configuracoes);
        options?.Invoke(danfse.Configuracoes);
        danfse.ImprimirPDF(nfse.NotasServico.ToArray(), aStream);
    }

    /// <summary>
    /// Retorna os bytes do PDF gerado a partir das notas carregadas no componente.
    /// </summary>
    /// <param name="nfse">Instância do componente OpenNFSe.</param>
    /// <param name="options">Ação para customização das opções de impressão.</param>
    /// <returns>Bytes do PDF.</returns>
    public static byte[] GerarPDF(this OpenNFSe nfse, Action<DANFSePDFSharpOptions>? options = null)
    {
        var danfse = new OpenDANFSePDFSharp(nfse.Configuracoes);
        options?.Invoke(danfse.Configuracoes);
        return danfse.GerarPDF(nfse.NotasServico.ToArray());
    }
}
