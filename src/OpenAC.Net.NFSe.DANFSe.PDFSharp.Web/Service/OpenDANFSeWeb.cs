using Microsoft.AspNetCore.Http;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Web.Configuration;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Web.Document;
using OpenAC.Net.NFSe.Web.Common;
using OpenAC.Net.NFSe.Web.Provider;

namespace OpenAC.Net.NFSe.DANFSe.PDFSharp.Web.Service;

/// <summary>Implementação padrão do gerador de DANFSe para ASP.NET Core.</summary>
public sealed class OpenDANFSeWeb : IOpenDANFSeWeb
{
    private readonly INFSeConfigurationProvider<DANFSeWebOptions> configurationProvider;

    /// <summary>Inicializa o serviço com o provider de configuração da impressão.</summary>
    /// <param name="configurationProvider">Provider de configuração single-tenant ou multiempresa.</param>
    public OpenDANFSeWeb(
        INFSeConfigurationProvider<DANFSeWebOptions> configurationProvider)
    {
        this.configurationProvider = configurationProvider;
    }

    /// <inheritdoc />
    public async ValueTask<DANFSeWebDocument> GerarAsync(NotaServico nota,
        string tenantId = NFSeTenant.Padrao,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentNullException.ThrowIfNull(nota);
        var configuration = await ObterConfiguracaoAsync(tenantId, cancellationToken).ConfigureAwait(false);
        var gerador = new OpenDANFSePDFSharp(configuration.Configuracoes);
        return new DANFSeWebDocument(gerador.GerarPDF(nota),
            NormalizarNome(configuration.NomeArquivo(nota)));
    }

    /// <inheritdoc />
    public async ValueTask<DANFSeWebDocument> GerarAsync(NotaServico[] notas,
        string tenantId = NFSeTenant.Padrao,
        CancellationToken cancellationToken = default)
    {
        ValidarNotas(tenantId, notas);
        var configuration = await ObterConfiguracaoAsync(tenantId, cancellationToken).ConfigureAwait(false);
        var gerador = new OpenDANFSePDFSharp(configuration.Configuracoes);
        return new DANFSeWebDocument(gerador.GerarPDF(notas),
            NormalizarNome(configuration.NomeArquivoLote));
    }

    /// <inheritdoc />
    public async ValueTask<IResult> GerarResultadoAsync(NotaServico nota,
        string tenantId = NFSeTenant.Padrao,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentNullException.ThrowIfNull(nota);
        var configuration = await ObterConfiguracaoAsync(tenantId, cancellationToken).ConfigureAwait(false);
        var gerador = new PDFSharp.OpenDANFSePDFSharp(configuration.Configuracoes);
        var documento = new DANFSeWebDocument(gerador.GerarPDF(nota),
            NormalizarNome(configuration.NomeArquivo(nota)));
        return CriarResultado(documento, configuration.ForcarDownload);
    }

    /// <inheritdoc />
    public async ValueTask<IResult> GerarResultadoAsync(NotaServico[] notas,
        string tenantId = NFSeTenant.Padrao,
        CancellationToken cancellationToken = default)
    {
        ValidarNotas(tenantId, notas);
        var configuration = await ObterConfiguracaoAsync(tenantId, cancellationToken).ConfigureAwait(false);
        var gerador = new PDFSharp.OpenDANFSePDFSharp(configuration.Configuracoes);
        var documento = new DANFSeWebDocument(gerador.GerarPDF(notas),
            NormalizarNome(configuration.NomeArquivoLote));
        return CriarResultado(documento, configuration.ForcarDownload);
    }

    private async ValueTask<DANFSeWebOptions> ObterConfiguracaoAsync(string tenantId,
        CancellationToken cancellationToken)
    {
        var configuration = await configurationProvider.GetConfigurationAsync(tenantId, cancellationToken)
            .ConfigureAwait(false);
        return configuration ?? throw new InvalidOperationException(
            $"O provider não retornou a configuração de impressão da empresa '{tenantId}'.");
    }

    private static void ValidarNotas(string tenantId, NotaServico[] notas)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        if (notas is null || notas.Length == 0)
            throw new ArgumentException("Nenhuma nota fiscal informada para impressão.", nameof(notas));
    }

    private static IResult CriarResultado(DANFSeWebDocument documento, bool forcarDownload)
    {
        var nomeDownload = forcarDownload ? documento.NomeArquivo : null;
        return Results.File(documento.Conteudo, documento.TipoConteudo, nomeDownload,
            enableRangeProcessing: false);
    }

    private static string NormalizarNome(string nomeArquivo)
    {
        if (string.IsNullOrWhiteSpace(nomeArquivo)) return "DANFSe.pdf";
        return nomeArquivo.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
            ? nomeArquivo
            : $"{nomeArquivo}.pdf";
    }
}
