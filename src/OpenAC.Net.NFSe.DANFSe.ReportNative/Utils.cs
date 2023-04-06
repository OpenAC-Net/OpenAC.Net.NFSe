using System; 
using System.IO;
using System.Text;

namespace OpenAC.Net.NFSe.DANFSe.ReportNative
{
    internal static class Utils
    {


        /// <summary>
        ///     Escrever arquivo em modo stream, assincronamente
        /// </summary>
        /// <param name="caminho">Caminho do arquivo</param>
        /// <param name="nomeArquivo">Nome do arquivo</param>
        /// <param name="conteudo">Conteudo</param>
        public static void EscreverArquivo(string caminho, string nomeArquivo, string conteudo)
        {
            if (caminho == null)
                throw new ArgumentNullException(nameof(caminho), "O caminho do arquivo deve ser informado");
            if (nomeArquivo == null)
                throw new ArgumentNullException(nameof(nomeArquivo), "O nome do arquivo deve ser informado");
            if (conteudo == null)
                throw new ArgumentNullException(nameof(conteudo), "O conteúdo do arquivo deve ser informado");
            CriarPastaSeNaoExistir(caminho);
            var encodedText = Encoding.UTF8.GetBytes(conteudo);
            var c1 = Path.Combine(caminho, nomeArquivo); // Combina caminho do arquivo como nome do arquivo
            using (var sourceStream = new FileStream(c1, FileMode.Append, FileAccess.Write, FileShare.None, 4096, true))
            {
                sourceStream.Write(encodedText, 0, encodedText.Length);
                sourceStream.Close();
            }
        }

        /// <summary>
        ///     Criar pasta no caminho indicador
        /// </summary>
        /// <param name="caminho">caminho do arquivo</param>
        public static bool CriarPasta(string caminho)
        {
            // Tentar criar diretorio
            Directory.CreateDirectory(caminho);
            return true;
        }

        /// <summary>
        ///     Criar pasta se não existir
        /// </summary>
        /// <param name="caminho">Caminho do arquivo</param>
        /// <returns></returns>
        public static void CriarPastaSeNaoExistir(string caminho)
        {
            if (!ExisteDiretorio(caminho)) CriarPasta(caminho);
        }

        /// <summary>
        ///     Verificar se o diretorio existe
        /// </summary>
        /// <param name="caminho">caminho do arquivo</param>
        /// <returns></returns>
        public static bool ExisteDiretorio(string caminho)
        {
            // Determinar se o diretorio existe
            if (Directory.Exists(caminho))
                return true;
            return false;
        }
    }
}
