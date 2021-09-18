// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Adriano Trentim
// Created          : 22-01-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-02-2020
// ***********************************************************************
// <copyright file="ProviderABRASF204.cs" company="OpenAC .Net">
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

using System.Xml.Linq;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Classe base para trabalhar com provedores que usam o padrão ABRASF 2.04
    /// </summary>
    /// <seealso cref="ProviderBase" />
    public abstract class ProviderABRASF204 : ProviderABRASF203
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderABRASF204"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="municipio">The municipio.</param>
        protected ProviderABRASF204(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "ABRASFv204";
            Versao = "2.04";
            UsaPrestadorEnvio = true;
        }

        #endregion Constructors

        #region RPS

        /// <inheritdoc />
        protected override XElement WriteRps(NotaServico nota)
        {
            var rootRps = base.WriteRps(nota);

            //ToDo: Implementar a tag evento, informações complementares e a lista de deduções.

            return rootRps;
        }

        /// <inheritdoc />
        protected override void LoadRps(NotaServico nota, XElement rpsRoot)
        {
            base.LoadRps(nota, rpsRoot);

            //ToDo: Ler a tag evento, informações complementares e a lista de deduções.
        }

        #endregion RPS
    }
}