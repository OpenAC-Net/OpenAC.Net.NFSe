// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Adriano Trentim
// Created          : 01-13-2024
//
// Last Modified By : Adriano Trentim
// Last Modified On : 01-13-2024
// ***********************************************************************
// <copyright file="ProviderISSIntegra.cs" company="OpenAC .Net">
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
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderISSIntegra : ProviderABRASF
{
    #region Constructors

    public ProviderISSIntegra(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "ISSIntegra";
    }

    #endregion Constructors

    #region Methods

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }
    
    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new ISSIntegraServiceClient(this, tipo);
    }

    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet.Root);
        if (retornoWebservice.Erros.Any()) return;

        var confirmacaoCancelamento = xmlRet.Root
            .ElementAnyNs("Cancelamento")?
            .ElementAnyNs("Confirmacao");

        if (confirmacaoCancelamento == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Confirmação do cancelamento não encontrada!" });
            return;
        }

        retornoWebservice.Data = confirmacaoCancelamento.ElementAnyNs("InfConfirmacaoCancelamento").ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;
        retornoWebservice.Sucesso = retornoWebservice.Data != DateTime.MinValue;
        retornoWebservice.CodigoCancelamento = confirmacaoCancelamento.ElementAnyNs("Pedido").ElementAnyNs("InfPedidoCancelamento")
            .ElementAnyNs("CodigoCancelamento").GetValue<string>();

        var numeroNFSe = confirmacaoCancelamento.ElementAnyNs("Pedido").ElementAnyNs("InfPedidoCancelamento")?
            .ElementAnyNs("IdentificacaoNfse")?.ElementAnyNs("Numero").GetValue<string>() ?? string.Empty;

        // Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
        var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == numeroNFSe);
        if (nota == null) return;

        nota.Situacao = SituacaoNFSeRps.Cancelado;
        nota.Cancelamento.Pedido.CodigoCancelamento = retornoWebservice.CodigoCancelamento;
        nota.Cancelamento.DataHora = retornoWebservice.Data;
    }

    protected override void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet,
        string elementName = "ListaMensagemRetorno", string messageElement = "MensagemRetorno")
    {
        var listaMenssagens = xmlRet?.ElementAnyNs(elementName);
        if (listaMenssagens == null) return;

        foreach (var mensagem in listaMenssagens.ElementsAnyNs(messageElement))
        {
            var evento = new EventoRetorno
            {
                Codigo = mensagem?.ElementAnyNs("codigo")?.GetValue<string>() ?? string.Empty,
                Descricao = mensagem?.ElementAnyNs("mensagem")?.GetValue<string>() ?? string.Empty,
                Correcao = mensagem?.ElementAnyNs("correcao")?.GetValue<string>() ?? string.Empty
            };

            retornoWs.Erros.Add(evento);
        }
    }

    #endregion Methods
}