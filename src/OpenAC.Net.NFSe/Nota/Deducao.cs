// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 10-01-2014
//
// Last Modified By : Rafael Dias
// Last Modified On : 10-01-2014
// ***********************************************************************
// <copyright file="Deducao.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     	Copyright (c) 2014 - 2024 Projeto OpenAC .Net
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
using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class Deducao : GenericClone<Deducao>
{
    #region Constructors

    internal Deducao()
    {
    }

    #endregion Constructors

    #region Propriedades

    /// <summary>
    /// Gets or sets the tipo deducao.
    /// </summary>
    /// <value>The tipo deducao.</value>
    public int TipoDeducao { get; set; }
    
    /// <summary>
    /// Gets or sets Descricao.
    /// </summary>
    /// <value>The Descricao referencia.</value>
    public string? Descricao { get; set; }
    
    /// <summary>
    /// Gets or sets the deducao por.
    /// </summary>
    /// <value>The deducao por.</value>
    public DeducaoPor DeducaoPor { get; set; }

    /// <summary>
    /// Gets or sets the CPFCNPJ referencia.
    /// </summary>
    /// <value>The CPFCNPJ referencia.</value>
    public string CPFCNPJReferencia { get; set; }

    /// <summary>
    /// Gets or sets the numero nf referencia.
    /// </summary>
    /// <value>The numero nf referencia.</value>
    public int? NumeroNFReferencia { get; set; }

    /// <summary>
    /// Gets or sets the valor total referencia.
    /// </summary>
    /// <value>The valor total referencia.</value>
    public decimal ValorTotalReferencia { get; set; }

    public DateTime DataEmissao { get; set; }
    
    public decimal ValorDedutivel { get; set; }

    public decimal ValorUtilizadoDeducao { get; set; }

    /// <summary>
    /// Gets or sets the percentual deduzir.
    /// </summary>
    /// <value>The percentual deduzir.</value>
    public decimal PercentualDeduzir { get; set; }

    /// <summary>
    /// Gets or sets the valor deduzir.
    /// </summary>
    /// <value>The valor deduzir.</value>
    public decimal ValorDeduzir { get; set; }

    public IdeFornecedor DadosFornecedor { get; set; } = new();

    public IdeDocumentoDeducao DocumentoDeducao { get; set; } = new();

    #endregion Propriedades
}