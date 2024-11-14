// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 28-07-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="ProviderGinfes.cs" company="OpenAC .Net">
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
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProvideriiBrasil : ProviderABRASF204
{
    #region Internal Types

    private enum LoadXmlFormato
    {
        Indefinido,
        NFSe,
        Rps
    }

    #endregion Internal Types

    #region Constructors

    public ProvideriiBrasil(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "iiBrasil";
    }

    #endregion Constructors

    #region Methods

    protected override IServiceClient GetClient(TipoUrl tipo) => new iiBrasilServiceClient(this, tipo);

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
    }

    protected override void AssinarGerarNfse(RetornoGerarNfse retornoWebservice)
    {
    }

    protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
    {
    }

    protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
    {
    }

    protected override void PrepararGerarNfse(RetornoGerarNfse retornoWebservice, NotaServico nota)
    {
        var xmlLote = new StringBuilder();
        xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        xmlLote.Append("<GerarNfseEnvio xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");

        var xmlRps = WriteXmlRps(nota, false, false);

        AdicionarTagIntegridade(ref xmlRps);

        GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);

        xmlLote.Append(xmlRps);
        xmlLote.Append("</GerarNfseEnvio>");
        retornoWebservice.XmlEnvio = xmlLote.ToString();
    }

    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        if (retornoWebservice.NumeroNFSe.IsEmpty() || retornoWebservice.CodigoCancelamento.IsEmpty())
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "AC0001", Descricao = "Número da NFSe/Codigo de cancelamento não informado para cancelamento." });
            return;
        }

        var loteBuilder = new StringBuilder();

        loteBuilder.Append($"<CancelarNfseEnvio {GetNamespace()}>");

        var infoBuilder = new StringBuilder();

        infoBuilder.Append("<Pedido>");
        infoBuilder.Append($"<InfPedidoCancelamento Id=\"N{retornoWebservice.NumeroNFSe}\">");
        infoBuilder.Append("<IdentificacaoNfse>");
        infoBuilder.Append($"<Numero>{retornoWebservice.NumeroNFSe}</Numero>");
        infoBuilder.Append($"<Serie>{retornoWebservice.SerieNFSe}</Serie>");
        infoBuilder.Append("<CpfCnpj>");
        infoBuilder.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
            ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>"
            : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
        infoBuilder.Append("</CpfCnpj>");
        if (!Configuracoes.PrestadorPadrao.InscricaoMunicipal.IsEmpty()) infoBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        infoBuilder.Append($"<CodigoMunicipio>{Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio}</CodigoMunicipio>");
        infoBuilder.Append("</IdentificacaoNfse>");
        infoBuilder.Append($"<CodigoCancelamento>{retornoWebservice.CodigoCancelamento}</CodigoCancelamento>");
        infoBuilder.Append("</InfPedidoCancelamento>");
        infoBuilder.Append("</Pedido>");
        infoBuilder.Append(GerarTagIntegridade(infoBuilder.ToString()));

        loteBuilder.Append(infoBuilder);
        loteBuilder.Append("</CancelarNfseEnvio>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.NumeroRps < 1)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Número da RPS não informado para a consulta." });
            return;
        }

        var loteBuilder = new StringBuilder();
        loteBuilder.Append($"<ConsultarNfseRpsEnvio {GetNamespace()}>");

        var infoBuilder = new StringBuilder();

        infoBuilder.Append("<IdentificacaoRps>");
        infoBuilder.Append($"<Numero>{retornoWebservice.NumeroRps}</Numero>");
        infoBuilder.Append($"<Serie>{retornoWebservice.Serie}</Serie>");
        infoBuilder.Append($"<Tipo>{(int)retornoWebservice.Tipo + 1}</Tipo>");
        infoBuilder.Append("</IdentificacaoRps>");
        infoBuilder.Append("<Prestador>");
        infoBuilder.Append("<CpfCnpj>");
        infoBuilder.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
            ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>"
            : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
        infoBuilder.Append("</CpfCnpj>");
        if (!Configuracoes.PrestadorPadrao.InscricaoMunicipal.IsEmpty()) infoBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        infoBuilder.Append("</Prestador>");
        infoBuilder.Append(GerarTagIntegridade(infoBuilder.ToString()));

        loteBuilder.Append(infoBuilder);
        loteBuilder.Append("</ConsultarNfseRpsEnvio>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    #endregion Methods

    #region RPS

    protected override XElement? WriteTomadorRps(NotaServico nota)
    {
        var tomador = new XElement("TomadorServico");

        if (!nota.Tomador.CpfCnpj.IsEmpty())
        {
            var ideTomador = new XElement("IdentificacaoTomador");
            tomador.Add(ideTomador);

            var cpfCnpjTomador = new XElement("CpfCnpj");
            ideTomador.Add(cpfCnpjTomador);

            cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

            ideTomador.AddChild(AddTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15,
                Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));
        }

        tomador.AddChild(AddTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));

        if (!nota.Tomador.Endereco.Logradouro.IsEmpty() ||
            !nota.Tomador.Endereco.Numero.IsEmpty() ||
            !nota.Tomador.Endereco.Complemento.IsEmpty() ||
            !nota.Tomador.Endereco.Bairro.IsEmpty() ||
            nota.Tomador.Endereco.CodigoMunicipio > 0 ||
            !nota.Tomador.Endereco.Uf.IsEmpty() ||
            nota.Tomador.Endereco.CodigoPais > 0 ||
            !nota.Tomador.Endereco.Cep.IsEmpty())
        {
            var endereco = new XElement("Endereco");
            tomador.Add(endereco);

            endereco.AddChild(AddTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
            endereco.AddChild(AddTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoMunicipio));
            endereco.AddChild(AddTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
            endereco.AddChild(AddTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
        }

        if (!nota.Tomador.DadosContato.Telefone.IsEmpty() ||
           !nota.Tomador.DadosContato.Email.IsEmpty())
        {
            var contato = new XElement("Contato");
            tomador.Add(contato);

            contato.AddChild(AddTag(TipoCampo.StrNumber, "", "Telefone", 1, 11, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));
            contato.AddChild(AddTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
        }

        var tomadorAdicional = new XElement("AtualizaTomador", "2");
        tomador.Add(tomadorAdicional);

        tomadorAdicional = new XElement("TomadorExterior", "2");
        tomador.Add(tomadorAdicional);

        return tomador;
    }

    private string GerarTagIntegridade(string xml)
    {
        string tag = xml;

        tag = Regex.Replace(tag, "/[^\x20-\x7E]+/", "");
        tag = Regex.Replace(tag, "/[ ]+/", "");

        if (Configuracoes.WebServices.ChavePrivada == "")
            throw new Exception("O token deve ser informado para esse provedor");

        string integridade = sha512(Configuracoes.WebServices.ChavePrivada + tag);

        var nodeIntegridade = new XElement("Integridade", integridade);

        return nodeIntegridade.AsString(showDeclaration: false);
    }

    private void AdicionarTagIntegridade(ref string xml)
    {
        xml += GerarTagIntegridade(xml);
    }

    #endregion RPS

    #region Uteis

    public static string sha512(string inputString)
    {
        SHA512 sha512 = SHA512.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(inputString);
        byte[] hash = sha512.ComputeHash(bytes);
        return GetStringFromHash(hash);
    }

    private static string GetStringFromHash(byte[] hash)
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            result.Append(hash[i].ToString("X2"));
        }
        return result.ToString();
    }

    #endregion Uteis

}