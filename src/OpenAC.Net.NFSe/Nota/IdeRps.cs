// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 01-07-2015
//
// Last Modified By : Rafael Dias
// Last Modified On : 01-07-2015
// ***********************************************************************
// <copyright file="IdeRps.cs" company="OpenAC .Net">
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

using System;
using System.ComponentModel;
using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota
{
    public sealed class IdeRps : GenericClone<IdeRps>, INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeRps"/> class.
        /// </summary>
        internal IdeRps()
        {
            Numero = string.Empty;
            Serie = string.Empty;
            Tipo = TipoRps.RPS;
        }

        #endregion Constructor

        #region Propriedades

        /// <summary>
        /// Gets or sets the numero.
        /// </summary>
        /// <value>The numero.</value>
        public string Numero { get; set; }

        /// <summary>
        /// Gets or sets the serie.
        /// </summary>
        /// <value>The serie.</value>
        public string Serie { get; set; }

        /// <summary>
        /// Gets or sets the serie prestacao.
        /// </summary>
        /// <value>The serie prestacao.</value>
        public string SeriePrestacao { get; set; }

        /// <summary>
        /// Gets or sets the tipo.
        /// </summary>
        /// <value>The tipo.</value>
        public TipoRps Tipo { get; set; }

        /// <summary>
        /// Gets or sets the data emissao RPS.
        /// </summary>
        /// <value>The data emissao RPS.</value>
        public DateTime DataEmissao { get; set; }

        /// <summary>
        /// Gets or sets the status do RPS.
        /// </summary>
        /// <value>status do RPS.</value>
        public StatusRps StatusRps { get; set; }

        #endregion Propriedades
    }
}