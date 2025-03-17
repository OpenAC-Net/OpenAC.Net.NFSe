using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderFintel204 : ProviderABRASF204
{
    public ProviderFintel204(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "Fintel";
    }

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new Fintel204ServiceClient(this, tipo, this.Certificado);
    }

    //protected override bool PrecisaValidarSchema(TipoUrl tipo) => false;
}