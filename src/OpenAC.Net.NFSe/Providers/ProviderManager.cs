// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 01-31-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-01-2018
// ***********************************************************************
// <copyright file="ProviderManager.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2022 Projeto OpenAC .Net
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Providers.Pvh;
using OpenAC.Net.NFSe.Providers.Thema;

namespace OpenAC.Net.NFSe.Providers
{
    /// <summary>
    /// Classe responsável por criar uma nova instancia do provedor
    /// </summary>
    public static class ProviderManager
    {
        #region Constructors

        static ProviderManager()
        {
            Municipios = new List<OpenMunicipioNFSe>();
            Providers = new Dictionary<NFSeProvider, Type>
            {
                {NFSeProvider.Abaco, typeof(ProviderAbaco)},
                {NFSeProvider.ABase, typeof(ProviderABase)},
                {NFSeProvider.Americana, typeof(ProviderAmericana)},
                {NFSeProvider.AssessorPublico, typeof(ProviderAssessorPublico)},
                {NFSeProvider.BHISS, typeof(ProviderBHISS)},
                {NFSeProvider.Betha, typeof(ProviderBetha)},
                {NFSeProvider.Betha2, typeof(ProviderBetha2)},
                {NFSeProvider.CITTA, typeof(ProviderCITTA)},
                {NFSeProvider.Conam, typeof(ProviderCONAM)},
                {NFSeProvider.Coplan, typeof(ProviderCoplan)},
                {NFSeProvider.Curitiba, typeof(ProviderCuritiba)},
                {NFSeProvider.DBSeller, typeof(ProviderDBSeller)},
                {NFSeProvider.DSF, typeof(ProviderDSF)},
                {NFSeProvider.DSFSJC, typeof(ProviderDSFSJC)},
                {NFSeProvider.Equiplano, typeof(ProviderEquiplano)},
                {NFSeProvider.Fiorilli, typeof(ProviderFiorilli)},
                {NFSeProvider.Fisco, typeof(ProviderFisco)},
                {NFSeProvider.FissLex, typeof(ProviderFissLex)},
                {NFSeProvider.Ginfes, typeof(ProviderGinfes)},
                {NFSeProvider.Goiania, typeof(ProviderGoiania)},
                {NFSeProvider.IPM, typeof(ProviderIPM)},
                {NFSeProvider.ISSe, typeof(ProviderISSe)},
                {NFSeProvider.ISSNet, typeof(ProviderISSNet)},
                {NFSeProvider.Mitra, typeof(ProviderMitra)},
                {NFSeProvider.NFeCidades, typeof(ProviderNFeCidades)},
                {NFSeProvider.NotaCarioca, typeof(ProviderNotaCarioca)},
                {NFSeProvider.Pronim2, typeof(ProviderPronim2)},
                {NFSeProvider.Pronim203, typeof(ProviderPronim203)},
                {NFSeProvider.PVH, typeof(ProviderPvh)},
                {NFSeProvider.RLZ, typeof(ProviderRLZ)},
                {NFSeProvider.SIAPNet, typeof(ProviderSIAPNet)},
                {NFSeProvider.Sigiss, typeof(ProviderSigiss)},
                {NFSeProvider.Sigiss2, typeof(ProviderSigiss2)},
                {NFSeProvider.SigissWeb, typeof(ProviderSigissWeb)},
                {NFSeProvider.SimplISS, typeof(ProviderSimplISS)},
                {NFSeProvider.Sintese, typeof(ProviderSintese)},
                {NFSeProvider.SpeedGov, typeof(ProviderSpeedGov)},
                {NFSeProvider.SystemPro, typeof(ProviderSystemPro)},
                {NFSeProvider.SaoPaulo, typeof(ProviderSaoPaulo)},
                {NFSeProvider.SmarAPDABRASF, typeof(ProviderSmarAPDABRASF)},
                {NFSeProvider.Vitoria, typeof(ProviderVitoria)},
                {NFSeProvider.WebIss, typeof(ProviderWebIss)},
                {NFSeProvider.WebIss2, typeof(ProviderWebIss2)},
                {NFSeProvider.MetropolisWeb, typeof(ProviderMetropolisWeb)},
                {NFSeProvider.Thema, typeof(ProviderThema)}
            };

            Load();
        }

        #endregion Constructors

        #region Propriedades

        /// <summary>
        /// Municipios cadastrados no OpenNFSe
        /// </summary>
        /// <value>Os municipios</value>
        public static List<OpenMunicipioNFSe> Municipios { get; }

