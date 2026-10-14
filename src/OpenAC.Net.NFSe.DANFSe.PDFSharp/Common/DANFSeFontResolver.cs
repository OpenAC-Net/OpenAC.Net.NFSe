// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.DANFSe.PDFSharp
// Author           : RFTD / OpenAC.Net Team
// Created          : 2026-08-16
// ***********************************************************************
// <copyright file="DANFSeFontResolver.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using PdfSharp.Fonts;

namespace OpenAC.Net.NFSe.DANFSe.PDFSharp.Common;

/// <summary>
/// Resolvedor de fontes nativo e compatível com Native AOT para PDFsharp 6.x.
/// Fornece fontes TrueType (LiberationSans - métrica compatível com Arial/Times) embutidas como recursos do assembly,
/// garantindo funcionamento imediato em Docker, Linux, Windows, macOS e ambientes sem fontes instaladas.
/// </summary>
public sealed class DANFSeFontResolver : IFontResolver
{
    #region Fields

    /// <summary>
    /// Instância singleton do resolvedor de fontes.
    /// </summary>
    public static readonly DANFSeFontResolver Instance = new();
    private static readonly ConcurrentDictionary<string, byte[]> FontCache = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Assembly ResourceAssembly = typeof(DANFSeFontResolver).Assembly;

    private const string ResourcePrefix = "OpenAC.Net.NFSe.DANFSe.PDFSharp.Resources.Fonts.";
    private const string FontRegular = "LiberationSans-Regular";
    private const string FontBold = "LiberationSans-Bold";
    private const string FontItalic = "LiberationSans-Italic";
    private const string FontBoldItalic = "LiberationSans-BoldItalic";

    #endregion Fields

    #region Methods

    /// <summary>
    /// Registra o resolvedor global caso ainda não esteja configurado.
    /// </summary>
    public static void GarantirInicializacao()
    {
        try
        {
            if (GlobalFontSettings.FontResolver == null)
            {
                GlobalFontSettings.FontResolver = Instance;
            }
        }
        catch
        {
            // Já inicializado por outro processo / thread
        }
    }

    /// <summary>
    /// Retorna os bytes da fonte solicitada a partir dos recursos embutidos (Native AOT Safe).
    /// </summary>
    public byte[]? GetFont(string faceName)
    {
        if (FontCache.TryGetValue(faceName, out var cached))
            return cached;

        var resourceName = MapearNomeRecurso(faceName);
        var fontBytes = CarregarRecursoEmbutido(resourceName);

        if (fontBytes != null)
        {
            FontCache[faceName] = fontBytes;
            return fontBytes;
        }

        // Fallback para a fonte regular se a variante específica não for encontrada
        var regularBytes = CarregarRecursoEmbutido(ResourcePrefix + FontRegular + ".ttf");
        if (regularBytes != null)
        {
            FontCache[faceName] = regularBytes;
            return regularBytes;
        }

        return null;
    }

    /// <summary>
    /// Resolve o tipo de fonte solicitado para os recursos embutidos.
    /// </summary>
    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        var faceName = (isBold, isItalic) switch
        {
            (true, true) => FontBoldItalic,
            (true, false) => FontBold,
            (false, true) => FontItalic,
            _ => FontRegular
        };

        return new FontResolverInfo(faceName);
    }

    private static string MapearNomeRecurso(string faceName)
    {
        var lower = faceName.ToLowerInvariant();

        if (lower.Contains("bold") && (lower.Contains("italic") || lower.Contains("oblique")))
            return ResourcePrefix + FontBoldItalic + ".ttf";

        if (lower.Contains("bold"))
            return ResourcePrefix + FontBold + ".ttf";

        if (lower.Contains("italic") || lower.Contains("oblique"))
            return ResourcePrefix + FontItalic + ".ttf";

        return ResourcePrefix + FontRegular + ".ttf";
    }

    private static byte[]? CarregarRecursoEmbutido(string resourceName)
    {
        using var stream = ResourceAssembly.GetManifestResourceStream(resourceName);
        if (stream == null) return null;

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    #endregion Methods
}
