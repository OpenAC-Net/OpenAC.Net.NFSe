// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 06-22-2022
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 06-22-2022
// ***********************************************************************
// <copyright file="ProviderDSFSJC.cs" company="OpenAC .Net">
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

using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderDSF : ProviderABRASF
{
    #region Constructors

    public ProviderDSF(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "DSF";
    }

    #endregion Constructors

    #region Private Methods

    protected override IServiceClient GetClient(TipoUrl tipo) => new DSFServiceClient(this, tipo);

    protected override string GerarCabecalho()
    {
        var cabecalho = new StringBuilder();
        cabecalho.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        cabecalho.Append("<ns2:cabecalho versao=\"3\" xmlns:ns2=\"http://www.abrasf.org.br/nfse.xsd\">");
        cabecalho.Append("<versaoDados>3</versaoDados>");
        cabecalho.Append("</ns2:cabecalho>");
        return cabecalho.ToString();
    }

    #endregion Private Methods
}