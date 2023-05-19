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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Configuracao;

namespace OpenAC.Net.NFSe.Providers;

/// <summary>
/// Classe responsável por criar uma nova instancia do provedor
/// </summary>
public static class ProviderManager
{
    #region Constructors

    static ProviderManager()
    {
        Municipios = new List<OpenMunicipioNFSe>();
        Providers = new Dictionary<NFSeProvider, Dictionary<VersaoNFSe, Type>>
        {
            {NFSeProvider.Abaco, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderAbaco)}}},
            {NFSeProvider.ABase, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve201, typeof(ProviderABase)}}},
            {NFSeProvider.AssessorPublico, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderAssessorPublico)}}},
            {NFSeProvider.Betha, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderBetha)}, {VersaoNFSe.ve202, typeof(ProviderBetha2)}}},
            {NFSeProvider.BHISS, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderBHISS)}}},
            {NFSeProvider.Citta, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve203, typeof(ProviderCitta)}}},
            {NFSeProvider.Conam, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve203, typeof(ProviderConam)}}},
            {NFSeProvider.Coplan, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve201, typeof(ProviderCoplan)}}},
            {NFSeProvider.DBSeller, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderDBSeller)}}},
            {NFSeProvider.DSF, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderDSF100)}}},
            {NFSeProvider.Equiplano, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderEquiplano)}}},
            {NFSeProvider.Fiorilli, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve200, typeof(ProviderFiorilli)}}},
            {NFSeProvider.Fisco, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve203, typeof(ProviderFisco)}}},
            {NFSeProvider.FissLex, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderFissLex)}}},
            {NFSeProvider.Ginfes, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderGinfes)}}},
            {NFSeProvider.IPM, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderIPM100)}, {VersaoNFSe.ve101, typeof(ProviderIPM101)}}},
            {NFSeProvider.ISSCuritiba, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderISSCuritiba)}}},
            {NFSeProvider.ISSDSF, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderISSDSF)}}},
            {NFSeProvider.ISSe, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve201, typeof(ProviderISSe)}}},
            {NFSeProvider.IISGoiania, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve200, typeof(ProviderISSGoiania)}}},
            {NFSeProvider.ISSNet, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve204, typeof(ProviderISSNet204)}}},
            {NFSeProvider.ISSRio, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderISSRio)}}},
            {NFSeProvider.ISSSaoPaulo, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderISSSaoPaulo)}}},
            {NFSeProvider.ISSVitoria, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve200, typeof(ProviderISSVitoria)}}},
            {NFSeProvider.MetropolisWeb, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderMetropolisWeb)}}},
            {NFSeProvider.Mitra, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve200, typeof(ProviderMitra)}}},
            {NFSeProvider.NFeCidades, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve201, typeof(ProviderNFeCidades)}}},
            {NFSeProvider.Pronim, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve202, typeof(ProviderPronim202)}, {VersaoNFSe.ve203, typeof(ProviderPronim203)}}},
            {NFSeProvider.IISPortoVelho, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve203, typeof(ProviderISSPortoVelho)}}},
            {NFSeProvider.RLZ, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve203, typeof(ProviderRLZ)}}},
            {NFSeProvider.SiapNet, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve201, typeof(ProviderSiapNet)}}},
            {NFSeProvider.SigISS, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderSigISS100)}, {VersaoNFSe.ve103, typeof(ProviderSigISS103)}}},
            {NFSeProvider.SigISSWeb, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderSigISSWeb)}}},
            {NFSeProvider.SimplISS, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderSimplISS100)}, {VersaoNFSe.ve203, typeof(ProviderSimplISS203)}}},
            {NFSeProvider.Sintese, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve204, typeof(ProviderSintese204)}}},
            {NFSeProvider.SmarAPD, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve204, typeof(ProviderSmarAPD204)}}},
            {NFSeProvider.SpeedGov, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderSpeedGov)}}},
            {NFSeProvider.SystemPro, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve201, typeof(ProviderSystemPro)}}},
            {NFSeProvider.Thema, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderThema)}}},
            {NFSeProvider.Tiplan, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve203, typeof(ProviderTiplan203)}}},
            {NFSeProvider.WebIss, new Dictionary<VersaoNFSe, Type> {{VersaoNFSe.ve100, typeof(ProviderWebIss)}, {VersaoNFSe.ve202, typeof(ProviderWebIss2)}}},
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
    public static Dictionary<NFSeProvider, Dictionary<VersaoNFSe, Type>> Providers { get; }

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
        var providerType = Providers[municipio.Provedor][municipio.Versao];
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