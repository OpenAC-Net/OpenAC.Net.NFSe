using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Test;

public class TestProviderISSSaoPaulo
{
    private const string EndpointLayouts1E2 =
        "https://nfews.prefeitura.sp.gov.br/lotenfe.asmx?WSDL";

    private const string CadeiaOficial =
        "123456789012RTNT 00000000000120260101TNN00000000205000000000000050000002658100013167474254209999999000106S";

    private const string AssinaturaEsperada =
        "dZCSPerLU4J6T3VDd8iJ3v2wiVjdSsyptQ+Zu2bn0XAazU+X6lSSEfKFo41WXMX9kFkAkKIQKRgbSmoLXnkX/OdHCL9RnrTDLmeTbdB3iYIxhgmCkyuqsM9mSIv2TH2HPBXSuLAJUoMJUdBxOeFjuSgF3b8FKvkeGeOkNo+luFvnFHYN3On/TT528vzDSxeYcYF6NNHb1aDAVaIm9RpMo+7QFhbGMbAjMAJy4o5LUtKS1+bJaLJPvuq53jZOHWFw66Ah7U5t2i7VYDCrr6tTYmiH6bxFcG8zK97Up3Wlbd6ssh8rPdiou14HlnNZCsTuvmploRJNhEzTMe7Qed9Rbw==";

    [Test]
    public async Task LayoutPadraoContinuaSendoLayout1()
    {
        await Assert.That(new ConfigNFSe().WebServices.LayoutISSSaoPaulo).IsEqualTo(LayoutISSSaoPaulo.Layout1);
    }

    [Test]
    public async Task SaoPauloUsaEndpointQueAceitaLayouts1E2()
    {
        var municipio = await Assert.That(ProviderManager.Municipios).HasSingleItem(x => x.Codigo == 3550308);

        await Assert.That(municipio.UrlHomologacao[TipoUrl.Enviar]).IsEqualTo(EndpointLayouts1E2);
        await Assert.That(municipio.UrlProducao[TipoUrl.Enviar]).IsEqualTo(EndpointLayouts1E2);
        await Assert.That(municipio.UrlProducao[TipoUrl.CancelarNFSe]).IsEqualTo(EndpointLayouts1E2);
        await Assert.That(municipio.UrlProducao[TipoUrl.ConsultarNFSe]).IsEqualTo(EndpointLayouts1E2);
        await Assert.That(municipio.UrlProducao[TipoUrl.ConsultarNFSeRps]).IsEqualTo(EndpointLayouts1E2);
        await Assert.That(municipio.UrlProducao[TipoUrl.ConsultarLoteRps]).IsEqualTo(EndpointLayouts1E2);
        await Assert.That(municipio.UrlProducao[TipoUrl.ConsultarSituacao]).IsEqualTo(EndpointLayouts1E2);
    }

    [Test]
    public async Task Layout1ExplicitoProduzOMesmoXmlDoPadrao()
    {
        using var certificado = CriarCertificado();
        var pfx = certificado.Export(X509ContentType.Pfx, "teste");
        var configuracaoPadrao = CriarConfiguracao(pfx);
        var configuracaoExplicita = CriarConfiguracao(pfx);
        configuracaoExplicita.WebServices.LayoutISSSaoPaulo = LayoutISSSaoPaulo.Layout1;

        var xmlPadrao = CriarNotaLayout1(configuracaoPadrao).GetXml();
        var xmlExplicito = CriarNotaLayout1(configuracaoExplicita).GetXml();

        await Assert.That(xmlExplicito).IsEqualTo(xmlPadrao);
        await Assert.That(xmlPadrao).Contains("<ValorServicos>100.00</ValorServicos>");
        await Assert.That(xmlPadrao).DoesNotContain("<IBSCBS>");
    }

