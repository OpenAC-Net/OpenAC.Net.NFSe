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

        protected override string GerarCabecalho() => $"<cabecalho {GetVersao()} {GetNamespace()} xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><versaoDados>{Versao}</versaoDados></cabecalho>";

        protected override IServiceClient GetClient(TipoUrl tipo) => new Tiplan2ServiceClient(this, tipo, Certificado);

        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
            //NAO PRECISA ASSINAR
        }

        #endregion Methods

    }
}