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

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
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

    #region LoadXml

    /// <inheritdoc />
    protected override void LoadRps(NotaServico nota, XElement rpsRoot)
    {
        base.LoadRps(nota, rpsRoot);
        LoadEvento(nota, rpsRoot);
        LoadDeducoes(nota, rpsRoot);
        
        nota.InformacoesComplementares = rpsRoot.ElementAnyNs("InformacoesComplementares")?.GetValue<string>() ?? string.Empty;
    }

    /// <inheritdoc />
    protected override void LoadTomador(NotaServico nota, XElement rpsRoot)
    {
        // Tomador
        var rootTomador = rpsRoot.ElementAnyNs("TomadorServico");
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

    /// <summary>
    /// Carrega os eventos do RPS
    /// </summary>
    /// <param name="nota"></param>
    /// <param name="rpsRoot"></param>
    protected virtual void LoadEvento(NotaServico nota, XElement rpsRoot)
    {
        var elemento = rpsRoot.ElementAnyNs("Evento");
        if(elemento == null) return;

        nota.Evento = new EventoRps
        {
            IdentificacaoEvento = elemento.ElementAnyNs("IdentificacaoEvento")?.GetValue<string>() ?? "",
            DescricaoEvento = elemento.ElementAnyNs("DescricaoEvento").GetValue<string>()
        };
    }

    /// <summary>
    /// Carrega os dados de deduções da RPs
    /// </summary>
    /// <param name="nota"></param>
    /// <param name="rpsRoot"></param>
    protected virtual void LoadDeducoes(NotaServico nota, XElement rpsRoot)
    {
        foreach (var elemento in rpsRoot.ElementsAnyNs("Deducao"))
        {
            var deducao = nota.Servico.Deducoes.AddNew();
            deducao.TipoDeducao = elemento.ElementAnyNs("TipoDeducao").GetValue<int>();
            deducao.Descricao = elemento.ElementAnyNs("DescricaoDeducao")?.GetValue<string>();
            deducao.DataEmissao = elemento.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;
            deducao.ValorDedutivel = elemento.ElementAnyNs("ValorDedutivel")?.GetValue<decimal>() ?? 0M;
            deducao.ValorUtilizadoDeducao = elemento.ElementAnyNs("ValorUtilizadoDeducao")?.GetValue<decimal>() ?? 0M;
            
            var ideNFSe = elemento.ElementAnyNs("IdentificacaoNfse");
            if (ideNFSe != null)
            {
                deducao.DocumentoDeducao.Tipo = TipoDocumentoDeducao.NFSe;
                deducao.DocumentoDeducao.CodigoMunicipio = ideNFSe.ElementAnyNs("CodigoMunicipioGerador")?.GetValue<string>() ?? "";
                deducao.DocumentoDeducao.NumeroNFe = ideNFSe.ElementAnyNs("NumeroNfse")?.GetValue<string>() ?? "";
                deducao.DocumentoDeducao.CodigoVerificacao = ideNFSe.ElementAnyNs("CodigoVerificacao")?.GetValue<string>();
            }
            
            var ideNFe = elemento.ElementAnyNs("IdentificacaoNfe");
            if (ideNFe != null)
            {
                deducao.DocumentoDeducao.Tipo = TipoDocumentoDeducao.NFe;
                deducao.DocumentoDeducao.NumeroNFe = ideNFe.ElementAnyNs("NumeroNfe")?.GetValue<string>() ?? "";
                deducao.DocumentoDeducao.UfNFe = ideNFe.ElementAnyNs("UfNfe")?.GetValue<string>() ?? "";
                deducao.DocumentoDeducao.ChaveNFe = ideNFe.ElementAnyNs("ChaveAcessoNfe")?.GetValue<string>();
            }
            
            var ideOutros = elemento.ElementAnyNs("ideOutros");
            if (ideOutros != null)
            {
                deducao.DocumentoDeducao.Tipo = TipoDocumentoDeducao.Outros;
                deducao.DocumentoDeducao.IdentificacaoDocumento = ideOutros.ElementAnyNs("IdentificacaoDocumento").GetValue<string>();
            }
        }
    }

    #endregion LoadXml

    #region RPS

    /// <inheritdoc />
    protected override XElement WriteRps(NotaServico nota)
    {
        var rootRps = base.WriteRps(nota);
        var info = rootRps.ElementAnyNs("InfDeclaracaoPrestacaoServico");

        if(nota.Evento != null) 
            info.AddChild(WriteEvento(nota));
        info.AddChild(AddTag(TipoCampo.Int, "", "InformacoesComplementares", 1, 2000, Ocorrencia.NaoObrigatoria, nota.InformacoesComplementares));
        
        if(nota.Servico.Deducoes.Count > 0)
            info.AddChild(WriteDeducoes(nota));
        
        return rootRps;
    }

    /// <inheritdoc />
    protected override XElement? WriteTomadorRps(NotaServico nota)
    {
        if (nota.Tomador.CpfCnpj.IsEmpty()) return null;

        var tomador = new XElement("TomadorServico");

        var idTomador = new XElement("IdentificacaoTomador");
        tomador.Add(idTomador);

        var cpfCnpjTomador = new XElement("CpfCnpj");
        idTomador.Add(cpfCnpjTomador);

        cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

        idTomador.AddChild(AddTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 150, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

        tomador.AddChild(AddTag(TipoCampo.Str, "", "NifTomador", 1, 150, Ocorrencia.NaoObrigatoria, nota.Tomador.DocEstrangeiro));
        tomador.AddChild(AddTag(TipoCampo.Str, "", "RazaoSocial", 1, 150, Ocorrencia.Obrigatoria, nota.Tomador.RazaoSocial));

        if (nota.Tomador.EnderecoExterior.CodigoPais > 0)
        {
            var enderecoExt = new XElement("EnderecoExterior");
            tomador.Add(enderecoExt);

            enderecoExt.AddChild(AddTag(TipoCampo.Int, "", "CodigoPais", 8, 8, Ocorrencia.Obrigatoria, nota.Tomador.EnderecoExterior.CodigoPais));
            enderecoExt.AddChild(AddTag(TipoCampo.Str, "", "EnderecoCompletoExterior", 8, 8, Ocorrencia.Obrigatoria, nota.Tomador.EnderecoExterior.EnderecoCompleto));
        }
        else
        {
            var endereco = new XElement("Endereco");
            tomador.Add(endereco);

            endereco.AddChild(AddTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Logradouro));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Numero));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Bairro));
            endereco.AddChild(AddTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.CodigoMunicipio));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Uf));
            endereco.AddChild(AddTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Cep));
        }

        if (nota.Tomador.DadosContato.Email.IsEmpty() && nota.Tomador.DadosContato.Telefone.IsEmpty()) return tomador;

        var contato = new XElement("Contato");
        tomador.Add(contato);

        contato.AddChild(AddTag(TipoCampo.Str, "", "Telefone", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Telefone));
        contato.AddChild(AddTag(TipoCampo.Str, "", "Email", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));

        return tomador;
    }

    /// <summary>
    /// Adiciona a tag eventos ao xml
    /// </summary>
    /// <param name="nota"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    protected virtual XElement WriteEvento(NotaServico nota)
    {
        if (nota.Evento == null) throw new ArgumentNullException(nameof(nota));
        
        var evento = new XElement("Evento");
        if(!nota.Evento!.IdentificacaoEvento.IsEmpty())
            evento.AddChild(AddTag(TipoCampo.Str, "", "IdentificacaoEvento", "", 1, 1, Ocorrencia.Obrigatoria, nota.Evento.IdentificacaoEvento));
        
        evento.AddChild(AddTag(TipoCampo.Str, "", "DescricaoEvento", "", 0, 1, Ocorrencia.NaoObrigatoria, nota.Evento.DescricaoEvento));
        return evento;
    }
    
    /// <summary>
    /// Adiciona as tags de deduções ai xml
    /// </summary>
    /// <param name="nota"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected virtual XElement[] WriteDeducoes(NotaServico nota)
    {
        var deducoes = new List<XElement>();
        foreach (var deducao in nota.Servico.Deducoes)
        {
            var elemento = new XElement("Deducao");
            elemento.AddChild(AddTag(TipoCampo.Int, "", "TipoDeducao", null, 1, 1, Ocorrencia.Obrigatoria, deducao.TipoDeducao));
            elemento.AddChild(AddTag(TipoCampo.Int, "", "DescricaoDeducao", null, 1, 150, Ocorrencia.NaoObrigatoria, deducao.Descricao));

            var identificacao = new XElement("IdentificacaoDocumentoDeducao");
            switch (deducao.DocumentoDeducao.Tipo)
            {
                case TipoDocumentoDeducao.NFSe:
                    var dadosNFSe = new XElement("IdentificacaoNfse");
                    dadosNFSe.AddChild(AddTag(TipoCampo.Str, "", "CodigoMunicipioGerador", null, 1, 1, Ocorrencia.Obrigatoria, deducao.DocumentoDeducao.CodigoMunicipio));
                    dadosNFSe.AddChild(AddTag(TipoCampo.Str, "", "NumeroNfse", null, 1, 1, Ocorrencia.Obrigatoria, deducao.DocumentoDeducao.NumeroNFSe));
                    dadosNFSe.AddChild(AddTag(TipoCampo.Str, "", "CodigoVerificacao", null, 1, 1, Ocorrencia.NaoObrigatoria, deducao.DocumentoDeducao.CodigoVerificacao));
                    identificacao.AddChild(dadosNFSe);
                    break;
                
                case TipoDocumentoDeducao.NFe:
                    var dadosNfe = new XElement("IdentificacaoNfe");
                    dadosNfe.AddChild(AddTag(TipoCampo.Str, "", "NumeroNfe", null, 1, 1, Ocorrencia.Obrigatoria, deducao.DocumentoDeducao.NumeroNFe));
                    dadosNfe.AddChild(AddTag(TipoCampo.Str, "", "UfNfe", null, 1, 1, Ocorrencia.Obrigatoria, deducao.DocumentoDeducao.UfNFe));
                    dadosNfe.AddChild(AddTag(TipoCampo.Str, "", "ChaveAcessoNfe", null, 1, 1, Ocorrencia.NaoObrigatoria, deducao.DocumentoDeducao.ChaveNFe));
                    identificacao.AddChild(dadosNfe);
                    break;
                
                case TipoDocumentoDeducao.Outros:
                    var dadosOutros = new XElement("OutroDocumento");
                    dadosOutros.AddChild(AddTag(TipoCampo.Str, "", "IdentificacaoDocumento", null, 1, 1, Ocorrencia.Obrigatoria, deducao.DocumentoDeducao.IdentificacaoDocumento));
                    identificacao.AddChild(dadosOutros);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            elemento.AddChild(identificacao);
            
            var fornecedor = new XElement("DadosFornecedor");
            if (deducao.DadosFornecedor.Nif.IsEmpty())
            {
                var dados = new XElement("IdentificacaoFornecedor");
                dados.AddChild(AddTag(TipoCampo.Str, "", "CpfCnpj", null, 11, 14, Ocorrencia.Obrigatoria, deducao.DadosFornecedor.Documento));
                
                fornecedor.AddChild(dados);
            }
            else
            {
                var dados = new XElement("FornecedorExterior");
                dados.AddChild(AddTag(TipoCampo.Str, "", "NifFornecedor", null, 1, 40, Ocorrencia.NaoObrigatoria, deducao.DadosFornecedor.Nif));
                dados.AddChild(AddTag(TipoCampo.Str, "", "CodigoPais", null, 4, 4, Ocorrencia.Obrigatoria, deducao.DadosFornecedor.CodigoPais));
                
                fornecedor.AddChild(dados);
            }
            elemento.AddChild(fornecedor);
            
            elemento.AddChild(AddTag(TipoCampo.Dat, "", "DataEmissao", null, 1, 150, Ocorrencia.Obrigatoria, deducao.DataEmissao));
            elemento.AddChild(AddTag(TipoCampo.De2, "", "ValorDedutivel", null, 1, 150, Ocorrencia.Obrigatoria, deducao.ValorDedutivel));
            elemento.AddChild(AddTag(TipoCampo.De2, "", "ValorUtilizadoDeducao", null, 1, 150, Ocorrencia.Obrigatoria, deducao.ValorUtilizadoDeducao));
            deducoes.Add(elemento);
        }

        return deducoes.ToArray();
    }

    #endregion RPS
}