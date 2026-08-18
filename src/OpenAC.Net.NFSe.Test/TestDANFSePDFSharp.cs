using System;
using System.IO;
using OpenAC.Net.NFSe.DANFSe.PDFSharp;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Extensions;
using OpenAC.Net.NFSe.Nota;
using Xunit;

namespace OpenAC.Net.NFSe.Test;

public class TestDANFSePDFSharp
{
    private const string OutDir = "/mnt/e/Programacao/OpenAC .Net/OpenAC.Net.NFSe/exemplos";

    [Fact]
    public void TestGerarPDFSimples()
    {
        var openNFSe = SetupOpenNFSe.Ginfes;
        openNFSe.NotasServico.Clear();

        var nota = openNFSe.NotasServico.AddNew();
        nota.IdentificacaoNFSe.Numero = "12345";
        nota.IdentificacaoNFSe.DataEmissao = DateTime.Now;
        nota.IdentificacaoNFSe.Chave = "ABCDEF123456789";

        nota.Prestador.CpfCnpj = "44818198000190";
        nota.Prestador.InscricaoMunicipal = "0010040441011";
        nota.Prestador.RazaoSocial = "EMPRESA PRESTADORA DE SERVICOS LTDA";
        nota.Prestador.Endereco.Logradouro = "RUA PRINCIPAL";
        nota.Prestador.Endereco.Numero = "100";
        nota.Prestador.Endereco.Bairro = "CENTRO";
        nota.Prestador.Endereco.Municipio = "SAO PAULO";
        nota.Prestador.Endereco.Uf = "SP";
        nota.Prestador.Endereco.Cep = "01001000";
        nota.Prestador.DadosContato.Email = "contato@prestadora.com.br";
        nota.Prestador.DadosContato.Telefone = "(11) 1234-5678";

        nota.Tomador.CpfCnpj = "12345678000195";
        nota.Tomador.RazaoSocial = "CLIENTE TOMADOR DE SERVICOS S/A";
        nota.Tomador.Endereco.Logradouro = "AVENIDA PAULISTA";
        nota.Tomador.Endereco.Numero = "2000";
        nota.Tomador.Endereco.Bairro = "BELA VISTA";
        nota.Tomador.Endereco.Municipio = "SAO PAULO";
        nota.Tomador.Endereco.Uf = "SP";
        nota.Tomador.Endereco.Cep = "01310200";

        nota.Servico.ItemListaServico = "01.07";
        nota.DescricaoCodigoTributacaoMunicipio = "Suporte técnico, manutenção e outros serviços em TI";
        nota.Servico.Discriminacao = "Serviços prestados de suporte técnico em sistemas de informação e consultoria técnica durante o período de 01/08 a 31/08.";
        nota.Servico.Valores.ValorServicos = 1500.00M;
        nota.Servico.Valores.BaseCalculo = 1500.00M;
        nota.Servico.Valores.Aliquota = 5.00M;
        nota.Servico.Valores.ValorIss = 75.00M;
        nota.Servico.Valores.ValorLiquidoNfse = 1425.00M;
        nota.Servico.Valores.ValorPis = 9.75M;
        nota.Servico.Valores.ValorCofins = 45.00M;
        nota.Servico.Valores.ValorInss = 0.00M;
        nota.Servico.Valores.ValorIr = 22.50M;
        nota.Servico.Valores.ValorCsll = 15.00M;

        nota.OutrasInformacoes = "Documento emitido por ME ou EPP optante pelo Simples Nacional.";

        var pdfBytes = openNFSe.GerarPDF();

        Assert.NotNull(pdfBytes);
        Assert.True(pdfBytes.Length > 0);
        Assert.True(pdfBytes[0] == (byte)'%' && pdfBytes[1] == (byte)'P' && pdfBytes[2] == (byte)'D' && pdfBytes[3] == (byte)'F');
    }

