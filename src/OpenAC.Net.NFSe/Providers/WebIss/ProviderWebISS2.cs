// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 12-24-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="ProviderWebIss2.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2023 Projeto OpenAC .Net
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

using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using System;
using System.Linq;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers;

// ReSharper disable once InconsistentNaming
internal sealed class ProviderWebIss2 : ProviderABRASF202
{
    #region Constructors

    public ProviderWebIss2(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "WebISS2";
    }

    #endregion Constructors

    #region Methods

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new WebIss2ServiceClient(this, tipo);
    }

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {

        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet, "EnviarLoteRpsSincronoResposta");
        if (retornoWebservice.Erros.Any()) return;

        retornoWebservice.Data = xmlRet.Root?.ElementAnyNs("DataRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;
                              
        MensagemErro(retornoWebservice, xmlRet, "EnviarLoteRpsSincronoResposta");

        var listaNfse = xmlRet.Root.ElementAnyNs("ListaNfse");

        if (listaNfse is null)
        {
            retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
            return;
        }

        retornoWebservice.Sucesso = true;
                
        foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
        {            
            var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
            var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            var chaveNFSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
            var dataNFSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
            var numeroRps = nfse.ElementAnyNs("DeclaracaoPrestacaoServico")?
                .ElementAnyNs("InfDeclaracaoPrestacaoServico")?
                .ElementAnyNs("Rps")?
                .ElementAnyNs("IdentificacaoRps")?
                .ElementAnyNs("Numero").GetValue<string>() ?? string.Empty;

            retornoWebservice.Protocolo = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;

            GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota == null)
            {
                nota = notas.Load(compNfse.ToString());
            }
            else
            {
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.IdentificacaoNFSe.Chave = chaveNFSe;
                nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
                nota.XmlOriginal = compNfse.ToString();
            }

            nota.Protocolo = retornoWebservice.Protocolo;
        }
    }

    #endregion Methods
}