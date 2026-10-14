using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

/// <summary>
/// Serialização e assinatura do RPS do Layout 2 da Prefeitura de São Paulo.
/// </summary>
public static class ISSSaoPauloLayout2
{
    /// <summary>
    /// Serializa um RPS no Layout 2.
    /// </summary>
    public static string WriteXmlRps(NotaServico nota, X509Certificate2 certificado, bool identado = true,
        bool showDeclaration = true)
    {
        if (certificado == null)
            throw new ArgumentNullException(nameof(certificado));

        var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null), WriteRps(nota, certificado));
        return xmlDoc.AsString(identado, showDeclaration, Encoding.UTF8);
    }

    /// <summary>
    /// Monta a cadeia ASCII definida nas páginas 46 a 48 do manual NFe_Web_Service 3.3.7.
    /// </summary>
    public static string MontarCadeiaAssinatura(NotaServico nota)
    {
        Validar(nota);

        var tipoTributacao = ObterTipoTributacao(nota.TipoTributacao);
        var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "N" : "C";
        var issRetido = nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? "S" : "N";
        var valorCobrado = nota.Servico.Valores.ValorInicialCobrado ??
                           nota.Servico.Valores.ValorFinalCobrado!.Value;

        ObterDocumentoTomador(nota, out var indicadorTomador, out var documentoTomador, out var nifNaoNif);

        var cadeia = new StringBuilder();
        cadeia.Append(PreencherNumero(nota.Prestador.InscricaoMunicipal, 12, "Inscrição Municipal do prestador"));
        cadeia.Append(PreencherSerie(nota.IdentificacaoRps.Serie));
        cadeia.Append(PreencherNumero(nota.IdentificacaoRps.Numero, 12, "Número do RPS"));
        cadeia.Append(nota.IdentificacaoRps.DataEmissao.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
        cadeia.Append(tipoTributacao);
        cadeia.Append(situacao);
        cadeia.Append(issRetido);
        cadeia.Append(FormatarValorAssinatura(valorCobrado));
        cadeia.Append(FormatarValorAssinatura(nota.Servico.Valores.ValorDeducoes));
        cadeia.Append(PreencherNumero(nota.Servico.ItemListaServico, 5, "Código do serviço"));
        cadeia.Append(indicadorTomador);
        cadeia.Append(documentoTomador);

        if (!string.IsNullOrWhiteSpace(nota.Intermediario.CpfCnpj))
        {
            var documentoIntermediario = SomenteDigitos(nota.Intermediario.CpfCnpj,
                "CPF/CNPJ do intermediário");
            var indicadorIntermediario = documentoIntermediario.Length switch
            {
                11 => "1",
                14 => "2",
                _ => throw new OpenException("Layout 2 de São Paulo: CPF/CNPJ do intermediário deve conter 11 ou 14 dígitos.")
            };

            cadeia.Append(indicadorIntermediario);
            cadeia.Append(documentoIntermediario.PadLeft(14, '0'));
            cadeia.Append(nota.Intermediario.IssRetido == SituacaoTributaria.Retencao ? "S" : "N");
        }

        cadeia.Append(nifNaoNif);
        return cadeia.ToString();
    }

    /// <summary>
    /// Assina a cadeia do Layout 2 com RSA-SHA1 e preenchimento PKCS#1 v1.5.
    /// </summary>
    public static string AssinarRps(NotaServico nota, RSA rsa)
    {
        if (rsa == null)
            throw new ArgumentNullException(nameof(rsa));

        var cadeia = MontarCadeiaAssinatura(nota);
        var assinatura = rsa.SignData(Encoding.ASCII.GetBytes(cadeia), HashAlgorithmName.SHA1,
            RSASignaturePadding.Pkcs1);
        return Convert.ToBase64String(assinatura);
    }

    /// <summary>
    /// Retorna o código de retenção de PIS/COFINS/CSLL específico do Layout 2 de São Paulo.
    /// </summary>
    public static int ObterRetencaoPisCofins(ValoresServico valores)
    {
        if (valores == null)
            throw new ArgumentNullException(nameof(valores));

        var pis = valores.ValorPis > 0;
        var cofins = valores.ValorCofins > 0;
        var csll = valores.ValorCsll > 0;

        var combinacao = (pis ? 1 : 0) | (cofins ? 2 : 0) | (csll ? 4 : 0);
        return combinacao switch
        {
            0 => 0,
            7 => 3,
            3 => 4,
            1 => 5,
            2 => 6,
            6 => 7,
            4 => 8,
            5 => 9,
            _ => throw new InvalidOperationException()
        };
    }

    private static XElement WriteRps(NotaServico nota, X509Certificate2 certificado)
    {
        Validar(nota);

        using var rsa = certificado.GetRSAPrivateKey();
        if (rsa == null)
            throw new OpenException("Layout 2 de São Paulo: o certificado não possui chave privada RSA.");

        var valores = nota.Servico.Valores;
        var rps = new XElement("RPS",
            new XElement("Assinatura", AssinarRps(nota, rsa)),
            new XElement("ChaveRPS",
                new XElement("InscricaoPrestador", nota.Prestador.InscricaoMunicipal),
                string.IsNullOrWhiteSpace(nota.IdentificacaoRps.Serie)
                    ? null
                    : new XElement("SerieRPS", nota.IdentificacaoRps.Serie),
                new XElement("NumeroRPS", nota.IdentificacaoRps.Numero)),
            new XElement("TipoRPS", ObterTipoRps(nota.IdentificacaoRps.Tipo)),
            new XElement("DataEmissao", nota.IdentificacaoRps.DataEmissao.ToString("yyyy-MM-dd",
                CultureInfo.InvariantCulture)),
            new XElement("StatusRPS", nota.Situacao == SituacaoNFSeRps.Normal ? "N" : "C"),
            new XElement("TributacaoRPS", ObterTipoTributacao(nota.TipoTributacao)),
            ElementoValor("ValorDeducoes", valores.ValorDeducoes),
            ElementoValor("ValorPIS", valores.ValorPis),
            ElementoValor("ValorCOFINS", valores.ValorCofins),
            ElementoValor("ValorINSS", valores.ValorInss),
            ElementoValor("ValorIR", valores.ValorIr),
            ElementoValor("ValorCSLL", valores.ValorCsll),
            new XElement("CodigoServico", nota.Servico.ItemListaServico),
            new XElement("AliquotaServicos", (valores.Aliquota / 100M).ToString("0.0000",
                CultureInfo.InvariantCulture)),
            new XElement("ISSRetido", valores.IssRetido == SituacaoTributaria.Retencao
                ? "true"
                : "false"));

        WriteTomador(rps, nota);
        WriteIntermediario(rps, nota);

        rps.Add(new XElement("Discriminacao", nota.Servico.Discriminacao));

        if (valores.ValorCargaTributaria > 0)
            rps.Add(ElementoValor("ValorCargaTributaria", valores.ValorCargaTributaria));
        if (valores.AliquotaCargaTributaria > 0)
            rps.Add(new XElement("PercentualCargaTributaria",
                (valores.AliquotaCargaTributaria / 100M).ToString("0.0000", CultureInfo.InvariantCulture)));
        if (!string.IsNullOrWhiteSpace(valores.FonteCargaTributaria))
            rps.Add(new XElement("FonteCargaTributaria", valores.FonteCargaTributaria));
        if (!string.IsNullOrWhiteSpace(nota.ConstrucaoCivil.CodigoCEI))
            rps.Add(new XElement("CodigoCEI", nota.ConstrucaoCivil.CodigoCEI));
        if (!string.IsNullOrWhiteSpace(nota.ConstrucaoCivil.Matricula))
            rps.Add(new XElement("MatriculaObra", nota.ConstrucaoCivil.Matricula));
        if (nota.Material.NumeroEncapsulamento > 0)
            rps.Add(new XElement("NumeroEncapsulamento", nota.Material.NumeroEncapsulamento));

        rps.Add(new XElement("RetencaoPisCofins", ObterRetencaoPisCofins(valores)));
        rps.Add(valores.ValorInicialCobrado.HasValue
            ? ElementoValor("ValorInicialCobrado", valores.ValorInicialCobrado.Value)
            : ElementoValor("ValorFinalCobrado", valores.ValorFinalCobrado!.Value));
        rps.Add(ElementoValor("ValorIPI", valores.ValorIpi!.Value));
        rps.Add(new XElement("ExigibilidadeSuspensa", valores.ExigibilidadeSuspensa!.Value ? 1 : 0));
        rps.Add(new XElement("NBS", nota.Servico.CodigoNbs));

        var codigoLocalPrestacao = nota.Servico.MunicipioIncidencia > 0
            ? nota.Servico.MunicipioIncidencia
            : nota.Servico.CodigoMunicipio;
        rps.Add(new XElement("cLocPrestacao", codigoLocalPrestacao));
        rps.Add(WriteIBSCBS(nota));

        return rps;
    }

    private static XElement WriteIBSCBS(NotaServico nota)
    {
        var info = nota.Servico.Valores.IBSCBS!;
        var codigoOperacao = ObterCodigoOperacao(nota);
        var classificacao = ObterClassificacaoTributaria(nota);

        return new XElement("IBSCBS",
            new XElement("finNFSe", info.FinalidadeNFSe),
            new XElement("indFinal", info.IndicadorFinal),
            new XElement("cIndOp", codigoOperacao),
            new XElement("indDest", info.IndicadorDestinatario),
            new XElement("valores",
                new XElement("trib",
                    new XElement("gIBSCBS",
                        new XElement("cClassTrib", classificacao)))));
    }

    private static void WriteTomador(XElement rps, NotaServico nota)
    {
        var tomador = nota.Tomador;
        if (!string.IsNullOrWhiteSpace(tomador.CpfCnpj) ||
            !string.IsNullOrWhiteSpace(tomador.DocEstrangeiro) ||
            !string.IsNullOrWhiteSpace(tomador.CodNaoNif))
        {
            var documento = new XElement("CPFCNPJTomador");
            if (!string.IsNullOrWhiteSpace(tomador.CpfCnpj))
            {
                var cpfCnpj = SomenteDigitos(tomador.CpfCnpj, "CPF/CNPJ do tomador");
                documento.Add(cpfCnpj.Length switch
                {
                    11 => new XElement("CPF", cpfCnpj),
                    14 => new XElement("CNPJ", cpfCnpj),
                    _ => throw new OpenException("Layout 2 de São Paulo: CPF/CNPJ do tomador deve conter 11 ou 14 dígitos.")
                });
            }
            else if (!string.IsNullOrWhiteSpace(tomador.DocEstrangeiro))
            {
                documento.Add(new XElement("NIF", tomador.DocEstrangeiro));
            }
            else
            {
                documento.Add(new XElement("NaoNIF", tomador.CodNaoNif));
            }

            rps.Add(documento);
        }

        if (!string.IsNullOrWhiteSpace(tomador.InscricaoMunicipal))
            rps.Add(new XElement("InscricaoMunicipalTomador", tomador.InscricaoMunicipal));
        if (!string.IsNullOrWhiteSpace(tomador.InscricaoEstadual))
            rps.Add(new XElement("InscricaoEstadualTomador", tomador.InscricaoEstadual));
        if (!string.IsNullOrWhiteSpace(tomador.RazaoSocial))
            rps.Add(new XElement("RazaoSocialTomador", tomador.RazaoSocial));

        if (!string.IsNullOrWhiteSpace(tomador.Endereco.Logradouro))
        {
            var endereco = new XElement("EnderecoTomador");
            if (!string.IsNullOrWhiteSpace(tomador.Endereco.TipoLogradouro))
                endereco.Add(new XElement("TipoLogradouro", tomador.Endereco.TipoLogradouro));
            endereco.Add(new XElement("Logradouro", tomador.Endereco.Logradouro));
            endereco.Add(new XElement("NumeroEndereco", tomador.Endereco.Numero));
            if (!string.IsNullOrWhiteSpace(tomador.Endereco.Complemento))
                endereco.Add(new XElement("ComplementoEndereco", tomador.Endereco.Complemento));
            if (!string.IsNullOrWhiteSpace(tomador.Endereco.Bairro))
                endereco.Add(new XElement("Bairro", tomador.Endereco.Bairro));
            if (tomador.Endereco.CodigoMunicipio > 0)
                endereco.Add(new XElement("Cidade", tomador.Endereco.CodigoMunicipio));
            if (!string.IsNullOrWhiteSpace(tomador.Endereco.Uf))
                endereco.Add(new XElement("UF", tomador.Endereco.Uf));
            if (!string.IsNullOrWhiteSpace(tomador.Endereco.Cep))
                endereco.Add(new XElement("CEP", SomenteDigitos(tomador.Endereco.Cep, "CEP do tomador")));
            rps.Add(endereco);
        }

        if (!string.IsNullOrWhiteSpace(tomador.DadosContato.Email))
            rps.Add(new XElement("EmailTomador", tomador.DadosContato.Email));
    }

    private static void WriteIntermediario(XElement rps, NotaServico nota)
    {
        if (string.IsNullOrWhiteSpace(nota.Intermediario.CpfCnpj))
            return;

        var cpfCnpj = SomenteDigitos(nota.Intermediario.CpfCnpj, "CPF/CNPJ do intermediário");
        var documento = new XElement("CPFCNPJIntermediario",
            cpfCnpj.Length switch
            {
                11 => new XElement("CPF", cpfCnpj),
                14 => new XElement("CNPJ", cpfCnpj),
                _ => throw new OpenException("Layout 2 de São Paulo: CPF/CNPJ do intermediário deve conter 11 ou 14 dígitos.")
            });
        rps.Add(documento);

        if (!string.IsNullOrWhiteSpace(nota.Intermediario.InscricaoMunicipal))
            rps.Add(new XElement("InscricaoMunicipalIntermediario", nota.Intermediario.InscricaoMunicipal));
        rps.Add(new XElement("ISSRetidoIntermediario",
            nota.Intermediario.IssRetido == SituacaoTributaria.Retencao ? "true" : "false"));
        if (!string.IsNullOrWhiteSpace(nota.Intermediario.EMail))
            rps.Add(new XElement("EmailIntermediario", nota.Intermediario.EMail));
    }

    private static void Validar(NotaServico nota)
    {
        if (nota == null)
            throw new ArgumentNullException(nameof(nota));

        _ = PreencherNumero(nota.Prestador.InscricaoMunicipal, 12, "Inscrição Municipal do prestador");
        _ = PreencherSerie(nota.IdentificacaoRps.Serie);
        _ = PreencherNumero(nota.IdentificacaoRps.Numero, 12, "Número do RPS");
        _ = PreencherNumero(nota.Servico.ItemListaServico, 5, "Código do serviço");

        var valores = nota.Servico.Valores;
        if (valores.ValorInicialCobrado.HasValue == valores.ValorFinalCobrado.HasValue)
            throw new OpenException("Layout 2 de São Paulo: informe exatamente um entre ValorInicialCobrado e ValorFinalCobrado.");
        if (!valores.ValorIpi.HasValue)
            throw new OpenException("Layout 2 de São Paulo: ValorIPI deve ser informado.");
        if (!valores.ExigibilidadeSuspensa.HasValue)
            throw new OpenException("Layout 2 de São Paulo: ExigibilidadeSuspensa deve ser informada.");
        if (!PossuiSomenteDigitos(nota.Servico.CodigoNbs, 9))
            throw new OpenException("Layout 2 de São Paulo: NBS deve conter exatamente 9 dígitos.");

        var codigoLocalPrestacao = nota.Servico.MunicipioIncidencia > 0
            ? nota.Servico.MunicipioIncidencia
            : nota.Servico.CodigoMunicipio;
        if (!PossuiSomenteDigitos(codigoLocalPrestacao.ToString(CultureInfo.InvariantCulture), 7))
            throw new OpenException("Layout 2 de São Paulo: cLocPrestacao deve conter um código IBGE com 7 dígitos.");

        var info = valores.IBSCBS;
        if (info == null)
            throw new OpenException("Layout 2 de São Paulo: o grupo IBSCBS deve ser informado.");
        if (info.FinalidadeNFSe != "0")
            throw new OpenException("Layout 2 de São Paulo: finNFSe deve ser informado com valor 0 para emissão regular.");
        if (info.IndicadorFinal is not ("0" or "1"))
            throw new OpenException("Layout 2 de São Paulo: indFinal deve ser 0 ou 1.");
        if (!PossuiSomenteDigitos(ObterCodigoOperacao(nota), 6))
            throw new OpenException("Layout 2 de São Paulo: cIndOp deve conter exatamente 6 dígitos.");
        if (info.IndicadorDestinatario is not ("0" or "1"))
            throw new OpenException("Layout 2 de São Paulo: indDest deve ser 0 ou 1.");
        if (!PossuiSomenteDigitos(ObterClassificacaoTributaria(nota), 6))
            throw new OpenException("Layout 2 de São Paulo: cClassTrib deve conter exatamente 6 dígitos.");

        ObterDocumentoTomador(nota, out _, out _, out _);
    }

    private static void ObterDocumentoTomador(NotaServico nota, out string indicador, out string documento,
        out string nifNaoNif)
    {
        indicador = "3";
        documento = new string('0', 14);
        nifNaoNif = string.Empty;

        if (!string.IsNullOrWhiteSpace(nota.Tomador.CpfCnpj))
        {
            var cpfCnpj = SomenteDigitos(nota.Tomador.CpfCnpj, "CPF/CNPJ do tomador");
            indicador = cpfCnpj.Length switch
            {
                11 => "1",
                14 => "2",
                _ => throw new OpenException("Layout 2 de São Paulo: CPF/CNPJ do tomador deve conter 11 ou 14 dígitos.")
            };
            documento = cpfCnpj.PadLeft(14, '0');
            return;
        }

        if (!string.IsNullOrWhiteSpace(nota.Tomador.DocEstrangeiro))
        {
            var nif = nota.Tomador.DocEstrangeiro.Trim();
            if (nif.Length > 40 || nif.Any(c => c > 127))
                throw new OpenException("Layout 2 de São Paulo: NIF deve conter no máximo 40 caracteres ASCII.");
            indicador = "4";
            nifNaoNif = nif;
            return;
        }

        if (!string.IsNullOrWhiteSpace(nota.Tomador.CodNaoNif))
        {
            if (nota.Tomador.CodNaoNif is not ("0" or "1" or "2"))
                throw new OpenException("Layout 2 de São Paulo: NaoNIF deve ser 0, 1 ou 2.");
            indicador = "4";
            nifNaoNif = nota.Tomador.CodNaoNif;
        }
    }

    private static string ObterCodigoOperacao(NotaServico nota)
    {
        var codigo = nota.Servico.Valores.IBSCBS?.CodigoIndicadorOperacao;
        return string.IsNullOrWhiteSpace(codigo) ? nota.Servico.CodigoIndicadorOperacao : codigo;
    }

    private static string ObterClassificacaoTributaria(NotaServico nota)
    {
        var codigo = nota.Servico.Valores.IBSCBS?.Valores?.Tributos?.SituacaoClassificacao
            ?.CodigoClassificacaoTributaria;
        return string.IsNullOrWhiteSpace(codigo) ? nota.Servico.CodigoClassificacaoTributaria : codigo;
    }

    private static string ObterTipoRps(TipoRps tipo)
    {
        return tipo switch
        {
            TipoRps.RPS => "RPS",
            TipoRps.NFConjugada => "RPS-M",
            TipoRps.Cupom => "RPS-C",
            _ => throw new OpenException("Layout 2 de São Paulo: TipoRPS inválido.")
        };
    }

    private static string ObterTipoTributacao(TipoTributacao tipo)
    {
        return tipo switch
        {
            TipoTributacao.Tributavel => "T",
            TipoTributacao.ForaMun => "F",
            TipoTributacao.Isenta => "A",
            TipoTributacao.ForaMunIsento => "B",
            TipoTributacao.Imune => "M",
            TipoTributacao.ForaMunImune => "N",
            TipoTributacao.Suspensa => "X",
            TipoTributacao.ForaMunSuspensa => "V",
            TipoTributacao.ExpServicos => "P",
            _ => throw new OpenException("Layout 2 de São Paulo: TributacaoRPS inválida.")
        };
    }

    private static XElement ElementoValor(string nome, decimal valor)
    {
        return new XElement(nome, valor.ToString("0.00", CultureInfo.InvariantCulture));
    }

    private static string FormatarValorAssinatura(decimal valor)
    {
        if (valor < 0)
            throw new OpenException("Layout 2 de São Paulo: valores da assinatura não podem ser negativos.");

        var valorSemSeparador = valor.ToString("0.00", CultureInfo.InvariantCulture).Replace(".", string.Empty);
        if (valorSemSeparador.Length > 15)
            throw new OpenException("Layout 2 de São Paulo: valor excede as 15 posições da assinatura.");
        return valorSemSeparador.PadLeft(15, '0');
    }

    private static string PreencherNumero(string valor, int tamanho, string campo)
    {
        var digitos = SomenteDigitos(valor, campo);
        if (digitos.Length == 0 || digitos.Length > tamanho)
            throw new OpenException($"Layout 2 de São Paulo: {campo} deve conter de 1 a {tamanho} dígitos.");
        return digitos.PadLeft(tamanho, '0');
    }

    private static string PreencherSerie(string serie)
    {
        serie ??= string.Empty;
        if (serie.Length > 5 || serie.Any(c => c > 127) || (serie.Length > 0 && char.IsWhiteSpace(serie[0])))
            throw new OpenException("Layout 2 de São Paulo: Série do RPS deve conter até 5 caracteres ASCII, sem espaços à esquerda.");
        return serie.PadRight(5, ' ');
    }

    private static string SomenteDigitos(string valor, string campo)
    {
        valor ??= string.Empty;
        if (valor.Any(c => !char.IsDigit(c)))
            throw new OpenException($"Layout 2 de São Paulo: {campo} deve conter somente dígitos.");
        return valor;
    }

    private static bool PossuiSomenteDigitos(string valor, int tamanho)
    {
        return valor?.Length == tamanho && valor.All(c => c >= '0' && c <= '9');
    }
}
