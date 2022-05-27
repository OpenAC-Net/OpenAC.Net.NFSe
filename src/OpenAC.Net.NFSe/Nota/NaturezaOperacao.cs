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
    /// Classe que cont�m as naturezas de opera��o por provedor.
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
            /// Obt�m a Natureza de Opera��o 1 � Tributa��o no munic�pio.
            /// </summary>
            public int TributacaoNoMunicipio { get; }

            /// <summary>
            /// Obt�m a Natureza de Opera��o 2 - Tributa��o fora do munic�pio.
            /// </summary>
            public int TributacaoForaMunicipio { get; }

            /// <summary>
            /// Obt�m a Natureza de Opera��o 3 - Isen��o.
            /// </summary>
            public int Isencao { get; }

            /// <summary>
            /// Obt�m a Natureza de Opera��o 4 - Imune.
            /// </summary>
            public int Imune { get; }

            /// <summary>
            /// Obt�m a Natureza de Opera��o 5 � Exigibilidade suspensa por decis�o judicial.
            /// </summary>
            public int ExigibilidadeSuspJud { get; }

            /// <summary>
            /// Obt�m a Natureza de Opera��o 6 � Exigibilidade suspensa por procedimento administrativo.
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
            /// Obt�m a Natureza de Opera��o A � Sem Dedu��o.
            /// </summary>
            public int SemDeducao { get; }

            /// <summary>
            /// Obt�m a Natureza de Opera��o B � Com Dedu��o/Materiais.
            /// </summary>
            public int ComDeducaoMateriais { get; }

            /// <summary>
            /// Obt�m a Natureza de Opera��o C � Imune/Isenta de ISSQN.
            /// </summary>
            public int ImuneIsenta { get; }

            /// <summary>
            /// Obt�m a Natureza de Opera��o D � Devolu��o/Simples Remessa.
            /// </summary>
            public int DevolucaoRemessa { get; }

            /// <summary>
            /// Obt�m a Natureza de Opera��o J � Intermedia��o.
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
                N�oTributada = 5; // "nt";
            }

            #endregion Constructors

            #region Methods

            /// <summary>
            /// Retorna o valor da natureza de opera��o
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
                        throw new Exception("Natureza de opera��o de NtSigiss n�o implementada");
                }
            }

            #endregion Methods

            #region Properties

            /// <summary>
            /// Obt�m a Natureza de Opera��o tp � Tributada no Prestador.
            /// </summary>
            public int TributadaNoPrestador { get; }

            /// <summary>
            /// Obt�m a Natureza de Opera��o tt - Tributada no Tomador.
            /// </summary>
            public int TributadaNoTomador { get; }

            /// <summary>
            /// Obt�m a Natureza de Opera��o is � Isenta.
            /// </summary>
            public int Isenta { get; }

            /// <summary>
            /// Obt�m a Natureza de Opera��o im � Imune.
            /// </summary>
            public int Imune { get; }

            /// <summary>
            /// Obt�m a Natureza de Opera��o nt � N�o Tributada.
            /// </summary>
            public int N�oTributada { get; }

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
        /// Obt�m as Natureza de Opera��o da ABRASAF v1.
        /// </summary>
        public static NtABRASF ABRASF => abrasf ??= new NtABRASF();

        /// <summary>
        /// Obt�m as Natureza de Opera��o do Ginfes.
        /// </summary>
        public static NtABRASF Ginfes => ABRASF;

        /// <summary>
        /// Obt�m as Natureza de Opera��o do DSF.
        /// </summary>
        public static NtDSF DSF => dsf ??= new NtDSF();

        /// <summary>
        /// Obt�m as Natura de Opera��es do Sigis
        /// </summary>
        public static NtSigiss Sigiss => sigiss ??= new NtSigiss();

        #endregion Properties
    }
}