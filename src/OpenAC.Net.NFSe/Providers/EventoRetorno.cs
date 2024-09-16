// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 06-17-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-17-2016
// ***********************************************************************
// <copyright file="Evento.cs" company="OpenAC .Net">
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

using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

public class EventoRetorno
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="EventoRetorno"/> class.
    /// </summary>
    public EventoRetorno()
    {
        IdentificacaoNfse = new IdeNFSe();
        IdentificacaoRps = new IdeRps();
    }

    #endregion Constructor

    #region Propriedades

    public string Codigo { get; set; } = "";

    public string Descricao { get; set; } = "";

    public string Correcao { get; set; } = "";

    public IdeRps IdentificacaoRps { get; set; }

    public IdeNFSe IdentificacaoNfse { get; set; }

    #endregion Propriedades
}