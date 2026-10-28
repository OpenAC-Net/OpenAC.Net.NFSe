# Demo NFS-e Municipal em Avalonia

Aplicação .NET 8 para Windows, Linux e macOS com Avalonia Fluent, tema azul e a mesma organização visual do demo NFSe.Nacional: Operações, Configurações e Municípios.

```sh
dotnet run --project OpenAC.Net.NFSe.Demo/OpenAC.Net.NFSe.Demo.csproj
```

Em Configurações, informe o certificado, o prestador e os caminhos de schemas e documentos. Selecione um município cadastrado na página Municípios. O ambiente inicial é homologação. A versão e o provedor vêm do cadastro do município.

A página Operações carrega RPS/NFS-e XML, gera RPS de exemplo, envia lotes normais ou síncronos, consulta situação, lote, RPS, NFS-e, período, sequencial, tomador e intermediário. Também cancela/substitui notas e gera ou visualiza DANFSe em PDF. As operações disponíveis dependem do provedor. Revise os dados de exemplo e os códigos municipais antes de transmitir.

O resultado, XML de envio/retorno, envelopes HTTP/SOAP e logs aparecem em abas com opção de copiar. Operações são executadas fora da thread de UI, com progresso, timeout e cancelamento. Fechar a janela cancela a operação e aguarda sua conclusão.

A página Municípios permite pesquisar, cadastrar, editar, copiar, excluir, selecionar, importar e exportar o catálogo. Cada cadastro permite URLs de homologação/produção por operação e parâmetros específicos do provedor.

As configurações ficam em `LocalApplicationData/OpenAC.Net/NFSe.Demo/opennfse.config`. Quando o arquivo ainda não existe, um `opennfse.config` no diretório de trabalho é importado com as mesmas chaves do demo WinForms. Senhas e chaves de autenticação devem ser informadas a cada execução e não são gravadas pelo novo demo. O catálogo local é salvo na mesma pasta das configurações.
