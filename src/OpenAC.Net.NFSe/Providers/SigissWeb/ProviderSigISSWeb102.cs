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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom.Interface;
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
    
    protected override IServiceClient GetClient(TipoUrl tipo) => new SigISSWeb102ServiceClient(this, tipo);

    public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        var xml = base.WriteXmlRps(nota, identado, showDeclaration);

        var doc = XDocument.Parse(xml);
        var notaTag = doc.Root;
        if (notaTag == null) return xml;

        NormalizeBaseTags(notaTag, nota);
        WriteIntermediarioTags(notaTag, nota);
        WriteConstrucaoCivilTags(notaTag, nota);
        WriteEventoTags(notaTag, nota);
        WriteIbsCbsTags(notaTag, nota);

        return doc.Root.AsString(identado, showDeclaration, Encoding.UTF8);
    }

    public override NotaServico LoadXml(XDocument xml)
    {
        var nota = base.LoadXml(xml);
        var notaXml = xml.ElementAnyNs("notafiscal");
        if (notaXml == null) return nota;

        var issRetido = notaXml.ElementAnyNs("iss_retido")?.GetValue<string>() ?? string.Empty;
        if (issRetido == "I") nota.Servico.Valores.IssRetido = SituacaoTributaria.Substituicao;

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

        LoadIntermediario(notaXml, nota);
        LoadConstrucaoCivil(notaXml, nota);
        LoadEvento(notaXml, nota);

        return nota;
    }

    private void NormalizeBaseTags(XElement notaTag, NotaServico nota)
    {
        ReplaceTag(notaTag, "descricao", AddTag(TipoCampo.Str, "", "descricao", 1, 1000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
        ReplaceTag(notaTag, "id_codigo_servico", AddTag(TipoCampo.Str, "", "id_codigo_servico", 1, 8, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));

        var issRetido = nota.Servico.Valores.IssRetido switch
        {
            SituacaoTributaria.Retencao => "S",
            SituacaoTributaria.Substituicao => "I",
            _ => "N"
        };

        ReplaceTag(notaTag, "iss_retido", AddTag(TipoCampo.Str, "", "iss_retido", 1, 1, Ocorrencia.Obrigatoria, issRetido));
    }

    private void ReplaceTag(XElement parent, string tag, XElement? element)
    {
        var existing = parent.ElementAnyNs(tag);
        if (existing == null)
        {
            if (element != null) parent.Add(element);
            return;
        }

        if (element == null)
        {
            existing.Remove();
            return;
        }

        existing.ReplaceWith(element);
    }

    private void WriteIntermediarioTags(XElement notaTag, NotaServico nota)
    {
        var intermediario = nota.Intermediario;
        if (nota.Intermediario.RazaoSocial.IsEmpty()) return;

        var cpfCnpj = intermediario.CpfCnpj.OnlyNumbers();
        var pessoaIntermediario = cpfCnpj.IsEmpty() ? string.Empty : cpfCnpj.Length == 11 ? "F" : "J";
        var exteriorIntermediario = GetIndicadorExterior(intermediario.Endereco, intermediario.EnderecoExterior);
        var emailIntermediario = intermediario.DadosContato.Email.IsEmpty() ? intermediario.EMail : intermediario.DadosContato.Email;

        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cnpj_cpf_intermediario", 1, 14, Ocorrencia.NaoObrigatoria, cpfCnpj));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "pessoa_intermediario", 1, 1, Ocorrencia.NaoObrigatoria, pessoaIntermediario));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "exterior_intermediario", 1, 1, Ocorrencia.NaoObrigatoria, exteriorIntermediario));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "ie_intermediario", 1, 15, Ocorrencia.NaoObrigatoria, intermediario.InscricaoEstadual));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "im_intermediario", 1, 15, Ocorrencia.NaoObrigatoria, intermediario.InscricaoMunicipal));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "razao_social_intermediario", 1, 100, Ocorrencia.NaoObrigatoria, intermediario.RazaoSocial));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "endereco_intermediario", 1, 60, Ocorrencia.NaoObrigatoria, intermediario.Endereco.Logradouro));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "numero_ende_intermediario", 1, 10, Ocorrencia.NaoObrigatoria, intermediario.Endereco.Numero));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "complemento_ende_intermediario", 1, 40, Ocorrencia.NaoObrigatoria, intermediario.Endereco.Complemento));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "bairro_intermediario", 1, 100, Ocorrencia.NaoObrigatoria, intermediario.Endereco.Bairro));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cep_intermediario", 1, 8, Ocorrencia.NaoObrigatoria, intermediario.Endereco.Cep));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cidade_intermediario", 1, 100, Ocorrencia.NaoObrigatoria, intermediario.Endereco.Municipio));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "uf_intermediario", 1, 2, Ocorrencia.NaoObrigatoria, intermediario.Endereco.Uf));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "pais_intermediario", 1, 50, Ocorrencia.NaoObrigatoria, intermediario.Endereco.Pais));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "fone_intermediario", 1, 30, Ocorrencia.NaoObrigatoria, intermediario.DadosContato.Telefone));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "email_intermediario", 1, 120, Ocorrencia.NaoObrigatoria, emailIntermediario));
    }

    private void WriteConstrucaoCivilTags(XElement notaTag, NotaServico nota)
    {
        var obra = nota.ConstrucaoCivil;
        if (!HasConstrucaoCivil(obra)) return;

        var nomeObra = !obra.NomeObra.IsEmpty()
            ? obra.NomeObra
            : !obra.Projeto.IsEmpty()
                ? obra.Projeto
                : obra.CodigoObra;
        var cidadeObra = !obra.CidadeObra.IsEmpty()
            ? obra.CidadeObra
            : obra.CodigoMunicipioObra > 0
                ? obra.CodigoMunicipioObra.ToString()
                : string.Empty;
        var inscricaoImobiliaria = obra.InscricaoImobiliariaFiscal.IsEmpty() ? obra.Matricula : obra.InscricaoImobiliariaFiscal;
        var cnoObra = obra.CnoObra.IsEmpty() ? obra.CodigoCEI : obra.CnoObra;
        var exteriorObra = GetIndicadorExterior(obra.CodigoPaisObra, obra.XPaisObra, obra.CidadeExteriorObra, obra.EstadoRegiaoExteriorObra);

        notaTag.AddChild(AddTag(TipoCampo.Str, "", "nome_obra", 1, 100, Ocorrencia.NaoObrigatoria, nomeObra));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cep_obra", 1, 8, Ocorrencia.NaoObrigatoria, obra.CepObra));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cidade_obra", 1, 100, Ocorrencia.NaoObrigatoria, cidadeObra));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "uf_obra", 1, 2, Ocorrencia.NaoObrigatoria, obra.UFObra));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "bairro_obra", 1, 100, Ocorrencia.NaoObrigatoria, obra.BairroObra));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "logradouro_obra", 1, 255, Ocorrencia.NaoObrigatoria, obra.LogradouroObra));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "logradouro_numero_obra", 1, 60, Ocorrencia.NaoObrigatoria, obra.NumeroObra));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "logradouro_complemento_obra", 1, 156, Ocorrencia.NaoObrigatoria, obra.ComplementoObra));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "insc_imobiliaria_fiscal_obra", 1, 30, Ocorrencia.NaoObrigatoria, inscricaoImobiliaria));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cno_obra", 1, 30, Ocorrencia.NaoObrigatoria, cnoObra));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "exterior_obra", 1, 1, Ocorrencia.NaoObrigatoria, exteriorObra));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cidade_exterior_obra", 1, 60, Ocorrencia.NaoObrigatoria, obra.CidadeExteriorObra));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "estado_regiao_exterior_obra", 1, 60, Ocorrencia.NaoObrigatoria, obra.EstadoRegiaoExteriorObra));
    }

    private void WriteEventoTags(XElement notaTag, NotaServico nota)
    {
        var evento = nota.Evento;
        if (!HasEvento(evento)) return;

        var dataInicio = evento?.DataInicio.HasValue == true ? evento.DataInicio.Value.ToString("dd/MM/yyyy") : string.Empty;
        var dataFim = evento?.DataFim.HasValue == true ? evento.DataFim.Value.ToString("dd/MM/yyyy") : string.Empty;
        var exteriorEvento = GetIndicadorExterior(evento!.Endereco.CodigoPais, evento.Endereco.Pais, evento.CidadeExterior, evento.EstadoRegiaoExterior);

        notaTag.AddChild(AddTag(TipoCampo.Str, "", "identificacao_evento", 1, 30, Ocorrencia.NaoObrigatoria, evento.IdentificacaoEvento));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "data_inicio_evento", 1, 10, Ocorrencia.NaoObrigatoria, dataInicio));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "data_final_evento", 1, 10, Ocorrencia.NaoObrigatoria, dataFim));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "descricao_evento", 1, 255, Ocorrencia.NaoObrigatoria, evento.DescricaoEvento));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cep_evento", 1, 8, Ocorrencia.NaoObrigatoria, evento.Endereco.Cep));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "nome_cidade_evento", 1, 150, Ocorrencia.NaoObrigatoria, evento.Endereco.Municipio));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "uf_evento", 1, 2, Ocorrencia.NaoObrigatoria, evento.Endereco.Uf));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "nome_bairro_evento", 1, 100, Ocorrencia.NaoObrigatoria, evento.Endereco.Bairro));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "logradouro_evento", 1, 255, Ocorrencia.NaoObrigatoria, evento.Endereco.Logradouro));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "numero_logradouro_evento", 1, 60, Ocorrencia.NaoObrigatoria, evento.Endereco.Numero));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "complemento_evento", 1, 156, Ocorrencia.NaoObrigatoria, evento.Endereco.Complemento));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "exterior_evento", 1, 1, Ocorrencia.NaoObrigatoria, exteriorEvento));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cidade_exterior_evento", 1, 60, Ocorrencia.NaoObrigatoria, evento.CidadeExterior));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "estado_regiao_evento", 1, 60, Ocorrencia.NaoObrigatoria, evento.EstadoRegiaoExterior));
    }

    private bool HasConstrucaoCivil(DadosConstrucaoCivil obra)
    {
        return !obra.NomeObra.IsEmpty()
               || !obra.CodigoObra.IsEmpty()
               || !obra.Projeto.IsEmpty()
               || !obra.CepObra.IsEmpty()
               || !obra.CidadeObra.IsEmpty()
               || !obra.UFObra.IsEmpty()
               || !obra.BairroObra.IsEmpty()
               || !obra.LogradouroObra.IsEmpty()
               || !obra.NumeroObra.IsEmpty()
               || !obra.ComplementoObra.IsEmpty()
               || !obra.InscricaoImobiliariaFiscal.IsEmpty()
               || !obra.Matricula.IsEmpty()
               || !obra.CnoObra.IsEmpty()
               || !obra.CodigoCEI.IsEmpty()
               || !obra.CidadeExteriorObra.IsEmpty()
               || !obra.EstadoRegiaoExteriorObra.IsEmpty()
               || obra.CodigoMunicipioObra > 0
               || obra.CodigoPaisObra > 0
               || !obra.XPaisObra.IsEmpty();
    }

    private bool HasEvento(EventoRps? evento)
    {
        if (evento == null) return false;

        return !evento.IdentificacaoEvento.IsEmpty()
               || !evento.DescricaoEvento.IsEmpty()
               || evento.DataInicio.HasValue
               || evento.DataFim.HasValue
               || !evento.Endereco.Cep.IsEmpty()
               || !evento.Endereco.Municipio.IsEmpty()
               || !evento.Endereco.Uf.IsEmpty()
               || !evento.Endereco.Bairro.IsEmpty()
               || !evento.Endereco.Logradouro.IsEmpty()
               || !evento.Endereco.Numero.IsEmpty()
               || !evento.Endereco.Complemento.IsEmpty()
               || !evento.CidadeExterior.IsEmpty()
               || !evento.EstadoRegiaoExterior.IsEmpty()
               || evento.Endereco.CodigoPais > 0
               || !evento.Endereco.Pais.IsEmpty();
    }

    private string GetIndicadorExterior(Endereco endereco, EnderecoExterior enderecoExterior)
    {
        if (!enderecoExterior.EnderecoCompleto.IsEmpty()) return "1";

        var codigoPais = endereco.CodigoPais > 0 ? endereco.CodigoPais : enderecoExterior.CodigoPais;
        return GetIndicadorExterior(codigoPais, endereco.Pais, string.Empty, string.Empty);
    }

    private string GetIndicadorExterior(int codigoPais, string pais, string cidadeExterior, string estadoExterior)
    {
        if (!cidadeExterior.IsEmpty() || !estadoExterior.IsEmpty()) return "1";

        if (codigoPais > 0) return codigoPais == 1058 ? "0" : "1";

        if (!pais.IsEmpty())
            return pais.Equals("Brasil", StringComparison.OrdinalIgnoreCase) ? "0" : "1";

        return "0";
    }

    private void LoadIntermediario(XElement notaXml, NotaServico nota)
    {
        var intermediario = nota.Intermediario;

        intermediario.CpfCnpj = notaXml.ElementAnyNs("cnpj_cpf_intermediario")?.GetValue<string>() ?? string.Empty;
        intermediario.InscricaoEstadual = notaXml.ElementAnyNs("ie_intermediario")?.GetValue<string>() ?? string.Empty;
        intermediario.InscricaoMunicipal = notaXml.ElementAnyNs("im_intermediario")?.GetValue<string>() ?? string.Empty;
        intermediario.RazaoSocial = notaXml.ElementAnyNs("razao_social_intermediario")?.GetValue<string>() ?? string.Empty;

        intermediario.Endereco.Logradouro = notaXml.ElementAnyNs("endereco_intermediario")?.GetValue<string>() ?? string.Empty;
        intermediario.Endereco.Numero = notaXml.ElementAnyNs("numero_ende_intermediario")?.GetValue<string>() ?? string.Empty;
        intermediario.Endereco.Complemento = notaXml.ElementAnyNs("complemento_ende_intermediario")?.GetValue<string>() ?? string.Empty;
        intermediario.Endereco.Bairro = notaXml.ElementAnyNs("bairro_intermediario")?.GetValue<string>() ?? string.Empty;
        intermediario.Endereco.Cep = notaXml.ElementAnyNs("cep_intermediario")?.GetValue<string>() ?? string.Empty;
        intermediario.Endereco.Municipio = notaXml.ElementAnyNs("cidade_intermediario")?.GetValue<string>() ?? string.Empty;
        intermediario.Endereco.Uf = notaXml.ElementAnyNs("uf_intermediario")?.GetValue<string>() ?? string.Empty;
        intermediario.Endereco.Pais = notaXml.ElementAnyNs("pais_intermediario")?.GetValue<string>() ?? string.Empty;

        intermediario.DadosContato.Telefone = notaXml.ElementAnyNs("fone_intermediario")?.GetValue<string>() ?? string.Empty;

        var emailIntermediario = notaXml.ElementAnyNs("email_intermediario")?.GetValue<string>() ?? string.Empty;
        if (!emailIntermediario.IsEmpty())
        {
            intermediario.DadosContato.Email = emailIntermediario;
            intermediario.EMail = emailIntermediario;
        }
    }

    private void LoadConstrucaoCivil(XElement notaXml, NotaServico nota)
    {
        var obra = nota.ConstrucaoCivil;

        obra.NomeObra = notaXml.ElementAnyNs("nome_obra")?.GetValue<string>() ?? string.Empty;
        obra.CepObra = notaXml.ElementAnyNs("cep_obra")?.GetValue<string>() ?? string.Empty;
        obra.CidadeObra = notaXml.ElementAnyNs("cidade_obra")?.GetValue<string>() ?? string.Empty;
        obra.UFObra = notaXml.ElementAnyNs("uf_obra")?.GetValue<string>() ?? string.Empty;
        obra.BairroObra = notaXml.ElementAnyNs("bairro_obra")?.GetValue<string>() ?? string.Empty;
        obra.LogradouroObra = notaXml.ElementAnyNs("logradouro_obra")?.GetValue<string>() ?? string.Empty;
        obra.NumeroObra = notaXml.ElementAnyNs("logradouro_numero_obra")?.GetValue<string>() ?? string.Empty;
        obra.ComplementoObra = notaXml.ElementAnyNs("logradouro_complemento_obra")?.GetValue<string>() ?? string.Empty;
        obra.InscricaoImobiliariaFiscal = notaXml.ElementAnyNs("insc_imobiliaria_fiscal_obra")?.GetValue<string>() ?? string.Empty;
        obra.CnoObra = notaXml.ElementAnyNs("cno_obra")?.GetValue<string>() ?? string.Empty;
        obra.CidadeExteriorObra = notaXml.ElementAnyNs("cidade_exterior_obra")?.GetValue<string>() ?? string.Empty;
        obra.EstadoRegiaoExteriorObra = notaXml.ElementAnyNs("estado_regiao_exterior_obra")?.GetValue<string>() ?? string.Empty;
    }

    private void LoadEvento(XElement notaXml, NotaServico nota)
    {
        if (!HasEventoXml(notaXml)) return;

        nota.Evento ??= new EventoRps();
        var evento = nota.Evento;

        evento.IdentificacaoEvento = notaXml.ElementAnyNs("identificacao_evento")?.GetValue<string>() ?? string.Empty;
        evento.DescricaoEvento = notaXml.ElementAnyNs("descricao_evento")?.GetValue<string>() ?? string.Empty;

        var dataInicio = ParseEventoData(notaXml, "data_inicio_evento");
        if (dataInicio.HasValue) evento.DataInicio = dataInicio;

        var dataFim = ParseEventoData(notaXml, "data_final_evento");
        if (dataFim.HasValue) evento.DataFim = dataFim;

        evento.Endereco.Cep = notaXml.ElementAnyNs("cep_evento")?.GetValue<string>() ?? string.Empty;
        evento.Endereco.Municipio = notaXml.ElementAnyNs("nome_cidade_evento")?.GetValue<string>() ?? string.Empty;
        evento.Endereco.Uf = notaXml.ElementAnyNs("uf_evento")?.GetValue<string>() ?? string.Empty;
        evento.Endereco.Bairro = notaXml.ElementAnyNs("nome_bairro_evento")?.GetValue<string>() ?? string.Empty;
        evento.Endereco.Logradouro = notaXml.ElementAnyNs("logradouro_evento")?.GetValue<string>() ?? string.Empty;
        evento.Endereco.Numero = notaXml.ElementAnyNs("numero_logradouro_evento")?.GetValue<string>() ?? string.Empty;
        evento.Endereco.Complemento = notaXml.ElementAnyNs("complemento_evento")?.GetValue<string>() ?? string.Empty;

        evento.CidadeExterior = notaXml.ElementAnyNs("cidade_exterior_evento")?.GetValue<string>() ?? string.Empty;
        evento.EstadoRegiaoExterior = notaXml.ElementAnyNs("estado_regiao_evento")?.GetValue<string>() ?? string.Empty;
    }

    private DateTime? ParseEventoData(XElement notaXml, string tag)
    {
        var valor = notaXml.ElementAnyNs(tag)?.GetValue<string>() ?? string.Empty;
        if (valor.IsEmpty()) return null;

        if (DateTime.TryParse(valor, new CultureInfo("pt-BR"), DateTimeStyles.None, out var data))
            return data;

        return null;
    }

    private bool HasEventoXml(XElement notaXml)
    {
        return notaXml.ElementAnyNs("identificacao_evento") != null
               || notaXml.ElementAnyNs("descricao_evento") != null
               || notaXml.ElementAnyNs("data_inicio_evento") != null
               || notaXml.ElementAnyNs("data_final_evento") != null
               || notaXml.ElementAnyNs("cep_evento") != null
               || notaXml.ElementAnyNs("nome_cidade_evento") != null
               || notaXml.ElementAnyNs("uf_evento") != null
               || notaXml.ElementAnyNs("logradouro_evento") != null;
    }

    private void WriteIbsCbsTags(XElement notaTag, NotaServico nota)
    {
        if (!HasIbsCbsData(nota)) return;

        notaTag.AddChild(AddTag(TipoCampo.Str, "", "codigo_nbs", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoNbs));

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
    
    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        // 1. Tentativa segura de parse do XML
        if (!TryParseXml(retornoWebservice.XmlRetorno, out var xmlRet))
        {
            // Retorno NÃO é XML → é mensagem pura
            retornoWebservice.Sucesso = false;
            retornoWebservice.Protocolo = string.Empty;
            retornoWebservice.Data = DateTime.MinValue;

            // Preserve a mensagem original para diagnóstico
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = retornoWebservice.XmlRetorno});

            return;
        }

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

    private bool HasIbsCbsData(NotaServico nota)
    {
        return !nota.Servico.CodigoNbs.IsEmpty()
               || nota.Servico.Valores.IBSCBS != null
               || HasDestinatarioCbsIbs(nota.DestinatarioCBSIBS);
    }

    private bool HasDestinatarioCbsIbs(DadosTomador destinatario)
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

    private string GetIndicadorExteriorPrestacao(Endereco endereco)
    {
        if (endereco.CodigoPais > 0)
            return endereco.CodigoPais == 1058 ? "0" : "1";

        if (!endereco.Pais.IsEmpty())
            return endereco.Pais.Equals("Brasil", StringComparison.OrdinalIgnoreCase) ? "0" : "1";

        return "0";
    }

    private string GetIndicadorExteriorOperacao(NotaServico nota)
    {
        if (nota.Servico.CodigoPais > 0)
            return nota.Servico.CodigoPais == 1058 ? "0" : "1";

        return GetIndicadorExteriorPrestacao(nota.EnderecoPrestacao);
    }

    private string GetCidadeLocalOperacao(NotaServico nota)
    {
        if (!nota.Servico.Municipio.IsEmpty()) return nota.Servico.Municipio;
        return nota.EnderecoPrestacao.Municipio ?? string.Empty;
    }

    private string GetUfLocalOperacao(NotaServico nota)
    {
        if (!nota.Servico.UfIncidencia.IsEmpty()) return nota.Servico.UfIncidencia;
        return nota.EnderecoPrestacao.Uf ?? string.Empty;
    }

    private bool HasDestinatarioCbsIbsXml(XElement notaXml)
    {
        return notaXml.ElementAnyNs("cnpj_cpf_destinatario_cbsibs") != null
               || notaXml.ElementAnyNs("razao_social_destinatario_cbsibs") != null
               || notaXml.ElementAnyNs("im_destinatario_cbsibs") != null
               || notaXml.ElementAnyNs("ie_destinatario_cbsibs") != null;
    }

    private bool HasTotaisIbsCbsXml(XElement notaXml)
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
    
    private bool TryParseXml(string xml, out XDocument document)
    {
        document = null;

        if (string.IsNullOrWhiteSpace(xml))
            return false;

        try
        {
            document = XDocument.Parse(xml);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
