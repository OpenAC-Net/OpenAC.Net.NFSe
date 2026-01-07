using System.Collections.Generic;
using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class InfoReeRepRes : GenericClone<InfoReeRepRes>
{
    public InfoReeRepRes()
    {
        Documentos = new List<IBSCBSDocumento>();
    }

    public ICollection<IBSCBSDocumento> Documentos { get; }
}