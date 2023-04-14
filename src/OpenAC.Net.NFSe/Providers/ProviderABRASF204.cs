// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Adriano Trentim
// Created          : 22-01-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-02-2020
// ***********************************************************************
// <copyright file="ProviderABRASF204.cs" company="OpenAC .Net">
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
public abstract class ProviderABRASF204 : ProviderABRASF203
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderABRASF204"/> class.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="municipio">The municipio.</param>
    protected ProviderABRASF204(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "ABRASF";
        Versao = VersaoNFSe.ve204;
        UsaPrestadorEnvio = true;
    }

    #endregion Constructors

    #region RPS

    /// <inheritdoc />
    //ToDo: Implementar a tag evento, informações complementares e a lista de deduções.
    protected override XElement WriteRps(NotaServico nota)
    {
        var rootRps = base.WriteRps(nota);
        //var info = rootRps.ElementAnyNs("InfDeclaracaoPrestacaoServico");
        
        //info.AddChild(AdicionarTag(TipoCampo.Int, "", "InformacoesComplementares", 1, 2000, Ocorrencia.NaoObrigatoria, nota.InformacoesComplementares));

        return rootRps;
    }

    /// <inheritdoc />
    //ToDo: Ler a tag evento, informações complementares e a lista de deduções.
    protected override void LoadRps(NotaServico nota, XElement rpsRoot)
    {
        base.LoadRps(nota, rpsRoot);
    }

    //ToDo: Ler novas informações do Xml.
    protected override XElement WriteTomadorRps(NotaServico nota)
    {
        var tomador = new XElement("TomadorServico");

        var ideTomador = new XElement("IdentificacaoTomador");
        tomador.Add(ideTomador);

        var cpfCnpjTomador = new XElement("CpfCnpj");
        ideTomador.Add(cpfCnpjTomador);

        cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

        ideTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

        tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));

        if (!nota.Tomador.Endereco.Logradouro.IsEmpty() || !nota.Tomador.Endereco.Numero.IsEmpty() ||
            !nota.Tomador.Endereco.Complemento.IsEmpty() || !nota.Tomador.Endereco.Bairro.IsEmpty() ||
            nota.Tomador.Endereco.CodigoMunicipio > 0 || !nota.Tomador.Endereco.Uf.IsEmpty() ||
            !nota.Tomador.Endereco.Cep.IsEmpty())
        {
            var endereco = new XElement("Endereco");
            tomador.Add(endereco);

            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
            endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoMunicipio));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
            endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
        } 
        else if (!nota.Tomador.EnderecoExterior.EnderecoCompleto.IsEmpty())
        {
            var endereco = new XElement("EnderecoExterior");
            tomador.Add(endereco);
            
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoPais", 4, 4, Ocorrencia.Obrigatoria, nota.Tomador.EnderecoExterior.CodigoPais));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "EnderecoCompletoExterior", 1, 255, Ocorrencia.Obrigatoria, nota.Tomador.EnderecoExterior.EnderecoCompleto));
        }

        if (!nota.Tomador.DadosContato.DDD.IsEmpty() || !nota.Tomador.DadosContato.Telefone.IsEmpty() ||
            !nota.Tomador.DadosContato.Email.IsEmpty())
        {
            var contato = new XElement("Contato");
            tomador.Add(contato);

            contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Telefone", 1, 11, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));
            contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
        }

        return tomador;
    }

    #endregion RPS
}