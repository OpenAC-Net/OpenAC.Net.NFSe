// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 18-08-2021
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 18-08-2021
// ***********************************************************************
// <copyright file="ProviderSystemPro.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2021 Projeto OpenAC .Net
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

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class ProviderSystemPro : ProviderABRASF201
    {
        #region Constructors

        public ProviderSystemPro(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "SystemPro";
        }

        #endregion Constructors

        #region Methods

        #region Protected Methods

        protected override string GerarCabecalho()
        {
            var cabecalho = new System.Text.StringBuilder();
            cabecalho.Append("<cabecalho versao=\"2.01\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            cabecalho.Append("<versaoDados>2.01</versaoDados>");
            cabecalho.Append("</cabecalho>");
            return cabecalho.ToString();
        }

        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsEnvio", "LoteRps", Certificado);
        }

        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsSincronoEnvio", "LoteRps", Certificado);
        }

        protected override IServiceClient GetClient(TipoUrl tipo) => new SystemProServiceClient(this, tipo, Certificado);

        #endregion Protected Methods

        #endregion Methods
    }
}