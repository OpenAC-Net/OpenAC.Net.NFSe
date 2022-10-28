using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenAC.Net.NFSe.Nota.Tipos
{
    public static class TipoDeducao
    {
        #region ABRASF Brasilia

        private static ABRASFBrasilia abrasfBrasilia;
        public static ABRASFBrasilia AbrasfBrasilia => abrasfBrasilia ??= new ABRASFBrasilia();

        public sealed class ABRASFBrasilia
        {
            internal ABRASFBrasilia()
            {
                Materiais = 1;
                Subempreitada = 2;
                Servicos = 3;
                ProducaoExterna = 4;
                AlimentacaoBebidasFrigobar = 5;
                ReembolsoDspesas = 6;
                RepasseConsorciado = 7;
                RepassPlanoSaude = 8;
                OutrasDeducoes = 99;
            }

            public int Materiais { get; }
            public int Subempreitada { get; }
            public int Servicos { get; }
            public int ProducaoExterna { get; }
            public int AlimentacaoBebidasFrigobar { get; }
            public int ReembolsoDspesas { get; }
            public int RepasseConsorciado { get; }
            public int RepassPlanoSaude { get; }
            public int OutrasDeducoes { get; }
        }

        #endregion ABRASF Brasilia

        #region DSF

        private static DSF dsf;
        public static DSF DsF => dsf ??= new DSF();

        public sealed class DSF
        {
            internal DSF()
            {
                Nenhum = 1;
                Materiais = 2;
                SubEmpreitada = 3;
                Mercadorias = 4;
                VeiculacaoeDivulgacao = 5;
                MapadeConstCivil = 6;
                Servicos = 7;
            }

            public int Nenhum { get; }
            public int Materiais { get; }
            public int SubEmpreitada { get; }
            public int Mercadorias { get; }
            public int VeiculacaoeDivulgacao { get; }
            public int MapadeConstCivil { get; }
            public int Servicos { get; }

            private const string _Nenhum = "";
            private const string _Materiais = "Despesas com Materiais";
            private const string _SubEmpreitada = "Despesas com Subempreitada";
            private const string _Mercadorias = "Despesas com Mercadorias";
            private const string _VeiculacaoeDivulgacao = "Servicos de Veiculacao e Divulgacao";
            private const string _MapadeConstCivil = "Mapa de Const. Civil";
            private const string _Servicos = "Servicos";

            public string GetDescription(int key)
            {
                switch (key)
                {
                    case 1:
                        return _Nenhum;
                    case 2:
                        return _Materiais;
                    case 3:
                        return _SubEmpreitada;
                    case 4:
                        return _Mercadorias;
                    case 5:
                        return _VeiculacaoeDivulgacao;
                    case 6:
                        return _MapadeConstCivil;
                    case 7:
                        return _Servicos;
                    default:
                        throw new Exception("DSF - Tipo de Dedução não implementada.");
                }
            }

            public int GetValue(string key)
            {
                key = key.Trim();

                switch (key)
                {
                    case _Nenhum:
                        return 1;
                    case _Materiais:
                        return 2;
                    case _SubEmpreitada:
                        return 3;
                    case _Mercadorias:
                        return 4;
                    case _VeiculacaoeDivulgacao:
                        return 5;
                    case _MapadeConstCivil:
                        return 6;
                    case _Servicos:
                        return 7;
                    default:
                        throw new Exception("DSF - Descrição de Dedução não implementada.");
                }
            }
        }

        #endregion DSF
    }
}
