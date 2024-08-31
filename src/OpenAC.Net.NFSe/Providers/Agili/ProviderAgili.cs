// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Flávio Vodzinski
// Created          : 04-24-2024
//
// Last Modified By : Rafael Dias
// Last Modified On : 08-15-2024
// ***********************************************************************
// <copyright file="ProviderAgili.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2024 Projeto OpenAC .Net
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
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal class ProviderAgili : ProviderABRASF
{
    #region Constructors

    public ProviderAgili(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "Agili";
        if(!Municipio.Parametros.TryGetValue(nameof(UnidadeGestora), out var value) || value.IsEmpty())
            throw new OpenDFeException($"Este provedor precisa que seja informado o parâmetro {nameof(UnidadeGestora)}.");

        UnidadeGestora = value ?? "";
    }

    #endregion Constructors

    #region Properties

    public string UnidadeGestora { get; }

    #endregion Properties
    
    #region Methods

    protected virtual XElement WriteIdentificacaoPrestador(NotaServico nota)
    {
        var identificacaoPrestador = new XElement("IdentificacaoPrestador");
        identificacaoPrestador.AddChild(AdicionarTag(TipoCampo.Str, "", "ChaveDigital", 1, 32, Ocorrencia.Obrigatoria,
            Configuracoes.WebServices.ChavePrivada));

        var cpfCnpj = new XElement("CpfCnpj");

        cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj));

        identificacaoPrestador.Add(cpfCnpj);
        identificacaoPrestador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 32,
            Ocorrencia.Obrigatoria, nota.Prestador.InscricaoMunicipal));

        return identificacaoPrestador;
    }

    protected override XElement WriteIdentificacao(NotaServico nota)
    {
        string tipoRps;
        switch (nota.IdentificacaoRps.Tipo)
        {
            case TipoRps.RPS:
                tipoRps = "-2";
                break;

            case TipoRps.NFConjugada:
                tipoRps = "-4";
                break;

            case TipoRps.Cupom:
                tipoRps = "-5";
                break;

            default:
                tipoRps = "0";
                break;
        }

        var ideRps = new XElement("IdentificacaoRps");
        ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", 1, 15, Ocorrencia.Obrigatoria,
            nota.IdentificacaoRps.Numero));
        ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Serie", 1, 5, Ocorrencia.Obrigatoria,
            nota.IdentificacaoRps.Serie));
        ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", 1, 1, Ocorrencia.Obrigatoria, tipoRps));

        return ideRps;
    }

    protected override XElement WriteTomadorRps(NotaServico nota)
    {
        var tomador = new XElement("DadosTomador");

        var ideTomador = new XElement("IdentificacaoTomador");
        tomador.Add(ideTomador);

        var cpfCnpjTomador = new XElement("CpfCnpj");
        ideTomador.Add(cpfCnpjTomador);

        cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

        ideTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria,
            nota.Tomador.InscricaoMunicipal));

        tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria,
            nota.Tomador.RazaoSocial));
        tomador.AddChild(AdicionarTag(TipoCampo.Int, "", "LocalEndereco", 1, 1, Ocorrencia.Obrigatoria, 1));

        if (!nota.Tomador.Endereco.Logradouro.IsEmpty() || !nota.Tomador.Endereco.Numero.IsEmpty() ||
            !nota.Tomador.Endereco.Complemento.IsEmpty() || !nota.Tomador.Endereco.Bairro.IsEmpty() ||
            nota.Tomador.Endereco.CodigoMunicipio > 0 || !nota.Tomador.Endereco.Uf.IsEmpty() ||
            !nota.Tomador.Endereco.Cep.IsEmpty())
        {
            var endereco = new XElement("Endereco");
            tomador.Add(endereco);

            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoLogradouro", 1, 120, Ocorrencia.NaoObrigatoria,
                nota.Tomador.Endereco.TipoLogradouro));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Logradouro", 1, 125, Ocorrencia.NaoObrigatoria,
                nota.Tomador.Endereco.Logradouro));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria,
                nota.Tomador.Endereco.Numero));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria,
                nota.Tomador.Endereco.Complemento));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria,
                nota.Tomador.Endereco.Bairro));

            var municipio = new XElement("Municipio");
            municipio.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipioIBGE", 7, 7, Ocorrencia.MaiorQueZero,
                nota.Tomador.Endereco.CodigoMunicipio));
            municipio.AddChild(AdicionarTag(TipoCampo.Str, "", "Descricao", 1, 300, Ocorrencia.Obrigatoria,
                nota.Tomador.Endereco.Municipio));
            municipio.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria,
                nota.Tomador.Endereco.Uf));

            endereco.AddChild(municipio);

            var pais = new XElement("Pais");
            pais.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPaisBacen", 1, 4, Ocorrencia.MaiorQueZero,
                nota.Tomador.Endereco.CodigoPais));
            pais.AddChild(AdicionarTag(TipoCampo.Str, "", "Descricao", 1, 4, Ocorrencia.MaiorQueZero,
                nota.Tomador.Endereco.Pais));

            endereco.AddChild(pais);

            endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria,
                nota.Tomador.Endereco.Cep));
        }

        if (!nota.Tomador.DadosContato.DDD.IsEmpty() || !nota.Tomador.DadosContato.Telefone.IsEmpty() ||
            !nota.Tomador.DadosContato.Email.IsEmpty())
        {
            var contato = new XElement("Contato");
            tomador.Add(contato);

            contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Telefone", 1, 11, Ocorrencia.NaoObrigatoria,
                nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));
            contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria,
                nota.Tomador.DadosContato.Email));
        }

        return tomador;
    }

    protected override XElement WriteIntermediarioRps(NotaServico nota)
    {
        if (nota.Intermediario.RazaoSocial.IsEmpty()) return null;

        var intermediario = new XElement("DadosIntermediario");

        var ideIntermediario = new XElement("IdentificacaoIntermediario");
        intermediario.Add(ideIntermediario);

        var cpfCnpj = new XElement("CpfCnpj");
        ideIntermediario.Add(cpfCnpj);

        cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Intermediario.CpfCnpj));

        ideIntermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15,
            Ocorrencia.NaoObrigatoria,
            nota.Intermediario.InscricaoMunicipal));

        intermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria,
            nota.Intermediario.RazaoSocial));

        return intermediario;
    }

    protected XElement WriteListaServico(NotaServico nota)
    {
        var listaServico = new XElement("ListaServico");
        var dadosServico = new XElement("DadosServico");

        dadosServico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria,
            nota.Servico.Discriminacao));
        dadosServico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoCnae", 1, 140, Ocorrencia.NaoObrigatoria,
            nota.Servico.CodigoCnae));
        dadosServico.AddChild(AdicionarTag(TipoCampo.Int, "", "Quantidade", 1, 13, Ocorrencia.NaoObrigatoria, 1));
        dadosServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServico", 1, 15, Ocorrencia.Obrigatoria,
            nota.Servico.Valores.ValorServicos));

        listaServico.AddChild(dadosServico);

        return listaServico;
    }

    protected override XElement WriteRps(NotaServico nota)
    {
        var declaracaoPrestacaoServico = new XElement("DeclaracaoPrestacaoServico");
        declaracaoPrestacaoServico.Add(WriteIdentificacaoPrestador(nota));

        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.Int, "", "NfseSubstituida", 1, 1,
            Ocorrencia.MaiorQueZero, 0));

        var rps = new XElement("Rps");
        rps.AddChild(WriteIdentificacao(nota));
        rps.AddChild(AdicionarTag(TipoCampo.Dat, "", "DataEmissao", 1, 1, Ocorrencia.Obrigatoria,
            nota.IdentificacaoRps.DataEmissao));
        declaracaoPrestacaoServico.AddChild(rps);

        declaracaoPrestacaoServico.AddChild(WriteTomadorRps(nota));
        declaracaoPrestacaoServico.AddChild(WriteIntermediarioRps(nota));

        if (nota.RegimeEspecialTributacao.IsIn(
                RegimeEspecialTributacao.Estimativa,
                RegimeEspecialTributacao.SociedadeProfissionais,
                RegimeEspecialTributacao.Cooperativa,
                RegimeEspecialTributacao.MicroEmpresarioIndividual,
                RegimeEspecialTributacao.MicroEmpresarioEmpresaPP))
        {
            var regimeEspecialTributacao = new XElement("RegimeEspecialTributacao");

            switch (nota.RegimeEspecialTributacao)
            {
                case RegimeEspecialTributacao.Estimativa:
                {
                    regimeEspecialTributacao.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1,
                        Ocorrencia.Obrigatoria, "-2"));
                    break;
                }
                case RegimeEspecialTributacao.SociedadeProfissionais:
                {
                    regimeEspecialTributacao.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1,
                        Ocorrencia.Obrigatoria, "-3"));
                    break;
                }
                case RegimeEspecialTributacao.Cooperativa:
                {
                    regimeEspecialTributacao.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1,
                        Ocorrencia.Obrigatoria, "-4"));
                    break;
                }
                case RegimeEspecialTributacao.MicroEmpresarioIndividual:
                {
                    regimeEspecialTributacao.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1,
                        Ocorrencia.Obrigatoria, "-5"));
                    break;
                }
                case RegimeEspecialTributacao.MicroEmpresarioEmpresaPP:
                {
                    regimeEspecialTributacao.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1,
                        Ocorrencia.Obrigatoria, "-6"));
                    break;
                }
            }

            declaracaoPrestacaoServico.AddChild(regimeEspecialTributacao);
        }

        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1,
            Ocorrencia.Obrigatoria, nota.OptanteSimplesNacional == NFSeSimNao.Sim ? 1 : 0));
        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteMEISimei", 1, 1,
            Ocorrencia.Obrigatoria, nota.OptanteMEISimei == NFSeSimNao.Sim ? 1 : 0));
        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.Int, "", "ISSQNRetido", 1, 1, Ocorrencia.Obrigatoria,
            nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 0));
        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemLei116AtividadeEconomica", 1, 140,
            Ocorrencia.NaoObrigatoria, nota.Servico.ItemListaServico));

        var exigibilidadeIss = new XElement("ExigibilidadeISSQN");

        switch (nota.Servico.ExigibilidadeIss)
        {
            case ExigibilidadeIss.Exigivel:
            {
                exigibilidadeIss.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria,
                    "-1"));
                break;
            }
            case ExigibilidadeIss.NaoIncidencia:
            {
                exigibilidadeIss.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria,
                    "-2"));
                break;
            }
            case ExigibilidadeIss.Isencao:
            {
                exigibilidadeIss.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria,
                    "-3"));
                break;
            }
            case ExigibilidadeIss.Exportacao:
            {
                exigibilidadeIss.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria,
                    "-4"));
                break;
            }
            case ExigibilidadeIss.Imunidade:
            {
                exigibilidadeIss.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria,
                    "-5"));
                break;
            }
            case ExigibilidadeIss.SuspensaDecisaoJudicial:
            {
                exigibilidadeIss.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria,
                    "-6"));
                break;
            }
            case ExigibilidadeIss.SuspensaProcessoAdministrativo:
            {
                exigibilidadeIss.AddChild(AdicionarTag(TipoCampo.Int, "", "Codigo", 1, 1, Ocorrencia.Obrigatoria,
                    "-7"));
                break;
            }
        }

        declaracaoPrestacaoServico.AddChild(exigibilidadeIss);

        var municipioIncidencia = new XElement("MunicipioIncidencia");

        municipioIncidencia.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipioIBGE", 1, 7,
            Ocorrencia.Obrigatoria, nota.Servico.MunicipioIncidencia));
        declaracaoPrestacaoServico.AddChild(municipioIncidencia);

        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15,
            Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero,
            nota.Servico.Valores.ValorPis));
        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", 1, 15,
            Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero,
            nota.Servico.Valores.ValorInss));
        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIrrf", 1, 15, Ocorrencia.MaiorQueZero,
            nota.Servico.Valores.ValorIr));
        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero,
            nota.Servico.Valores.ValorCsll));
        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorOutrasRetencoes", 1, 15,
            Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorOutrasRetencoes));
        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorBaseCalculoISSQN", 1, 15,
            Ocorrencia.MaiorQueZero, nota.Servico.Valores.BaseCalculo));
        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "AliquotaISSQN", 1, 15,
            Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota));
        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorISSQNCalculado", 1, 15,
            Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorLiquido", 1, 15,
            Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorLiquidoNfse));

        declaracaoPrestacaoServico.AddChild(WriteListaServico(nota));

        declaracaoPrestacaoServico.AddChild(AdicionarTag(TipoCampo.Str, "", "Versao", 1, 1, Ocorrencia.Obrigatoria,
            "1.00"));

        return declaracaoPrestacaoServico;
    }

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        switch (notas.Count)
        {
            case 0:
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
                break;
            case > 3:
                retornoWebservice.Erros.Add(new Evento
                    { Codigo = "0", Descricao = "Apenas 3 RPS podem ser enviados em modo Sincrono." });
                break;
        }

        if (retornoWebservice.Erros.Count > 0) return;

        var xmlLoteRps = new StringBuilder();

        foreach (var nota in notas)
        {
            var xmlRps = WriteXmlRps(nota, false, false);
            xmlLoteRps.Append(xmlRps);
            GravarRpsEmDisco(xmlRps,
                $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml",
                nota.IdentificacaoRps.DataEmissao);
        }

        var xmlLote = new StringBuilder();
        xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        xmlLote.Append($"<GerarNfseEnvio {GetNamespace()}>");
        xmlLote.Append($"<UnidadeGestora>{UnidadeGestora}</UnidadeGestora>");
        xmlLote.Append(xmlLoteRps);

        xmlLote.Append("</GerarNfseEnvio>");
        retornoWebservice.XmlEnvio = xmlLote.ToString();
    }

    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.Lote == 0)
            retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
        if (notas.Count == 0)
            retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
        if (retornoWebservice.Erros.Count > 0) return;

        var xmlLoteRps = new StringBuilder();

        foreach (var nota in notas)
        {
            nota.Servico.ItemListaServico = nota.Servico.ItemListaServico.OnlyNumbers();

            var xmlRps = WriteXmlRps(nota, false, false);
            xmlLoteRps.Append(xmlRps);
            GravarRpsEmDisco(xmlRps,
                $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml",
                nota.IdentificacaoRps.DataEmissao);
        }

        var xmlLote = new StringBuilder();
        xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        xmlLote.Append($"<EnviarLoteRpsEnvio {GetNamespace()}>");
        xmlLote.Append($"<UnidadeGestora>{UnidadeGestora}</UnidadeGestora>");
        xmlLote.Append("<LoteRps>");
        xmlLote.Append($"<NumeroLote>{retornoWebservice.Lote}</NumeroLote>");
        xmlLote.Append("<IdentificacaoPrestador>");
        xmlLote.Append($"<ChaveDigital>{Configuracoes.WebServices.ChavePrivada}</ChaveDigital>");
        xmlLote.Append("<CpfCnpj>");

        switch (Configuracoes.PrestadorPadrao.CpfCnpj.Length)
        {
            case 11:
            {
                xmlLote.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cnpj>");
                break;
            }
            case 14:
            {
                xmlLote.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
                break;
            }
        }

        xmlLote.Append("</CpfCnpj>");
        xmlLote.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        xmlLote.Append("</IdentificacaoPrestador>");

        xmlLote.Append($"<QuantidadeRps>{notas.Count}</QuantidadeRps>");
        xmlLote.Append("<ListaRps>");

        xmlLote.Append(xmlLoteRps);

        xmlLote.Append("</ListaRps>");
        xmlLote.Append("</LoteRps>");
        xmlLote.Append("<Versao>1.00</Versao>");
        xmlLote.Append("</EnviarLoteRpsEnvio>");
        retornoWebservice.XmlEnvio = xmlLote.ToString();
    }

    protected override bool PrecisaValidarSchema(TipoUrl tipo) => false;

    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
    }

    protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
    {
    }

    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        if (retornoWebservice.NumeroNFSe.IsEmpty() || retornoWebservice.CodigoCancelamento.IsEmpty())
        {
            retornoWebservice.Erros.Add(new Evento
                { Codigo = "0", Descricao = "Número da NFSe/Codigo de cancelamento não informado para cancelamento." });
            return;
        }

        var loteBuilder = new StringBuilder();
        loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        loteBuilder.Append($"<CancelarNfseEnvio {GetNamespace()}>");
        loteBuilder.Append($"<UnidadeGestora>{UnidadeGestora}</UnidadeGestora>");
        loteBuilder.Append("<PedidoCancelamento>");
        loteBuilder.Append("<IdentificacaoNfse>");
        loteBuilder.Append($"<Numero>{retornoWebservice.NumeroNFSe}</Numero>");
        loteBuilder.Append("<IdentificacaoPrestador>");
        loteBuilder.Append($"<ChaveDigital>{Configuracoes.WebServices.ChavePrivada}</ChaveDigital>");
        loteBuilder.Append("<CpfCnpj>");

        switch (Configuracoes.PrestadorPadrao.CpfCnpj.Length)
        {
            case 11:
            {
                loteBuilder.Append($"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj}</Cpf>");
                break;
            }
            case 14:
            {
                loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj}</Cnpj>");
                break;
            }
        }

        loteBuilder.Append("</CpfCnpj>");
        loteBuilder.Append(
            $"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        loteBuilder.Append("</IdentificacaoPrestador>");
        loteBuilder.Append("</IdentificacaoNfse>");
        loteBuilder.Append($"<CodigoCancelamento>{retornoWebservice.CodigoCancelamento}</CodigoCancelamento>");
        loteBuilder.Append($"<JustificativaCancelamento>{retornoWebservice.Motivo}</JustificativaCancelamento>");
        loteBuilder.Append("<Versao>1.00</Versao>");
        loteBuilder.Append("</PedidoCancelamento>");
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
        loteBuilder.Append(
            $"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
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
        loteBuilder.Append(
            $"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        loteBuilder.Append("</Prestador>");
        loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");
        loteBuilder.Append("</ConsultarLoteRpsEnvio>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice,
        NotaServicoCollection notas)
    {
        if (retornoWebservice.NumeroRps < 1)
        {
            retornoWebservice.Erros.Add(new Evento
                { Codigo = "0", Descricao = "Número da NFSe não informado para a consulta." });
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
        loteBuilder.Append(
            $"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
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
        loteBuilder.Append(
            $"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
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
            if (!retornoWebservice.IMTomador.IsEmpty())
                loteBuilder.Append($"<InscricaoMunicipal>{retornoWebservice.IMTomador}</InscricaoMunicipal>");
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

    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet.Root);
        if (retornoWebservice.Erros.Count > 0) return;

        var protocoloCancelamento = xmlRet.Root.ElementAnyNs("ProtocoloRequerimentoCancelamento");
        if (protocoloCancelamento == null)
        {
            retornoWebservice.Erros.Add(new Evento
                { Codigo = "0", Descricao = "Confirmação do cancelamento não encontrada!" });
            return;
        }

        retornoWebservice.Data = xmlRet.Root.ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;
        retornoWebservice.Sucesso = true;
        retornoWebservice.CodigoCancelamento = protocoloCancelamento.GetValue<string>();

        var numeroNfSe =
            xmlRet.Root.ElementAnyNs("PedidoCancelamento").ElementAnyNs("IdentificacaoNfse")?.ElementAnyNs("Numero")
                .GetValue<string>() ?? string.Empty;

        // Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
        var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == numeroNfSe);
        if (nota == null) return;

        nota.Situacao = SituacaoNFSeRps.Cancelado;
        nota.Cancelamento.Pedido.CodigoCancelamento = retornoWebservice.CodigoCancelamento;
        nota.Cancelamento.DataHora = retornoWebservice.Data;
    }

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet.Root);
        if (retornoWebservice.Erros.Count > 0) return;

        var nfse = xmlRet.ElementAnyNs("GerarNfseResposta")?.ElementAnyNs("Nfse");
        var numeroNfSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
        var chaveNfSe = nfse.ElementAnyNs("CodigoAutenticidade")?.GetValue<string>() ?? string.Empty;
        var dataNfSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
        var numeroRps =
            nfse?.ElementAnyNs("DeclaracaoPrestacaoServico")?.ElementAnyNs("Rps")?.ElementAnyNs("IdentificacaoRps")
                ?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;

        GravarNFSeEmDisco(nfse.AsString(true), $"NFSe-{numeroNfSe}-{chaveNfSe}-.xml", dataNfSe);

        var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
        if (nota != null)
        {
            nota.IdentificacaoNFSe.Numero = numeroNfSe;
            nota.IdentificacaoNFSe.Chave = chaveNfSe;
            nota.IdentificacaoNFSe.DataEmissao = dataNfSe;
        }

        retornoWebservice.Sucesso = true;
    }

    protected override void LoadServicosValoresRps(NotaServico nota, XElement rootNfSe)
    {
        var rootServico = rootNfSe.ElementAnyNs("DeclaracaoPrestacaoServico");
        if (rootServico == null) return;

        nota.Servico.Valores.ValorServicos = rootServico.ElementAnyNs("ValorServicos")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorDeducoes =
            rootServico.ElementAnyNs("ValorDeducaoConstCivil")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorPis = rootServico.ElementAnyNs("ValorPis")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorCofins = rootServico.ElementAnyNs("ValorCofins")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorInss = rootServico.ElementAnyNs("ValorInss")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorIr = rootServico.ElementAnyNs("ValorIr")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorCsll = rootServico.ElementAnyNs("ValorCsll")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.IssRetido = (rootServico.ElementAnyNs("ISSQNRetido")?.GetValue<int>() ?? 0) == 1
            ? SituacaoTributaria.Retencao
            : SituacaoTributaria.Normal;
        nota.Servico.Valores.ValorIss = rootServico.ElementAnyNs("ValorISSQNCalculado")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorOutrasRetencoes =
            rootServico.ElementAnyNs("ValorOutrasRetencoes")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.BaseCalculo = rootServico.ElementAnyNs("ValorBaseCalculoISSQN")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.Aliquota = rootServico.ElementAnyNs("AliquotaISSQN")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorLiquidoNfse = rootServico.ElementAnyNs("ValorLiquido")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorIssRetido = rootServico.ElementAnyNs("ValorISSQNRecolher")?.GetValue<decimal>() ?? 0;

        nota.Servico.ItemListaServico = rootServico.ElementAnyNs("ItemLei116AtividadeEconomica")?.GetValue<string>() ??
                                        string.Empty;
        nota.Servico.CodigoCnae =
            rootServico.ElementAnyNs("CodigoCnaeAtividadeEconomica")?.GetValue<string>() ?? string.Empty;

        nota.Servico.Discriminacao =
            rootServico.ElementAnyNs("ListaServico").ElementAnyNs("DadosServico").ElementAnyNs("Discriminacao")
                ?.GetValue<string>() ?? string.Empty;
    }

    protected override string GetNamespace() => "xmlns=\"http://www.agili.com.br/nfse_v_1.00.xsd\"";

    protected override string GetSchema(TipoUrl tipo) => "nfse-v-100.xsd";

    protected override IServiceClient GetClient(TipoUrl tipo) => new AgiliServiceClient(this, tipo);

    #endregion Methods
}