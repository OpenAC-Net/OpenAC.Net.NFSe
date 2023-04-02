// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 21-01-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 21-01-2020
// ***********************************************************************
// <copyright file="ProviderSmarAPDABRASF.cs" company="OpenAC .Net">
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

using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderSmarAPDABRASF : ProviderABRASF204
{
    #region Constructors

    public ProviderSmarAPDABRASF(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "SmarAPDABRASF";
    }

    #endregion Constructors

    #region Methods

    #region Protected Methods

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new SmarAPDABRASFServiceClient(this, tipo, Certificado);
    }

    protected override XElement WriteTomadorRps(NotaServico nota)
    {
        if (nota.Tomador.CpfCnpj.IsEmpty()) return null;

        var tomador = new XElement("TomadorServico");

        var idTomador = new XElement("IdentificacaoTomador");
        tomador.Add(idTomador);

        var cpfCnpjTomador = new XElement("CpfCnpj");
        idTomador.Add(cpfCnpjTomador);

        cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

        idTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 150, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

        tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "NifTomador", 1, 150, Ocorrencia.NaoObrigatoria, nota.Tomador.DocTomadorEstrangeiro));
        tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 150, Ocorrencia.Obrigatoria, nota.Tomador.RazaoSocial));

        if (nota.Tomador.EnderecoExterior.CodigoPais > 0)
        {
            var enderecoExt = new XElement("EnderecoExterior");
            tomador.Add(enderecoExt);

            enderecoExt.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPais", 8, 8, Ocorrencia.Obrigatoria, nota.Tomador.EnderecoExterior.CodigoPais));
            enderecoExt.AddChild(AdicionarTag(TipoCampo.Str, "", "EnderecoCompletoExterior", 8, 8, Ocorrencia.Obrigatoria, nota.Tomador.EnderecoExterior.EnderecoCompleto));
        }
        else
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

    #endregion Protected Methods

    #endregion Methods
}