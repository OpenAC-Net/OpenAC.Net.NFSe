// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 08-17-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 08-17-2016
// ***********************************************************************
// <copyright file="TipoUrl.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		Copyright (c) 2014 - 2024 Projeto OpenAC .Net
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

namespace OpenAC.Net.NFSe.Commom.Types;

/// <summary>
/// Tipos de operações e endpoints de webservice de NFSe.
/// </summary>
public enum TipoUrl
{
    /// <summary>
    /// URL para envio de lote de RPS (assíncrono).
    /// </summary>
    Enviar,

    /// <summary>
    /// URL para envio de lote ou RPS no modo síncrono.
    /// </summary>
    EnviarSincrono,

    /// <summary>
    /// URL para consulta da situação de processamento do lote de RPS.
    /// </summary>
    ConsultarSituacao,

    /// <summary>
    /// URL para consulta do resultado do processamento do lote de RPS.
    /// </summary>
    ConsultarLoteRps,

    /// <summary>
    /// URL para consulta do próximo número sequencial de RPS.
    /// </summary>
    ConsultarSequencialRps,

    /// <summary>
    /// URL para consulta de NFSe por meio dos dados do RPS emitido.
    /// </summary>
    ConsultarNFSeRps,

    /// <summary>
    /// URL para consulta de NFSe por filtros (número, período, tomador, etc.).
    /// </summary>
    ConsultarNFSe,

    /// <summary>
    /// URL para cancelamento de NFSe individual.
    /// </summary>
    CancelarNFSe,

    /// <summary>
    /// URL para cancelamento de NFSe em lote.
    /// </summary>
    CancelarNFSeLote,

    /// <summary>
    /// URL para substituição de NFSe.
    /// </summary>
    SubstituirNFSe,

    /// <summary>
    /// URL para autenticação/obtenção de token de acesso.
    /// </summary>
    Autenticacao,
}
