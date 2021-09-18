using System;
using ACBr.Net.DFe.Core.Common;
using FastReport.Export;
using OpenAC.Net.DFe.Core.Common;

namespace ACBr.Net.NFSe.DANFSe.FastReport
{
    public sealed class DANFSeExportEventArgs : EventArgs
    {
        #region Constructors

        internal DANFSeExportEventArgs()
        {
        }

        #endregion Constructors

        #region Properties

        public FiltroDFeReport Filtro { get; internal set; }

        public ExportBase Export { get; internal set; }

        #endregion Properties
    }
}