// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 12-26-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 12-26-2017
// ***********************************************************************
// <copyright file="ProviderThema.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2021 Projeto OpenAC .Net
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
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.Providers.Thema;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderThema : ProviderABRASF
{
    #region Constructors

    public ProviderThema(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "Thema";
    }

    #endregion Constructors

    #region Methods

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor.");
    }

    protected override string GetNamespace()
    {
        return "xmlns=\"http://www.abrasf.org.br/ABRASF/arquivos/nfse.xsd\"";
    }

    /// <inheritdoc />
    protected override string GerarCabecalho()
    {
        return "xmlns=\"http://server.nfse.thema.inf.br\"";
    }

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new ThemaServiceClient(this, tipo);
    }

    protected override string GetSchema(TipoUrl tipo)
    {
        return "nfse.xsd";
    }

    protected override void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string elementName = "ListaMensagemRetorno", string messageElement = "MensagemRetorno")
    {
        var listaMenssagens = xmlRet?.ElementAnyNs(elementName);
        if (listaMenssagens == null) return;

        foreach (var mensagem in listaMenssagens.ElementsAnyNs(messageElement))
        {
            var evento = new Evento
            {
                Codigo = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty,
                Descricao = mensagem?.ElementAnyNs("Mensagem")?.GetValue<string>() ?? string.Empty,
                Correcao = mensagem?.ElementAnyNs("Correcao")?.GetValue<string>() ?? string.Empty
            };

            if (new[] { evento.Codigo, evento.Descricao }.All(s => !string.IsNullOrWhiteSpace(s)))
                retornoWs.Erros.Add(evento);
        }
    }

    #endregion Methods
}