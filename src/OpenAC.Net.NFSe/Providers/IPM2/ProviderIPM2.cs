// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 03-29-2023
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 03-29-2023
//
// ***********************************************************************
// <copyright file="ProviderIPM2.cs" company="OpenAC .Net">
//		       		   The MIT License (MIT)
//	     	Copyright (c) 2014 - 2023 Projeto OpenAC .Net
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

using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderIPM2 : ProviderBase
{
    public ProviderIPM2(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "IPM";
    }

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new IPM2ServiceClient(this, tipo);
    }

    public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
        xmlDoc.Add(WriteRps(nota));
        return xmlDoc.AsString(identado, showDeclaration);
    }

    private XElement WriteRps(NotaServico nota)
    {
        var rootRps = new XElement("Rps");

        var infServico = new XElement("InfDeclaracaoPrestacaoServico", new XAttribute("Id", $"RPS_{nota.IdentificacaoRps.Numero.OnlyNumbers()}"));
        rootRps.Add(infServico);

        infServico.Add(WriteRpsRps(nota));

        infServico.AddChild(AdicionarTag(TipoCampo.Dat, "", "Competencia", 10, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));

        infServico.AddChild(WriteServicosRps(nota));
        infServico.AddChild(WritePrestadorRps(nota));
        infServico.AddChild(WriteTomadorRps(nota));
        infServico.AddChild(WriteIntermediarioRps(nota));
        infServico.AddChild(WriteConstrucaoCivilRps(nota));

        string regimeEspecialTributacao;
        string optanteSimplesNacional;
        if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
        {
            regimeEspecialTributacao = "6";
            optanteSimplesNacional = "1";
        }
        else
        {
            regimeEspecialTributacao = ((int)nota.RegimeEspecialTributacao).ToString();
            optanteSimplesNacional = "2";
        }

        if (nota.RegimeEspecialTributacao != RegimeEspecialTributacao.Nenhum)
            infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));

        infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
        infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivoFiscal", 1, 1, Ocorrencia.Obrigatoria, nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2));

        return rootRps;
    }

    private XElement WriteRpsRps(NotaServico nota)
    {
        var rps = new XElement("Rps");

        rps.Add(WriteIdentificacaoRps(nota));

        rps.AddChild(AdicionarTag(TipoCampo.Dat, "", "DataEmissao", 10, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
        rps.AddChild(AdicionarTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Situacao + 1));

        rps.AddChild(WriteSubstituidoRps(nota));

        return rps;
    }

    private XElement WriteSubstituidoRps(NotaServico nota)
    {
        if (nota.RpsSubstituido.NumeroRps.IsEmpty()) return null;

        var rpsSubstituto = new XElement("RpsSubstituido");

        rpsSubstituto.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.RpsSubstituido.NumeroRps));
        rpsSubstituto.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Serie", 1, 15, Ocorrencia.Obrigatoria, nota.RpsSubstituido.Serie));
        rpsSubstituto.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Tipo", 1, 15, Ocorrencia.Obrigatoria, (int)nota.RpsSubstituido.Tipo + 1));

        return rpsSubstituto;
    }

    private XElement WriteIdentificacaoRps(NotaServico nota)
    {
        var indRps = new XElement("IdentificacaoRps");

        indRps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));

        var serie = nota.IdentificacaoRps.Serie;

        //Algumas prefeituras não permitem controle de série de RPS
        switch (Municipio.Codigo)
        {
            case 3551702: //Sertãozinho/SP
                serie = "00000";
                break;

            case 5208707: //Goiania/GO
                serie = "UNICA";
                break;
        }

        indRps.AddChild(AdicionarTag(TipoCampo.Str, "", "Serie", 1, 5, Ocorrencia.Obrigatoria, serie));
        indRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", 1, 1, Ocorrencia.Obrigatoria, (int)nota.IdentificacaoRps.Tipo + 1));

        return indRps;
    }

    private XElement WriteServicosRps(NotaServico nota)
    {
        var servico = new XElement("Servico");

        servico.Add(WriteValoresRps(nota));

        servico.AddChild(AdicionarTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));

        if (nota.Servico.ResponsavelRetencao.HasValue)
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "ResponsavelRetencao", 1, 1, Ocorrencia.NaoObrigatoria, (int)nota.Servico.ResponsavelRetencao + 1));

        servico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));
        //servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoCnae", 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));
        //servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
        servico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
        servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoMunicipio", 1, 20, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));
        servico.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPais", 4, 4, Ocorrencia.MaiorQueZero, nota.Servico.CodigoPais));
        servico.AddChild(AdicionarTag(TipoCampo.Int, "", "ExigibilidadeISS", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Servico.ExigibilidadeIss + 1));
        servico.AddChild(AdicionarTag(TipoCampo.Int, "", "MunicipioIncidencia", 7, 7, Ocorrencia.MaiorQueZero, nota.Servico.MunicipioIncidencia));
        servico.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroProcesso", 1, 30, Ocorrencia.NaoObrigatoria, nota.Servico.NumeroProcesso));

        return servico;
    }

    private XElement WriteValoresRps(NotaServico nota)
    {
        var valores = new XElement("Valores");

        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorDeducoes));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorPis));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorCofins));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorInss));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorIr));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorCsll));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.OutrasRetencoes));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
        valores.AddChild(AdicionarTag(TipoCampo.De4, "", "Aliquota", 1, 6, Ocorrencia.Obrigatoria, nota.Servico.Valores.Aliquota));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
        valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));

        return valores;
    }

    private XElement WritePrestadorRps(NotaServico nota)
    {
        if (nota.Prestador.CpfCnpj.IsEmpty() && nota.Prestador.InscricaoMunicipal.IsEmpty()) return null;

        var prestador = new XElement("Prestador");

        var cpfCnpjPrestador = new XElement("CpfCnpj");
        prestador.Add(cpfCnpjPrestador);

        cpfCnpjPrestador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj));

        prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Prestador.InscricaoMunicipal));
        return prestador;
    }

    private XElement WriteTomadorRps(NotaServico nota)
    {
        var tomador = new XElement("TomadorServico");

        if (!nota.Tomador.CpfCnpj.IsEmpty())
        {
            var ideTomador = new XElement("IdentificacaoTomador");
            tomador.Add(ideTomador);

            var cpfCnpjTomador = new XElement("CpfCnpj");
            ideTomador.Add(cpfCnpjTomador);

            cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

            ideTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15,
                Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));
        }

        tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));

        if (!nota.Tomador.Endereco.Logradouro.IsEmpty() ||
            !nota.Tomador.Endereco.Numero.IsEmpty() ||
            !nota.Tomador.Endereco.Complemento.IsEmpty() ||
            !nota.Tomador.Endereco.Bairro.IsEmpty() ||
            nota.Tomador.Endereco.CodigoMunicipio > 0 ||
            !nota.Tomador.Endereco.Uf.IsEmpty() ||
            nota.Tomador.Endereco.CodigoPais > 0 ||
            !nota.Tomador.Endereco.Cep.IsEmpty())
        {
            var endereco = new XElement("Endereco");
            tomador.Add(endereco);

            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
            endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoMunicipio));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
            //endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPais", 4, 4, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoPais));
            endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
        }

        if (!nota.Tomador.DadosContato.Telefone.IsEmpty() ||
            !nota.Tomador.DadosContato.Email.IsEmpty())
        {
            var contato = new XElement("Contato");
            tomador.Add(contato);

            contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Telefone", 1, 11, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));
            contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
        }

        return tomador;
    }

    private XElement WriteIntermediarioRps(NotaServico nota)
    {
        if (nota.Intermediario.CpfCnpj.IsEmpty()) return null;

        var intermediario = new XElement("Intermediario");
        var ideIntermediario = new XElement("IdentificacaoIntermediario");
        intermediario.Add(ideIntermediario);

        var cpfCnpj = new XElement("CpfCnpj");
        ideIntermediario.Add(cpfCnpj);

        cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Intermediario.CpfCnpj));

        ideIntermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Intermediario.InscricaoMunicipal));
        intermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Intermediario.RazaoSocial));

        return intermediario;
    }

    private XElement WriteConstrucaoCivilRps(NotaServico nota)
    {
        if (nota.ConstrucaoCivil.ArtObra.IsEmpty()) return null;

        var construcao = new XElement("ConstrucaoCivil");
        construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoObra", 1, 15, Ocorrencia.NaoObrigatoria, nota.ConstrucaoCivil.CodigoObra));
        construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "Art", 1, 15, Ocorrencia.Obrigatoria, nota.ConstrucaoCivil.ArtObra));

        return construcao;
    }

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.Lote == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
        if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
        foreach (var nota in notas)
        {
            if (!nota.IdentificacaoRps.Serie.IsNumeric())
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "A serie da nota precisa ser numérica. Serie informada: " + nota.IdentificacaoRps.Serie });
        }
        if (retornoWebservice.Erros.Count > 0) return;
        var xmlLoteRps = new StringBuilder();

        foreach (var nota in notas)
        {
            var xmlRps = WriteXmlRps(nota, false, false);
            xmlLoteRps.Append(xmlRps);
            GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
        }

        var xmlLote = new StringBuilder();
        xmlLote.Append($"<GerarNfseEnvio>");
        xmlLote.Append(xmlLoteRps);
        xmlLote.Append("</GerarNfseEnvio>");

        retornoWebservice.XmlEnvio = AdicionaEnvelope(xmlLote.ToString());
    }

    private string AdicionaEnvelope(string message)
    {
        var envelope = new StringBuilder();
        envelope.Append("<soapenv:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:net=\"net.atende\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");

        envelope.Append("<soapenv:Header/>");
        envelope.Append("<soapenv:Body>");
        envelope.Append(message);
        envelope.Append("</soapenv:Body>");
        envelope.Append("</soapenv:Envelope>");
        return envelope.ToString();
    }

    protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        var message = new StringBuilder();
        message.Append("<net:ConsultarNfseServicoPrestadoEnvio>");
        message.Append("<pesquisa>");
        message.Append($"<codigo_autenticidade>{retornoWebservice.Protocolo}</codigo_autenticidade>");
        message.Append("</pesquisa>");
        message.Append("</net:ConsultarNfseServicoPrestadoEnvio>");
        retornoWebservice.XmlEnvio = message.ToString();
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException();
    }

    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        var message = new StringBuilder();
        message.Append("<Cancela>");
        message.Append($"<NumeroNFSe>{retornoWebservice.NumeroNFSe}</NumeroNFSe>");
        message.Append($"<SerieNFSe>{retornoWebservice.SerieNFSe}</SerieNFSe>");
        message.Append($"<Motivo>{retornoWebservice.Motivo}</Motivo>");
        message.Append("</Cancela>");
        retornoWebservice.XmlEnvio = message.ToString();
    }

    protected override bool PrecisaValidarSchema(TipoUrl tipo)
    {
        return false;
    }

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        try
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            retornoWebservice.Data = xmlRet.Root?.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.MinValue;
            retornoWebservice.Protocolo = xmlRet.Root?.ElementAnyNs("codigo")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.Sucesso = !retornoWebservice.Protocolo.IsEmpty();

            if (!retornoWebservice.Sucesso) return;

            var numeroNFSe = xmlRet.Root.ElementAnyNs("numero_nf")?.GetValue<string>() ?? string.Empty;
            var chaveNFSe = xmlRet.Root.ElementAnyNs("codigo")?.GetValue<string>() ?? string.Empty;
            var dataNFSe = xmlRet.Root.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.Now;
            var numeroRps = xmlRet.Root.ElementAnyNs("rps")?.GetValue<string>() ?? string.Empty;

            GravarNFSeEmDisco(xmlRet.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota == null)
            {
                notas.Load(retornoWebservice.XmlRetorno);
            }
            else
            {
                nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.IdentificacaoNFSe.Chave = chaveNFSe;
                nota.XmlOriginal = retornoWebservice.XmlRetorno;
            }
        }
        catch { }
    }

    protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

        var numeroNFSe = xmlRet.Root.ElementAnyNs("numero_nf")?.GetValue<string>() ?? string.Empty;
        var chaveNFSe = xmlRet.Root.ElementAnyNs("codigo")?.GetValue<string>() ?? string.Empty;
        var dataNFSe = xmlRet.Root.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.Now;
        var numeroRps = xmlRet.Root.ElementAnyNs("rps")?.GetValue<string>() ?? string.Empty;

        GravarNFSeEmDisco(xmlRet.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

        var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
        if (nota == null)
        {
            nota = notas.Load(retornoWebservice.XmlRetorno);
        }
        else
        {
            nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
            nota.IdentificacaoNFSe.Numero = numeroNFSe;
            nota.IdentificacaoNFSe.Chave = chaveNFSe;
            nota.XmlOriginal = retornoWebservice.XmlRetorno;
        }

        retornoWebservice.Nota = nota;
        retornoWebservice.Sucesso = true;
    }

    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == retornoWebservice.NumeroNFSe);

        retornoWebservice.Sucesso = true;
        nota.Situacao = SituacaoNFSeRps.Cancelado;
        nota.Cancelamento.MotivoCancelamento = retornoWebservice.Motivo;
        nota.Cancelamento.DataHora = retornoWebservice.Data;
    }

    public override NotaServico LoadXml(XDocument xml)
    {
        Guard.Against<XmlException>(xml == null, "Xml invalido.");
        XElement notaXml = xml.ElementAnyNs("notafiscal");
        Guard.Against<XmlException>(notaXml == null, "Xml de RPS ou NFSe invalido.");

        var nota = new NotaServico(Configuracoes);

        // Nota Fiscal
        nota.IdentificacaoNFSe.Numero = notaXml.ElementAnyNs("numero_nf")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoNFSe.Chave = notaXml.ElementAnyNs("codigo")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoNFSe.DataEmissao = notaXml.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.MinValue;

        // RPS
        nota.IdentificacaoRps.Numero = notaXml.ElementAnyNs("rps")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoRps.Serie = notaXml.ElementAnyNs("serie_rps")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoRps.Tipo = TipoRps.RPS;
        nota.IdentificacaoRps.DataEmissao = notaXml.ElementAnyNs("data_emissao")?.GetValue<DateTime>(new CultureInfo("pt-BR")) ?? DateTime.MinValue;

        // Situação do RPS
        nota.Situacao = (notaXml.ElementAnyNs("cancelada")?.GetValue<string>() ?? string.Empty) == "S" ? SituacaoNFSeRps.Cancelado : SituacaoNFSeRps.Normal;

        // Serviços e Valores
        nota.Servico.Valores.ValorServicos = notaXml.ElementAnyNs("valor_servico")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorDeducoes = notaXml.ElementAnyNs("deducao")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorPis = notaXml.ElementAnyNs("valor_pis")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorCofins = notaXml.ElementAnyNs("valor_cofins")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorInss = notaXml.ElementAnyNs("valor_inss")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorIr = notaXml.ElementAnyNs("valor_irrf")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorCsll = notaXml.ElementAnyNs("valor_csll")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.IssRetido = (notaXml.ElementAnyNs("iss_retido")?.GetValue<string>() ?? string.Empty) == "S" ? SituacaoTributaria.Retencao : SituacaoTributaria.Normal;
        nota.Servico.Valores.ValorIss = notaXml.ElementAnyNs("valor_iss")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.BaseCalculo = notaXml.ElementAnyNs("valor_servico")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.Aliquota = notaXml.ElementAnyNs("aliq_iss")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorLiquidoNfse = notaXml.ElementAnyNs("valor_nf")?.GetValue<decimal>() ?? 0;
        nota.Servico.Valores.ValorIssRetido = nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? nota.Servico.Valores.ValorIss : 0;
        nota.Servico.ItemListaServico = notaXml.ElementAnyNs("id_codigo_servico")?.GetValue<string>() ?? string.Empty;
        nota.Servico.Discriminacao = notaXml.ElementAnyNs("descricao")?.GetValue<string>() ?? string.Empty;

        // Prestador
        nota.Prestador.CpfCnpj = notaXml.ElementAnyNs("cnpj_cpf_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.InscricaoMunicipal = notaXml.ElementAnyNs("im_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.RazaoSocial = notaXml.ElementAnyNs("razao_social_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Logradouro = notaXml.ElementAnyNs("endereco_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Numero = notaXml.ElementAnyNs("numero_ende_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Complemento = notaXml.ElementAnyNs("complemento_ende_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Bairro = notaXml.ElementAnyNs("bairro_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Municipio = notaXml.ElementAnyNs("cidade_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Uf = notaXml.ElementAnyNs("uf_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Cep = notaXml.ElementAnyNs("cep_prestador")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.DadosContato.Email = notaXml.ElementAnyNs("email_prestador")?.GetValue<string>() ?? string.Empty;

        // Tomador
        nota.Tomador.CpfCnpj = notaXml.ElementAnyNs("cnpj_cpf_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.InscricaoMunicipal = notaXml.ElementAnyNs("im_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.RazaoSocial = notaXml.ElementAnyNs("razao_social_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Logradouro = notaXml.ElementAnyNs("endereco_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Numero = notaXml.ElementAnyNs("numero_ende_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Complemento = notaXml.ElementAnyNs("complemento_ende_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Bairro = notaXml.ElementAnyNs("bairro_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Cep = notaXml.ElementAnyNs("cep_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Municipio = notaXml.ElementAnyNs("cidade_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Uf = notaXml.ElementAnyNs("uf_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Pais = notaXml.ElementAnyNs("pais_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.DadosContato.Telefone = notaXml.ElementAnyNs("fone_destinatario")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.DadosContato.Email = notaXml.ElementAnyNs("email_destinatario")?.GetValue<string>() ?? string.Empty;

        return nota;
    }

    protected override string GerarCabecalho()
    {
        return string.Empty;
    }

    protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
    {
        return;
    }

    protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        return;
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        //NAO PRECISA ASSINAR
        //retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "nfse", "", Certificado);
    }

    protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        var loteBuilder = new StringBuilder();

        loteBuilder.Append($"<net:ConsultarNfseServicoPrestadoEnvio>");
        loteBuilder.Append("<Prestador>");
        loteBuilder.Append("<CpfCnpj>");
        loteBuilder.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
            ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>"
            : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
        loteBuilder.Append("</CpfCnpj>");
        loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        loteBuilder.Append("</Prestador>");

        if (retornoWebservice.NumeroNFse > 0)
            loteBuilder.Append($"<NumeroNfse>{retornoWebservice.NumeroNFse}</NumeroNfse>");

        //if (retornoWebservice.Inicio.HasValue && retornoWebservice.Fim.HasValue)
        //{
        //    loteBuilder.Append("<PeriodoEmissao>");
        //    loteBuilder.Append($"<DataInicial>{retornoWebservice.Inicio:yyyy-MM-dd}</DataInicial>");
        //    loteBuilder.Append($"<DataFinal>{retornoWebservice.Fim:yyyy-MM-dd}</DataFinal>");
        //    loteBuilder.Append("</PeriodoEmissao>");
        //}

        if (!retornoWebservice.CPFCNPJTomador.IsEmpty())
        {
            loteBuilder.Append("<Tomador>");
            loteBuilder.Append("<CpfCnpj>");
            loteBuilder.Append(retornoWebservice.CPFCNPJTomador.IsCNPJ()
                ? $"<Cnpj>{retornoWebservice.CPFCNPJTomador.ZeroFill(14)}</Cnpj>"
                : $"<Cpf>{retornoWebservice.CPFCNPJTomador.ZeroFill(11)}</Cpf>");
            loteBuilder.Append("</CpfCnpj>");
            if (!retornoWebservice.IMTomador.IsEmpty()) loteBuilder.Append($"<InscricaoMunicipal>{retornoWebservice.IMTomador}</InscricaoMunicipal>");
            loteBuilder.Append("</Tomador>");
        }

        //if (!retornoWebservice.NomeIntermediario.IsEmpty() && !retornoWebservice.CPFCNPJIntermediario.IsEmpty())
        //{
        //    loteBuilder.Append("<IntermediarioServico>");
        //    loteBuilder.Append($"<RazaoSocial>{retornoWebservice.NomeIntermediario}</RazaoSocial>");
        //    loteBuilder.Append("<CpfCnpj>");
        //    loteBuilder.Append(retornoWebservice.CPFCNPJIntermediario.IsCNPJ()
        //        ? $"<Cnpj>{retornoWebservice.CPFCNPJIntermediario.ZeroFill(14)}</Cnpj>"
        //        : $"<Cpf>{retornoWebservice.CPFCNPJIntermediario.ZeroFill(11)}</Cpf>");
        //    loteBuilder.Append("</CpfCnpj>");
        //    if (!retornoWebservice.IMIntermediario.IsEmpty())
        //        loteBuilder.Append($"<InscricaoMunicipal>{retornoWebservice.IMIntermediario}</InscricaoMunicipal>");
        //    loteBuilder.Append("</IntermediarioServico>");
        //}

        loteBuilder.Append($"<Pagina>{Math.Max(retornoWebservice.Pagina, 1)}</Pagina>");
        loteBuilder.Append("</net:ConsultarNfseServicoPrestadoEnvio>");
        retornoWebservice.XmlEnvio = AdicionaEnvelope(loteBuilder.ToString());
    }

    protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
    {
        try
        {
            var xmlDoc = new XmlDocument();
            //verifica se a mensagem eh xml para exibicao correta do erro
            xmlDoc.LoadXml(retornoWebservice.XmlRetorno);
        }
        catch
        {
            retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = retornoWebservice.XmlRetorno });
            //LIMPA O XML RETORNO PARA NAO DAR ERRO DE PARSE MAIS ADIANTE
            retornoWebservice.XmlRetorno = null;
        }

        return;
    }
    protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        //NAO PRECISA ASSINAR A CONSULTA
        return;
    }
    protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice) { }

    #region Não implementados

    public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true) => throw new NotImplementedException();

    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice) => throw new NotImplementedException();

    protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) => throw new NotImplementedException();

    protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void AssinarEnviar(RetornoEnviar retornoWebservice) => throw new NotImplementedException();

    protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice) => throw new NotImplementedException();

    protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) => throw new NotImplementedException();

    protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice) => throw new NotImplementedException();

    protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice) => throw new NotImplementedException();

    protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice) => throw new NotImplementedException();

    protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) => throw new NotImplementedException();

    protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException();

    protected override string GetSchema(TipoUrl tipo) => throw new NotImplementedException();

    #endregion Não implementados
}