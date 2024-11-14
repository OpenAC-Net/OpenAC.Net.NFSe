// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 06-19-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 08-03-2017
// ***********************************************************************
// <copyright file="OpenMunicipioNFSe.cs" company="OpenAC .Net">
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

using System.Collections.Generic;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Document;
using OpenAC.Net.DFe.Core.Serializer;
//using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Commom.Model;

[DFeRoot("Municipio", Namespace = "https://www.openac.net.br/")]
public sealed class OpenMunicipioNFSe : DFeDocument<OpenMunicipioNFSe>
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenMunicipioNFSe"/> class.
    /// </summary>
    public OpenMunicipioNFSe()
    {
        UrlHomologacao = new Dictionary<TipoUrl, string>(10)
        {
            { TipoUrl.Enviar, string.Empty },
            { TipoUrl.EnviarSincrono, string.Empty },
            { TipoUrl.CancelarNFSe, string.Empty },
            { TipoUrl.CancelarNFSeLote, string.Empty },
            { TipoUrl.ConsultarNFSe, string.Empty },
            { TipoUrl.ConsultarNFSeRps, string.Empty },
            { TipoUrl.ConsultarLoteRps, string.Empty },
            { TipoUrl.ConsultarSituacao, string.Empty },
            { TipoUrl.ConsultarSequencialRps, string.Empty },
            { TipoUrl.SubstituirNFSe, string.Empty},
            { TipoUrl.Autenticacao, string.Empty}
        };

        UrlProducao = new Dictionary<TipoUrl, string>(10)
        {
            { TipoUrl.Enviar, string.Empty },
            { TipoUrl.EnviarSincrono, string.Empty },
            { TipoUrl.CancelarNFSe, string.Empty },
            { TipoUrl.CancelarNFSeLote, string.Empty },
            { TipoUrl.ConsultarNFSe, string.Empty },
            { TipoUrl.ConsultarNFSeRps, string.Empty },
            { TipoUrl.ConsultarLoteRps, string.Empty },
            { TipoUrl.ConsultarSituacao, string.Empty },
            { TipoUrl.ConsultarSequencialRps, string.Empty },
            { TipoUrl.SubstituirNFSe, string.Empty },
            { TipoUrl.Autenticacao, string.Empty }
        };

        Parametros = new Dictionary<string, string?>();
    }

    #endregion Constructors

    #region Propriedades

    /// <summary>
    /// Define ou retorna o codigo IBGE do municipio
    /// </summary>
    /// <value>The codigo.</value>
    [DFeElement(TipoCampo.Int, "Codigo")]
    public int Codigo { get; set; }

    /// <summary>
    /// Define ou retorna o nome do municipio
    /// </summary>
    /// <value>The nome.</value>
    [DFeElement(TipoCampo.Str, "Nome")]
    public string Nome { get; set; } = "";

    /// <summary>
    /// Define ou retorna a UF do municipio.
    /// </summary>
    /// <value>The uf.</value>
    [DFeElement(TipoCampo.Enum, "UF")]
    public DFeSiglaUF UF { get; set; }

    /// <summary>
    /// Define ou retorna o provedor de NFSe.
    /// </summary>
    /// <value>The provedor.</value>
    [DFeElement(TipoCampo.Enum, "Provedor")]
    public NFSeProvider Provedor { get; set; }

    /// <summary>
    /// Define ou retorna a versão do provedor de NFSe.
    /// </summary>
    /// <value>The provedor.</value>
    [DFeElement(TipoCampo.Enum, "Versao")]
    public VersaoNFSe Versao { get; set; }

    [DFeDictionary("Parametros")]
    [DFeDictionaryKey(TipoCampo.Str, "Id")]
    [DFeDictionaryValue(TipoCampo.Str, "Parametro")]
    public Dictionary<string, string?> Parametros { get; set; }

    /// <summary>
    /// Lista de url de homologação dos serviços.
    /// </summary>
    /// <value>The URL homologacao.</value>
    [DFeDictionary("UrlHomologacao", "Item")]
    [DFeDictionaryValue(TipoCampo.Str, "Url")]
    [DFeDictionaryKey(TipoCampo.Enum, "TipoUrl", false)]
    public Dictionary<TipoUrl, string> UrlHomologacao { get; set; }

    /// <summary>
    /// Lista de url de produção dos serviços.
    /// </summary>
    /// <value>The URL producao.</value>
    [DFeDictionary("UrlProducao", "Item")]
    [DFeDictionaryValue(TipoCampo.Str, "Url")]
    [DFeDictionaryKey(TipoCampo.Enum, "TipoUrl", false)]
    public Dictionary<TipoUrl, string> UrlProducao { get; set; }

    #endregion Propriedades
}