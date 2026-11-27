using System.Runtime.CompilerServices;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Web.Common;

namespace OpenAC.Net.NFSe.Web.Accessor;

internal sealed class NFSeTenantContextAccessor : INFSeTenantContextAccessor
{
    private readonly ConditionalWeakTable<ConfigNFSe, TenantIdentity> tenants = new();
    private readonly string poolScope = $"OpenAC.Net.NFSe.Web/{Guid.NewGuid():N}";

    internal void Bind(ConfigNFSe configuration, string tenantId) =>
        tenants.AddOrUpdate(configuration, new TenantIdentity(tenantId));

    internal string GetPoolPartition(string tenantId) => $"{poolScope}/{tenantId}";

    public string GetTenant(ConfigNFSe configuration) =>
        tenants.TryGetValue(configuration, out var tenant) ? tenant.Value : NFSeTenant.Padrao;

    private sealed record TenantIdentity(string Value);
}
