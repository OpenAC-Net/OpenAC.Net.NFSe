using System;

namespace OpenAC.Net.NFSe.Commom.Interface;

/// <summary>
/// “Interface” que define os métodos que os serviços de NFSe precisam ter.
/// </summary>
public interface IServiceClient : IDisposable
{
    #region Properties

    /// <summary>
    /// Obtém os dados enviados ao provedor.
    /// </summary>
    string EnvelopeEnvio { get; }

    /// <summary>
    /// Obtém os dados retornados pelo provedor.
    /// </summary>
    string EnvelopeRetorno { get; }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Método para envio de rps de forma assíncrona.
    /// </summary>
    /// <param name="cabec"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    string Enviar(string? cabec, string msg);

    /// <summary>
    /// Método para envio de rps de forma síncrona.
    /// </summary>
    /// <param name="cabec"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    string EnviarSincrono(string? cabec, string msg);

    /// <summary>
    /// Método para consultar a situação da emissão da NFSe.
    /// </summary>
    /// <param name="cabec"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    string ConsultarSituacao(string? cabec, string msg);

    /// <summary>
    /// Método para consulta a situação do lote de RPS.
    /// </summary>
    /// <param name="cabec"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    string ConsultarLoteRps(string? cabec, string msg);

    /// <summary>
    /// Método para consulta o número da última RPS emitida.
    /// </summary>
    /// <param name="cabec"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    string ConsultarSequencialRps(string? cabec, string msg);

    /// <summary>
    /// Método para consulta a NFSe a partir da RPS.
    /// </summary>
    /// <param name="cabec"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    string ConsultarNFSeRps(string? cabec, string msg);

    /// <summary>
    /// Método para consulta a NFSe.
    /// </summary>
    /// <param name="cabec"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    string ConsultarNFSe(string? cabec, string msg);

    /// <summary>
    /// Método para cancelar a NFSe.
    /// </summary>
    /// <param name="cabec"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    string CancelarNFSe(string? cabec, string msg);

    /// <summary>
    /// Método um lote de RPS.
    /// </summary>
    /// <param name="cabec"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    string CancelarNFSeLote(string? cabec, string msg);

    /// <summary>
    /// Método para substituir uma NFSe por outra.
    /// </summary>
    /// <param name="cabec"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    string SubstituirNFSe(string? cabec, string msg);

    #endregion Methods
}