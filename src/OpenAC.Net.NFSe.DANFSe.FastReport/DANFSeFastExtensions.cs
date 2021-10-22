using System;

namespace OpenAC.Net.NFSe.DANFSe.FastReport
{
    public static class DANFSeFastExtensions
    {
        public static void Imprimir(this OpenNFSe nfse, Action<IDANFSeOptions> options = null)
        {
            var danfse = new DANFSeFastReport(nfse.Configuracoes);
            options?.Invoke(danfse);
            danfse.Imprimir(nfse.NotasServico.ToArray());
        }

        public static void ImprimirPDF(this OpenNFSe nfse, Action<IDANFSeOptions> options = null)
        {
            var danfse = new DANFSeFastReport(nfse.Configuracoes);
            options?.Invoke(danfse);
            danfse.ImprimirPDF(nfse.NotasServico.ToArray());
        }

        public static void ImprimirHTML(this OpenNFSe nfse, Action<IDANFSeOptions> options = null)
        {
            var danfse = new DANFSeFastReport(nfse.Configuracoes);
            options?.Invoke(danfse);
            danfse.ImprimirHTML(nfse.NotasServico.ToArray());
        }
    }
}