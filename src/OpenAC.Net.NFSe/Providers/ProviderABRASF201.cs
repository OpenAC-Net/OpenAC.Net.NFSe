// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 27-01-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-02-2020
// ***********************************************************************
// <copyright file="ProviderABRASF201.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2022 Projeto OpenAC .Net
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
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers
{
    /// <summary>
    /// Classe base para trabalhar com provedores que usam o padrão ABRASF 2.02
    /// </summary>
    /// <seealso cref="ProviderBase" />
    public abstract class ProviderABRASF201 : ProviderABRASF200
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderABRASF201"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="municipio">The municipio.</param>
        protected ProviderABRASF201(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "ABRASFv201";
            Versao = "2.01";
            UsaPrestadorEnvio = false;
        }

        #endregion Constructors

        #region RPS

        protected override XElement WriteRps(NotaServico nota)
        {
            var rootRps = new XElement("Rps");

            var infServico = new XElement("InfDeclaracaoPrestacaoServico", new XAttribute("Id", $"R{nota.IdentificacaoRps.Numero.OnlyNumbers()}"));
            rootRps.Add(infServico);

            infServico.Add(WriteRpsRps(nota));

            infServico.AddChild(AdicionarTag(TipoCampo.Dat, "", "Competencia", 10, 10, Ocorrencia.Obrigatoria, nota.Competencia));

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
                optanteSimplesNacional =  "2";
            }

            if (nota.OptanteSimplesNacional.HasValue)
                optanteSimplesNacional = nota.OptanteSimplesNacional == true ? "1" : "2";

            if (nota.RegimeEspecialTributacao != RegimeEspecialTributacao.Nenhum)
                infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));

            infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
            infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivoFiscal", 1, 1, Ocorrencia.Obrigatoria, nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2));

            return rootRps;
        }

        protected override XElement WriteRpsRps(NotaServico nota)
        {
            var rps = new XElement("Rps");

            rps.Add(WriteIdentificacaoRps(nota));

            rps.AddChild(AdicionarTag(TipoCampo.Dat, "", "DataEmissao", 10, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            rps.AddChild(AdicionarTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Situacao + 1));

            rps.AddChild(WriteSubstituidoRps(nota));

            return rps;
        }

        #endregion RPS

        #region NFSe

        protected override XElement WritePrestador(NotaServico nota)
        {
            var prestador = new XElement("PrestadorServico");

            prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 150, Ocorrencia.Obrigatoria, nota.Prestador.RazaoSocial));
            prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "NomeFantasia", 1, 60, Ocorrencia.NaoObrigatoria, nota.Prestador.NomeFantasia));

            var indPrestador = new XElement("IdentificacaoPrestador");
            prestador.AddChild(indPrestador);

            var cpfCNPJ = new XElement("CpfCnpj");
            indPrestador.AddChild(cpfCNPJ);

            cpfCNPJ.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj));
            indPrestador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Prestador.InscricaoMunicipal));

            var endereco = new XElement("Endereco");
            prestador.AddChild(endereco);

            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria, nota.Prestador.Endereco.Logradouro));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Prestador.Endereco.Numero));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Prestador.Endereco.Complemento));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Prestador.Endereco.Bairro));
            endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero, nota.Prestador.Endereco.CodigoMunicipio));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Prestador.Endereco.Uf));
            endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPais", 4, 4, Ocorrencia.MaiorQueZero, nota.Prestador.Endereco.CodigoPais));
            endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Prestador.Endereco.Cep));

            if (!nota.Prestador.DadosContato.Telefone.IsEmpty() ||
                !nota.Prestador.DadosContato.Email.IsEmpty())
            {
                var contato = new XElement("contato");
                prestador.AddChild(contato);

                contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Telefone", 1, 20, Ocorrencia.NaoObrigatoria, nota.Prestador.DadosContato.Telefone));
                contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria, nota.Prestador.DadosContato.Email));
            }

            return prestador;
        }

        /// <inheritdoc />
        protected override void LoadPrestador(NotaServico nota, XElement rootNFSe)
        {
            // Endereco Prestador
            var prestadorServico = rootNFSe.ElementAnyNs("PrestadorServico");
            if (prestadorServico == null) return;

            nota.Prestador.RazaoSocial = prestadorServico.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.NomeFantasia = prestadorServico.ElementAnyNs("NomeFantasia")?.GetValue<string>() ?? string.Empty;

            var indPrestador = prestadorServico.ElementAnyNs("IdentificacaoPrestador");
            if (indPrestador != null)
            {
                nota.Prestador.CpfCnpj = indPrestador.ElementAnyNs("CpfCnpj")?.GetCPF_CNPJ();
                nota.Prestador.InscricaoMunicipal = indPrestador.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
            }

            // Endereco Prestador
            var enderecoPrestador = prestadorServico.ElementAnyNs("Endereco");
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
            var contatoPrestador = prestadorServico.ElementAnyNs("Contato");
            if (contatoPrestador == null) return;

            nota.Prestador.DadosContato.Telefone = contatoPrestador.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.DadosContato.Email = contatoPrestador.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
        }

        #endregion NFSe

        #region Services

        /// <inheritdoc />
        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "InfDeclaracaoPrestacaoServico", Certificado);
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsEnvio", "LoteRps", Certificado);
        }

        /// <inheritdoc />
        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "InfDeclaracaoPrestacaoServico", Certificado);
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsSincronoEnvio", "LoteRps", Certificado);
        }

        #endregion Services
    }
}