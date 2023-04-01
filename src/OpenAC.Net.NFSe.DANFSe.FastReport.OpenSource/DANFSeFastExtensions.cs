// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.DANFSe.FastReport.OpenSource
// Author           : Rafael Dias
// Created          : 21-10-2021
//
// Last Modified By : Rafael Dias
// Last Modified On : 21-10-2021
// ***********************************************************************
// <copyright file="DANFSeFastExtensions.cs" company="OpenAC.Net">
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
using System.IO;

namespace OpenAC.Net.NFSe.DANFSe.FastReport.OpenSource;

public static class DANFSeFastExtensions
{
    public static void Imprimir(this OpenNFSe nfse, Action<DANFSeFastOpenOptions> options = null)
    {
        var danfse = new DANFSeFastReportOpenSource(nfse.Configuracoes);
        options?.Invoke(danfse.Configuracoes);
        danfse.Imprimir(nfse.NotasServico.ToArray());
    }

    public static void ImprimirPDF(this OpenNFSe nfse, Action<DANFSeFastOpenOptions> options = null)
    {
        var danfse = new DANFSeFastReportOpenSource(nfse.Configuracoes);
        options?.Invoke(danfse.Configuracoes);
        danfse.ImprimirPDF(nfse.NotasServico.ToArray());
    }

    public static void ImprimirPDF(this OpenNFSe nfse, Stream aStream, Action<DANFSeFastOpenOptions> options = null)
    {
        var danfse = new DANFSeFastReportOpenSource(nfse.Configuracoes);
        options?.Invoke(danfse.Configuracoes);
        danfse.ImprimirPDF(nfse.NotasServico.ToArray(), aStream);
    }

    public static void ImprimirHTML(this OpenNFSe nfse, Action<DANFSeFastOpenOptions> options = null)
    {
        var danfse = new DANFSeFastReportOpenSource(nfse.Configuracoes);
        options?.Invoke(danfse.Configuracoes);
        danfse.ImprimirHTML(nfse.NotasServico.ToArray());
    }

    public static void ImprimirHTML(this OpenNFSe nfse, Stream aStream, Action<DANFSeFastOpenOptions> options = null)
    {
        var danfse = new DANFSeFastReportOpenSource(nfse.Configuracoes);
        options?.Invoke(danfse.Configuracoes);
        danfse.ImprimirHTML(nfse.NotasServico.ToArray(), aStream);
    }
}