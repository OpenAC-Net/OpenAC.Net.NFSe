// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.DANFSe.PDFSharp
// Author           : RFTD / OpenAC.Net Team
// Created          : 2026-08-16
// ***********************************************************************
// <copyright file="PdfDrawHelper.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using PdfSharp.Drawing;
using QRCoder;

namespace OpenAC.Net.NFSe.DANFSe.PDFSharp.Common;

/// <summary>
/// Utilitários de desenho em PDF para o layout do DANFSe.
/// </summary>
internal static class PdfDrawHelper
{
    #region Cores, Canetas e Pincéis

    public static readonly XColor CorCinzaClaro = XColor.FromArgb(242, 242, 242);
    public static readonly XColor CorCinzaBorda = XColor.FromArgb(160, 160, 160);
    public static readonly XColor CorCinzaMarcaDagua = XColor.FromArgb(210, 210, 210);

    public static readonly XPen PenBorda = new(XColors.Black, 0.5);
    public static readonly XPen PenBordaFina = new(XColors.Black, 0.35);
    public static readonly XPen PenLinhaTracejada = new(XColors.Gray, 0.35) { DashStyle = XDashStyle.Dash };

    public static readonly XBrush BrushFundoSombreado = new XSolidBrush(CorCinzaClaro);
    public static readonly XBrush BrushFundoBranco = new XSolidBrush(XColors.White);
    public static readonly XBrush BrushPreto = new XSolidBrush(XColors.Black);
    public static readonly XBrush BrushCinzaEscuro = new XSolidBrush(XColor.FromArgb(60, 60, 60));

    #endregion Cores, Canetas e Pincéis

    #region Conversões de Unidade

    public static double MmToPt(double mm) => mm * 72.0 / 25.4;

    public static double PtToMm(double pt) => pt * 25.4 / 72.0;

    #endregion Conversões de Unidade

    #region Métodos de Desenho de Formas e Caixas

    public static void DesenharRetangulo(XGraphics gfx, double xMm, double yMm, double wMm, double hMm, bool preencherSombreado = false)
    {
        var rect = new XRect(MmToPt(xMm), MmToPt(yMm), MmToPt(wMm), MmToPt(hMm));
        if (preencherSombreado)
            gfx.DrawRectangle(PenBorda, BrushFundoSombreado, rect);
        else
            gfx.DrawRectangle(PenBorda, rect);
    }

    public static void DesenharTituloBloco(
        XGraphics gfx,
        double xMm,
        double yMm,
        double wMm,
        double hMm,
        string titulo,
        bool preencherSombreado = false,
        XStringAlignment alinhamento = XStringAlignment.Center)
    {
        var rect = new XRect(MmToPt(xMm), MmToPt(yMm), MmToPt(wMm), MmToPt(hMm));
        if (preencherSombreado)
            gfx.DrawRectangle(PenBorda, BrushFundoSombreado, rect);
        else
            gfx.DrawRectangle(PenBorda, rect);

        var font = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteTituloBlocoPt, XFontStyleEx.Bold);
        var format = new XStringFormat
        {
            Alignment = alinhamento,
            LineAlignment = XLineAlignment.Center
        };

