// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-22-2018
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="IGovDigitalService.cs" company="OpenAC .Net">
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

using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using System;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderNFeCidades : ProviderABRASF201
{
    #region Constructors

    public ProviderNFeCidades(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "NFeCidades";
    }

    #endregion Constructors

    #region Methods

    #region Protected Methods

    protected override XElement WriteRps(NotaServico nota)
    {
        var rootRps = new XElement("Rps");

        var infServico = new XElement("InfDeclaracaoPrestacaoServico", new XAttribute("Id", $"R{nota.IdentificacaoRps.Numero.OnlyNumbers()}"));
        rootRps.Add(infServico);

        infServico.Add(WriteRpsRps(nota));

        infServico.AddChild(AddTag(TipoCampo.Dat, "", "Competencia", 10, 10, Ocorrencia.Obrigatoria, nota.Competencia));
        infServico.AddChild(WriteServicosRps(nota));
        infServico.AddChild(WritePrestadorRps(nota));
        infServico.AddChild(WriteTomadorRps(nota));
        infServico.AddChild(WriteIntermediarioRps(nota));
        infServico.AddChild(WriteConstrucaoCivilRps(nota));

        string regimeEspecialTributacao;
        string optanteSimplesNacional;
        if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
        {
            regimeEspecialTributacao = "6";
            optanteSimplesNacional = "1";
        }
        else
        {
            regimeEspecialTributacao = ((int)nota.RegimeEspecialTributacao).ToString();
            optanteSimplesNacional = "2";
        }

        if (nota.RegimeEspecialTributacao != RegimeEspecialTributacao.Nenhum)
            infServico.AddChild(AddTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));

        infServico.AddChild(AddTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
        infServico.AddChild(AddTag(TipoCampo.Int, "", "IncentivoFiscal", 1, 1, Ocorrencia.Obrigatoria, nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2));
        infServico.AddChild(WriteComExteriorRps(nota));



        return rootRps;
    }

    protected override XElement WriteRpsRps(NotaServico nota)
    {
        var rps = new XElement("Rps");

        rps.Add(WriteIdentificacaoRps(nota));

        rps.AddChild(AddTag(TipoCampo.Dat, "", "DataEmissao", 10, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
        rps.AddChild(AddTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Situacao + 1));

        rps.AddChild(WriteSubstituidoRps(nota));

        return rps;
    }

    protected override XElement WriteServicosRps(NotaServico nota)
    {
        var servico = new XElement("Servico");

        servico.Add(WriteValoresRps(nota));

        servico.AddChild(AddTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));

        if (nota.Servico.ResponsavelRetencao.HasValue)
            servico.AddChild(AddTag(TipoCampo.Int, "", "ResponsavelRetencao", 1, 1, Ocorrencia.NaoObrigatoria, (int)nota.Servico.ResponsavelRetencao + 1));

        servico.AddChild(AddTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoCnae", 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
        servico.AddChild(AddTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoMunicipio", 1, 20, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoPais", 1, 20, Ocorrencia.Obrigatoria, nota.Servico.CodigoPais));
        servico.AddChild(AddTag(TipoCampo.Int, "", "ExigibilidadeISS", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Servico.ExigibilidadeIss + 1));
        servico.AddChild(AddTag(TipoCampo.Int, "", "MunicipioIncidencia", 7, 7, Ocorrencia.MaiorQueZero, nota.Servico.MunicipioIncidencia));
        servico.AddChild(AddTag(TipoCampo.Str, "", "NumeroProcesso", 1, 30, Ocorrencia.NaoObrigatoria, nota.Servico.NumeroProcesso));
        servico.AddChild(AddTag(TipoCampo.Str, "", "MunicipioPrestacao", 1, 20, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));
        servico.AddChild(AddTag(TipoCampo.Int, "", "PaisPrestacao", 4, 4, Ocorrencia.MaiorQueZero, nota.Servico.PaisPrestacao > 0 ? nota.Servico.PaisPrestacao : nota.Servico.CodigoPais));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoNBS", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.CodigoNbs));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CIndOp", 6, 6, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoIndicadorOperacao));
        servico.AddChild(AddTag(TipoCampo.Str, "", "CClassTribReg", 6, 6, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoClassificacaoTributaria));

        return servico;
    }

    protected override XElement WriteValoresRps(NotaServico nota)
    {
        var valores = new XElement("Valores");

        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));
        valores.AddChild(AddTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
        if (Municipio.Provedor == NFSeProvider.Fiorilli)
            valores.AddChild(AddTag(TipoCampo.De4, "", "Aliquota", 1, 6, Ocorrencia.Obrigatoria, nota.Servico.Valores.Aliquota));
        else
            valores.AddChild(AddTag(TipoCampo.De4, "", "Aliquota", 1, 6, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota));
        valores.AddChild(AddTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
        valores.AddChild(AddTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));
        valores.AddChild(AddTag(TipoCampo.Str, "", "CST", 2, 2, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.CstPisCofins));
        valores.AddChild(AddTag(TipoCampo.Str, "", "TpRetPisCofins", 1, 1, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.TipoRetencaoPisCofins));

        return valores;
    }

    protected override XElement? WriteTomadorRps(NotaServico nota)
    {
        var tomador = new XElement("Tomador");

        if (!nota.Tomador.CpfCnpj.IsEmpty() ||
            !nota.Tomador.InscricaoMunicipal.IsEmpty() ||
            !nota.Tomador.DocEstrangeiro.IsEmpty() ||
            !nota.Tomador.CodNaoNif.IsEmpty())
        {
            var ideTomador = new XElement("IdentificacaoTomador");
            tomador.Add(ideTomador);

            if (!nota.Tomador.CpfCnpj.IsEmpty())
            {
                var cpfCnpjTomador = new XElement("CpfCnpj");
                ideTomador.Add(cpfCnpjTomador);

                cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));
            }

            ideTomador.AddChild(AddTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15,
                Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));
            ideTomador.AddChild(AddTag(TipoCampo.Str, "", "NIF", 1, 40,
                Ocorrencia.NaoObrigatoria, nota.Tomador.DocEstrangeiro));
            ideTomador.AddChild(AddTag(TipoCampo.Str, "", "CodNaoNIF", 1, 1,
                Ocorrencia.NaoObrigatoria, nota.Tomador.CodNaoNif));
        }

        tomador.AddChild(AddTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));

        if (!nota.Tomador.Endereco.Logradouro.IsEmpty() ||
            !nota.Tomador.Endereco.Numero.IsEmpty() ||
            !nota.Tomador.Endereco.Complemento.IsEmpty() ||
            !nota.Tomador.Endereco.Bairro.IsEmpty() ||
            nota.Tomador.Endereco.CodigoMunicipio > 0 ||
            !nota.Tomador.Endereco.Uf.IsEmpty() ||
            nota.Tomador.Endereco.CodigoPais > 0 ||
            !nota.Tomador.Endereco.Cep.IsEmpty() ||
            !nota.Tomador.Endereco.CodigoPostalExterior.IsEmpty() ||
            !nota.Tomador.Endereco.CidadeExterior.IsEmpty() ||
            !nota.Tomador.Endereco.EstadoProvinciaExterior.IsEmpty())
        {
            var endereco = new XElement("Endereco");
            tomador.Add(endereco);

            endereco.AddChild(AddTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
            endereco.AddChild(AddTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoMunicipio));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
            endereco.AddChild(AddTag(TipoCampo.Int, "", "CodigoPais", 4, 4, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoPais));
            endereco.AddChild(AddTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));

            if (!nota.Tomador.Endereco.CodigoPostalExterior.IsEmpty() ||
                !nota.Tomador.Endereco.CidadeExterior.IsEmpty() ||
                !nota.Tomador.Endereco.EstadoProvinciaExterior.IsEmpty())
            {
                var enderecoExterior = new XElement("EndExt");
                endereco.Add(enderecoExterior);

                enderecoExterior.AddChild(AddTag(TipoCampo.Str, "", "CodigoEndPost", 1, 15, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.CodigoPostalExterior));
                enderecoExterior.AddChild(AddTag(TipoCampo.Str, "", "CidadeExterior", 1, 60, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.CidadeExterior));
                enderecoExterior.AddChild(AddTag(TipoCampo.Str, "", "EstProvRegExterior", 1, 60, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.EstadoProvinciaExterior));
            }
        }

        if (!nota.Tomador.DadosContato.Telefone.IsEmpty() ||
            !nota.Tomador.DadosContato.Email.IsEmpty())
        {
            var contato = new XElement("Contato");
            tomador.Add(contato);

            contato.AddChild(AddTag(TipoCampo.StrNumber, "", "Telefone", 1, 11, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));
            contato.AddChild(AddTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
        }

        return tomador;
    }

    private XElement? WriteComExteriorRps(NotaServico nota)
    {
        if (nota.ComExterior == null) return null;
        if (nota.ComExterior.ModoPrestacao.IsEmpty() &&
            nota.ComExterior.VinculoPrestacao.IsEmpty() &&
            nota.ComExterior.TipoMoeda.IsEmpty() &&
            nota.ComExterior.ValorServicoMoeda <= 0 &&
            nota.ComExterior.MecanismoApoioPrestador.IsEmpty() &&
            nota.ComExterior.MecanismoApoioTomador.IsEmpty() &&
            nota.ComExterior.MovimentoTemporarioBens.IsEmpty() &&
            nota.ComExterior.EnviarMdic.IsEmpty()) return null;

        var comExterior = new XElement("ComExt");
        comExterior.AddChild(AddTag(TipoCampo.Str, "", "MdPrestacao", 1, 1, Ocorrencia.Obrigatoria, nota.ComExterior.ModoPrestacao));
        comExterior.AddChild(AddTag(TipoCampo.Str, "", "VincPrest", 1, 1, Ocorrencia.Obrigatoria, nota.ComExterior.VinculoPrestacao));
        comExterior.AddChild(AddTag(TipoCampo.Str, "", "TpMoeda", 1, 3, Ocorrencia.Obrigatoria, nota.ComExterior.TipoMoeda));
        comExterior.AddChild(AddTag(TipoCampo.De2, "", "VServMoeda", 1, 15, Ocorrencia.Obrigatoria, nota.ComExterior.ValorServicoMoeda));
        comExterior.AddChild(AddTag(TipoCampo.Str, "", "MecAFComexP", 1, 2, Ocorrencia.Obrigatoria, nota.ComExterior.MecanismoApoioPrestador));
        comExterior.AddChild(AddTag(TipoCampo.Str, "", "MecAFComexT", 1, 2, Ocorrencia.Obrigatoria, nota.ComExterior.MecanismoApoioTomador));
        comExterior.AddChild(AddTag(TipoCampo.Str, "", "MovTempBens", 1, 1, Ocorrencia.Obrigatoria, nota.ComExterior.MovimentoTemporarioBens));
        comExterior.AddChild(AddTag(TipoCampo.Str, "", "Mdic", 1, 1, Ocorrencia.Obrigatoria, nota.ComExterior.EnviarMdic));

        return comExterior;
    }

    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException($"O provedor [{Name}] não implementa o método [{nameof(Enviar)}], utilize o método [{nameof(EnviarSincrono)}]");
    }

    protected override IServiceClient GetClient(TipoUrl tipo) => new NFeCidadesServiceClient(this, tipo);

    #endregion Protected Methods

    #endregion Methods
}
