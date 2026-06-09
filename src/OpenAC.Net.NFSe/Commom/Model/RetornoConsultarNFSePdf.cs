// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : danilobreda
// Created          : 06-09-2026
//
// Last Modified By : danilobreda
// Last Modified On : 06-09-2026
// ***********************************************************************
// <copyright file="RetornoConsultarNFSePdf.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2026 Projeto OpenAC .Net
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

namespace OpenAC.Net.NFSe.Commom.Model;

/// <summary>
/// Retorno da consulta da versão digital (PDF) de uma NFS-e/RPS.
/// </summary>
public sealed class RetornoConsultarNFSePdf : RetornoWebservice
{
    /// <summary>
    /// Número do RPS consultado.
    /// </summary>
    public int NumeroRps { get; internal set; }

    /// <summary>
    /// Conteúdo binário do PDF retornado pelo provedor (nulo quando não encontrado).
    /// </summary>
    public byte[]? Pdf { get; internal set; }

    /// <summary>
    /// Nome do arquivo informado pelo provedor (cabeçalho Content-Disposition), quando disponível.
    /// </summary>
    public string NomeArquivo { get; internal set; } = "";
}
