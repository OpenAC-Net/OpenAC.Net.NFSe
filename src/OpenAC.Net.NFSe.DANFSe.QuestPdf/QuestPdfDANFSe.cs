using System;
using System.IO;
using System.Linq;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.DANFSe.QuestPdf.Layout;
using OpenAC.Net.NFSe.Nota;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

namespace OpenAC.Net.NFSe.DANFSe.QuestPdf;

/// <summary>
/// 
/// </summary>
public class QuestPdfDANFSe : OpenDANFSeBase<QuestPdfDANFSeOptions, FiltroDFeReport>
{
    #region Constructors

    static QuestPdfDANFSe()
    {
        FontManager.RegisterFontFromEmbeddedResource("OpenAC.Net.NFSe.DANFSe.QuestPdf.Font.OpenSans-Regular.ttf");
        FontManager.RegisterFontFromEmbeddedResource("OpenAC.Net.NFSe.DANFSe.QuestPdf.Font.UbuntuCondensed-Regular.ttf");
    }
    
    /// <summary>
    /// 
    /// </summary>
    public QuestPdfDANFSe()
    {
        Configuracoes = new QuestPdfDANFSeOptions();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    public QuestPdfDANFSe(ConfigNFSe config)
    {
        Configuracoes = new QuestPdfDANFSeOptions(config);
    }
    
    #endregion Constructors
    
    #region Methods

    /// <inheritdoc />
    public override void Imprimir(NotaServico[] notas)
    {
        var document = GerarPdf(notas);
        document.GeneratePdfAndShow();
    }

    /// <inheritdoc />
    public override void ImprimirPDF(NotaServico[] notas)
    {
        var document = GerarPdf(notas);
        document.GeneratePdf(Configuracoes.NomeArquivo);
    }

    /// <inheritdoc />
    public override void ImprimirPDF(NotaServico[] notas, Stream stream)
    {
        var document = GerarPdf(notas);
        document.GeneratePdf(stream);
    }

    /// <inheritdoc />
    public override void ImprimirHTML(NotaServico[] notas) => throw new NotImplementedException();

    /// <inheritdoc />
    public override void ImprimirHTML(NotaServico[] notas, Stream stream) => throw new NotImplementedException();


    private IDocument GerarPdf(NotaServico[] notas) => Document.Merge(notas.Select(GetDocument));

    private IDocument GetDocument(NotaServico nota)
    {
        return Configuracoes.Layout switch
        {
            LayoutImpressao.ABRASF => new DANFSeABRASAFDocument(Configuracoes, nota),
            LayoutImpressao.ABRASF2 => new DANFSeABRASAFDocument(Configuracoes, nota),
            LayoutImpressao.DSF => new DANFSeABRASAFDocument(Configuracoes, nota),
            LayoutImpressao.Ginfes => new DANFSeABRASAFDocument(Configuracoes, nota),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    #endregion Methods
}