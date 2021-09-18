using System;

namespace OpenAC.Net.NFSe.Providers
{
    /// <summary>
    /// Interface que define os métodos que os serviços de NFSe precisam ter.
    /// </summary>
    public interface IServiceClient : IDisposable
    {
        #region Properties

        string EnvelopeEnvio { get; }

        string EnvelopeRetorno { get; }

        #endregion Properties

        #region Methods

        string Enviar(string cabec, string msg);

        string EnviarSincrono(string cabec, string msg);

        string ConsultarSituacao(string cabec, string msg);

        string ConsultarLoteRps(string cabec, string msg);

        string ConsultarSequencialRps(string cabec, string msg);

        string ConsultarNFSeRps(string cabec, string msg);

        string ConsultarNFSe(string cabec, string msg);

        string CancelarNFSe(string cabec, string msg);

        string CancelarNFSeLote(string cabec, string msg);

        string SubstituirNFSe(string cabec, string msg);

        #endregion Methods
    }
}