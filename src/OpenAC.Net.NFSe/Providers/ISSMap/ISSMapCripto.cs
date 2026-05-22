// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : danilobreda
// Created          : 22-05-2026
//
// Last Modified By : danilobreda
// Last Modified On : 22-05-2026
// ***********************************************************************
// <copyright file="ISSMapCripto.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2026 Projeto OpenAC .Net
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
using System.Security.Cryptography;
using System.Text;

namespace OpenAC.Net.NFSe.Providers;

/// <summary>
/// Criptografia utilizada pelo provedor ISSMap (Gemmap Informática).
/// Algoritmo AES, modo ECB, padding PKCS7 (PKCS5), chave de 128 bits, saída em Base64.
/// </summary>
internal static class ISSMapCripto
{
    /// <summary>
    /// Criptografa um texto utilizando a chave de integração do contribuinte.
    /// </summary>
    /// <param name="texto">Texto em claro a ser criptografado.</param>
    /// <param name="chaveBase64">Chave de Acesso (criptografia) fornecida pelo sistema IssMap, em Base64.</param>
    /// <returns>Texto criptografado e codificado em Base64.</returns>
    public static string Criptografar(string? texto, string chaveBase64)
    {
        texto ??= string.Empty;

        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(chaveBase64);
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        var dados = Encoding.UTF8.GetBytes(texto);
        var cifrado = encryptor.TransformFinalBlock(dados, 0, dados.Length);

        return Convert.ToBase64String(cifrado);
    }

    /// <summary>
    /// Descriptografa um texto previamente cifrado pelo WebService IssMap.
    /// </summary>
    /// <param name="textoCifrado">Texto criptografado e codificado em Base64.</param>
    /// <param name="chaveBase64">Chave de Acesso (criptografia) fornecida pelo sistema IssMap, em Base64.</param>
    /// <returns>Texto em claro.</returns>
    public static string Descriptografar(string? textoCifrado, string chaveBase64)
    {
        if (string.IsNullOrEmpty(textoCifrado)) return string.Empty;

        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(chaveBase64);
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        var dados = Convert.FromBase64String(textoCifrado!);
        var decifrado = decryptor.TransformFinalBlock(dados, 0, dados.Length);

        return Encoding.UTF8.GetString(decifrado);
    }
}
