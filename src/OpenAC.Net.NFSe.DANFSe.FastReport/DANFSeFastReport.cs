using System;
using System.IO;
using FastReport;
using FastReport.Export.Html;
using FastReport.Export.Pdf;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.DANFSe.FastReport
{

    public sealed class DANFSeFastReport : OpenDANFSeBase
    {
        #region Fields

        private Report internalReport;
        private bool isDesign;

        #endregion Fields

        #region Events

        public event EventHandler<DANFSeEventArgs> OnGetReport;

        public event EventHandler<DANFSeExportEventArgs> OnExport;

        #endregion Events

        #region Constructors

        public DANFSeFastReport(ConfigNFSe config) : base(config)
        {
        }

        #endregion


        #region Methods

        public void ShowDesign(NotaServico[] notas)
        {
            isDesign = true;

            try
            {
                Imprimir(notas, null);
            }
            finally
            {
                isDesign = false;
            }
        }

        public override void Imprimir(NotaServico[] notas)
        {
            Imprimir(notas, null);
        }

        /// <inheritdoc />
        public override void ImprimirPDF(NotaServico[] notas)
        {
            var oldFiltro = Filtro;

            try
            {
                Filtro = FiltroDFeReport.PDF;
                Imprimir(notas, null);
            }
            finally
            {
                Filtro = oldFiltro;
            }
        }

        /// <inheritdoc />
        public override void ImprimirPDF(NotaServico[] notas, Stream stream)
        {
            var oldFiltro = Filtro;

            try
            {
                Filtro = FiltroDFeReport.PDF;
                Imprimir(notas, stream);
            }
            finally
            {
                Filtro = oldFiltro;
            }
        }

        /// <inheritdoc />
        public override void ImprimirHTML(NotaServico[] notas)
        {
            var oldFiltro = Filtro;

            try
            {
                Filtro = FiltroDFeReport.HTML;
                Imprimir(notas, null);
            }
            finally
            {
                Filtro = oldFiltro;
            }
        }

        /// <inheritdoc />
        public override void ImprimirHTML(NotaServico[] notas,Stream stream)
        {
            var oldFiltro = Filtro;

            try
            {
                Filtro = FiltroDFeReport.HTML;
                Imprimir(notas, stream);
            }
            finally
            {
                Filtro = oldFiltro;
            }
        }

        private void Imprimir(NotaServico[] notas, Stream stream)
        {
            using (internalReport = new Report())
            {
                PrepararImpressao();

                internalReport.RegisterData(notas, "NotaServico");
                internalReport.Prepare();

                if (isDesign)
                {
                    internalReport.Design();
                }
                else
                {
                    switch (Filtro)
                    {
                        case FiltroDFeReport.Nenhum:
                            if (MostrarPreview)
                                internalReport.Show();
                            else
                                internalReport.Print();
                            break;

                        case FiltroDFeReport.PDF:
                            var evtPdf = new DANFSeExportEventArgs();
                            evtPdf.Export = new PDFExport
                            {
                                PdfCompliance = PDFExport.PdfStandard.PdfA_1a,
                                ShowProgress = MostrarSetup,
                                OpenAfterExport = MostrarPreview
                            };

                            OnExport.Raise(this, evtPdf);
                            if (stream.IsNull())
                                internalReport.Export(evtPdf.Export, NomeArquivo);
                            else
                                internalReport.Export(evtPdf.Export, stream);
                            break;

                        case FiltroDFeReport.HTML:
                            var evtHtml = new DANFSeExportEventArgs();
                            evtHtml.Export = new HTMLExport()
                            {
                                Format = HTMLExportFormat.HTML,
                                EmbedPictures = true,
                                Preview = MostrarPreview,
                                ShowProgress = MostrarSetup
                            };

                            OnExport.Raise(this, evtHtml);
                            if (stream.IsNull())
                                internalReport.Export(evtHtml.Export, NomeArquivo);
                            else
                                internalReport.Export(evtHtml.Export, stream);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                internalReport.Dispose();
            }

            internalReport = null;
        }

        private void PrepararImpressao()
        {
            var e = new DANFSeEventArgs(Layout);
            OnGetReport.Raise(this, e);
            if (e.FilePath.IsEmpty() || !File.Exists(e.FilePath))
            {
                MemoryStream ms;

                //ToDo: Adicionar os layouts de acordo com o provedor
                switch (Layout)
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

            internalReport.SetParameterValue("Logo", Logo.ToByteArray());
            internalReport.SetParameterValue("LogoPrefeitura", LogoPrefeitura.ToByteArray());
            internalReport.SetParameterValue("MunicipioPrestador", Configuracoes.WebServices.Municipio);
            internalReport.SetParameterValue("Ambiente", (int)Configuracoes.WebServices.Ambiente);
            internalReport.SetParameterValue("SoftwareHouse", SoftwareHouse);
            internalReport.SetParameterValue("Site", Site);

            internalReport.PrintSettings.Copies = NumeroCopias;
            internalReport.PrintSettings.Printer = Impressora;
            internalReport.PrintSettings.ShowDialog = MostrarSetup;
        }

        #endregion Methods
    }
}