        var textRect = new XRect(rect.X + MmToPt(1.0), rect.Y, rect.Width - MmToPt(2.0), rect.Height);
        gfx.DrawString(titulo, font, BrushPreto, textRect, format);
    }

    public static void DesenharCampo(
        XGraphics gfx,
        double xMm,
        double yMm,
        double wMm,
        double hMm,
        string label,
        string valor,
        bool sombreado = false,
        bool negrito = true,
        XStringAlignment alinhamento = XStringAlignment.Near,
        double fontValorPt = DANFSeConstantes.FonteConteudoNegritoPt)
    {
        var rect = new XRect(MmToPt(xMm), MmToPt(yMm), MmToPt(wMm), MmToPt(hMm));
        var brushFundo = sombreado ? BrushFundoSombreado : BrushFundoBranco;
        gfx.DrawRectangle(PenBorda, brushFundo, rect);

        var paddingLeftPt = MmToPt(0.8);
        var paddingRightPt = MmToPt(0.8);
        var innerWidthPt = rect.Width - (paddingLeftPt + paddingRightPt);

        // Label superior
        if (!string.IsNullOrEmpty(label))
        {
            var fontLabel = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteLabelCampoPt, XFontStyleEx.Regular);
            var labelRect = new XRect(rect.X + paddingLeftPt, rect.Y + MmToPt(0.3), innerWidthPt, MmToPt(2.2));
            gfx.DrawString(label, fontLabel, BrushCinzaEscuro, labelRect, XStringFormats.TopLeft);
        }

        // Valor inferior
        var valorTexto = string.IsNullOrWhiteSpace(valor) ? "-" : valor.Trim();
        var styleValor = negrito ? XFontStyleEx.Bold : XFontStyleEx.Regular;
        var fontValor = new XFont(DANFSeConstantes.FontePadrao, fontValorPt, styleValor);

        var posYMm = string.IsNullOrEmpty(label) ? 0.6 : 2.5;
        var valorRect = new XRect(rect.X + paddingLeftPt, rect.Y + MmToPt(posYMm), innerWidthPt, rect.Height - MmToPt(posYMm) - MmToPt(0.3));

        var format = new XStringFormat
        {
            Alignment = alinhamento,
            LineAlignment = XLineAlignment.Near
        };

        var textoAjustado = TruncarTexto(gfx, valorTexto, fontValor, innerWidthPt);
        gfx.DrawString(textoAjustado, fontValor, BrushPreto, valorRect, format);
    }

    public static void DesenharImposto(
        XGraphics gfx,
        double xMm,
        double yMm,
        double wMm,
        double hMm,
        string label,
        decimal valor,
        CultureInfo culture,
        string formato = "#,##0.00")
    {
        var rect = new XRect(MmToPt(xMm), MmToPt(yMm), MmToPt(wMm), MmToPt(hMm));
        gfx.DrawRectangle(PenBorda, rect);

        var paddingPt = MmToPt(0.8);
        var innerWidthPt = rect.Width - (2 * paddingPt);

        // Label centralizada no topo
        var fontLabel = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteLabelCampoPt, XFontStyleEx.Regular);
        var labelRect = new XRect(rect.X + paddingPt, rect.Y + MmToPt(0.4), innerWidthPt, MmToPt(2.2));
        var formatLabel = new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Near };
        gfx.DrawString(label, fontLabel, BrushCinzaEscuro, labelRect, formatLabel);

        // Valor alinhado à direita na base
        var fontValor = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteValorDestaquePt, XFontStyleEx.Bold);
        var valorTexto = valor.ToString(formato, culture);
        var valorRect = new XRect(rect.X + paddingPt, rect.Y + MmToPt(2.6), innerWidthPt, rect.Height - MmToPt(2.9));
        var formatValor = new XStringFormat { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.Center };
        gfx.DrawString(valorTexto, fontValor, BrushPreto, valorRect, formatValor);
    }

    public static double DesenharCampoInline(
        XGraphics gfx,
        double xMm,
        double yMm,
        string label,
        string valor,
        double maxWMm = 0)
    {
        var fontLabel = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteConteudoPt, XFontStyleEx.Regular);
        var fontValor = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteConteudoNegritoPt, XFontStyleEx.Bold);

        var sizeLabel = gfx.MeasureString(label, fontLabel);
        var sizeValor = gfx.MeasureString(valor, fontValor);

        var startXPt = MmToPt(xMm);
        var startYPt = MmToPt(yMm) + MmToPt(3.2);

        gfx.DrawString(label, fontLabel, BrushPreto, startXPt, startYPt);
        gfx.DrawString(valor, fontValor, BrushPreto, startXPt + sizeLabel.Width + MmToPt(0.8), startYPt);

        var totalWidthPt = sizeLabel.Width + MmToPt(0.8) + sizeValor.Width;
        return PtToMm(totalWidthPt);
    }

    #endregion Métodos de Desenho de Formas e Caixas

    #region Desenho de Textos e Parágrafos

    /// <summary>
    /// Desenha texto com quebra automática de palavras (word wrap), centralização ou alinhamento configurável e ajuste progressivo do tamanho da fonte para que caiba no retângulo delimitador sem estourar.
    /// </summary>
    public static void DesenharTextoAjustado(
        XGraphics gfx,
        double xMm,
        double yMm,
        double wMm,
        double hMm,
        string texto,
        double maxFontSizePt = 9.5,
        double minFontSizePt = 5.5,
        bool negrito = true,
        XStringAlignment alinhamento = XStringAlignment.Center,
        XBrush? brush = null)
    {
        if (string.IsNullOrWhiteSpace(texto)) return;

        brush ??= BrushPreto;
        var paddingMm = 0.8;
        var maxWPt = MmToPt(Math.Max(0.1, wMm - (2 * paddingMm)));
        var maxHPt = MmToPt(Math.Max(0.1, hMm - (2 * paddingMm)));

        var rawParagraphs = texto.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        // Itera diminuindo a fonte até caber em largura e altura
        var bestFontSize = minFontSizePt;
        var bestLines = new List<string>();

        for (var sizePt = maxFontSizePt; sizePt >= minFontSizePt; sizePt -= 0.5)
        {
            var fontTest = new XFont(DANFSeConstantes.FontePadrao, sizePt, negrito ? XFontStyleEx.Bold : XFontStyleEx.Regular);
            var lineHeight = sizePt * 1.15;
            var testLines = new List<string>();
            var fits = true;

            foreach (var para in rawParagraphs)
            {
                var trimmedPara = para.Trim();
                if (string.IsNullOrEmpty(trimmedPara))
                {
                    testLines.Add("");
                    continue;
                }

                var words = trimmedPara.Split(' ');
                var currentLine = new StringBuilder();

                foreach (var word in words)
                {
                    if (string.IsNullOrEmpty(word)) continue;

                    var candidate = currentLine.Length == 0 ? word : $"{currentLine} {word}";
                    var measure = gfx.MeasureString(candidate, fontTest);

                    if (measure.Width <= maxWPt)
                    {
                        currentLine.Clear();
                        currentLine.Append(candidate);
                    }
                    else
                    {
                        if (currentLine.Length > 0)
                        {
                            testLines.Add(currentLine.ToString());
                            currentLine.Clear();
                        }

                        var wordMeasure = gfx.MeasureString(word, fontTest);
                        if (wordMeasure.Width > maxWPt && sizePt > minFontSizePt)
                        {
                            fits = false;
                            break;
                        }
                        currentLine.Append(word);
                    }
                }

                if (!fits) break;

                if (currentLine.Length > 0)
                    testLines.Add(currentLine.ToString());
            }

            if (fits)
            {
                var totalHeight = testLines.Count * lineHeight;
                if (totalHeight <= maxHPt || sizePt <= minFontSizePt)
                {
                    bestFontSize = sizePt;
                    bestLines = testLines;
                    break;
                }
            }
        }

        if (bestLines.Count == 0) return;

        var finalFont = new XFont(DANFSeConstantes.FontePadrao, bestFontSize, negrito ? XFontStyleEx.Bold : XFontStyleEx.Regular);
        var finalLineHeightPt = bestFontSize * 1.15;
        var totalBlockHeightPt = bestLines.Count * finalLineHeightPt;

        // Centralização vertical do bloco dentro de hMm
        var startYPt = MmToPt(yMm) + (MmToPt(hMm) - totalBlockHeightPt) / 2.0;

        var format = new XStringFormat
        {
            Alignment = alinhamento,
            LineAlignment = XLineAlignment.Center
        };

        for (var i = 0; i < bestLines.Count; i++)
        {
            var line = bestLines[i];
            if (string.IsNullOrEmpty(line)) continue;

            var lineRect = new XRect(
                MmToPt(xMm + paddingMm),
                startYPt + (i * finalLineHeightPt),
                maxWPt,
                finalLineHeightPt
            );

            gfx.DrawString(line, finalFont, brush, lineRect, format);
        }
    }

    public static void DesenharTextoMultiLinhas(
        XGraphics gfx,
        double xMm,
        double yMm,
        double wMm,
        double hMm,
        string texto,
        double fontSizePt = DANFSeConstantes.FonteConteudoPt,
        bool negrito = false)
    {
        if (string.IsNullOrWhiteSpace(texto)) return;

        var font = new XFont(DANFSeConstantes.FontePadrao, fontSizePt, negrito ? XFontStyleEx.Bold : XFontStyleEx.Regular);
        var lineHeightPt = fontSizePt * 1.25;
        var maxWidthPt = MmToPt(wMm);
        var maxHeightPt = MmToPt(hMm);
        var startXPt = MmToPt(xMm);
        var currentYPt = MmToPt(yMm) + (fontSizePt * 0.9);

        var lines = texto.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        foreach (var rawLine in lines)
        {
            if (currentYPt > MmToPt(yMm) + maxHeightPt)
                break;

            if (string.IsNullOrEmpty(rawLine))
            {
                currentYPt += lineHeightPt;
                continue;
            }

            var words = rawLine.Split(' ');
            var currentLine = new StringBuilder();

            foreach (var word in words)
            {
                var testLine = currentLine.Length == 0 ? word : $"{currentLine} {word}";
                var size = gfx.MeasureString(testLine, font);

                if (size.Width <= maxWidthPt)
                {
                    currentLine.Clear();
                    currentLine.Append(testLine);
                }
                else
                {
                    if (currentLine.Length > 0)
                    {
                        gfx.DrawString(currentLine.ToString(), font, BrushPreto, startXPt, currentYPt);
                        currentYPt += lineHeightPt;

                        if (currentYPt > MmToPt(yMm) + maxHeightPt)
                            break;
                    }

                    currentLine.Clear();
                    currentLine.Append(word);
                }
            }

            if (currentLine.Length > 0 && currentYPt <= MmToPt(yMm) + maxHeightPt)
            {
                gfx.DrawString(currentLine.ToString(), font, BrushPreto, startXPt, currentYPt);
                currentYPt += lineHeightPt;
            }
        }
    }

    public static double MedirAlturaTexto(
        XGraphics gfx,
        string texto,
        double wMm,
        double fontSizePt = DANFSeConstantes.FonteConteudoPt,
        bool negrito = false)
    {
        if (string.IsNullOrWhiteSpace(texto)) return 0;

        var font = new XFont(DANFSeConstantes.FontePadrao, fontSizePt, negrito ? XFontStyleEx.Bold : XFontStyleEx.Regular);
        var lineHeightPt = fontSizePt * 1.25;
        var maxWidthPt = MmToPt(wMm);
        var totalLines = 0;

        var lines = texto.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        foreach (var rawLine in lines)
        {
            if (string.IsNullOrEmpty(rawLine))
            {
                totalLines++;
                continue;
            }

            var words = rawLine.Split(' ');
            var currentLine = new StringBuilder();

            foreach (var word in words)
            {
                var testLine = currentLine.Length == 0 ? word : $"{currentLine} {word}";
                var size = gfx.MeasureString(testLine, font);

                if (size.Width <= maxWidthPt)
                {
                    currentLine.Clear();
                    currentLine.Append(testLine);
                }
                else
                {
                    if (currentLine.Length > 0)
                    {
                        totalLines++;
                    }

                    currentLine.Clear();
                    currentLine.Append(word);
                }
            }

            if (currentLine.Length > 0)
                totalLines++;
        }

        return PtToMm(totalLines * lineHeightPt);
    }

    #endregion Desenho de Textos e Parágrafos

    #region QR Code e Imagens

    public static void DesenharQrCode(XGraphics gfx, double xMm, double yMm, double sizeMm, string conteudo)
    {
        if (string.IsNullOrWhiteSpace(conteudo)) return;

        try
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(conteudo, QRCodeGenerator.ECCLevel.M);
            var matrix = qrCodeData.ModuleMatrix;
            var moduleCount = matrix.Count;
            if (moduleCount == 0) return;

            var totalSizePt = MmToPt(sizeMm);
            var moduleSizePt = totalSizePt / moduleCount;
            var startXPt = MmToPt(xMm);
            var startYPt = MmToPt(yMm);

            for (var row = 0; row < moduleCount; row++)
            {
                var rowBits = matrix[row];
                for (var col = 0; col < rowBits.Length; col++)
                {
                    if (rowBits[col])
                    {
                        gfx.DrawRectangle(BrushPreto, startXPt + (col * moduleSizePt), startYPt + (row * moduleSizePt), moduleSizePt + 0.05, moduleSizePt + 0.05);
                    }
                }
            }
        }
        catch
        {
            // Ignora se não for possível gerar o QR
        }
    }

    public static void DesenharImagem(XGraphics gfx, double xMm, double yMm, double maxWMm, double maxHMm, byte[]? imagemBytes)
    {
        if (imagemBytes == null || imagemBytes.Length == 0) return;

        try
        {
            using var ms = new MemoryStream(imagemBytes);
            using var xImage = XImage.FromStream(ms);

            var maxWidthPt = MmToPt(maxWMm);
            var maxHeightPt = MmToPt(maxHMm);

            var ratioW = maxWidthPt / xImage.PixelWidth;
            var ratioH = maxHeightPt / xImage.PixelHeight;
            var ratio = Math.Min(ratioW, ratioH);

            var destWidth = xImage.PixelWidth * ratio;
            var destHeight = xImage.PixelHeight * ratio;

            var posX = MmToPt(xMm) + (maxWidthPt - destWidth) / 2.0;
            var posY = MmToPt(yMm) + (maxHMm > 0 ? (maxHeightPt - destHeight) / 2.0 : 0);

            gfx.DrawImage(xImage, posX, posY, destWidth, destHeight);
        }
        catch
        {
            // Imagem inválida ou não suportada
        }
    }

    public static void DesenharMarcaDagua(XGraphics gfx, double larguraPaginaMm, double alturaPaginaMm, string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return;

        var state = gfx.Save();
        var centroX = MmToPt(larguraPaginaMm / 2.0);
        var centroY = MmToPt(alturaPaginaMm / 2.0);

        gfx.TranslateTransform(centroX, centroY);
        gfx.RotateTransform(-35);

        var lines = texto.Replace("\r\n", "\n").Replace('\r', '\n')
            .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 0)
        {
            gfx.Restore(state);
            return;
        }

        // Limite máximo de largura ao longo da diagonal dentro da folha
        var maxDiagonalWPt = MmToPt(160.0);

        // Ajuste dinâmico de tamanho de fonte para caber na folha sem estourar
        var bestFontSize = 18.0;
        for (var sizePt = 32.0; sizePt >= 16.0; sizePt -= 1.0)
        {
            var testFont = new XFont(DANFSeConstantes.FontePadrao, sizePt, XFontStyleEx.Bold);
            var maxLineW = 0.0;
            foreach (var line in lines)
            {
                var measure = gfx.MeasureString(line, testFont);
                if (measure.Width > maxLineW)
                    maxLineW = measure.Width;
            }

            if (maxLineW <= maxDiagonalWPt || sizePt <= 16.0)
            {
                bestFontSize = sizePt;
                break;
            }
        }

        var font = new XFont(DANFSeConstantes.FontePadrao, bestFontSize, XFontStyleEx.Bold);
        var brush = new XSolidBrush(CorCinzaMarcaDagua);
        var format = new XStringFormat
        {
            Alignment = XStringAlignment.Center,
            LineAlignment = XLineAlignment.Center
        };

        var lineHeightPt = bestFontSize * 1.35;
        var totalHeightPt = lines.Length * lineHeightPt;
        var startYPt = -(totalHeightPt / 2.0) + (lineHeightPt / 2.0);

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var yPt = startYPt + (i * lineHeightPt);
            gfx.DrawString(line, font, brush, 0, yPt, format);
        }

        gfx.Restore(state);
    }

    #endregion QR Code e Imagens

    #region Utilitários de Formatação

    public static string FormatarCNPJouCPF(string? documento)
    {
        if (string.IsNullOrWhiteSpace(documento)) return string.Empty;
        var num = Regex.Replace(documento, @"\D", "");
        return num.Length switch
        {
            11 => Convert.ToUInt64(num).ToString(@"000\.000\.000\-00"),
            14 => Convert.ToUInt64(num).ToString(@"00\.000\.000\/0000\-00"),
            _ => documento
        };
    }

    public static string FormatarCEP(string? cep)
    {
        if (string.IsNullOrWhiteSpace(cep)) return string.Empty;
        var num = Regex.Replace(cep, @"\D", "");
        return num.Length == 8 ? Convert.ToUInt64(num).ToString(@"00000\-000") : cep;
    }

    public static string TruncarTexto(XGraphics gfx, string texto, XFont font, double maxWidthPt)
    {
        var size = gfx.MeasureString(texto, font);
        if (size.Width <= maxWidthPt) return texto;

        var ellipsis = "...";
        var ellipsisWidth = gfx.MeasureString(ellipsis, font).Width;
        if (ellipsisWidth >= maxWidthPt) return string.Empty;

        var len = texto.Length;
        while (len > 1 && (gfx.MeasureString(texto.Substring(0, len), font).Width + ellipsisWidth) > maxWidthPt)
        {
            len--;
        }

        return texto.Substring(0, len) + ellipsis;
    }

    #endregion Utilitários de Formatação
}
