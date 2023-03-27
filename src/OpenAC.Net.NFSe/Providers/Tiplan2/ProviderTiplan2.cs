// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 03-27-2023
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 03-27-2023
// ***********************************************************************
// <copyright file="ProviderTiplan2.cs" company="OpenAC .Net">
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
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class ProviderTiplan2 : ProviderABRASF203
    {
        #region Constructors

        public ProviderTiplan2(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Tiplan2";
            Versao = "2.03";
        }

        #endregion Constructors

        #region Methods

        protected override IServiceClient GetClient(TipoUrl tipo) => new Tiplan2ServiceClient(this, tipo, Certificado);

        //protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        //{
        //    retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "InfDeclaracaoPrestacaoServico", Certificado, true, false, true, SignDigest.SHA1);
        //    retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsSincronoEnvio", "LoteRps", Certificado, true, false, true, SignDigest.SHA1);
        //}

        #endregion Methods

        //#region RPS

        //protected override XElement WriteRps(NotaServico nota)
        //{
        //    var rootRps = new XElement("Rps");

        //    var infServico = new XElement("InfDeclaracaoPrestacaoServico", new XAttribute("Id", $"R{nota.IdentificacaoRps.Numero.OnlyNumbers()}"));
        //    rootRps.Add(infServico);

        //    infServico.Add(WriteRpsRps(nota));

        //    infServico.AddChild(AdicionarTag(TipoCampo.Dat, "", "Competencia", 10, 10, Ocorrencia.Obrigatoria, nota.Competencia));

        //    infServico.AddChild(WriteServicosRps(nota));
        //    infServico.AddChild(WritePrestadorRps(nota));
        //    infServico.AddChild(WriteTomadorRps(nota));
        //    infServico.AddChild(WriteIntermediarioRps(nota));
        //    infServico.AddChild(WriteConstrucaoCivilRps(nota));

        //    string regimeEspecialTributacao;
        //    string optanteSimplesNacional;
        //    if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
        //    {
        //        regimeEspecialTributacao = "6";
        //        optanteSimplesNacional = "1";
        //    }
        //    else
        //    {
        //        regimeEspecialTributacao = ((int)nota.RegimeEspecialTributacao).ToString();
        //        optanteSimplesNacional = "2";
        //    }

        //    //if (nota.RegimeEspecialTributacao != RegimeEspecialTributacao.Nenhum)
        //    //    infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));

        //    infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
        //    infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivoFiscal", 1, 1, Ocorrencia.Obrigatoria, nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2));

        //    return rootRps;
        //}

        //protected override XElement WriteRpsRps(NotaServico nota)
        //{
        //    var rps = new XElement("Rps");

        //    rps.Add(WriteIdentificacaoRps(nota));

        //    rps.AddChild(AdicionarTag(TipoCampo.Dat, "", "DataEmissao", 10, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
        //    rps.AddChild(AdicionarTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Situacao + 1));

        //    rps.AddChild(WriteSubstituidoRps(nota));

        //    return rps;
        //}

        //protected override XElement WriteTomadorRps(NotaServico nota)
        //{
        //    var tomador = new XElement("Tomador");

        //    if (!nota.Tomador.CpfCnpj.IsEmpty())
        //    {
        //        var ideTomador = new XElement("IdentificacaoTomador");
        //        tomador.Add(ideTomador);

        //        var cpfCnpjTomador = new XElement("CpfCnpj");
        //        ideTomador.Add(cpfCnpjTomador);

        //        cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

        //        ideTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15,
        //            Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));
        //    }

        //    tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));

        //    if (!nota.Tomador.Endereco.Logradouro.IsEmpty() ||
        //        !nota.Tomador.Endereco.Numero.IsEmpty() ||
        //        !nota.Tomador.Endereco.Complemento.IsEmpty() ||
        //        !nota.Tomador.Endereco.Bairro.IsEmpty() ||
        //        nota.Tomador.Endereco.CodigoMunicipio > 0 ||
        //        !nota.Tomador.Endereco.Uf.IsEmpty() ||
        //        nota.Tomador.Endereco.CodigoPais > 0 ||
        //        !nota.Tomador.Endereco.Cep.IsEmpty())
        //    {
        //        var endereco = new XElement("Endereco");
        //        tomador.Add(endereco);

        //        endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
        //        endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
        //        endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
        //        endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
        //        endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoMunicipio));
        //        endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
        //        //endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPais", 4, 4, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoPais));
        //        endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
        //    }

        //    if (!nota.Tomador.DadosContato.Telefone.IsEmpty() ||
        //        !nota.Tomador.DadosContato.Email.IsEmpty())
        //    {
        //        var contato = new XElement("Contato");
        //        tomador.Add(contato);

        //        contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Telefone", 1, 11, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));
        //        contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
        //    }

        //    return tomador;
        //}


        //#endregion RPS

    }
}