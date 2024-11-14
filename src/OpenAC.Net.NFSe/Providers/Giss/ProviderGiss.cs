// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 28-07-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="ProviderGiss.cs" company="OpenAC .Net">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderGiss : ProviderABRASF204
{
    #region Internal Types

    private enum LoadXmlFormato
    {
        Indefinido,
        NFSe,
        Rps
    }

    #endregion Internal Types

    #region Constructors

    public ProviderGiss(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "Giss";
    }

    #endregion Constructors

    #region Methods

    protected override IServiceClient GetClient(TipoUrl tipo) => new GissServiceClient(this, tipo);

    protected override string GerarCabecalho()
    {
       return "<p:cabecalho versao=\"2.00\" xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\" xmlns:p=\"http://www.giss.com.br/cabecalho-v2_04.xsd\" xmlns:p1=\"http://www.giss.com.br/tipos-v2_04.xsd\" " +
              "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"> <p:versaoDados>2.00</p:versaoDados> </p:cabecalho>";
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "InfDeclaracaoPrestacaoServico", Certificado);
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsSincronoEnvio", "LoteRps", Certificado);
    }

    protected override void AssinarGerarNfse(RetornoGerarNfse retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "InfDeclaracaoPrestacaoServico", Certificado);
    }

    protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "can:Pedido", "tip:InfPedidoCancelamento", Certificado);
    }

    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        if (retornoWebservice.NumeroNFSe.IsEmpty() || retornoWebservice.CodigoCancelamento.IsEmpty())
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "AC0001", Descricao = "Número da NFSe/Codigo de cancelamento não informado para cancelamento." });
            return;
        }

        var loteBuilder = new StringBuilder();

        loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        loteBuilder.Append("<can:CancelarNfseEnvio xmlns:can=\"http://www.giss.com.br/cancelar-nfse-envio-v2_04.xsd\" xmlns:tip=\"http://www.giss.com.br/tipos-v2_04.xsd\" xmlns:xd=\"http://www.w3.org/2000/09/xmldsig#\">");
        loteBuilder.Append("<can:Pedido>");
        loteBuilder.Append($"<tip:InfPedidoCancelamento Id=\"N{retornoWebservice.NumeroNFSe}\">");
        loteBuilder.Append("<tip:IdentificacaoNfse>");
        loteBuilder.Append($"<tip:Numero>{retornoWebservice.NumeroNFSe}</tip:Numero>");
        loteBuilder.Append("<tip:CpfCnpj>");
        loteBuilder.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
            ? $"<tip:Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tip:Cnpj>"
            : $"<tip:Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</tip:Cpf>");
        loteBuilder.Append("</tip:CpfCnpj>");
        if (!Configuracoes.PrestadorPadrao.InscricaoMunicipal.IsEmpty()) loteBuilder.Append($"<tip:InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</tip:InscricaoMunicipal>");
        loteBuilder.Append($"<tip:CodigoMunicipio>{Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio}</tip:CodigoMunicipio>");
        loteBuilder.Append("</tip:IdentificacaoNfse>");
        loteBuilder.Append($"<tip:CodigoCancelamento>{retornoWebservice.CodigoCancelamento}</tip:CodigoCancelamento>");
        loteBuilder.Append("</tip:InfPedidoCancelamento>");
        loteBuilder.Append("</can:Pedido>");
        loteBuilder.Append("</can:CancelarNfseEnvio>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    #endregion Methods
}