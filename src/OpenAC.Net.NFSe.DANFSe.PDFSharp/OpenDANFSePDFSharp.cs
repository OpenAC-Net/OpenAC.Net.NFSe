// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.DANFSe.PDFSharp
// Author           : RFTD / OpenAC.Net Team
// Created          : 2026-08-16
// ***********************************************************************
// <copyright file="OpenDANFSePDFSharp.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Diagnostics;
using System.IO;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Common;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Configuracao;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Layout;
using OpenAC.Net.NFSe.Nota;
using PdfSharp;
using PdfSharp.Pdf;

namespace OpenAC.Net.NFSe.DANFSe.PDFSharp;

/// <summary>
/// Componente de impressão e exportação do DANFSe (Documento Auxiliar da NFS-e) em PDF (100% Open Source / MIT) usando PDFsharp.
/// </summary>
public class OpenDANFSePDFSharp : OpenDANFSeBase<DANFSePDFSharpOptions, FiltroDFeReport>
{
    #region Constructors

    static OpenDANFSePDFSharp()
    {
        DANFSeFontResolver.GarantirInicializacao();
    }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="OpenDANFSePDFSharp"/> com as configurações padrão.
    /// </summary>
    public OpenDANFSePDFSharp()
    {
        Configuracoes = new DANFSePDFSharpOptions();
    }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="OpenDANFSePDFSharp"/> associada às configurações do componente OpenNFSe.
    /// </summary>
    /// <param name="config">Configurações da NFS-e.</param>
    public OpenDANFSePDFSharp(ConfigNFSe config)
    {
        Configuracoes = new DANFSePDFSharpOptions(config);
    }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="OpenDANFSePDFSharp"/> com as opções especificadas.
    /// </summary>
    /// <param name="options">Opções do DANFSe PDFSharp.</param>
    public OpenDANFSePDFSharp(DANFSePDFSharpOptions? options)
    {
        DANFSeFontResolver.GarantirInicializacao();
        Configuracoes = options ?? new DANFSePDFSharpOptions();
    }

    #endregion Constructors

    #region Public Instance Methods

