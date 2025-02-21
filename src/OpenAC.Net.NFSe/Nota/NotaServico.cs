// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 10-01-2014
//
// Last Modified By : Rafael Dias
// Last Modified On : 10-01-2014
// ***********************************************************************
// <copyright file="NotaServico.cs" company="OpenAC .Net">
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
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.Core.Generics;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Document;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Nota;

/// <summary>
/// 
/// </summary>
public sealed class NotaServico : GenericClone<NotaServico>, INotifyPropertyChanged
{
    #region Events

    /// <inheritdoc />
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events

    #region Fields

    private readonly ConfigNFSe config;

    #endregion Fields

    #region Constructor

    public NotaServico(ConfigNFSe config)
    {
        Guard.Against<OpenDFeException>(config == null, "Configurações não podem ser nulas");

        this.config = config;

        Id = 0;
        IdentificacaoRps = new IdeRps();
        IdentificacaoNFSe = new IdeNFSe();
        RpsSubstituido = new IdeRpsSubtituida();
        Servico = new DadosServico();
        EnderecoPrestacao = new Endereco();
        Prestador = new DadosPrestador();
        Tomador = new DadosTomador();
        Intermediario = new DadosIntermediario();
        ConstrucaoCivil = new DadosConstrucaoCivil();
        Material = new DadosMateriais();
        Pagamento = new DadosPagamento();
        OrgaoGerador = new IdeOrgaoGerador();
        Signature = new DFeSignature();
        Cancelamento = new IdeCancelamento();
        Transportadora = new DadosTransportadora();
        Emails = new EmailCollection();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotaServico"/> class.
    /// </summary>
    public NotaServico(ConfigNFSe config, DadosPrestador prestador) : this(config)
    {
        Prestador = prestador;
    }

    #endregion Constructor

    #region Propriedades

    public int Id { get; set; }

    public IdeRps IdentificacaoRps { get; }

    public IdeNFSe IdentificacaoNFSe { get; }

    public IdeRpsSubtituida RpsSubstituido { get; }

    public IdeCancelamento Cancelamento { get; }

    public IdeOrgaoGerador OrgaoGerador { get; }

    public DadosPrestador Prestador { get; internal set; }

    public DadosTomador Tomador { get; }

    public DadosIntermediario Intermediario { get; }

    public DadosServico Servico { get; }

    public Endereco EnderecoPrestacao { get; set; }

    public DadosConstrucaoCivil ConstrucaoCivil { get; }

    public DadosMateriais Material { get; set; }

    public DadosTransportadora Transportadora { get; }

    public DadosPagamento Pagamento { get; }

    public EmailCollection Emails { get; }

    public int NaturezaOperacao { get; set; }

    public RegimeEspecialTributacao RegimeEspecialTributacao { get; set; }

    public NFSeSimNao OptanteSimplesNacional { get; set; }

    public NFSeSimNao OptanteMEISimei { get; set; }

    public DateTime DataOptanteSimplesNacional { get; set; }

    public NFSeSimNao? IncentivadorCultural { get; set; }

    public NFSeSimNao Producao { get; set; }

    public SituacaoNFSeRps Situacao { get; set; }

    public TipoLocalServico LocalServico { get; set; }

    public int NumeroLote { get; set; }

    public string Protocolo { get; set; }

    public DateTime Competencia { get; set; }

    public string OutrasInformacoes { get; set; }

    public string DiscriminacaoImpostos { get; set; }

    public string InformacoesComplementares { get; set; }

    public string DescricaoCodigoTributacaoMunicipio { get; set; }

    public decimal ValorCredito { get; set; }

    public TipoEmissao TipoEmissao { get; set; }

    public TipoEmpreitadaGlobal EmpreitadaGlobal { get; set; }

    public TipoTributacao TipoTributacao { get; set; }

    public string Assinatura { get; set; }

    public DFeSignature Signature { get; set; }

    public string XmlOriginal { get; set; }

    public string LinkNFSe { get; set; }

    public EventoRps? Evento { get; set; }

    #endregion Propriedades

    #region Methods

    /// <summary>
    /// Salvar o xml da Rps/NFSe no determinado arquivo
    /// </summary>
    /// <param name="path">Caminho onde sera salvo o arquivo.</param>
    /// <returns></returns>
    public void Save(string path = "")
    {
        var isNFSe = IdentificacaoNFSe.Numero.IsEmpty();

        var file = isNFSe
            ? $"Rps-{IdentificacaoRps.DataEmissao:yyyyMMdd}-{IdentificacaoRps.Numero}-{IdentificacaoRps.Serie}.xml"
            : $"NFSe-{IdentificacaoNFSe.Chave}-{IdentificacaoNFSe.Numero}.xml";

        var xmlNota = GetXml();

        path = Path.Combine(path, file);

        var doc = XDocument.Parse(xmlNota);
        doc.Save(path, SaveOptions.OmitDuplicateNamespaces);
    }

    /// <summary>
    /// Salvar o xml da Rps/NFSe no determinado arquivo
    /// </summary>
    /// <param name="stream">Stream onde sera salvo o xml</param>
    /// <returns></returns>
    public void Save(Stream stream)
    {
        var xmlNota = GetXml();

        var doc = XDocument.Parse(xmlNota);
        doc.Save(stream, SaveOptions.OmitDuplicateNamespaces);
    }

    /// <summary>
    /// Gera o Xml Da Rps
    /// </summary>
    /// <returns></returns>
    public string GetXml()
    {
        var provider = ProviderManager.GetProvider(config);

        try
        {
            return IdentificacaoNFSe.Numero.IsEmpty() ? provider.WriteXmlRps(this) : provider.WriteXmlNFSe(this);
        }
        finally
        {
            provider.Dispose();
        }
    }

    #endregion Methods
}