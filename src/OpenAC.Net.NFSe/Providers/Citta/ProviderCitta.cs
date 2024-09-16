// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 26-08-2021
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 26-08-2021
// ***********************************************************************
// <copyright file="ProviderISSe.cs" company="OpenAC .Net">
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

using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderCitta : ProviderABRASF203
{
    #region Constructors

    public ProviderCitta(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "Citta";
    }

    #endregion Constructors

    #region Methods

    #region Protected Methods

    protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        var loteBuilder = new StringBuilder();
        loteBuilder.Append($"<nfse:ConsultarLoteRpsEnvio xmlns:nfse=\"http://nfse.abrasf.org.br\">");
        loteBuilder.Append("<Prestador>");
        loteBuilder.Append("<CpfCnpj>");
        loteBuilder.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
            ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>"
            : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
        loteBuilder.Append("</CpfCnpj>");
        loteBuilder.Append("</Prestador>");
        loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");
        loteBuilder.Append("</nfse:ConsultarLoteRpsEnvio>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.NumeroRps < 1)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Número da RPS não informado para a consulta." });
            return;
        }

        var loteBuilder = new StringBuilder();
        loteBuilder.Append($"<nfse:ConsultarNfseRpsEnvio xmlns:nfse=\"http://nfse.abrasf.org.br\">");
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
        loteBuilder.Append("</Prestador>");
        loteBuilder.Append("</nfse:ConsultarNfseRpsEnvio>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (notas.Count == 0) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Nenhuma RPS informada." });
        if (notas.Count > 1) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Apenas uma RPS pode ser enviada em modo Sincrono." });
        if (retornoWebservice.Erros.Count > 0) return;

        var xmlLoteRps = new StringBuilder();

        foreach (var nota in notas)
        {
            var xmlRps = WriteXmlRps(nota, false, false);
            GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            xmlLoteRps.Append(xmlRps);
        }

        var xmlLote = new StringBuilder();
        xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        xmlLote.Append("<nfse:RecepcionarLoteRpsSincronoEnvio xmlns:nfse=\"http://nfse.abrasf.org.br\" xmlns:nfse1=\"http://nfse.citta.com.br\">");
        xmlLote.Append($"<LoteRps Id=\"L{retornoWebservice.Lote}\" {GetVersao()}>");
        xmlLote.Append($"<NumeroLote>{retornoWebservice.Lote}</NumeroLote>");
        if (UsaPrestadorEnvio) xmlLote.Append("<Prestador>");
        xmlLote.Append("<CpfCnpj>");
        xmlLote.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
            ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>"
            : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
        xmlLote.Append("</CpfCnpj>");
        if (!Configuracoes.PrestadorPadrao.InscricaoMunicipal.IsEmpty()) xmlLote.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        if (UsaPrestadorEnvio) xmlLote.Append("</Prestador>");
        xmlLote.Append($"<QuantidadeRps>{notas.Count}</QuantidadeRps>");
        xmlLote.Append("<ListaRps>");
        xmlLote.Append(xmlLoteRps);//.Replace("InfDeclaracaoPrestacaoServico", "nfse1:InfDeclaracaoPrestacaoServico")
        xmlLote.Append("</ListaRps>");
        xmlLote.Append("</LoteRps>");
        xmlLote.Append("</nfse:RecepcionarLoteRpsSincronoEnvio>");
        string xml = xmlLote.ToString();
        retornoWebservice.XmlEnvio = xml;
    }

    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
        //retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsEnvio", "LoteRps", Certificado);
        //NAO PRECISA ASSINAR
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        //retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "nfse:RecepcionarLoteRpsSincronoEnvio", "LoteRps", Certificado);
        //NAO PRECISA ASSINAR
    }

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new CittaServiceClient(this, tipo);
    }

    protected override bool PrecisaValidarSchema(TipoUrl tipo)
    {
        //NAO CONSEGUI OBTER OS SCHEMAS DESTE PROVEDOR ATE O DIA 24/08/21
        return false;
    }

    #endregion Protected Methods

    #endregion Methods
}