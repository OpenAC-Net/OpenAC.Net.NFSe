// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 17-02-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 17-02-2020
// ***********************************************************************
// <copyright file="ProviderSimplISS.cs" company="OpenAC .Net">
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
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers
{
    internal sealed class ProviderSimplISS : ProviderABRASF
    {
        #region Constructors

        public ProviderSimplISS(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "SimplISS";
        }

        #endregion Constructors

        protected override void LoadServicosValoresRps(NotaServico nota, XElement rootNFSe)
        {
            base.LoadServicosValoresRps(nota, rootNFSe);
            var rootServico = rootNFSe.ElementAnyNs("Servico");
            if (rootServico == null) return;

            var items = rootServico.ElementsAnyNs("ItensServico");
            foreach (var item in items)
            {
                var servico = nota.Servico.ItensServico.AddNew();
                servico.Descricao = item.ElementAnyNs("Descricao")?.GetValue<string>() ?? "";
                servico.Quantidade = item.ElementAnyNs("Quantidade")?.GetValue<decimal>() ?? 0;
                servico.ValorUnitario = item.ElementAnyNs("ValorUnitario")?.GetValue<decimal>() ?? 0;

                var trib = item.ElementAnyNs("IssTributavel")?.GetValue<int>() ?? 2;
                servico.Tributavel = trib == 1 ? NFSeSimNao.Sim : NFSeSimNao.Nao;
            }
        }

        protected override XElement WriteServicosValoresRps(NotaServico nota)
        {
            var servico = base.WriteServicosValoresRps(nota);
            foreach (var item in nota.Servico.ItensServico)
            {
                var itemServico = new XElement("ItensServico");
                itemServico.AddChild(AdicionarTag(TipoCampo.Str, "", "Descricao", 1, 100, Ocorrencia.Obrigatoria, item.Descricao));
                itemServico.AddChild(AdicionarTag(TipoCampo.De2, "", "Quantidade", 4, 15, Ocorrencia.Obrigatoria, item.Quantidade));
                itemServico.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorUnitario", 4, 15, Ocorrencia.Obrigatoria, item.ValorUnitario));
                itemServico.AddChild(AdicionarTag(TipoCampo.Int, "", "IssTributavel", 4, 15, Ocorrencia.NaoObrigatoria, item.Tributavel == NFSeSimNao.Sim ? 1 : 2));

                servico.AddChild(itemServico);
            }

            return servico;
        }

        #region Methods

        protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

        protected override string GetNamespace() => "";

        protected override IServiceClient GetClient(TipoUrl tipo) => new SimplISSServiceClient(this, tipo);

        protected override string GetSchema(TipoUrl tipo) => "nfse_3.xsd";

        #endregion Methods
    }
}