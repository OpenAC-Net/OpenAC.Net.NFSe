// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 07-30-2017
//
// Last Modified By : Fabio Dias
// Last Modified On : 12-11-2021
// ***********************************************************************
// <copyright file="NFSeProvider.cs" company="OpenAC .Net">
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

using System.ComponentModel;
using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Providers;

public enum NFSeProvider : sbyte
{
    Nenhum = -1,

    Americana = 29,

    Abaco = 0,

    ABase = 39,

    AssessorPublico = 33,

    Betha = 1,

    BHISS = 8,

    Citta = 28,

    Coplan = 3,

    ISSCuritiba = 26,

    DBSeller = 19,

    ISSDSF = 4,

    DSF = 38,

    Equiplano = 15,

    Fiorilli = 16,

    Fisco = 42,

    FissLex = 12,

    Ginfes = 5,

    IPM = 36,

    ISSe = 23,

    ISSNet = 18,

    ISSSJP = 43,

    Mitra = 34,

    [Description("NFe Cidades")]
    NFeCidades = 6,

    [Description("Nota Carioca")]
    ISSRio = 7,

    Pronim = 17,

    [Description("São Paulo")]
    ISSSaoPaulo = 9,

    SmarAPD = 14,

    SiapNet = 35,

    SigISS = 20,

    SimplISS = 24,

    Sintese = 37,

    SpeedGov = 25,

    SystemPro = 27,

    Conam = 21,

    [Description("Goiania")]
    ISSGoiania = 22,

    SigISSWeb = 30,

    [Description("RLZ Informática")]
    RLZ = 31,

    Tiplan = 46,

    [Description("Vitoria")]
    ISSVitoria = 13,

    WebIss = 10,

    [Description("Porto Velho")]
    ISSPortoVelho = 32,

    [Description("Metro Web")]
    MetropolisWeb = 40,

    Thema = 41
}