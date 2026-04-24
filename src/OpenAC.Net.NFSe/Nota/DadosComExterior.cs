using System.ComponentModel;
using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class DadosComExterior : GenericClone<DadosComExterior>, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    internal DadosComExterior()
    {
    }

    public string ModoPrestacao { get; set; }

    public string VinculoPrestacao { get; set; }

    public string TipoMoeda { get; set; }

    public decimal ValorServicoMoeda { get; set; }

    public string MecanismoApoioPrestador { get; set; }

    public string MecanismoApoioTomador { get; set; }

    public string MovimentoTemporarioBens { get; set; }

    public string EnviarMdic { get; set; }
}
