// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.DANFSe.PDFSharp
// Author           : RFTD / OpenAC.Net Team
// Created          : 2026-08-16
// ***********************************************************************
// <copyright file="DANFSeA4RetratoReport.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Globalization;
using System.Text;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Common;
using OpenAC.Net.NFSe.DANFSe.PDFSharp.Configuracao;
using OpenAC.Net.NFSe.Nota;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace OpenAC.Net.NFSe.DANFSe.PDFSharp.Layout;

/// <summary>
/// Motor de renderização do DANFSe A4 Retrato em PDF (PDFsharp), convertido a partir do modelo ACBrDANFSeX FPDF A4 Retrato.
/// </summary>
internal sealed class DANFSeA4RetratoReport
{
    #region Fields

    private readonly NotaServico nota;
    private readonly DANFSePDFSharpOptions config;
    private static readonly CultureInfo PtBr = new("pt-BR");

    private double xMm;
    private double yMm;
    private double largUtilMm;

    #endregion Fields

    #region Constructors

    public DANFSeA4RetratoReport(NotaServico nota, DANFSePDFSharpOptions config)
    {
        this.nota = nota ?? throw new ArgumentNullException(nameof(nota));
        this.config = config ?? throw new ArgumentNullException(nameof(config));
    }

    #endregion Constructors

    #region Public Render Method

    public void Render(PdfPage page)
    {
        using var gfx = XGraphics.FromPdfPage(page);

        xMm = config.MargemHorizontalMm;
        yMm = config.MargemVerticalMm;
        largUtilMm = DANFSeConstantes.PaginaLarguraMm - (2 * config.MargemHorizontalMm);

        // 1. Cabeçalho (Prefeitura, Dados da Nota, QR Code)
        DesenharBlocoCabecalho(gfx);

        // 2. Prestador de Serviços
        DesenharBlocoPrestador(gfx);

        // 3. Tomador de Serviços
        DesenharBlocoTomador(gfx);

        // Cálculo da altura necessária para o rodapé (Valores, Outras Informações, Linha de Rodapé)
        var hValores = 38.0;
        var hOutrasInfo = CalcularAlturaBlocoOutrasInformacoes(gfx);
        var hRodape = DANFSeConstantes.AlturaRodapeMm;
        var hTotalRodape = hValores + hOutrasInfo + hRodape;

        var yLimiteRodape = (DANFSeConstantes.PaginaAlturaMm - config.MargemVerticalMm) - hTotalRodape;
        var hDiscriminacao = Math.Max(yLimiteRodape - yMm, 20.0);

        // 4. Discriminação dos Serviços / Itens
        if (nota.Servico.ItemsServico.Count == 0)
            DesenharBlocoDiscriminacaoServico(gfx, hDiscriminacao);
        else
            DesenharBlocoItens(gfx, hDiscriminacao);

        yMm = yLimiteRodape;

        // 5. Bloco de Valores e Impostos
        DesenharBlocoValores(gfx);

        // 6. Bloco de Outras Informações
        DesenharBlocoOutrasInformacoes(gfx, hOutrasInfo);

        // 7. Rodapé do Relatório
        DesenharBlocoRodape(gfx);

        // 8. Marca d'água (Homologação ou Cancelada)
        DesenharBlocoWatermark(gfx);
    }

    #endregion Public Render Method

    #region Layout Blocks