    [Fact]
    public void TestGerarPDFComItensServico()
    {
        var openNFSe = SetupOpenNFSe.Ginfes;
        openNFSe.NotasServico.Clear();

        var nota = openNFSe.NotasServico.AddNew();
        nota.IdentificacaoNFSe.Numero = "54321";
        nota.IdentificacaoNFSe.DataEmissao = DateTime.Now;
        nota.IdentificacaoNFSe.Chave = "XYZ987654321";

        nota.Prestador.CpfCnpj = "44818198000190";
        nota.Prestador.RazaoSocial = "EMPRESA PRESTADORA DE SERVICOS LTDA";

        nota.Tomador.CpfCnpj = "12345678000195";
        nota.Tomador.RazaoSocial = "CLIENTE TOMADOR DE SERVICOS S/A";

        nota.Servico.ItemListaServico = "07.02";
        nota.Servico.Valores.ValorServicos = 3000.00M;
        nota.Servico.Valores.BaseCalculo = 3000.00M;
        nota.Servico.Valores.Aliquota = 2.00M;
        nota.Servico.Valores.ValorIss = 60.00M;
        nota.Servico.Valores.ValorLiquidoNfse = 2940.00M;

        var item1 = nota.Servico.ItemsServico.AddNew();
        item1.Descricao = "Serviço de instalação de painéis";
        item1.Quantidade = 2;
        item1.ValorUnitario = 1000.00M;
        item1.ValorTotal = 2000.00M;

        var item2 = nota.Servico.ItemsServico.AddNew();
        item2.Descricao = "Serviço de configuração e testes";
        item2.Quantidade = 1;
        item2.ValorUnitario = 1000.00M;
        item2.ValorTotal = 1000.00M;

        using var ms = new MemoryStream();
        openNFSe.ImprimirPDF(ms, o =>
        {
            o.CabecalhoLinha1 = "PREFEITURA MUNICIPAL DE TESTE";
            o.CabecalhoLinha2 = "SECRETARIA MUNICIPAL DE FAZENDA";
            o.Homologacao = true;
        });

        var pdfBytes = ms.ToArray();
        Assert.NotNull(pdfBytes);
        Assert.True(pdfBytes.Length > 0);
    }

