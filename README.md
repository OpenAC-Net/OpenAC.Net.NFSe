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
  - **Native AOT Safe & Multiplataforma**: Fontes TrueType embutidas, pronto para Docker, Linux (Alpine/Debian/Ubuntu), Windows e macOS.
  - Suporte à customização de cabeçalhos, logos da prefeitura e do prestador, discriminação de itens em tabela ou texto livre e criptografia por senha.
- **Assinatura e Validação Digital**:
  - Assinatura digital XML com certificados A1 e A3 (`DFeSignature`).
  - Validação de esquemas XML / Schemas XSD oficiais.
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

---

## 🛠️ Como Usar

### 1. Inicializando e Configurando o Componente

```csharp
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe;

var openNFSe = new OpenNFSe();

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
var retorno = openNFSe.Enviar(lote: 1, sincronico: false);

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
var retornoRps = openNFSe.ConsultarNFSePorRps(numeroRps: 100, serieRps: "1", tipoRps: TipoRps.RPS);

// Consulta de Situação do Lote
var retornoSituacao = openNFSe.ConsultarSituacaoLoteRps(protocolo: "123456789", lote: 1);

// Consulta de Lote de RPS
var retornoLote = openNFSe.ConsultarLoteRps(protocolo: "123456789", lote: 1);
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

## 📁 Estrutura da Solução

- **`OpenAC.Net.NFSe`**: Biblioteca principal contendo o core, provedores municipais, webservices, serializadores e modelos de dados.
- **`OpenAC.Net.NFSe.DANFSe.PDFSharp`**: Gerador e impressor do DANFSe em PDF via PDFsharp 6.x e QRCoder (100% MIT / Open Source).
- **`OpenAC.Net.NFSe.Demo`**: Aplicativo de demonstração Windows Forms completo com exemplos de preenchimento, testes de cidades e impressão.
- **`OpenAC.Net.NFSe.Test`**: Suíte de testes unitários e de integração com xUnit.

---

## 📚 Documentação e Guias

- [📖 Guia Completo do DANFSe PDFSharp](docs/DANFSE_PDFSHARP.md): Arquitetura, opções de layout, marcas d'água, logos e criptografia.
- [🔄 Guia de Migração para PDFSharp](docs/MIGRACAO_DANFSE.md): Como migrar do QuestPDF / FastReport para o PDFSharp sem complicações.
- [🏛️ Referência de Provedores e Cidades](docs/PROVEDORES.md): Lista de provedores, cidades atendidas e resolução automática de municípios.

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
