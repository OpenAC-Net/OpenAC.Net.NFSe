using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.DANFSe.ReportNative.Danfe
{
    public class DanfeInfo
    {
        /// <summary>
        /// Titulo do documento
        /// </summary>
        public string Titulo { get; set; }
        /// <summary>
        /// Nome da prefeitura
        /// </summary>
        public string NomePrefeitura { get; set; }
        /// <summary>
        /// Link da imagem da prefeitura
        /// </summary>
        public string LinkImagemLogo { get; set; }

        public NotaServico NotaServico { get; set; }
    }
}
