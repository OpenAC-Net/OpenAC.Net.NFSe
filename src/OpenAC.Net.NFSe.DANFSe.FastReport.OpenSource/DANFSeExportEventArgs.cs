// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.DANFSe.FastReport.OpenSource
// Author           : Rafael Dias
// Created          : 01-31-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-05-2018
// ***********************************************************************
// <copyright file="DANFSeExportEventArgs.cs" company="OpenAC.Net">
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

using System;
using FastReport.Export;
using OpenAC.Net.DFe.Core.Common;

namespace OpenAC.Net.NFSe.DANFSe.FastReport.OpenSource;

public sealed class DANFSeExportEventArgs : EventArgs
{
    #region Constructors

    internal DANFSeExportEventArgs()
    {
    }

    #endregion Constructors

    #region Properties

    public FiltroDFeReport Filtro { get; internal set; }

    public ExportBase Export { get; internal set; }

    #endregion Properties
}