        /// <summary>
        /// Provedores cadastrados no OpenNFSe
        /// </summary>
        /// <value>Os provedores</value>
        public static Dictionary<NFSeProvider, Type> Providers { get; }

        #endregion Propriedades

        #region Methods

        #region Public

        /// <summary>
        /// Salva o arquivo de cidades.
        /// </summary>
        /// <param name="path">Caminho para salvar o arquivo</param>
        public static void Save(string path = "Municipios.nfse")
        {
            Guard.Against<ArgumentNullException>(path == null, "Path invalido.");

            if (File.Exists(path)) File.Delete(path);

            using var fileStream = new FileStream(path, FileMode.CreateNew);
            Save(fileStream);
        }

        /// <summary>
        /// Salva o arquivo de cidades.
        /// </summary>
        /// <param name="stream">O stream.</param>
        public static void Save(Stream stream)
        {
            foreach (var value in Enum.GetValues(typeof(TipoUrl)).Cast<TipoUrl>())
            {
                foreach (var m in Municipios)
                {
                    if (!m.UrlHomologacao.ContainsKey(value)) m.UrlHomologacao.Add(value, string.Empty);
                    if (!m.UrlProducao.ContainsKey(value)) m.UrlProducao.Add(value, string.Empty);
                }
            }

            var serializer = new MunicipiosNFSe { Municipios = Municipios.OrderBy(x => x.Nome).ToArray() };
            serializer.Save(stream, DFeSaveOptions.None);
        }

        /// <summary>
        /// Carrega o arquivo de cidades.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="clean">if set to <c>true</c> [clean].</param>
        public static void Load(string path = "", bool clean = true)
        {
            byte[] buffer = null;
            if (path.IsEmpty())
            {
                var assembly = Assembly.GetExecutingAssembly();
                using var resourceStream = assembly.GetManifestResourceStream("OpenAC.Net.NFSe.Resources.Municipios.nfse");
                if (resourceStream != null)
                {
                    buffer = new byte[resourceStream.Length];
                    resourceStream.Read(buffer, 0, buffer.Length);
                }
            }
            else if (File.Exists(path))
            {
                buffer = File.ReadAllBytes(path);
            }

            Guard.Against<ArgumentException>(buffer == null, "Arquivo de cidades não encontrado");

            using var stream = new MemoryStream(buffer);
            Load(stream, clean);
        }

        /// <summary>
        /// Carrega o arquivo de cidades.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="clean">if set to <c>true</c> [clean].</param>
        public static void Load(Stream stream, bool clean = true)
        {
            Guard.Against<ArgumentException>(stream == null, "Arquivo de cidades não encontrado");

            var municipiosNFSe = MunicipiosNFSe.Load(stream);
            if (clean) Municipios.Clear();
            Municipios.AddRange(municipiosNFSe.Municipios);
        }

        /// <summary>
        /// Retorna o provedor para o municipio nas configurações informadas.
        /// </summary>
        /// <param name="config">A configuração.</param>
        /// <returns>Provedor NFSe.</returns>
        public static ProviderBase GetProvider(ConfigNFSe config)
        {
            var municipio = Municipios.SingleOrDefault(x => x.Codigo == config.WebServices.CodigoMunicipio);
            Guard.Against<OpenException>(municipio == null, "Provedor para esta cidade não implementado ou não especificado!");

            // ReSharper disable once PossibleNullReferenceException
            var providerType = Providers[municipio.Provedor];
            Guard.Against<OpenException>(providerType == null, "Provedor não encontrado!");
            Guard.Against<OpenException>(!CheckBaseType(providerType), "Classe base do provedor incorreta!");

            // ReSharper disable once AssignNullToNotNullAttribute
            return (ProviderBase)Activator.CreateInstance(providerType, config, municipio);
        }

        #endregion Public

        #region Private

        private static bool CheckBaseType(Type providerType)
        {
            return typeof(ProviderBase).IsAssignableFrom(providerType) ||
                   typeof(ProviderABRASF).IsAssignableFrom(providerType) ||
                   typeof(ProviderABRASF201).IsAssignableFrom(providerType) ||
                   typeof(ProviderABRASF202).IsAssignableFrom(providerType) ||
                   typeof(ProviderABRASF204).IsAssignableFrom(providerType);
        }

        #endregion Private

        #endregion Methods
    }
}