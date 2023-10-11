namespace OpenAC.Net.NFSe.Nota;

/// <summary>
/// Classe que contém os tipos de Tomador por provedor.
/// </summary>
public static class TipoTomador
{
    #region InnerTypes

    public sealed class TipoSigiss
    {

        #region Constructors

        internal TipoSigiss()
        {
            PFNI = 1;
            PessoaFisica = 2;
            JuridicaDoMunicipio = 3;
            JuridicaForaMunicipio = 4;
            JuridicaForaPais = 5;
            ProdutorRuralOuPolitico = 6;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Obtém a Tipo de Tomador 1 – Pessoa Física não Identificada.
        /// </summary>
        public int PFNI { get; }

        /// <summary>
        /// Obtém a Tipo de Tomador 2 - Pessoa Física.
        /// </summary>
        public int PessoaFisica { get; }

        /// <summary>
        /// Obtém a Tipo de Tomador 3 – Pessoa Jurídica do Município.
        /// </summary>
        public int JuridicaDoMunicipio { get; }

        /// <summary>
        /// Obtém a Tipo de Tomador 4 – Pessoa Jurídica Fora do Município.
        /// </summary>
        public int JuridicaForaMunicipio { get; }

        /// <summary>
        /// Obtém a Tipo de Tomador 5 – Pessoa Jurídica Fora do País.
        /// </summary>
        public int JuridicaForaPais { get; }

        /// <summary>
        /// Obtém a Tipo de Tomador 6 – Produtor Rural ou Político.
        /// </summary>
        public int ProdutorRuralOuPolitico { get; }

        #endregion Properties
    }

    #endregion InnerTypes

    #region Fields

    private static TipoSigiss sigiss;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Obtém os Tipos de Tomador Sigiss.
    /// </summary>
    public static TipoSigiss Sigiss => sigiss ?? (sigiss = new TipoSigiss());

    #endregion Properties
}