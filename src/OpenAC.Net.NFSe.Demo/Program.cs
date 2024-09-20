using System;
using System.Text;
using System.Windows.Forms;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Demo;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FormMain());
    }
}