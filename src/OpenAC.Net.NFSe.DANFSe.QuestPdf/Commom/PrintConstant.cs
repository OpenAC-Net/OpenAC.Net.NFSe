using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.DANFSe.QuestPdf.Commom;

internal class PrintConstant
{
    public const string OpenSans = "Open Sans";
    public const string UbuntuCondensed = "Ubuntu";
    public const float BorderSize = 0.5f;
    public const float TitleSize = 2.3f;
    public static readonly TextStyle ItemTitleStyle;
    public static readonly TextStyle ItemContentStyle;
    public static readonly TextStyle BoxTitleStyle;
    public static readonly TextStyle BoxContentStyle;
    
    static PrintConstant()
    {
        ItemTitleStyle = TextStyle.Default.FontSize(8)
            .FontColor(Colors.Black)
            .FontFamily(OpenSans);

        ItemContentStyle = TextStyle.Default.FontSize(10).Bold()
            .FontColor(Colors.Black)
            .FontFamily(OpenSans)
            .Bold();

        BoxTitleStyle = TextStyle.Default.FontSize(5)
            .FontColor(Colors.Black)
            .FontFamily(UbuntuCondensed);

        BoxContentStyle = TextStyle.Default.FontSize(8)
            .FontColor(Colors.Black)
            .FontFamily(UbuntuCondensed)
            .Bold();
    }
}