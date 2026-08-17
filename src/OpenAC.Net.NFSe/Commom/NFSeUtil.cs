// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Shared
// Author           : Rafael Dias
// Created          : 06-02-2018
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-02-2018
// ***********************************************************************
// <copyright file="NFSeUtil.cs" company="OpenAC .Net">
//		        	   The MIT License (MIT)
//	     		Copyright (c) 2014 - 2023 Projeto OpenAC .Net
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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;

namespace OpenAC.Net.NFSe.Commom;

/// <summary>
/// Métodos utilitários e extensões para manipulação de XML, codificação de caracteres e tratamento de dados para NFSe.
/// </summary>
internal static class NFSeUtil
{
    #region Fields

    private static readonly Dictionary<string, string> HtmlChars = new()
    {
        {"&", "&amp;"},
        {"<", "&lt;"},
        {">", "&gt;"},
        {"\"", "&quot;"},
        {"'", "&apos;"},
        {"#39", "&#39;"},
        {"á", "&aacute;"},
        {"Á", "&Aacute;"},
        {"â", "&acirc;"},
        {"Â", "&Acirc;"},
        {"ã", "&atilde;"},
        {"Ã", "&Atilde;"},
        {"à", "&agrave;"},
        {"À", "&Agrave;"},
        {"é", "&eacute;"},
        {"É", "&Eacute;"},
        {"ê", "&ecirc;"},
        {"Ê", "&Ecirc;"},
        {"í", "&iacute;"},
        {"Í", "&Iacute;"},
        {"ó", "&oacute;"},
        {"Ó", "&Oacute;"},
        {"õ", "&otilde;"},
        {"Õ", "&Otilde;"},
        {"ô", "&ocirc;"},
        {"Ô", "&Ocirc;"},
        {"ú", "&uacute;"},
        {"Ú", "&Uacute;"},
        {"ü", "&uuml;"},
        {"Ü", "&Uuml;"},
        {"ç", "&ccedil;"},
        {"Ç", "&Ccedil;"},
        {"º", "&ordm;"},
        {"ª", "&ordf;"},
        {"°", "&deg;"}
    };

    #endregion Fields

    #region Methods

    /// <summary>
    /// Aplica recursivamente um namespace aos elementos XML especificados, exceto os contidos em <paramref name="excludeElements"/>.
    /// </summary>
    /// <param name="parent">Elemento raiz.</param>
    /// <param name="nameSpace">Namespace a ser aplicado.</param>
    /// <param name="excludeElements">Elementos que não devem receber o namespace.</param>
    public static void ApplyNamespace(XElement? parent, XNamespace nameSpace, params string[] excludeElements)
    {
        if (parent == null) return;

        if (!excludeElements.Contains(parent.Name.LocalName))
            parent.Name = nameSpace + parent.Name.LocalName;

        foreach (var child in parent.Elements())
            ApplyNamespace(child, nameSpace, excludeElements);
    }

    /// <summary>
    /// Remove a declaração de cabeçalho XML da string informada.
    /// </summary>
    /// <param name="xml">Conteúdo XML.</param>
    /// <returns>XML sem cabeçalho de declaração.</returns>
    public static string? RemoverDeclaracaoXml(this string? xml)
    {
        if (xml == null || xml.IsEmpty()) return xml;

        var posIni = xml.IndexOf("<?", StringComparison.Ordinal);
        if (posIni < 0) return xml;

        var posFinal = xml.IndexOf("?>", StringComparison.Ordinal);
        return posFinal < 0 ? xml : xml.Remove(posIni, (posFinal + 2) - posIni);
    }

    /// <summary>
    /// Adiciona o texto codificado em HTML ao <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">Instância do StringBuilder.</param>
    /// <param name="dados">Dados a serem adicionados.</param>
    /// <returns>A própria instância do StringBuilder.</returns>
    public static StringBuilder AppendEnvio(this StringBuilder sb, string dados) => sb.Append(dados.HtmlEncode());

    /// <summary>
    /// Adiciona o texto envolvido em bloco CDATA ao <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">Instância do StringBuilder.</param>
    /// <param name="dados">Dados a serem adicionados.</param>
    /// <returns>A própria instância do StringBuilder.</returns>
    public static StringBuilder AppendCData(this StringBuilder sb, string dados) => sb.Append($"<![CDATA[{dados}]]>");

    /// <summary>
    /// Codifica caracteres especiais para entidades HTML.
    /// </summary>
    /// <param name="dados">Texto a ser codificado.</param>
    /// <returns>Texto codificado.</returns>
    public static string HtmlEncode(this string dados) => HtmlChars.Aggregate(dados, (current, htmlChar) => current.Replace(htmlChar.Key, htmlChar.Value));

    /// <summary>
    /// Decodifica entidades HTML para seus respectivos caracteres.
    /// </summary>
    /// <param name="dados">Texto codificado.</param>
    /// <returns>Texto decodificado.</returns>
    public static string HtmlDecode(this string dados) => HtmlChars.Aggregate(dados, (current, htmlChar) => current.Replace(htmlChar.Value, htmlChar.Key));

    /// <summary>
    /// Extrai o CPF ou CNPJ de um elemento XML.
    /// </summary>
    /// <param name="element">Elemento XML que contém o nó Cpf ou Cnpj.</param>
    /// <returns>CPF ou CNPJ encontrado ou string vazia.</returns>
    public static string GetCPF_CNPJ(this XElement? element)
    {
        if (element == null) return string.Empty;

        return (element.ElementAnyNs("Cnpj")?.GetValue<string>() ?? element.ElementAnyNs("Cpf")?.GetValue<string>()) ?? string.Empty;
    }

    /// <summary>
    /// Verifica se a string informada é um documento XML válido.
    /// </summary>
    /// <param name="xmlstring">String com o XML a validar.</param>
    /// <returns><c>true</c> se for um XML válido; caso contrário, <c>false</c>.</returns>
    public static bool IsValidXml(this string xmlstring)
    {
        try
        {
            if (string.IsNullOrEmpty(xmlstring))
                return false;

            _ = XDocument.Parse(xmlstring);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    #endregion Methods
}