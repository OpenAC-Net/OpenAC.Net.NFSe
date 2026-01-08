// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : danilobreda
// Created          : 07-10-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 10-10-2020
// ***********************************************************************
// <copyright file="ProviderSigiss.cs" company="OpenAC .Net">
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
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderSigISS100 : ProviderBase
{
    #region Constructors

    public ProviderSigISS100(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "SigISS";
        Versao = VersaoNFSe.ve100;
    }

    #endregion Constructors

    #region Methods

    #region Public

    public override NotaServico LoadXml(XDocument xml)
    {
        throw new NotImplementedException("Metodo LoadXml não implementado");
    }

    public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        var xmldoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
        var notaTag = new XElement("DescricaoRps");
        xmldoc.Add(notaTag);

        notaTag.AddChild(AddTag(TipoCampo.Str, "", "ccm", 1, 120, Ocorrencia.Obrigatoria, Configuracoes.WebServices.Usuario));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cnpj", 1, 14, Ocorrencia.Obrigatoria, nota.Prestador.CpfCnpj.OnlyNumbers()));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "senha", 1, 120, Ocorrencia.Obrigatoria, Configuracoes.WebServices.Senha));

        //obrigatorio apenas para simplesnacional
        if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "aliquota_simples", 1, 11, Ocorrencia.Obrigatoria, nota.Servico.Valores.Aliquota));

        if (nota.NumeroLote > 0) //nao obrigatorio e pode ser utilizado para controle
            notaTag.AddChild(AddTag(TipoCampo.Int, "", "id_sis_legado", 1, 15, Ocorrencia.NaoObrigatoria, nota.NumeroLote));

        notaTag.AddChild(AddTag(TipoCampo.Int, "", "servico", 1, 20, Ocorrencia.Obrigatoria, nota.Servico.CodigoTributacaoMunicipio));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "situacao", 2, 2, Ocorrencia.Obrigatoria, NaturezaOperacao.Sigiss.GetValue(nota.NaturezaOperacao)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "valor", 1, 15, Ocorrencia.Obrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.ValorServicos)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "base", 1, 15, Ocorrencia.Obrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.BaseCalculo)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "descricaoNF", 0, 2000, Ocorrencia.NaoObrigatoria, nota.Servico.Descricao.RemoveAccent()));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "tomador_tipo", 14, 14, Ocorrencia.NaoObrigatoria, 
            nota.Tomador.Tipo == TipoTomador.NaoIdentificado ? 1 :
                nota.Tomador.Tipo == TipoTomador.PessoaFisica ? 2 :
                nota.Tomador.Tipo == TipoTomador.PessoaJuridica && nota.Tomador.Endereco.CodigoMunicipio == nota.Prestador.Endereco.CodigoMunicipio ? 3 :
                nota.Tomador.Tipo == TipoTomador.PessoaJuridica && nota.Tomador.Endereco.CodigoMunicipio != nota.Prestador.Endereco.CodigoMunicipio && nota.Tomador.Endereco.CodigoPais == nota.Prestador.Endereco.CodigoPais ? 4 :
                nota.Tomador.Tipo == TipoTomador.PessoaJuridica && nota.Tomador.Endereco.CodigoPais != nota.Prestador.Endereco.CodigoPais ? 5 :
                nota.Tomador.Tipo == TipoTomador.ProdutorRural ? 6 :
                1
            )); //TipoTomador

        if (nota.Tomador.Tipo != TipoTomador.NaoIdentificado)
            notaTag.AddChild(AdicionarTagCNPJCPF("", "tomador_cnpj", "tomador_cnpj", nota.Tomador.CpfCnpj ?? string.Empty));

        notaTag.AddChild(AddTag(TipoCampo.Str, "", "tomador_email", 1, 120, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
        notaTag.AddChild(AddTag(TipoCampo.Int, "", "tomador_im", 1, 120, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal.OnlyNumbers()));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "tomador_ie", 1, 120, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoEstadual.OnlyNumbers()));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "tomador_razao", 1, 120, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Tomador.RazaoSocial.RemoveAccent() : nota.Tomador.RazaoSocial));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "tomador_endereco", 1, 120, Ocorrencia.NaoObrigatoria, RetirarAcentos ? nota.Tomador.Endereco.Logradouro.RemoveAccent() : nota.Tomador.Endereco.Logradouro));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "tomador_numero", 1, 9, Ocorrencia.NaoObrigatoria, string.IsNullOrEmpty(nota.Tomador.Endereco.Numero) ? "S/N" : nota.Tomador.Endereco.Numero));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "tomador_complemento", 1, 30, Ocorrencia.NaoObrigatoria, RetirarAcentos ? nota.Tomador.Endereco.Complemento.RemoveAccent() : nota.Tomador.Endereco.Complemento));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "tomador_bairro", 1, 50, Ocorrencia.NaoObrigatoria, RetirarAcentos ? nota.Tomador.Endereco.Bairro.RemoveAccent() : nota.Tomador.Endereco.Bairro));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "tomador_CEP", 1, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep.OnlyNumbers()));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "tomador_cod_cidade", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.CodigoMunicipio));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "tomador_fone", 0, 15, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.DDD.OnlyNumbers() + nota.Tomador.DadosContato.Telefone.OnlyNumbers()));

        if (!string.IsNullOrEmpty(nota.IdentificacaoRps.Numero) && !string.IsNullOrEmpty(nota.IdentificacaoRps.Serie))
        {
            notaTag.AddChild(AddTag(TipoCampo.Int, "", "rps_num", 1, 12, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "rps_serie", 1, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
            notaTag.AddChild(AddTag(TipoCampo.Int, "", "rps_dia", 1, 2, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao.Day));
            notaTag.AddChild(AddTag(TipoCampo.Int, "", "rps_mes", 1, 2, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao.Month));
            notaTag.AddChild(AddTag(TipoCampo.Int, "", "rps_ano", 1, 4, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao.Year));
        }

        if (nota.Tomador.Tipo == TipoTomador.PessoaJuridica && nota.Tomador.Endereco.CodigoMunicipio != nota.Prestador.Endereco.CodigoMunicipio)
        {
            notaTag.AddChild(AddTag(TipoCampo.Int, "", "outro_municipio", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.MunicipioIncidencia));

            if (nota.Servico.MunicipioIncidencia == 1)
            {
                notaTag.AddChild(AddTag(TipoCampo.Int, "", "cod_outro_municipio", 1, 10, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));
            }
        }

        //iis retido pode acontecer em varios casos, no manual está que apenas caso for fora do município, o que está incorreto segundo contadores da região.
        if (nota.Servico.Valores.ValorIssRetido > 0)
        {
            notaTag.AddChild(AddTag(TipoCampo.Str, "", "retencao_iss", 1, 15, Ocorrencia.Obrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.ValorIssRetido)));
        }

        notaTag.AddChild(AddTag(TipoCampo.Str, "", "pis", 1, 15, Ocorrencia.NaoObrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.ValorPis)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "cofins", 1, 15, Ocorrencia.NaoObrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.ValorCofins)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "inss", 1, 15, Ocorrencia.NaoObrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.ValorInss)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "irrf", 1, 15, Ocorrencia.NaoObrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.ValorIr)));
        notaTag.AddChild(AddTag(TipoCampo.Str, "", "csll", 1, 15, Ocorrencia.NaoObrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.ValorCsll)));

        notaTag.AddChild(AddTag(TipoCampo.Str, "", "dps_serv_cnbs", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.CodigoNbs));

        return xmldoc.Root.AsString(identado, showDeclaration, Encoding.UTF8);
    }

    public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        throw new NotImplementedException("Metodo WriteXmlNFSe não implementado");
    }

    #endregion Public

    #region Services

    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (notas.Count > 1) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Apenas o envio de uma nota por vez é permitido para esse serviço." });
        if (notas.Count == 0) retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "RPS não informado." });
        var nota = notas.FirstOrDefault() ?? throw new Exception("Nenhuma nota para ser enviada");
        nota.NumeroLote = retornoWebservice.Lote;

        var xmlRps = WriteXmlRps(nota, false, false);
        GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);

        var xmlLote = new StringBuilder();
        xmlLote.Append(xmlRps);

        retornoWebservice.XmlEnvio = xmlLote.ToString();
    }

    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
        //não assina
    }

    //GerarNotaResponse
    protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        var root = xmlRet.ElementAnyNs("GerarNotaResponse");
        var errors = root.ElementAnyNs("DescricaoErros");

        if (errors != null && errors.HasElements)
        {
            foreach (var node in errors.Descendants().Where(x => x.Name.LocalName == "item"))
            {
                var errorDescricao = node.ElementAnyNs("DescricaoErro")?.Value ?? string.Empty;
                
                // Se tem DescricaoErro, é um erro real
                if (!string.IsNullOrEmpty(errorDescricao))
                {
                    var errorId = node.ElementAnyNs("id")?.Value ?? string.Empty;
                    var errorProcesso = node.ElementAnyNs("DescricaoProcesso")?.Value ?? string.Empty;
                    retornoWebservice.Erros.Add(new EventoRetorno() { Codigo = errorId, Correcao = errorProcesso, Descricao = errorDescricao });
                }
                else
                {
                    // Sem DescricaoErro = sucesso, extrair ID da nota do DescricaoProcesso
                    var descricaoProcesso = node.ElementAnyNs("DescricaoProcesso")?.Value ?? string.Empty;
                    
                    // Extrair o ID da nota da mensagem (ex: "...NFS-e:42161")
                    var match = System.Text.RegularExpressions.Regex.Match(descricaoProcesso, @"NFS-e:(\d+)");
                    if (match.Success)
                    {
                        retornoWebservice.Protocolo = match.Groups[1].Value;
                    }
                }
            }
        }

        if (retornoWebservice.Erros.Any())
        {
            retornoWebservice.Sucesso = false;
        }
        else
        {
            retornoWebservice.Sucesso = true;
            retornoWebservice.Lote = 0; //Não tem lote nesse serviço
        }
    }

    //CancelarNota
    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        var xmldoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
        var cancelamentoXml = new XElement("DadosCancelaNota");
        xmldoc.Add(cancelamentoXml);

        cancelamentoXml.AddChild(AddTag(TipoCampo.Int, "", "ccm", 1, 120, Ocorrencia.Obrigatoria, Configuracoes.WebServices.Usuario.OnlyNumbers()));
        cancelamentoXml.AddChild(AddTag(TipoCampo.Str, "", "cnpj", 1, 14, Ocorrencia.Obrigatoria, Configuracoes.PrestadorPadrao.CpfCnpj.OnlyNumbers()));
        cancelamentoXml.AddChild(AddTag(TipoCampo.Str, "", "senha", 1, 120, Ocorrencia.Obrigatoria, Configuracoes.WebServices.Senha));
        cancelamentoXml.AddChild(AddTag(TipoCampo.Int, "", "nota", 1, 15, Ocorrencia.Obrigatoria, retornoWebservice.NumeroNFSe.OnlyNumbers()));
        cancelamentoXml.AddChild(AddTag(TipoCampo.Str, "", "motivo", 1, 3000, Ocorrencia.Obrigatoria, retornoWebservice.Motivo));
        cancelamentoXml.AddChild(AddTag(TipoCampo.Str, "", "email", 1, 120, Ocorrencia.NaoObrigatoria, retornoWebservice.CodigoCancelamento));

        retornoWebservice.XmlEnvio = cancelamentoXml.ToString();
    }

    //CancelarNota Não utiliza
    protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        //não assina
    }

    //CancelarNota
    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        var root = xmlRet.ElementAnyNs("CancelarNotaResponse");
        var data = root.ElementAnyNs("RetornoNota") ?? throw new Exception("Elemento do xml RetornoNota não encontado");
        var errors = root.ElementAnyNs("DescricaoErros") ?? throw new Exception("Elemento do xml DescricaoErros não encontado");

        var resultado = data.ElementAnyNs("Resultado")?.GetValue<int>() ?? 0;
        //var linkImpressao = data.ElementAnyNs("LinkImpressao")?.GetValue<string>() ?? string.Empty; //não utilizado pois não tem como retornar

        if (resultado != 1 && errors.HasElements)
        {
            retornoWebservice.Sucesso = false;
            foreach (var node in errors.Descendants().Where(x => x.Name == "item"))
            {
                var errorId = node.ElementAnyNs("id")?.Value ?? string.Empty;
                var errorProcesso = node.ElementAnyNs("DescricaoProcesso")?.Value ?? string.Empty;
                var errorDescricao = node.ElementAnyNs("DescricaoErro")?.Value ?? string.Empty;
                retornoWebservice.Erros.Add(new EventoRetorno() { Codigo = errorId, Correcao = errorProcesso, Descricao = errorDescricao });
            }
        }
        else
        {
            retornoWebservice.Sucesso = true;
        }
    }

    //ConsultarNotaValida - provedor suporta porem não implementado por conta da dificuldade de dados para procurar nota.
    protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada neste Provedor.");
    }

    //ConsultarNotaValida
    protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada neste Provedor.");
    }

    //ConsultarNotaValida - provedor suporta porem  não implementado por conta da dificuldade de dados para procurar nota.
    protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada neste Provedor.");
    }

    protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        var loteBuilder = new StringBuilder();
        loteBuilder.Append($"<ConsultarNotaPrestador>");
        loteBuilder.Append("<DadosPrestador>");

        loteBuilder.Append("<ccm>");
        loteBuilder.Append(Configuracoes.WebServices.Usuario);
        loteBuilder.Append("</ccm>");

        if (Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ() == false)
            throw new Exception("PrestadorPadrao.CpfCnpj não preenchido ou foi informado CPF e esse metodo só aceita cnpj");

        loteBuilder.Append($"<cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</cnpj>");

        loteBuilder.Append("<senha>");
        loteBuilder.Append(Configuracoes.WebServices.Senha);
        loteBuilder.Append("</senha>");

        loteBuilder.Append("</DadosPrestador>");

        loteBuilder.Append($"<Nota>{retornoWebservice.NumeroNFse}</Nota>");
        loteBuilder.Append("</ConsultarNotaPrestador>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
    }

    protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
    {
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        var root = xmlRet.ElementAnyNs("ConsultarNotaPrestadorResponse");
        var dadosNota = root?.ElementAnyNs("DadosNota");
        var errors = root?.ElementAnyNs("DescricaoErros");

        // Verifica se há erros reais (quando id = 0 é erro, quando id = número da nota é sucesso)
        if (errors != null && errors.HasElements)
        {
            foreach (var node in errors.Descendants().Where(x => x.Name.LocalName == "item"))
            {
                var errorId = node.ElementAnyNs("id")?.GetValue<int>() ?? 0;
                
                // Se o id for 0, é um erro real
                if (errorId == 0)
                {
                    var errorProcesso = node.ElementAnyNs("DescricaoProcesso")?.Value ?? string.Empty;
                    var errorDescricao = node.ElementAnyNs("DescricaoErro")?.Value ?? string.Empty;
                    retornoWebservice.Erros.Add(new EventoRetorno() { Codigo = errorId.ToString(), Correcao = errorProcesso, Descricao = errorDescricao });
                }
            }
        }

        if (retornoWebservice.Erros.Any())
        {
            retornoWebservice.Sucesso = false;
            return;
        }

        if (dadosNota == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno() { Codigo = "", Descricao = "dadosNota = null" });
            return;
        }

        retornoWebservice.Sucesso = true;

        //retorno de dados.
        var nota = notas.FirstOrDefault() ?? new NotaServico(Configuracoes);
        
        // Dados da NFSe
        nota.IdentificacaoNFSe.Numero = dadosNota.ElementAnyNs("nota")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoNFSe.Chave = dadosNota.ElementAnyNs("autenticidade")?.GetValue<string>() ?? string.Empty;
        
        var dtConversao = dadosNota.ElementAnyNs("dt_conversao")?.GetValue<string>();
        if (!string.IsNullOrEmpty(dtConversao) && DateTime.TryParse(dtConversao, out var dataEmissao))
        {
            nota.IdentificacaoNFSe.DataEmissao = dataEmissao;
        }

        // Link de Impressão
        nota.LinkNFSe = dadosNota.ElementAnyNs("LinkImpressao")?.GetValue<string>() ?? string.Empty;

        // Dados do RPS
        nota.IdentificacaoRps.Numero = dadosNota.ElementAnyNs("num_rps")?.GetValue<string>() ?? string.Empty;
        nota.IdentificacaoRps.Serie = dadosNota.ElementAnyNs("serie_rps")?.GetValue<string>() ?? string.Empty;
        
        var emissaoRps = dadosNota.ElementAnyNs("emissao_rps")?.GetValue<string>();
        if (!string.IsNullOrEmpty(emissaoRps) && DateTime.TryParse(emissaoRps, out var dataEmissaoRps))
        {
            nota.IdentificacaoRps.DataEmissao = dataEmissaoRps;
        }

        // Dados do Prestador
        nota.Prestador.RazaoSocial = dadosNota.ElementAnyNs("prestador_razao")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Logradouro = dadosNota.ElementAnyNs("prestador_endereco")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Numero = dadosNota.ElementAnyNs("prestador_numero")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Complemento = dadosNota.ElementAnyNs("prestador_complemento")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Bairro = dadosNota.ElementAnyNs("prestador_bairro")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Municipio = dadosNota.ElementAnyNs("prestador_cidade")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Uf = dadosNota.ElementAnyNs("prestador_estado")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.Endereco.Cep = dadosNota.ElementAnyNs("prestador_cep")?.GetValue<string>() ?? string.Empty;
        nota.Prestador.DadosContato.Email = dadosNota.ElementAnyNs("prestador_email")?.GetValue<string>() ?? string.Empty;

        // Dados do Serviço
        nota.Servico.Descricao = dadosNota.ElementAnyNs("descricao")?.GetValue<string>() ?? string.Empty;
        nota.Servico.CodigoTributacaoMunicipio = dadosNota.ElementAnyNs("servico")?.GetValue<string>() ?? string.Empty;
        nota.Servico.Valores.ValorServicos = ParseDecimalSigiss(dadosNota.ElementAnyNs("valor")?.GetValue<string>());
        nota.Servico.Valores.BaseCalculo = ParseDecimalSigiss(dadosNota.ElementAnyNs("base")?.GetValue<string>());
        nota.Servico.Valores.Aliquota = ParseDecimalSigiss(dadosNota.ElementAnyNs("aliquota_atividade")?.GetValue<string>());
        nota.Servico.Valores.ValorIss = ParseDecimalSigiss(dadosNota.ElementAnyNs("iss")?.GetValue<string>());
        nota.Servico.Valores.ValorPis = ParseDecimalSigiss(dadosNota.ElementAnyNs("pis")?.GetValue<string>());
        nota.Servico.Valores.ValorCofins = ParseDecimalSigiss(dadosNota.ElementAnyNs("cofins")?.GetValue<string>());
        nota.Servico.Valores.ValorInss = ParseDecimalSigiss(dadosNota.ElementAnyNs("inss")?.GetValue<string>());
        nota.Servico.Valores.ValorIr = ParseDecimalSigiss(dadosNota.ElementAnyNs("irrf")?.GetValue<string>());
        nota.Servico.Valores.ValorCsll = ParseDecimalSigiss(dadosNota.ElementAnyNs("csll")?.GetValue<string>());

        // ISS Retido
        var issRetido = dadosNota.ElementAnyNs("ISSRetido")?.GetValue<string>() ?? string.Empty;
        nota.Servico.Valores.IssRetido = issRetido.Equals("SIM", StringComparison.OrdinalIgnoreCase) ? SituacaoTributaria.Retencao : SituacaoTributaria.Normal;

        // Dados do Tomador
        nota.Tomador.CpfCnpj = dadosNota.ElementAnyNs("cnpj_tomador")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.RazaoSocial = dadosNota.ElementAnyNs("razao_tomador")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Logradouro = dadosNota.ElementAnyNs("endereco_tomador")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Numero = dadosNota.ElementAnyNs("numero_tomador")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Complemento = dadosNota.ElementAnyNs("complemento_tomador")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Bairro = dadosNota.ElementAnyNs("bairro_tomador")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Municipio = dadosNota.ElementAnyNs("cidade_tomador")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Uf = dadosNota.ElementAnyNs("estado_tomador")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.Endereco.Cep = dadosNota.ElementAnyNs("cep_tomador")?.GetValue<string>() ?? string.Empty;
        nota.Tomador.DadosContato.Email = dadosNota.ElementAnyNs("email_tomador")?.GetValue<string>() ?? string.Empty;

        // Situação da NFSe
        var statusNFe = dadosNota.ElementAnyNs("StatusNFe")?.GetValue<string>() ?? string.Empty;
        nota.Situacao = statusNFe.Equals("Ativa", StringComparison.OrdinalIgnoreCase) ? SituacaoNFSeRps.Normal : SituacaoNFSeRps.Cancelado;

        nota.XmlOriginal = retornoWebservice.XmlRetorno;

        if (!notas.Contains(nota))
            notas.Add(nota);

        retornoWebservice.Notas = notas.ToArray();
    }

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor.");
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor.");
    }

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor.");
    }

    protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        var loteBuilder = new StringBuilder();
        loteBuilder.Append($"<ConsultarLoteRpsEnvio xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
        loteBuilder.Append("<Prestador>");
        loteBuilder.Append("<CpfCnpj>");
        loteBuilder.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
            ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>"
            : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
        loteBuilder.Append("</CpfCnpj>");
        loteBuilder.Append("</Prestador>");
        loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");
        loteBuilder.Append("</ConsultarLoteRpsEnvio>");
        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice) { }

    protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

        retornoWebservice.Lote = xmlRet.Root?.ElementAnyNs("NumeroLote")?.GetValue<int>() ?? 0;

        var retornoLote = xmlRet.ElementAnyNs("ConsultarLoteRpsResposta");
        var situacao = retornoLote?.ElementAnyNs("Situacao");
        if (situacao != null)
        {
            switch (situacao.GetValue<int>())
            {
                case 2:
                    retornoWebservice.Situacao = "2 – Não Processado";
                    break;

                case 3:
                    retornoWebservice.Situacao = "3 – Processado com Erro";
                    break;

                case 4:
                    retornoWebservice.Situacao = "4 – Processado com Sucesso";
                    break;

                default:
                    retornoWebservice.Situacao = "1 – Não Recebido";
                    break;
            }
        }

        MensagemErro(retornoWebservice, xmlRet, "ConsultarLoteRpsResposta");
        if (retornoWebservice.Erros.Any()) return;
        if (retornoWebservice.XmlRetorno.Contains("ListaMensagemRetorno"))
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = retornoWebservice.XmlRetorno });
            return;
        }
        if (notas == null) return;

        retornoWebservice.Sucesso = true;

        var listaNfse = retornoLote?.ElementAnyNs("ListaNfse");

        if (listaNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
            return;
        }

        var notasFiscais = new List<NotaServico>();

        foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
        {
            var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
            var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            var chaveNFSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
            var dataNFSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
            var numeroRps = nfse.ElementAnyNs("DeclaracaoPrestacaoServico")?
                .ElementAnyNs("InfDeclaracaoPrestacaoServico")?
                .ElementAnyNs("Rps")?
                .ElementAnyNs("IdentificacaoRps")?
                .ElementAnyNs("Numero").GetValue<string>() ?? string.Empty;

            GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota == null)
            {
                nota = notas.Load(compNfse.ToString());
            }
            else
            {
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.IdentificacaoNFSe.Chave = chaveNFSe;
                nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
                nota.XmlOriginal = compNfse.ToString();
            }

            nota.Protocolo = retornoWebservice.Protocolo;
            notasFiscais.Add(nota);
        }

        retornoWebservice.Notas = notasFiscais.ToArray();
    }

    private static void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string xmlTag)
    {
        var mensagens = xmlRet?.ElementAnyNs(xmlTag);
        mensagens = mensagens?.ElementAnyNs("Messages");
        if (mensagens == null)
            return;

        foreach (var mensagem in mensagens.ElementsAnyNs("Message"))
        {
            retornoWs.Erros.Add(new EventoRetorno
            {
                Codigo = mensagem?.ElementAnyNs("Id")?.GetValue<string>() ?? string.Empty,
                Descricao = mensagem?.ElementAnyNs("Description")?.GetValue<string>() ?? string.Empty,
                Correcao = mensagem?.ElementAnyNs("Description")?.GetValue<string>() ?? string.Empty
            });
        }
    }

    protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    #endregion Services

    #region Private

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new SigISS100ServiceClient(this, tipo);
    }

    protected override string GerarCabecalho()
    {
        return "";
    }

    protected override string GetSchema(TipoUrl tipo)
    {
        return "";
    }

    protected override bool PrecisaValidarSchema(TipoUrl tipo)
    {
        return false;
    }

    private string FormataDecimalModeloSigiss(decimal valor)
    {
        var formatado = valor.ToString("0.00").Replace(".", ",");
        var trim = formatado.Contains(",") ? formatado.TrimEnd('0').TrimEnd(',') : formatado;
        return trim;
    }

    private decimal ParseDecimalSigiss(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return 0;
        
        // Remove espaços e converte vírgula para ponto
        valor = valor.Trim().Replace(",", ".");
        
        return decimal.TryParse(valor, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result) 
            ? result 
            : 0;
    }

    #endregion Private

    #endregion Methods
}