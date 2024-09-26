// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Valnei Filho v_marinpietri@yahoo.com.br
// Created          : 24-07-2022
//
// Last Modified By : Rafael Dias
// Last Modified On : 26-08-2022
// ***********************************************************************
// <copyright file="ProviderMetropolisWeb.cs" company="OpenAC .Net">
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

using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderMetropolisWeb : ProviderABRASF
{
    #region Construtor

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProviderABRASF" /> class.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="municipio">The municipio.</param>
    public ProviderMetropolisWeb(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "MetropolisWeb";
    }

    #endregion Construtor

    #region Methods

    protected override bool PrecisaValidarSchema(TipoUrl tipo) => false;

    /// <summary>
    ///     Retorna o cliente de comunicação com o webservice.
    /// </summary>
    /// <param name="tipo"></param>
    /// <returns></returns>
    protected override IServiceClient GetClient(TipoUrl tipo) => new MetropolisWebClient(this, tipo, Certificado);

    #endregion Methods
}