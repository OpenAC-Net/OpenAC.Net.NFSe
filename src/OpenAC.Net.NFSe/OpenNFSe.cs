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
//	     		    Copyright (c) 2014 - 2021 Projeto OpenAC .Net
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
using System.ComponentModel;
using System.IO;
using System.Net;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.Core.Logging;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe
{
    public sealed class OpenNFSe : OpenComponent, IOpenLog
    {
        #region Fields

        private OpenDANFSeBase danfSe;
        internal ProviderBase provider;

        #endregion Fields

        #region Propriedades

        /// <summary>
        /// Configurações do Componente
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ConfigNFSe Configuracoes { get; private set; }

        /// <summary>
        /// Componente de impressão
        /// </summary>
        public OpenDANFSeBase DANFSe
        {
            get => danfSe;
            set
            {
                danfSe = value;
                if (danfSe != null && danfSe.Parent != this)
                    danfSe.Parent = this;
            }
        }

        /// <summary>
        /// Coleção de NFSe para processar e/ou processadas
        /// </summary>
        [Browsable(false)]
        public NotaServicoCollection NotasServico { get; private set; }

        #endregion Propriedades

        #region Methods

        /// <summary>
        /// Envia as NFSe para o provedor da cidade.
        /// </summary>
        /// <param name="lote">Numero do lote.</param>
        /// <param name="sincrono">Se for informado <c>true</c> o envio será sincrono.</param>
        /// <param name="imprimir">Se for informado <c>true</c> imprime as RPS, se o envio foi executado com sucesso.</param>
        /// <returns>RetornoWebservice.</returns>
        public RetornoEnviar Enviar(int lote, bool sincrono = false, bool imprimir = false)

        {
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");
            Guard.Against<OpenException>(NotasServico.Count < 1, "ERRO: Nenhuma RPS adicionada ao Lote");

            Guard.Against<OpenException>(NotasServico.Count > 50,
                $"ERRO: Conjunto de RPS transmitidos (máximo de 50 RPS) excedido.{Environment.NewLine}" +
                $"Quantidade atual: {NotasServico.Count}");

            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;

                var ret = sincrono
                    ? provider.EnviarSincrono(lote, NotasServico)
                    : provider.Enviar(lote, NotasServico);

                if (ret.Sucesso && imprimir)
                    DANFSe?.Imprimir();

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
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");
            Guard.Against<ArgumentException>(lote < 1, "Lote não pode ser Zero ou negativo.");

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
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");

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
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");
            Guard.Against<ArgumentNullException>(serie.IsEmpty(), "Serie não pode ser vazia ou nulo.");

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
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");

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
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");
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
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");
            Guard.Against<OpenException>(numeroNfse < 1, "O número da NFSe não pode ser zero ou negativo.");

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
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");
            Guard.Against<OpenException>(numeroNfse < 1, "O número da NFSe não pode ser zero ou negativo.");

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
            }
        }

        /// <summary>
        /// Consulta as NFSe de acordo com os filtros.
        ///
        /// Obs.: Nem todos provedores suportam este metodo.
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fim"></param>
        /// <returns>RetornoWebservice.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public RetornoConsultarNFSe ConsultaNFSePeriodo(DateTime inicio, DateTime fim)
        {
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");
            Guard.Against<OpenException>(inicio.Date > fim.Date, "A data inicial não pode ser maior que a data final.");

            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
                return provider.ConsultaNFSe(NotasServico, inicio, fim);
            }
            catch (Exception exception)
            {
                this.Log().Error("[ConsultaNFSePeriodo]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
            }
        }

        /// <summary>
        /// Consulta as NFSe de acordo com os filtros.
        ///
        /// Obs.: Nem todos provedores suportam este metodo.
        /// </summary>
        /// <param name="cnpjTomador"></param>
        /// <param name="imTomador"></param>
        /// <returns>RetornoWebservice.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public RetornoConsultarNFSe ConsultaNFSeTomador(string cnpjTomador, string imTomador)
        {
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");
            Guard.Against<OpenException>(cnpjTomador.IsEmpty(), "O CNPJ/CPF do tomador não pode ser vazio.");

            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
                return provider.ConsultaNFSe(NotasServico, cnpjTomador: cnpjTomador, imTomador: imTomador);
            }
            catch (Exception exception)
            {
                this.Log().Error("[ConsultaNFSeTomador]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
            }
        }

        /// <summary>
        /// Consulta as NFSe de acordo com os filtros.
        ///
        /// Obs.: Nem todos provedores suportam este metodo.
        /// </summary>
        /// <param name="cnpjInter"></param>
        /// <param name="imInter"></param>
        /// <returns></returns>
        public RetornoConsultarNFSe ConsultaNFSeIntermediario(string nomeInter, string cnpjInter, string imInter)
        {
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");
            Guard.Against<OpenException>(nomeInter.IsEmpty(), "O Nome do intermediário não pode ser vazio.");
            Guard.Against<OpenException>(cnpjInter.IsEmpty(), "O CNPJ/CPF do intermediário não pode ser vazio.");

            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
                return provider.ConsultaNFSe(NotasServico, nomeInter: nomeInter, cnpjInter: cnpjInter, imInter: imInter);
            }
            catch (Exception exception)
            {
                this.Log().Error("[ConsultaNFSeIntermediario]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
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
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");

            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
                return provider.CancelarNFSe(codigoCancelamento, numeroNFSe, "", 0, motivo, NotasServico);
            }
            catch (Exception exception)
            {
                this.Log().Error("[CancelarNFSe]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
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
        public RetornoCancelar CancelarNFSe(string codigoCancelamento, string numeroNFSe, string serie, decimal valor, string motivo)
        {
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");

            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
                return provider.CancelarNFSe(codigoCancelamento, numeroNFSe, serie, valor, motivo, NotasServico);
            }
            catch (Exception exception)
            {
                this.Log().Error("[CancelarNFSe]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
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
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");

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
            Guard.Against<OpenException>(provider == null, "ERRO: Nenhuma cidade informada.");
            Guard.Against<ArgumentException>(codigoCancelamento.IsEmpty(), "ERRO: Código de Cancelamento não informado");
            Guard.Against<ArgumentException>(numeroNFSe.IsEmpty(), "ERRO: Numero da NFS-e não informada");
            Guard.Against<ArgumentException>(NotasServico.Count < 1, "ERRO: Nenhuma RPS carregada ao componente");

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
            }
        }

        /// <summary>
        /// Imprime o DANFse
        /// </summary>
        public void Imprimir()
        {
            Guard.Against<ArgumentNullException>(DANFSe == null, "Nenhum componente de impressão especificado.");
            DANFSe?.Imprimir();
        }

        /// <summary>
        /// Imprime o PDF do DANFse
        /// </summary>
        public void ImprimirPDF()
        {
            Guard.Against<ArgumentNullException>(DANFSe == null, "Nenhum componente de impressão especificado.");
            DANFSe?.ImprimirPDF();
        }

        /// <summary>
        /// Imprime o PDF do DANFse para uma stream
        /// </summary>
        public void ImprimirPDF(Stream stream)
        {
            Guard.Against<ArgumentNullException>(DANFSe == null, "Nenhum componente de impressão especificado.");
            DANFSe?.ImprimirPDF(stream);
        }

        /// <summary>
        /// Imprime o HTML do DANFse
        /// </summary>
        public void ImprimirHTML()
        {
            Guard.Against<ArgumentNullException>(DANFSe == null, "Nenhum componente de impressão especificado.");
            DANFSe?.ImprimirHTML();
        }

        /// <summary>
        /// Imprime o HTML do DANFse para uma stream
        /// </summary>
        public void ImprimirHTML(Stream stream)
        {
            Guard.Against<ArgumentNullException>(DANFSe == null, "Nenhum componente de impressão especificado.");
            DANFSe?.ImprimirHTML(stream);
        }

        #endregion Methods

        #region Override Methods

        /// <summary>
        /// Função executada na inicialização do componente
        /// </summary>
        protected override void OnInitialize()
        {
            Configuracoes = new ConfigNFSe(this);
            NotasServico = new NotaServicoCollection(Configuracoes);
        }

        /// <summary>
        /// Função executada na desinicialização do componente
        /// </summary>
        protected override void OnDisposing()
        {
            provider?.Dispose(); ;
        }

        #endregion Override Methods
    }
}