# OpenAC.Net.NFSe.DANFSe.PDFSharp.Web

Geração de DANFSe municipal em memória e respostas HTTP para ASP.NET Core (.NET 8, 9 e 10).

```csharp
services.AddOpenDANFSeWeb(options =>
{
    options.ForcarDownload = true;
    options.NomeArquivo = nota => $"DANFSe-{nota.IdentificacaoNFSe.Numero}.pdf";
});

var document = await gerador.GerarAsync(nota, cancellationToken: cancellationToken);
byte[] pdf = document.Conteudo;

// Em uma Minimal API, devolva diretamente o IResult:
return await gerador.GerarResultadoAsync(nota, cancellationToken: cancellationToken);
```

`gerador` implementa `IOpenDANFSeWeb`. Também aceita arrays de notas para gerar um único PDF. O serviço não abre visualizadores, não imprime em dispositivos e não cria arquivos temporários. `DANFSeWebDocument` expõe conteúdo, nome, MIME e stream somente leitura.

Para impressão multiempresa, implemente `INFSeConfigurationProvider<DANFSeWebOptions>` retornando opções novas para cada chamada e registre `AddOpenDANFSeWeb<EmpresaDANFSeConfigurationProvider>()`. Passe a identidade da empresa em `GerarAsync` ou `GerarResultadoAsync`. O token cancela a obtenção da configuração; a renderização PDFSharp é síncrona.
