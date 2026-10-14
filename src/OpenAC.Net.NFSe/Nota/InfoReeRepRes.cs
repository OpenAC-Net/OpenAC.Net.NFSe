using System.Collections.Generic;
using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;/// <summary>
/// Informações de renegociação, repasse ou ressarcimento de tributos IBS e CBS.
/// </summary>


public sealed class InfoReeRepRes : GenericClone<InfoReeRepRes>
{
    public InfoReeRepRes()
    {
        Documentos = new List<IBSCBSDocumento>();
    }

    /// <summary>
    /// Grupo relativo aos documentos referenciados nos casos de reembolso, repasse e ressarcimento
    /// considerados na base de calculo do ISSQN, do IBS e da CBS.
    /// </summary>
    public ICollection<IBSCBSDocumento> Documentos { get; }
}
