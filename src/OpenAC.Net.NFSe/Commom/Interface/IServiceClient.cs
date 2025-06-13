using System;

namespace OpenAC.Net.NFSe.Commom.Interface;

/// <summary>
/// Interface que define os métodos que os serviços de NFSe precisam implementar.
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
    /// Método para envio de RPS de forma assíncrona.
    /// </summary>
    /// <param name="cabec">Cabeçalho da requisição, pode ser nulo dependendo do provedor.</param>
    /// <param name="msg">Mensagem XML a ser enviada ao provedor.</param>
    /// <returns>Retorna a resposta do provedor como string.</returns>
    string Enviar(string? cabec, string msg);

    /// <summary>
    /// Método para envio de RPS de forma síncrona.
    /// </summary>
    /// <param name="cabec">Cabeçalho da requisição, pode ser nulo dependendo do provedor.</param>
    /// <param name="msg">Mensagem XML a ser enviada ao provedor.</param>
    /// <returns>Retorna a resposta do provedor como string.</returns>
    string EnviarSincrono(string? cabec, string msg);

    /// <summary>
    /// Método para consultar a situação da emissão da NFSe.
    /// </summary>
    /// <param name="cabec"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    string ConsultarSituacao(string? cabec, string msg);

    /// <summary>
    /// Método para consultar a situação da emissão da NFSe.
    /// </summary>
    /// <param name="cabec">Cabeçalho da requisição, pode ser nulo dependendo do provedor.</param>
    /// <param name="msg">Mensagem XML a ser enviada ao provedor.</param>
    /// <returns>Retorna a resposta do provedor como string.</returns>
    string ConsultarLoteRps(string? cabec, string msg);

    /// <summary>
    /// Método para consultar o número sequencial do último RPS emitido.
    /// </summary>
    /// <param name="cabec">Cabeçalho da requisição, pode ser nulo dependendo do provedor.</param>
    /// <param name="msg">Mensagem XML a ser enviada ao provedor.</param>
    /// <returns>Retorna a resposta do provedor como string.</returns>
    string ConsultarSequencialRps(string? cabec, string msg);

    /// <summary>
    /// Método para consultar a NFSe a partir da RPS.
    /// </summary>
    /// <param name="cabec">Cabeçalho da requisição, pode ser nulo dependendo do provedor.</param>
    /// <param name="msg">Mensagem XML a ser enviada ao provedor.</param>
    /// <returns>Retorna a resposta do provedor como string.</returns>
    string ConsultarNFSeRps(string? cabec, string msg);

    /// <summary>
    /// Método para consultar a NFSe.
    /// </summary>
    /// <param name="cabec">Cabeçalho da requisição, pode ser nulo dependendo do provedor.</param>
    /// <param name="msg">Mensagem XML a ser enviada ao provedor.</param>
    /// <returns>Retorna a resposta do provedor como string.</returns>
    string ConsultarNFSe(string? cabec, string msg);

    /// <summary>
    /// Método para cancelar a NFSe.
    /// </summary>
    /// <param name="cabec">Cabeçalho da requisição, pode ser nulo dependendo do provedor.</param>
    /// <param name="msg">Mensagem XML a ser enviada ao provedor.</param>
    /// <returns>Retorna a resposta do provedor como string.</returns>
    string CancelarNFSe(string? cabec, string msg);

    /// <summary>
    /// Método para cancelar um lote de RPS.
    /// </summary>
    /// <param name="cabec">Cabeçalho da requisição, pode ser nulo dependendo do provedor.</param>
    /// <param name="msg">Mensagem XML a ser enviada ao provedor.</param>
    /// <returns>Retorna a resposta do provedor como string.</returns>
    string CancelarNFSeLote(string? cabec, string msg);

    /// <summary>
    /// Método para substituir uma NFSe por outra.
    /// </summary>
    /// <param name="cabec">Cabeçalho da requisição, pode ser nulo dependendo do provedor.</param>
    /// <param name="msg">Mensagem XML a ser enviada ao provedor.</param>
    /// <returns>Retorna a resposta do provedor como string.</returns>
    string SubstituirNFSe(string? cabec, string msg);

    #endregion Methods
}