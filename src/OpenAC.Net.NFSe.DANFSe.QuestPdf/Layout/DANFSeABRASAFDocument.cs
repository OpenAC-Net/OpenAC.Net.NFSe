using System;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.DANFSe.QuestPdf.Commom;
using OpenAC.Net.NFSe.Nota;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.DANFSe.QuestPdf.Layout;

/// <summary>
/// 
/// </summary>
internal class DANFSeABRASAFDocument : IDocument
{
    #region Fields

    private QuestPdfDANFSeOptions options;
    private NotaServico nota;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="nota"></param>
    public DANFSeABRASAFDocument(QuestPdfDANFSeOptions options, NotaServico nota)
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
        settings.CompressDocument = true;

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
            column.Item().Border(PrintConstant.BorderSize).Height(2.1f, Unit.Centimetre).Row(row =>
            {
                var logoPrefeitura = row.ConstantItem(2.9f, Unit.Centimetre)
                    .Padding(1, Unit.Millimetre);

                if (options.LogoPrefeitura == null)
                    logoPrefeitura.Placeholder();
                else
                    logoPrefeitura.Image(options.LogoPrefeitura.ToStream()).FitArea();

                row.RelativeItem().Width(11.6f, Unit.Centimetre)
                    .PaddingHorizontal(2)
                    .PaddingVertical(1)
                    .Column(headerColumn =>
                    {
                        headerColumn.Item().PaddingHorizontal(2)
                            .Text($"SECRETARIA DO MUNICÍPIO DE {options.NFSe.WebServices.Municipio.ToUpper()}")
                            .AlignCenter()
                            .Style(TextStyle.Default.FontSize(14).Bold()
                                .FontColor(Colors.Black)
                                .FontFamily(PrintConstant.OpenSans));

                        headerColumn.Item().Text("Secretaria Municipal de Finanças")
                            .AlignCenter()
                            .Style(TextStyle.Default.FontSize(9).Bold()
                                .FontColor(Colors.Black)
                                .FontFamily(PrintConstant.OpenSans));

                        headerColumn.Item().Text("NOTA FISCAL DE SERVIÇOS ELETRÔNICA - NFS-e")
                            .AlignCenter()
                            .Style(TextStyle.Default.FontSize(9).Bold()
                                .FontColor(Colors.Black)
                                .FontFamily(PrintConstant.OpenSans));

                        headerColumn.Item()
                            .Text(
                                $"RPS Nº {nota.IdentificacaoRps.Numero} Série {nota.IdentificacaoRps.Serie}, emitido em {nota.IdentificacaoRps.DataEmissao:dd/MM/yyyy}")
                            .AlignCenter()
                            .Style(TextStyle.Default.FontSize(8)
                                .FontColor(Colors.Black)
                                .FontFamily(PrintConstant.OpenSans));
                    });

                row.ConstantItem(4.2f, Unit.Centimetre)
                    .Column(dadosNota =>
                    {
                        dadosNota.Item()
                            .MinHeight(0.7f, Unit.Centimetre)
                            .BorderLeft(PrintConstant.BorderSize)
                            .BorderBottom(PrintConstant.BorderSize)
                            .Column(c =>
                            {
                                c.Item()
                                    .PaddingTop(1)
                                    .PaddingLeft(1)
                                    .Text("NÚMERO NOTA")
                                    .AlignStart()
                                    .Style(PrintConstant.BoxTitleStyle);

                                c.Item().Text(nota.IdentificacaoNFSe.Numero)
                                    .AlignCenter()
                                    .Style(PrintConstant.BoxContentStyle);
                            });

                        dadosNota.Item()
                            .MinHeight(0.7f, Unit.Centimetre)
                            .BorderLeft(PrintConstant.BorderSize)
                            .BorderBottom(PrintConstant.BorderSize)
                            .Column(c =>
                            {
                                c.Item()
                                    .PaddingTop(1)
                                    .PaddingLeft(1)
                                    .Text("DATA E HORA DA EMISSÃO")
                                    .AlignStart()
                                    .Style(PrintConstant.BoxTitleStyle);

                                c.Item().Text(nota.IdentificacaoNFSe.DataEmissao.ToString("dd/MM/yyyy HH:mm"))
                                    .AlignCenter()
                                    .Style(PrintConstant.BoxContentStyle);
                            });

                        dadosNota.Item()
                            .MinHeight(0.7f, Unit.Centimetre)
                            .BorderLeft(PrintConstant.BorderSize)
                            .Column(c =>
                            {
                                c.Item()
                                    .PaddingTop(1)
                                    .PaddingLeft(1)
                                    .Text("CÓDIGO DE VERIFICAÇÃO")
                                    .AlignStart()
                                    .Style(PrintConstant.BoxTitleStyle);

                                c.Item().Text(nota.IdentificacaoNFSe.Chave)
                                    .AlignCenter()
                                    .Style(PrintConstant.BoxContentStyle);
                            });
                    });
            });

