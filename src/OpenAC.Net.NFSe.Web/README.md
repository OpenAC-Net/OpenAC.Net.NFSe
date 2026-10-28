# OpenAC.Net.NFSe.Web

Integração ASP.NET Core para os provedores municipais de NFS-e, no padrão de serviços do NFSe.Nacional. Suporta .NET 8, 9 e 10.

```csharp
using Microsoft.Extensions.DependencyInjection;
using OpenAC.Net.NFSe;

services.AddOpenNFSeWeb(config =>
{
    config.WebServices.CodigoMunicipio = 3529005;
    config.PrestadorPadrao.CpfCnpj = "CNPJ_DO_PRESTADOR";
    config.PrestadorPadrao.InscricaoMunicipal = "INSCRICAO";
    config.Certificados.CertificadoBytes = bytesPfx;
    config.Certificados.Senha = senhaPfx;
    config.Arquivos.PathSchemas = caminhoSchemas;
}, options =>
{
    options.TimeoutOperacao = TimeSpan.FromSeconds(100);
    options.TimeoutTentativa = TimeSpan.FromSeconds(30);
    options.TimeoutsPorOperacao[nameof(IOpenNFSeClient.EnviarAsync)] = TimeSpan.FromMinutes(2);
    options.PersistenciaHabilitada = false;
});
```

`IOpenNFSeClient` é registrado por escopo. Sua configuração e coleção `NotasServico` são independentes. Configure o certificado por bytes PFX/P12 ou caminho de arquivo existente; a biblioteca Web rejeita seleção interativa, certificado sem chave privada, fora da validade ou próximo do vencimento.

```csharp
app.MapGet("/nfse/{numero:int}", async (
    int numero, IOpenNFSeClient client, CancellationToken cancellationToken) =>
    await client.ConsultaNFSeAsync(numero, cancellationToken: cancellationToken));
```

## Multiempresa

Implemente `INFSeConfigurationProvider<ConfigNFSe>` e `INFSeCertificateProvider` e registre:

```csharp
services.AddOpenNFSeWebMultiTenant<EmpresaConfigurationProvider, EmpresaCertificateProvider>();

using var client = await factory.CreateAsync(empresaId, cancellationToken);
var result = await client.ConsultaNFSeAsync(numero, cancellationToken: cancellationToken);
```

`factory` implementa `IOpenNFSeClientFactory`. Cada chamada deve receber uma configuração nova, exclusiva para aquele cliente. Os providers são singletons e precisam ser seguros para concorrência. Resolva `empresaId` a partir do contexto autenticado da aplicação consumidora.

Os handlers HTTP são separados por empresa, certificado, endpoint, TLS, proxy e regras de comunicação. Credenciais e cabeçalhos de autenticação são aplicados por requisição. Cookies não são compartilhados. O `IHttpClientFactory` gerencia a renovação de handlers e conexões.

## Comunicação e limites

As operações municipais existentes continuam disponíveis na API síncrona de `OpenNFSe`. Os métodos `Async` executam esses provedores em uma thread de trabalho e propagam o cancelamento até o HTTP; eles ainda ocupam uma thread durante a comunicação síncrona do provedor. Operações assíncronas são serializadas por cliente. Use clientes independentes para concorrência e não altere sua configuração ou suas notas durante uma operação.

Timeouts de operação, de tentativa e do provedor são limites cumulativos: prevalece o primeiro a expirar. O timeout próprio do provedor é configurado por `AjustaAguardaConsultaRet` e `AguardarConsultaRet`, em segundos.

Tentativas seguem `WebServices.Tentativas` e `IntervaloTentativas`. Repetição automática é permitida somente para métodos HTTP seguros. POSTs, inclusive consultas SOAP, emissão, cancelamento e substituição, não são repetidos automaticamente. Respostas fiscais continuam em `RetornoWebservice`; falhas de transporte são expostas como `NFSeTechnicalException`. Cancelamento mantém `OperationCanceledException`.

Logs, `ActivitySource` e métricas usam o nome `OpenAC.Net.NFSe.Web`. Métricas: `openac.nfse.operations` e `openac.nfse.operation.duration`. Os logs próprios não incluem XMLs, senhas ou cabeçalhos de autenticação. Configure separadamente o logging HTTP do host se os endpoints de um provedor incluírem dados sensíveis em URLs.

## Documentos

A persistência Web é desabilitada por padrão, mesmo quando as flags legadas de salvamento estão habilitadas. Para ativar, configure `PersistenciaHabilitada`, `CaminhoDocumentos` e as flags correspondentes de `Geral`, `WebServices` e `Arquivos`.

`FileSystemNFSeDocumentStore` usa a raiz do host, separa diretórios por hash da empresa, categoria e mês e evita sobrescritas por colisão de nome. Pode ser substituído por uma implementação de `INFSeDocumentStore`, por exemplo para armazenamento de objetos. O transporte também pode ser substituído por `INFSeHttpTransport`; registre suas implementações antes de `AddOpenNFSeWeb`.

O catálogo de municípios continua sendo compartilhado pelo processo. Carregue e configure o catálogo na inicialização do host; não o modifique durante requisições concorrentes.

Veja também [o exemplo ASP.NET Core](../OpenAC.Net.NFSe.Web.Exemplo/README.md) e [o DANFSe Web](../OpenAC.Net.NFSe.DANFSe.PDFSharp.Web/README.md).