    private void DesenharBlocoCabecalho(XGraphics gfx)
    {
        var hCab = DANFSeConstantes.AlturaCabecalhoMm;
        var prefeituraLogo = ObterLogoPrefeituraBytes();
        var wLogo = prefeituraLogo != null && prefeituraLogo.Length > 0 ? 20.0 : 0.0;
        var temQrCode = config.ExibirQRCode && !string.IsNullOrWhiteSpace(nota.LinkNFSe);
        var wQRCode = temQrCode ? 24.0 : 0.0;
        var wDadosNota = 50.0;
        var wTitulo = largUtilMm - wQRCode - wDadosNota;

        // Caixa da Prefeitura / Título
        PdfDrawHelper.DesenharRetangulo(gfx, xMm, yMm, wTitulo, hCab);

        var xTitle = xMm;
        var wTitleText = wTitulo;

        if (wLogo > 0)
        {
            PdfDrawHelper.DesenharImagem(gfx, xMm + 2.0, yMm + 2.0, 18.0, 18.0, prefeituraLogo);
            xTitle = xMm + wLogo;
            wTitleText = wTitulo - wLogo;
        }

        var linha1 = !string.IsNullOrWhiteSpace(config.CabecalhoLinha1)
            ? config.CabecalhoLinha1.ToUpper()
            : DANFSeConstantes.MsgCabecalhoLinha1Padrao;

        var linha2 = !string.IsNullOrWhiteSpace(config.CabecalhoLinha2)
            ? config.CabecalhoLinha2.ToUpper()
            : DANFSeConstantes.MsgCabecalhoLinha2Padrao;

        var textoTitulo = $"{linha1}\n{linha2}\n{DANFSeConstantes.MsgTituloDanfse}";

        // Desenha Prefeitura, Secretaria e Título com wordwrap automático, ajuste progressivo de fonte e centralização perfeita
        PdfDrawHelper.DesenharTextoAjustado(
            gfx,
            xTitle,
            yMm + 0.8,
            wTitleText,
            18.2,
            textoTitulo,
            maxFontSizePt: 9.0,
            minFontSizePt: 5.5,
            negrito: true,
            alinhamento: XStringAlignment.Center
        );

        // Sub-box: RPS / Série
        var dataRps = nota.IdentificacaoRps.DataEmissao != DateTime.MinValue
            ? nota.IdentificacaoRps.DataEmissao.ToString("dd/MM/yyyy")
            : "";
        var textoRps = $"RPS/SÉRIE: {nota.IdentificacaoRps.Numero}/{nota.IdentificacaoRps.Serie} ({dataRps})";
        var rectRps = new XRect(PdfDrawHelper.MmToPt(xTitle), PdfDrawHelper.MmToPt(yMm + 19.5), PdfDrawHelper.MmToPt(wTitleText), PdfDrawHelper.MmToPt(4.5));
        var fontRps = new XFont(DANFSeConstantes.FontePadrao, 7.5, XFontStyleEx.Regular);
        var formatCenter = new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center };
        gfx.DrawString(textoRps, fontRps, PdfDrawHelper.BrushPreto, rectRps, formatCenter);

        // Caixa de Dados da Nota (3 linhas empilhadas)
        var xDados = xMm + wTitulo;
        var hCell = hCab / 3.0;

        var numNota = string.IsNullOrWhiteSpace(nota.IdentificacaoNFSe.Numero)
            ? ""
            : (nota.IdentificacaoNFSe.Numero.Length < 8 ? nota.IdentificacaoNFSe.Numero.PadLeft(8, '0') : nota.IdentificacaoNFSe.Numero);
        PdfDrawHelper.DesenharCampo(gfx, xDados, yMm, wDadosNota, hCell, "NÚMERO DA NOTA", numNota, negrito: true, alinhamento: XStringAlignment.Center, fontValorPt: 10.0);

        var dataEmissao = nota.IdentificacaoNFSe.DataEmissao != DateTime.MinValue
            ? nota.IdentificacaoNFSe.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss")
            : (nota.IdentificacaoRps.DataEmissao != DateTime.MinValue ? nota.IdentificacaoRps.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss") : "");
        PdfDrawHelper.DesenharCampo(gfx, xDados, yMm + hCell, wDadosNota, hCell, "DATA/HORA DE EMISSÃO", dataEmissao, negrito: true, alinhamento: XStringAlignment.Center, fontValorPt: 9.0);

        var codVerificacao = !string.IsNullOrWhiteSpace(nota.IdentificacaoNFSe.Chave)
            ? nota.IdentificacaoNFSe.Chave
            : (!string.IsNullOrWhiteSpace(nota.IdentificacaoNFSe.ChaveNotaNacional) ? nota.IdentificacaoNFSe.ChaveNotaNacional : nota.Protocolo);
        PdfDrawHelper.DesenharCampo(gfx, xDados, yMm + (2 * hCell), wDadosNota, hCell, "CÓDIGO DE VERIFICAÇÃO", codVerificacao ?? "", negrito: true, alinhamento: XStringAlignment.Center, fontValorPt: 8.5);

        // Caixa do QR Code
        if (temQrCode)
        {
            var xQr = xDados + wDadosNota;
            PdfDrawHelper.DesenharRetangulo(gfx, xQr, yMm, wQRCode, hCab);
            PdfDrawHelper.DesenharQrCode(gfx, xQr + 2.0, yMm + 3.0, DANFSeConstantes.QrCodeTamanhoMm, nota.LinkNFSe);
        }

        yMm += hCab;
    }