            // Dados Prestador
            column.Item().ShowOnce().Border(PrintConstant.BorderSize).Column(prestador =>
            {
                prestador.Item()
                    .Padding(2)
                    .Text("PRESTADOR DE SERVIÇOS")
                    .AlignCenter()
                    .Style(TextStyle.Default.FontSize(10).Bold()
                        .FontColor(Colors.Black)
                        .FontFamily(PrintConstant.OpenSans));

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
                            row1.ConstantItem(PrintConstant.TitleSize, Unit.Centimetre)
                                .Text("CPF / CNPJ").Style(PrintConstant.ItemTitleStyle);

                            row1.RelativeItem()
                                .Text(nota.Prestador.CpfCnpj.FormataCPFCNPJ())
                                .Style(PrintConstant.ItemContentStyle);

                            row1.ConstantItem(3.3f, Unit.Centimetre)
                                .Text("INSCRIÇÃO MUNICIPAL")
                                .Style(PrintConstant.ItemTitleStyle);

                            row1.ConstantItem(3.6f, Unit.Centimetre)
                                .Text(nota.Prestador.InscricaoMunicipal)
                                .Style(PrintConstant.ItemContentStyle);
                        });

                        dadosprestador.Item().Row(row1 =>
                        {
                            row1.ConstantItem(PrintConstant.TitleSize, Unit.Centimetre)
                                .Text("NOME / RAZÃO").Style(PrintConstant.ItemTitleStyle);

                            row1.RelativeItem()
                                .Text(nota.Prestador.RazaoSocial)
                                .Style(PrintConstant.ItemContentStyle);
                        });

                        dadosprestador.Item().Row(row1 =>
                        {
                            row1.ConstantItem(PrintConstant.TitleSize, Unit.Centimetre)
                                .Text("ENDEREÇO").Style(PrintConstant.ItemTitleStyle);

                            row1.RelativeItem()
                                .Text($"{nota.Prestador.Endereco.Logradouro}, {nota.Prestador.Endereco.Numero}")
                                .Style(PrintConstant.ItemContentStyle);
                        });

                        dadosprestador.Item().Row(row1 =>
                        {
                            row1.ConstantItem(PrintConstant.TitleSize, Unit.Centimetre)
                                .Text("MUNICÍPIO").Style(PrintConstant.ItemTitleStyle);

                            row1.RelativeItem()
                                .Text(nota.Prestador.Endereco.Municipio)
                                .Style(PrintConstant.ItemContentStyle);

                            row1.ConstantItem(1.5f, Unit.Centimetre)
                                .Text("TELEFONE")
                                .Style(PrintConstant.ItemTitleStyle);

                            row1.ConstantItem(3, Unit.Centimetre)
                                .Text(nota.Prestador.DadosContato.Telefone)
                                .Style(PrintConstant.ItemContentStyle);
                        });

