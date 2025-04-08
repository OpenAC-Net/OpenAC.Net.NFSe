// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-26-2016
//
// Last Modified By : jairgmjunior1 
// Last Modified On : 09-12-2024
// ***********************************************************************
// <copyright file="TipoDeducao.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	      Copyright (c) 2014 - 2024 Projeto OpenAC .Net
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

namespace OpenAC.Net.NFSe.Nota;

public static class TipoDeducao
{
    #region ABRASF

    private static DeducaoABRASF? abrasf;
    public static DeducaoABRASF ABRASF => abrasf ??= new();

    public sealed class DeducaoABRASF
    {
        internal DeducaoABRASF()
        {
            Materiais = 1;
            Subempreitada = 2;
            Servicos = 3;
            ProducaoExterna = 4;
            AlimentacaoBebidasFrigobar = 5;
            ReembolsoDspesas = 6;
            RepasseConsorciado = 7;
            RepassPlanoSaude = 8;
            OutrasDeducoes = 99;
        }

        public int Materiais { get; }
        public int Subempreitada { get; }
        public int Servicos { get; }
        public int ProducaoExterna { get; }
        public int AlimentacaoBebidasFrigobar { get; }
        public int ReembolsoDspesas { get; }
        public int RepasseConsorciado { get; }
        public int RepassPlanoSaude { get; }
        public int OutrasDeducoes { get; }
    }

    #endregion ABRASF

    #region DSF

    private static DeducaoDSF? dsf;
    public static DeducaoDSF DSF => dsf ??= new DeducaoDSF();

    public sealed class DeducaoDSF
    {
        internal DeducaoDSF()
        {
            Nenhum = 1;
            Materiais = 2;
            SubEmpreitada = 3;
            Mercadorias = 4;
            VeiculacaoeDivulgacao = 5;
            MapadeConstCivil = 6;
            Servicos = 7;
        }

        public int Nenhum { get; } = 1;
        public int Materiais { get; } = 2;
        public int SubEmpreitada { get; }
        public int Mercadorias { get; }
        public int VeiculacaoeDivulgacao { get; }
        public int MapadeConstCivil { get; }
        public int Servicos { get; }

        private const string _Nenhum = "";
        private const string _Materiais = "Despesas com Materiais";
        private const string _SubEmpreitada = "Despesas com Subempreitada";
        private const string _Mercadorias = "Despesas com Mercadorias";
        private const string _VeiculacaoeDivulgacao = "Servicos de Veiculacao e Divulgacao";
        private const string _MapadeConstCivil = "Mapa de Const. Civil";
        private const string _Servicos = "Servicos";

        public string GetDescription(int key)
        {
            return key switch
            {
                1 => _Nenhum,
                2 => _Materiais,
                3 => _SubEmpreitada,
                4 => _Mercadorias,
                5 => _VeiculacaoeDivulgacao,
                6 => _MapadeConstCivil,
                7 => _Servicos,
                _ => throw new Exception("DSF - Tipo de Dedução não implementada.")
            };
        }

        public int GetValue(string key)
        {
            key = key.Trim();

            return key switch
            {
                _Nenhum => 1,
                _Materiais => 2,
                _SubEmpreitada => 3,
                _Mercadorias => 4,
                _VeiculacaoeDivulgacao => 5,
                _MapadeConstCivil => 6,
                _Servicos => 7,
                _ => throw new Exception("DSF - Descrição de Dedução não implementada.")
            };
        }
    }

    #endregion DSF
}