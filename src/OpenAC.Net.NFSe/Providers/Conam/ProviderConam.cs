using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderConam : ProviderBase
{
    #region Internal Types

    private List<decimal> valoresTipo30 = new List<decimal>();

    #endregion Internal Types

    #region Constructors

    public ProviderConam(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "Conam";
    }

    #endregion Constructors

    #region Methods

    #region XML

    public override NotaServico LoadXml(XDocument xml)
    {
        Guard.Against<XmlException>(xml == null, "Xml invalido.");

        var ret = new NotaServico(Configuracoes)
        {
            XmlOriginal = xml.AsString()
        };

        var xmlElement = xml.ElementAnyNs("Nota");

        ret.Competencia = xmlElement.ElementAnyNs("DtEmiNf")?.GetValue<DateTime>() ?? DateTime.MinValue;

        //Dados da NFS-e
        ret.IdentificacaoNFSe.Numero = xmlElement.ElementAnyNs("NumNf")?.GetValue<string>() ?? string.Empty;
        ret.IdentificacaoNFSe.ModeloNfse = xmlElement.ElementAnyNs("SerNf")?.GetValue<string>() ?? string.Empty;
        ret.IdentificacaoNFSe.Chave = xmlElement.ElementAnyNs("CodVernf")?.GetValue<string>() ?? string.Empty;
        ret.IdentificacaoNFSe.DataEmissao = xmlElement.ElementAnyNs("DtEmiNf")?.GetValue<DateTime>() ?? DateTime.MinValue;
        ret.Situacao = SituacaoNFSeRps.Normal;

        if ((xmlElement.ElementAnyNs("SitNf")?.GetValue<string>() ?? string.Empty) == "2")
        {
            ret.Situacao = SituacaoNFSeRps.Cancelado;
            ret.Cancelamento.DataHora = xmlElement.ElementAnyNs("DataCncNf")?.GetValue<DateTime>() ?? DateTime.MinValue;
            ret.Cancelamento.MotivoCancelamento = xmlElement.ElementAnyNs("MotivoCncNf")?.GetValue<string>() ?? string.Empty;
        }

        //Dados do RPS
        ret.IdentificacaoRps.Numero = xmlElement.ElementAnyNs("NumRps")?.GetValue<string>() ?? string.Empty;
        ret.IdentificacaoRps.Serie = xmlElement.ElementAnyNs("SerRps")?.GetValue<string>() ?? string.Empty;
        ret.IdentificacaoRps.Tipo = TipoRps.RPS;
        ret.IdentificacaoRps.DataEmissao = xmlElement.ElementAnyNs("DtEmiRps")?.GetValue<DateTime>() ?? DateTime.MinValue;

        //Dados do Prestador
        ret.Prestador.CpfCnpj = xmlElement.ElementAnyNs("CpfCnpjPre")?.GetValue<string>() ?? string.Empty;
        ret.Prestador.RazaoSocial = xmlElement.ElementAnyNs("RazSocPre")?.GetValue<string>() ?? string.Empty;
        ret.Prestador.Endereco.Logradouro = xmlElement.ElementAnyNs("LogPre")?.GetValue<string>() ?? string.Empty;
        ret.Prestador.Endereco.Numero = xmlElement.ElementAnyNs("NumEndPre")?.GetValue<string>() ?? string.Empty;
        ret.Prestador.Endereco.Complemento = xmlElement.ElementAnyNs("ComplEndPre")?.GetValue<string>() ?? string.Empty;
        ret.Prestador.Endereco.Bairro = xmlElement.ElementAnyNs("BairroPre")?.GetValue<string>() ?? string.Empty;
        ret.Prestador.Endereco.Municipio = xmlElement.ElementAnyNs("MunPre")?.GetValue<string>() ?? string.Empty;
        ret.Prestador.Endereco.Uf = xmlElement.ElementAnyNs("SiglaUFPre")?.GetValue<string>() ?? string.Empty;
        ret.Prestador.Endereco.Cep = xmlElement.ElementAnyNs("CepPre")?.GetValue<string>() ?? string.Empty;
        ret.Prestador.DadosContato.Email = xmlElement.ElementAnyNs("EmailPre")?.GetValue<string>() ?? string.Empty;

        //Dados do Tomador
        ret.Tomador.CpfCnpj = xmlElement.ElementAnyNs("CpfCnpjTom")?.GetValue<string>() ?? string.Empty;
        ret.Tomador.RazaoSocial = xmlElement.ElementAnyNs("RazSocTom")?.GetValue<string>() ?? string.Empty;
        ret.Tomador.Endereco.Logradouro = xmlElement.ElementAnyNs("LogTom")?.GetValue<string>() ?? string.Empty;
        ret.Tomador.Endereco.Numero = xmlElement.ElementAnyNs("NumEndTom")?.GetValue<string>() ?? string.Empty;
        ret.Tomador.Endereco.Complemento = xmlElement.ElementAnyNs("ComplEndTom")?.GetValue<string>() ?? string.Empty;
        ret.Tomador.Endereco.Bairro = xmlElement.ElementAnyNs("BairroTom")?.GetValue<string>() ?? string.Empty;
        ret.Tomador.Endereco.Municipio = xmlElement.ElementAnyNs("MunTom")?.GetValue<string>() ?? string.Empty;
        ret.Tomador.Endereco.Uf = xmlElement.ElementAnyNs("SiglaUFTom")?.GetValue<string>() ?? string.Empty;
        ret.Tomador.Endereco.Cep = xmlElement.ElementAnyNs("CepTom")?.GetValue<string>() ?? string.Empty;
        ret.Tomador.DadosContato.Email = xmlElement.ElementAnyNs("EMailTom")?.GetValue<string>() ?? string.Empty;

        //Dados do Serviço
        ret.Servico.CodigoTributacaoMunicipio = xmlElement.ElementAnyNs("CodSrv")?.GetValue<string>() ?? string.Empty;
        ret.Servico.Discriminacao = xmlElement.ElementAnyNs("DiscrSrv")?.GetValue<string>() ?? string.Empty;
        ret.Servico.Valores.ValorServicos = xmlElement.ElementAnyNs("VlNFS")?.GetValue<decimal>() ?? 0;
        ret.Servico.Valores.ValorDeducoes = xmlElement.ElementAnyNs("VlDed")?.GetValue<decimal>() ?? 0;
        ret.Servico.Valores.JustificativaDeducao = xmlElement.ElementAnyNs("DiscrDed")?.GetValue<string>() ?? string.Empty;
        ret.Servico.Valores.BaseCalculo = xmlElement.ElementAnyNs("VlBasCalc")?.GetValue<decimal>() ?? 0;
        ret.Servico.Valores.Aliquota = xmlElement.ElementAnyNs("AlqIss")?.GetValue<decimal>() ?? 0;
        ret.Servico.Valores.ValorIss = xmlElement.ElementAnyNs("VlIss")?.GetValue<decimal>() ?? 0;
        ret.Servico.Valores.ValorIssRetido = xmlElement.ElementAnyNs("VlIssRet")?.GetValue<decimal>() ?? 0;

        return ret;
    }

    public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    #endregion XML

    #region RPS

    public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
    {
        valoresTipo30 = new List<decimal>();

        var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));

        var sdt = new XElement("SDTRPS");
        xmlDoc.AddChild(sdt);

        var tipoTrib = 1;

        if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
            tipoTrib = 4;

        switch (nota.TipoTributacao)
        {
            case TipoTributacao.Isenta:
                tipoTrib = 2;
                break;

            case TipoTributacao.Suspensa:
                tipoTrib = 3;
                break;

            case TipoTributacao.TributavelFixo:
                tipoTrib = 5;
                break;
        }

        sdt.AddChild(AddTag(TipoCampo.Int, "", "Ano", 0, 0, Ocorrencia.Obrigatoria, nota.Competencia.Year));
        sdt.AddChild(AddTag(TipoCampo.Int, "", "Mes", 0, 0, Ocorrencia.Obrigatoria, nota.Competencia.Month.ZeroFill(2)));
        sdt.AddChild(AddTag(TipoCampo.Str, "", "CPFCNPJ", 0, 0, Ocorrencia.Obrigatoria, nota.Prestador.CpfCnpj.ZeroFill(14)));
        sdt.AddChild(AddTag(TipoCampo.Str, "", "DTIni", 0, 0, Ocorrencia.Obrigatoria, nota.Competencia.ToString("01/MM/yyyy")));
        sdt.AddChild(AddTag(TipoCampo.Str, "", "DTFin", 0, 0, Ocorrencia.Obrigatoria, DateTime.Parse(nota.Competencia.AddMonths(1).ToString("01/MM/yyyy")).AddDays(-1).ToString("dd/MM/yyyy")));
        sdt.AddChild(AddTag(TipoCampo.Int, "", "TipoTrib", 0, 0, Ocorrencia.Obrigatoria, tipoTrib));
        sdt.AddChild(AddTag(TipoCampo.Str, "", "DtAdeSN", 0, 0, Ocorrencia.Obrigatoria, nota.DataOptanteSimplesNacional == DateTime.MinValue || tipoTrib != 4 ? "" : nota.DataOptanteSimplesNacional.ToString("dd/MM/yyyy")));
        sdt.AddChild(AddTag(TipoCampo.Str, "", "AlqIssSN_IP", 0, 0, Ocorrencia.Obrigatoria, tipoTrib != 4 ? "" : nota.Servico.Valores.Aliquota.ToString("##0.00")));
        sdt.AddChild(AddTag(TipoCampo.Str, "", "Versao", 0, 0, Ocorrencia.Obrigatoria, "2.00"));

        sdt.Add(WriteREG20(nota));

        sdt.Add(WriteREG90(nota));

        return xmlDoc.AsString(identado, showDeclaration);
    }

    private XElement WriteREG20(NotaServico nota)
    {
        var reg20 = new XElement("Reg20");
        var reg20Item = new XElement("Reg20Item");
        reg20.AddChild(reg20Item);

        reg20Item.AddChild(AddTag(TipoCampo.Str, "", "TipoNFS", 3, 3, Ocorrencia.Obrigatoria, "RPS"));
        reg20Item.AddChild(AddTag(TipoCampo.Int, "", "NumRps", 0, 0, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
        reg20Item.AddChild(AddTag(TipoCampo.Str, "", "SerRps", 1, 3, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
        reg20Item.AddChild(AddTag(TipoCampo.Str, "", "DtEmi", 0, 0, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao.ToString("dd/MM/yyyy")));
        reg20Item.AddChild(AddTag(TipoCampo.Str, "", "RetFonte", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? "SIM" : "NAO"));
        reg20Item.AddChild(AddTag(TipoCampo.Str, "", "CodSrv", 5, 5, Ocorrencia.Obrigatoria, nota.Servico.CodigoTributacaoMunicipio));
        reg20Item.AddChild(AddTag(TipoCampo.Str, "", "DiscrSrv", 1, 4000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
        reg20Item.AddChild(AddTag(TipoCampo.Str, "", "VlNFS", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos.ToString("##0.00")));
        reg20Item.AddChild(AddTag(TipoCampo.Str, "", "VlDed", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorDeducoes.ToString("##0.00")));
        reg20Item.AddChild(AddTag(TipoCampo.Str, "", "DiscrDed", 0, 4000, Ocorrencia.Obrigatoria, nota.Servico.Valores.JustificativaDeducao));
        reg20Item.AddChild(AddTag(TipoCampo.Str, "", "VlBasCalc", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.BaseCalculo.ToString("##0.00")));
        reg20Item.AddChild(AddTag(TipoCampo.Str, "", "AlqIss", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.Aliquota.ToString("##0.00")));
        reg20Item.AddChild(AddTag(TipoCampo.Str, "", "VlIss", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorIss.ToString("##0.00")));
        reg20Item.AddChild(AddTag(TipoCampo.Str, "", "VlIssRet", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorIssRetido.ToString("##0.00")));

        if (!string.IsNullOrEmpty(nota.Tomador.CpfCnpj))
        {
            reg20Item.AddChild(AddTag(TipoCampo.Str, "", "CpfCnpTom", 14, 14, Ocorrencia.Obrigatoria, nota.Tomador.CpfCnpj.Length <= 11 ? nota.Tomador.CpfCnpj.ZeroFill(11) : nota.Tomador.CpfCnpj.ZeroFill(14)));
            reg20Item.AddChild(AddTag(TipoCampo.Str, "", "RazSocTom", 1, 60, Ocorrencia.Obrigatoria, nota.Tomador.RazaoSocial));
            reg20Item.AddChild(AddTag(TipoCampo.Str, "", "TipoLogtom", 1, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.TipoLogradouro));
            reg20Item.AddChild(AddTag(TipoCampo.Str, "", "LogTom", 1, 60, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Logradouro));
            reg20Item.AddChild(AddTag(TipoCampo.Str, "", "NumEndTom", 1, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Numero));
            reg20Item.AddChild(AddTag(TipoCampo.Str, "", "ComplEndTom", 0, 60, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Complemento));
            reg20Item.AddChild(AddTag(TipoCampo.Str, "", "BairroTom", 1, 60, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Bairro));
            reg20Item.AddChild(AddTag(TipoCampo.Str, "", "MunTom", 1, 60, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Municipio));
            reg20Item.AddChild(AddTag(TipoCampo.Str, "", "SiglaUFTom", 2, 2, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Uf));
            reg20Item.AddChild(AddTag(TipoCampo.Str, "", "CepTom", 8, 8, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Cep.ZeroFill(8)));
            reg20Item.AddChild(AddTag(TipoCampo.Str, "", "Telefone", 10, 10, Ocorrencia.Obrigatoria, (nota.Tomador.DadosContato.DDD.OnlyNumbers() + nota.Tomador.DadosContato.Telefone.OnlyNumbers()).ZeroFill(10)));
            reg20Item.AddChild(AddTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 20, Ocorrencia.Obrigatoria, nota.Tomador.InscricaoMunicipal));

            if (
                !string.IsNullOrEmpty(nota.EnderecoPrestacao.Logradouro)
            )
            {
                if (
                    string.IsNullOrEmpty(nota.EnderecoPrestacao.Numero) ||
                    string.IsNullOrEmpty(nota.EnderecoPrestacao.Bairro) ||
                    string.IsNullOrEmpty(nota.EnderecoPrestacao.Municipio) ||
                    string.IsNullOrEmpty(nota.EnderecoPrestacao.Uf) ||
                    string.IsNullOrEmpty(nota.EnderecoPrestacao.Cep)
                )
                    throw new Exception("Para emissão com endereço de prestação do serviço diferente do endereço do tomador, informe os campos CEP, Logradouro, Numero, Bairro, Cidade e UF do local da prestação!");

                reg20Item.AddChild(AddTag(TipoCampo.Str, "", "TipoLogLocPre", 1, 10, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.TipoLogradouro));
                reg20Item.AddChild(AddTag(TipoCampo.Str, "", "LogLocPre", 1, 60, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Logradouro));
                reg20Item.AddChild(AddTag(TipoCampo.Str, "", "NumEndLocPre", 1, 10, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Numero));
                reg20Item.AddChild(AddTag(TipoCampo.Str, "", "ComplEndLocPre", 0, 60, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Complemento));
                reg20Item.AddChild(AddTag(TipoCampo.Str, "", "BairroLocPre", 1, 60, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Bairro));
                reg20Item.AddChild(AddTag(TipoCampo.Str, "", "MunLocPre", 1, 60, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Municipio));
                reg20Item.AddChild(AddTag(TipoCampo.Str, "", "SiglaUFLocpre", 2, 2, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Uf));
                reg20Item.AddChild(AddTag(TipoCampo.Str, "", "CepLocPre", 8, 8, Ocorrencia.NaoObrigatoria, nota.EnderecoPrestacao.Cep.ZeroFill(8)));
            }

            reg20Item.AddChild(AddTag(TipoCampo.Str, "", "Email1", 0, 120, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
        }
        else
        {
            reg20Item.AddChild(AddTag(TipoCampo.Str, "", "CpfCnpjTom", 1, 14, Ocorrencia.Obrigatoria, "CONSUMIDOR"));
        }

        if (
            nota.Servico.Valores.ValorCofins > 0 ||
            nota.Servico.Valores.ValorCsll > 0 ||
            nota.Servico.Valores.ValorInss > 0 ||
            nota.Servico.Valores.ValorIr > 0 ||
            nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ||
            nota.Servico.Valores.ValorPis > 0
        )
            reg20Item.Add(WriteREG30(nota));

        return reg20;
    }

    private XElement WriteREG30(NotaServico nota)
    {
        var reg30 = new XElement("Reg30");

        if (nota.Servico.Valores.ValorCofins > 0)
        {
            var reg30Item = new XElement("Reg30Item");
            reg30.AddChild(reg30Item);

            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoSigla", 1, 10, Ocorrencia.Obrigatoria, "COFINS"));
            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoAliquota", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaCofins.ToString("##0.00")));
            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoValor", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorCofins.ToString("##0.00")));

            valoresTipo30.Add(nota.Servico.Valores.ValorCofins);
        }

        if (nota.Servico.Valores.ValorCsll > 0)
        {
            var reg30Item = new XElement("Reg30Item");
            reg30.AddChild(reg30Item);

            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoSigla", 1, 10, Ocorrencia.Obrigatoria, "CSLL"));
            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoAliquota", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaCsll.ToString("##0.00")));
            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoValor", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorCsll.ToString("##0.00")));

            valoresTipo30.Add(nota.Servico.Valores.ValorCsll);
        }

        if (nota.Servico.Valores.ValorInss > 0)
        {
            var reg30Item = new XElement("Reg30Item");
            reg30.AddChild(reg30Item);

            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoSigla", 1, 10, Ocorrencia.Obrigatoria, "INSS"));
            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoAliquota", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaInss.ToString("##0.00")));
            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoValor", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorInss.ToString("##0.00")));

            valoresTipo30.Add(nota.Servico.Valores.ValorInss);
        }

        if (nota.Servico.Valores.ValorIr > 0)
        {
            var reg30Item = new XElement("Reg30Item");
            reg30.AddChild(reg30Item);

            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoSigla", 1, 10, Ocorrencia.Obrigatoria, "IR"));
            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoAliquota", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaIR.ToString("##0.00")));
            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoValor", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorIr.ToString("##0.00")));

            valoresTipo30.Add(nota.Servico.Valores.ValorIr);
        }

        //Não entendi ao certo, pois no manual fala isso, porém, olhando exemplos de clientes, esta tag não vai
        //if (nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao)
        //{
        //    var reg30Item = new XElement("Reg30Item");
        //    reg30.AddChild(reg30Item);

        //    reg30Item.AddChild(AdicionarTag(TipoCampo.Str, "", "TributoSigla", 1, 10, Ocorrencia.Obrigatoria, "ISS"));
        //    reg30Item.AddChild(AdicionarTag(TipoCampo.Str, "", "TributoAliquota", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.Aliquota.ToString("##0.00")));
        //    reg30Item.AddChild(AdicionarTag(TipoCampo.Str, "", "TributoValor", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorIssRetido.ToString("##0.00")));

        //    valoresTipo30.Add(nota.Servico.Valores.ValorIssRetido);
        //}

        if (nota.Servico.Valores.ValorPis > 0)
        {
            var reg30Item = new XElement("Reg30Item");
            reg30.AddChild(reg30Item);

            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoSigla", 1, 10, Ocorrencia.Obrigatoria, "PIS"));
            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoAliquota", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.AliquotaPis.ToString("##0.00")));
            reg30Item.AddChild(AddTag(TipoCampo.Str, "", "TributoValor", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorPis.ToString("##0.00")));

            valoresTipo30.Add(nota.Servico.Valores.ValorPis);
        }

        return reg30;
    }

    private XElement WriteREG90(NotaServico nota)
    {
        var reg90 = new XElement("Reg90");

        reg90.AddChild(AddTag(TipoCampo.Int, "", "QtdRegNormal", 0, 0, Ocorrencia.Obrigatoria, "1"));
        reg90.AddChild(AddTag(TipoCampo.Str, "", "ValorNFS", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos.ToString("##0.00")));
        reg90.AddChild(AddTag(TipoCampo.Str, "", "ValorISS", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorIss.ToString("##0.00")));
        reg90.AddChild(AddTag(TipoCampo.Str, "", "ValorDed", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorDeducoes.ToString("##0.00")));
        reg90.AddChild(AddTag(TipoCampo.Str, "", "ValorIssRetTom", 0, 0, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorIssRetido.ToString("##0.00")));
        reg90.AddChild(AddTag(TipoCampo.Int, "", "QtdReg30", 0, 0, Ocorrencia.Obrigatoria, valoresTipo30.Count));
        reg90.AddChild(AddTag(TipoCampo.Str, "", "ValorTributos", 0, 0, Ocorrencia.Obrigatoria, valoresTipo30.Sum().ToString("##0.00")));

        return reg90;
    }

    #endregion RPS

    #region Services

    protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        if (notas.Count == 0)
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "RPS não informado." });

        if (retornoWebservice.Erros.Count > 0)
            return;

        var xmlLoteRps = new StringBuilder();

        foreach (var nota in notas)
        {
            nota.XmlOriginal = WriteXmlRps(nota, false, false);
            xmlLoteRps.Append(nota.XmlOriginal);
            GravarRpsEmDisco(nota.XmlOriginal, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
        }

        var xmlLote = new StringBuilder();
        xmlLote.Append(xmlLoteRps);

        retornoWebservice.XmlEnvio = xmlLote.ToString();
    }

    protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
    {
    }

    protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

        if (!(xmlRet.Root.ElementAnyNs("Sdt_processarpsout").ElementAnyNs("Retorno")?.GetValue<bool>() ?? false))
        {
            MensagemErro(retornoWebservice, xmlRet.Root, "Sdt_processarpsout");
            retornoWebservice.Sucesso = false;

            return;
        }

        var xmlElement = xmlRet.Root.ElementAnyNs("Sdt_processarpsout");

        retornoWebservice.Lote = 0;
        retornoWebservice.Data = DateTime.Now;
        retornoWebservice.Protocolo = xmlElement.ElementAnyNs("Protocolo")?.GetValue<string>() ?? string.Empty;
        retornoWebservice.Sucesso = true;
    }

    protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        // Monta mensagem de envio
        var loteBuilder = new StringBuilder();
        loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");

        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
        // Analisa mensagem de retorno// Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        var sucesso = true;

        var xmlElement = xmlRet.Root.ElementAnyNs("Sdt_consultaprotocoloout");

        var situacao = "";
        switch (xmlElement.ElementAnyNs("PrtXSts")?.GetValue<string>() ?? "0")
        {
            case "1": //Aguardando processamento
                situacao = "2"; //Padrão Ginfes e ABRASF - Ainda não processado
                break;

            case "2": //Em Processamento
                situacao = "2"; //Padrão Ginfes e ABRASF - Ainda não processado
                break;

            case "3": //Rejeitado
                situacao = "3"; //Padrão Ginfes e ABRASF - Processado com Erros
                break;

            case "4": //Rejeitado Parcialmente
                situacao = "3"; //Padrão Ginfes e ABRASF - Processado com Erros
                break;

            case "5": //Processado
                situacao = "4"; //Padrão Ginfes e ABRASF - Processado
                break;
        }

        retornoWebservice.Lote = 0;
        retornoWebservice.Situacao = situacao;

        if (!(xmlElement.ElementAnyNs("Retorno")?.GetValue<bool>() ?? false))
        {
            MensagemErro(retornoWebservice, xmlRet.Root, "Sdt_consultaprotocoloout");
            retornoWebservice.Sucesso = false;

            return;
        }

        retornoWebservice.Sucesso = true;
    }

    protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
        // Monta mensagem de envio
        var loteBuilder = new StringBuilder();
        loteBuilder.Append($"<Protocolo>{retornoWebservice.Protocolo}</Protocolo>");

        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno// Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

        if (!(xmlRet.Root.ElementAnyNs("Sdt_consultanotasprotocoloout").ElementAnyNs("Retorno")?.GetValue<bool>() ?? false))
        {
            MensagemErro(retornoWebservice, xmlRet.Root, "Sdt_consultanotasprotocoloout");
            retornoWebservice.Sucesso = false;

            return;
        }

        var retornoLote = xmlRet.ElementAnyNs("Sdt_consultanotasprotocoloout");
        var listaNfse = retornoLote?.ElementAnyNs("XML_Notas");
        if (listaNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
            return;
        }

        retornoWebservice.Sucesso = true;

        var notasServico = new List<NotaServico>();

        foreach (var compNfse in listaNfse.ElementAnyNs("Reg20")?.ElementsAnyNs("Reg20Item"))
        {
            var nfse = compNfse;
            var numeroNFSe = nfse.ElementAnyNs("NumNf")?.GetValue<string>() ?? string.Empty;
            var chaveNFSe = nfse.ElementAnyNs("CodVernf")?.GetValue<string>() ?? string.Empty;
            var dataNFSe = nfse.ElementAnyNs("DtEmiNf")?.GetValue<DateTime>() ?? DateTime.Now;
            var numeroRps = nfse?.ElementAnyNs("NumRps")?.GetValue<string>() ?? string.Empty;

            GravarNFSeEmDisco(compNfse.ToString(), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota == null)
            {
                nota = LoadXml(compNfse.ToString());
            }
            else
            {
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.IdentificacaoNFSe.Chave = chaveNFSe;
            }

            notas.Add(nota);
            notasServico.Add(nota);
        }

        retornoWebservice.Notas = notasServico.ToArray();
    }

    protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        if (retornoWebservice.NumeroRps < 1)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Número do RPS não informado para a consulta." });
            return;
        }

        if (retornoWebservice.MesCompetencia <= 0 || retornoWebservice.MesCompetencia >= 13)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Você deve informar o mês de competência." });
            return;
        }

        if (retornoWebservice.AnoCompetencia <= 2000 || retornoWebservice.AnoCompetencia > DateTime.Now.Year)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Você deve informar o ano de competência." });
            return;
        }

        // Monta mensagem de envio
        var loteBuilder = new StringBuilder();
        loteBuilder.Append($"<Competencia_Mes>{retornoWebservice.MesCompetencia.ZeroFill(2)}</Competencia_Mes>");
        loteBuilder.Append($"<Competencia_Ano>{retornoWebservice.AnoCompetencia}</Competencia_Ano>");
        loteBuilder.Append($"<RPS_Serie>{retornoWebservice.Serie}</RPS_Serie>");
        loteBuilder.Append($"<RPS_Numero>{retornoWebservice.NumeroRps}</RPS_Numero>");
        loteBuilder.Append($"<Nota_Serie></Nota_Serie>");
        loteBuilder.Append($"<Nota_Numero></Nota_Numero>");

        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        var xmlString = xmlRet.Root.ElementAnyNs("Xml_saida").GetValue<string>();
        xmlRet = XDocument.Parse(xmlString);

        MensagemErro2(retornoWebservice, xmlRet);
        if (retornoWebservice.Erros.Any()) return;

        var xmlElement = xmlRet.ElementAnyNs("SDT_IMPRESSAO_OUT");

        var listaNfse = xmlElement.ElementAnyNs("Lista_Notas");
        if (listaNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (Nota)" });
            return;
        }

        var notasServico = new List<NotaServico>();

        foreach (var compNfse in listaNfse.ElementsAnyNs("Nota"))
        {
            // Carrega a nota fiscal na coleção de Notas Fiscais
            var nota = LoadXml(compNfse.AsString());
            notas.Add(nota);
            notasServico.Add(nota);
        }

        retornoWebservice.Nota = notasServico.FirstOrDefault();
        retornoWebservice.Sucesso = true;
    }

    protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
        if (retornoWebservice.NumeroNFse < 1)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Número do NFSe não informado para a consulta." });
            return;
        }

        if (retornoWebservice.Inicio == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Você deve informar a competência no campo Início." });
            return;
        }

        // Monta mensagem de envio
        var loteBuilder = new StringBuilder();
        loteBuilder.Append($"<Competencia_Mes>{retornoWebservice.Inicio?.Month.ZeroFill(2)}</Competencia_Mes>");
        loteBuilder.Append($"<Competencia_Ano>{retornoWebservice.Inicio?.Year}</Competencia_Ano>");
        loteBuilder.Append($"<RPS_Serie></RPS_Serie>");
        loteBuilder.Append($"<RPS_Numero></RPS_Numero>");
        loteBuilder.Append($"<Nota_Serie>{retornoWebservice.SerieNFse}</Nota_Serie>");
        loteBuilder.Append($"<Nota_Numero>{retornoWebservice.NumeroNFse}</Nota_Numero>");

        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        var xmlString = xmlRet.Root.ElementAnyNs("Xml_saida").GetValue<string>();
        xmlRet = XDocument.Parse(xmlString);

        MensagemErro2(retornoWebservice, xmlRet);
        if (retornoWebservice.Erros.Any()) return;

        var xmlElement = xmlRet.ElementAnyNs("SDT_IMPRESSAO_OUT");

        var listaNfse = xmlElement.ElementAnyNs("Lista_Notas");
        if (listaNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (Nota)" });
            return;
        }

        var notasServico = new List<NotaServico>();

        foreach (var compNfse in listaNfse.ElementsAnyNs("Nota"))
        {
            // Carrega a nota fiscal na coleção de Notas Fiscais
            var nota = LoadXml(compNfse.AsString());
            notas.Add(nota);
            notasServico.Add(nota);
        }

        retornoWebservice.Notas = notasServico.ToArray();
        retornoWebservice.Sucesso = true;
    }

    protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
    {
        if (retornoWebservice.NumeroNFSe.IsEmpty())
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Número da NFSe não informado para cancelamento." });
            return;
        }

        var loteBuilder = new StringBuilder();
        loteBuilder.Append($"<SerieNota>NFE</SerieNota>");
        loteBuilder.Append($"<NumeroNota>{retornoWebservice.NumeroNFSe}</NumeroNota>");
        loteBuilder.Append($"<ValorNota>{retornoWebservice.ValorNFSe.ToString("##0.00").Replace(",", ".")}</ValorNota>");
        loteBuilder.Append($"<MotivoCancelamento>{retornoWebservice.Motivo}</MotivoCancelamento>");
        loteBuilder.Append($"<PodeCancelarGuia>S</PodeCancelarGuia>");

        retornoWebservice.XmlEnvio = loteBuilder.ToString();
    }

    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno// Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

        if (!(xmlRet.Root.ElementAnyNs("Sdt_retornocancelanfe").ElementAnyNs("Retorno")?.GetValue<bool>() ?? false))
        {
            MensagemErro(retornoWebservice, xmlRet.Root, "Sdt_retornocancelanfe");
            retornoWebservice.Sucesso = false;

            return;
        }

        retornoWebservice.Sucesso = true;
        retornoWebservice.Data = DateTime.MinValue;
    }

    protected override bool PrecisaValidarSchema(TipoUrl tipo)
    {
        return false;
    }

    protected override IServiceClient GetClient(TipoUrl tipo)
    {
        return new ConamServiceClient(this, tipo);
    }

    #endregion Services

    #region Not Implemented Methods

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
    {
    }

    protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
    {
    }

    protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
    }

    protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
    {
    }

    protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
    {
    }

    protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
    {
    }

    protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice)
    {
    }

    protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice)
    {
    }

    protected override void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override string GetSchema(TipoUrl tipo)
    {
        throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
    }

    protected override string GerarCabecalho()
    {
        return "";
    }

    #endregion Not Implemented Methods

    #endregion Methods

    #region Private Methods

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

    private static void MensagemErro2(RetornoWebservice retornoWs, XContainer xmlRet)
    {
        if (xmlRet.ElementAnyNs("SDT_IMPRESSAO_OUT").ElementAnyNs("Sucesso").GetValue<bool>())
            return;

        var mensagem = xmlRet.ElementAnyNs("SDT_IMPRESSAO_OUT").ElementAnyNs("Message");

        var evento = new EventoRetorno
        {
            Codigo = mensagem?.ElementAnyNs("Id")?.GetValue<string>() ?? string.Empty,
            Descricao = mensagem?.ElementAnyNs("Description")?.GetValue<string>() ?? string.Empty,
            Correcao = mensagem?.ElementAnyNs("Description")?.GetValue<string>() ?? string.Empty
        };

        retornoWs.Erros.Add(evento);
    }

    #endregion Private Methods
}