    /// <inheritdoc />
    public override void Imprimir(NotaServico[] notas)
    {
        if (notas == null || notas.Length == 0)
            throw new ArgumentException("Nenhuma nota fiscal informada para impressão.", nameof(notas));

        var tempFile = Path.Combine(Path.GetTempPath(), $"DANFSe_{Guid.NewGuid():N}.pdf");
        ImprimirPDF(notas, tempFile);

        if (!Configuracoes.MostrarPreview) return;
        
        try
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = tempFile,
                UseShellExecute = true
            };
            process.Start();
        }
        catch
        {
            // Em caso de falha ao abrir o visualizador padrão
        }
    }

    /// <inheritdoc />
    public override void ImprimirPDF(NotaServico[] notas)
    {
        if (string.IsNullOrWhiteSpace(Configuracoes.NomeArquivo))
            throw new InvalidOperationException("O nome do arquivo de destino (Configuracoes.NomeArquivo) não foi informado.");

        ImprimirPDF(notas, Configuracoes.NomeArquivo);
    }

    /// <summary>
    /// Gera o documento PDF da NFS-e e salva no arquivo especificado.
    /// </summary>
    /// <param name="nota">Nota de serviço.</param>
    /// <param name="caminhoArquivo">Caminho do arquivo de destino.</param>
    public void ImprimirPDF(NotaServico nota, string caminhoArquivo)
    {
        ImprimirPDF([nota], caminhoArquivo);
    }

    /// <summary>
    /// Gera o documento PDF das NFS-e e salva no arquivo especificado.
    /// </summary>
    /// <param name="notas">Array de notas de serviço.</param>
    /// <param name="caminhoArquivo">Caminho do arquivo de destino.</param>
    public void ImprimirPDF(NotaServico[] notas, string caminhoArquivo)
    {
        if (string.IsNullOrWhiteSpace(caminhoArquivo))
            throw new ArgumentNullException(nameof(caminhoArquivo));

        using var doc = GerarPdfDocument(notas);
        doc.Save(caminhoArquivo);
    }

    /// <summary>
    /// Gera o documento PDF da NFS-e e salva na Stream informada.
    /// </summary>
    /// <param name="nota">Nota de serviço.</param>
    /// <param name="stream">Stream de destino.</param>
    public void ImprimirPDF(NotaServico nota, Stream stream)
    {
        ImprimirPDF([nota], stream);
    }

    /// <inheritdoc />
    public override void ImprimirPDF(NotaServico[] notas, Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        using var doc = GerarPdfDocument(notas);
        doc.Save(stream, false);
    }

    /// <inheritdoc />
    public override void ImprimirHTML(NotaServico[] notas) => throw new NotSupportedException("Geração de HTML não é suportada pelo gerador PDFsharp.");

    /// <inheritdoc />
    public override void ImprimirHTML(NotaServico[] notas, Stream stream) => throw new NotSupportedException("Geração de HTML não é suportada pelo gerador PDFsharp.");

    /// <summary>
    /// Retorna os bytes do PDF gerado a partir da NFS-e.
    /// </summary>
    /// <param name="nota">Nota de serviço.</param>
    /// <returns>Bytes do PDF.</returns>
    public byte[] GerarPDF(NotaServico nota)
    {
        using var ms = new MemoryStream();
        ImprimirPDF(nota, ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Retorna os bytes do PDF gerado a partir de múltiplas NFS-e.
    /// </summary>
    /// <param name="notas">Array de notas de serviço.</param>
    /// <returns>Bytes do PDF.</returns>
    public byte[] GerarPDF(NotaServico[] notas)
    {
        using var ms = new MemoryStream();
        ImprimirPDF(notas, ms);
        return ms.ToArray();
    }

    #endregion Public Instance Methods

    #region Static Helper Methods

    /// <summary>
    /// Gera o documento PDF da NFS-e e salva na Stream informada (estático).
    /// </summary>
    public static void GerarPDF(NotaServico nota, Stream stream, DANFSePDFSharpOptions? config = null)
    {
        var danfse = new OpenDANFSePDFSharp(config ?? new DANFSePDFSharpOptions());
        danfse.ImprimirPDF(nota, stream);
    }

    /// <summary>
    /// Gera o documento PDF da NFS-e e salva no arquivo informado (estático).
    /// </summary>
    public static void GerarPDF(NotaServico nota, string caminhoArquivo, DANFSePDFSharpOptions? config = null)
    {
        var danfse = new OpenDANFSePDFSharp(config ?? new DANFSePDFSharpOptions());
        danfse.ImprimirPDF(nota, caminhoArquivo);
    }

    /// <summary>
    /// Retorna os bytes do PDF gerado a partir da NFS-e (estático).
    /// </summary>
    public static byte[] GerarPDF(NotaServico nota, DANFSePDFSharpOptions? config = null)
    {
        var danfse = new OpenDANFSePDFSharp(config ?? new DANFSePDFSharpOptions());
        return danfse.GerarPDF(nota);
    }

    #endregion Static Helper Methods

    #region Private Methods

    private PdfDocument GerarPdfDocument(params NotaServico[] notas)
    {
        if (notas == null || notas.Length == 0)
            throw new ArgumentException("Nenhuma nota fiscal informada para impressão.", nameof(notas));

        var doc = new PdfDocument();
        doc.Info.Title = $"DANFSe - {notas[0].IdentificacaoNFSe.Numero}";
        doc.Info.Author = "OpenAC .Net";
        doc.Info.Creator = "OpenAC.Net.NFSe.DANFSe.PDFSharp";

        foreach (var nota in notas)
        {
            var report = new DANFSeA4RetratoReport(nota, Configuracoes);
            report.Render(doc);
        }

        AplicarSeguranca(doc);

        return doc;
    }

    private void AplicarSeguranca(PdfDocument doc)
    {
        var seguranca = Configuracoes.Seguranca;
        if (seguranca is not { TemCriptografia: true }) return;

        var sec = doc.SecuritySettings;
        if (!string.IsNullOrEmpty(seguranca.SenhaUsuario))
            sec.UserPassword = seguranca.SenhaUsuario!;

        if (!string.IsNullOrEmpty(seguranca.SenhaProprietario))
            sec.OwnerPassword = seguranca.SenhaProprietario!;

        sec.PermitPrint = seguranca.PermitirImpressao;
        sec.PermitFullQualityPrint = seguranca.PermitirImpressaoAltaQualidade;
        sec.PermitModifyDocument = seguranca.PermitirModificacao;
        sec.PermitExtractContent = seguranca.PermitirCopiarConteudo;
        sec.PermitAnnotations = seguranca.PermitirAnotacoes;
        sec.PermitFormsFill = seguranca.PermitirPreenchimentoFormularios;
        sec.PermitAssembleDocument = seguranca.PermitirMontarDocumento;
    }

    #endregion Private Methods
}
