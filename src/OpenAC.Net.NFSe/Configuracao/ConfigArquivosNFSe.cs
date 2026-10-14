// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 01-31-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-07-2016
// ***********************************************************************
// <copyright file="ConfigArquivosNFSe.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		Copyright (c) 2014 - 2024 Projeto OpenAC .Net
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
using System.IO;
using System.Reflection;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Configuracao;

/// <summary>
/// Configurações de diretórios e caminhos para gravação dos arquivos da NFSe, RPS, Lotes e mensagens SOAP.
/// </summary>
public sealed class ConfigArquivosNFSe : DFeArquivosConfigBase
{
    #region Constructor

    /// <summary>
    /// Inicializa uma nova instancia da classe <see cref="ConfigArquivosNFSe"/>.
    /// </summary>
    internal ConfigArquivosNFSe()
    {
        EmissaoPathNFSe = false;

        var path = Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location);
        if (!path.IsEmpty())
        {
            PathNFSe = Path.Combine(path, "NFSe");
            PathLote = Path.Combine(path, "Lote");
            PathRps = Path.Combine(path, "RPS");
        }
        else
        {
            PathNFSe = string.Empty;
            PathLote = string.Empty;
            PathRps = string.Empty;
        }
    }

    #endregion Constructor

    #region Properties

    /// <summary>
    /// Indica se o caminho de emissão da NFSe será utilizado na estrutura de pastas.
    /// </summary>
    /// <value><c>true</c> se deve incluir data de emissão no caminho; senão, <c>false</c>.</value>
    public bool EmissaoPathNFSe { get; set; }

    /// <summary>
    /// Obtém ou define o diretório base para salvar as notas fiscais de serviço eletrônicas (NFSe).
    /// </summary>
    /// <value>O caminho do diretório de NFSe.</value>
    public string PathNFSe { get; set; }

    /// <summary>
    /// Obtém ou define o diretório base para salvar os lotes de RPS enviados.
    /// </summary>
    /// <value>O caminho do diretório de Lotes.</value>
    public string PathLote { get; set; }

    /// <summary>
    /// Obtém ou define o diretório base para salvar os recibos provisórios de serviço (RPS).
    /// </summary>
    /// <value>O caminho do diretório de RPS.</value>
    public string PathRps { get; set; }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Obtém o caminho completo para gravação de mensagens SOAP de acordo com a data e CNPJ.
    /// </summary>
    /// <param name="data">Data de referência.</param>
    /// <param name="cnpj">CNPJ do prestador (opcional).</param>
    /// <returns>Caminho do diretório de arquivos SOAP.</returns>
    public string GetPathSoap(DateTime data, string cnpj = "")
    {
        return GetPath(PathNFSe, "SOAP", cnpj, data);
    }

    /// <summary>
    /// Obtém o caminho completo para gravação das notas de serviço (NFSe).
    /// </summary>
    /// <param name="data">Data de referência.</param>
    /// <param name="cnpj">CNPJ do prestador (opcional).</param>
    /// <returns>Caminho do diretório de NFSe.</returns>
    public string GetPathNFSe(DateTime data, string cnpj = "")
    {
        return GetPath(PathNFSe, "NFSe", cnpj, data, "NFSe");
    }

    /// <summary>
    /// Obtém o caminho completo para gravação de arquivos de lote de RPS.
    /// </summary>
    /// <param name="data">Data de referência.</param>
    /// <param name="cnpj">CNPJ do prestador (opcional).</param>
    /// <returns>Caminho do diretório de lotes.</returns>
    public string GetPathLote(DateTime data, string cnpj = "")
    {
        return GetPath(PathLote, "Lote", cnpj, data);
    }

    /// <summary>
    /// Obtém o caminho completo para gravação de arquivos de RPS individuais.
    /// </summary>
    /// <param name="data">Data de referência.</param>
    /// <param name="cnpj">CNPJ do prestador (opcional).</param>
    /// <returns>Caminho do diretório de RPS.</returns>
    public string GetPathRps(DateTime data, string cnpj = "")
    {
        return GetPath(PathRps, "Rps", cnpj, data, "Rps");
    }

    /// <inheritdoc />
    protected override void ArquivoServicoChange()
    {
        ProviderManager.Load(ArquivoServicos);
    }

    #endregion Methods
}