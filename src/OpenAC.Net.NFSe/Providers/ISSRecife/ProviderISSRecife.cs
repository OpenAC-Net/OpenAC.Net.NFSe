// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-22-2018
//
// Last Modified By : Leandro Rossi (rossism.com.br)
// Last Modified On : 14-04-2023
// ***********************************************************************
// <copyright file="ISSNetServiceClient.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		Copyright (c) 2014 - 2024 Projeto OpenAC .Net
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

using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal class ProviderISSRecife : ProviderABRASF
{
    #region Constructors

    public ProviderISSRecife(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "ABRASF";
    }

    #endregion Constructors

    protected override XElement WriteInfoRPS(NotaServico nota)
    {
        var incentivadorCultural = nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2;            
        var optanteSimplesNacional  = nota.OptanteSimplesNacional == NFSeSimNao.Sim ? 1 : 2;
        var regimeEspecialTributacao = nota.RegimeEspecialTributacao == 0 ? string.Empty : nota.RegimeEspecialTributacao.ToString();
        var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "1" : "2";

        var infoRps = new XElement("InfRps", new XAttribute("Id", $"R{nota.IdentificacaoRps.Numero}"));

        infoRps.Add(WriteIdentificacao(nota));
        infoRps.AddChild(AddTag(TipoCampo.DatHor, "", "DataEmissao", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "NaturezaOperacao", 1, 1, Ocorrencia.Obrigatoria, nota.NaturezaOperacao));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "IncentivadorCultural", 1, 1, Ocorrencia.Obrigatoria, incentivadorCultural));
        infoRps.AddChild(AddTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, situacao));

        return infoRps;
    }

    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.Lote == 0) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Lote não informado." });
        if (notas.Count == 0) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "RPS não informado." });
        if (retornoWebservice.Erros.Any()) return;

        var xmlLoteRps = new StringBuilder();

        foreach (var nota in notas)
        {
            nota.Servico.ItemListaServico = nota.Servico.ItemListaServico.OnlyNumbers();

            var xmlRps = WriteXmlRps(nota, false, false);
            xmlLoteRps.Append(xmlRps);
            GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
        }

        var xmlLote = new StringBuilder();
        xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        xmlLote.Append($"<EnviarLoteRpsEnvio {GetNamespace()}>");
        xmlLote.Append($"<LoteRps Id=\"L{retornoWebservice.Lote}\">");
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

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        if (retornoWebservice.NumeroNFSe.IsEmpty() || retornoWebservice.CodigoCancelamento.IsEmpty())
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Número da NFSe/Codigo de cancelamento não informado para cancelamento." });
            return;
        }

        var loteBuilder = new StringBuilder();
        loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        loteBuilder.Append($"<CancelarNfseEnvio {GetNamespace()}>");
        loteBuilder.Append("<Pedido xmlns=\"http://www.abrasf.org.br/ABRASF/arquivos/nfse.xsd\">");
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
        loteBuilder.Append("<Prestador>");
        loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
        loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        loteBuilder.Append("</Prestador>");
        loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");
        loteBuilder.Append("</ConsultarLoteRpsEnvio>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.NumeroRps < 1)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Número da NFSe não informado para a consulta." });
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



    #region Protected Methods

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new ISSRecifeServiceClient(this, tipo);
    }

    protected override string GetNamespace()
    {
        return "xmlns=\"http://www.abrasf.org.br/ABRASF/arquivos/nfse.xsd\"";
    }

    protected override string GetSchema(TipoUrl tipo)
    {
        return "nfse.xsd";
    }

    protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno.HtmlDecode());

        var rootElement = xmlRet.ElementAnyNs("EnviarLoteRpsResposta");
        MensagemErro(retornoWebservice, rootElement);
        if (retornoWebservice.Erros.Count > 0) return;

        retornoWebservice.Lote = rootElement?.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;
        retornoWebservice.Data = rootElement?.ElementAnyNs("DataRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;
        retornoWebservice.Protocolo = rootElement?.ElementAnyNs("Protocolo")?.GetValue<string>() ?? string.Empty;
        retornoWebservice.Sucesso = retornoWebservice.Lote > 0;

        if (!retornoWebservice.Sucesso) return;

        foreach (var nota in notas)
        {
            nota.NumeroLote = retornoWebservice.Lote;
        }
    }

    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet.Root);
        if (retornoWebservice.Erros.Any()) return;

        var confirmacaoCancelamento = xmlRet.Root.ElementAnyNs("Cancelamento")?.ElementAnyNs("Confirmacao");

        if (confirmacaoCancelamento == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Confirmação do cancelamento não encontrada!" });
            return;
        }

        retornoWebservice.Data = confirmacaoCancelamento.ElementAnyNs("DataHoraCancelamento")?.GetValue<DateTime>() ?? DateTime.MinValue;
        retornoWebservice.Sucesso = retornoWebservice.Data != DateTime.MinValue;
        retornoWebservice.CodigoCancelamento = confirmacaoCancelamento.ElementAnyNs("Pedido").ElementAnyNs("InfPedidoCancelamento")
            .ElementAnyNs("CodigoCancelamento").GetValue<string>();

        var numeroNFSe = confirmacaoCancelamento.ElementAnyNs("Pedido").ElementAnyNs("InfPedidoCancelamento")?
            .ElementAnyNs("IdentificacaoNfse")?.ElementAnyNs("Numero").GetValue<string>() ?? string.Empty;

        // Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
        var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == numeroNFSe);
        if (nota == null) return;

        nota.Situacao = SituacaoNFSeRps.Cancelado;
        nota.Cancelamento.Pedido.CodigoCancelamento = retornoWebservice.CodigoCancelamento;
        nota.Cancelamento.DataHora = retornoWebservice.Data;
    }

    #endregion Protected Methods
}