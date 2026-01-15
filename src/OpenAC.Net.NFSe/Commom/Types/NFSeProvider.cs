// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 30-07-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 13-06-2025
// ***********************************************************************
// <copyright file="NFSeProvider.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2024 - 2023 Projeto OpenAC .Net
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

namespace OpenAC.Net.NFSe.Commom.Types;

/// <summary>
/// Enumeração dos provedores de NFSe suportados.
/// </summary>
public enum NFSeProvider : sbyte
{
    /// <summary>
    /// Provedor Abaco.
    /// </summary>
    Abaco = 0,

    /// <summary>
    /// Provedor ABase.
    /// </summary>
    ABase = 39,

    /// <summary>
    /// Provedor Agili.
    /// </summary>
    Agili = 49,

    /// <summary>
    /// Provedor Assessor Público.
    /// </summary>
    AssessorPublico = 33,

    /// <summary>
    /// Provedor Betha.
    /// </summary>
    Betha = 1,

    /// <summary>
    /// Provedor BHISS.
    /// </summary>
    BHISS = 8,

    /// <summary>
    /// Provedor Citta.
    /// </summary>
    Citta = 28,

    /// <summary>
    /// Provedor Conam.
    /// </summary>
    Conam = 21,

    /// <summary>
    /// Provedor Coplan.
    /// </summary>
    Coplan = 3,

    /// <summary>
    /// Provedor DBSeller.
    /// </summary>
    DBSeller = 19,

    /// <summary>
    /// Provedor DSF.
    /// </summary>
    DSF = 38,

    /// <summary>
    /// Provedor Equiplano.
    /// </summary>
    Equiplano = 15,

    /// <summary>
    /// Provedor Fiorilli.
    /// </summary>
    Fiorilli = 16,

    /// <summary>
    /// Provedor FintelISS.
    /// </summary>
    FintelISS = 51,

    /// <summary>
    /// Provedor Fisco.
    /// </summary>
    Fisco = 42,

    /// <summary>
    /// Provedor FissLex.
    /// </summary>
    FissLex = 12,

    /// <summary>
    /// Provedor Ginfes.
    /// </summary>
    Ginfes = 5,

    /// <summary>
    /// Provedor GIAP.
    /// </summary>
    GIAP = 53,

    /// <summary>
    /// Provedor GISS.
    /// </summary>
    GISS = 52,
    
    /// <summary>
    /// ISS govbr.cloud
    /// </summary>
    [Description("ISS govbr.cloud")]
    GovBR = 54,

    /// <summary>
    /// Provedor IISPortoVelho.
    /// </summary>
    [Description("Porto Velho")]
    IISPortoVelho = 32,

    /// <summary>
    /// Provedor IPM.
    /// </summary>
    IPM = 36,

    /// <summary>
    /// Provedor ISSe.
    /// </summary>
    ISSe = 23,

    /// <summary>
    /// Provedor ISS Curitiba.
    /// </summary>
    ISSCuritiba = 26,

    /// <summary>
    /// Provedor ISS DSF.
    /// </summary>
    ISSDSF = 4,

    /// <summary>
    /// Provedor ISS Goiânia.
    /// </summary>
    [Description("Goiania")]
    ISSGoiania = 22,

    /// <summary>
    /// Provedor ISS Integra - Nobe Sistemas.
    /// </summary>
    [Description("ISS Integra - Nobe Sistemas")]
    ISSIntegra = 47,

    /// <summary>
    /// Provedor ISSNet.
    /// </summary>
    ISSNet = 18,

    /// <summary>
    /// Provedor ISSRecife.
    /// </summary>
    ISSRecife = 48,

    /// <summary>
    /// Provedor ISSRio.
    /// </summary>
    [Description("Nota Carioca")]
    ISSRio = 7,

    /// <summary>
    /// Provedor ISSSaoPaulo.
    /// </summary>
    [Description("São Paulo")]
    ISSSaoPaulo = 9,

    /// <summary>
    /// Provedor ISSSJP.
    /// </summary>
    ISSSJP = 43,

    /// <summary>
    /// Provedor ISSVitoria.
    /// </summary>
    [Description("Vitoria")]
    ISSVitoria = 13,

    /// <summary>
    /// Provedor Megasoft.
    /// </summary>
    Megasoft = 50,

    /// <summary>
    /// Provedor MetropolisWeb.
    /// </summary>
    [Description("Metro Web")]
    MetropolisWeb = 40,

    /// <summary>
    /// Provedor Mitra.
    /// </summary>
    Mitra = 34,

    /// <summary>
    /// Provedor NFe Cidades.
    /// </summary>
    [Description("NFe Cidades")]
    NFeCidades = 6,

    /// <summary>
    /// Nenhum provedor selecionado.
    /// </summary>
    Nenhum = -1,

    /// <summary>
    /// Provedor Pronim.
    /// </summary>
    Pronim = 17,

    /// <summary>
    /// Provedor RLZ Informática.
    /// </summary>
    [Description("RLZ Informática")]
    RLZ = 31,

    /// <summary>
    /// Provedor SiapNet.
    /// </summary>
    SiapNet = 35,

    /// <summary>
    /// Provedor Sigep.
    /// </summary>
    Sigep = 45,

    /// <summary>
    /// Provedor SigISS.
    /// </summary>
    SigISS = 20,

    /// <summary>
    /// Provedor SigISS Web.
    /// </summary>
    SigISSWeb = 30,

    /// <summary>
    /// Provedor SimplISS.
    /// </summary>
    SimplISS = 24,

    /// <summary>
    /// Provedor Sintese.
    /// </summary>
    Sintese = 37,

    /// <summary>
    /// Provedor SmarAPD.
    /// </summary>
    SmarAPD = 14,

    /// <summary>
    /// Provedor SpeedGov.
    /// </summary>
    SpeedGov = 25,

    /// <summary>
    /// Provedor SystemPro.
    /// </summary>
    SystemPro = 27,

    /// <summary>
    /// Provedor Thema.
    /// </summary>
    Thema = 41,

    /// <summary>
    /// Provedor Tiplan.
    /// </summary>
    Tiplan = 46,

    /// <summary>
    /// Provedor WebIss.
    /// </summary>
    WebIss = 10,

   /// <summary>
   /// Provedor Campinas - SP.
   /// </summary>
    ISSCampinas = 55
}