using System;
using System.IO;

namespace OpenAC.Net.NFSe.DANFSe.QuestPdf.Extensions;

/// <summary>
/// 
/// </summary>
public static class QuestPdfDANFSeExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="nfse"></param>
    /// <param name="options"></param>
    public static void Imprimir(this OpenNFSe nfse, Action<QuestPdfDANFSeOptions> options = null)
    {
        var danfse = new QuestPdfDANFSe(nfse.Configuracoes);
        options?.Invoke(danfse.Configuracoes);
        danfse.Imprimir(nfse.NotasServico.ToArray());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nfse"></param>
    /// <param name="options"></param>
    public static void ImprimirPDF(this OpenNFSe nfse, Action<QuestPdfDANFSeOptions> options = null)
    {
        var danfse = new QuestPdfDANFSe(nfse.Configuracoes);
        options?.Invoke(danfse.Configuracoes);
        danfse.ImprimirPDF(nfse.NotasServico.ToArray());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nfse"></param>
    /// <param name="aStream"></param>
    /// <param name="options"></param>
    public static void ImprimirPDF(this OpenNFSe nfse, Stream aStream, Action<QuestPdfDANFSeOptions> options = null)
    {
        var danfse = new QuestPdfDANFSe(nfse.Configuracoes);
        options?.Invoke(danfse.Configuracoes);
        danfse.ImprimirPDF(nfse.NotasServico.ToArray(), aStream);
    }
}