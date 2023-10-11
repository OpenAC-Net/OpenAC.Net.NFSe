// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 31-05-2022
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 31-05-2022
// ***********************************************************************
// <copyright file="ProviderSintese.cs" company="OpenAC .Net">
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

using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderSintese204 : ProviderABRASF204
{
    #region Constructors

    public ProviderSintese204(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "Sintese";
    }

    #endregion Constructors

    #region Methods

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "Rps", "", Certificado);
    }

    protected override IServiceClient GetClient(TipoUrl tipo) => new Sintese204ServiceClient(this, tipo, Certificado);

    protected override bool PrecisaValidarSchema(TipoUrl tipo) => false;

    protected override string GetVersao() => "versao=\"1.00\"";
    
    protected override string GerarCabecalho() => $"<cabecalho {GetVersao()} {GetNamespace()}><versaoDados>1.00</versaoDados></cabecalho>";
    
    #endregion Methods
}