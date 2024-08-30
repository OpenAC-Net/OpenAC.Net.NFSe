// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Adriano Trentim
// Created          : 22-01-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-02-2020
// ***********************************************************************
// <copyright file="ProviderABRASF203.cs" company="OpenAC .Net">
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

using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

// ReSharper disable once InconsistentNaming
/// <summary>
/// Classe base para trabalhar com provedores que usam o padrão ABRASF 2.04
/// </summary>
/// <seealso cref="ProviderBase" />
public abstract class ProviderABRASF203 : ProviderABRASF202
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderABRASF203"/> class.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="municipio">The municipio.</param>
    protected ProviderABRASF203(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "ABRASF";
        Versao = VersaoNFSe.ve203;
    }

    #endregion Constructors

    #region Methods

    #region LoadXml

    /// <inheritdoc />
    protected override void LoadTomador(NotaServico nota, XElement rpsRoot)
    {
        // Tomador
        var rootTomador = rpsRoot.ElementAnyNs("Tomador");
        if (rootTomador == null) return;

        var tomadorIdentificacao = rootTomador.ElementAnyNs("IdentificacaoTomador");
        if (tomadorIdentificacao != null)
        {
            nota.Tomador.CpfCnpj = tomadorIdentificacao.ElementAnyNs("CpfCnpj")?.GetCPF_CNPJ();
            nota.Tomador.InscricaoMunicipal = tomadorIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
        }

        nota.Tomador.DocEstrangeiro = rootTomador.ElementAnyNs("NifTomador")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.RazaoSocial = rootTomador.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;

        var endereco = rootTomador.ElementAnyNs("Endereco");
        if (endereco != null)
        {
            nota.Tomador.Endereco.Logradouro = endereco.ElementAnyNs("Endereco")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.Numero = endereco.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.Complemento = endereco.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.Bairro = endereco.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.CodigoMunicipio = endereco.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
            nota.Tomador.Endereco.Uf = endereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.Endereco.CodigoPais = endereco.ElementAnyNs("CodigoPais")?.GetValue<int>() ?? 0;
            nota.Tomador.Endereco.Cep = endereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
        }

        var enderecoExterior = rootTomador.ElementAnyNs("EnderecoExterior");
        if (enderecoExterior != null)
        {
            nota.Tomador.EnderecoExterior.CodigoPais = enderecoExterior.ElementAnyNs("CodigoPais")?.GetValue<int>() ?? 0;
            nota.Tomador.EnderecoExterior.EnderecoCompleto = enderecoExterior.ElementAnyNs("EnderecoCompletoExterior")?.GetValue<string>() ?? string.Empty;
        }

        var rootTomadorContato = rootTomador.ElementAnyNs("Contato");
        if (rootTomadorContato == null) return;

        nota.Tomador.DadosContato.DDD = "";
        nota.Tomador.DadosContato.Telefone = rootTomadorContato.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.DadosContato.Email = rootTomadorContato.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
    }

    /// <inheritdoc />
    protected override void LoadPrestador(NotaServico nota, XElement rootNFSe)
    {
        // Prestador
        var rootPrestador = rootNFSe.ElementAnyNs("PrestadorServico");
        if (rootPrestador == null) return;

        var prestadorIdentificacao = rootPrestador.ElementAnyNs("IdentificacaoPrestador");
        if (prestadorIdentificacao != null)
        {
            nota.Prestador.CpfCnpj = prestadorIdentificacao.ElementAnyNs("CpfCnpj")?.GetCPF_CNPJ() ?? string.Empty;
            nota.Prestador.InscricaoMunicipal = prestadorIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
        }

        nota.Prestador.RazaoSocial = rootPrestador.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.NomeFantasia = rootPrestador.ElementAnyNs("NomeFantasia")?.GetValue<string>() ?? string.Empty;

        // Endereco Prestador
        var enderecoPrestador = rootPrestador.ElementAnyNs("Endereco");
        if (enderecoPrestador != null)
        {
            nota.Prestador.Endereco.Logradouro = enderecoPrestador.ElementAnyNs("Endereco")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Numero = enderecoPrestador.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Complemento = enderecoPrestador.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Bairro = enderecoPrestador.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.CodigoMunicipio = enderecoPrestador.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
            nota.Prestador.Endereco.Uf = enderecoPrestador.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.Endereco.Cep = enderecoPrestador.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
        }

        // Contato Prestador
        var contatoPrestador = rootPrestador.ElementAnyNs("Contato");
        if (contatoPrestador != null)
        {
            nota.Prestador.DadosContato.Telefone = contatoPrestador.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.DadosContato.Email = contatoPrestador.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
        }
    }

    /// <inheritdoc />
    protected override void LoadIntermediario(NotaServico nota, XElement rootNFSe)
    {
        // Intermediario
        var rootIntermediario = rootNFSe.ElementAnyNs("Intermediario");
        if (rootIntermediario == null) return;

        nota.Intermediario.RazaoSocial = rootIntermediario.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;

        var intermediarioIdentificacao = rootIntermediario.ElementAnyNs("IdentificacaoIntermediario");
        if (intermediarioIdentificacao == null) return;

        nota.Intermediario.CpfCnpj = intermediarioIdentificacao.ElementAnyNs("CpfCnpj")?.GetCPF_CNPJ();
        nota.Intermediario.InscricaoMunicipal = intermediarioIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
        nota.Intermediario.CodigoMunicipio = intermediarioIdentificacao.ElementAnyNs("CodigoMunicipio")?.GetValue<string>() ?? string.Empty;
    }

    #endregion LoadXml

    #region RPS

    protected override XElement WriteServicosRps(NotaServico nota)
    {
        var servico = new XElement("Servico");

        servico.Add(WriteValoresRps(nota));

        servico.AddChild(AdicionarTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));

        if (nota.Servico.ResponsavelRetencao.HasValue)
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "ResponsavelRetencao", 1, 1, Ocorrencia.NaoObrigatoria, (int)nota.Servico.ResponsavelRetencao + 1));

        servico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));
        servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoCnae", 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));

        // Algumas prefeituras não permitem TAG Código de Tributacao
        // Sertãozinho/SP
        if (!Municipio.Codigo.IsIn(3551702))
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));

        servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoNbs", 1, 9, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoNbs));
        servico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
        servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoMunicipio", 1, 20, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));
        servico.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPais", 4, 4, Ocorrencia.MaiorQueZero, nota.Servico.CodigoPais));
        servico.AddChild(AdicionarTag(TipoCampo.Int, "", "ExigibilidadeISS", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Servico.ExigibilidadeIss + 1));
        servico.AddChild(AdicionarTag(TipoCampo.Str, "", "IdentifNaoExigibilidade", 1, 4, Ocorrencia.NaoObrigatoria, nota.Servico.IdentifNaoExigibilidade));
        servico.AddChild(AdicionarTag(TipoCampo.Int, "", "MunicipioIncidencia", 7, 7, Ocorrencia.MaiorQueZero, nota.Servico.MunicipioIncidencia));
        servico.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroProcesso", 1, 30, Ocorrencia.NaoObrigatoria, nota.Servico.NumeroProcesso));

        return servico;
    }

    protected override XElement WriteValoresRps(NotaServico nota)
    {
        var valores = new XElement("Valores");

        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValTotTributos", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValTotTributos));

        var valorISS = nota.Servico.Valores.ValorIss;

        if (valorISS <= 0 && nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao && nota.Servico.Valores.ValorIssRetido > 0)
            valorISS = nota.Servico.Valores.ValorIssRetido;

        if (nota.Prestador.Endereco.CodigoMunicipio != nota.Servico.MunicipioIncidencia)
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, valorISS));

        if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional || nota.Prestador.Endereco.CodigoMunicipio != nota.Servico.MunicipioIncidencia)
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "Aliquota", 1, 5, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota));

        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));

        return valores;
    }

    protected override XElement WriteTomadorRps(NotaServico nota)
    {
        if (nota.Tomador.CpfCnpj.IsEmpty()) return null;

        var tomador = new XElement("Tomador");

        var idTomador = new XElement("IdentificacaoTomador");
        tomador.Add(idTomador);

        var cpfCnpjTomador = new XElement("CpfCnpj");
        idTomador.Add(cpfCnpjTomador);

        cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

        idTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 150, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

        tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "NifTomador", 1, 150, Ocorrencia.NaoObrigatoria, nota.Tomador.DocEstrangeiro));
        tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 150, Ocorrencia.Obrigatoria, nota.Tomador.RazaoSocial));

        if (nota.Tomador.EnderecoExterior.CodigoPais > 0)
        {
            var enderecoExt = new XElement("EnderecoExterior");
            tomador.Add(enderecoExt);

            enderecoExt.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPais", 8, 8, Ocorrencia.Obrigatoria, nota.Tomador.EnderecoExterior.CodigoPais));
            enderecoExt.AddChild(AdicionarTag(TipoCampo.Str, "", "EnderecoCompletoExterior", 8, 8, Ocorrencia.Obrigatoria, nota.Tomador.EnderecoExterior.EnderecoCompleto));
        }
        else if (nota.Tomador.Endereco.CodigoMunicipio > 0)
        {
            var endereco = new XElement("Endereco");
            tomador.Add(endereco);

            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Logradouro));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Numero));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Bairro));
            endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.CodigoMunicipio));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Uf));
            endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Cep));
        }

        if (nota.Tomador.DadosContato.Email.IsEmpty() && nota.Tomador.DadosContato.Telefone.IsEmpty()) return tomador;

        var contato = new XElement("Contato");
        tomador.Add(contato);

        contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Telefone", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Telefone));
        contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));

        return tomador;
    }

    protected override XElement WriteIntermediarioRps(NotaServico nota)
    {
        if (nota.Intermediario.CpfCnpj.IsEmpty()) return null;

        var intermediario = new XElement("Intermediario");
        var ideIntermediario = new XElement("IdentificacaoIntermediario");
        intermediario.Add(ideIntermediario);

        var cpfCnpj = new XElement("CpfCnpj");
        ideIntermediario.Add(cpfCnpj);

        cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Intermediario.CpfCnpj));

        ideIntermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Intermediario.InscricaoMunicipal));
        intermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Intermediario.RazaoSocial));
        intermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoMunicipio", 1, 115, Ocorrencia.NaoObrigatoria, nota.Intermediario.CodigoMunicipio));

        return intermediario;
    }

    #endregion RPS

    #region NFSe

    /// <inheritdoc />
    protected override XElement WritePrestador(NotaServico nota)
    {
        var prestador = new XElement("PrestadorServico");
        prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 150, Ocorrencia.Obrigatoria, nota.Prestador.RazaoSocial));
        prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "NomeFantasia", 1, 60, Ocorrencia.NaoObrigatoria, nota.Prestador.NomeFantasia));

        var endereco = new XElement("Endereco");
        prestador.Add(endereco);

        endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.Logradouro));
        endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.Numero));
        endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Prestador.Endereco.Complemento));
        endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.Bairro));
        endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.CodigoMunicipio));
        endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.Uf));
        endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.Cep));

        if (nota.Prestador.DadosContato.Email.IsEmpty() && nota.Prestador.DadosContato.Telefone.IsEmpty()) return prestador;

        var contato = new XElement("Contato");
        prestador.Add(contato);

        contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Telefone", 8, 8, Ocorrencia.NaoObrigatoria, nota.Prestador.DadosContato.Telefone));
        contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 8, 8, Ocorrencia.NaoObrigatoria, nota.Prestador.DadosContato.Email));

        return prestador;
    }

    #endregion NFSe

    #endregion Methods
}