    private void DesenharBlocoPrestador(XGraphics gfx)
    {
        var hBloco = DANFSeConstantes.AlturaPrestadorMm;
        PdfDrawHelper.DesenharRetangulo(gfx, xMm, yMm, largUtilMm, hBloco);
        PdfDrawHelper.DesenharTituloBloco(gfx, xMm, yMm, largUtilMm, 4.0, "PRESTADOR DE SERVIÇOS", preencherSombreado: false, alinhamento: XStringAlignment.Center);

        var yContent = yMm + 4.0;
        var prestadorLogo = ObterLogoPrestadorBytes();
        var wLogo = prestadorLogo != null && prestadorLogo.Length > 0 ? 24.0 : 0.0;

        var xContent = xMm + 2.0;
        if (wLogo > 0)
        {
            PdfDrawHelper.DesenharImagem(gfx, xMm + 2.0, yContent + 1.0, 20.0, 18.0, prestadorLogo);
            xContent = xMm + wLogo + 1.0;
        }

        var incY = 5.0;
        var yLinha = yContent + 1.0;

        // Linha 1: CPF / CNPJ e Inscrição Municipal
        var xLinha = xContent;
        xLinha += PdfDrawHelper.DesenharCampoInline(gfx, xLinha, yLinha, "CPF / CNPJ: ", PdfDrawHelper.FormatarCNPJouCPF(nota.Prestador.CpfCnpj)) + 6.0;
        PdfDrawHelper.DesenharCampoInline(gfx, xLinha, yLinha, "INSC. MUNICIPAL: ", nota.Prestador.InscricaoMunicipal ?? "");

        // Linha 2: Razão Social
        yLinha += incY;
        var razaoSocial = !string.IsNullOrWhiteSpace(nota.Prestador.RazaoSocial) ? nota.Prestador.RazaoSocial : nota.Prestador.NomeFantasia;
        PdfDrawHelper.DesenharCampoInline(gfx, xContent, yLinha, "NOME / RAZÃO SOCIAL: ", razaoSocial ?? "");

        // Linha 3: Endereço
        yLinha += incY;
        var logr = !string.IsNullOrWhiteSpace(nota.Prestador.Endereco.Logradouro) ? nota.Prestador.Endereco.Logradouro : nota.Prestador.Endereco.TipoLogradouro;
        var num = !string.IsNullOrWhiteSpace(nota.Prestador.Endereco.Numero) ? $", {nota.Prestador.Endereco.Numero}" : "";
        var compl = !string.IsNullOrWhiteSpace(nota.Prestador.Endereco.Complemento) ? $" - {nota.Prestador.Endereco.Complemento}" : "";
        var bairro = !string.IsNullOrWhiteSpace(nota.Prestador.Endereco.Bairro) ? $" - {nota.Prestador.Endereco.Bairro}" : "";
        var cep = !string.IsNullOrWhiteSpace(nota.Prestador.Endereco.Cep) ? $" - CEP {PdfDrawHelper.FormatarCEP(nota.Prestador.Endereco.Cep)}" : "";
        var enderecoCompleto = $"{logr}{num}{compl}{bairro}{cep}";
        PdfDrawHelper.DesenharCampoInline(gfx, xContent, yLinha, "ENDEREÇO: ", enderecoCompleto);

        // Linha 4: Município, E-mail e Telefone
        yLinha += incY;
        xLinha = xContent;
        var mun = !string.IsNullOrWhiteSpace(nota.Prestador.Endereco.Municipio)
            ? nota.Prestador.Endereco.Municipio.ToUpper()
            : (nota.Prestador.Endereco.CodigoMunicipio > 0 ? nota.Prestador.Endereco.CodigoMunicipio.ToString() : "");
        var munUf = $"{mun} / {nota.Prestador.Endereco.Uf}";
        xLinha += PdfDrawHelper.DesenharCampoInline(gfx, xLinha, yLinha, "MUNICÍPIO: ", munUf) + 6.0;

        if (!string.IsNullOrWhiteSpace(nota.Prestador.DadosContato.Email))
            xLinha += PdfDrawHelper.DesenharCampoInline(gfx, xLinha, yLinha, "EMAIL: ", nota.Prestador.DadosContato.Email.ToLower()) + 6.0;

        if (!string.IsNullOrWhiteSpace(nota.Prestador.DadosContato.Telefone))
            PdfDrawHelper.DesenharCampoInline(gfx, xLinha, yLinha, "TELEFONE: ", nota.Prestador.DadosContato.Telefone);

        yMm += hBloco;
    }