    [Fact]
    public void GerarExemplosArquivosPDF()
    {
        Directory.CreateDirectory(OutDir);

        var openNFSe = SetupOpenNFSe.Ginfes;

        // 1. Exemplo Padrão com Discriminação de Serviços em Texto Livre
        openNFSe.NotasServico.Clear();
        var nota1 = openNFSe.NotasServico.AddNew();
        nota1.IdentificacaoNFSe.Numero = "20260000000105";
        nota1.IdentificacaoNFSe.DataEmissao = new DateTime(2026, 8, 16, 14, 35, 10);
        nota1.IdentificacaoNFSe.Chave = "K9B8-4X7Z-2M1Q-987A";
        nota1.IdentificacaoRps.Numero = "105";
        nota1.IdentificacaoRps.Serie = "1";
        nota1.IdentificacaoRps.DataEmissao = new DateTime(2026, 8, 16);
        nota1.LinkNFSe = "https://nfe.prefeitura.sp.gov.br/verificacao.aspx?ccm=12345678&nf=20260000000105&cv=K9B84X7Z2M1Q";

        nota1.Prestador.CpfCnpj = "12345678000195";
        nota1.Prestador.InscricaoMunicipal = "45678901";
        nota1.Prestador.RazaoSocial = "TECH SOLUTIONS DESENVOLVIMENTO DE SOFTWARE LTDA";
        nota1.Prestador.NomeFantasia = "TECH SOLUTIONS";
        nota1.Prestador.Endereco.Logradouro = "Avenida Paulista";
        nota1.Prestador.Endereco.Numero = "1000";
        nota1.Prestador.Endereco.Complemento = "Conjunto 101";
        nota1.Prestador.Endereco.Bairro = "Bela Vista";
        nota1.Prestador.Endereco.Municipio = "SÃO PAULO";
        nota1.Prestador.Endereco.Uf = "SP";
        nota1.Prestador.Endereco.Cep = "01310100";
        nota1.Prestador.DadosContato.Email = "contato@techsolutions.com.br";
        nota1.Prestador.DadosContato.Telefone = "(11) 3214-5500";

        nota1.Tomador.CpfCnpj = "98765432000110";
        nota1.Tomador.InscricaoMunicipal = "12345678";
        nota1.Tomador.InscricaoEstadual = "110220330111";
        nota1.Tomador.RazaoSocial = "INDÚSTRIA E COMÉRCIO GLOBAL BRASIL S/A";
        nota1.Tomador.Endereco.Logradouro = "Rua Vergueiro";
        nota1.Tomador.Endereco.Numero = "2500";
        nota1.Tomador.Endereco.Bairro = "Vila Mariana";
        nota1.Tomador.Endereco.Municipio = "SÃO PAULO";
        nota1.Tomador.Endereco.Uf = "SP";
        nota1.Tomador.Endereco.Cep = "04102000";
        nota1.Tomador.DadosContato.Email = "fiscal@globalbrasil.com.br";
        nota1.Tomador.DadosContato.Telefone = "(11) 5080-9000";

        nota1.Servico.ItemListaServico = "01.07";
        nota1.DescricaoCodigoTributacaoMunicipio = "Suporte técnico, manutenção e outros serviços em tecnologia da informação";
        nota1.Servico.Discriminacao = "PRESTAÇÃO DE SERVIÇOS DE CONSULTORIA E SUPORTE TÉCNICO ESPECIALIZADO EM ARQUITETURA DE SOFTWARE E NUVEM.\n" +
                                     "- Análise de infraestrutura e migração para ambiente Cloud.\n" +
                                     "- Refatoração de microserviços e suporte N3 aos sistemas internos.\n" +
                                     "- Horas técnicas faturadas: 40 horas conforme Ordem de Serviço nº 2026/894.\n\n" +
                                     "Dados para pagamento via PIX: pix@techsolutions.com.br\n" +
                                     "Banco: 260 - Nu Pagamentos | Agência: 0001 | Conta: 1234567-8";

        nota1.Servico.Valores.ValorServicos = 8500.00M;
        nota1.Servico.Valores.BaseCalculo = 8500.00M;
        nota1.Servico.Valores.Aliquota = 2.00M;
        nota1.Servico.Valores.ValorIss = 170.00M;
        nota1.Servico.Valores.ValorPis = 55.25M;
        nota1.Servico.Valores.ValorCofins = 255.00M;
        nota1.Servico.Valores.ValorInss = 0.00M;
        nota1.Servico.Valores.ValorIr = 127.50M;
        nota1.Servico.Valores.ValorCsll = 85.00M;
        nota1.Servico.Valores.ValorLiquidoNfse = 7977.25M;

        nota1.OutrasInformacoes = "DOCUMENTO EMITIDO POR ME OU EPP OPTANTE PELO SIMPLES NACIONAL. NÃO GERA DIREITO A CRÉDITO FISCAL DE IPI.\n" +
                                  "Val Aprox Tributos: Fed R$ 1.143,25 (13.45%), Est R$ 0,00 (0.00%), Mun R$ 170,00 (2.00%) Fonte: IBPT.";

        var path1 = Path.Combine(OutDir, "danfse_padrao_texto_livre.pdf");
        openNFSe.ImprimirPDF(path1, o =>
        {
            o.CabecalhoLinha1 = "PREFEITURA DO MUNICÍPIO DE SÃO PAULO";
            o.CabecalhoLinha2 = "SECRETARIA MUNICIPAL DA FAZENDA";
            o.ExibirQRCode = true;
            o.Homologacao = false;
            o.MensagemRodape = "Desenvolvido por OpenAC.Net | www.openac.net.br";
        });
        Assert.True(File.Exists(path1));

        // 2. Exemplo com Tabela de Itens
        openNFSe.NotasServico.Clear();
        var nota2 = openNFSe.NotasServico.AddNew();
        nota2.IdentificacaoNFSe.Numero = "20260000000106";
        nota2.IdentificacaoNFSe.DataEmissao = new DateTime(2026, 8, 16, 16, 00, 00);
        nota2.IdentificacaoNFSe.Chave = "M7N6-3V2X-8W9Q-123B";
        nota2.IdentificacaoRps.Numero = "106";
        nota2.IdentificacaoRps.Serie = "1";
        nota2.IdentificacaoRps.DataEmissao = new DateTime(2026, 8, 16);
        nota2.LinkNFSe = "https://nfe.prefeitura.sp.gov.br/verificacao.aspx?ccm=12345678&nf=20260000000106&cv=M7N63V2X8W9Q";

        nota2.Prestador.CpfCnpj = "12345678000195";
        nota2.Prestador.InscricaoMunicipal = "45678901";
        nota2.Prestador.RazaoSocial = "TECH SOLUTIONS DESENVOLVIMENTO DE SOFTWARE LTDA";
        nota2.Prestador.Endereco.Logradouro = "Avenida Paulista";
        nota2.Prestador.Endereco.Numero = "1000";
        nota2.Prestador.Endereco.Bairro = "Bela Vista";
        nota2.Prestador.Endereco.Municipio = "SÃO PAULO";
        nota2.Prestador.Endereco.Uf = "SP";
        nota2.Prestador.Endereco.Cep = "01310100";

        nota2.Tomador.CpfCnpj = "98765432000110";
        nota2.Tomador.RazaoSocial = "INDÚSTRIA E COMÉRCIO GLOBAL BRASIL S/A";
        nota2.Tomador.Endereco.Logradouro = "Rua Vergueiro";
        nota2.Tomador.Endereco.Numero = "2500";
        nota2.Tomador.Endereco.Bairro = "Vila Mariana";
        nota2.Tomador.Endereco.Municipio = "SÃO PAULO";
        nota2.Tomador.Endereco.Uf = "SP";
        nota2.Tomador.Endereco.Cep = "04102000";

        nota2.Servico.ItemListaServico = "01.01";
        nota2.DescricaoCodigoTributacaoMunicipio = "Análise e desenvolvimento de sistemas";

        var it1 = nota2.Servico.ItemsServico.AddNew();
        it1.Descricao = "Desenvolvimento de módulo de integração REST API";
        it1.Quantidade = 1;
        it1.ValorUnitario = 3500.00M;
        it1.ValorTotal = 3500.00M;

        var it2 = nota2.Servico.ItemsServico.AddNew();
        it2.Descricao = "Configuração de esteira CI/CD e pipelines DevOps";
        it2.Quantidade = 2;
        it2.ValorUnitario = 1200.00M;
        it2.ValorTotal = 2400.00M;

        var it3 = nota2.Servico.ItemsServico.AddNew();
        it3.Descricao = "Treinamento especializado de equipe técnica";
        it3.Quantidade = 1;
        it3.ValorUnitario = 1600.00M;
        it3.ValorTotal = 1600.00M;

        var it4 = nota2.Servico.ItemsServico.AddNew();
        it4.Descricao = "Auditoria de segurança e testes de penetração";
        it4.Quantidade = 1;
        it4.ValorUnitario = 2500.00M;
        it4.ValorTotal = 2500.00M;

        nota2.Servico.Valores.ValorServicos = 10000.00M;
        nota2.Servico.Valores.BaseCalculo = 10000.00M;
        nota2.Servico.Valores.Aliquota = 3.00M;
        nota2.Servico.Valores.ValorIss = 300.00M;
        nota2.Servico.Valores.ValorPis = 65.00M;
        nota2.Servico.Valores.ValorCofins = 300.00M;
        nota2.Servico.Valores.ValorIr = 150.00M;
        nota2.Servico.Valores.ValorCsll = 100.00M;
        nota2.Servico.Valores.ValorLiquidoNfse = 9385.00M;

        nota2.OutrasInformacoes = "Documento emitido por ME ou EPP optante pelo Simples Nacional.";

        var path2 = Path.Combine(OutDir, "danfse_com_tabela_de_itens.pdf");
        openNFSe.ImprimirPDF(path2, o =>
        {
            o.CabecalhoLinha1 = "PREFEITURA DO MUNICÍPIO DE SÃO PAULO";
            o.CabecalhoLinha2 = "SECRETARIA MUNICIPAL DA FAZENDA";
            o.ExibirQRCode = true;
            o.Homologacao = false;
        });
        Assert.True(File.Exists(path2));

        // 3. Exemplo de Homologação (Marca d'água SEM VALOR FISCAL / AMBIENTE DE HOMOLOGAÇÃO)
        var path3 = Path.Combine(OutDir, "danfse_homologacao.pdf");
        openNFSe.ImprimirPDF(path3, o =>
        {
            o.CabecalhoLinha1 = "PREFEITURA MUNICIPAL";
            o.CabecalhoLinha2 = "SECRETARIA MUNICIPAL DE FINANÇAS";
            o.Homologacao = true;
        });
        Assert.True(File.Exists(path3));

        // 4. Exemplo de Nota Cancelada (Marca d'água NOTA CANCELADA)
        var path4 = Path.Combine(OutDir, "danfse_cancelada.pdf");
        openNFSe.ImprimirPDF(path4, o =>
        {
            o.CabecalhoLinha1 = "PREFEITURA MUNICIPAL";
            o.CabecalhoLinha2 = "SECRETARIA MUNICIPAL DE FINANÇAS";
            o.Cancelada = true;
        });
        Assert.True(File.Exists(path4));

        // 5. Exemplo de Nota com Muitos Itens (Folha de Continuação)
        openNFSe.NotasServico.Clear();
        var notaLonga = openNFSe.NotasServico.AddNew();
        notaLonga.IdentificacaoNFSe.Numero = "20260000000999";
        notaLonga.IdentificacaoNFSe.DataEmissao = DateTime.Now;
        notaLonga.IdentificacaoNFSe.Chave = "CONT-9876-5432-1000";
        notaLonga.Prestador.CpfCnpj = "12345678000195";
        notaLonga.Prestador.RazaoSocial = "TECH SOLUTIONS DESENVOLVIMENTO DE SOFTWARE LTDA";
        notaLonga.Tomador.CpfCnpj = "98765432000110";
        notaLonga.Tomador.RazaoSocial = "GLOBAL DISTRIBUIDORA DE ALIMENTOS S.A.";
        notaLonga.Servico.ItemListaServico = "01.07";
        notaLonga.Servico.Valores.ValorServicos = 50000.00M;
        notaLonga.Servico.Valores.BaseCalculo = 50000.00M;
        notaLonga.Servico.Valores.Aliquota = 5.00M;
        notaLonga.Servico.Valores.ValorIss = 2500.00M;
        notaLonga.Servico.Valores.ValorLiquidoNfse = 47500.00M;

        for (int i = 1; i <= 35; i++)
        {
            var it = notaLonga.Servico.ItemsServico.AddNew();
            it.Descricao = $"Item {i:D2}: Serviço técnico especializado de implantação de infraestrutura e suporte.";
            it.Quantidade = 1;
            it.ValorUnitario = 1000.00M;
            it.ValorTotal = 1000.00M;
        }

        var path5 = Path.Combine(OutDir, "danfse_com_folha_continuacao.pdf");
        openNFSe.ImprimirPDF(path5, o =>
        {
            o.CabecalhoLinha1 = "PREFEITURA DO MUNICÍPIO DE SÃO PAULO";
            o.CabecalhoLinha2 = "SECRETARIA MUNICIPAL DA FAZENDA";
            o.ExibirQRCode = true;
        });
        Assert.True(File.Exists(path5));
    }

