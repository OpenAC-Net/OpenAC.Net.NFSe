// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 01-31-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-07-2016
// ***********************************************************************
// <copyright file="ConfigWebServicesNFSe.cs" company="OpenAC .Net">
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
using System.Linq;
using System.Net;
using OpenAC.Net.Core;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Configuracao;

/// <summary>
/// Configurações de comunicação com os webservices de NFSe dos municípios e provedores.
/// </summary>
public sealed class ConfigWebServicesNFSe : DFeWebserviceConfigBase
{
    #region Fields

    private int codigoMunicipio;

    #endregion Fields

    #region Constructor

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="ConfigWebServicesNFSe"/>.
    /// </summary>
    internal ConfigWebServicesNFSe()
    {
        Usuario = string.Empty;
        Senha = string.Empty;
        FraseSecreta = string.Empty;
        ChaveAcesso = string.Empty;
        Protocolos = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
    }

    #endregion Constructor

    #region Properties

    /// <summary>
    /// Nome do município do webservice em uso.
    /// </summary>
    /// <value>O nome do município.</value>
    public string Municipio { get; private set; }

    /// <summary>
    /// Obtém o provedor de NFSe correspondente ao município selecionado.
    /// </summary>
    public NFSeProvider Provider { get; private set; } = NFSeProvider.Nenhum;

    /// <summary>
    /// Obtém ou define o layout específico do webservice de São Paulo (quando aplicável).
    /// </summary>
    public LayoutISSSaoPaulo LayoutISSSaoPaulo { get; set; }

    /// <summary>
    /// Obtém ou define o usuário de autenticação no webservice.
    /// </summary>
    public string Usuario { get; set; }

    /// <summary>
    /// Obtém ou define a senha de autenticação no webservice.
    /// </summary>
    public string Senha { get; set; }

    /// <summary>
    /// Obtém ou define a frase secreta utilizada por alguns provedores para autenticação/assinatura.
    /// </summary>
    public string FraseSecreta { get; set; }

    /// <summary>
    /// Obtém ou define a chave de acesso (API Key / Token) para autenticação em provedores REST/SOAP.
    /// </summary>
    public string ChaveAcesso { get; set; }

    /// <summary>
    /// Obtém ou define a chave privada em formato texto, quando exigida pelo provedor.
    /// </summary>
    public string ChavePrivada { get; set; }

    /// <summary>
    /// Obtém ou define o endereço do servidor proxy (se utilizado).
    /// </summary>
    public string Proxy { get; set; }

    /// <summary>
    /// Código IBGE do município dos webservices em uso.
    /// </summary>
    /// <value>O código IBGE do município.</value>
    public int CodigoMunicipio
    {
        get => codigoMunicipio;
        set
        {
            if (codigoMunicipio == value) return;

            var municipio = ProviderManager.Municipios.SingleOrDefault(x => x.Codigo == value);
            Guard.Against<ArgumentException>(municipio == null, "Município não cadastrado.");

            codigoMunicipio = value;
            Municipio = municipio?.Nome ?? string.Empty;
            Provider = municipio?.Provedor ?? NFSeProvider.Nenhum;
        }
    }

    #endregion Properties
}
