// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 06-17-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-17-2016
// ***********************************************************************
// <copyright file="RetornoWebservice.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2021 Projeto OpenAC .Net
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
using System.Collections.Generic;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers
{
    public abstract class RetornoWebservice
    {
        #region Propriedades

        /// <summary>
        /// Informa se a comunicação ocorreu com sucesso ou não.
        /// </summary>
        /// <value><c>true</c> se não teve erro, senão <c>false</c>.</value>
        public bool Sucesso { get; internal set; }

        public List<Evento> Alertas { get; } = new List<Evento>();

        public List<Evento> Erros { get; } = new List<Evento>();

        public string XmlEnvio { get; internal set; } = "";

        public string XmlRetorno { get; internal set; } = "";

        public string EnvelopeEnvio { get; internal set; } = "";

        public string EnvelopeRetorno { get; internal set; } = "";

        #endregion Propriedades
    }

    public sealed class RetornoEnviar : RetornoWebservice
    {
        public int Lote { get; internal set; }

        public DateTime Data { get; internal set; }

        public string Protocolo { get; internal set; }

        public bool Sincrono { get; internal set; }
    }

    public sealed class RetornoConsultarSituacao : RetornoWebservice
    {
        public int Lote { get; internal set; }

        public string Protocolo { get; internal set; }

        public string Situacao { get; internal set; }
    }

    public sealed class RetornoConsultarLoteRps : RetornoWebservice
    {
        public int Lote { get; internal set; }

        public string Protocolo { get; internal set; }

        public string Situacao { get; internal set; }

        public NotaServico[] Notas { get; internal set; }
    }

    public sealed class RetornoConsultarNFSeRps : RetornoWebservice
    {
        public int NumeroRps { get; internal set; }

        public string Serie { get; internal set; }

        public TipoRps Tipo { get; internal set; }

        public NotaServico Nota { get; internal set; }

        public int AnoCompetencia { get; internal set; }

        public int MesCompetencia { get; internal set; }
    }

    public sealed class RetornoConsultarNFSe : RetornoWebservice
    {
        public DateTime? Inicio { get; internal set; }

        public DateTime? Fim { get; internal set; }

        public int NumeroNFse { get; internal set; }

        public string SerieNFse { get; internal set; }

        public int Pagina { get; internal set; }

        public int ProximaPagina { get; internal set; }

        public string CPFCNPJTomador { get; internal set; }

        public string IMTomador { get; internal set; }

        public string NomeIntermediario { get; internal set; }

        public string CPFCNPJIntermediario { get; internal set; }

        public string IMIntermediario { get; internal set; }

        public NotaServico[] Notas { get; internal set; }
    }

    public sealed class RetornoConsultarSequencialRps : RetornoWebservice
    {
        public string Serie { get; internal set; }

        public int UltimoNumero { get; internal set; }
    }

    public sealed class RetornoCancelar : RetornoWebservice
    {
        public DateTime Data { get; internal set; }

        public string NumeroNFSe { get; internal set; }

        public string SerieNFSe { get; internal set; }

        public decimal ValorNFSe { get; internal set; }

        public string CodigoCancelamento { get; internal set; }

        public string Motivo { get; internal set; }
    }

    public sealed class RetornoCancelarNFSeLote : RetornoWebservice
    {
        public int Lote { get; internal set; }
    }

    public sealed class RetornoSubstituirNFSe : RetornoWebservice
    {
        public string CodigoCancelamento { get; internal set; }

        public string NumeroNFSe { get; internal set; }

        public string Motivo { get; internal set; }

        public NotaServico Nota { get; internal set; }
    }
}