// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 10-01-2014
//
// Last Modified By : Rafael Dias
// Last Modified On : 10-01-2014
// ***********************************************************************
// <copyright path="NotaServicoCollection.cs" company="OpenAC .Net">
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

using System.IO;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Collection;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Nota
{
    public sealed class NotaServicoCollection : DFeCollection<NotaServico>
    {
        #region Fields

        private readonly ConfigNFSe config;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Inicializa uma nova instacia da classe <see cref="NotaServicoCollection" />.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public NotaServicoCollection(ConfigNFSe config)
        {
            Guard.Against<OpenDFeException>(config == null, "Configurações não podem ser nulas");

            this.config = config;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Adiciona uma nova nota fiscal na coleção.
        /// </summary>
        /// <returns>T.</returns>
        public override NotaServico AddNew()
        {
            var nota = new NotaServico(config, config.PrestadorPadrao);
            Add(nota);
            return nota;
        }

        /// <summary>
        /// Carrega a NFSe/RPS do arquivo.
        /// </summary>
        /// <param name="xml">caminho do arquivo XML ou string com o XML.</param>
        /// <param name="encoding">encoding do XML.</param>
        /// <returns>NotaServico carregada.</returns>
        public NotaServico Load(string xml, Encoding encoding = null)
        {
            var provider = ProviderManager.GetProvider(config);

            try
            {
                var nota = provider.LoadXml(xml, encoding);
                nota.XmlOriginal = xml;
                Add(nota);
                return nota;
            }
            finally
            {
                provider.Dispose();
            }
        }

        /// <summary>
        /// Carrega a NFSe/RPS do xml.
        /// </summary>
        /// <param name="stream">Stream do XML.</param>
        /// <returns>NotaServico carregada.</returns>
        public NotaServico Load(Stream stream)
        {
            var provider = ProviderManager.GetProvider(config);

            var nota = provider.LoadXml(stream);

            try
            {
                stream.Position = 0;
                using (var sr = new StreamReader(stream))
                    nota.XmlOriginal = sr.ReadToEnd();

                Add(nota);
                return nota;
            }
            finally
            {
                provider.Dispose();
            }
        }

        /// <summary>
        /// Carrega a NFSe/RPS do XMLDocument.
        /// </summary>
        /// <param name="xml">XMLDocument da NFSe/RPS.</param>
        /// <returns>NotaServico carregada.</returns>
        public NotaServico Load(XDocument xml)
        {
            var provider = ProviderManager.GetProvider(config);

            try
            {
                var nota = provider.LoadXml(xml);
                nota.XmlOriginal = xml.AsString();
                Add(nota);
                return nota;
            }
            finally
            {
                provider.Dispose();
            }
        }

        #endregion Methods
    }
}