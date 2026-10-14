using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Providers;

/// <summary>
/// Provedor de NFSe para o sistema/padrão FintelISS.
/// </summary>
internal sealed class ProviderFintelISS204 : ProviderABRASF204
{
    public ProviderFintelISS204(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "FintelISS";
    }

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new FintelISS204ServiceClient(this, tipo, this.Certificado);
    }

    //protected override bool PrecisaValidarSchema(TipoUrl tipo) => false;
}