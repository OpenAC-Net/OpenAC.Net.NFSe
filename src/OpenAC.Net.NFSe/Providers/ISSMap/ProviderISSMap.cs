// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : danilobreda
// Created          : 22-05-2026
//
// Last Modified By : danilobreda
// Last Modified On : 22-05-2026
// ***********************************************************************
// <copyright file="ProviderISSMap.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014 - 2026 Projeto OpenAC .Net
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
// <summary>
// Provedor ISSMap (Gemmap Informática). Comunicação RESTFul sobre HTTPS, trafegando
// XML cujos conteúdos das tags são criptografados em AES-128/ECB (exceto a tag key).
// Manual de referência: "Manual de Integração RPS" v4.4.x.
// </summary>
// ***********************************************************************

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderISSMap : ProviderBase
{
    #region Constructors

    public ProviderISSMap(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "ISSMap";
        Versao = VersaoNFSe.ve100;
    }

    #endregion Constructors

    #region Methods

    #region Public

    public override NotaServico LoadXml(XDocument xml)
    {
        throw new NotImplementedException("Metodo LoadXml não implementado");
    }

    /// <summary>
    /// Monta o XML <c>&lt;rps&gt;</c> do ISSMap com os conteúdos em claro.
    /// A criptografia AES dos conteúdos é aplicada posteriormente em <see cref="AssinarEnviar"/>.
    /// </summary>
    public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        var prestador = Configuracoes.PrestadorPadrao;
        var valores = nota.Servico.Valores;
        var retido = valores.IssRetido == SituacaoTributaria.Retencao;

        var rps = new XElement("rps");

        // key é o ÚNICO campo que NÃO é criptografado.
        rps.Add(new XElement("key", Configuracoes.WebServices.Usuario));
        rps.Add(new XElement("pass", Configuracoes.WebServices.Senha));

        rps.Add(new XElement("numero", nota.IdentificacaoRps.Numero));
        rps.Add(new XElement("serie", nota.IdentificacaoRps.Serie));
        rps.Add(new XElement("numeroNfse", string.Empty));
        rps.Add(new XElement("dataHoraEmissao", nota.IdentificacaoRps.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)));
        rps.Add(new XElement("status", string.Empty));
        rps.Add(new XElement("motivoCancelamento", string.Empty));

        rps.Add(new XElement("descServico", Truncar(nota.Servico.Descricao, 2000)));
        rps.Add(new XElement("servicoPrestado", Truncar(string.IsNullOrWhiteSpace(nota.Servico.Discriminacao) ? nota.Servico.Descricao : nota.Servico.Discriminacao, 1000)));
        rps.Add(new XElement("codigoServicoPrestado", FormatarCodigoServico(nota.Servico.ItemListaServico)));
        rps.Add(new XElement("cNBS", (nota.Servico.CodigoNbs ?? string.Empty).OnlyNumbers()));

        rps.Add(new XElement("retidoNaFonte", retido ? "S" : "N"));
        rps.Add(new XElement("localExecucao", nota.Servico.CodigoMunicipio.ToString()));

        rps.Add(new XElement("valorNota", FormataValor(valores.ValorServicos)));
        rps.Add(new XElement("valorBaseCalculo", FormataValor(valores.BaseCalculo)));
        rps.Add(new XElement("valorDeducoes", FormataValor(valores.ValorDeducoes)));
        rps.Add(new XElement("valorIss", FormataValor(valores.ValorIss)));
        rps.Add(new XElement("aliquota", FormataValor(valores.Aliquota)));

        rps.Add(new XElement("aliquotaEstadual", FormataValor(valores.AliquotaTotalEstadual ?? 0)));
        rps.Add(new XElement("aliquotaFederal", FormataValor(valores.AliquotaTotalFederal ?? 0)));
        rps.Add(new XElement("aliquotaMunicipal", FormataValor(valores.AliquotaTotalMunicipal ?? valores.Aliquota)));

        // Prestador
        rps.Add(new XElement("cpfCnpjPrestador", prestador.CpfCnpj.OnlyNumbers()));
        rps.Add(new XElement("nomeRazaoPrestador", Truncar(prestador.RazaoSocial, 120)));
        rps.Add(new XElement("enderecoPrestador", Truncar(MontaEndereco(prestador.Endereco), 120)));
        rps.Add(new XElement("cidadePrestador", Truncar(prestador.Endereco.Municipio, 100)));
        rps.Add(new XElement("estadoPrestador", prestador.Endereco.Uf));
        rps.Add(new XElement("cepPrestador", prestador.Endereco.Cep.OnlyNumbers()));
        rps.Add(new XElement("imPrestador", (prestador.InscricaoMunicipal ?? string.Empty).Trim()));
        rps.Add(new XElement("ieRgPrestador", string.Empty));
        rps.Add(new XElement("emailPrestador", prestador.DadosContato.Email ?? string.Empty));

        // Tomador
        rps.Add(new XElement("cpfCnpjTomador", (nota.Tomador.CpfCnpj ?? string.Empty).OnlyNumbers()));
        rps.Add(new XElement("nomeRazaoTomador", Truncar(nota.Tomador.RazaoSocial, 120)));
        rps.Add(new XElement("enderecoTomador", Truncar(MontaEndereco(nota.Tomador.Endereco), 120)));
        rps.Add(new XElement("cidadeTomador", Truncar(nota.Tomador.Endereco.Municipio, 100)));
        rps.Add(new XElement("estadoTomador", nota.Tomador.Endereco.Uf));
        rps.Add(new XElement("cepTomador", (nota.Tomador.Endereco.Cep ?? string.Empty).OnlyNumbers()));
        rps.Add(new XElement("imTomador", (nota.Tomador.InscricaoMunicipal ?? string.Empty).Trim()));
        rps.Add(new XElement("ieRgTomador", (nota.Tomador.InscricaoEstadual ?? string.Empty).Trim()));
        rps.Add(new XElement("emailTomador", nota.Tomador.DadosContato.Email ?? string.Empty));

        // Impostos retidos na fonte (não obrigatórios)
        rps.Add(new XElement("valorPIS", FormataValor(valores.ValorPis)));
        rps.Add(new XElement("valorCOFINS", FormataValor(valores.ValorCofins)));
        rps.Add(new XElement("valorCSLL", FormataValor(valores.ValorCsll)));
        rps.Add(new XElement("valorIRRF", FormataValor(valores.ValorIr)));
        rps.Add(new XElement("valorINSS", FormataValor(valores.ValorInss)));
        rps.Add(new XElement("valorOutros", FormataValor(valores.ValorOutrasRetencoes)));

        rps.Add(new XElement("retencaoObrigatoriaTomador", "false"));
        rps.Add(new XElement("tpRetPisCofins", valores.ValorPis > 0 || valores.ValorCofins > 0 ? "1" : "2"));

        var xmldoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), rps);
        return xmldoc.Root!.AsString(identado, showDeclaration, Encoding.UTF8);
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
        if (retornoWebservice.Erros.Any()) return;

        var nota = notas[0];
        retornoWebservice.XmlEnvio = WriteXmlRps(nota, false, false);
    }

    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
        // No ISSMap "assinar" = criptografar os conteúdos das tags (exceto a tag key).
        retornoWebservice.XmlEnvio = CriptografarXml(retornoWebservice.XmlEnvio);
    }

    protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        var retorno = retornoWebservice.XmlRetorno ?? string.Empty;

        XDocument xmlRet;
        try
        {
            xmlRet = XDocument.Parse(retorno);
        }
        catch
        {
            retornoWebservice.Sucesso = false;
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Retorno não reconhecido: " + retorno });
            return;
        }

        var codigo = ExtrairCodigoResposta(xmlRet);

        if (string.IsNullOrEmpty(codigo))
        {
            retornoWebservice.Sucesso = false;
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Retorno não reconhecido: " + retorno });
            return;
        }

        if (codigo != "100")
        {
            retornoWebservice.Sucesso = false;
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = codigo, Descricao = DescricaoErro(codigo) });
            return;
        }

        retornoWebservice.Sucesso = true;

        // Quando o serviço devolve a NFS-e gerada, tenta extrair o número da nota (campos criptografados).
        try
        {
            var doc = XDocument.Parse(DescriptografarXml(retorno));
            var numero = doc.Descendants()
                .FirstOrDefault(x => !x.HasElements &&
                                     (x.Name.LocalName.Equals("numeroNFSE", StringComparison.OrdinalIgnoreCase) ||
                                      x.Name.LocalName.Equals("numeroNfse", StringComparison.OrdinalIgnoreCase)))
                ?.Value?.Trim() ?? string.Empty;

            retornoWebservice.Protocolo = numero;
            retornoWebservice.XmlRetorno = doc.ToString(SaveOptions.DisableFormatting);
        }
        catch
        {
            // Resposta simples, sem NFS-e embutida (ex.: teste/enviar) — mantém apenas Sucesso = true.
        }
    }

    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        var carta = new XElement("cartaCancelamento");
        carta.Add(new XElement("key", Configuracoes.WebServices.Usuario));
        carta.Add(new XElement("pass", Configuracoes.WebServices.Senha));
        carta.Add(new XElement("cpf_cnpj_pre", Configuracoes.PrestadorPadrao.CpfCnpj.OnlyNumbers()));
        carta.Add(new XElement("cpf_cnpj_tom", (retornoWebservice.CodigoVerificacao ?? string.Empty).OnlyNumbers()));
        carta.Add(new XElement("num_RPS", retornoWebservice.NumeroNFSe.OnlyNumbers()));
        carta.Add(new XElement("motivoCancelamento", Truncar(retornoWebservice.Motivo, 500)));

        var xmldoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), carta);
        retornoWebservice.XmlEnvio = xmldoc.Root!.AsString(false, false, Encoding.UTF8);
    }

    protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        retornoWebservice.XmlEnvio = CriptografarXml(retornoWebservice.XmlEnvio);
    }

    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        TratarRespostaIssMap(retornoWebservice, null);
    }

    protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        // O serviço de consulta do ISSMap usa GET com parâmetros no path:
        // /ws/rps/consulta/[cidade]/[docPrestador]/[docTomador]/[numeroRPS]
        // O ISSMap não tem conceito de "série" na consulta, então o campo SerieNFse do
        // RetornoConsultarNFSe é reaproveitado para transportar o CPF/CNPJ do tomador
        // (exigido na URL) - assim a fachada OpenNFSe.ConsultaNFSe(numero, serie) já atende.
        var docPrestador = Configuracoes.PrestadorPadrao.CpfCnpj.OnlyNumbers();
        var docTomador = (retornoWebservice.SerieNFse ?? string.Empty).OnlyNumbers();
        var numeroRps = retornoWebservice.NumeroNFse.ToString();

        retornoWebservice.XmlEnvio = $"/{docPrestador}/{docTomador}/{numeroRps}";
    }

    protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        // Consulta é GET, sem corpo a criptografar.
    }

    protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
    {
        var retorno = retornoWebservice.XmlRetorno ?? string.Empty;

        if (string.IsNullOrWhiteSpace(retorno) || retorno.Contains("<rps/>") || retorno.Contains("<rps />"))
        {
            retornoWebservice.Sucesso = false;
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "206", Descricao = DescricaoErro("206") });
            return;
        }

        // Quando encontrado, o RPS retorna com os conteúdos criptografados.
        retornoWebservice.Sucesso = true;
        retornoWebservice.XmlRetorno = DescriptografarXml(retorno);
    }

    #region Não implementados neste provedor

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException("Função não implementada/suportada neste Provedor.");

    #endregion Não implementados neste provedor

    #endregion Services

    #region Private

    protected override IServiceClient GetClient(TipoUrl tipo) => new ISSMapServiceClient(this, tipo);

    protected override string GerarCabecalho() => string.Empty;

    protected override string GetSchema(TipoUrl tipo) => string.Empty;

    protected override bool PrecisaValidarSchema(TipoUrl tipo) => false;

    /// <summary>
    /// Criptografa (AES-128/ECB) o conteúdo de todas as tags folha do XML, exceto a tag <c>key</c>.
    /// </summary>
    private string CriptografarXml(string xml)
    {
        var chave = Configuracoes.WebServices.ChaveAcesso;
        if (string.IsNullOrWhiteSpace(chave))
            throw new OpenException("Chave de Acesso (criptografia) do ISSMap não informada em Configuracoes.WebServices.ChaveAcesso.");

        var doc = XDocument.Parse(xml);
        foreach (var element in doc.Descendants())
        {
            if (element.HasElements) continue;
            if (element.Name.LocalName.Equals("key", StringComparison.OrdinalIgnoreCase)) continue;

            element.Value = ISSMapCripto.Criptografar(element.Value, chave);
        }

        return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" + doc.Root!.ToString(SaveOptions.DisableFormatting);
    }

    /// <summary>
    /// Descriptografa o conteúdo de todas as tags folha de um XML retornado pelo ISSMap, exceto a tag <c>key</c>.
    /// </summary>
    private string DescriptografarXml(string xml)
    {
        var chave = Configuracoes.WebServices.ChaveAcesso;
        var doc = XDocument.Parse(xml);
        foreach (var element in doc.Descendants())
        {
            if (element.HasElements) continue;
            if (element.Name.LocalName.Equals("key", StringComparison.OrdinalIgnoreCase)) continue;
            if (string.IsNullOrWhiteSpace(element.Value)) continue;

            try
            {
                element.Value = ISSMapCripto.Descriptografar(element.Value, chave);
            }
            catch
            {
                // mantém o valor original caso a tag não esteja criptografada
            }
        }

        return doc.ToString(SaveOptions.DisableFormatting);
    }

    /// <summary>
    /// Interpreta a resposta padrão do ISSMap. Código 100 indica sucesso; demais códigos
    /// seguem a tabela de erros do manual.
    /// </summary>
    private void TratarRespostaIssMap(RetornoWebservice retornoWebservice, NotaServico? nota)
    {
        var retorno = retornoWebservice.XmlRetorno ?? string.Empty;

        string codigo;
        try
        {
            codigo = ExtrairCodigoResposta(XDocument.Parse(retorno));
        }
        catch
        {
            retornoWebservice.Sucesso = false;
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Retorno não reconhecido: " + retorno });
            return;
        }

        if (codigo == "100")
        {
            retornoWebservice.Sucesso = true;
            return;
        }

        retornoWebservice.Sucesso = false;
        retornoWebservice.Erros.Add(new EventoRetorno { Codigo = codigo, Descricao = DescricaoErro(codigo) });
    }

    /// <summary>
    /// Extrai o código de resposta do ISSMap, que aparece em formatos variados conforme o serviço:
    /// <c>&lt;resposta&gt;&lt;codigo&gt;NNN&lt;/codigo&gt;&lt;/resposta&gt;</c>,
    /// <c>&lt;nfse&gt;&lt;resposta&gt;NNN&lt;/resposta&gt;&lt;/nfse&gt;</c> ou
    /// <c>&lt;resposta&gt;NNN&lt;/resposta&gt;</c>.
    /// </summary>
    private static string ExtrairCodigoResposta(XDocument doc)
    {
        // A tag <codigo> tem prioridade.
        var codigoEl = doc.Descendants()
            .FirstOrDefault(x => !x.HasElements && x.Name.LocalName.Equals("codigo", StringComparison.OrdinalIgnoreCase));
        if (codigoEl != null && !string.IsNullOrWhiteSpace(codigoEl.Value))
            return codigoEl.Value.Trim();

        // Caso contrário, <resposta> como folha numérica.
        var respostaEl = doc.Descendants()
            .FirstOrDefault(x => !x.HasElements && x.Name.LocalName.Equals("resposta", StringComparison.OrdinalIgnoreCase));
        if (respostaEl != null && int.TryParse(respostaEl.Value.Trim(), out _))
            return respostaEl.Value.Trim();

        return string.Empty;
    }

    private static string FormataValor(decimal valor) => valor.ToString("0.00", CultureInfo.InvariantCulture);

    /// <summary>
    /// Formata o código do serviço no padrão nacional de 6 dígitos exigido pelo ISSMap
    /// (99 item, 88 subitem, 77 desdobro). O item da lista (LC 116, ex.: "99.99") é
    /// completado à direita com o desdobro "00" quando não informado.
    /// </summary>
    private static string FormatarCodigoServico(string? itemListaServico)
    {
        var codigo = (itemListaServico ?? string.Empty).OnlyNumbers();
        if (codigo.Length == 0) return string.Empty;
        return codigo.Length >= 6 ? codigo.Substring(0, 6) : codigo.PadRight(6, '0');
    }

    private static string MontaEndereco(Endereco endereco)
    {
        var logradouro = (endereco.Logradouro ?? string.Empty).Trim();
        var numero = string.IsNullOrWhiteSpace(endereco.Numero) ? "S/N" : endereco.Numero.Trim();
        return $"{logradouro}, {numero}".Trim(',', ' ');
    }

    private static string Truncar(string? valor, int tamanho)
    {
        valor = (valor ?? string.Empty).Trim();
        return valor.Length > tamanho ? valor.Substring(0, tamanho) : valor;
    }

    /// <summary>
    /// Descrição dos códigos de resposta do WebService ISSMap (tabela de Códigos de Resposta do manual).
    /// </summary>
    private static string DescricaoErro(string codigo) => codigo switch
    {
        "100" => "Processo concluído com sucesso.",
        "200" => "Erro no recebimento dos dados.",
        "201" => "Código da chave de criptografia (campo Key) inválida.",
        "202" => "Erro de criptografia.",
        "203" => "Cadastro Contribuinte Mobiliário (CCM) não existe.",
        "204" => "CNPJ/CPF do prestador não é o mesmo do CCM dono da chave.",
        "205" => "Senha incorreta ou inválida.",
        "206" => "RPS inválida (não encontrou a RPS).",
        "207" => "RPS foi cancelada.",
        "208" => "RPS possui uma solicitação de cancelamento.",
        "209" => "O prazo para cancelamento da nota expirou.",
        "210" => "Bloqueio de competência - notas em aberto para geração de guia de ISSQN.",
        "212" => "Contribuinte não liberado para emitir RPS.",
        "213" => "Contribuinte está bloqueado no sistema IssMap.",
        "221" => "Erro na validação do campo Data/Hora de Emissão da Nota.",
        "222" => "Erro na validação do campo Descrição do Serviço da Nota.",
        "223" => "Erro na validação do campo Flag de Cancelamento da Nota.",
        "224" => "Erro na validação do campo Motivo do Cancelamento da Nota.",
        "225" => "Erro na validação do campo Flag Retido na Fonte.",
        "226" => "Erro na validação do campo Local de Execução.",
        "227" => "Erro na validação do campo Código do Serviço Prestado.",
        "228" => "Erro na validação do campo Serviço Prestado.",
        "229" => "Erro na validação do campo Valor da Nota.",
        "230" => "Erro na validação do campo Valor das Deduções.",
        "231" => "Erro na validação do campo Valor da Base de Cálculo.",
        "232" => "Erro na validação do campo Alíquota.",
        "233" => "Erro na validação do campo Valor do ISS.",
        "235" => "Retenção não permitida.",
        "236" => "Retenção não permitida para pessoa física.",
        "237" => "E-mail do Tomador obrigatório em retenções.",
        "241" => "Erro na validação do campo Nome/Razão do Prestador.",
        "242" => "Erro na validação do campo Endereço do Prestador.",
        "243" => "Erro na validação do campo Cidade do Prestador.",
        "244" => "Erro na validação do campo Inscrição Municipal do Prestador.",
        "245" => "Erro na validação do campo Inscrição Estadual/RG do Prestador.",
        "246" => "Erro na validação do campo Estado do Prestador.",
        "247" => "Erro na validação do campo CEP do Prestador.",
        "248" => "Erro na validação do campo Email do Prestador.",
        "251" => "Erro na validação do campo Nome/Razão do Tomador.",
        "252" => "Erro na validação do campo Endereço do Tomador.",
        "253" => "Erro na validação do campo Cidade do Tomador.",
        "254" => "Erro na validação do campo Inscrição Municipal do Tomador.",
        "255" => "Erro na validação do campo Inscrição Estadual/RG do Tomador.",
        "256" => "Erro na validação do campo Estado do Tomador.",
        "257" => "Erro na validação do campo CEP do Tomador.",
        "259" => "Erro na validação da Pass.",
        "294" => "Solicitação de integração indeferida.",
        "295" => "Integração bloqueada.",
        "296" => "Emissão de RPS não permitida em Homologação.",
        "297" => "Sua cidade não possui emissão de RPS.",
        "298" => "Erro ao tentar enviar a nota para o ambiente nacional.",
        "299" => "Campo cNBS obrigatório não foi informado ou está incorreto.",
        "300" => "Campo aliquotaEstadual obrigatório não foi informado.",
        "301" => "Campo aliquotaFederal obrigatório não foi informado.",
        "302" => "Campo aliquotaMunicipal obrigatório não foi informado.",
        "303" => "Campo serie obrigatório não foi informado ou está incorreto.",
        "304" => "Campo tpRetPisCofins obrigatório não foi informado ou está incorreto.",
        _ => $"Erro retornado pelo WebService ISSMap (código {codigo})."
    };

    #endregion Private

    #endregion Methods
}
