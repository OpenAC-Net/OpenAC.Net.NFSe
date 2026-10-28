using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Web.Common;

namespace OpenAC.Net.NFSe.Web.Accessor;

/// <summary>Consulta a identidade associada a uma configuração sem expô-la nos nomes de arquivo.</summary>
public interface INFSeTenantContextAccessor
{
    /// <summary>Obtém a identidade da empresa associada à configuração.</summary>
    /// <param name="configuration">Configuração cuja empresa associada será consultada.</param>
    /// <returns>Identificador da empresa ou <see cref="NFSeTenant.Padrao"/> quando não houver associação explícita.</returns>
    string GetTenant(ConfigNFSe configuration);
}
