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

using System.ComponentModel;
using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Providers
{
    public enum NFSeProvider : sbyte
    {
        Nenhum = -1,

        Americana = 29,

        Abaco = 0,

        AssessorPublico = 33,

        Betha = 1,

        [Description("Betha v2")]
        Betha2 = 2,

        BHISS = 8,

        CITTA = 28,

        Coplan = 3,

        Curitiba = 26,

        DBSeller = 19,

        DSF = 4,

        DSFSJC = 38,

        Equiplano = 15,

        Fiorilli = 16,

        FissLex = 12,

        Ginfes = 5,

        IPM = 36,

        ISSe = 23,

        ISSNet = 18,

        Mitra = 34,

        [Description("NFe Cidades")]
        NFeCidades = 6,

        [Description("Nota Carioca")]
        NotaCarioca = 7,

        [Description("Pronim v2")]
        Pronim2 = 17,

        [Description("São Paulo")]
        SaoPaulo = 9,

        [Description("SmarAPD ABRASF")]
        SmarAPDABRASF = 14,

        SIAPNet = 35,

        Sigiss = 20,

        SimplISS = 24,

        Sintese = 37,

        SpeedGov = 25,

        SystemPro = 27,

        [Description("CONAM")]
        Conam = 21,

        [Description("Goiania")]
        Goiania = 22,

        SigissWeb = 30,

        [Description("RLZ Informática")]
        RLZ = 31,

        [Description("Vitoria")]
        Vitoria = 13,

        WebIss = 10,

        [Description("WebIss v2")]
        WebIss2 = 11,

        [Description("Porto Velho")]
        PVH = 32,

        [Description("Metro Web")]
        MetroWebAbrasf = 39
    }
}