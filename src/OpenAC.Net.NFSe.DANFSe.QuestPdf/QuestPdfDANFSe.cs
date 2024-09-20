using System;
using System.IO;
using System.Linq;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using QuestPDF.Fluent;
using QuestPDF.Previewer;

namespace OpenAC.Net.NFSe.DANFSe.QuestPdf;

public class QuestPdfDANFSe : OpenDANFSeBase<QuestPdfDANFSeOptions, FiltroDFeReport>
{
    #region Constructors

    public QuestPdfDANFSe(ConfigNFSe config)
    {
        Configuracoes = new QuestPdfDANFSeOptions(config);
    }
    
    #endregion Constructors
    
    #region Methods

    public override async void Imprimir(NotaServico[] notas)
    {
        var document = Document.Merge(notas.Select(x => new DANFSeDocument(Configuracoes, x)));
        await document.ShowInPreviewerAsync();
    }

    public override void ImprimirPDF(NotaServico[] notas)
    {
        throw new NotImplementedException();
    }

    public override void ImprimirPDF(NotaServico[] notas, Stream stream)
    {
        throw new NotImplementedException();
    }

    public override void ImprimirHTML(NotaServico[] notas) => new NotImplementedException();

    public override void ImprimirHTML(NotaServico[] notas, Stream stream) => throw new NotImplementedException();

    #endregion Methods
}