using OpenAC.Net.Core.Generics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAC.Net.NFSe.Nota
{
    public sealed class Evento : GenericClone<Evento>, INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Constructors

        internal Evento()
        {

        }

        #endregion Constructors

        #region Propriedades

        public string IdentificacaoEvento { get; set; }

        public string DescricaoEvento { get; set; }
        public string InformacoesComplementares { get; set; }

        #endregion Propriedades
    }
}
