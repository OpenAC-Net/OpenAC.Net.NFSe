using System.Collections.Generic;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Commom;

/// <summary>
/// Classe com lista de parâmetros personalizados dos provedores.
/// Use para saber se o seu provedor tem algum parêmetro personalizado
/// </summary>
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
            },
            {
                NFSeProvider.Equiplano,
                [
                    new ParametroProvider
                    {
                        Nome = "IdEntidade",
                        Descricao = "Identificador do município no provedor Equiplano",
                        Tipo = TipoParametro.Text,
                        Obrigatoria = false,
                        VersoesAfetadas = [VersaoNFSe.ve100]
                    }
                ]
            },
            {
                NFSeProvider.SmarAPD,
                [
                    new ParametroProvider
                    {
                        Nome = "SubVersao",
                        Descricao = "Indica o tipo de implementação com relação às TAGs do XML do provedor SmarAPD",
                        Tipo = TipoParametro.Int,
                        Obrigatoria = false,
                        VersoesAfetadas = [VersaoNFSe.ve204]
                    }
                ]
            }
        };
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Lista de parâmetros personalizados dos provedores.
    /// </summary>
    public static Dictionary<NFSeProvider, List<ParametroProvider>> Parametros;

    #endregion Properties
}