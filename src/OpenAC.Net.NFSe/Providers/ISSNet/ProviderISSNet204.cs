// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-22-2018
//
// Last Modified By : Leandro Rossi (rossism.com.br)
// Last Modified On : 14-04-2023
// ***********************************************************************
// <copyright file="ProviderISSNet.cs" company="OpenAC .Net">
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
using OpenAC.Net.DFe.Core.Extensions;
using OpenAC.Net.NFSe.Configuracao;
using System.Text;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.DFe.Core;
using System.Linq;
using System.Xml.Linq;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Commom;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderISSNet204 : ProviderABRASF204
{
    #region Constructors

    public ProviderISSNet204(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "ISSNet";
    }

    #endregion Constructors

    #region Methods

    protected override IServiceClient GetClient(TipoUrl tipo) => new ISSNet204ServiceClient(this, tipo, Certificado);

    protected override string GetSchema(TipoUrl tipo) => "nfse.xsd";

    protected override string GerarCabecalho() => $"<cabecalho versao=\"2.04\" {GetNamespace()}><versaoDados>{Versao.GetDFeValue()}</versaoDados></cabecalho>";

    #endregion Methods

    #region Services 

    protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "ConsultarNfseRpsEnvio", "", Certificado);
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.NumeroRps < 1)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Número da RPS não informado para a consulta." });
            return;
        }

        var loteBuilder = new StringBuilder();
        loteBuilder.Append($"<ConsultarNfseRpsEnvio {GetNamespace()}>");
        loteBuilder.Append("<Pedido>");
        loteBuilder.Append("<IdentificacaoRps>");
        loteBuilder.Append($"<Numero>{retornoWebservice.NumeroRps}</Numero>");
        loteBuilder.Append($"<Serie>{retornoWebservice.Serie}</Serie>");
        loteBuilder.Append($"<Tipo>{(int)retornoWebservice.Tipo + 1}</Tipo>");
        loteBuilder.Append("</IdentificacaoRps>");
        loteBuilder.Append("<Prestador>");
        loteBuilder.Append("<CpfCnpj>");
        loteBuilder.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
            ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>"
            : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
        loteBuilder.Append("</CpfCnpj>");
        if (!Configuracoes.PrestadorPadrao.InscricaoMunicipal.IsEmpty()) loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        loteBuilder.Append("</Prestador>");
        loteBuilder.Append("</Pedido>");
        loteBuilder.Append("</ConsultarNfseRpsEnvio>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet, "SubstituirNfseResult");
        if (retornoWebservice.Erros.Any()) return;

        var retornoLote = xmlRet.Root.ElementAnyNs("RetSubstituicao");
        var nfseSubstituida = retornoLote?.ElementAnyNs("NfseSubstituida");
        var nfseSubstituidora = retornoLote?.ElementAnyNs("NfseSubstituidora");

        if (nfseSubstituida == null) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "NFSe Substituida não encontrada! (NfseSubstituida)" });
        if (nfseSubstituidora == null) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "NFSe Substituidora não encontrada! (NfseSubstituidora)" });
        if (retornoWebservice.Erros.Any()) return;


        /******* TRATANDO A NOTA SUBSTITUÍDA *******/
        var compNfse = nfseSubstituida.ElementAnyNs("CompNfse");
        if (compNfse == null) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "NFSe não encontrada! (CompNfse)" });
        if (retornoWebservice.Erros.Any()) return;

        var notaSubistituida = LoadXml(compNfse.ToString());

        var notaSubistituidaExistente = notas.FirstOrDefault(t => t.IdentificacaoRps.Numero == notaSubistituida.IdentificacaoRps.Numero);
        if (notaSubistituidaExistente == null)
        {
            notaSubistituidaExistente = notaSubistituida;
            notas.Add(notaSubistituidaExistente);
        }
        else
        {
            notaSubistituidaExistente.IdentificacaoNFSe.Numero = notaSubistituida.IdentificacaoNFSe.Numero;
            notaSubistituidaExistente.IdentificacaoNFSe.Chave = notaSubistituida.IdentificacaoNFSe.Chave;
            notaSubistituidaExistente.IdentificacaoNFSe.DataEmissao = notaSubistituida.IdentificacaoNFSe.DataEmissao;
            notaSubistituidaExistente.XmlOriginal = compNfse.ToString();
        }

        /******* TRATANDO A NOTA SUBSTITUIDORA *******/
        compNfse = nfseSubstituidora.ElementAnyNs("CompNfse");
        if (compNfse == null) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "NFSe não encontrada! (CompNfse)" });
        if (retornoWebservice.Erros.Any()) return;

        var notaSubistituidora = LoadXml(compNfse.ToString());

        var notaSubistituidoraExistente = notas.FirstOrDefault(t => t.IdentificacaoRps.Numero == notaSubistituidora.IdentificacaoRps.Numero);
        if (notaSubistituidoraExistente == null)
        {
            notaSubistituidoraExistente = notaSubistituidora;
            notas.Add(notaSubistituidoraExistente);
        }
        else
        {
            notaSubistituidoraExistente.IdentificacaoNFSe.Numero = notaSubistituidora.IdentificacaoNFSe.Numero;
            notaSubistituidoraExistente.IdentificacaoNFSe.Chave = notaSubistituidora.IdentificacaoNFSe.Chave;
            notaSubistituidoraExistente.IdentificacaoNFSe.DataEmissao = notaSubistituidora.IdentificacaoNFSe.DataEmissao;
            notaSubistituidoraExistente.XmlOriginal = compNfse.ToString();
        }

        /******* TRATAMENTOS FINAIS *******/
        retornoWebservice.Sucesso = true;

        notaSubistituidoraExistente.RpsSubstituido.NFSeSubstituidora = notaSubistituidoraExistente.IdentificacaoNFSe.Numero;
        notaSubistituidoraExistente.RpsSubstituido.NumeroNfse = notaSubistituidaExistente.IdentificacaoNFSe.Numero;
        notaSubistituidoraExistente.RpsSubstituido.DataEmissaoNfseSubstituida = notaSubistituidaExistente.IdentificacaoNFSe.DataEmissao;
        notaSubistituidoraExistente.RpsSubstituido.Id = notaSubistituidaExistente.Id;
        notaSubistituidoraExistente.RpsSubstituido.NumeroRps = notaSubistituidaExistente.IdentificacaoRps.Numero;
        notaSubistituidoraExistente.RpsSubstituido.Serie = notaSubistituidaExistente.IdentificacaoRps.Serie;
        notaSubistituidoraExistente.RpsSubstituido.Signature = notaSubistituidaExistente.Signature;

        retornoWebservice.Nota = notaSubistituidoraExistente;
    }

    #endregion
}