    private void DesenharBlocoTomador(XGraphics gfx)
    {
        var hBloco = DANFSeConstantes.AlturaTomadorMm;
        PdfDrawHelper.DesenharRetangulo(gfx, xMm, yMm, largUtilMm, hBloco);
        PdfDrawHelper.DesenharTituloBloco(gfx, xMm, yMm, largUtilMm, 4.0, "TOMADOR DE SERVIÇOS", preencherSombreado: false, alinhamento: XStringAlignment.Center);

        var yContent = yMm + 4.0;
        var xContent = xMm + 2.0;

        var incY = 5.0;
        var yLinha = yContent + 1.0;

        // Linha 1: CPF / CNPJ, Inscrição Municipal e Inscrição Estadual
        var xLinha = xContent;
        xLinha += PdfDrawHelper.DesenharCampoInline(gfx, xLinha, yLinha, "CPF / CNPJ: ", PdfDrawHelper.FormatarCNPJouCPF(nota.Tomador.CpfCnpj)) + 6.0;
        xLinha += PdfDrawHelper.DesenharCampoInline(gfx, xLinha, yLinha, "INSC. MUNICIPAL: ", nota.Tomador.InscricaoMunicipal ?? "") + 6.0;
        if (!string.IsNullOrWhiteSpace(nota.Tomador.InscricaoEstadual))
            PdfDrawHelper.DesenharCampoInline(gfx, xLinha, yLinha, "INSC. ESTADUAL: ", nota.Tomador.InscricaoEstadual);

        // Linha 2: Razão Social
        yLinha += incY;
        var razaoSocial = !string.IsNullOrWhiteSpace(nota.Tomador.RazaoSocial) ? nota.Tomador.RazaoSocial : nota.Tomador.NomeFantasia;
        PdfDrawHelper.DesenharCampoInline(gfx, xContent, yLinha, "NOME / RAZÃO SOCIAL: ", razaoSocial ?? "");

        // Linha 3: Endereço
        yLinha += incY;
        var logr = !string.IsNullOrWhiteSpace(nota.Tomador.Endereco.Logradouro) ? nota.Tomador.Endereco.Logradouro : nota.Tomador.Endereco.TipoLogradouro;
        var num = !string.IsNullOrWhiteSpace(nota.Tomador.Endereco.Numero) ? $", {nota.Tomador.Endereco.Numero}" : "";
        var compl = !string.IsNullOrWhiteSpace(nota.Tomador.Endereco.Complemento) ? $" - {nota.Tomador.Endereco.Complemento}" : "";
        var bairro = !string.IsNullOrWhiteSpace(nota.Tomador.Endereco.Bairro) ? $" - {nota.Tomador.Endereco.Bairro}" : "";
        var cep = !string.IsNullOrWhiteSpace(nota.Tomador.Endereco.Cep) ? $" - CEP {PdfDrawHelper.FormatarCEP(nota.Tomador.Endereco.Cep)}" : "";
        var enderecoCompleto = $"{logr}{num}{compl}{bairro}{cep}";
        PdfDrawHelper.DesenharCampoInline(gfx, xContent, yLinha, "ENDEREÇO: ", enderecoCompleto);

        // Linha 4: Município, E-mail e Telefone
        yLinha += incY;
        xLinha = xContent;
        var mun = !string.IsNullOrWhiteSpace(nota.Tomador.Endereco.Municipio)
            ? nota.Tomador.Endereco.Municipio.ToUpper()
            : (nota.Tomador.Endereco.CodigoMunicipio > 0 ? nota.Tomador.Endereco.CodigoMunicipio.ToString() : "");
        var munUf = $"{mun} / {nota.Tomador.Endereco.Uf}";
        xLinha += PdfDrawHelper.DesenharCampoInline(gfx, xLinha, yLinha, "MUNICÍPIO: ", munUf) + 6.0;

        if (!string.IsNullOrWhiteSpace(nota.Tomador.DadosContato.Email))
            xLinha += PdfDrawHelper.DesenharCampoInline(gfx, xLinha, yLinha, "EMAIL: ", nota.Tomador.DadosContato.Email.ToLower()) + 6.0;

        if (!string.IsNullOrWhiteSpace(nota.Tomador.DadosContato.Telefone))
            PdfDrawHelper.DesenharCampoInline(gfx, xLinha, yLinha, "TELEFONE: ", nota.Tomador.DadosContato.Telefone);

        yMm += hBloco;
    }