    [Fact]
    public void TestGerarPDFComMuitosItensServico_GeraPaginaDeContinuacao()
    {
        var openNFSe = SetupOpenNFSe.Ginfes;
        openNFSe.NotasServico.Clear();

        var nota = openNFSe.NotasServico.AddNew();
        nota.IdentificacaoNFSe.Numero = "99901";
        nota.IdentificacaoNFSe.DataEmissao = DateTime.Now;
        nota.IdentificacaoNFSe.Chave = "CHAVE99901";
        nota.Prestador.CpfCnpj = "44818198000190";
        nota.Prestador.RazaoSocial = "EMPRESA PRESTADORA LTDA";
        nota.Tomador.CpfCnpj = "12345678000195";
        nota.Tomador.RazaoSocial = "CLIENTE TOMADOR S/A";
        nota.Servico.ItemListaServico = "07.02";

        for (int i = 1; i <= 30; i++)
        {
            var item = nota.Servico.ItemsServico.AddNew();
            item.Descricao = $"Item {i:D2}: Prestação de serviços de manutenção técnica preventiva com relatório.";
            item.Quantidade = 1;
            item.ValorUnitario = 100.00M;
            item.ValorTotal = 100.00M;
        }

        using var ms = new MemoryStream();
        openNFSe.ImprimirPDF(ms);

        using var readMs = new MemoryStream(ms.ToArray());
        using var pdfDoc = PdfSharp.Pdf.IO.PdfReader.Open(readMs, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);

        Assert.True(pdfDoc.PageCount > 1);
    }

