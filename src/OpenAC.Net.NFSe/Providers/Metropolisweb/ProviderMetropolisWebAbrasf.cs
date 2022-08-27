// ===================================================================
//  Empresa: DSBR - Empresa de Desenvolvimento de Sistemas 
//  Site: www.dsbrbrasil.com.br
//  Autores:  Valnei Filho v_marinpietri@yahoo.com.br
//  Data Criação: 24/07/2022
//  Todos os direitos reservados
// ===================================================================


#region

using OpenAC.Net.NFSe.Configuracao;

#endregion

namespace OpenAC.Net.NFSe.Providers.Metropolisweb
{
    internal sealed class ProviderMetropolisWebAbrasf : ProviderABRASF
    {
        #region Construtor

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProviderABRASF" /> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="municipio">The municipio.</param>
        public ProviderMetropolisWebAbrasf(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "MetroWebAbrasf";
        }

        #endregion

        protected override bool PrecisaValidarSchema(TipoUrl tipo)
        {
            return false;
        }

        /// <summary>
        ///     Retorna o cliente de comunicação com o webservice.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        protected override IServiceClient GetClient(TipoUrl tipo)
        {
            return new MetropolisWebAbrasfClient(this, tipo, Certificado);
        }
    }
}