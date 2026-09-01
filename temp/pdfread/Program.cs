using System;
using System.IO;
using System.Linq;
using UglyToad.PdfPig;

var path = @"j:\\rcky\\docs\\nfse\\barueri\\RPS_Layout.pdf";
using var doc = PdfDocument.Open(path);
for (int i = 1; i <= doc.NumberOfPages; i++)
{
    var text = doc.GetPage(i).Text ?? string.Empty;
    if (text.Contains("Registro Tipo", StringComparison.OrdinalIgnoreCase) || text.Contains("Layout", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine($"--- page {i} ---");
        Console.WriteLine(text);
    }
}
