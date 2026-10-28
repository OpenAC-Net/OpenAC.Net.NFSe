using OpenAC.Net.NFSe;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Web.Service;
using OpenAC.Net.NFSe.Web.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenNFSeWeb(config =>
{
    config.WebServices.CodigoMunicipio = builder.Configuration.GetValue("NFSe:Municipio", 3529005);
    config.PrestadorPadrao.CpfCnpj = builder.Configuration["NFSe:CpfCnpj"] ?? "";
    config.PrestadorPadrao.InscricaoMunicipal = builder.Configuration["NFSe:InscricaoMunicipal"] ?? "";
    config.Certificados.Certificado = builder.Configuration["NFSe:Certificado"] ?? "";
    config.Certificados.Senha = builder.Configuration["NFSe:SenhaCertificado"] ?? "";
    config.WebServices.Usuario = builder.Configuration["NFSe:Usuario"] ?? "";
    config.WebServices.Senha = builder.Configuration["NFSe:Senha"] ?? "";
    config.Arquivos.PathSchemas = Path.Combine(AppContext.BaseDirectory, "Schemas");
}, options =>
{
    options.TimeoutOperacao = TimeSpan.FromSeconds(100);
    options.PersistenciaHabilitada = false;
});
builder.Services.AddOpenDANFSeWeb();

var app = builder.Build();
app.MapGet("/", () => Results.Ok(new { Servico = "OpenAC.Net.NFSe.Web", Ambiente = "Homologação" }));
app.MapGet("/nfse/{numero:int}", async (int numero, IOpenNFSeClient client, CancellationToken ct) =>
    Results.Ok(await client.ConsultaNFSeAsync(numero, cancellationToken: ct)));
app.MapGet("/lotes/{lote:int}/{protocolo}", async (int lote, string protocolo, IOpenNFSeClient client, CancellationToken ct) =>
    Results.Ok(await client.ConsultarLoteRpsAsync(lote, protocolo, ct)));
app.MapPost("/emitir/{lote:int}", async (int lote, XmlRequest request, IOpenNFSeClient client, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Xml)) return Results.BadRequest("Informe o XML de um RPS.");
    client.NotasServico.Load(request.Xml);
    return Results.Ok(await client.EnviarAsync(lote, cancellationToken: ct));
});
app.MapPost("/danfse", async (XmlRequest request, IOpenNFSeClientFactory factory, IOpenDANFSeWeb pdf, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Xml)) return Results.BadRequest("Informe o XML da NFS-e.");
    using var client = await factory.CreateAsync(cancellationToken: ct);
    var note = client.NotasServico.Load(request.Xml);
    return await pdf.GerarResultadoAsync(note, cancellationToken: ct);
});
app.Run();

public sealed record XmlRequest(string Xml);
