using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Web.Provider;

/// <summary>Define o contrato para carregar ou renovar o certificado digital de uma empresa.</summary>
/// <remarks>A implementação não deve compartilhar configuração mutável de certificado entre empresas.</remarks>
public interface INFSeCertificateProvider
{
    /// <summary>Preenche a configuração com o certificado vigente da empresa.</summary>
    /// <param name="tenantId">Identificador estável da empresa no sistema consumidor.</param>
    /// <param name="certificateConfiguration">Configuração exclusiva que receberá os bytes, caminho ou senha do certificado.</param>
    /// <param name="cancellationToken">Token para cancelar o carregamento ou a renovação.</param>
    /// <returns>Uma tarefa que representa a conclusão da configuração do certificado.</returns>
    ValueTask ConfigureAsync(string tenantId, ConfigCertificadosNFSe certificateConfiguration,
        CancellationToken cancellationToken = default);
}
