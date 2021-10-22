using System;

namespace OpenAC.Net.NFSe.DANFSe.FastReport
{
    public static class DANFSeFastExtensions
    {
        public static void Imprimir(this OpenNFSe nfse, Action<IDANFSeConfig> danfeConfig = null)
        {
            var danfse = new DANFSeFastReport(nfse.Configuracoes);
            danfeConfig?.Invoke(danfse);
            danfse.Imprimir(nfse.NotasServico.ToArray());
        }

        public static void ImprimirPDF(this OpenNFSe nfse, Action<IDANFSeConfig> danfeConfig = null)
        {
            var danfse = new DANFSeFastReport(nfse.Configuracoes);
            danfeConfig?.Invoke(danfse);
            danfse.ImprimirPDF(nfse.NotasServico.ToArray());
        }

        public static void ImprimirHTML(this OpenNFSe nfse, Action<IDANFSeConfig> danfeConfig = null)
        {
            var danfse = new DANFSeFastReport(nfse.Configuracoes);
            danfeConfig?.Invoke(danfse);
            danfse.ImprimirHTML(nfse.NotasServico.ToArray());
        }
    }
}