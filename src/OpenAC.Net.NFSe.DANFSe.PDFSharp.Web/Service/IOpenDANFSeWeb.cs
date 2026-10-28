using Microsoft.AspNetCore.Http;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Web.Document;
using OpenAC.Net.NFSe.Web;
using OpenAC.Net.NFSe.Web.Common;

namespace OpenAC.Net.NFSe.DANFSe.PDFSharp.Web.Service;

/// <summary>Gera DANFSe em memória e respostas de arquivo para aplicações ASP.NET Core.</summary>
public interface IOpenDANFSeWeb
{
    /// <summary>Gera o PDF de uma NFS-e.</summary>
    /// <param name="nota">NFS-e que será representada no DANFSe.</param>
    /// <param name="tenantId">Identificador da empresa ou <see cref="NFSeTenant.Padrao"/>.</param>
    /// <param name="cancellationToken">Token para cancelar a obtenção da configuração.</param>
    /// <returns>Documento PDF gerado.</returns>
    ValueTask<DANFSeWebDocument> GerarAsync(NotaServico nota,
        string tenantId = NFSeTenant.Padrao, CancellationToken cancellationToken = default);

    /// <summary>Gera um único PDF contendo várias NFS-e.</summary>
    /// <param name="notas">NFS-e que serão representadas no DANFSe.</param>
    /// <param name="tenantId">Identificador da empresa ou <see cref="NFSeTenant.Padrao"/>.</param>
    /// <param name="cancellationToken">Token para cancelar a obtenção da configuração.</param>
    /// <returns>Documento PDF gerado.</returns>
    ValueTask<DANFSeWebDocument> GerarAsync(NotaServico[] notas,
        string tenantId = NFSeTenant.Padrao, CancellationToken cancellationToken = default);

    /// <summary>Gera uma resposta HTTP contendo o PDF de uma NFS-e.</summary>
    /// <param name="nota">NFS-e que será representada no DANFSe.</param>
    /// <param name="tenantId">Identificador da empresa ou <see cref="NFSeTenant.Padrao"/>.</param>
    /// <param name="cancellationToken">Token para cancelar a obtenção da configuração.</param>
    /// <returns>Resultado HTTP pronto para Minimal APIs.</returns>
    ValueTask<IResult> GerarResultadoAsync(NotaServico nota,
        string tenantId = NFSeTenant.Padrao, CancellationToken cancellationToken = default);

    /// <summary>Gera uma resposta HTTP contendo um único PDF com várias NFS-e.</summary>
    /// <param name="notas">NFS-e que serão representadas no DANFSe.</param>
    /// <param name="tenantId">Identificador da empresa ou <see cref="NFSeTenant.Padrao"/>.</param>
    /// <param name="cancellationToken">Token para cancelar a obtenção da configuração.</param>
    /// <returns>Resultado HTTP pronto para Minimal APIs.</returns>
    ValueTask<IResult> GerarResultadoAsync(NotaServico[] notas,
        string tenantId = NFSeTenant.Padrao, CancellationToken cancellationToken = default);
}
