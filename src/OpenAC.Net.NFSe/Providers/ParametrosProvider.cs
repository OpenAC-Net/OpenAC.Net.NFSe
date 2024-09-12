using System.Collections.Generic;

namespace OpenAC.Net.NFSe.Providers;

public static class ParametrosProvider
{
    #region Constructors

    static ParametrosProvider()
    {
        Parametros = new Dictionary<NFSeProvider, List<ParametroProvider>>
        {
            {
                NFSeProvider.Agili,
                [
                    new ParametroProvider
                    {
                        Nome = "UnidadeGestora",
                        Descricao = "Informe o CNPJ da Prefeitura",
                        Tipo = TipoParametro.Text,
                        Obrigatoria = false,
                        VersoesAfetadas = [VersaoNFSe.ve100]
                    }
                ]
            },
            {
                NFSeProvider.IPM,
                [
                    new ParametroProvider
                    {
                        Nome = "NaoGerarGrupoRps",
                        Descricao = "Define se deve ou não gerar a tag RPS",
                        Tipo = TipoParametro.Boolean,
                        Obrigatoria = false,
                        VersoesAfetadas = [VersaoNFSe.ve100, VersaoNFSe.ve101]
                    }
                ]
            }
        };
    }

    #endregion Constructors

    #region Properties

    public static Dictionary<NFSeProvider, List<ParametroProvider>> Parametros;

    #endregion Properties
}