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
//	     		Copyright (c) 2014 - 2021 Projeto OpenAC .Net
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
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;

namespace OpenAC.Net.NFSe.Providers
{
    internal static class NFSeUtil
    {
        #region Fields

        private static readonly string[] escapedCharacters = { "&amp;", "&lt;", "&gt;" };
        private static readonly string[] unescapedCharacters = { "&", "<", ">", };

        #endregion Fields

        #region Methods

        public static void ApplyNamespace(XElement parent, XNamespace nameSpace, params string[] excludeElements)
        {
            if (parent == null) return;

            if (!excludeElements.Contains(parent.Name.LocalName))
                parent.Name = nameSpace + parent.Name.LocalName;

            foreach (var child in parent.Elements())
                ApplyNamespace(child, nameSpace, excludeElements);
        }

        public static string RemoverDeclaracaoXml(this string xml)
        {
            if (xml.IsEmpty()) return xml;

            var posIni = xml.IndexOf("<?", StringComparison.Ordinal);
            if (posIni < 0) return xml;

            var posFinal = xml.IndexOf("?>", StringComparison.Ordinal);
            return posFinal < 0 ? xml : xml.Remove(posIni, (posFinal + 2) - posIni);
        }

        public static StringBuilder AppendEnvio(this StringBuilder sb, string dados)
        {
            for (var i = 0; i < escapedCharacters.Length; i++)
            {
                dados = dados.Replace(unescapedCharacters[i], escapedCharacters[i]);
            }

            sb.Append(dados);
            return sb;
        }

        public static StringBuilder AppendCData(this StringBuilder sb, string dados)
        {
            sb.Append($"<![CDATA[{dados}]]>");
            return sb;
        }

        public static string AjustarString(this string dados)
        {
            for (var i = 0; i < escapedCharacters.Length; i++)
            {
                dados = dados.Replace(unescapedCharacters[i], escapedCharacters[i]);
            }

            return dados;
        }

        public static string GetCPF_CNPJ(this XElement element)
        {
            if (element == null) return string.Empty;

            return (element.ElementAnyNs("Cnpj")?.GetValue<string>() ?? element.ElementAnyNs("Cpf")?.GetValue<string>()) ?? string.Empty;
        }

        public static bool IsValidXml(this string xmlstring)
        {
            try
            {
                var xDocument = XDocument.Parse(xmlstring);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Methods
    }
}