    [Test]
    public async Task SeletorDeLayout2AcionaOSerializerNovo()
    {
        using var certificado = CriarCertificado();
        var config = CriarConfiguracao(certificado.Export(X509ContentType.Pfx, "teste"));
        config.WebServices.LayoutISSSaoPaulo = LayoutISSSaoPaulo.Layout2;

        var xml = CriarNotaExemploOficial(config).GetXml();

        await Assert.That(xml).Contains("<ValorFinalCobrado>20500.00</ValorFinalCobrado>");
        await Assert.That(xml).Contains("<IBSCBS>");
        await Assert.That(xml).DoesNotContain("<ValorServicos>");
    }

    [Test]
    public async Task CadeiaOficialTemConteudoPreenchimentosETamanhoEsperados()
    {
        var cadeia = ISSSaoPauloLayout2.MontarCadeiaAssinatura(CriarNotaExemploOficial());

        await Assert.That(cadeia).IsEqualTo(CadeiaOficial);
        await Assert.That(cadeia.Length).IsEqualTo(106);
        await Assert.That(cadeia[..12]).IsEqualTo("123456789012");
        await Assert.That(cadeia.Substring(12, 5)).IsEqualTo("RTNT ");
        await Assert.That(cadeia.Substring(17, 12)).IsEqualTo("000000000001");
        await Assert.That(cadeia.Substring(29, 8)).IsEqualTo("20260101");
        await Assert.That(cadeia.Substring(40, 15)).IsEqualTo("000000002050000");
        await Assert.That(cadeia.Substring(55, 15)).IsEqualTo("000000000500000");
        await Assert.That(cadeia.Substring(70, 5)).IsEqualTo("02658");
    }

    [Test]
    public async Task Sha1DaCadeiaOficialConfere()
    {
        var hash = SHA1.HashData(Encoding.ASCII.GetBytes(CadeiaOficial));

        await Assert.That(Convert.ToHexString(hash)).IsEqualTo("D30FFC47832BB2CD5397ACBA4393AE00565608CD");
    }

    [Test]
    public async Task AssinaturaRsaSha1DoExemploEhDeterministica()
    {
        using var rsa = CriarRsa();

        var assinatura = ISSSaoPauloLayout2.AssinarRps(CriarNotaExemploOficial(), rsa);

        await Assert.That(assinatura).IsEqualTo(AssinaturaEsperada);
        await Assert.That(rsa.VerifyData(Encoding.ASCII.GetBytes(CadeiaOficial),
            Convert.FromBase64String(assinatura), HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1)).IsTrue();
    }

    public static IEnumerable<(bool Pis, bool Cofins, bool Csll, int Esperado)> RetencoesPisCofins()
    {
        yield return (false, false, false, 0);
        yield return (true, true, true, 3);
        yield return (true, true, false, 4);
        yield return (true, false, false, 5);
        yield return (false, true, false, 6);
        yield return (false, true, true, 7);
        yield return (false, false, true, 8);
        yield return (true, false, true, 9);
    }

    [Test]
    [MethodDataSource(nameof(RetencoesPisCofins))]
    public async Task RetencaoPisCofinsSegueAsOitoCombinacoesDoLayout2(bool pis, bool cofins, bool csll,
        int esperado)
    {
        var valores = new NotaServico(new ConfigNFSe()).Servico.Valores;
        valores.ValorPis = pis ? 1 : 0;
        valores.ValorCofins = cofins ? 1 : 0;
        valores.ValorCsll = csll ? 1 : 0;

        await Assert.That(ISSSaoPauloLayout2.ObterRetencaoPisCofins(valores)).IsEqualTo(esperado);
    }

