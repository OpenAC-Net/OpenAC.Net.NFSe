// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.DANFSe.FastReport.OpenSource
// Author           : Rafael Dias
// Created          : 01-31-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-05-2018
// ***********************************************************************
// <copyright file="FastReportExtensions.cs" company="OpenAC.Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2022 Projeto OpenAC .Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using FastReport;
using FastReport.Export;
using FastReport.Export.Image;
using FastReport.Utils;

namespace OpenAC.Net.NFSe.DANFSe.FastReport.OpenSource;

internal static class FastReportExtensions
{
    #region Fields

    private const float scaleFactor = 300 / 96f;

    #endregion Fields

    #region Methods

    public static void PrintWithDialog(this Report report)
    {
        using var dlg = new PrintDialog();
        dlg.AllowSomePages = true;
        dlg.AllowSelection = true;
        dlg.UseEXDialog = true;

        if (dlg.ShowDialog() != DialogResult.OK) return;

        report.Print(dlg.PrinterSettings);
    }

    public static void Print(this Report report, PrinterSettings settings = null)
    {
        var doc = report.PrepareDoc(settings);
        if (doc == null) return;

        doc.Print();
        doc.Dispose();
    }

    public static void Show(this Report report, PrinterSettings settings = null)
    {
        var doc = report.PrepareDoc(settings);
        if (doc == null) return;

        using (var preview = new PrintPreviewDialog
               {
                   Document = doc,
                   StartPosition = FormStartPosition.CenterScreen,
                   WindowState = FormWindowState.Maximized
               })
            preview.ShowDialog();

        doc.Dispose();
    }

    private static PrintDocument PrepareDoc(this Report report, PrinterSettings settings = null)
    {
        if (report.PreparedPages.Count < 1)
        {
            report.Prepare();
            if (report.PreparedPages.Count < 1) return null;
        }

        var page = 0;
        var exp = new ImageExport { ImageFormat = ImageExportFormat.Png, Resolution = 600 };

        var doc = new PrintDocument { DocumentName = report.Name };

        if (settings != null)
            doc.PrinterSettings = settings;

        // Ajustando o tamanho da pagina
        doc.QueryPageSettings += (_, args) =>
        {
            var rPage = report.PreparedPages.GetPage(page);
            args.PageSettings.Landscape = rPage.Landscape;
            args.PageSettings.Margins = new Margins((int)(scaleFactor * rPage.LeftMargin * Units.HundrethsOfInch),
                (int)(scaleFactor * rPage.RightMargin * Units.HundrethsOfInch),
                (int)(scaleFactor * rPage.TopMargin * Units.HundrethsOfInch),
                (int)(scaleFactor * rPage.BottomMargin * Units.HundrethsOfInch));

            args.PageSettings.PaperSize = new PaperSize("Custom", (int)(ExportUtils.GetPageWidth(rPage) * scaleFactor * Units.HundrethsOfInch),
                (int)(ExportUtils.GetPageHeight(rPage) * scaleFactor * Units.HundrethsOfInch));
        };

        doc.PrintPage += (_, args) =>
        {
            using (var ms = new MemoryStream())
            {
                exp.PageRange = PageRange.PageNumbers;
                exp.PageNumbers = $"{page + 1}";
                exp.Export(report, ms);

                args.Graphics?.DrawImage(Image.FromStream(ms), args.PageBounds);
            }

            page++;

            args.HasMorePages = page < report.PreparedPages.Count;
        };

        doc.EndPrint += (_, _) => page = 0;
        doc.Disposed += (_, _) => exp?.Dispose();

        return doc;
    }

    #endregion Methods
}