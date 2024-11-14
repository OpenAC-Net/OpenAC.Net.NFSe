// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira/Transis
// Created          : 02-14-2020
//
// Last Modified By : Felipe Silveira/Transis
// Last Modified On : 03-27-2023
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

using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using System.Xml.Linq;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderPronim203 : ProviderABRASF203
{
    #region Constructors

    public ProviderPronim203(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "Pronim";
    }

    #endregion Constructors

    #region Methods

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new Pronim203ServiceClient(this, tipo, null);
    }
    
    protected override string GerarCabecalho()
    {
        return "<tem:cabecalho versao=\"203\">" +
               "<tem:versaoDados>2.03</tem:versaoDados>" +
               "</tem:cabecalho>";
    }

    protected override XElement WriteValoresRps(NotaServico nota)
    {
        var valores = new XElement("Valores");

        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorPis));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorCofins));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorInss));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorIr));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorCsll));
        valores.AddChild(AddTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValTotTributos", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValTotTributos));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorIss));

        if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
            valores.AddChild(AddTag(TipoCampo.De2, "", "Aliquota", 1, 5, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota));

        valores.AddChild(AddTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.DescontoIncondicionado));
        valores.AddChild(AddTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.DescontoCondicionado));

        return valores;
    }
    
    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
        //NAO PRECISA ASSINAR
    }
    
    protected override void ValidarSchema(RetornoWebservice retorno, string schema)
    {
        base.ValidarSchema(retorno, schema);
        if(retorno.Erros.Count > 0) return;

        retorno.XmlEnvio = retorno.XmlEnvio.Replace(" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"", "");
    }
 
    #endregion Methods
}