    [Test]
    public async Task XmlLayout2SegueOrdemEscolhasEGrupoMinimo()
    {
        using var certificado = CriarCertificado();
        var nota = CriarNotaExemploOficial();

        var xml = XDocument.Parse(ISSSaoPauloLayout2.WriteXmlRps(nota, certificado));
        var rps = xml.Root!;
        var nomes = rps.Elements().Select(x => x.Name.LocalName).ToArray();

        await Assert.That(Array.IndexOf(nomes, "ValorFinalCobrado") < Array.IndexOf(nomes, "ValorIPI")).IsTrue();
        await Assert.That(Array.IndexOf(nomes, "ValorIPI") < Array.IndexOf(nomes, "ExigibilidadeSuspensa")).IsTrue();
        await Assert.That(Array.IndexOf(nomes, "ExigibilidadeSuspensa") < Array.IndexOf(nomes, "NBS")).IsTrue();
        await Assert.That(Array.IndexOf(nomes, "NBS") < Array.IndexOf(nomes, "cLocPrestacao")).IsTrue();
        await Assert.That(Array.IndexOf(nomes, "cLocPrestacao") < Array.IndexOf(nomes, "IBSCBS")).IsTrue();
        await Assert.That(nomes).DoesNotContain("ValorInicialCobrado");
        await Assert.That(nomes).DoesNotContain("ValorServicos");
        await Assert.That(rps.Element("ValorFinalCobrado")?.Value).IsEqualTo("20500.00");
        await Assert.That(rps.Element("NBS")?.Value).IsEqualTo("118054000");
        await Assert.That(rps.Element("cLocPrestacao")?.Value).IsEqualTo("3550308");

        var ibsCbs = rps.Element("IBSCBS")!;
        await Assert.That(ibsCbs.Elements().Select(x => x.Name.LocalName)).IsEquivalentTo(new[] { "finNFSe", "indFinal", "cIndOp", "indDest", "valores" }, TUnit.Assertions.Enums.CollectionOrdering.Matching);
        await Assert.That(ibsCbs.Element("valores")?.Element("trib")?.Element("gIBSCBS")
            ?.Element("cClassTrib")?.Value).IsEqualTo("200001");
    }

    [Test]
    public async Task ExatamenteUmValorCobradoEhObrigatorio()
    {
        var nota = CriarNotaExemploOficial();
        nota.Servico.Valores.ValorInicialCobrado = 100;

        var erro = await Assert.That(() => ISSSaoPauloLayout2.MontarCadeiaAssinatura(nota)).Throws<Exception>();

        await Assert.That(erro!.Message).Contains("exatamente um");
    }

    [Test]
    public async Task LoteLayout2RecusaMaisDeCinquentaRpsAntesDaTransmissao()
    {
        var config = new ConfigNFSe();
        config.WebServices.CodigoMunicipio = 3550308;
        config.WebServices.LayoutISSSaoPaulo = LayoutISSSaoPaulo.Layout2;
        var openNFSe = new OpenNFSe(config);
        for (var i = 0; i < 51; i++)
            openNFSe.NotasServico.AddNew();

        var erro = await Assert.That(() => openNFSe.Enviar(1)).Throws<Exception>();

        await Assert.That(erro!.Message).Contains("máximo de 50 RPS");
    }

