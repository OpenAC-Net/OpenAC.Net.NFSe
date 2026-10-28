# Exemplo ASP.NET Core

Demonstra os registros de `OpenAC.Net.NFSe.Web` e `OpenAC.Net.NFSe.DANFSe.PDFSharp.Web`, consultas, envio de um RPS e geração de DANFSe em memória.

Configure os dados usando variáveis de ambiente, configuração do host ou User Secrets:

- `NFSe__Municipio`: código IBGE (padrão: 3529005).
- `NFSe__CpfCnpj` e `NFSe__InscricaoMunicipal`: identificação do prestador.
- `NFSe__Certificado` e `NFSe__SenhaCertificado`: arquivo PFX/P12 existente e sua senha.
- `NFSe__Usuario` e `NFSe__Senha`: autenticação exigida pelo provedor.

```sh
dotnet run --project OpenAC.Net.NFSe.Web.Exemplo/OpenAC.Net.NFSe.Web.Exemplo.csproj --urls http://localhost:5000
```

O exemplo usa homologação e mantém a persistência desabilitada. A raiz `/` informa o estado da aplicação; as rotas fiscais exigem certificado explícito. Consulte o arquivo `.http` para exemplos de requisições. O endpoint `/emitir/{lote}` recebe `{"xml":"XML do RPS"}` e transmite ao provedor configurado. `/danfse` recebe o XML de uma nota autorizada.

Antes de expor as rotas em uma aplicação real, integre autenticação, autorização por empresa e tratamento de erros ao host. Para várias empresas, use `AddOpenNFSeWebMultiTenant` e a fábrica documentada no projeto Web.
