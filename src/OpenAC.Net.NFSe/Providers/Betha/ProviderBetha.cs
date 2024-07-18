// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-22-2018
//
// Last Modified By : Rafael Dias
// Last Modified On : 05-22-2018
// ***********************************************************************
// <copyright file="ProviderBetha.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2023 Projeto OpenAC .Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderBetha : ProviderABRASF
{
    #region Constructors

    public ProviderBetha(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "Betha";
    }

    #endregion Constructors

    #region Methods

    #region Services

    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.Lote == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
        if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
        if (retornoWebservice.Erros.Any()) return;

        var xmlLoteRps = new StringBuilder();

        foreach (var nota in notas)
        {
            var xmlRps = WriteXmlRps(nota, false, false);
            xmlLoteRps.Append(xmlRps);
            GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
        }

        var xmlLote = new StringBuilder();
        xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        xmlLote.Append($"<EnviarLoteRpsEnvio {GetNamespace()}>");
        xmlLote.Append($"<LoteRps Id=\"L{retornoWebservice.Lote}\" xmlns=\"\">");
        xmlLote.Append($"<NumeroLote>{retornoWebservice.Lote}</NumeroLote>");
        xmlLote.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
        xmlLote.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        xmlLote.Append($"<QuantidadeRps>{notas.Count}</QuantidadeRps>");
        xmlLote.Append("<ListaRps>");
        xmlLote.Append(xmlLoteRps);
        xmlLote.Append("</ListaRps>");
        xmlLote.Append("</LoteRps>");
        xmlLote.Append("</EnviarLoteRpsEnvio>");
        retornoWebservice.XmlEnvio = xmlLote.ToString();
    }

    protected override XElement WriteServicosValoresRps(NotaServico nota)
    {
        var servico = new XElement("Servico");
        var valores = new XElement("Valores");
        servico.AddChild(valores);

        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));

        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));

        valores.AddChild(AdicionarTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));

        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIssRetido", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIssRetido));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "BaseCalculo", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.BaseCalculo));

        // Valor Percentual - Exemplos: 1% => 0.01   /   25,5% => 0.255   /   100% => 1
        valores.AddChild(AdicionarTag(TipoCampo.De4, "", "Aliquota", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota / 100));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorLiquidoNfse", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorLiquidoNfse));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));

        servico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico.OnlyNumbers()));

        servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoCnae", 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));

        servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
        servico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
        servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoMunicipio", 1, 7, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));

        System.IO.File.WriteAllText("C:\\tmp\\teste.txt", "bettha1");
        return servico;
    }

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        if (retornoWebservice.NumeroNFSe.IsEmpty() || retornoWebservice.CodigoCancelamento.IsEmpty())
        {
            retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe/Codigo de cancelamento não informado para cancelamento." });
            return;
        }

        var loteBuilder = new StringBuilder();
        loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        loteBuilder.Append($"<CancelarNfseEnvio {GetNamespace()}>");
        loteBuilder.Append("<Pedido xmlns=\"\">");
        loteBuilder.Append($"<InfPedidoCancelamento Id=\"N{retornoWebservice.NumeroNFSe}\">");
        loteBuilder.Append("<IdentificacaoNfse>");
        loteBuilder.Append($"<Numero>{retornoWebservice.NumeroNFSe}</Numero>");
        loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
        loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        loteBuilder.Append($"<CodigoMunicipio>{Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio}</CodigoMunicipio>");
        loteBuilder.Append("</IdentificacaoNfse>");
        loteBuilder.Append($"<CodigoCancelamento>{retornoWebservice.CodigoCancelamento}</CodigoCancelamento>");
        loteBuilder.Append("</InfPedidoCancelamento>");
        loteBuilder.Append("</Pedido>");
        loteBuilder.Append("</CancelarNfseEnvio>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        var loteBuilder = new StringBuilder();
        loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        loteBuilder.Append($"<ConsultarSituacaoLoteRpsEnvio {GetNamespace()}>");
        loteBuilder.Append("<Prestador xmlns=\"\">");
        loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
        loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        loteBuilder.Append("</Prestador>");
        loteBuilder.Append($"<Protocolo xmlns=\"\">{retornoWebservice.Protocolo}</Protocolo>");
        loteBuilder.Append("</ConsultarSituacaoLoteRpsEnvio>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        var loteBuilder = new StringBuilder();
        loteBuilder.Append($"<ConsultarLoteRpsEnvio {GetNamespace()}>");
        loteBuilder.Append("<Prestador xmlns=\"\">");
        loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
        loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        loteBuilder.Append("</Prestador>");
        loteBuilder.Append($"<Protocolo xmlns=\"\">{retornoWebservice.Protocolo}</Protocolo>");
        loteBuilder.Append("</ConsultarLoteRpsEnvio>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.NumeroRps < 1)
        {
            retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe não informado para a consulta." });
            return;
        }

        var loteBuilder = new StringBuilder();
        loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        loteBuilder.Append($"<ConsultarNfsePorRpsEnvio {GetNamespace()}>");
        loteBuilder.Append("<IdentificacaoRps xmlns=\"\">");
        loteBuilder.Append($"<Numero>{retornoWebservice.NumeroRps}</Numero>");
        loteBuilder.Append($"<Serie>{retornoWebservice.Serie}</Serie>");
        loteBuilder.Append($"<Tipo>{(int)retornoWebservice.Tipo + 1}</Tipo>");
        loteBuilder.Append("</IdentificacaoRps>");
        loteBuilder.Append("<Prestador xmlns=\"\">");
        loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
        loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        loteBuilder.Append("</Prestador>");
        loteBuilder.Append("</ConsultarNfsePorRpsEnvio>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        var loteBuilder = new StringBuilder();
        loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        loteBuilder.Append($"<ConsultarNfseEnvio {GetNamespace()}>");
        loteBuilder.Append("<Prestador xmlns=\"\">");
        loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
        loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        loteBuilder.Append("</Prestador>");

        if (retornoWebservice.NumeroNFse > 0)
            loteBuilder.Append($"<NumeroNfse xmlns=\"\">{retornoWebservice}</NumeroNfse>");

        if (retornoWebservice.Inicio.HasValue && retornoWebservice.Fim.HasValue)
        {
            loteBuilder.Append("<PeriodoEmissao xmlns=\"\">");
            loteBuilder.Append($"<DataInicial>{retornoWebservice.Inicio:yyyy-MM-dd}</DataInicial>");
            loteBuilder.Append($"<DataFinal>{retornoWebservice.Fim:yyyy-MM-dd}</DataFinal>");
            loteBuilder.Append("</PeriodoEmissao>");
        }

        if (!retornoWebservice.CPFCNPJTomador.IsEmpty())
        {
            loteBuilder.Append("<Tomador xmlns=\"\">");
            loteBuilder.Append("<CpfCnpj>");
            loteBuilder.Append(retornoWebservice.CPFCNPJTomador.IsCNPJ()
                ? $"<Cnpj>{retornoWebservice.CPFCNPJTomador.ZeroFill(14)}</Cnpj>"
                : $"<Cpf>{retornoWebservice.CPFCNPJTomador.ZeroFill(11)}</Cpf>");
            loteBuilder.Append("</CpfCnpj>");
            if (!retornoWebservice.IMTomador.IsEmpty()) loteBuilder.Append($"<InscricaoMunicipal>{retornoWebservice.IMTomador}</InscricaoMunicipal>");
            loteBuilder.Append("</Tomador>");
        }

        if (!retornoWebservice.NomeIntermediario.IsEmpty() && !retornoWebservice.CPFCNPJIntermediario.IsEmpty())
        {
            loteBuilder.Append("<IntermediarioServico xmlns=\"\">");
            loteBuilder.Append($"<RazaoSocial>{retornoWebservice.NomeIntermediario}</RazaoSocial>");
            loteBuilder.Append(retornoWebservice.CPFCNPJIntermediario.IsCNPJ()
                ? $"<Cnpj>{retornoWebservice.CPFCNPJIntermediario.ZeroFill(14)}</Cnpj>"
                : $"<Cpf>{retornoWebservice.CPFCNPJIntermediario.ZeroFill(11)}</Cpf>");
            loteBuilder.Append("</CpfCnpj>");
            if (!retornoWebservice.IMIntermediario.IsEmpty())
                loteBuilder.Append($"<InscricaoMunicipal>{retornoWebservice.IMIntermediario}</InscricaoMunicipal>");
            loteBuilder.Append("</IntermediarioServico>");
        }

        loteBuilder.Append("</ConsultarNfseEnvio>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    #endregion Services

    #region Protected Methods

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        switch (tipo)
        {
            case TipoUrl.CancelarNFSe: return new BethaServiceClient(this, tipo, null);
            case TipoUrl.ConsultarNFSeRps: return new BethaServiceClient(this, tipo, null);
            case TipoUrl.ConsultarNFSe: return new BethaServiceClient(this, tipo, null);
            default: return new BethaServiceClient(this, tipo);
        }
    }

    protected override string GetNamespace()
    {
        return "xmlns=\"http://www.betha.com.br/e-nota-contribuinte-ws\"";
    }

    protected override string GetSchema(TipoUrl tipo)
    {
        switch (tipo)
        {
            case TipoUrl.Enviar: return "servico_enviar_lote_rps_envio_v01.xsd";
            case TipoUrl.ConsultarSituacao: return "servico_consultar_situacao_lote_rps_envio_v01.xsd";
            case TipoUrl.ConsultarLoteRps: return "servico_consultar_lote_rps_envio_v01.xsd";
            case TipoUrl.ConsultarNFSeRps: return "servico_consultar_nfse_rps_envio_v01.xsd";
            case TipoUrl.ConsultarNFSe: return "servico_consultar_nfse_envio_v01.xsd";
            case TipoUrl.CancelarNFSe: return "servico_cancelar_nfse_envio_v01.xsd";
            default: throw new ArgumentOutOfRangeException(nameof(tipo), tipo, @"Valor incorreto ou serviço não suportado.");
        }
    }

    #endregion Protected Methods

    #endregion Methods
}