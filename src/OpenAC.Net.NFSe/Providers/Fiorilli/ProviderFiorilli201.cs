// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 29-01-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 29-01-2020
// ***********************************************************************
// <copyright file="ProviderFiorilli.cs" company="OpenAC .Net">
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

using OpenAC.Net.NFSe.Configuracao;
using System.Xml.Linq;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.DFe.Core.Extensions;
using OpenAC.Net.DFe.Core;
using System.IO;
using System.Linq;
using OpenAC.Net.Core.Extensions;
using System.Text;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderFiorilli201 : ProviderABRASF201
{
    #region Constructors

    public ProviderFiorilli201(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "Fiorilli";
    }

    #endregion Constructors

    #region Methods

    protected override XElement? WriteTomadorRps(NotaServico nota)
    {
        if (nota.Tomador.Endereco.CodigoMunicipio != 9999999)
            nota.Tomador.Endereco.CodigoPais = 0;

        return base.WriteTomadorRps(nota);
    }

    protected override IServiceClient GetClient(TipoUrl tipo) => new Fiorilli201ServiceClient(this, tipo);

    #endregion Methods

    #region  Services

    protected override void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.NumeroNFSe.IsEmpty())
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Número da NFSe não informado para substituição." });
        if (retornoWebservice.CodigoCancelamento.IsEmpty())
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Codigo de cancelamento não informado para substituição." });
        if (notas.Count < 1)
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Nota para subituição não informada." });

        if (retornoWebservice.Erros.Any()) return;

        var pedidoCancelamento = new StringBuilder();
        pedidoCancelamento.Append("<Pedido>");
        pedidoCancelamento.Append($"<InfPedidoCancelamento Id=\"N{retornoWebservice.NumeroNFSe}\">");
        pedidoCancelamento.Append("<IdentificacaoNfse>");
        pedidoCancelamento.Append($"<Numero>{retornoWebservice.NumeroNFSe}</Numero>");
        pedidoCancelamento.Append("<CpfCnpj>");
        pedidoCancelamento.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ() ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>" : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
        pedidoCancelamento.Append("</CpfCnpj>");

        if (!Configuracoes.PrestadorPadrao.InscricaoMunicipal.IsEmpty()) pedidoCancelamento.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");

        pedidoCancelamento.Append($"<CodigoMunicipio>{Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio}</CodigoMunicipio>");
        pedidoCancelamento.Append("</IdentificacaoNfse>");
        pedidoCancelamento.Append($"<CodigoCancelamento>{retornoWebservice.CodigoCancelamento}</CodigoCancelamento>");
        pedidoCancelamento.Append("</InfPedidoCancelamento>");
        pedidoCancelamento.Append("</Pedido>");

        var loteBuilder = new StringBuilder();
        loteBuilder.Append($"<SubstituirNfseEnvio {GetNamespace()}>");
        loteBuilder.Append($"<SubstituicaoNfse Id=\"SB{retornoWebservice.CodigoCancelamento}\">");

        loteBuilder.Append(pedidoCancelamento.ToString().RemoverDeclaracaoXml());

        var xmlRps = WriteXmlRps(notas[0], false, false);
        loteBuilder.Append(xmlRps.RemoverDeclaracaoXml());
        GravarRpsEmDisco(xmlRps, $"Rps-{notas[0].IdentificacaoRps.DataEmissao:yyyyMMdd}-{notas[0].IdentificacaoRps.Numero}.xml", notas[0].IdentificacaoRps.DataEmissao);

        loteBuilder.Append("</SubstituicaoNfse>");
        loteBuilder.Append("</SubstituirNfseEnvio>");

        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
        if(Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Producao)
            base.AssinarEnviar(retornoWebservice);
        else
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "", Certificado);
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        if(Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Producao)
            base.AssinarEnviarSincrono(retornoWebservice);
        else
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "", Certificado);
    }
    
    protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        if(Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Producao)
            base.AssinarCancelarNFSe(retornoWebservice);
    }
    
    protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice)
    {
        if(Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Producao)
            base.AssinarSubstituirNFSe(retornoWebservice);
    }
    
    #endregion Services
}