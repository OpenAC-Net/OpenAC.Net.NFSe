// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Diego Martins
// Created          : 08-30-2021
//
// Last Modified By : Rafael Dias
// Last Modified On : 27-08-2022
// ***********************************************************************
// <copyright file="NFSeRestServiceClient.cs" company="OpenAC .Net">
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Commom.Client;

/// <summary>
/// Classe base abstrata para clientes HTTP que realizam upload de arquivos multipart para serviços NFSe.
/// </summary>
public abstract class NFSeMultiPartClient : NFSeHttpServiceClient
{
    #region Inner Types

    /// <summary>
    /// Formatos de envio suportados para upload multipart.
    /// </summary>
    protected enum SendFormat
    {
        /// <summary>
        /// Envia o conteúdo como texto.
        /// </summary>
        Text,
        
        /// <summary>
        /// Envia o conteúdo como binário.
        /// </summary>
        Binary,
        
        /// <summary>
        /// Envia o conteúdo como arquivo.
        /// </summary>
        File
    }

    #endregion  Inner Types
    
    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="NFSeMultiPartClient"/>.
    /// </summary>
    /// <param name="provider">Instância do provedor de serviços.</param>
    /// <param name="tipoUrl">Tipo da URL do serviço.</param>
    protected NFSeMultiPartClient(ProviderBase provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
    {
    }

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="NFSeMultiPartClient"/> com certificado digital.
    /// </summary>
    /// <param name="provider">Instância do provedor de serviços.</param>
    /// <param name="tipoUrl">Tipo da URL do serviço.</param>
    /// <param name="certificado">Certificado digital a ser utilizado na requisição.</param>
    protected NFSeMultiPartClient(ProviderBase provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado)
    {
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Nome do campo de formulário utilizado para o arquivo no envio multipart.
    /// </summary>
    protected string FileNameForm { get; set; } = "file";
    
    /// <summary>
    /// Nome do campo de formulário utilizado para o usuário no envio multipart.
    /// </summary>
    protected string UsuarioForm { get; set; } = "login";

    /// <summary>
    /// Nome do campo de formulário utilizado para a senha no envio multipart.
    /// </summary>
    protected string SenhaForm { get; set; } = "senha";

    /// <summary>
    /// Indica se a autenticação via formulário deve ser utilizada.
    /// </summary>
    protected bool UseFormAuth { get; set; } = true;

    #endregion Properties

    #region Methods

    /// <summary>
    /// Realiza o upload de um arquivo ou mensagem para o serviço NFSe utilizando multipart/form-data.
    /// </summary>
    /// <param name="message">Conteúdo a ser enviado (texto ou caminho do arquivo, dependendo do formato).</param>
    /// <param name="contentType">Tipo de conteúdo do arquivo (padrão: text/xml).</param>
    /// <param name="sendFormat">Formato de envio do conteúdo (Texto, Binário ou Arquivo).</param>
    /// <returns>Resposta do serviço como string.</returns>
    protected string Upload(string message, string contentType = "text/xml", SendFormat sendFormat = SendFormat.Text)
    {
        var url = Url;

        try
        {
            EnvelopeEnvio = message;
            HttpContent content = sendFormat switch
            {
                SendFormat.Text => new StringContent(EnvelopeEnvio),
                SendFormat.Binary => new ByteArrayContent(Encoding.UTF8.GetBytes(EnvelopeEnvio)),
                SendFormat.File => new StreamContent(GetFileStream(message)),
                _ => throw new ArgumentException("Formato de envio inválido", nameof(sendFormat))
            };
            
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            
            if (content is ByteArrayContent arrayContent)
            {
                arrayContent.Headers.Add("Content-Transfer-Encoding", "binary");
                arrayContent.Headers.ContentEncoding.Add("Cp1252");
            }

            using var form = new MultipartFormDataContent();
            form.Add(content, FileNameForm, $"{DateTime.Now:yyyyMMddssfff}_{PrefixoEnvio}_envio.xml");

            if (AuthenticationScheme != AuthScheme.None)
            {
                var usuarioWeb = Provider.Configuracoes.WebServices.Usuario.Trim();
                Guard.Against<OpenDFeException>(usuarioWeb.IsEmpty(), "O provedor necessita que a propriedade: Configuracoes.WebServices.Usuario seja informada.");

                var senhaWeb = Provider.Configuracoes.WebServices.Senha.Trim();
                Guard.Against<OpenDFeException>(senhaWeb.IsEmpty(), "O provedor necessita que a propriedade: Configuracoes.WebServices.Senha seja informada.");
                
                if (UseFormAuth)
                {
                    form.Add(new StringContent(usuarioWeb), UsuarioForm);
                    form.Add(new StringContent(senhaWeb), SenhaForm);
                }
            }

            Execute(form, HttpMethod.Post);
            return EnvelopeRetorno;
        }
        finally
        {
            Url = url;
        }
    }
    
    private static FileStream GetFileStream(string message)
    {
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, message);
        return new FileStream(tempFile, FileMode.Open);
    }
    
    #endregion Methods
}