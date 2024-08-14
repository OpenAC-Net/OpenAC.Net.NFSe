// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira/Transis
// Created          : 02-14-2020
//
// Last Modified By : Felipe Silveira/Transis
// Last Modified On : 02-17-2020
// ***********************************************************************
// <copyright file="ProviderFiorilli.cs" company="OpenAC .Net">
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

using System.Linq;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderPronim202 : ProviderABRASF202
{
    #region Constructors

    public ProviderPronim202(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "Pronim";
    }

    #endregion Constructors

    #region Methods

    protected override string GerarCabecalho()
    {
        return "<tem:cabecalho versao=\"202\">" +
               "<tem:versaoDados>2.02</tem:versaoDados>" +
               "</tem:cabecalho>";
    }

    protected override IServiceClient GetClient(TipoUrl tipo) => new Pronim202ServiceClient(this, tipo, null);

    protected override void ValidarSchema(RetornoWebservice retorno, string schema)
    {
        base.ValidarSchema(retorno, schema);
        if(retorno.Erros.Count > 0) return;

        retorno.XmlEnvio = retorno.XmlEnvio.Replace(" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"", "");
    }

    #endregion Methods
}