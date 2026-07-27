using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.Providers;
using Xunit;

namespace OpenAC.Net.NFSe.Test;

public class TestProviderISSSaoPaulo
{
    private const string EndpointLayouts1E2 =
        "https://nfews.prefeitura.sp.gov.br/lotenfe.asmx?WSDL";

    private const string CadeiaOficial =
        "123456789012RTNT 00000000000120260101TNN00000000205000000000000050000002658100013167474254209999999000106S";

    private const string AssinaturaEsperada =
        "dZCSPerLU4J6T3VDd8iJ3v2wiVjdSsyptQ+Zu2bn0XAazU+X6lSSEfKFo41WXMX9kFkAkKIQKRgbSmoLXnkX/OdHCL9RnrTDLmeTbdB3iYIxhgmCkyuqsM9mSIv2TH2HPBXSuLAJUoMJUdBxOeFjuSgF3b8FKvkeGeOkNo+luFvnFHYN3On/TT528vzDSxeYcYF6NNHb1aDAVaIm9RpMo+7QFhbGMbAjMAJy4o5LUtKS1+bJaLJPvuq53jZOHWFw66Ah7U5t2i7VYDCrr6tTYmiH6bxFcG8zK97Up3Wlbd6ssh8rPdiou14HlnNZCsTuvmploRJNhEzTMe7Qed9Rbw==";

    [Fact]
    public void LayoutPadraoContinuaSendoLayout1()
    {
        Assert.Equal(LayoutISSSaoPaulo.Layout1, new ConfigNFSe().WebServices.LayoutISSSaoPaulo);
    }

    [Fact]
    public void SaoPauloUsaEndpointQueAceitaLayouts1E2()
    {
        var municipio = Assert.Single(ProviderManager.Municipios, x => x.Codigo == 3550308);

        Assert.Equal(EndpointLayouts1E2, municipio.UrlHomologacao[TipoUrl.Enviar]);
        Assert.Equal(EndpointLayouts1E2, municipio.UrlProducao[TipoUrl.Enviar]);
        Assert.Equal(EndpointLayouts1E2, municipio.UrlProducao[TipoUrl.CancelarNFSe]);
        Assert.Equal(EndpointLayouts1E2, municipio.UrlProducao[TipoUrl.ConsultarNFSe]);
        Assert.Equal(EndpointLayouts1E2, municipio.UrlProducao[TipoUrl.ConsultarNFSeRps]);
        Assert.Equal(EndpointLayouts1E2, municipio.UrlProducao[TipoUrl.ConsultarLoteRps]);
        Assert.Equal(EndpointLayouts1E2, municipio.UrlProducao[TipoUrl.ConsultarSituacao]);
    }

    [Fact]
    public void Layout1ExplicitoProduzOMesmoXmlDoPadrao()
    {
        using var certificado = CriarCertificado();
        var pfx = certificado.Export(X509ContentType.Pfx, "teste");
        var configuracaoPadrao = CriarConfiguracao(pfx);
        var configuracaoExplicita = CriarConfiguracao(pfx);
        configuracaoExplicita.WebServices.LayoutISSSaoPaulo = LayoutISSSaoPaulo.Layout1;

        var xmlPadrao = CriarNotaLayout1(configuracaoPadrao).GetXml();
        var xmlExplicito = CriarNotaLayout1(configuracaoExplicita).GetXml();

        Assert.Equal(xmlPadrao, xmlExplicito);
        Assert.Contains("<ValorServicos>100.00</ValorServicos>", xmlPadrao);
        Assert.DoesNotContain("<IBSCBS>", xmlPadrao);
    }

    [Fact]
    public void SeletorDeLayout2AcionaOSerializerNovo()
    {
        using var certificado = CriarCertificado();
        var config = CriarConfiguracao(certificado.Export(X509ContentType.Pfx, "teste"));
        config.WebServices.LayoutISSSaoPaulo = LayoutISSSaoPaulo.Layout2;

        var xml = CriarNotaExemploOficial(config).GetXml();

        Assert.Contains("<ValorFinalCobrado>20500.00</ValorFinalCobrado>", xml);
        Assert.Contains("<IBSCBS>", xml);
        Assert.DoesNotContain("<ValorServicos>", xml);
    }