    private void DesenharBlocoDiscriminacaoServico(XGraphics gfx, double altura)
    {
        PdfDrawHelper.DesenharRetangulo(gfx, xMm, yMm, largUtilMm, altura);
        PdfDrawHelper.DesenharTituloBloco(gfx, xMm, yMm, largUtilMm, 4.0, "DISCRIMINAÇÃO DOS SERVIÇOS", preencherSombreado: false, alinhamento: XStringAlignment.Center);

        var texto = ObterTextoDiscriminacaoServicos();
        PdfDrawHelper.DesenharTextoMultiLinhas(gfx, xMm + 2.0, yMm + 5.0, largUtilMm - 4.0, altura - 6.0, texto, fontSizePt: 8.0);

        yMm += altura;
    }

    private void DesenharBlocoItens(XGraphics gfx, double altura)
    {
        PdfDrawHelper.DesenharRetangulo(gfx, xMm, yMm, largUtilMm, altura);
        PdfDrawHelper.DesenharTituloBloco(gfx, xMm, yMm, largUtilMm, 4.0, "DISCRIMINAÇÃO DOS SERVIÇOS", preencherSombreado: false, alinhamento: XStringAlignment.Center);

        var yLinha = yMm + 4.0;
        var hHeader = DANFSeConstantes.AlturaLinhaCabecalhoItemMm;

        var wQtde = Math.Round(largUtilMm * 0.08);
        var wUnit = Math.Round(largUtilMm * 0.15);
        var wTotal = Math.Round(largUtilMm * 0.15);
        var wItem = largUtilMm - wQtde - wUnit - wTotal;

        // Cabeçalhos de Coluna
        var fontCol = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteLabelCampoPt, XFontStyleEx.Bold);
        var formatCenter = new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center };
        var formatLeft = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Center };

        var xCol = xMm;
        gfx.DrawString("ITEM", fontCol, PdfDrawHelper.BrushPreto, new XRect(PdfDrawHelper.MmToPt(xCol + 1.0), PdfDrawHelper.MmToPt(yLinha), PdfDrawHelper.MmToPt(wItem - 2.0), PdfDrawHelper.MmToPt(hHeader)), formatLeft);
        xCol += wItem;

        gfx.DrawString("QTDE.", fontCol, PdfDrawHelper.BrushPreto, new XRect(PdfDrawHelper.MmToPt(xCol), PdfDrawHelper.MmToPt(yLinha), PdfDrawHelper.MmToPt(wQtde), PdfDrawHelper.MmToPt(hHeader)), formatCenter);
        xCol += wQtde;

        gfx.DrawString("VALOR UNITÁRIO (R$)", fontCol, PdfDrawHelper.BrushPreto, new XRect(PdfDrawHelper.MmToPt(xCol), PdfDrawHelper.MmToPt(yLinha), PdfDrawHelper.MmToPt(wUnit), PdfDrawHelper.MmToPt(hHeader)), formatCenter);
        xCol += wUnit;

        gfx.DrawString("VALOR TOTAL (R$)", fontCol, PdfDrawHelper.BrushPreto, new XRect(PdfDrawHelper.MmToPt(xCol), PdfDrawHelper.MmToPt(yLinha), PdfDrawHelper.MmToPt(wTotal), PdfDrawHelper.MmToPt(hHeader)), formatCenter);

        yLinha += hHeader;
        gfx.DrawLine(PdfDrawHelper.PenBorda, PdfDrawHelper.MmToPt(xMm), PdfDrawHelper.MmToPt(yLinha), PdfDrawHelper.MmToPt(xMm + largUtilMm), PdfDrawHelper.MmToPt(yLinha));

        // Linhas de Itens
        var fontItem = new XFont(DANFSeConstantes.FontePadrao, 7.0, XFontStyleEx.Regular);
        var formatRight = new XStringFormat { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.Near };

        foreach (var item in nota.Servico.ItemsServico)
        {
            if (yLinha > yMm + altura - 6.0)
                break;

            var desc = item.Descricao ?? "";
            var sizeDesc = gfx.MeasureString(desc, fontItem);
            var hItem = Math.Max(PdfDrawHelper.PtToMm(sizeDesc.Height) + 1.5, 4.5);

            xCol = xMm;
            gfx.DrawString(desc, fontItem, PdfDrawHelper.BrushPreto, new XRect(PdfDrawHelper.MmToPt(xCol + 1.0), PdfDrawHelper.MmToPt(yLinha + 0.5), PdfDrawHelper.MmToPt(wItem - 2.0), PdfDrawHelper.MmToPt(hItem)), formatLeft);
            xCol += wItem;

            gfx.DrawString(item.Quantidade.ToString("#,##0.00", PtBr), fontItem, PdfDrawHelper.BrushPreto, new XRect(PdfDrawHelper.MmToPt(xCol), PdfDrawHelper.MmToPt(yLinha + 0.5), PdfDrawHelper.MmToPt(wQtde - 1.0), PdfDrawHelper.MmToPt(hItem)), formatRight);
            xCol += wQtde;

            gfx.DrawString(item.ValorUnitario.ToString("#,##0.00", PtBr), fontItem, PdfDrawHelper.BrushPreto, new XRect(PdfDrawHelper.MmToPt(xCol), PdfDrawHelper.MmToPt(yLinha + 0.5), PdfDrawHelper.MmToPt(wUnit - 1.0), PdfDrawHelper.MmToPt(hItem)), formatRight);
            xCol += wUnit;

            gfx.DrawString(item.ValorTotal.ToString("#,##0.00", PtBr), fontItem, PdfDrawHelper.BrushPreto, new XRect(PdfDrawHelper.MmToPt(xCol), PdfDrawHelper.MmToPt(yLinha + 0.5), PdfDrawHelper.MmToPt(wTotal - 1.0), PdfDrawHelper.MmToPt(hItem)), formatRight);

            yLinha += hItem;
            gfx.DrawLine(PdfDrawHelper.PenLinhaTracejada, PdfDrawHelper.MmToPt(xMm), PdfDrawHelper.MmToPt(yLinha), PdfDrawHelper.MmToPt(xMm + largUtilMm), PdfDrawHelper.MmToPt(yLinha));
        }

        yMm += altura;
    }

    private void DesenharBlocoValores(XGraphics gfx)
    {
        var yCurr = yMm;

        // 1. Banner Valor Total da Nota
        var textoTotal = $"VALOR TOTAL DA NOTA = R$ {nota.Servico.Valores.ValorServicos.ToString("#,##0.00", PtBr)}";
        var rectTotal = new XRect(PdfDrawHelper.MmToPt(xMm), PdfDrawHelper.MmToPt(yCurr), PdfDrawHelper.MmToPt(largUtilMm), PdfDrawHelper.MmToPt(6.0));
        gfx.DrawRectangle(PdfDrawHelper.PenBorda, PdfDrawHelper.BrushFundoSombreado, rectTotal);

        var fontTotal = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteValorTotalNotaPt, XFontStyleEx.Bold);
        var formatCenter = new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center };
        gfx.DrawString(textoTotal, fontTotal, PdfDrawHelper.BrushPreto, rectTotal, formatCenter);
        yCurr += 6.0;

        // 2. Código de Classificação do Serviço
        var descClass = !string.IsNullOrWhiteSpace(nota.DescricaoCodigoTributacaoMunicipio)
            ? nota.DescricaoCodigoTributacaoMunicipio
            : (!string.IsNullOrWhiteSpace(nota.Servico.Descricao) ? nota.Servico.Descricao : nota.Servico.ItemListaServico);
        var textoClassificacao = $"{nota.Servico.ItemListaServico} - {descClass}";
        PdfDrawHelper.DesenharCampo(gfx, xMm, yCurr, largUtilMm, 8.0, "CÓDIGO DE CLASSIFICAÇÃO DO SERVIÇO", textoClassificacao, negrito: true, fontValorPt: 9.0);
        yCurr += 8.0;

        // 3. Impostos Federais (5 colunas)
        var wCol5 = largUtilMm / 5.0;
        PdfDrawHelper.DesenharImposto(gfx, xMm, yCurr, wCol5, 8.0, "INSS (R$)", nota.Servico.Valores.ValorInss, PtBr);
        PdfDrawHelper.DesenharImposto(gfx, xMm + wCol5, yCurr, wCol5, 8.0, "IRRF (R$)", nota.Servico.Valores.ValorIr, PtBr);
        PdfDrawHelper.DesenharImposto(gfx, xMm + (2 * wCol5), yCurr, wCol5, 8.0, "CSLL (R$)", nota.Servico.Valores.ValorCsll, PtBr);
        PdfDrawHelper.DesenharImposto(gfx, xMm + (3 * wCol5), yCurr, wCol5, 8.0, "COFINS (R$)", nota.Servico.Valores.ValorCofins, PtBr);
        PdfDrawHelper.DesenharImposto(gfx, xMm + (4 * wCol5), yCurr, wCol5, 8.0, "PIS (R$)", nota.Servico.Valores.ValorPis, PtBr);
        yCurr += 8.0;

        // 4. Deduções e Descontos (4 colunas)
        var wCol4 = largUtilMm / 4.0;
        PdfDrawHelper.DesenharImposto(gfx, xMm, yCurr, wCol4, 8.0, "DEDUÇÃO (R$)", nota.Servico.Valores.ValorDeducoes, PtBr);
        PdfDrawHelper.DesenharImposto(gfx, xMm + wCol4, yCurr, wCol4, 8.0, "DESCONTO INCONDICIONADO (R$)", nota.Servico.Valores.DescontoIncondicionado, PtBr);
        PdfDrawHelper.DesenharImposto(gfx, xMm + (2 * wCol4), yCurr, wCol4, 8.0, "DESCONTO CONDICIONADO (R$)", nota.Servico.Valores.DescontoCondicionado, PtBr);
        PdfDrawHelper.DesenharImposto(gfx, xMm + (3 * wCol4), yCurr, wCol4, 8.0, "OUTRAS RETENÇÕES (R$)", nota.Servico.Valores.OutrasRetencoes, PtBr);
        yCurr += 8.0;

        // 5. Totais Líquidos e ISSQN (5 colunas)
        PdfDrawHelper.DesenharImposto(gfx, xMm, yCurr, wCol5, 8.0, "TOTAL LÍQUIDO DA NOTA (R$)", nota.Servico.Valores.ValorLiquidoNfse, PtBr);
        PdfDrawHelper.DesenharImposto(gfx, xMm + wCol5, yCurr, wCol5, 8.0, "BASE DE CÁLCULO ISSQN (R$)", nota.Servico.Valores.BaseCalculo, PtBr);
        PdfDrawHelper.DesenharImposto(gfx, xMm + (2 * wCol5), yCurr, wCol5, 8.0, "ALÍQUOTA ISSQN (%)", nota.Servico.Valores.Aliquota, PtBr, "#,##0.00");
        PdfDrawHelper.DesenharImposto(gfx, xMm + (3 * wCol5), yCurr, wCol5, 8.0, "VALOR DO ISSQN (R$)", nota.Servico.Valores.ValorIss, PtBr);
        PdfDrawHelper.DesenharImposto(gfx, xMm + (4 * wCol5), yCurr, wCol5, 8.0, "VALOR DO ISSQN RETIDO (R$)", nota.Servico.Valores.ValorIssRetido, PtBr);
        yCurr += 8.0;

        yMm = yCurr;
    }

    private void DesenharBlocoOutrasInformacoes(XGraphics gfx, double altura)
    {
        PdfDrawHelper.DesenharRetangulo(gfx, xMm, yMm, largUtilMm, altura);
        PdfDrawHelper.DesenharTituloBloco(gfx, xMm, yMm, largUtilMm, 4.0, "OUTRAS INFORMAÇÕES", preencherSombreado: false, alinhamento: XStringAlignment.Center);

        var texto = ObterTextoOutrasInformacoes();
        PdfDrawHelper.DesenharTextoMultiLinhas(gfx, xMm + 2.0, yMm + 5.0, largUtilMm - 4.0, altura - 6.0, texto, fontSizePt: 8.0);

        yMm += altura;
    }

    private void DesenharBlocoRodape(XGraphics gfx)
    {
        var fontRodape = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteRodapePt, XFontStyleEx.Italic);
        var yRodape = yMm + 1.0;

        var mensagens = !string.IsNullOrWhiteSpace(config.MensagemRodape)
            ? config.MensagemRodape.Split('|')
            : [];

        var textoEsquerda = mensagens.Length >= 1
            ? mensagens[0]
            : (!string.IsNullOrWhiteSpace(config.SoftwareHouse) ? config.SoftwareHouse : "OpenAC.Net.NFSe - www.openac.net.br");

        var textoCentro = mensagens.Length >= 2 ? mensagens[1] : "";
        var textoDireita = mensagens.Length >= 3 ? mensagens[2] : $"Impresso em {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

        var rectEsquerda = new XRect(PdfDrawHelper.MmToPt(xMm), PdfDrawHelper.MmToPt(yRodape), PdfDrawHelper.MmToPt(largUtilMm / 3.0), PdfDrawHelper.MmToPt(4.0));
        gfx.DrawString(textoEsquerda, fontRodape, PdfDrawHelper.BrushCinzaEscuro, rectEsquerda, XStringFormats.CenterLeft);

        if (!string.IsNullOrWhiteSpace(textoCentro))
        {
            var rectCentro = new XRect(PdfDrawHelper.MmToPt(xMm + (largUtilMm / 3.0)), PdfDrawHelper.MmToPt(yRodape), PdfDrawHelper.MmToPt(largUtilMm / 3.0), PdfDrawHelper.MmToPt(4.0));
            gfx.DrawString(textoCentro, fontRodape, PdfDrawHelper.BrushCinzaEscuro, rectCentro, new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center });
        }

        var rectDireita = new XRect(PdfDrawHelper.MmToPt(xMm + (2 * (largUtilMm / 3.0))), PdfDrawHelper.MmToPt(yRodape), PdfDrawHelper.MmToPt(largUtilMm / 3.0), PdfDrawHelper.MmToPt(4.0));
        gfx.DrawString(textoDireita, fontRodape, PdfDrawHelper.BrushCinzaEscuro, rectDireita, XStringFormats.CenterRight);

        yMm += DANFSeConstantes.AlturaRodapeMm;
    }

    private void DesenharBlocoWatermark(XGraphics gfx)
    {
        var sb = new StringBuilder();

        if (config.Cancelada || config.Homologacao)
        {
            sb.Append(DANFSeConstantes.MsgSemValorFiscal);

            if (config.Cancelada)
                sb.Append("\n").Append(DANFSeConstantes.MsgCancelada);

            if (config.Homologacao)
                sb.Append("\n").Append(DANFSeConstantes.MsgHomologacao);
        }

        var texto = sb.ToString();
        if (!string.IsNullOrWhiteSpace(texto))
        {
            PdfDrawHelper.DesenharMarcaDagua(gfx, DANFSeConstantes.PaginaLarguraMm, DANFSeConstantes.PaginaAlturaMm, texto);
        }
    }

    #endregion Layout Blocks

    #region Helper Methods

    private double CalcularAlturaBlocoOutrasInformacoes(XGraphics gfx)
    {
        var baseHeight = 25.0;
        var texto = ObterTextoOutrasInformacoes();
        if (string.IsNullOrWhiteSpace(texto))
            return baseHeight;

        var altTexto = PdfDrawHelper.MedirAlturaTexto(gfx, texto, largUtilMm - 4.0, fontSizePt: 8.0);
        return Math.Max(baseHeight, altTexto + 6.0);
    }

    private string ObterTextoDiscriminacaoServicos()
    {
        var texto = (nota.Servico.Discriminacao ?? "").Trim();
        if (!string.IsNullOrEmpty(config.QuebraDeLinha))
            texto = texto.Replace(config.QuebraDeLinha, Environment.NewLine);

        return texto;
    }

    private string ObterTextoOutrasInformacoes()
    {
        var sb = new StringBuilder();

        var textoBase = (nota.OutrasInformacoes ?? "").Trim();
        if (string.IsNullOrWhiteSpace(textoBase))
            textoBase = (nota.InformacoesComplementares ?? "").Trim();

        if (!string.IsNullOrEmpty(config.QuebraDeLinha))
            textoBase = textoBase.Replace(config.QuebraDeLinha, Environment.NewLine);

        if (!string.IsNullOrWhiteSpace(textoBase))
            sb.AppendLine(textoBase);

        if (!string.IsNullOrWhiteSpace(nota.DiscriminacaoImpostos))
            sb.AppendLine(nota.DiscriminacaoImpostos);

        return sb.ToString().TrimEnd();
    }

    private byte[]? ObterLogoPrefeituraBytes()
    {
        if (config.LogoPrefeituraBytes != null && config.LogoPrefeituraBytes.Length > 0)
            return config.LogoPrefeituraBytes;

        return config.LogoPrefeitura?.ToByteArray();
    }

    private byte[]? ObterLogoPrestadorBytes()
    {
        if (config.LogoPrestadorBytes != null && config.LogoPrestadorBytes.Length > 0)
            return config.LogoPrestadorBytes;

        return config.Logo?.ToByteArray();
    }

    #endregion Helper Methods
}