    [Fact]
    public void TestGerarPDFComTextoLongoDiscriminacao_GeraPaginaDeContinuacao()
    {
        var openNFSe = SetupOpenNFSe.Ginfes;
        openNFSe.NotasServico.Clear();

        var nota = openNFSe.NotasServico.AddNew();
        nota.IdentificacaoNFSe.Numero = "99902";
        nota.IdentificacaoNFSe.DataEmissao = DateTime.Now;
        nota.IdentificacaoNFSe.Chave = "CHAVE99902";
        nota.Prestador.CpfCnpj = "44818198000190";
        nota.Prestador.RazaoSocial = "EMPRESA PRESTADORA LTDA";
        nota.Tomador.CpfCnpj = "12345678000195";
        nota.Tomador.RazaoSocial = "CLIENTE TOMADOR S/A";
        nota.Servico.ItemListaServico = "01.07";

        var sb = new System.Text.StringBuilder();
        for (int i = 1; i <= 80; i++)
        {
            sb.AppendLine($"Linha {i:D2}: Descrição detalhada dos serviços prestados ao longo do contrato de consultoria e suporte técnico.");
        }
        nota.Servico.Discriminacao = sb.ToString();

        using var ms = new MemoryStream();
        openNFSe.ImprimirPDF(ms);

        using var readMs = new MemoryStream(ms.ToArray());
        using var pdfDoc = PdfSharp.Pdf.IO.PdfReader.Open(readMs, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);

        Assert.True(pdfDoc.PageCount > 1);
    }