    [Fact]
    public void CadeiaOficialTemConteudoPreenchimentosETamanhoEsperados()
    {
        var cadeia = ISSSaoPauloLayout2.MontarCadeiaAssinatura(CriarNotaExemploOficial());

        Assert.Equal(CadeiaOficial, cadeia);
        Assert.Equal(106, cadeia.Length);
        Assert.Equal("123456789012", cadeia[..12]);
        Assert.Equal("RTNT ", cadeia.Substring(12, 5));
        Assert.Equal("000000000001", cadeia.Substring(17, 12));
        Assert.Equal("20260101", cadeia.Substring(29, 8));
        Assert.Equal("000000002050000", cadeia.Substring(40, 15));
        Assert.Equal("000000000500000", cadeia.Substring(55, 15));
        Assert.Equal("02658", cadeia.Substring(70, 5));
    }

    [Fact]
    public void Sha1DaCadeiaOficialConfere()
    {
        var hash = SHA1.HashData(Encoding.ASCII.GetBytes(CadeiaOficial));

        Assert.Equal("D30FFC47832BB2CD5397ACBA4393AE00565608CD", Convert.ToHexString(hash));
    }

    [Fact]
    public void AssinaturaRsaSha1DoExemploEhDeterministica()
    {
        using var rsa = CriarRsa();

        var assinatura = ISSSaoPauloLayout2.AssinarRps(CriarNotaExemploOficial(), rsa);

        Assert.Equal(AssinaturaEsperada, assinatura);
        Assert.True(rsa.VerifyData(Encoding.ASCII.GetBytes(CadeiaOficial),
            Convert.FromBase64String(assinatura), HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1));
    }

    public static IEnumerable<object[]> RetencoesPisCofins()
    {
        yield return new object[] { false, false, false, 0 };
        yield return new object[] { true, true, true, 3 };
        yield return new object[] { true, true, false, 4 };
        yield return new object[] { true, false, false, 5 };
        yield return new object[] { false, true, false, 6 };
        yield return new object[] { false, true, true, 7 };
        yield return new object[] { false, false, true, 8 };
        yield return new object[] { true, false, true, 9 };
    }

    [Theory]
    [MemberData(nameof(RetencoesPisCofins))]
    public void RetencaoPisCofinsSegueAsOitoCombinacoesDoLayout2(bool pis, bool cofins, bool csll,
        int esperado)
    {
        var valores = new NotaServico(new ConfigNFSe()).Servico.Valores;
        valores.ValorPis = pis ? 1 : 0;
        valores.ValorCofins = cofins ? 1 : 0;
        valores.ValorCsll = csll ? 1 : 0;

        Assert.Equal(esperado, ISSSaoPauloLayout2.ObterRetencaoPisCofins(valores));
    }

    [Fact]
    public void XmlLayout2SegueOrdemEscolhasEGrupoMinimo()
    {
        using var certificado = CriarCertificado();
        var nota = CriarNotaExemploOficial();

        var xml = XDocument.Parse(ISSSaoPauloLayout2.WriteXmlRps(nota, certificado));
        var rps = xml.Root!;
        var nomes = rps.Elements().Select(x => x.Name.LocalName).ToArray();

        Assert.True(Array.IndexOf(nomes, "ValorFinalCobrado") < Array.IndexOf(nomes, "ValorIPI"));
        Assert.True(Array.IndexOf(nomes, "ValorIPI") < Array.IndexOf(nomes, "ExigibilidadeSuspensa"));
        Assert.True(Array.IndexOf(nomes, "ExigibilidadeSuspensa") < Array.IndexOf(nomes, "NBS"));
        Assert.True(Array.IndexOf(nomes, "NBS") < Array.IndexOf(nomes, "cLocPrestacao"));
        Assert.True(Array.IndexOf(nomes, "cLocPrestacao") < Array.IndexOf(nomes, "IBSCBS"));
        Assert.DoesNotContain("ValorInicialCobrado", nomes);
        Assert.DoesNotContain("ValorServicos", nomes);
        Assert.Equal("20500.00", rps.Element("ValorFinalCobrado")?.Value);
        Assert.Equal("118054000", rps.Element("NBS")?.Value);
        Assert.Equal("3550308", rps.Element("cLocPrestacao")?.Value);

        var ibsCbs = rps.Element("IBSCBS")!;
        Assert.Equal(new[] { "finNFSe", "indFinal", "cIndOp", "indDest", "valores" },
            ibsCbs.Elements().Select(x => x.Name.LocalName));
        Assert.Equal("200001", ibsCbs.Element("valores")?.Element("trib")?.Element("gIBSCBS")
            ?.Element("cClassTrib")?.Value);
    }

