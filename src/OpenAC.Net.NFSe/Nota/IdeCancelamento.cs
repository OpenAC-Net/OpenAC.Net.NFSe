// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 06-05-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-05-2016
// ***********************************************************************
// <copyright file="ConfirmacaoCancelamento.cs" company="OpenAC .Net">
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

using System;
using System.ComponentModel;
using OpenAC.Net.Core.Generics;
using OpenAC.Net.DFe.Core.Document;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IdeCancelamento : GenericClone<IdeCancelamento>, INotifyPropertyChanged
{
    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events

    #region Constructors

    internal IdeCancelamento()
    {
        Id = 0;
        Pedido = new PedidoCancelamento();
        Signature = new DFeSignature();
        DataHora = DateTime.Now;
    }

    #endregion Constructors

    #region Propriedades

    public int Id { get; set; }

    public PedidoCancelamento Pedido { get; }

    public string MotivoCancelamento { get; set; }

    public DateTime DataHora { get; set; }

    public DFeSignature Signature { get; internal set; }

    #endregion Propriedades
}