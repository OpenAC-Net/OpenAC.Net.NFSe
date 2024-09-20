using System;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Nota;
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
    private const float TitleSize = 2f;
    private static readonly TextStyle ItemTitleStyle;
    private static readonly TextStyle ItemContentStyle;
    private static readonly TextStyle BoxTitleStyle;
    private static readonly TextStyle BoxContentStyle;

    #endregion Fields

    #region Constructors

    static DANFSeDocument()
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
                page.Content().Element(ComposeBody);
                page.Footer().Element(ComposeFooter);
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
                    .Column(dadosNota =>
                    {
                        dadosNota.Item()
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
                                    .Style(BoxTitleStyle);

                                c.Item().Text(nota.IdentificacaoNFSe.Numero)
                                    .AlignCenter()
                                    .Style(BoxContentStyle);
                            });

                        dadosNota.Item()
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
                                    .Style(BoxTitleStyle);

                                c.Item().Text(nota.IdentificacaoNFSe.DataEmissao.ToString("dd/MM/yyyy HH:mm"))
                                    .AlignCenter()
                                    .Style(BoxContentStyle);
                            });

                        dadosNota.Item()
                            .MinHeight(0.7f, Unit.Centimetre)
                            .BorderLeft(BorderSize)
                            .Column(c =>
                            {
                                c.Item()
                                    .PaddingTop(1)
                                    .PaddingLeft(1)
                                    .Text("CÓDIGO DE VERIFICAÇÃO")
                                    .AlignStart()
                                    .Style(BoxTitleStyle);

                                c.Item().Text(nota.IdentificacaoNFSe.Chave)
                                    .AlignCenter()
                                    .Style(BoxContentStyle);
                            });
                    });
            });

            // Dados Prestador
            column.Item().ShowOnce().Border(BorderSize).Column(prestador =>
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
                        .MinHeight(2.3f, Unit.Centimetre)
                        .Padding(1, Unit.Millimetre);

                    if (options.Logo == null)
                        logo.Placeholder();
                    else
                        logo.Image(options.Logo.ToStream()).FitArea();

                    row.RelativeItem().Column(dadosprestador =>
                    {
                        dadosprestador.Item().Row(row1 =>
                        {
                            row1.ConstantItem(TitleSize, Unit.Centimetre)
                                .Text("CPF / CNPJ").Style(ItemTitleStyle);

                            row1.RelativeItem()
                                .Text(nota.Prestador.CpfCnpj.FormataCPFCNPJ())
                                .Style(ItemContentStyle);

                            row1.ConstantItem(3, Unit.Centimetre)
                                .Text("INSCRIÇÃO MUNICIPAL")
                                .Style(ItemTitleStyle);

                            row1.ConstantItem(3.6f, Unit.Centimetre)
                                .Text(nota.Prestador.InscricaoMunicipal)
                                .Style(ItemContentStyle);
                        });

                        dadosprestador.Item().Row(row1 =>
                        {
                            row1.ConstantItem(TitleSize, Unit.Centimetre)
                                .Text("NOME / RAZÃO").Style(ItemTitleStyle);

                            row1.RelativeItem()
                                .Text(nota.Prestador.RazaoSocial)
                                .Style(ItemContentStyle);
                        });

                        dadosprestador.Item().Row(row1 =>
                        {
                            row1.ConstantItem(TitleSize, Unit.Centimetre)
                                .Text("ENDEREÇO").Style(ItemTitleStyle);

                            row1.RelativeItem()
                                .Text($"{nota.Prestador.Endereco.Logradouro}, {nota.Prestador.Endereco.Numero}")
                                .Style(ItemContentStyle);
                        });

                        dadosprestador.Item().Row(row1 =>
                        {
                            row1.ConstantItem(TitleSize, Unit.Centimetre)
                                .Text("MUNICÍPIO").Style(ItemTitleStyle);

                            row1.RelativeItem()
                                .Text(nota.Prestador.Endereco.Municipio)
                                .Style(ItemContentStyle);

                            row1.ConstantItem(1.5f, Unit.Centimetre)
                                .Text("TELEFONE")
                                .Style(ItemTitleStyle);

                            row1.ConstantItem(3, Unit.Centimetre)
                                .Text(nota.Prestador.DadosContato.Telefone)
                                .Style(ItemContentStyle);
                        });

                        dadosprestador.Item().Row(row1 =>
                        {
                            row1.ConstantItem(TitleSize, Unit.Centimetre)
                                .Text("COMPLEMENTO")
                                .Style(ItemTitleStyle);

                            row1.RelativeItem()
                                .Text(nota.Prestador.Endereco.Complemento)
                                .Style(ItemContentStyle);
                        });
                    });
                });
            });

            // Dados Tomador
            column.Item().ShowOnce().Border(BorderSize).Column(tomador =>
            {
                tomador.Item()
                    .Padding(2)
                    .Text("TOMADOR DE SERVIÇOS")
                    .AlignCenter()
                    .Style(TextStyle.Default.FontSize(10).Bold()
                        .FontColor(Colors.Black)
                        .FontFamily(OpenSans));

                tomador.Item().Padding(2).Column(dadosTomador =>
                {
                    dadosTomador.Item().Row(row1 =>
                    {
                        row1.ConstantItem(TitleSize, Unit.Centimetre)
                            .Text("CPF / CNPJ").Style(ItemTitleStyle);

                        row1.RelativeItem()
                            .Text(nota.Tomador.CpfCnpj.FormataCPFCNPJ())
                            .Style(ItemContentStyle);

                        row1.ConstantItem(3, Unit.Centimetre)
                            .Text("INSCRIÇÃO MUNICIPAL")
                            .Style(ItemTitleStyle);

                        row1.ConstantItem(3.6f, Unit.Centimetre)
                            .Text(nota.Tomador.InscricaoMunicipal)
                            .Style(ItemContentStyle);
                    });

                    dadosTomador.Item().Row(row1 =>
                    {
                        row1.ConstantItem(TitleSize, Unit.Centimetre)
                            .Text("NOME / RAZÃO").Style(ItemTitleStyle);

                        row1.RelativeItem()
                            .Text(nota.Tomador.RazaoSocial)
                            .Style(ItemContentStyle);
                    });

                    dadosTomador.Item().Row(row1 =>
                    {
                        row1.ConstantItem(TitleSize, Unit.Centimetre)
                            .Text("ENDEREÇO").Style(ItemTitleStyle);

                        row1.RelativeItem()
                            .Text($"{nota.Tomador.Endereco.Logradouro}, {nota.Tomador.Endereco.Numero}")
                            .Style(ItemContentStyle);
                    });

                    dadosTomador.Item().Row(row1 =>
                    {
                        row1.ConstantItem(TitleSize, Unit.Centimetre)
                            .Text("MUNICÍPIO").Style(ItemTitleStyle);

                        row1.RelativeItem()
                            .Text(nota.Tomador.Endereco.Municipio)
                            .Style(ItemContentStyle);

                        row1.ConstantItem(1.5f, Unit.Centimetre)
                            .Text("TELEFONE")
                            .Style(ItemTitleStyle);

                        row1.ConstantItem(3, Unit.Centimetre)
                            .Text(nota.Tomador.DadosContato.Telefone)
                            .Style(ItemContentStyle);
                    });

                    dadosTomador.Item().Row(row1 =>
                    {
                        row1.ConstantItem(TitleSize, Unit.Centimetre)
                            .Text("COMPLEMENTO")
                            .Style(ItemTitleStyle);

                        row1.RelativeItem()
                            .Text(nota.Tomador.Endereco.Complemento)
                            .Style(ItemContentStyle);
                    });
                });
            });
            
            // Dados Pestração
            column.Item().ShowOnce().Border(BorderSize).Row(dados =>
            {
                dados.RelativeItem()
                    .Column(c =>
                    {
                        c.Item()
                            .PaddingTop(1)
                            .PaddingLeft(1)
                            .Text("LOCAL DA PRESTAÇÃO DO(S) SERVIÇO(S)")
                            .AlignStart()
                            .Style(BoxTitleStyle);

                        c.Item().Text(nota.Servico.CodigoMunicipio.ToString())
                            .AlignCenter()
                            .Style(BoxContentStyle);
                    });
                
                dados.RelativeItem()
                    .BorderLeft(BorderSize)
                    .Column(c =>
                    {
                        c.Item()
                            .PaddingTop(1)
                            .PaddingLeft(1)
                            .Text("LOCAL DA INCIDÊNCIA DO(S) SERVIÇO(S)")
                            .AlignStart()
                            .Style(BoxTitleStyle);

                        c.Item().Text(nota.Servico.MunicipioIncidencia.ToString())
                            .AlignCenter()
                            .Style(BoxContentStyle);
                    });
            });
        });
    }

    private void ComposeBody(IContainer container)
    {
    }
    
    private void ComposeFooter(IContainer container)
    {
        
    }
    
    #endregion Methods
}