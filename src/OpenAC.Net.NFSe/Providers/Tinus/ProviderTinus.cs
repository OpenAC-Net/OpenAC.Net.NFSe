// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Carlos Machado
// Created          : 11-05-2026
//
// Last Modified By : Carlos Machado
// Last Modified On : 11-05-2026
// ***********************************************************************
// <copyright file="ProviderTinus.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		Copyright (c) 2014 - 2026 Projeto OpenAC .Net
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
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderTinus : ProviderABRASF203
{
    #region Constructors

    public ProviderTinus(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "Tinus";
    }

    #endregion Constructors

    #region Protected Methods

    /// <inheritdoc />
    public override NotaServico LoadXml(XDocument xml)
    {
        Guard.Against<XmlException>(xml == null, "Xml invalido.");

        var rootGrupo = xml.ElementAnyNs("CompNfse");
        if (rootGrupo != null)
        {
            var nfseElement = rootGrupo.ElementAnyNs("Nfse");
            var infNFSeNacional = nfseElement?.ElementAnyNs("infNFSe");
            if (infNFSeNacional != null)
                return LoadXmlNacionalTinus(xml, infNFSeNacional);
        }

        return base.LoadXml(xml);
    }

    private NotaServico LoadXmlNacionalTinus(XDocument xml, XElement infNFSe)
    {
        var ret = new NotaServico(Configuracoes)
        {
            XmlOriginal = xml.AsString()
        };

        ret.IdentificacaoNFSe.Numero = infNFSe.ElementAnyNs("nNFSe")?.GetValue<string>() ?? string.Empty;
        ret.IdentificacaoNFSe.DataEmissao = infNFSe.ElementAnyNs("dhProc")?.GetValue<DateTime>() ?? DateTime.MinValue;

        // A chave da NFS-e nacional est� no atributo Id (ex: "NFS26079011...").
        // O ADNChave para cancelamento usa os 50 d�gitos sem o prefixo "NFS".
        var nfseId = infNFSe.Attribute("Id")?.Value ?? string.Empty;
        ret.IdentificacaoNFSe.Chave = nfseId.StartsWith("NFS", StringComparison.OrdinalIgnoreCase)
            ? nfseId.Substring(3)
            : nfseId;

        var infDPS = infNFSe.ElementAnyNs("DPS")?.ElementAnyNs("infDPS");
        if (infDPS != null)
        {
            ret.IdentificacaoRps.Numero = infDPS.ElementAnyNs("nDPS")?.GetValue<string>() ?? string.Empty;
            ret.IdentificacaoRps.Serie = infDPS.ElementAnyNs("serie")?.GetValue<string>() ?? string.Empty;
            ret.Competencia = infDPS.ElementAnyNs("dCompet")?.GetValue<DateTime>() ?? DateTime.MinValue;

            var prest = infDPS.ElementAnyNs("prest");
            if (prest != null)
            {
                ret.Prestador.CpfCnpj = prest.ElementAnyNs("CNPJ")?.GetValue<string>()
                                      ?? prest.ElementAnyNs("CPF")?.GetValue<string>()
                                      ?? string.Empty;
                ret.Prestador.InscricaoMunicipal = prest.ElementAnyNs("IM")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.DadosContato.Email = prest.ElementAnyNs("email")?.GetValue<string>() ?? string.Empty;
            }

            var toma = infDPS.ElementAnyNs("toma");
            if (toma != null)
            {
                ret.Tomador.CpfCnpj = toma.ElementAnyNs("CNPJ")?.GetValue<string>()
                                    ?? toma.ElementAnyNs("CPF")?.GetValue<string>()
                                    ?? string.Empty;
                ret.Tomador.RazaoSocial = toma.ElementAnyNs("xNome")?.GetValue<string>() ?? string.Empty;
                ret.Tomador.DadosContato.Email = toma.ElementAnyNs("email")?.GetValue<string>() ?? string.Empty;

                var endNac = toma.ElementAnyNs("end")?.ElementAnyNs("endNac");
                if (endNac != null)
                {
                    ret.Tomador.Endereco.CodigoMunicipio = endNac.ElementAnyNs("cMun")?.GetValue<int>() ?? 0;
                    ret.Tomador.Endereco.Cep = endNac.ElementAnyNs("CEP")?.GetValue<string>() ?? string.Empty;
                }

                var endToma = toma.ElementAnyNs("end");
                if (endToma != null)
                {
                    ret.Tomador.Endereco.Logradouro = endToma.ElementAnyNs("xLgr")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Numero = endToma.ElementAnyNs("nro")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Bairro = endToma.ElementAnyNs("xBairro")?.GetValue<string>() ?? string.Empty;
                }
            }

            var serv = infDPS.ElementAnyNs("serv");
            if (serv != null)
            {
                ret.Servico.CodigoMunicipio = serv.ElementAnyNs("locPrest")
                    ?.ElementAnyNs("cLocPrestacao")?.GetValue<int>() ?? 0;

                var cServ = serv.ElementAnyNs("cServ");
                if (cServ != null)
                {
                    ret.Servico.CodigoTributacaoMunicipio = cServ.ElementAnyNs("cTribNac")?.GetValue<string>() ?? string.Empty;
                    ret.Servico.Discriminacao = cServ.ElementAnyNs("xDescServ")?.GetValue<string>() ?? string.Empty;
                }
            }

            var valoresDPS = infDPS.ElementAnyNs("valores");
            if (valoresDPS != null)
                ret.Servico.Valores.ValorServicos = valoresDPS.ElementAnyNs("vServPrest")
                    ?.ElementAnyNs("vServ")?.GetValue<decimal>() ?? 0;
        }

        var valoresNFSe = infNFSe.ElementAnyNs("valores");
        if (valoresNFSe != null)
        {
            ret.Servico.Valores.BaseCalculo = valoresNFSe.ElementAnyNs("vBC")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.ValorLiquidoNfse = valoresNFSe.ElementAnyNs("vLiq")?.GetValue<decimal>() ?? 0;
        }

        return ret;
    }

    /// <inheritdoc />
    protected override IServiceClient GetClient(TipoUrl tipo) => new TinusServiceClient(this, tipo);

    /// <inheritdoc />
    protected override string GetSchema(TipoUrl tipo) => "nfse.xsd";

    /// <inheritdoc />
    protected override string GetNamespace() =>
        "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
        "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
        "xmlns=\"http://www.abrasf.org.br/nfse.xsd\"";

    /// <inheritdoc />
    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "InfDeclaracaoPrestacaoServico", Certificado);
    }

    /// <inheritdoc />
    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "InfDeclaracaoPrestacaoServico", Certificado);
    }

    /// <inheritdoc />
    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

        MensagemErro(retornoWebservice, xmlRet, "EnviarLoteRpsSincronoResposta");
        if (retornoWebservice.Erros.Any()) return;

        retornoWebservice.Data = xmlRet.Root?.ElementAnyNs("DataRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;
        retornoWebservice.Protocolo = xmlRet.Root?.ElementAnyNs("Protocolo")?.GetValue<string>() ?? string.Empty;
        retornoWebservice.Sucesso = !retornoWebservice.Protocolo.IsEmpty();
        MensagemErro(retornoWebservice, xmlRet, "EnviarLoteRpsSincronoResposta");

        if (!retornoWebservice.Sucesso) return;

        var listaNfse = xmlRet.Root?.ElementAnyNs("ListaNfse");
        if (listaNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Lista de NFSe n�o encontrada! (ListaNfse)" });
            return;
        }

        foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
        {
            var infNFSe = compNfse.ElementAnyNs("Nfse")?.ElementAnyNs("infNFSe");
            if (infNFSe == null) continue;

            var numeroNFSe = infNFSe.ElementAnyNs("nNFSe")?.GetValue<string>() ?? string.Empty;
            var dataNFSe = infNFSe.ElementAnyNs("dhProc")?.GetValue<DateTime>() ?? DateTime.Now;

            var infDPS = infNFSe.ElementAnyNs("DPS")?.ElementAnyNs("infDPS");
            var numeroRps = infDPS?.ElementAnyNs("nDPS")?.GetValue<string>() ?? string.Empty;

            GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-.xml", dataNFSe);

            // A chave da NFS-e nacional est� no atributo Id do infNFSe (ex: "NFS26079011...").
            var chaveNFSe = (infNFSe.Attribute("Id")?.Value ?? string.Empty);
            if (chaveNFSe.StartsWith("NFS", StringComparison.OrdinalIgnoreCase))
                chaveNFSe = chaveNFSe.Substring(3);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota == null)
            {
                notas.Load(compNfse.ToString());
            }
            else
            {
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.IdentificacaoNFSe.Chave = chaveNFSe;
                nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
                nota.XmlOriginal = compNfse.AsString();
            }
        }
    }

    /// <inheritdoc />
    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        if (retornoWebservice.NumeroNFSe.IsEmpty() || retornoWebservice.CodigoCancelamento.IsEmpty())
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "AC0001", Descricao = "N�mero da NFSe/C�digo de cancelamento n�o informado para cancelamento." });
            return;
        }

        // O Tinus exige ADNCodMotivo com valores "1" (erro na emiss�o), "2" (servi�o n�o prestado) ou "9" (outros).
        // Mapeia a partir do CodigoCancelamento ABRASF padr�o.
        var adnCodMotivo = retornoWebservice.CodigoCancelamento switch
        {
            "1" => "1",
            "2" => "2",
            _   => "9"
        };

        var loteBuilder = new StringBuilder();
        loteBuilder.Append($"<CancelarNfseEnvio {GetNamespace()}>");
        loteBuilder.Append("<Pedido>");
        loteBuilder.Append($"<InfPedidoCancelamento Id=\"N{retornoWebservice.NumeroNFSe}\">");
        loteBuilder.Append("<IdentificacaoNfse>");
        loteBuilder.Append($"<Numero>{retornoWebservice.NumeroNFSe}</Numero>");
        loteBuilder.Append("<CpfCnpj>");
        loteBuilder.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
            ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>"
            : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
        loteBuilder.Append("</CpfCnpj>");
        if (!Configuracoes.PrestadorPadrao.InscricaoMunicipal.IsEmpty())
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
        loteBuilder.Append($"<CodigoMunicipio>{Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio}</CodigoMunicipio>");
        loteBuilder.Append("</IdentificacaoNfse>");
        loteBuilder.Append($"<CodigoCancelamento>{retornoWebservice.CodigoCancelamento}</CodigoCancelamento>");
        if (!retornoWebservice.CodigoVerificacao.IsEmpty())
            loteBuilder.Append($"<ADNChave>{retornoWebservice.CodigoVerificacao}</ADNChave>");
        loteBuilder.Append($"<ADNCodMotivo>{adnCodMotivo}</ADNCodMotivo>");
        loteBuilder.Append($"<ADNMotivo>{retornoWebservice.Motivo}</ADNMotivo>");
        loteBuilder.Append("</InfPedidoCancelamento>");
        loteBuilder.Append("</Pedido>");
        loteBuilder.Append("</CancelarNfseEnvio>");

        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    /// <inheritdoc />
    protected override XElement WriteRps(NotaServico nota)
    {
        var rps = base.WriteRps(nota);

        var infServico = rps.ElementAnyNs("InfDeclaracaoPrestacaoServico");
        infServico?.AddChild(AddTag(TipoCampo.Int, "", "regApTribSN", 1, 1, Ocorrencia.Obrigatoria, 1));
        infServico?.AddChild(WriteInfoIBSCBSRps(nota));

        rps.Add(WriteIBSCBSTotalRps(nota));

        return rps;
    }

    /// <summary>
    /// Escreve o grupo IBSCBS (TCRTCIBSCBS) como elemento raiz, irmão de InfDeclaracaoPrestacaoServico.
    /// </summary>
    private XElement? WriteIBSCBSTotalRps(NotaServico nota)
    {
        if (nota.Servico.Valores.IBSCBS is null) return null;

        var total = nota.IBSCBSTotal;

        var ibsCbs = new XElement("IBSCBS");
        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "cLocalidadeIncid", 7, 7, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));
        ibsCbs.AddChild(AddTag(TipoCampo.Str, "", "xLocalidadeIncid", 1, 600, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.Municipio));

        var valores = new XElement("valores");
        valores.AddChild(AddTag(TipoCampo.De2, "", "vBC", 1, 15, Ocorrencia.Obrigatoria, total.Valores.ValorBaseCalculo));

        var uf = new XElement("uf");
        uf.AddChild(AddTag(TipoCampo.De2, "", "pIBSUF", 1, 5, Ocorrencia.Obrigatoria, total.Valores.UF.PercentualIBSUF));
        if (total.Valores.UF.PercentualReducaoAliquotaUF > 0)
            uf.AddChild(AddTag(TipoCampo.De2, "", "pRedAliqUF", 1, 6, Ocorrencia.MaiorQueZero, total.Valores.UF.PercentualReducaoAliquotaUF));
        uf.AddChild(AddTag(TipoCampo.De2, "", "pAliqEfetUF", 1, 5, Ocorrencia.Obrigatoria, total.Valores.UF.PercentualAliquotaEfetivaUF));
        valores.AddChild(uf);

        var mun = new XElement("mun");
        mun.AddChild(AddTag(TipoCampo.De2, "", "pIBSMun", 1, 5, Ocorrencia.Obrigatoria, total.Valores.Municipio.PercentualIBSMun));
        if (total.Valores.Municipio.PercentualReducaoAliquotaMun > 0)
            mun.AddChild(AddTag(TipoCampo.De2, "", "pRedAliqMun", 1, 6, Ocorrencia.MaiorQueZero, total.Valores.Municipio.PercentualReducaoAliquotaMun));
        mun.AddChild(AddTag(TipoCampo.De2, "", "pAliqEfetMun", 1, 5, Ocorrencia.Obrigatoria, total.Valores.Municipio.PercentualAliquotaEfetivaMun));
        valores.AddChild(mun);

        var fed = new XElement("fed");
        fed.AddChild(AddTag(TipoCampo.De2, "", "pCBS", 1, 5, Ocorrencia.Obrigatoria, total.Valores.Federal.PercentualCBS));
        if (total.Valores.Federal.PercentualReducaoAliquotaCBS > 0)
            fed.AddChild(AddTag(TipoCampo.De2, "", "pRedAliqCBS", 1, 6, Ocorrencia.MaiorQueZero, total.Valores.Federal.PercentualReducaoAliquotaCBS));
        fed.AddChild(AddTag(TipoCampo.De2, "", "pAliqEfetCBS", 1, 5, Ocorrencia.Obrigatoria, total.Valores.Federal.PercentualAliquotaEfetivaCBS));
        valores.AddChild(fed);

        ibsCbs.AddChild(valores);

        var totCIBS = new XElement("totCIBS");
        totCIBS.AddChild(AddTag(TipoCampo.De2, "", "vTotNF", 1, 15, Ocorrencia.Obrigatoria, total.Totalizadores.ValorTotalNF));

        var gIBS = new XElement("gIBS");
        gIBS.AddChild(AddTag(TipoCampo.De2, "", "vIBSTot", 1, 15, Ocorrencia.Obrigatoria, total.Totalizadores.IBS.ValorIBSTotal));

        var gIBSUFTot = new XElement("gIBSUFTot");
        if (total.Totalizadores.IBS.TotalIBSUF.ValorDiferimento > 0)
            gIBSUFTot.AddChild(AddTag(TipoCampo.De2, "", "vDifUF", 1, 15, Ocorrencia.MaiorQueZero, total.Totalizadores.IBS.TotalIBSUF.ValorDiferimento));
        gIBSUFTot.AddChild(AddTag(TipoCampo.De2, "", "vIBSUF", 1, 15, Ocorrencia.Obrigatoria, total.Totalizadores.IBS.TotalIBSUF.ValorIBSUF));
        gIBS.AddChild(gIBSUFTot);

        var gIBSMunTot = new XElement("gIBSMunTot");
        if (total.Totalizadores.IBS.TotalIBSMun.ValorDiferimento > 0)
            gIBSMunTot.AddChild(AddTag(TipoCampo.De2, "", "vDifMun", 1, 15, Ocorrencia.MaiorQueZero, total.Totalizadores.IBS.TotalIBSMun.ValorDiferimento));
        gIBSMunTot.AddChild(AddTag(TipoCampo.De2, "", "vIBSMun", 1, 15, Ocorrencia.Obrigatoria, total.Totalizadores.IBS.TotalIBSMun.ValorIBSMun));
        gIBS.AddChild(gIBSMunTot);

        totCIBS.AddChild(gIBS);

        var gCBS = new XElement("gCBS");
        if (total.Totalizadores.CBS.ValorDiferimento > 0)
            gCBS.AddChild(AddTag(TipoCampo.De2, "", "vDifCBS", 1, 15, Ocorrencia.MaiorQueZero, total.Totalizadores.CBS.ValorDiferimento));
        gCBS.AddChild(AddTag(TipoCampo.De2, "", "vCBS", 1, 15, Ocorrencia.Obrigatoria, total.Totalizadores.CBS.ValorCBS));
        totCIBS.AddChild(gCBS);

        ibsCbs.AddChild(totCIBS);

        return ibsCbs;
    }

    /// <summary>
    /// Escreve o grupo IBSCBS (TCRTCInfoIBSCBS) dentro de InfDeclaracaoPrestacaoServico.
    /// </summary>
    private XElement? WriteInfoIBSCBSRps(NotaServico nota)
    {
        var info = nota.Servico.Valores.IBSCBS;
        if (info is null) return null;

        var ibsCbs = new XElement("IBSCBS");
        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "finNFSe", 1, 1, Ocorrencia.Obrigatoria, info.FinalidadeNFSe));
        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "indFinal", 1, 1, Ocorrencia.NaoObrigatoria, info.IndicadorFinal));
        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "cIndOp", 6, 6, Ocorrencia.Obrigatoria, info.CodigoIndicadorOperacao));
        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "tpOper", 1, 1, Ocorrencia.NaoObrigatoria, info.TipoOperacao));

        var referencias = info.ReferenciasNFSe.Where(x => !x.IsEmpty()).ToList();
        if (referencias.Count > 0)
        {
            var gRefNFSe = new XElement("gRefNFSe");
            foreach (var referencia in referencias)
                gRefNFSe.AddChild(AddTag(TipoCampo.Str, "", "refNFSe", 1, 50, Ocorrencia.Obrigatoria, referencia));
            ibsCbs.AddChild(gRefNFSe);
        }

        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "tpEnteGov", 1, 1, Ocorrencia.NaoObrigatoria, info.TipoEnteGov));
        ibsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "indDest", 1, 1, Ocorrencia.Obrigatoria, info.IndicadorDestinatario));

        var valores = new XElement("valores");
        var trib = new XElement("trib");
        var gIbsCbs = new XElement("gIBSCBS");

        var sitClass = info.Valores.Tributos.SituacaoClassificacao;
        gIbsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "CST", 3, 3, Ocorrencia.Obrigatoria, sitClass.CodigoSituacaoTributaria));
        gIbsCbs.AddChild(AddTag(TipoCampo.StrNumber, "", "cClassTrib", 6, 6, Ocorrencia.Obrigatoria, sitClass.CodigoClassificacaoTributaria));

        trib.AddChild(gIbsCbs);
        valores.AddChild(trib);
        ibsCbs.AddChild(valores);

        return ibsCbs;
    }

    /// <inheritdoc />
    protected override XElement WriteServicosRps(NotaServico nota)
    {
        var codigoCnaeOriginal = nota.Servico.CodigoCnae;
        nota.Servico.CodigoCnae = codigoCnaeOriginal.OnlyNumbers();

        try
        {
            return base.WriteServicosRps(nota);
        }
        finally
        {
            nota.Servico.CodigoCnae = codigoCnaeOriginal;
        }
    }

    #endregion Protected Methods
}
