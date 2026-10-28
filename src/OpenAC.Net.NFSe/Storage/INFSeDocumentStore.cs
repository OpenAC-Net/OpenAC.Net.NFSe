using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Storage;

/// <summary>Armazena documentos fiscais e payloads técnicos sem acoplar providers ao filesystem.</summary>
public interface INFSeDocumentStore
{
    /// <summary>Persiste um documento produzido pela operação.</summary>
    /// <param name="document">Documento ou payload que será persistido.</param>
    /// <param name="configuration">Configuração fiscal que fornece os diretórios e regras de armazenamento.</param>
    /// <param name="cancellationToken">Token para cancelar a gravação assíncrona.</param>
    /// <returns>Uma tarefa que representa a conclusão da persistência.</returns>
    Task SaveAsync(NFSeDocument document, ConfigNFSe configuration,
        CancellationToken cancellationToken = default);
}
