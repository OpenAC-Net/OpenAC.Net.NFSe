using System;
using FastReport.Export;
using OpenAC.Net.DFe.Core.Common;

namespace OpenAC.Net.NFSe.DANFSe.FastReport
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