                        dadosprestador.Item().Row(row1 =>
                        {
                            row1.ConstantItem(PrintConstant.TitleSize, Unit.Centimetre)
                                .Text("COMPLEMENTO")
                                .Style(PrintConstant.ItemTitleStyle);

                            row1.RelativeItem()
                                .Text(nota.Prestador.Endereco.Complemento)
                                .Style(PrintConstant.ItemContentStyle);
                        });
                    });
                });
            });

            // Dados Tomador
            column.Item().ShowOnce().Border(PrintConstant.BorderSize).Column(tomador =>
            {
                tomador.Item()
                    .Padding(2)
                    .Text("TOMADOR DE SERVIÇOS")
                    .AlignCenter()
                    .Style(TextStyle.Default.FontSize(10).Bold()
                        .FontColor(Colors.Black)
                        .FontFamily(PrintConstant.OpenSans));

                tomador.Item().Padding(2).Column(dadosTomador =>
                {
                    dadosTomador.Item().Row(row1 =>
                    {
                        row1.ConstantItem(PrintConstant.TitleSize, Unit.Centimetre)
                            .Text("CPF / CNPJ").Style(PrintConstant.ItemTitleStyle);

                        row1.RelativeItem()
                            .Text(nota.Tomador.CpfCnpj.FormataCPFCNPJ())
                            .Style(PrintConstant.ItemContentStyle);

                        row1.ConstantItem(3.3f, Unit.Centimetre)
                            .Text("INSCRIÇÃO MUNICIPAL")
                            .Style(PrintConstant.ItemTitleStyle);

                        row1.ConstantItem(3.6f, Unit.Centimetre)
                            .Text(nota.Tomador.InscricaoMunicipal)
                            .Style(PrintConstant.ItemContentStyle);
                    });

                    dadosTomador.Item().Row(row1 =>
                    {
                        row1.ConstantItem(PrintConstant.TitleSize, Unit.Centimetre)
                            .Text("NOME / RAZÃO").Style(PrintConstant.ItemTitleStyle);

                        row1.RelativeItem()
                            .Text(nota.Tomador.RazaoSocial)
                            .Style(PrintConstant.ItemContentStyle);
                    });

                    dadosTomador.Item().Row(row1 =>
                    {
                        row1.ConstantItem(PrintConstant.TitleSize, Unit.Centimetre)
                            .Text("ENDEREÇO").Style(PrintConstant.ItemTitleStyle);

                        row1.RelativeItem()
                            .Text($"{nota.Tomador.Endereco.Logradouro}, {nota.Tomador.Endereco.Numero}")
                            .Style(PrintConstant.ItemContentStyle);
                    });

                    dadosTomador.Item().Row(row1 =>
                    {
                        row1.ConstantItem(PrintConstant.TitleSize, Unit.Centimetre)
                            .Text("MUNICÍPIO").Style(PrintConstant.ItemTitleStyle);

                        row1.RelativeItem()
                            .Text(nota.Tomador.Endereco.Municipio)
                            .Style(PrintConstant.ItemContentStyle);

                        row1.ConstantItem(1.5f, Unit.Centimetre)
                            .Text("TELEFONE")
                            .Style(PrintConstant.ItemTitleStyle);

                        row1.ConstantItem(3, Unit.Centimetre)
                            .Text(nota.Tomador.DadosContato.Telefone)
                            .Style(PrintConstant.ItemContentStyle);
                    });

                    dadosTomador.Item().Row(row1 =>
                    {
                        row1.ConstantItem(PrintConstant.TitleSize, Unit.Centimetre)
                            .Text("COMPLEMENTO")
                            .Style(PrintConstant.ItemTitleStyle);

                        row1.RelativeItem()
                            .Text(nota.Tomador.Endereco.Complemento)
                            .Style(PrintConstant.ItemContentStyle);
                    });
                });
            });

            // Dados Prestação
            column.Item().ShowOnce().Border(PrintConstant.BorderSize).Row(dados =>
            {
                dados.RelativeItem()
                    .Column(c =>
                    {
                        c.Item()
                            .PaddingTop(1)
                            .PaddingLeft(1)
                            .Text("LOCAL DA PRESTAÇÃO DO(S) SERVIÇO(S)")
                            .AlignStart()
                            .Style(PrintConstant.BoxTitleStyle);

                        c.Item().Text(nota.Servico.CodigoMunicipio.ToString())
                            .AlignCenter()
                            .Style(PrintConstant.BoxContentStyle);
                    });

                dados.RelativeItem()
                    .BorderLeft(PrintConstant.BorderSize)
                    .Column(c =>
                    {
                        c.Item()
                            .PaddingTop(1)
                            .PaddingLeft(1)
                            .Text("LOCAL DA INCIDÊNCIA DO(S) SERVIÇO(S)")
                            .AlignStart()
                            .Style(PrintConstant.BoxTitleStyle);

                        c.Item().Text(nota.Servico.MunicipioIncidencia.ToString())
                            .AlignCenter()
                            .Style(PrintConstant.BoxContentStyle);
                    });
            });
        });
    }

    private void ComposeBody(IContainer container)
    {
        container.Column(body =>
        {
            body.Item()
                .Border(PrintConstant.BorderSize)
                .Text("DISCRIMINAÇÃO DOS SERVIÇOS")
                .AlignCenter()
                .Style(TextStyle.Default
                    .FontSize(10)
                    .FontFamily(PrintConstant.OpenSans)
                    .Bold());

            body.Item()
                .Border(PrintConstant.BorderSize)
                .ScaleToFit()
                .MinHeight(14f, Unit.Centimetre)
                .Padding(2)
                .Text(nota.Servico.Discriminacao)
                .Justify()
                .Style(TextStyle.Default
                    .FontSize(9)
                    .FontFamily(PrintConstant.OpenSans));
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(footer =>
        {
            footer.Item()
                .Border(PrintConstant.BorderSize)
                .Text($"VALOR TOTAL DA NOTA = {nota.Servico.Valores.ValorServicos:c2}")
                .AlignCenter()
                .Style(TextStyle.Default
                    .FontSize(10)
                    .FontFamily(PrintConstant.OpenSans)
                    .Bold());

            footer.Item()
                .Border(PrintConstant.BorderSize)
                .Padding(2)
                .Column(c =>
                {
                    c.Item()
                        .Text("CÓDIGO DE CLASSIFICAÇÃO DO SERVIÇO")
                        .AlignStart()
                        .Style(PrintConstant.BoxTitleStyle);

                    c.Item()
                        .Text($"{nota.Servico.CodigoCnae} - {nota.Servico.ItemListaServico}")
                        .AlignStart()
                        .Style(PrintConstant.BoxContentStyle);
                });

            footer.Item()
                .Border(PrintConstant.BorderSize)
                .Row(row =>
                {
                    const float boxSize = 3.8f;

                    row.ConstantItem(boxSize, Unit.Centimetre)
                        .Border(PrintConstant.BorderSize)
                        .Column(c =>
                        {
                            c.Item()
                                .BorderBottom(PrintConstant.BorderSize)
                                .Text("PIS")
                                .AlignCenter()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text($"{nota.Servico.Valores.ValorPis:C2}")
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });

                    row.ConstantItem(boxSize, Unit.Centimetre)
                        .Border(PrintConstant.BorderSize)
                        .Column(c =>
                        {
                            c.Item()
                                .BorderBottom(PrintConstant.BorderSize)
                                .Text("COFINS")
                                .AlignCenter()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text($"{nota.Servico.Valores.ValorCofins:C2}")
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });

                    row.ConstantItem(boxSize, Unit.Centimetre)
                        .Border(PrintConstant.BorderSize)
                        .Column(c =>
                        {
                            c.Item()
                                .BorderBottom(PrintConstant.BorderSize)
                                .Text("IMPOSTO DE RENDA")
                                .AlignCenter()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text($"{nota.Servico.Valores.ValorIr:C2}")
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });

                    row.ConstantItem(boxSize, Unit.Centimetre)
                        .Border(PrintConstant.BorderSize)
                        .Column(c =>
                        {
                            c.Item()
                                .BorderBottom(PrintConstant.BorderSize)
                                .Text("INSS")
                                .AlignCenter()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text($"{nota.Servico.Valores.ValorInss:C2}")
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });

                    row.RelativeItem()
                        .Border(PrintConstant.BorderSize)
                        .Column(c =>
                        {
                            c.Item()
                                .BorderBottom(PrintConstant.BorderSize)
                                .Text("CSLL")
                                .AlignCenter()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text($"{nota.Servico.Valores.ValorCsll:C2}")
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });
                });

            footer.Item()
                .Border(PrintConstant.BorderSize)
                .Row(row =>
                {
                    const float boxSize = 4.75f;

                    row.ConstantItem(boxSize, Unit.Centimetre)
                        .Border(PrintConstant.BorderSize)
                        .Column(c =>
                        {
                            c.Item()
                                .BorderBottom(PrintConstant.BorderSize)
                                .Text("VALOR DEDUÇÃO")
                                .AlignCenter()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text($"{nota.Servico.Valores.ValorDeducoes:C2}")
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });

                    row.ConstantItem(boxSize, Unit.Centimetre)
                        .Border(PrintConstant.BorderSize)
                        .Column(c =>
                        {
                            c.Item()
                                .BorderBottom(PrintConstant.BorderSize)
                                .Text("DESCONTO INCONDICIONADO")
                                .AlignCenter()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text($"{nota.Servico.Valores.DescontoIncondicionado:C2}")
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });

                    row.ConstantItem(boxSize, Unit.Centimetre)
                        .Border(PrintConstant.BorderSize)
                        .Column(c =>
                        {
                            c.Item()
                                .BorderBottom(PrintConstant.BorderSize)
                                .Text("DESCONTO CONDICIONADO")
                                .AlignCenter()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text($"{nota.Servico.Valores.DescontoCondicionado:C2}")
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });

                    row.RelativeItem()
                        .Border(PrintConstant.BorderSize)
                        .Column(c =>
                        {
                            c.Item()
                                .BorderBottom(PrintConstant.BorderSize)
                                .Text("OUTRAS RENTENÇÕES")
                                .AlignCenter()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text($"{nota.Servico.Valores.OutrasRetencoes:C2}")
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });
                });

            footer.Item()
                .Border(PrintConstant.BorderSize)
                .Row(row =>
                {
                    const float boxSize = 3.8f;

                    row.ConstantItem(boxSize, Unit.Centimetre)
                        .Border(PrintConstant.BorderSize)
                        .Column(c =>
                        {
                            c.Item()
                                .BorderBottom(PrintConstant.BorderSize)
                                .Text("TOTAL LÍQUIDO DA NOTA")
                                .AlignCenter()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text($"{nota.Servico.Valores.ValorLiquidoNfse:C2}")
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });

                    row.ConstantItem(boxSize, Unit.Centimetre)
                        .Border(PrintConstant.BorderSize)
                        .Column(c =>
                        {
                            c.Item()
                                .BorderBottom(PrintConstant.BorderSize)
                                .Text("BASE DE CÁLCULO ISS")
                                .AlignCenter()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text($"{nota.Servico.Valores.BaseCalculo:C2}")
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });

                    row.ConstantItem(boxSize, Unit.Centimetre)
                        .Border(PrintConstant.BorderSize)
                        .Column(c =>
                        {
                            c.Item()
                                .BorderBottom(PrintConstant.BorderSize)
                                .Text("ALÍQUOTA ISS (%)")
                                .AlignCenter()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text($"{nota.Servico.Valores.Aliquota:N2}")
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });

                    row.ConstantItem(boxSize, Unit.Centimetre)
                        .Border(PrintConstant.BorderSize)
                        .Column(c =>
                        {
                            c.Item()
                                .BorderBottom(PrintConstant.BorderSize)
                                .Text("VALOR DO ISS")
                                .AlignCenter()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text($"{nota.Servico.Valores.ValorIss:C2}")
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });

                    row.RelativeItem()
                        .Border(PrintConstant.BorderSize)
                        .Column(c =>
                        {
                            c.Item()
                                .BorderBottom(PrintConstant.BorderSize)
                                .Text("VALOR DO ISS RETIDO")
                                .AlignCenter()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text($"{nota.Servico.Valores.ValorIssRetido:C2}")
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });
                });

            footer.Item()
                .Border(PrintConstant.BorderSize)
                .Column(column =>
                {
                    column.Item()
                        .Text("OUTRAS INFORMAÇÕES")
                        .AlignCenter()
                        .Style(TextStyle.Default
                            .FontSize(10)
                            .FontFamily(PrintConstant.OpenSans)
                            .Bold());

                    column.Item()
                        .MinHeight(2.3f, Unit.Centimetre)
                        .Text(nota.OutrasInformacoes)
                        .Justify()
                        .Style(TextStyle.Default
                            .FontSize(8)
                            .FontFamily(PrintConstant.OpenSans)
                            .Bold());
                });

            footer.Item()
                .Border(PrintConstant.BorderSize)
                .Row(row =>
                {
                    row.RelativeItem()
                        .PaddingHorizontal(2)
                        .BorderRight(PrintConstant.BorderSize)
                        .Text($"Data e Hora da Impressão: {DateTime.Now:dd/MM/yyyy hh:mm:ss}")
                        .AlignStart()
                        .Style(TextStyle.Default
                            .FontSize(7)
                            .FontFamily(PrintConstant.OpenSans));

                    row.RelativeItem()
                        .PaddingHorizontal(2)
                        .Text(options.SoftwareHouse)
                        .AlignEnd()
                        .Style(TextStyle.Default
                            .FontSize(7)
                            .FontFamily(PrintConstant.OpenSans));
                });

            footer.Item()
                .Border(PrintConstant.BorderSize)
                .Row(row =>
                {
                    row.RelativeItem()
                        .Column(column =>
                        {
                            column.Item()
                                .Padding(2)
                                .Text($"Recebi(emos) de {nota.Prestador.RazaoSocial}\nos serviços constantes da Nota Fiscal Eletrônica de Serviço (NFSe) ao lado.")
                                .Justify()
                                .Style(TextStyle.Default.FontSize(8)
                                    .FontFamily(PrintConstant.OpenSans));

                            column.Item()
                                .PaddingTop(3)
                                .Row(r =>
                                {
                                    r.RelativeItem()
                                        .AlignCenter()
                                        .Text("       /                 /           \nDATA")
                                        .AlignCenter()
                                        .FontSize(8)
                                        .FontFamily(PrintConstant.OpenSans);

                                    r.RelativeItem()
                                        .AlignCenter()
                                        .Text("_____________________________________________\nIdentificação e Assinatura do Recebedor")
                                        .AlignCenter()
                                        .FontSize(8)
                                        .FontFamily(PrintConstant.OpenSans);
                                });
                        });
                    
                    row.ConstantItem(4.9f, Unit.Centimetre)
                        .BorderLeft(PrintConstant.BorderSize)
                        .Padding(2)
                        .Column(c =>
                        {
                            c.Item()
                                .Text("NÚMERO NOTA")
                                .AlignStart()
                                .Style(PrintConstant.BoxTitleStyle);

                            c.Item()
                                .Text(nota.IdentificacaoNFSe.Numero)
                                .AlignCenter()
                                .Style(PrintConstant.BoxContentStyle);
                        });
                });
        });
    }

    #endregion Methods
}