    [Fact]
    public void TestGerarPDFComTextoLongoOutrasInformacoes_GeraPaginaDeContinuacao()
    {
        var openNFSe = SetupOpenNFSe.Ginfes;
        openNFSe.NotasServico.Clear();

        var nota = openNFSe.NotasServico.AddNew();
        nota.IdentificacaoNFSe.Numero = "99903";
        nota.IdentificacaoNFSe.DataEmissao = DateTime.Now;
        nota.IdentificacaoNFSe.Chave = "CHAVE99903";
        nota.Prestador.CpfCnpj = "44818198000190";
        nota.Prestador.RazaoSocial = "EMPRESA PRESTADORA LTDA";
        nota.Tomador.CpfCnpj = "12345678000195";
        nota.Tomador.RazaoSocial = "CLIENTE TOMADOR S/A";
        nota.Servico.ItemListaServico = "01.07";

        var sb = new System.Text.StringBuilder();
        for (int i = 1; i <= 80; i++)
        {
            sb.AppendLine($"Cláusula {i:D2}: Informações adicionais, detalhes de convênio, retenções contratuais e observações fiscais.");
        }
        nota.OutrasInformacoes = sb.ToString();

        using var ms = new MemoryStream();
        openNFSe.ImprimirPDF(ms);

        using var readMs = new MemoryStream(ms.ToArray());
        using var pdfDoc = PdfSharp.Pdf.IO.PdfReader.Open(readMs, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);

        Assert.True(pdfDoc.PageCount > 1);
    }
}
