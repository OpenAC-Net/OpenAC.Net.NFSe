// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.DANFSe.FastReport
// Author           : Rafael Dias
// Created          : 01-31-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-05-2018
// ***********************************************************************
// <copyright file="DANFSeFastReport.cs" company="OpenAC.Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2023 Projeto OpenAC .Net
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
using FastReport;
using FastReport.Export.Html;
using FastReport.Export.Pdf;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.DANFSe.FastReport;

public sealed class DANFSeFastReport : OpenDANFSeBase<DANFSeFastOptions, FiltroDANFSe>
{
    #region Fields

    private Report internalReport;

    #endregion Fields

    #region Events

    public event EventHandler<DANFSeEventArgs> OnGetReport;

    public event EventHandler<DANFSeExportEventArgs> OnExport;

    #endregion Events

    #region Constructors

    public DANFSeFastReport(ConfigNFSe config)
    {
        Configuracoes = new DANFSeFastOptions(config);
    }

    #endregion Constructors

    #region Methods

    public override void Imprimir(NotaServico[] notas)
    {
        Imprimir(notas, null);
    }

    /// <inheritdoc />
    public override void ImprimirPDF(NotaServico[] notas)
    {
        var oldFiltro = Configuracoes.Filtro;

        try
        {
            Configuracoes.Filtro = FiltroDANFSe.PDF;
            Imprimir(notas, null);
        }
        finally
        {
            Configuracoes.Filtro = oldFiltro;
        }
    }

    /// <inheritdoc />
    public override void ImprimirPDF(NotaServico[] notas, Stream stream)
    {
        var oldFiltro = Configuracoes.Filtro;

        try
        {
            Configuracoes.Filtro = FiltroDANFSe.PDF;
            Imprimir(notas, stream);
        }
        finally
        {
            Configuracoes.Filtro = oldFiltro;
        }
    }

    /// <inheritdoc />
    public override void ImprimirHTML(NotaServico[] notas)
    {
        var oldFiltro = Configuracoes.Filtro;

        try
        {
            Configuracoes.Filtro = FiltroDANFSe.HTML;
            Imprimir(notas, null);
        }
        finally
        {
            Configuracoes.Filtro = oldFiltro;
        }
    }

    /// <inheritdoc />
    public override void ImprimirHTML(NotaServico[] notas, Stream stream)
    {
        var oldFiltro = Configuracoes.Filtro;

        try
        {
            Configuracoes.Filtro = FiltroDANFSe.HTML;
            Imprimir(notas, stream);
        }
        finally
        {
            Configuracoes.Filtro = oldFiltro;
        }
    }

    private void Imprimir(NotaServico[] notas, Stream stream)
    {
        using (internalReport = new Report())
        {
            PrepararImpressao();

            internalReport.RegisterData(notas, "NotaServico");
            internalReport.Prepare();

            switch (Configuracoes.Filtro)
            {
                case FiltroDANFSe.Nenhum:
                    if (Configuracoes.MostrarPreview)
                        internalReport.Show();
                    else
                        internalReport.Print();
                    break;

                case FiltroDANFSe.PDF:
                    var evtPdf = new DANFSeExportEventArgs
                    {
                        Export = new PDFExport
                        {
                            PdfCompliance = PDFExport.PdfStandard.PdfA_1a,
                            ShowProgress = Configuracoes.MostrarSetup,
                            OpenAfterExport = Configuracoes.MostrarPreview
                        }
                    };

                    OnExport.Raise(this, evtPdf);
                    if (stream.IsNull())
                        internalReport.Export(evtPdf.Export, Configuracoes.NomeArquivo);
                    else
                        internalReport.Export(evtPdf.Export, stream);
                    break;

                case FiltroDANFSe.HTML:
                    var evtHtml = new DANFSeExportEventArgs
                    {
                        Export = new HTMLExport()
                        {
                            Format = HTMLExportFormat.HTML,
                            EmbedPictures = true,
                            Preview = Configuracoes.MostrarPreview,
                            ShowProgress = Configuracoes.MostrarSetup
                        }
                    };

                    OnExport.Raise(this, evtHtml);
                    if (stream.IsNull())
                        internalReport.Export(evtHtml.Export, Configuracoes.NomeArquivo);
                    else
                        internalReport.Export(evtHtml.Export, stream);
                    break;

                case FiltroDANFSe.Design:
                    internalReport.Design();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            internalReport.Dispose();
        }

        internalReport = null;
    }

    private void PrepararImpressao()
    {
        var e = new DANFSeEventArgs(Configuracoes.Layout);
        OnGetReport.Raise(this, e);
        if (e.FilePath.IsEmpty() || !File.Exists(e.FilePath))
        {
            MemoryStream ms;

            //ToDo: Adicionar os layouts de acordo com o provedor
            switch (Configuracoes.Layout)
            {
                case LayoutImpressao.ABRASF2:
                    ms = new MemoryStream(Properties.Resources.DANFSe);
                    break;

                case LayoutImpressao.DSF:
                    ms = new MemoryStream(Properties.Resources.DANFSe);
                    break;

                case LayoutImpressao.Ginfes:
                    ms = new MemoryStream(Properties.Resources.DANFSe);
                    break;

                case LayoutImpressao.ABRASF:
                    ms = new MemoryStream(Properties.Resources.DANFSe);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            internalReport.Load(ms);
        }
        else
        {
            internalReport.Load(e.FilePath);
        }

        internalReport.SetParameterValue("Logo", Configuracoes.Logo.ToByteArray());
        internalReport.SetParameterValue("LogoPrefeitura", Configuracoes.LogoPrefeitura.ToByteArray());
        internalReport.SetParameterValue("MunicipioPrestador", Configuracoes.NFSe.WebServices.Municipio);
        internalReport.SetParameterValue("Ambiente", (int)Configuracoes.NFSe.WebServices.Ambiente);
        internalReport.SetParameterValue("SoftwareHouse", Configuracoes.SoftwareHouse);
        internalReport.SetParameterValue("Site", Configuracoes.Site);

        internalReport.PrintSettings.Copies = Configuracoes.NumeroCopias;
        internalReport.PrintSettings.Printer = Configuracoes.Impressora;
        internalReport.PrintSettings.ShowDialog = Configuracoes.MostrarSetup;
    }

    #endregion Methods
}