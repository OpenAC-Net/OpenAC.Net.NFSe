// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : lucasmoraes804
// Created          : 01-07-2026
//
// ***********************************************************************
// <copyright file="ProviderSigISSWeb102.cs" company="OpenAC .Net">
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
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderSigISSWeb102 : ProviderSigISSWeb
{
    public ProviderSigISSWeb102(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Versao = VersaoNFSe.ve102;
    }

    public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        var xml = base.WriteXmlRps(nota, identado, showDeclaration);

        var doc = XDocument.Parse(xml);
        var notaTag = doc.Root;
        if (notaTag == null) return xml;

        WriteIbsCbsTags(notaTag, nota);

        return doc.Root.AsString(identado, showDeclaration, Encoding.UTF8);
    }

    public override NotaServico LoadXml(XDocument xml)
    {
        var nota = base.LoadXml(xml);
        var notaXml = xml.ElementAnyNs("notafiscal");
        if (notaXml == null) return nota;

        var codigoNbs = notaXml.ElementAnyNs("codigo_nbs")?.GetValue<string>() ?? string.Empty;
        if (!codigoNbs.IsEmpty()) nota.Servico.CodigoNbs = codigoNbs;

        var paisPrestacao = notaXml.ElementAnyNs("pais_local_prest")?.GetValue<string>() ?? string.Empty;
        if (!paisPrestacao.IsEmpty()) nota.EnderecoPrestacao.Pais = paisPrestacao;

        var cidadePrestacao = notaXml.ElementAnyNs("cidade_local_prest")?.GetValue<string>() ?? string.Empty;
        if (!cidadePrestacao.IsEmpty()) nota.EnderecoPrestacao.Municipio = cidadePrestacao;

        var ufPrestacao = notaXml.ElementAnyNs("uf_local_prest")?.GetValue<string>() ?? string.Empty;
        if (!ufPrestacao.IsEmpty()) nota.EnderecoPrestacao.Uf = ufPrestacao;

        var cidadeOperacao = notaXml.ElementAnyNs("cidade_local_op")?.GetValue<string>() ?? string.Empty;
        if (!cidadeOperacao.IsEmpty()) nota.Servico.Municipio = cidadeOperacao;

        var ufOperacao = notaXml.ElementAnyNs("uf_local_op")?.GetValue<string>() ?? string.Empty;
        if (!ufOperacao.IsEmpty()) nota.Servico.UfIncidencia = ufOperacao;

        var codigoClassificacao = notaXml.ElementAnyNs("c_classtrib")?.GetValue<string>() ?? string.Empty;
        var indicadorOperacao = notaXml.ElementAnyNs("ind_op")?.GetValue<string>() ?? string.Empty;
        var consumoPessoal = notaXml.ElementAnyNs("consumo_pessoal")?.GetValue<string>() ?? string.Empty;
        if (!codigoClassificacao.IsEmpty() || !indicadorOperacao.IsEmpty() || !consumoPessoal.IsEmpty())
        {
            nota.Servico.Valores.IBSCBS ??= new InfoIBSCBS();
            var info = nota.Servico.Valores.IBSCBS;
            info.CodigoIndicadorOperacao = indicadorOperacao;
            info.IndicadorFinal = consumoPessoal;
            info.Valores.Tributos.SituacaoClassificacao.CodigoClassificacaoTributaria = codigoClassificacao;
        }

        if (HasDestinatarioCbsIbsXml(notaXml))
        {
            var destinatario = nota.DestinatarioCBSIBS;
            destinatario.CpfCnpj = notaXml.ElementAnyNs("cnpj_cpf_destinatario_cbsibs")?.GetValue<string>() ?? string.Empty;
            destinatario.InscricaoEstadual = notaXml.ElementAnyNs("ie_destinatario_cbsibs")?.GetValue<string>() ?? string.Empty;
            destinatario.InscricaoMunicipal = notaXml.ElementAnyNs("im_destinatario_cbsibs")?.GetValue<string>() ?? string.Empty;
            destinatario.RazaoSocial = notaXml.ElementAnyNs("razao_social_destinatario_cbsibs")?.GetValue<string>() ?? string.Empty;
            destinatario.Endereco.Logradouro = notaXml.ElementAnyNs("endereco_destinatario_cbsibs")?.GetValue<string>() ?? string.Empty;
            destinatario.Endereco.Numero = notaXml.ElementAnyNs("numero_ende_destinatario_cbsibs")?.GetValue<string>() ?? string.Empty;
            destinatario.Endereco.Complemento = notaXml.ElementAnyNs("complemento_ende_destinatario_cbsibs")?.GetValue<string>() ?? string.Empty;
            destinatario.Endereco.Bairro = notaXml.ElementAnyNs("bairro_destinatario_cbsibs")?.GetValue<string>() ?? string.Empty;
            destinatario.Endereco.Cep = notaXml.ElementAnyNs("cep_destinatario_cbsibs")?.GetValue<string>() ?? string.Empty;
            destinatario.Endereco.Municipio = notaXml.ElementAnyNs("cidade_destinatario_cbsibs")?.GetValue<string>() ?? string.Empty;
            destinatario.Endereco.Uf = notaXml.ElementAnyNs("uf_destinatario_cbsibs")?.GetValue<string>() ?? string.Empty;
            destinatario.Endereco.Pais = notaXml.ElementAnyNs("pais_destinatario_cbsibs")?.GetValue<string>() ?? string.Empty;
            destinatario.DadosContato.Email = notaXml.ElementAnyNs("email_destinatario_cbsibs")?.GetValue<string>() ?? string.Empty;
        }

        if (HasTotaisIbsCbsXml(notaXml))
        {
            nota.IBSCBSTotal ??= new IBSCBSTotal();
            var total = nota.IBSCBSTotal;

            var valorCbs = notaXml.ElementAnyNs("valor_cbs")?.GetValue<decimal>() ?? 0m;
            var valorIbsEst = notaXml.ElementAnyNs("valor_ibs_est")?.GetValue<decimal>() ?? 0m;
            var valorIbsMun = notaXml.ElementAnyNs("valor_ibs_mun")?.GetValue<decimal>() ?? 0m;

            total.Totalizadores.CBS.ValorCBS = valorCbs;
            total.Totalizadores.IBS.TotalIBSUF.ValorIBSUF = valorIbsEst;
            total.Totalizadores.IBS.TotalIBSMun.ValorIBSMun = valorIbsMun;

            if (valorIbsEst > 0m || valorIbsMun > 0m)
                total.Totalizadores.IBS.ValorIBSTotal = valorIbsEst + valorIbsMun;

            total.Valores.Federal.PercentualCBS = notaXml.ElementAnyNs("aliq_cbs")?.GetValue<decimal>() ?? 0m;
            total.Valores.UF.PercentualIBSUF = notaXml.ElementAnyNs("aliq_ibs_est")?.GetValue<decimal>() ?? 0m;
            total.Valores.Municipio.PercentualIBSMun = notaXml.ElementAnyNs("aliq_ibs_mun")?.GetValue<decimal>() ?? 0m;

            total.Valores.Federal.PercentualReducaoAliquotaCBS = notaXml.ElementAnyNs("reducao_cbs")?.GetValue<decimal>() ?? 0m;
            total.Valores.UF.PercentualReducaoAliquotaUF = notaXml.ElementAnyNs("reducao_ibs_est")?.GetValue<decimal>() ?? 0m;
            total.Valores.Municipio.PercentualReducaoAliquotaMun = notaXml.ElementAnyNs("reducao_ibs_mun")?.GetValue<decimal>() ?? 0m;
        }

        return nota;
    }

    private void WriteIbsCbsTags(XElement notaTag, NotaServico nota)
    {
        if (!HasIbsCbsData(nota)) return;

        notaTag.AddChild(AddTag(TipoCampo.Str, "", "codigo_nbs", 1, 14, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoNbs));

        notaTag.AddChild(AddTag(TipoCampo.Str, "", "exterior_prestacao_servico", 1, 1, Ocorrencia.NaoObrigatoria,
            GetIndicadorExteriorPrestacao(nota.EnderecoPrestacao)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "pais_local_prest", 1, 50, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Pais));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cidade_local_prest", 1, 100, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Municipio));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "uf_local_prest", 1, 2, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Uf));

        if (HasDestinatarioCbsIbs(nota.DestinatarioCBSIBS))
        {
            var cpfCnpjDest = nota.DestinatarioCBSIBS.CpfCnpj.OnlyNumbers();
            var pessoaDest = cpfCnpjDest.IsEmpty() ? string.Empty : cpfCnpjDest.Length == 11 ? "F" : "J";

            notaTag.AddChild(AddTag(TipoCampo.Str, "", "pessoa_destinatario_cbsibs", 1, 1, Ocorrencia.NaoObrigatoria, pessoaDest));
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "cnpj_cpf_destinatario_cbsibs", 1, 14, Ocorrencia.NaoObrigatoria, cpfCnpjDest));
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "ie_destinatario_cbsibs", 1, 15, Ocorrencia.NaoObrigatoria, nota.DestinatarioCBSIBS.InscricaoEstadual));
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "im_destinatario_cbsibs", 1, 15, Ocorrencia.NaoObrigatoria, nota.DestinatarioCBSIBS.InscricaoMunicipal));
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "razao_social_destinatario_cbsibs", 1, 100, Ocorrencia.NaoObrigatoria, nota.DestinatarioCBSIBS.RazaoSocial));
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "endereco_destinatario_cbsibs", 1, 60, Ocorrencia.NaoObrigatoria, nota.DestinatarioCBSIBS.Endereco.Logradouro));
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "numero_ende_destinatario_cbsibs", 1, 10, Ocorrencia.NaoObrigatoria, nota.DestinatarioCBSIBS.Endereco.Numero));
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "complemento_ende_destinatario_cbsibs", 1, 40, Ocorrencia.NaoObrigatoria, nota.DestinatarioCBSIBS.Endereco.Complemento));
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "bairro_destinatario_cbsibs", 1, 100, Ocorrencia.NaoObrigatoria, nota.DestinatarioCBSIBS.Endereco.Bairro));
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "cep_destinatario_cbsibs", 1, 8, Ocorrencia.NaoObrigatoria, nota.DestinatarioCBSIBS.Endereco.Cep));
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "cidade_destinatario_cbsibs", 1, 100, Ocorrencia.NaoObrigatoria, nota.DestinatarioCBSIBS.Endereco.Municipio));
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "uf_destinatario_cbsibs", 1, 2, Ocorrencia.NaoObrigatoria, nota.DestinatarioCBSIBS.Endereco.Uf));
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "pais_destinatario_cbsibs", 1, 50, Ocorrencia.NaoObrigatoria, nota.DestinatarioCBSIBS.Endereco.Pais));
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "email_destinatario_cbsibs", 1, 120, Ocorrencia.NaoObrigatoria, nota.DestinatarioCBSIBS.DadosContato.Email));
        }

        var info = nota.Servico.Valores.IBSCBS;
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "c_classtrib", 1, 20, Ocorrencia.NaoObrigatoria,
            info?.Valores?.Tributos?.SituacaoClassificacao?.CodigoClassificacaoTributaria));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "ind_op", 1, 6, Ocorrencia.NaoObrigatoria, info?.CodigoIndicadorOperacao));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "consumo_pessoal", 1, 1, Ocorrencia.NaoObrigatoria, info?.IndicadorFinal));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "exterior_op", 1, 1, Ocorrencia.NaoObrigatoria, GetIndicadorExteriorOperacao(nota)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cidade_local_op", 1, 100, Ocorrencia.NaoObrigatoria, GetCidadeLocalOperacao(nota)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "uf_local_op", 1, 2, Ocorrencia.NaoObrigatoria, GetUfLocalOperacao(nota)));
    }

    private static bool HasIbsCbsData(NotaServico nota)
    {
        return !nota.Servico.CodigoNbs.IsEmpty()
               || nota.Servico.Valores.IBSCBS != null
               || HasDestinatarioCbsIbs(nota.DestinatarioCBSIBS);
    }

    private static bool HasDestinatarioCbsIbs(DadosTomador destinatario)
    {
        return !destinatario.CpfCnpj.IsEmpty()
               || !destinatario.InscricaoEstadual.IsEmpty()
               || !destinatario.InscricaoMunicipal.IsEmpty()
               || !destinatario.RazaoSocial.IsEmpty()
               || !destinatario.Endereco.Logradouro.IsEmpty()
               || !destinatario.Endereco.Numero.IsEmpty()
               || !destinatario.Endereco.Complemento.IsEmpty()
               || !destinatario.Endereco.Bairro.IsEmpty()
               || !destinatario.Endereco.Cep.IsEmpty()
               || !destinatario.Endereco.Municipio.IsEmpty()
               || !destinatario.Endereco.Uf.IsEmpty()
               || !destinatario.Endereco.Pais.IsEmpty()
               || !destinatario.DadosContato.Email.IsEmpty();
    }

    private static string GetIndicadorExteriorPrestacao(Endereco endereco)
    {
        if (endereco.CodigoPais > 0)
            return endereco.CodigoPais == 1058 ? "0" : "1";

        if (!endereco.Pais.IsEmpty())
            return endereco.Pais.Equals("Brasil", StringComparison.OrdinalIgnoreCase) ? "0" : "1";

        return "0";
    }

    private static string GetIndicadorExteriorOperacao(NotaServico nota)
    {
        if (nota.Servico.CodigoPais > 0)
            return nota.Servico.CodigoPais == 1058 ? "0" : "1";

        return GetIndicadorExteriorPrestacao(nota.EnderecoPrestacao);
    }

    private static string GetCidadeLocalOperacao(NotaServico nota)
    {
        if (!nota.Servico.Municipio.IsEmpty()) return nota.Servico.Municipio;
        return nota.EnderecoPrestacao.Municipio ?? string.Empty;
    }

    private static string GetUfLocalOperacao(NotaServico nota)
    {
        if (!nota.Servico.UfIncidencia.IsEmpty()) return nota.Servico.UfIncidencia;
        return nota.EnderecoPrestacao.Uf ?? string.Empty;
    }

    private static bool HasDestinatarioCbsIbsXml(XElement notaXml)
    {
        return notaXml.ElementAnyNs("cnpj_cpf_destinatario_cbsibs") != null
               || notaXml.ElementAnyNs("razao_social_destinatario_cbsibs") != null
               || notaXml.ElementAnyNs("im_destinatario_cbsibs") != null
               || notaXml.ElementAnyNs("ie_destinatario_cbsibs") != null;
    }

    private static bool HasTotaisIbsCbsXml(XElement notaXml)
    {
        return notaXml.ElementAnyNs("valor_cbs") != null
               || notaXml.ElementAnyNs("valor_ibs_est") != null
               || notaXml.ElementAnyNs("valor_ibs_mun") != null
               || notaXml.ElementAnyNs("aliq_cbs") != null
               || notaXml.ElementAnyNs("aliq_ibs_est") != null
               || notaXml.ElementAnyNs("aliq_ibs_mun") != null
               || notaXml.ElementAnyNs("reducao_cbs") != null
               || notaXml.ElementAnyNs("reducao_ibs_est") != null
               || notaXml.ElementAnyNs("reducao_ibs_mun") != null;
    }
}
