// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 16-09-2024
//
// Last Modified By : Rafael Dias
// Last Modified On : 16-09-2024
// ***********************************************************************
// <copyright file="CodigoTOM.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2024 Projeto OpenAC .Net
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

using System.Collections.Generic;
using OpenAC.Net.Core.Extensions;

namespace OpenAC.Net.NFSe.Commom.Types;


/// <summary>
/// Classe para conversão de Codigo IBGE para Codigo TOM
/// Código TOM é um padrão de código das Cidades brasileiras utilizado por sistemas,
/// por exemplo, SIAFI - Sistema Integrado de Administração Financeira do Governo Federal, que é composto por 4 dígitos.
/// </summary>
public static partial class CodigoTOM
{
    #region Fields

    private static readonly Dictionary<int, string> TabelaIBGETOM = [];
    private static readonly Dictionary<string, int> TabelaTOMIBGE = [];

    #endregion Fields

    #region Constructors

    static CodigoTOM()
    {
        AddAC();
        AddAL();
        AddAM();
        AddAP();
        AddBA();
        AddCE();
        AddDF();
        AddES();
        AddGO();
        AddMA();
        AddMG();
        AddMS();
        AddMT();
        AddPA();
        AddPB();
        AddPE();
        AddPI();
        AddPR();
        AddRJ();
        AddRN();
        AddRO();
        AddRR();
        AddRS();
        AddSC();
        AddSE();
        AddSP();
        AddTO();
    }

    #endregion Constructors
    
    #region Methods

    /// <summary>
    /// Convert eo codigo IBGE para o padrão TOM
    /// </summary>
    /// <param name="codIbge"></param>
    /// <returns></returns>
    public static string? FromIBGE(int codIbge) => TabelaIBGETOM.GetValueOrDefault(codIbge);
    
    /// <summary>
    /// Convert eo codigo TOM para o padrão IBGE
    /// </summary>
    /// <param name="codTom"></param>
    /// <returns></returns>
    public static int? ToIBGE(string codTom) => TabelaTOMIBGE.GetValueOrDefault(codTom);

    /// <summary>
    /// Adiciona uma cidade na lista se a mesma não existir.
    /// </summary>
    /// <param name="codigoIBGE"></param>
    /// <param name="codigoTom"></param>
    public static void AddCidade(string codigoIBGE, string codigoTom)
    {
        TabelaIBGETOM.TryAdd(codigoIBGE.ToInt32(), codigoTom);
        TabelaTOMIBGE.TryAdd(codigoTom, codigoIBGE.ToInt32());
    }

    #endregion Methods
}