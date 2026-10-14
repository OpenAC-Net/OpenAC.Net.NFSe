# OpenAC.Net.NFSe.DANFSe.PDFSharp

[![NuGet Version](https://img.shields.io/nuget/v/OpenAC.Net.NFSe.DANFSe.PDFSharp.svg)](https://www.nuget.org/packages/OpenAC.Net.NFSe.DANFSe.PDFSharp/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/OpenAC.Net.NFSe.DANFSe.PDFSharp.svg)](https://www.nuget.org/packages/OpenAC.Net.NFSe.DANFSe.PDFSharp/)
[![Discord](https://img.shields.io/badge/Chat%20on-Discord-purple.svg)](https://discord.com/invite/brdmJ7Yv6w)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Target Frameworks](https://img.shields.io/badge/.NET%20Framework-4.6.2%20%7C%204.8-blue.svg)](https://dotnet.microsoft.com/)
[![Target Frameworks](https://img.shields.io/badge/.NET%20Core%20%2F%20Standard-netstandard2.0%20%7C%20net8.0%20%7C%20net9.0%20%7C%20net10.0-blue.svg)](https://dotnet.microsoft.com/)

Biblioteca .NET para geração, visualização e exportação do **DANFSe (Documento Auxiliar da NFS-e)** em PDF utilizando **PDFsharp 6.x** e **QRCoder**.

- **100% Open Source (MIT)**: Sem custos de licença, limites de uso ou marcas d'água comerciais.
- **Native AOT & Multiplataforma**: Fontes TrueType (LiberationSans) embutidas nos recursos do assembly, garantindo compatibilidade imediata com **Linux (Debian, Ubuntu, Alpine, Docker, Distroless)**, **Windows** e **macOS** sem necessidade de instalar fontes no sistema operacional.
- **Fiel ao padrão ACBr**: Convertido a partir do modelo de referência em Pascal (`ACBr.DANFSeX.FPDFA4Retrato.pas`).

---

## 📦 Instalação

Adicione o pacote via NuGet:

### .NET CLI:
```bash
dotnet add package OpenAC.Net.NFSe.DANFSe.PDFSharp
```

### Package Manager Console:
```powershell
Install-Package OpenAC.Net.NFSe.DANFSe.PDFSharp
```

### PackageReference:
```xml
<PackageReference Include="OpenAC.Net.NFSe.DANFSe.PDFSharp" Version="1.0.0" />
```

---

## 🚀 Como Usar

### 1. Utilizando Métodos de Extensão no `OpenNFSe`

A forma mais simples e direta de gerar o DANFSe é através dos métodos de extensão no próprio objeto `OpenNFSe`:

```csharp
using OpenAC.Net.NFSe;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Extensions;

var openNFSe = new OpenNFSe();
// ... Carregar ou emitir notas ...

// 1. Exportar diretamente para arquivo PDF
openNFSe.ImprimirPDF(@"C:\NFSe\Danfse.pdf", opt =>
{
    opt.CabecalhoLinha1 = "PREFEITURA MUNICIPAL DE SÃO PAULO";
    opt.CabecalhoLinha2 = "SECRETARIA MUNICIPAL DA FAZENDA";
    opt.ExibirQRCode = true;
    opt.Homologacao = false;
});

// 2. Exportar para uma Stream (ex: download em Controller / Minimal API ASP.NET Core)
using var ms = new MemoryStream();
openNFSe.ImprimirPDF(ms);
byte[] pdfBytes = ms.ToArray();

// 3. Obter bytes diretamente
byte[] bytes = openNFSe.GerarPDF();

// 4. Imprimir / Abrir no visualizador padrão
openNFSe.Imprimir(opt => opt.MostrarPreview = true);
```

---

### 2. Utilizando o Componente `OpenDANFSePDFSharp` Diretamente

Você também pode instanciar o gerador diretamente com objetos `NotaServico`:

```csharp
using OpenAC.Net.NFSe.DANFSe.PDFSharp;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Configuracao;
using OpenAC.Net.NFSe.Nota;

var options = new DANFSePDFSharpOptions
{
    CabecalhoLinha1 = "PREFEITURA MUNICIPAL",
    CabecalhoLinha2 = "SECRETARIA DE FINANÇAS",
    ExibirQRCode = true,
    LogoPrefeituraPath = @"C:\Imagens\brasao_prefeitura.png",
    LogoPrestadorPath = @"C:\Imagens\logo_empresa.png"
};

var danfse = new OpenDANFSePDFSharp(options);

// Gerar a partir de uma ou várias notas
danfse.ImprimirPDF(notaServico, @"C:\NFSe\Nota.pdf");
```

---

### 3. Utilizando Métodos Estáticos Rápidos

```csharp
// Salvar em arquivo
OpenDANFSePDFSharp.GerarPDF(notaServico, @"C:\NFSe\Nota.pdf");

// Salvar em Stream
using var ms = new MemoryStream();
OpenDANFSePDFSharp.GerarPDF(notaServico, ms);

// Obter bytes
byte[] pdf = OpenDANFSePDFSharp.GerarPDF(notaServico);
```

---

## ⚙️ Opções de Configuração (`DANFSePDFSharpOptions`)

| Propriedade | Tipo | Padrão | Descrição |
| :--- | :--- | :--- | :--- |
| `CabecalhoLinha1` | `string` | `"PREFEITURA MUNICIPAL"` | Texto da 1ª linha do cabeçalho da prefeitura. |
| `CabecalhoLinha2` | `string` | `"SECRETARIA MUNICIPAL DE FINANÇAS"` | Texto da 2ª linha do cabeçalho da prefeitura. |
| `LogoPrefeituraBytes` | `byte[]?` | `null` | Logotipo da prefeitura / brasão em bytes. |
| `LogoPrefeituraPath` | `string?` | `null` | Caminho do arquivo de imagem do logotipo da prefeitura. |
| `LogoPrestadorBytes` | `byte[]?` | `null` | Logotipo do prestador em bytes. |
| `LogoPrestadorPath` | `string?` | `null` | Caminho do arquivo de imagem do logotipo do prestador. |
| `ExibirQRCode` | `bool` | `true` | Exibe o QR Code no cabeçalho se houver link da NFS-e. |
| `Homologacao` | `bool` | `false` | Adiciona marca d'água de ambiente de homologação (`SEM VALOR FISCAL`). |
| `Cancelada` | `bool` | `false` | Adiciona marca d'água de nota cancelada (`NOTA CANCELADA`). |
| `QuebraDeLinha` | `string` | `""` | Caractere ou delimitador especial para quebra de linhas na discriminação. |
| `MensagemRodape` | `string` | `""` | Mensagem do rodapé (pode usar `\|` para separar `Esquerda\|Centro\|Direita`). |
| `MargemHorizontalMm` | `double` | `8.0` | Margem lateral em milímetros. |
| `MargemVerticalMm` | `double` | `8.0` | Margem superior/inferior em milímetros. |
| `Seguranca` | `DANFSeSegurancaConfig` | `...` | Opções de senha e restrições de segurança do PDF. |

---

## 🔒 Criptografia e Proteção do PDF com Senha

Permite definir senhas de abertura e permissões granulares:

```csharp
var options = new DANFSePDFSharpOptions
{
    Seguranca = new DANFSeSegurancaConfig
    {
        SenhaUsuario = "123456",            // Senha exigida para abrir e visualizar
        SenhaProprietario = "adminMaster",  // Senha do proprietário / administrador
        PermitirImpressao = true,           // Permite impressão
        PermitirModificacao = false,        // Bloqueia alterações
        PermitirCopiarConteudo = false      // Bloqueia cópia de texto
    }
};

openNFSe.ImprimirPDF(@"C:\NFSe\Danfse_Protegido.pdf", opt => opt.Seguranca = options.Seguranca);
```

---

## 📄 Licença

Este projeto é distribuído sob a licença **MIT**. Consulte o arquivo [LICENSE](../../LICENSE) para mais detalhes.
