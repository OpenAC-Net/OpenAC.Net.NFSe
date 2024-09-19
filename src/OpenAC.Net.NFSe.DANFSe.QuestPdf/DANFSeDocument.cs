using System;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Nota;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.DANFSe.QuestPdf;

/// <summary>
/// 
/// </summary>
public class DANFSeDocument : IDocument
{
    #region Fields

    private QuestPdfDANFSeOptions options;
    private NotaServico nota;

    private const string OpenSans = "Open Sans";
    private const string UbuntuCondensed = "Ubuntu Condensed";
    private const float BorderSize = 0.5f;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="nota"></param>
    public DANFSeDocument(QuestPdfDANFSeOptions options, NotaServico nota)
    {
        this.options = options;
        this.nota = nota;
    }

    #endregion Constructors

    #region Methods

    /// <inheritdoc />
    public DocumentMetadata GetMetadata()
    {
        var metadata = DocumentMetadata.Default;
        metadata.Author = "OpenAC.Net";
        metadata.Creator = "OpenAC.Net";
        metadata.Producer = "OpenAC.Net";
        metadata.CreationDate = DateTimeOffset.Now;
        metadata.ModifiedDate = DateTimeOffset.Now;

        metadata.Keywords = "DANFSe Nota Fiscal Serviço";
        metadata.Title = "DANFSe - Documento Auxiliar da Nota Fiscal de Serviço Eletrônica";

        return metadata;
    }

    /// <inheritdoc />
    public DocumentSettings GetSettings()
    {
        var settings = DocumentSettings.Default;
        settings.PdfA = true;

        return settings;
    }

    /// <inheritdoc />
    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);

                page.Header().Element(ComposeHeader);
            });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Border(BorderSize).Height(2.1f, Unit.Centimetre).Row(row =>
            {
                var logoPrefeitura = row.ConstantItem(2.9f, Unit.Centimetre)
                    .Padding(1, Unit.Millimetre);

                if (options.LogoPrefeitura == null)
                    logoPrefeitura.Placeholder();
                else
                    logoPrefeitura.Image(options.LogoPrefeitura.ToStream()).FitArea();

                row.RelativeItem().Width(11.6f, Unit.Centimetre)
                    .PaddingHorizontal(2).PaddingVertical(1)
                    .Column(headerColumn =>
                    {
                        headerColumn.Item().PaddingHorizontal(2)
                            .Text($"SECRETARIA DO MUNICÍPIO DE {options.NFSe.WebServices.Municipio.ToUpper()}")
                            .AlignCenter()
                            .Style(TextStyle.Default.FontSize(14).Bold()
                                .FontColor(Colors.Black)
                                .FontFamily(OpenSans));

                        headerColumn.Item().Text("Secretaria Municipal de Finanças")
                            .AlignCenter()
                            .Style(TextStyle.Default.FontSize(9).Bold()
                                .FontColor(Colors.Black)
                                .FontFamily(OpenSans));

                        headerColumn.Item().Text("NOTA FISCAL DE SERVIÇOS ELETRÔNICA - NFS-e")
                            .AlignCenter()
                            .Style(TextStyle.Default.FontSize(9).Bold()
                                .FontColor(Colors.Black)
                                .FontFamily(OpenSans));

                        headerColumn.Item()
                            .Text(
                                $"RPS Nº {nota.IdentificacaoRps.Numero} Série {nota.IdentificacaoRps.Serie}, emitido em {nota.IdentificacaoRps.DataEmissao:dd/MM/yyyy}")
                            .AlignCenter()
                            .Style(TextStyle.Default.FontSize(8)
                                .FontColor(Colors.Black)
                                .FontFamily(OpenSans));
                    });

                row.ConstantItem(4.2f, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Item()
                            .MinHeight(0.7f, Unit.Centimetre)
                            .BorderLeft(BorderSize)
                            .BorderBottom(BorderSize)
                            .Column(c =>
                            {
                                c.Item()
                                    .PaddingTop(1)
                                    .PaddingLeft(1)
                                    .Text("NÚMERO NOTA")
                                    .AlignStart()
                                    .Style(TextStyle.Default.FontSize(5)
                                        .FontColor(Colors.Black)
                                        .FontFamily(UbuntuCondensed));

                                c.Item().Text(nota.IdentificacaoNFSe.Numero)
                                    .AlignCenter()
                                    .Style(TextStyle.Default.FontSize(8)
                                        .FontColor(Colors.Black)
                                        .FontFamily(UbuntuCondensed)
                                        .Bold());
                            });

                        column.Item()
                            .MinHeight(0.7f, Unit.Centimetre)
                            .BorderLeft(BorderSize)
                            .BorderBottom(BorderSize)
                            .Column(c =>
                            {
                                c.Item()
                                    .PaddingTop(1)
                                    .PaddingLeft(1)
                                    .Text("DATA E HORA DA EMISSÃO")
                                    .AlignStart()
                                    .Style(TextStyle.Default.FontSize(5)
                                        .FontColor(Colors.Black)
                                        .FontFamily(UbuntuCondensed));

                                c.Item().Text(nota.IdentificacaoNFSe.DataEmissao.ToString("dd/MM/yyyy HH:mm"))
                                    .AlignCenter()
                                    .Style(TextStyle.Default.FontSize(8)
                                        .FontColor(Colors.Black)
                                        .FontFamily(UbuntuCondensed)
                                        .Bold());
                            });

                        column.Item()
                            .MinHeight(0.7f, Unit.Centimetre)
                            .BorderLeft(BorderSize)
                            .Column(c =>
                            {
                                c.Item()
                                    .PaddingTop(1)
                                    .PaddingLeft(1)
                                    .Text("CÓDIGO DE VERIFICAÇÃO")
                                    .AlignStart()
                                    .Style(TextStyle.Default.FontSize(5)
                                        .FontColor(Colors.Black)
                                        .FontFamily(UbuntuCondensed));

                                c.Item().Text(nota.IdentificacaoNFSe.Chave)
                                    .AlignCenter()
                                    .Style(TextStyle.Default.FontSize(8)
                                        .FontColor(Colors.Black)
                                        .FontFamily(UbuntuCondensed)
                                        .Bold());
                            });
                    });
            });

            column.Item().Border(BorderSize).MinHeight(3.12f, Unit.Centimetre).Column(prestador =>
            {
                prestador.Item()
                    .Padding(2)
                    .Text("PRESTADOR DE SERVIÇOS")
                    .AlignCenter()
                    .Style(TextStyle.Default.FontSize(10).Bold()
                        .FontColor(Colors.Black)
                        .FontFamily(OpenSans));

                prestador.Item().Row(row =>
                {
                    var logo = row.ConstantItem(2.9f, Unit.Centimetre)
                        .Padding(1, Unit.Millimetre);

                    if (options.Logo == null)
                        logo.Placeholder();
                    else
                        logo.Image(options.Logo.ToStream()).FitArea();

                    var titleStyle = TextStyle.Default.FontSize(6).Bold()
                        .FontColor(Colors.Black)
                        .FontFamily(OpenSans);
                    
                    var contentStyle = TextStyle.Default.FontSize(8).Bold()
                        .FontColor(Colors.Black)
                        .FontFamily(OpenSans)
                        .Bold();
                    row.RelativeItem().Column(dadosprestador =>
                    {
                        dadosprestador.Item().Row(row1 =>
                        {
                            
                        });
                    });
                });
            });
        });
    }

    #endregion Methods
}