    [Fact]
    public void ExatamenteUmValorCobradoEhObrigatorio()
    {
        var nota = CriarNotaExemploOficial();
        nota.Servico.Valores.ValorInicialCobrado = 100;

        var erro = Assert.ThrowsAny<Exception>(() => ISSSaoPauloLayout2.MontarCadeiaAssinatura(nota));

        Assert.Contains("exatamente um", erro.Message);
    }

    [Fact]
    public void LoteLayout2RecusaMaisDeCinquentaRpsAntesDaTransmissao()
    {
        var config = new ConfigNFSe();
        config.WebServices.CodigoMunicipio = 3550308;
        config.WebServices.LayoutISSSaoPaulo = LayoutISSSaoPaulo.Layout2;
        var openNFSe = new OpenNFSe(config);
        for (var i = 0; i < 51; i++)
            openNFSe.NotasServico.AddNew();

        var erro = Assert.ThrowsAny<Exception>(() => openNFSe.Enviar(1));

        Assert.Contains("máximo de 50 RPS", erro.Message);
    }

    [Fact]
    public void XmlLayout2ValidaNoXsdDistribuido()
    {
        using var certificado = CriarCertificado();
        var rps = XElement.Parse(ISSSaoPauloLayout2.WriteXmlRps(CriarNotaExemploOficial(),
            certificado, false, false));
        XNamespace ns = "http://www.prefeitura.sp.gov.br/nfe";
        var raiz = new XElement(ns + "PedidoEnvioRPS",
            new XElement("Cabecalho",
                new XAttribute("Versao", 2),
                new XElement("CPFCNPJRemetente",
                    new XElement("CNPJ", "12345678000190"))),
            rps);
        var xml = new XDocument(new XDeclaration("1.0", "UTF-8", null), raiz)
            .ToString(SaveOptions.DisableFormatting);
        xml = XmlSigning.AssinarXml(xml, "PedidoEnvioRPS", "", certificado);

        ValidarXml(xml, ObterSchema("PedidoEnvioRPS_v02.xsd"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RetornosV01EV02PreservamIdentificadoresEOriginal(bool layout2)
    {
        var config = new ConfigNFSe();
        config.WebServices.CodigoMunicipio = 3550308;
        config.WebServices.LayoutISSSaoPaulo = layout2
            ? LayoutISSSaoPaulo.Layout2
            : LayoutISSSaoPaulo.Layout1;
        var openNFSe = new OpenNFSe(config);
        var complemento = layout2
            ? "<RetornoComplementarIBSCBS><ValorBaseCalculo>100.00</ValorBaseCalculo></RetornoComplementarIBSCBS>"
            : string.Empty;
        var camposV2 = layout2
            ? "<ChaveNotaNacional>35503081234567890000000000000000000000000000000000</ChaveNotaNacional>"
            : string.Empty;
        var dataFato = layout2
            ? "<DataFatoGeradorNFe>2026-08-01T10:30:00</DataFatoGeradorNFe>"
            : string.Empty;
        var xml = "<NFe><ChaveNFe><InscricaoPrestador>123456789012</InscricaoPrestador>" +
                  "<NumeroNFe>987</NumeroNFe><CodigoVerificacao>ABC123</CodigoVerificacao>" +
                  camposV2 + "</ChaveNFe><DataEmissaoNFe>2026-08-01T10:31:00</DataEmissaoNFe>" +
                  dataFato + "<NumeroLote>42</NumeroLote>" + complemento + "</NFe>";

        var nota = openNFSe.NotasServico.Load(xml);

        Assert.Equal("987", nota.IdentificacaoNFSe.Numero);
        Assert.Equal("ABC123", nota.IdentificacaoNFSe.Chave);
        Assert.Equal(42, nota.NumeroLote);
        Assert.Equal(xml, nota.XmlOriginal);
        if (layout2)
        {
            Assert.Equal(new DateTime(2026, 8, 1, 10, 30, 0), nota.IdentificacaoNFSe.DataFatoGerador);
            Assert.StartsWith("3550308", nota.IdentificacaoNFSe.ChaveNotaNacional);
            Assert.Equal(complemento, nota.XmlRetornoComplementarIBSCBS);
        }
        else
        {
            Assert.Null(nota.IdentificacaoNFSe.DataFatoGerador);
            Assert.True(string.IsNullOrEmpty(nota.IdentificacaoNFSe.ChaveNotaNacional));
            Assert.True(string.IsNullOrEmpty(nota.XmlRetornoComplementarIBSCBS));
        }
    }

    private static ConfigNFSe CriarConfiguracao(byte[] pfx)
    {
        var config = new ConfigNFSe();
        config.Geral.Salvar = false;
        config.Arquivos.Salvar = false;
        config.WebServices.CodigoMunicipio = 3550308;
        config.Certificados.CertificadoBytes = pfx;
        config.Certificados.Senha = "teste";
        config.PrestadorPadrao.CpfCnpj = "12345678000190";
        config.PrestadorPadrao.InscricaoMunicipal = "12345678";
        return config;
    }

    private static NotaServico CriarNotaLayout1(ConfigNFSe config)
    {
        var nota = new OpenNFSe(config).NotasServico.AddNew();
        nota.IdentificacaoRps.Numero = "1";
        nota.IdentificacaoRps.Serie = "A";
        nota.IdentificacaoRps.Tipo = TipoRps.RPS;
        nota.IdentificacaoRps.DataEmissao = new DateTime(2026, 7, 31);
        nota.Situacao = SituacaoNFSeRps.Normal;
        nota.TipoTributacao = TipoTributacao.Tributavel;
        nota.Servico.Valores.ValorServicos = 100;
        nota.Servico.Valores.Aliquota = 2;
        nota.Servico.ItemListaServico = "2658";
        nota.Servico.Discriminacao = "Servico de teste";
        nota.Tomador.CpfCnpj = "13167474254";
        return nota;
    }

    private static NotaServico CriarNotaExemploOficial(ConfigNFSe? config = null)
    {
        var nota = new NotaServico(config ?? new ConfigNFSe());
        nota.Prestador.InscricaoMunicipal = "123456789012";
        nota.IdentificacaoRps.Serie = "RTNT";
        nota.IdentificacaoRps.Numero = "1";
        nota.IdentificacaoRps.Tipo = TipoRps.RPS;
        nota.IdentificacaoRps.DataEmissao = new DateTime(2026, 1, 1);
        nota.Situacao = SituacaoNFSeRps.Normal;
        nota.TipoTributacao = TipoTributacao.Tributavel;
        nota.Servico.Valores.IssRetido = SituacaoTributaria.Normal;
        nota.Servico.Valores.ValorFinalCobrado = 20500;
        nota.Servico.Valores.ValorDeducoes = 5000;
        nota.Servico.Valores.ValorIpi = 0;
        nota.Servico.Valores.ExigibilidadeSuspensa = false;
        nota.Servico.Valores.Aliquota = 2;
        nota.Servico.ItemListaServico = "2658";
        nota.Servico.CodigoNbs = "118054000";
        nota.Servico.Discriminacao = "Servico de teste sem dados reais";
        nota.Servico.CodigoMunicipio = 3550308;
        nota.Servico.MunicipioIncidencia = 3550308;
        nota.Tomador.CpfCnpj = "13167474254";
        nota.Intermediario.CpfCnpj = "09999999000106";
        nota.Intermediario.IssRetido = SituacaoTributaria.Retencao;
        nota.Servico.Valores.IBSCBS = new InfoIBSCBS
        {
            FinalidadeNFSe = "0",
            IndicadorFinal = "1",
            CodigoIndicadorOperacao = "123456",
            IndicadorDestinatario = "1"
        };
        nota.Servico.Valores.IBSCBS.Valores.Tributos.SituacaoClassificacao
            .CodigoClassificacaoTributaria = "200001";
        return nota;
    }

    private static X509Certificate2 CriarCertificado()
    {
        using var rsa = CriarRsa();
        var request = new CertificateRequest("CN=OpenAC NFSe Teste", rsa, HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        return request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));
    }

    private static RSA CriarRsa()
    {
        var rsa = RSA.Create();
        rsa.ImportParameters(new RSAParameters
        {
            Modulus = Convert.FromBase64String("6FgMyOlnY2mN8/3SL12k7gmDO7JM7PhFy8mCUVg5kN5IOIra6gEKMSoUIxWDg7Cn5fClD5iF82CUTfadRj8433i3snFJ0SRjCD2nrLVITdyDMzbitwdG2Dqbp7604C+evobXTrn9d2e37EoRiTOqPuRNC/PhfcDzt5EbM3hxd1wWS0UJAnBcCK1qxnk213WgR0dHgXfZsOYtRuGsgWojeR0NWGqDqIM357DvGhQm5S69ob38Wj6n4Eem6fX/l1m3R3d0MyIDWWTBi7kzFJOxfaahYJO3OLTk0g1dtvGKCV6DoFGXiU4xywjEUfhUs5Dw07dK6F23tueleOoHdh8Aew=="),
            Exponent = Convert.FromBase64String("AQAB"),
            D = Convert.FromBase64String("DNZYln025io92p5Kj61n4HMMGi9Gys0I5jKTDbWHMLbnXKBnagh2rLK7fBjDNHJ9RFogdJUjyYerigc3N1tk5AwCckyKHJEbG6h0bDlz7kFhymGc8ynmwymx0fnaeoyHA9XlbYcfNwq3Acox39fH70Oj8iYebllL3feZfiWId2S3KoAhhtiUKu69+Eo8/U4RYZLLkXuUcOqktEPe01QeVluwdLO09r71THV/A1FOOMT3aW3S+1J6M0QgIK5X1l+GxAYwBXmkSP4znpvBaEeMDghLOoAXA0CN1MWt7Ve0TN/AD447LwtjGskxEYBNYslpGGV3VYoSCkvUdhcDU8UAEQ=="),
            P = Convert.FromBase64String("/TBS++rjM6aAb36VrpbX4ey/qxIJX7S6ApMw7QbcMTv22fYK8VUcjH4zSYx//mY8Mwu2y4dGmld8d//Zdk+AOC36hBJ8fd1PGvmVPudUXAHuvjPBWGJDhvrJUUE0psn+LNl1JeH6B/fM6azwX+NiM69Vii/vHLsZrvd9EYOuTGk="),
            Q = Convert.FromBase64String("6ux5uFNs/SaDK5DVzAOZYeez28L8qC/Y6RmY6oUpyjCQHtqezML4F/nNJjzjldyKCS3xYvBq+NZ38WuZJDOaorFaJqj6PERE1Yc58KOrwxiS9tgIELEAzu+M0aMU7s0fp347n3Jn4LXfdg/mvGOsqVwxsl5DOeNsNCkW5BNc2UM="),
            DP = Convert.FromBase64String("YPb/4QDdEKvkpk6ZbqrQdPLhmNeohWHGlzPd2fj1nVl0uZbULAbHjzrJ05IedsSaq4YB9MKTFIsK3T47/2aFGX7qYWhfCykVoaQSN2wKz83hrDBQDNRdPjWPojHRw0q6sFx71A1OX3zUmm2kBWUk99xfazPeZGd3d53K5UlEGHk="),
            DQ = Convert.FromBase64String("HNsjMGL+9jFu10EZIdAnXQFK9GmFA1utNySvxc7JjU5dxYxxCRHBy6AhdNrx0YyfX/VGuzJw0VP2s67Vxr6X9ff27NzAr/pqwhe0JDzWckZodu2eP/6d7M077NwtTA/iHX7B8BnrbIyqgCP/4ZAUu1DZweEWPNwUhGuvpiBCvWU="),
            InverseQ = Convert.FromBase64String("zn2buxoHQvWUUnJgrUTuVHn3Eznay/HNWs52yf4MeEhUGmxjHx159wcoNiOQc7FJePFT98KoSJ99HOKhKwhqu+sDk3E66LGwIItZOYZyl5/kJNY651/w8qhAwSg57FUfGK8BsR2qkiFe6607LDH/GTYAfPI2Ulu5IIMmmPIo+os=")
        });
        return rsa;
    }

    private static string ObterSchema(string nome)
    {
        return Path.Combine(AppContext.BaseDirectory, "Schemas", "ISSSaoPaulo", "1.00", nome);
    }

    private static void ValidarXml(string xml, string schema)
    {
        var erros = new List<string>();
        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema
        };
        settings.Schemas.XmlResolver = new XmlUrlResolver();
        settings.Schemas.Add("http://www.prefeitura.sp.gov.br/nfe", schema);
        settings.ValidationEventHandler += (_, args) => erros.Add(args.Message);
        using var reader = XmlReader.Create(new StringReader(xml), settings);
        while (reader.Read())
        {
        }

        Assert.True(erros.Count == 0, string.Join(Environment.NewLine, erros));
    }
}
