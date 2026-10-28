# OpenAC.Net.NFSe

[![NuGet Version](https://img.shields.io/nuget/v/OpenAC.Net.NFSe.svg)](https://www.nuget.org/packages/OpenAC.Net.NFSe/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/OpenAC.Net.NFSe.svg)](https://www.nuget.org/packages/OpenAC.Net.NFSe/)
[![Discord](https://img.shields.io/badge/Chat%20on-Discord-purple.svg)](https://discord.com/invite/brdmJ7Yv6w)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Target Frameworks](https://img.shields.io/badge/.NET%20Framework-4.6.2%20%7C%204.7.0%20%7C%204.8-blue.svg)](https://dotnet.microsoft.com/)
[![Target Frameworks](https://img.shields.io/badge/.NET%20Core%20%2F%20Standard-netstandard2.0%20%7C%20net6.0%20%7C%20net7.0%20%7C%20net8.0%20%7C%20net9.0%20%7C%20net10.0-blue.svg)](https://dotnet.microsoft.com/)

Biblioteca .NET multiplataforma para geração, assinatura, transmissão, consulta e impressão de **NFS-e (Nota Fiscal de Serviço Eletrônica)** para centenas de municípios brasileiros.

Desenvolvida pelo time **OpenAC .Net**, a biblioteca abstrai as particularidades dos diversos provedores municipais e padrões existentes (ABRASF 1.0, ABRASF 2.0x, DSF, Ginfes, SigISS, Betha, IPM, Governos próprios, entre outros).

---

## 🚀 Funcionalidades

- **Emissão e Transmissão de RPS / NFS-e**:
  - Emissão Síncrona e Assíncrona.
  - Envio de RPS individual ou em lote.
  - Substituição de NFS-e / RPS.
- **Cancelamento de NFS-e**:
  - Cancelamento por código de motivo, justificativa ou pedido assinado.
- **Consultas**:
  - Consulta de Situação do Lote.
  - Consulta de Lote de RPS.
  - Consulta de NFS-e por RPS.
  - Consulta de NFS-e por período / faixa de datas.
  - Consulta de Link da NFS-e.
- **Impressão do DANFSe em PDF**:
  - Motor de impressão **`OpenAC.Net.NFSe.DANFSe.PDFSharp`** baseado em **PDFsharp 6.x** e **QRCoder**.
  - **100% Open Source / MIT**, sem custos de licença ou marcas d'água de terceiros.
  - Fontes TrueType embutidas e geração de PDF em memória para Windows, Linux e macOS.
  - Suporte à customização de cabeçalhos, logos da prefeitura e do prestador, discriminação de itens em tabela ou texto livre e criptografia por senha.
- **Assinatura e Validação Digital**:
  - Assinatura digital XML com certificados A1 e A3 (`DFeSignature`).
  - Validação de esquemas XML / Schemas XSD oficiais.
- **Comunicação HTTP**:
  - Reutilização de conexões por certificado, TLS, proxy e validação do servidor.
  - Transporte substituível por `INFSeHttpTransport` e armazenamento por `INFSeDocumentStore`.
  - Métodos `Async` com `CancellationToken`, preservando a API síncrona existente.
- **Integração ASP.NET Core**:
  - Registro por escopo com `AddOpenNFSeWeb` e clientes independentes por empresa.
  - `IHttpClientFactory`, timeouts por operação, retentativas para métodos HTTP seguros e circuit breaker.
  - Logs, rastreamento e métricas de operações.
  - DANFSe em memória e respostas HTTP para Minimal APIs.
- **Demo Avalonia**:
  - Interface Fluent com tema azul para Windows, Linux e macOS.
  - Operações fiscais, configurações, catálogo de municípios e geração de DANFSe.
- **Alta Performance**:
  - Serialização e desserialização XML baseada em *Source Generators* (`OpenAC.Net.DFe.Core`).
- **Suporte à Reforma Tributária (IBS / CBS)**:
  - Estruturas prontas para os novos tributos municipais e federais.

---

## 🏛️ Provedores e Padrões Suportados

A biblioteca suporta os principais padrões de mercado e provedores municipais:

| Padrão / Provedor | Padrão / Provedor | Padrão / Provedor |
| :--- | :--- | :--- |
| **ABRASF v1.00** | **ABRASF v2.00 a v2.04** | **Abaco** |
| **ABase** | **Agili** | **Assessor Público** |
| **Betha (v1 e v2)** | **BHISS (Belo Horizonte)** | **Citta** |
| **Conam** | **Coplan** | **DBSeller** |
| **DSF / ISSDSF** | **Equiplano** | **FintelISS** |
| **Fiorilli** | **Fisco** | **FissLex** |
| **GIAP** | **Ginfes** | **GISS / SigISS** |
| **GovBR** | **IPM (v1 e v2)** | **ISSCampinas** |
| **ISSCuritiba** | **ISSGoiania** | **ISSIntegra** |
| **ISSNet** | **ISSPortoVelho** | **ISSRecife** |
| **ISSRio (Nota Carioca)** | **ISSSJP** | **ISSSaoPaulo (Paulistana)** |
| **ISSVitoria** | **ISSe** | **Megasoft** |
| **MetropolisWeb** | **Mitra** | **NFe Cidades (GovDigital)** |
| **Prodata** | **Pronim (v1, v2, v203)** | **RLZ Informática** |
| **SiapNet** | **Sigep** | **SigissWeb** |
| **SimplISS (v1 e v2)** | **Sintese** | **SmarAPD** |
| **SpeedGov** | **SystemPro** | **Thema** |
| **Tinus** | **Tiplan (v1 e v2)** | **WebISS (v1 e v2)** |

---

## 📦 Instalação

Adicione os pacotes via NuGet:

### 1. Componente Principal (NFS-e)

```bash
dotnet add package OpenAC.Net.NFSe
```

```powershell
Install-Package OpenAC.Net.NFSe
```

### 2. Componente de Impressão (DANFSe em PDF via PDFsharp)

```bash
dotnet add package OpenAC.Net.NFSe.DANFSe.PDFSharp
```

```powershell
Install-Package OpenAC.Net.NFSe.DANFSe.PDFSharp
```

### 3. Integração Web e DANFSe para ASP.NET Core

Os projetos Web suportam .NET 8, 9 e 10:

```bash
dotnet add package OpenAC.Net.NFSe.Web
dotnet add package OpenAC.Net.NFSe.DANFSe.PDFSharp.Web
```

Para usar os projetos diretamente a partir deste repositório, adicione referências aos respectivos arquivos `.csproj` em `src`.

### Compatibilidade

| Projeto | Frameworks |
| :--- | :--- |
| `OpenAC.Net.NFSe` | .NET Framework 4.6.2, 4.7.0 e 4.8; .NET Standard 2.0; .NET 6, 7, 8, 9 e 10 |
| `OpenAC.Net.NFSe.DANFSe.PDFSharp` | .NET Framework 4.6.2 e 4.8; .NET Standard 2.0; .NET 8, 9 e 10 |
| `OpenAC.Net.NFSe.Web` e `OpenAC.Net.NFSe.DANFSe.PDFSharp.Web` | .NET 8, 9 e 10 |
| Demo Avalonia e exemplo ASP.NET Core | .NET 8 |

---

## 🛠️ Como Usar

### 1. Inicializando e Configurando o Componente

```csharp
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe;

using var openNFSe = new OpenNFSe();

// Configurações Gerais
openNFSe.Configuracoes.Geral.RetirarAcentos = true;
openNFSe.Configuracoes.Geral.Salvar = true;

// Configurações dos WebServices / Cidade
openNFSe.Configuracoes.WebServices.Ambiente = DFeTipoAmbiente.Homologacao;
openNFSe.Configuracoes.WebServices.CodigoMunicipio = 3550308; // Código IBGE (Ex: São Paulo - SP)
openNFSe.Configuracoes.WebServices.Usuario = "usuario_ws";
openNFSe.Configuracoes.WebServices.Senha = "senha_ws";

// Configurações dos Arquivos
openNFSe.Configuracoes.Arquivos.PathSalvar = @"C:\NFSe\Arquivos";
openNFSe.Configuracoes.Arquivos.PathSchemas = @"C:\NFSe\Schemas";

// Configurações do Certificado Digital
openNFSe.Configuracoes.Certificados.Certificado = @"C:\Certificados\certificado.pfx";
openNFSe.Configuracoes.Certificados.Senha = "123456";

// Dados do Prestador Padrão
openNFSe.Configuracoes.PrestadorPadrao.CpfCnpj = "12345678000195";
openNFSe.Configuracoes.PrestadorPadrao.InscricaoMunicipal = "12345678";
```

---

### 2. Criando e Emitindo uma Nota Fiscal de Serviço (RPS)

```csharp
using System;
using OpenAC.Net.NFSe.Nota;

// Limpa notas anteriores
openNFSe.NotasServico.Clear();

// Adiciona um novo RPS
var nota = openNFSe.NotasServico.AddNew();

// Identificação do RPS
nota.IdentificacaoRps.Numero = "100";
nota.IdentificacaoRps.Serie = "1";
nota.IdentificacaoRps.Tipo = TipoRps.RPS;
nota.IdentificacaoRps.DataEmissao = DateTime.Now;

// Regime Tributário
nota.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
nota.OptanteSimplesNacional = NFSeSimNao.Sim;
nota.IncentivadorCultural = NFSeSimNao.Nao;

// Prestador
nota.Prestador.CpfCnpj = "12345678000195";
nota.Prestador.InscricaoMunicipal = "12345678";
nota.Prestador.RazaoSocial = "EMPRESA PRESTADORA DE SERVICOS LTDA";

// Tomador
nota.Tomador.CpfCnpj = "98765432000100";
nota.Tomador.RazaoSocial = "CLIENTE TOMADOR DE SERVICOS S/A";
nota.Tomador.Endereco.Logradouro = "AVENIDA PAULISTA";
nota.Tomador.Endereco.Numero = "1000";
nota.Tomador.Endereco.Bairro = "BELA VISTA";
nota.Tomador.Endereco.Municipio = "SAO PAULO";
nota.Tomador.Endereco.Uf = "SP";
nota.Tomador.Endereco.Cep = "01310100";
nota.Tomador.DadosContato.Email = "financeiro@cliente.com.br";

// Serviço
nota.Servico.ItemListaServico = "01.07";
nota.Servico.Discriminacao = "Desenvolvimento e manutenção de software sob medida.";
nota.Servico.Valores.ValorServicos = 2500.00M;
nota.Servico.Valores.BaseCalculo = 2500.00M;
nota.Servico.Valores.Aliquota = 2.00M;
nota.Servico.Valores.ValorIss = 50.00M;
nota.Servico.Valores.ValorLiquidoNfse = 2450.00M;

// Envio do Lote de RPS
var retorno = openNFSe.Enviar(lote: 1, sincrono: false);

if (retorno.Sucesso)
{
    Console.WriteLine($"Sucesso! Protocolo: {retorno.Protocolo}");
}
else
{
    foreach (var alerta in retorno.Alertas)
        Console.WriteLine($"Alerta: {alerta.Codigo} - {alerta.Descricao}");
    foreach (var erro in retorno.Erros)
        Console.WriteLine($"Erro: {erro.Codigo} - {erro.Descricao}");
}
```

---

### 3. Gerando e Imprimindo o DANFSe em PDF

Utilizando o pacote **`OpenAC.Net.NFSe.DANFSe.PDFSharp`**:

```csharp
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Extensions;

// 1. Exportar diretamente para arquivo PDF
openNFSe.ImprimirPDF(@"C:\NFSe\Danfse.pdf", opt =>
{
    opt.CabecalhoLinha1 = "PREFEITURA MUNICIPAL DE SÃO PAULO";
    opt.CabecalhoLinha2 = "SECRETARIA MUNICIPAL DA FAZENDA";
    opt.ExibirQRCode = true;
    opt.Homologacao = false;
});

// 2. Exportar para uma Stream (ex: download em API Web / ASP.NET Core)
using var ms = new MemoryStream();
openNFSe.ImprimirPDF(ms);
var bytes = ms.ToArray();

// 3. Obter os bytes do PDF diretamente
byte[] pdfBytes = openNFSe.GerarPDF();

// 4. Imprimir / Visualizar na tela
openNFSe.Imprimir(opt => opt.MostrarPreview = true);
```

#### Proteção e Criptografia do PDF com Senha:

```csharp
openNFSe.ImprimirPDF(@"C:\NFSe\Danfse_Protegido.pdf", opt =>
{
    opt.Seguranca.SenhaUsuario = "123456";          // Senha exigida para abrir o PDF
    opt.Seguranca.SenhaProprietario = "adminMaster"; // Senha do proprietário
    opt.Seguranca.PermitirImpressao = true;
    opt.Seguranca.PermitirModificacao = false;
    opt.Seguranca.PermitirCopiarConteudo = false;
});
```

---

### 4. Consultas

```csharp
// Consulta de NFS-e por Número do RPS
var retornoRps = openNFSe.ConsultaNFSeRps(numero: 100, serie: "1", tipo: TipoRps.RPS);

// Consulta de Situação do Lote
var retornoSituacao = openNFSe.ConsultarSituacao(lote: 1, protocolo: "123456789");

// Consulta de Lote de RPS
var retornoLote = openNFSe.ConsultarLoteRps(lote: 1, protocolo: "123456789");
```

---

### 5. Cancelamento de NFS-e

```csharp
var retornoCancelamento = openNFSe.CancelarNFSe(
    codigoCancelamento: "1",
    numeroNFSe: "202600000000100",
    motivo: "Erro na discriminação dos serviços."
);

if (retornoCancelamento.Sucesso)
{
    Console.WriteLine("NFS-e cancelada com sucesso!");
}
```

---

### 6. Operações Assíncronas e CancellationToken

O componente também implementa `IOpenNFSeClient`, com versões assíncronas das operações municipais:

```csharp
using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(100));

var retornoConsulta = await openNFSe.ConsultaNFSeAsync(
    numeroNfse: 100,
    cancellationToken: cancellation.Token);

// Após preencher NotasServico, envie o lote:
var retornoEnvio = await openNFSe.EnviarAsync(
    lote: 1,
    sincrono: false,
    cancellationToken: cancellation.Token);
```

Os provedores municipais continuam síncronos internamente. Os métodos `Async` executam em uma thread de trabalho e propagam o cancelamento até a requisição HTTP. Operações assíncronas são serializadas por cliente; use clientes independentes para processar empresas ou operações em paralelo e mantenha suas configurações e notas estáveis durante a execução.

As operações e os formatos aceitos dependem do provedor e da versão cadastrados para o município. No envio, `sincrono` seleciona a modalidade de processamento do lote pelo provedor; o sufixo `Async` identifica a API com cancelamento do componente.

---

## 🌐 ASP.NET Core e Multiempresa

Exemplo de registro em `Program.cs`, com configurações fornecidas pelo host:

```csharp
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Web.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenNFSeWeb(config =>
{
    config.WebServices.Ambiente = DFeTipoAmbiente.Homologacao;
    config.WebServices.CodigoMunicipio = builder.Configuration.GetValue<int>("NFSe:Municipio");
    config.PrestadorPadrao.CpfCnpj = builder.Configuration["NFSe:CpfCnpj"] ?? "";
    config.PrestadorPadrao.InscricaoMunicipal = builder.Configuration["NFSe:InscricaoMunicipal"] ?? "";
    config.Certificados.Certificado = builder.Configuration["NFSe:Certificado"] ?? "";
    config.Certificados.Senha = builder.Configuration["NFSe:SenhaCertificado"] ?? "";
    config.Arquivos.PathSchemas = Path.Combine(AppContext.BaseDirectory, "Schemas");
}, options =>
{
    options.TimeoutOperacao = TimeSpan.FromSeconds(100);
    options.TimeoutTentativa = TimeSpan.FromSeconds(30);
    options.PersistenciaHabilitada = false;
});

builder.Services.AddOpenDANFSeWeb(options => options.ForcarDownload = true);

var app = builder.Build();

app.MapGet("/nfse/{numero:int}", async (
    int numero, IOpenNFSeClient client, CancellationToken cancellationToken) =>
    await client.ConsultaNFSeAsync(numero, cancellationToken: cancellationToken));

app.MapGet("/nfse/{numero:int}/pdf", async (
    int numero, IOpenNFSeClient client, IOpenDANFSeWeb danfse,
    CancellationToken cancellationToken) =>
{
    var retorno = await client.ConsultaNFSeAsync(numero, cancellationToken: cancellationToken);
    if (!retorno.Sucesso || retorno.Notas.Length == 0)
        return Results.BadRequest(retorno);

    return await danfse.GerarResultadoAsync(retorno.Notas, cancellationToken: cancellationToken);
});

app.Run();
```

`IOpenNFSeClient` é criado por escopo, com configuração e notas independentes. Configure o certificado explicitamente por `CertificadoBytes` ou caminho PFX/P12 existente. A integração Web valida a chave privada e a validade do certificado e não abre seleção interativa.

Para várias empresas, implemente `INFSeConfigurationProvider<ConfigNFSe>` e `INFSeCertificateProvider` e registre:

```csharp
builder.Services.AddOpenNFSeWebMultiTenant<EmpresaConfigurationProvider, EmpresaCertificateProvider>();
```

Resolva `IOpenNFSeClientFactory` no consumidor e crie um cliente para a empresa autenticada:

```csharp
using var client = await factory.CreateAsync(empresaId, cancellationToken);
var retorno = await client.ConsultaNFSeAsync(numero, cancellationToken: cancellationToken);
```

Os providers devem retornar configurações novas por cliente e ser seguros para concorrência. Handlers são isolados por empresa, certificado, endpoint, TLS, proxy e regras de comunicação. O catálogo de municípios é compartilhado pelo processo e deve ser configurado na inicialização do host.

As tentativas automáticas seguem `WebServices.Tentativas` e `IntervaloTentativas`, apenas para métodos HTTP seguros. POSTs, inclusive consultas SOAP, emissão e cancelamento, não são repetidos automaticamente. Rejeições fiscais permanecem em `RetornoWebservice`; falhas técnicas de transporte geram `NFSeTechnicalException`, e cancelamentos geram `OperationCanceledException`.

A persistência Web é desabilitada por padrão. Para ativar, configure `PersistenciaHabilitada`, `CaminhoDocumentos` e as flags de salvamento correspondentes. O armazenamento separa documentos por empresa e categoria, sem sobrescrever arquivos em colisões de nome. O DANFSe Web gera PDFs em memória e devolve `byte[]`, stream ou `IResult`.

Consulte os detalhes de [configuração Web](src/OpenAC.Net.NFSe.Web/README.md), [DANFSe Web](src/OpenAC.Net.NFSe.DANFSe.PDFSharp.Web/README.md) e o [exemplo ASP.NET Core](src/OpenAC.Net.NFSe.Web.Exemplo/README.md).

---

## 🖥️ Demo Avalonia com Tema Azul

O demo usa Avalonia Fluent e segue a organização do NFSe.Nacional, com navegação entre **Operações**, **Configurações** e **Municípios**. Funciona em Windows, Linux e macOS com .NET 8.

Execute a partir da raiz do repositório:

```bash
dotnet run --project src/OpenAC.Net.NFSe.Demo/OpenAC.Net.NFSe.Demo.csproj
```

- **Operações**: carregar XML, gerar RPS de exemplo, enviar lotes, consultar, cancelar/substituir notas e gerar ou visualizar DANFSe.
- **Configurações**: certificado, prestador, ambiente, autenticação do provedor, timeout e caminhos de arquivos.
- **Municípios**: pesquisar, cadastrar, editar, copiar, excluir, selecionar, importar e exportar o catálogo, com URLs por ambiente e parâmetros do provedor.

Resultados, XMLs, envelopes HTTP/SOAP e logs aparecem em abas. A interface apresenta progresso e permite cancelar a operação. O ambiente inicial é homologação.

As configurações ficam em `LocalApplicationData/OpenAC.Net/NFSe.Demo/opennfse.config`. O demo importa um `opennfse.config` legado do diretório de trabalho na primeira execução. Senhas e chaves de autenticação devem ser informadas a cada execução e não são gravadas pelo novo demo. Veja o [guia do demo](src/OpenAC.Net.NFSe.Demo/README.md).

---

## 📁 Estrutura da Solução

- **`OpenAC.Net.NFSe`**: Biblioteca principal contendo o core, provedores municipais, webservices, serializadores e modelos de dados.
- **`OpenAC.Net.NFSe.DANFSe.PDFSharp`**: Gerador e impressor do DANFSe em PDF via PDFsharp 6.x e QRCoder (100% MIT / Open Source).
- **`OpenAC.Net.NFSe.Web`**: Integração ASP.NET Core com DI, fábrica multiempresa, transporte HTTP, resiliência, observabilidade e persistência opcional.
- **`OpenAC.Net.NFSe.DANFSe.PDFSharp.Web`**: Geração de DANFSe em memória e respostas HTTP para ASP.NET Core.
- **`OpenAC.Net.NFSe.Web.Exemplo`**: Exemplo de API com consultas, emissão e geração de DANFSe.
- **`OpenAC.Net.NFSe.Demo`**: Aplicativo Avalonia com tema azul, operações fiscais, configurações e cadastro de municípios.
- **`OpenAC.Net.NFSe.Test`**: Suíte de testes unitários e de integração com xUnit.

---

## 📚 Documentação e Guias

- [Integração ASP.NET Core e multiempresa](src/OpenAC.Net.NFSe.Web/README.md): Transporte, certificados, timeouts, resiliência, métricas e persistência.
- [DANFSe Web](src/OpenAC.Net.NFSe.DANFSe.PDFSharp.Web/README.md): Geração em memória e retorno de PDF por HTTP.
- [DANFSe PDFSharp](src/OpenAC.Net.NFSe.DANFSe.PDFSharp/README.md): Uso e opções do gerador de PDF.
- [Demo Avalonia](src/OpenAC.Net.NFSe.Demo/README.md): Operações, configurações e catálogo de municípios.
- [Exemplo ASP.NET Core](src/OpenAC.Net.NFSe.Web.Exemplo/README.md): Configuração e execução da API de demonstração.

---

## 🤝 Como Contribuir

Contribuições são muito bem-vindas! Para contribuir:

1. Faça um **Fork** do projeto.
2. Crie uma Branch para a sua feature (`git checkout -b feature/novo-provedor`).
3. Commit suas alterações (`git commit -m 'feat: adiciona suporte ao provedor XYZ'`).
4. Push para a branch (`git push origin feature/novo-provedor`).
5. Abra um **Pull Request**.

---

## 💖 Apoie o Projeto OpenAC .Net

Se o **OpenAC.Net.NFSe** é útil para você ou sua empresa e você deseja apoiar a continuidade e evolução da biblioteca:

- Participe da nossa comunidade no [Discord](https://discord.com/invite/brdmJ7Yv6w).
- Deixe uma estrela ⭐ no [GitHub](https://github.com/OpenAC-Net/OpenAC.Net.NFSe).
- Compartilhe com outros desenvolvedores da comunidade .NET.

---

## 📄 Licença

Este projeto é distribuído sob a licença **MIT**. Consulte o arquivo [LICENSE](LICENSE) para mais detalhes.
