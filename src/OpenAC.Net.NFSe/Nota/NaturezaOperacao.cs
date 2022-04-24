// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-19-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 05-19-2016
// ***********************************************************************
// <copyright file="NaturezaOperacao.cs" company="OpenAC .Net">
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

using System;

namespace OpenAC.Net.NFSe.Nota
{
    /// <summary>
    /// Classe que contém as naturezas de operação por provedor.
    /// </summary>
    public static class NaturezaOperacao
    {
        #region InnerTypes

        public sealed class NtABRASF
        {
            #region Constructors

            internal NtABRASF()
            {
                TributacaoNoMunicipio = 1;
                TributacaoForaMunicipio = 2;
                Isencao = 3;
                Imune = 4;
                ExigibilidadeSuspJud = 5;
                ExigibilidadeSuspAdm = 6;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Obtém a Natureza de Operação 1 – Tributação no município.
            /// </summary>
            public int TributacaoNoMunicipio { get; }

            /// <summary>
            /// Obtém a Natureza de Operação 2 - Tributação fora do município.
            /// </summary>
            public int TributacaoForaMunicipio { get; }

            /// <summary>
            /// Obtém a Natureza de Operação 3 - Isenção.
            /// </summary>
            public int Isencao { get; }

            /// <summary>
            /// Obtém a Natureza de Operação 4 - Imune.
            /// </summary>
            public int Imune { get; }

            /// <summary>
            /// Obtém a Natureza de Operação 5 – Exigibilidade suspensa por decisão judicial.
            /// </summary>
            public int ExigibilidadeSuspJud { get; }

            /// <summary>
            /// Obtém a Natureza de Operação 6 – Exigibilidade suspensa por procedimento administrativo.
            /// </summary>
            public int ExigibilidadeSuspAdm { get; }

            #endregion Properties
        }

        public sealed class NtDSF
        {
            #region Constructors

            internal NtDSF()
            {
                SemDeducao = 'A';
                ComDeducaoMateriais = 'B';
                ImuneIsenta = 'C';
                DevolucaoRemessa = 'D';
                Intermediacao = 'J';
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Obtém a Natureza de Operação A – Sem Dedução.
            /// </summary>
            public int SemDeducao { get; }

            /// <summary>
            /// Obtém a Natureza de Operação B – Com Dedução/Materiais.
            /// </summary>
            public int ComDeducaoMateriais { get; }

            /// <summary>
            /// Obtém a Natureza de Operação C – Imune/Isenta de ISSQN.
            /// </summary>
            public int ImuneIsenta { get; }

            /// <summary>
            /// Obtém a Natureza de Operação D – Devolução/Simples Remessa.
            /// </summary>
            public int DevolucaoRemessa { get; }

            /// <summary>
            /// Obtém a Natureza de Operação J – Intermediação.
            /// </summary>
            public int Intermediacao { get; }

            #endregion Properties
        }

        public sealed class NtSigiss
        {
            #region Constructors

            internal NtSigiss()
            {
                TributadaNoPrestador = 1; //"tp";
                TributadaNoTomador = 2; //"tt";
                Isenta = 3; //"is";
                Imune = 4; // "im";
                NãoTributada = 5; // "nt";
            }

            #endregion Constructors

            #region Methods

            /// <summary>
            /// Retorna o valor da natureza de operação
            /// </summary>
            /// <param name="key">Chave</param>
            /// <returns></returns>
            public string GetValue(int key)
            {
                switch (key)
                {
                    case 1:
                        return "tp";

                    case 2:
                        return "tt";

                    case 3:
                        return "is";

                    case 4:
                        return "im";

                    case 5:
                        return "nt";

                    default:
                        throw new Exception("Natureza de operação de NtSigiss não implementada");
                }
            }

            #endregion Methods

            #region Properties

            /// <summary>
            /// Obtém a Natureza de Operação tp – Tributada no Prestador.
            /// </summary>
            public int TributadaNoPrestador { get; }

            /// <summary>
            /// Obtém a Natureza de Operação tt - Tributada no Tomador.
            /// </summary>
            public int TributadaNoTomador { get; }

            /// <summary>
            /// Obtém a Natureza de Operação is – Isenta.
            /// </summary>
            public int Isenta { get; }

            /// <summary>
            /// Obtém a Natureza de Operação im – Imune.
            /// </summary>
            public int Imune { get; }

            /// <summary>
            /// Obtém a Natureza de Operação nt – Não Tributada.
            /// </summary>
            public int NãoTributada { get; }

            #endregion Properties
        }

        #endregion InnerTypes

        #region Fields

        private static NtABRASF abrasf;
        private static NtDSF dsf;
        private static NtSigiss sigiss;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Obtém as Natureza de Operação da ABRASAF v1.
        /// </summary>
        public static NtABRASF ABRASF => abrasf ??= new NtABRASF();

        /// <summary>
        /// Obtém as Natureza de Operação do Ginfes.
        /// </summary>
        public static NtABRASF Ginfes => ABRASF;

        /// <summary>
        /// Obtém as Natureza de Operação do DSF.
        /// </summary>
        public static NtDSF DSF => dsf ??= new NtDSF();

        /// <summary>
        /// Obtém as Natura de Operações do Sigis
        /// </summary>
        public static NtSigiss Sigiss => sigiss ??= new NtSigiss();

        #endregion Properties
    }
}