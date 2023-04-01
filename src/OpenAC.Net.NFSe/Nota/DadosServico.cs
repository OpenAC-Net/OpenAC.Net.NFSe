// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-26-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-07-2016
// ***********************************************************************
// <copyright file="DadosServico.cs" company="OpenAC .Net">
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
using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class DadosServico : GenericClone<DadosServico>, INotifyPropertyChanged
{
    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events

    #region Constructors

    internal DadosServico()
    {
        Valores = new ValoresServico();
        ItemListaServico = string.Empty;
        CodigoCnae = string.Empty;
        CodigoTributacaoMunicipio = string.Empty;
        Discriminacao = string.Empty;
        NumeroProcesso = string.Empty;
        ExigibilidadeIss = ExigibilidadeIss.Exigivel;
        ItensServico = new ServicosCollection();
        Deducoes = new DeducoesCollection();
    }

    #endregion Constructors

    #region Propriedades

    public ValoresServico Valores { get; }

    public string ItemListaServico { get; set; }

    public string CodigoCnae { get; set; }

    public string CodigoTributacaoMunicipio { get; set; }

    public string CodigoNbs { get; set; }

    public string Discriminacao { get; set; }

    public int CodigoMunicipio { get; set; }

    public string Municipio { get; set; }

    public int CodigoPais { get; set; }

    public ExigibilidadeIss ExigibilidadeIss { get; set; }

    public string IdentifNaoExigibilidade { get; set; }

    public int MunicipioIncidencia { get; set; }

    public string UfIncidencia { get; set; }

    public string NumeroProcesso { get; set; }

    public ServicosCollection ItensServico { get; }

    public ResponsavelRetencao? ResponsavelRetencao { get; set; }

    public string Descricao { get; set; }

    public DeducoesCollection Deducoes { get; }

    #endregion Propriedades
}