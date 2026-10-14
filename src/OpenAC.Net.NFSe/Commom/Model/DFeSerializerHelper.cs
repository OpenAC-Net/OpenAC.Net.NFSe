// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 06-19-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 08-03-2017
// ***********************************************************************
// <copyright file="DFeSerializerHelper.cs" company="OpenAC .Net">
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
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom.Types;

namespace OpenAC.Net.NFSe.Commom.Model;

public sealed partial class OpenMunicipioNFSe
{
    /// <summary>
    /// Estrutura auxiliar para conversão de valores enumerados durante a serialização.
    /// </summary>
    private readonly struct DFeEnumParseHelper
    {
        private readonly string? value;

        public DFeEnumParseHelper(string? value)
        {
            this.value = value;
        }

        public static explicit operator TipoUrl(DFeEnumParseHelper helper)
        {
            if (string.IsNullOrEmpty(helper.value)) return default;
            return Enum.TryParse<TipoUrl>(helper.value, true, out var result) ? result : default;
        }

        public static explicit operator string?(DFeEnumParseHelper helper) => helper.value;
    }

    /// <summary>
    /// Classe auxiliar para formatação e conversão de valores durante a serialização/deserialização DFe.
    /// </summary>
    private static class DFeSerializerHelper
    {
        public static string? FormatValue_Int(int val, int size, SerializerOptions? options) =>
            OpenAC.Net.DFe.Core.Serializer.DFeSerializerHelper.FormatValue_Int(val, size, options);

        public static string? FormatValue_Str(string? val, int size, SerializerOptions? options) =>
            OpenAC.Net.DFe.Core.Serializer.DFeSerializerHelper.FormatValue_Str(val, size, options);

        public static int ParseValue_Int(string? val) =>
            OpenAC.Net.DFe.Core.Serializer.DFeSerializerHelper.ParseValue_Int(val);

        public static string? ParseValue_Str(string? val) =>
            OpenAC.Net.DFe.Core.Serializer.DFeSerializerHelper.ParseValue_Str(val);

        public static DFeEnumParseHelper ParseValue_Enum(string? val) =>
            new(val);
    }
}