    [Test]
    [Arguments(null)]
    [Arguments("1")]
    [Arguments("2")]
    [Arguments("3")]
    [Arguments("4")]
    [Arguments("5")]
    public async Task XmlLayout2ValidaNoXsdDistribuido(string? tipoOperacao)
    {
        using var certificado = CriarCertificado();
        var nota = CriarNotaExemploOficial();
        nota.Servico.Valores.IBSCBS!.TipoOperacao = tipoOperacao;
        var rps = XElement.Parse(ISSSaoPauloLayout2.WriteXmlRps(nota,
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

        await ValidarXml(xml, ObterSchema("PedidoEnvioRPS_v02.xsd"));
    }

    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task RetornosV01EV02PreservamIdentificadoresEOriginal(bool layout2)
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

        await Assert.That(nota.IdentificacaoNFSe.Numero).IsEqualTo("987");
        await Assert.That(nota.IdentificacaoNFSe.Chave).IsEqualTo("ABC123");
        await Assert.That(nota.NumeroLote).IsEqualTo(42);
        await Assert.That(nota.XmlOriginal).IsEqualTo(xml);
        if (layout2)
        {
            await Assert.That(nota.IdentificacaoNFSe.DataFatoGerador).IsEqualTo(new DateTime(2026, 8, 1, 10, 30, 0));
            await Assert.That(nota.IdentificacaoNFSe.ChaveNotaNacional).StartsWith("3550308");
            await Assert.That(nota.XmlRetornoComplementarIBSCBS).IsEqualTo(complemento);
        }
        else
        {
            await Assert.That(nota.IdentificacaoNFSe.DataFatoGerador).IsNull();
            await Assert.That(string.IsNullOrEmpty(nota.IdentificacaoNFSe.ChaveNotaNacional)).IsTrue();
            await Assert.That(string.IsNullOrEmpty(nota.XmlRetornoComplementarIBSCBS)).IsTrue();
        }
    }

    [Test]
    [Arguments("1")]
    [Arguments("2")]
    [Arguments("3")]
    [Arguments("4")]
    [Arguments("5")]
    public async Task TipoOperacaoSegueOrdemDoLayout2(string tipoOperacao)
    {
        using var certificado = CriarCertificado();
        var nota = CriarNotaExemploOficial();
        nota.Servico.Valores.IBSCBS!.TipoOperacao = tipoOperacao;
        var rps = XElement.Parse(ISSSaoPauloLayout2.WriteXmlRps(nota, certificado, false, false));
        var ibsCbs = rps.Element("IBSCBS")!;

        await Assert.That(ibsCbs.Element("tpOper")?.Value).IsEqualTo(tipoOperacao);
        await Assert.That(ibsCbs.Elements().Select(elemento => elemento.Name.LocalName)).IsEquivalentTo(new[] { "finNFSe", "indFinal", "cIndOp", "tpOper", "indDest", "valores" }, TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments(" ")]
    public async Task TipoOperacaoNaoInformadoPreservaXmlAnterior(string? tipoOperacao)
    {
        using var certificado = CriarCertificado();
        var nota = CriarNotaExemploOficial();
        var original = ISSSaoPauloLayout2.WriteXmlRps(nota, certificado, false, false);
        nota.Servico.Valores.IBSCBS!.TipoOperacao = tipoOperacao;

        await Assert.That(ISSSaoPauloLayout2.WriteXmlRps(nota, certificado, false, false)).IsEqualTo(original);
    }

    [Test]
    [Arguments("0")]
    [Arguments("6")]
    [Arguments("01")]
    [Arguments("abc")]
    public async Task TipoOperacaoInvalidoNaoGeraXml(string tipoOperacao)
    {
        using var certificado = CriarCertificado();
        var nota = CriarNotaExemploOficial();
        nota.Servico.Valores.IBSCBS!.TipoOperacao = tipoOperacao;

        var erro = await Assert.That(() =>
            ISSSaoPauloLayout2.WriteXmlRps(nota, certificado, false, false)).Throws<Exception>();

        await Assert.That(erro!.Message).Contains("tpOper");
    }

    [Test]
    [Arguments("1")]
    [Arguments("5")]
    public async Task RetornoLayout2PreservaTipoOperacao(string tipoOperacao)
    {
        var config = new ConfigNFSe();
        config.WebServices.CodigoMunicipio = 3550308;
        config.WebServices.LayoutISSSaoPaulo = LayoutISSSaoPaulo.Layout2;
        var openNFSe = new OpenNFSe(config);
        var xml = "<NFe><ChaveNFe><InscricaoPrestador>123456789012</InscricaoPrestador>" +
                  "<NumeroNFe>987</NumeroNFe><CodigoVerificacao>ABC123</CodigoVerificacao>" +
                  "</ChaveNFe><IBSCBS><tpOper>" + tipoOperacao + "</tpOper></IBSCBS></NFe>";

        var nota = openNFSe.NotasServico.Load(xml);

        await Assert.That(nota.Servico.Valores.IBSCBS?.TipoOperacao).IsEqualTo(tipoOperacao);
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

    private static async Task ValidarXml(string xml, string schema)
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

        await Assert.That(erros).IsEmpty().Because(string.Join(Environment.NewLine, erros));
    }
}
