// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 01-31-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-05-2018
// ***********************************************************************
// <copyright file="OpenNFSe.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2023 Projeto OpenAC .Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Net;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.Core.Logging;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe;

public sealed class OpenNFSe : OpenDisposable, IOpenLog
{
    #region Propriedades

    /// <summary>
    /// Configurações do Componente
    /// </summary>
    public ConfigNFSe Configuracoes { get; private set; }

    /// <summary>
    /// Coleção de NFSe para processar e/ou processadas
    /// </summary>
    public NotaServicoCollection NotasServico { get; private set; }

    #endregion Propriedades

    #region Constructors

    public OpenNFSe()
    {
        Configuracoes = new ConfigNFSe();
        NotasServico = new NotaServicoCollection(Configuracoes);
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Envia as NFSe para o provedor da cidade.
    /// </summary>
    /// <param name="lote">Numero do lote.</param>
    /// <param name="sincrono">Se for informado <c>true</c> o envio será sincrono.</param>
    /// <returns>RetornoWebservice.</returns>
    public RetornoEnviar Enviar(int lote, bool sincrono = false)
    {
        Guard.Against<OpenException>(NotasServico.Count < 1, "ERRO: Nenhuma RPS adicionada ao Lote");
        Guard.Against<OpenException>(NotasServico.Count > 50,
            $"ERRO: Conjunto de RPS transmitidos (máximo de 50 RPS) excedido.{Environment.NewLine}" +
            $"Quantidade atual: {NotasServico.Count}");

        var provider = ProviderManager.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;
        
        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;

            var ret = sincrono
                ? provider.EnviarSincrono(lote, NotasServico)
                : provider.Enviar(lote, NotasServico);

            return ret;
        }
        catch (Exception exception)
        {
            this.Log().Error("[Enviar]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
            provider?.Dispose();
        }
    }

    /// <summary>
    /// Consulta a situação do lote de RPS.
    ///
    /// Obs.: Nem todos provedores suportam este metodo.
    /// </summary>
    /// <param name="lote">The lote.</param>
    /// <param name="protocolo">The protocolo.</param>
    /// <returns>RetornoWebservice.</returns>
    public RetornoConsultarSituacao ConsultarSituacao(int lote, string protocolo = "")
    {
        Guard.Against<ArgumentException>(lote < 1, "Lote não pode ser Zero ou negativo.");

        var provider = ProviderManager.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultarSituacao(lote, protocolo);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultarSituacao]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
            provider.Dispose();
        }
    }

    /// <summary>
    /// Consulta o lote de Rps
    ///
    /// Obs.: Nem todos provedores suportam este metodo.
    /// </summary>
    /// <param name="protocolo">The protocolo.</param>
    /// <param name="lote">The lote.</param>
    /// <returns>RetornoWebservice.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoConsultarLoteRps ConsultarLoteRps(int lote, string protocolo)
    {
        var provider = ProviderManager.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultarLoteRps(lote, protocolo, NotasServico);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultarLoteRps]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
            provider.Dispose();
        }
    }

    /// <summary>
    /// Consulta o numero de sequencia dos lotes de RPS.
    ///
    /// Obs.: Nem todos provedores suportam este metodo.
    /// </summary>
    /// <param name="serie">The serie.</param>
    /// <returns>RetornoWebservice.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoConsultarSequencialRps ConsultarSequencialRps(string serie)
    {
        Guard.Against<ArgumentNullException>(serie.IsEmpty(), "Serie não pode ser vazia ou nulo.");

        var provider = ProviderManager.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultarSequencialRps(serie);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultarSequencialRps]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
            provider.Dispose();
        }
    }

    /// <summary>
    /// Consulta a NFSe/RPS que atende os filtros informados.
    ///
    /// Obs.: Nem todos provedores suportam este metodo.
    /// </summary>
    /// <param name="numero">The numero.</param>
    /// <param name="serie">The serie.</param>
    /// <param name="tipo">The tipo.</param>
    /// <returns>RetornoWebservice.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoConsultarNFSeRps ConsultaNFSeRps(int numero, string serie, TipoRps tipo)
    {
        var provider = ProviderManager.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultaNFSeRps(numero, serie, tipo, NotasServico, 0, 0);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultarNFSeRps]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
            provider.Dispose();
        }
    }

    /// <summary>
    /// Consulta a NFSe/RPS que atende os filtros informados.
    ///
    /// Obs.: Nem todos provedores suportam este metodo.
    /// </summary>
    /// <param name="numero">The numero.</param>
    /// <param name="serie">The serie.</param>
    /// <param name="tipo">The tipo.</param>
    /// <returns>RetornoWebservice.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoConsultarNFSeRps ConsultaNFSeRps(int numero, string serie, TipoRps tipo, int mesCompetencia, int anoCompetencia)
    {
        var provider = ProviderManager.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultaNFSeRps(numero, serie, tipo, NotasServico, mesCompetencia, anoCompetencia);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultarNFSeRps]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
            provider.Dispose();
        }
    }

    /// <summary>
    /// Consulta as NFSe de acordo com os filtros.
    ///
    /// Obs.: Nem todos provedores suportam este metodo.
    /// </summary>
    /// <param name="numeroNfse">The numero nfse.</param>
    /// <param name="pagina"></param>
    /// <returns>RetornoWebservice.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoConsultarNFSe ConsultaNFSe(int numeroNfse, int pagina = 0)
    {
        Guard.Against<OpenException>(numeroNfse < 1, "O número da NFSe não pode ser zero ou negativo.");

        var provider = ProviderManager.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultaNFSe(NotasServico, numeroNfse: numeroNfse, pagina: pagina);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultarNFSe]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
            provider.Dispose();
        }
    }

    /// <summary>
    /// Consulta as NFSe de acordo com os filtros.
    ///
    /// Obs.: Nem todos provedores suportam este metodo.
    /// </summary>
    /// <param name="numeroNfse">The numero nfse.</param>
    /// <returns>RetornoWebservice.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoConsultarNFSe ConsultaNFSe(int numeroNfse, string serieNfse)
    {
        Guard.Against<OpenException>(numeroNfse < 1, "O número da NFSe não pode ser zero ou negativo.");

        var provider = ProviderManager.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultaNFSe(NotasServico, numeroNfse: numeroNfse, serieNfse: serieNfse);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultarNFSe]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
            provider.Dispose();
        }
    }

    /// <summary>
    /// Consulta as NFSe de acordo com os filtros.
    ///
    /// Obs.: Nem todos provedores suportam este metodo.
    /// </summary>
    /// <param name="inicio"></param>
    /// <param name="fim"></param>
    /// <param name="pagina"></param>
    /// <returns>RetornoWebservice.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoConsultarNFSe ConsultaNFSePeriodo(DateTime inicio, DateTime fim, int pagina = 0)
    {
        Guard.Against<OpenException>(inicio.Date > fim.Date, "A data inicial não pode ser maior que a data final.");

        var provider = ProviderManager.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultaNFSe(NotasServico, inicio, fim, pagina: pagina);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultaNFSePeriodo]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
            provider.Dispose();
        }
    }

    /// <summary>
    /// Consulta as NFSe de acordo com os filtros.
    ///
    /// Obs.: Nem todos provedores suportam este metodo.
    /// </summary>
    /// <param name="cnpjTomador"></param>
    /// <param name="imTomador"></param>
    /// <param name="pagina"></param>
    /// <returns>RetornoWebservice.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public RetornoConsultarNFSe ConsultaNFSeTomador(string cnpjTomador, string imTomador, int pagina = 0)
    {
        Guard.Against<OpenException>(cnpjTomador.IsEmpty(), "O CNPJ/CPF do tomador não pode ser vazio.");

        var provider = ProviderManager.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultaNFSe(NotasServico, cnpjTomador: cnpjTomador, imTomador: imTomador, pagina: pagina);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultaNFSeTomador]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
            provider.Dispose();
        }
    }

    /// <summary>
    /// Consulta as NFSe de acordo com os filtros.
    ///
    /// Obs.: Nem todos provedores suportam este metodo.
    /// </summary>
    /// <param name="nomeInter"></param>
    /// <param name="cnpjInter"></param>
    /// <param name="imInter"></param>
    /// <param name="pagina"></param>
    /// <returns></returns>
    public RetornoConsultarNFSe ConsultaNFSeIntermediario(string nomeInter, string cnpjInter, string imInter, int pagina = 0)
    {
        Guard.Against<OpenException>(nomeInter.IsEmpty(), "O Nome do intermediário não pode ser vazio.");
        Guard.Against<OpenException>(cnpjInter.IsEmpty(), "O CNPJ/CPF do intermediário não pode ser vazio.");

        var provider = ProviderManager.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultaNFSe(NotasServico, nomeInter: nomeInter, cnpjInter: cnpjInter, imInter: imInter, pagina: pagina);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultaNFSeIntermediario]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
            provider.Dispose();
        }
    }

    /// <summary>
    /// Cancela uma NFSe
    ///
    /// Obs.: Nem todos provedores suportam este metodo.
    /// </summary>
    /// <param name="codigoCancelamento">O codigo de cancelamento.</param>
    /// <param name="numeroNFSe">O numero da NFSe.</param>
    /// <param name="motivo">O motivo.</param>
    /// <returns>RetornoWebservice.</returns>
    public RetornoCancelar CancelarNFSe(string codigoCancelamento, string numeroNFSe, string motivo)
    {
        var provider = ProviderManager.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
                return provider.CancelarNFSe(codigoCancelamento, numeroNFSe, "", 0, motivo, "", NotasServico);
            }
            catch (Exception exception)
            {
                this.Log().Error("[CancelarNFSe]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
                provider.Dispose();
            }
        }

        /// <summary>
        /// Cancela uma NFSe
        ///
        /// Obs.: Nem todos provedores suportam este metodo.
        /// </summary>
        /// <param name="codigoCancelamento">O codigo de cancelamento.</param>
        /// <param name="numeroNFSe">O numero da NFSe.</param>
        /// <param name="motivo">O motivo.</param>
        /// <returns>RetornoWebservice.</returns>
        public RetornoCancelar CancelarNFSe(string codigoCancelamento, string numeroNFSe, string serie, decimal valor, string motivo, string codigoVerificacao = null)
        {
            var provider = ProviderManager.GetProvider(Configuracoes);
            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
                return provider.CancelarNFSe(codigoCancelamento, numeroNFSe, serie, valor, motivo, codigoVerificacao, NotasServico);
            }
            catch (Exception exception)
            {
                this.Log().Error("[CancelarNFSe]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
                provider.Dispose();
            }
        }

    /// <summary>
    /// Cancela as NFSe que estão carregadas na lista.
    ///
    /// Obs.: Adicionar o motivo de cancelamento nas notas da lista.
    ///       Nem todos provedores suportam este metodo.
    /// </summary>
    /// <param name="lote">Identificação do lote.</param>
    /// <returns>RetornoWebservice.</returns>
    public RetornoCancelarNFSeLote CancelarNFSeLote(int lote)
    {
        var provider = ProviderManager.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            Guard.Against<ArgumentException>(NotasServico.Count < 1, "ERRO: Nenhuma NFS-e carregada ao componente");
            return provider.CancelarNFSeLote(lote, NotasServico);
        }
        catch (Exception exception)
        {
            this.Log().Error("[CancelarNFSe]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
            provider.Dispose();
        }
    }

    /// <summary>
    /// Substitui uma NFSe
    ///
    /// Obs.: Nem todos provedores suportam este metodo.
    /// </summary>
    /// <param name="codigoCancelamento">O codigo de cancelamento.</param>
    /// <param name="numeroNFSe">O numero da NFSe.</param>
    /// <param name="motivo">O motivo.</param>
    /// <returns>RetornoWebservice.</returns>
    public RetornoSubstituirNFSe SubstituirNFSe(string codigoCancelamento, string numeroNFSe, string motivo)
    {
        Guard.Against<ArgumentException>(codigoCancelamento.IsEmpty(), "ERRO: Código de Cancelamento não informado");
        Guard.Against<ArgumentException>(numeroNFSe.IsEmpty(), "ERRO: Numero da NFS-e não informada");
        Guard.Against<ArgumentException>(NotasServico.Count < 1, "ERRO: Nenhuma RPS carregada ao componente");

        var provider = ProviderManager.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.SubstituirNFSe(NotasServico, codigoCancelamento, numeroNFSe, motivo);
        }
        catch (Exception exception)
        {
            this.Log().Error("[SubstituirNFSe]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
            provider.Dispose();
        }
    }

    #endregion Methods
}