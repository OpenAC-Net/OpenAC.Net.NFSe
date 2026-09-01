using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.Providers;
using Xunit;

namespace OpenAC.Net.NFSe.Test;

public class TestProviderBarueri
{
    private const int CodigoMunicipioBarueri = 3505708;
    private const string Endpoint = "https://www.barueri.sp.gov.br/nfeservice/wsrps.asmx";
    private const string NamespaceBarueri = "http://www.barueri.sp.gov.br/nfe";

    [Fact]
    public void CadastroMunicipalSelecionaProviderEVersaoCorretos()
    {
        var municipio = Assert.Single(ProviderManager.Municipios,
            x => x.Codigo == CodigoMunicipioBarueri);

        Assert.Equal("Barueri", municipio.Nome);
        Assert.Equal("SP", municipio.UF.ToString());
        Assert.Equal(NFSeProvider.Barueri, municipio.Provedor);
        Assert.Equal(VersaoNFSe.ve100, municipio.Versao);
        Assert.Equal(58, (sbyte)NFSeProvider.Barueri);
    }

    [Theory]
    [InlineData(TipoUrl.Enviar)]
    [InlineData(TipoUrl.CancelarNFSe)]
    [InlineData(TipoUrl.ConsultarNFSe)]
    [InlineData(TipoUrl.ConsultarNFSeRps)]
    [InlineData(TipoUrl.ConsultarLoteRps)]
    [InlineData(TipoUrl.ConsultarSituacao)]
    public void EndpointsDeHomologacaoEProducaoApontamParaServicoOficial(TipoUrl tipoUrl)
    {
        var municipio = Assert.Single(ProviderManager.Municipios,
            x => x.Codigo == CodigoMunicipioBarueri);

        Assert.Equal(Endpoint, municipio.UrlHomologacao[tipoUrl]);
        Assert.Equal(Endpoint, municipio.UrlProducao[tipoUrl]);
    }

    [Fact]
    public void FactoryCriaProviderBarueriNaVersao100()
    {
        var config = CriarConfiguracao();

        using var provider = ProviderManager.GetProvider(config);

        Assert.Equal("ProviderBarueri", provider.GetType().Name);
        Assert.Equal("Barueri", provider.Name);
        Assert.Equal(VersaoNFSe.ve100, provider.Versao);
        Assert.Equal(CodigoMunicipioBarueri, provider.Municipio.Codigo);
    }

    [Fact]
    public void XmlRpsContemCamposObrigatoriosEFormataValores()
    {
        using var culture = new CultureScope(CultureInfo.InvariantCulture);
        using var openNFSe = new OpenNFSe(CriarConfiguracao());
        var nota = CriarNota(openNFSe);

        var xml = XDocument.Parse(nota.GetXml());
        var rps = Assert.IsType<XElement>(xml.Root);

        Assert.Equal("RPS", rps.Name.LocalName);
        Assert.Equal("12345678000190", rps.Element("Prestador")?.Element("CPFCNPJPrestador")?.Value);
        Assert.Equal("7654321", rps.Element("Prestador")?.Element("InscricaoMunicipal")?.Value);
        Assert.Equal("RPS", rps.Element("TipoRPS")?.Value);
        Assert.Equal("UNICA", rps.Element("Serie")?.Value);
        Assert.Equal("42", rps.Element("Numero")?.Value);
        Assert.Equal("2026-08-15", rps.Element("DataEmissao")?.Value);
        Assert.Equal("123.45", rps.Element("ValorServico")?.Value);
        Assert.Equal("2.5000", rps.Element("AliquotaServico")?.Value);
        Assert.Equal("2", rps.Element("ISSRetido")?.Value);
        Assert.Null(rps.Element("ValorISSRetido"));
        Assert.Equal("Manutenção de equipamentos", rps.Element("Discriminacao")?.Value);
        Assert.Equal("Cliente de Teste", rps.Element("Tomador")?.Element("RazaoSocialTomador")?.Value);
    }

    [Fact]
    public void XmlRpsIncluiValorIssQuandoRetido()
    {
        using var culture = new CultureScope(CultureInfo.InvariantCulture);
        using var openNFSe = new OpenNFSe(CriarConfiguracao());
        var nota = CriarNota(openNFSe);
        nota.Servico.Valores.IssRetido = SituacaoTributaria.Retencao;
        nota.Servico.Valores.ValorIssRetido = 3.09m;

        var rps = XDocument.Parse(nota.GetXml()).Root!;

        Assert.Equal("1", rps.Element("ISSRetido")?.Value);
        Assert.Equal("3.09", rps.Element("ValorISSRetido")?.Value);
    }

    [Fact]
    public void LeituraDoXmlRpsPreservaOsDadosMapeados()
    {
        using var culture = new CultureScope(CultureInfo.InvariantCulture);
        var config = CriarConfiguracao();
        using var origem = new OpenNFSe(config);
        var xml = CriarNota(origem).GetXml();
        using var destino = new OpenNFSe(CriarConfiguracao());

        var nota = destino.NotasServico.Load(xml);

        Assert.Equal("42", nota.IdentificacaoRps.Numero);
        Assert.Equal("UNICA", nota.IdentificacaoRps.Serie);
        Assert.Equal("12345678000190", nota.Prestador.CpfCnpj);
        Assert.Equal("7654321", nota.Prestador.InscricaoMunicipal);
        Assert.Equal("98765432100", nota.Tomador.CpfCnpj);
        Assert.Equal("Cliente de Teste", nota.Tomador.RazaoSocial);
        Assert.Equal(123.45m, nota.Servico.Valores.ValorServicos);
        Assert.Equal(2.5m, nota.Servico.Valores.Aliquota);
        Assert.Equal("Manutenção de equipamentos", nota.Servico.Discriminacao);
        Assert.Equal(xml, nota.XmlOriginal);
    }

    [Fact]
    public void XmlRpsUsaFormatoNumericoXmlMesmoEmPtBr()
    {
        using var culture = new CultureScope(new CultureInfo("pt-BR"));
        using var openNFSe = new OpenNFSe(CriarConfiguracao());

        var rps = XDocument.Parse(CriarNota(openNFSe).GetXml()).Root!;

        Assert.Equal("123.45", rps.Element("ValorServico")?.Value);
        Assert.Equal("10.00", rps.Element("ValorDeducao")?.Value);
        Assert.Equal("2.5000", rps.Element("AliquotaServico")?.Value);
    }

    [Fact]
    public void PreparacaoDoEnvioGeraEnvelopeValidoEArquivoPosicionalEmBase64()
    {
        var config = CriarConfiguracao();
        using var openNFSe = new OpenNFSe(config);
        CriarNota(openNFSe);
        using var provider = ProviderManager.GetProvider(config);
        var retorno = CriarRetornoEnviar(7);

        InvocarPrepararEnviar(provider, retorno, openNFSe.NotasServico);

        Assert.Empty(retorno.Erros);
        var envelope = XDocument.Parse(retorno.XmlEnvio).Root!;
        XNamespace ns = NamespaceBarueri;
        Assert.Equal(ns + "NFeLoteEnviarArquivo", envelope.Name);
        Assert.Equal("7654321", envelope.Element(ns + "InscricaoMunicipal")?.Value);
        Assert.Equal("12345678000190", envelope.Element(ns + "CPFCNPJContrib")?.Value);
        Assert.Equal("lote_2026-08-15-42.txt", envelope.Element(ns + "NomeArquivoRPS")?.Value);
        Assert.Equal("false", envelope.Element(ns + "ApenasValidaArq")?.Value);

        var arquivo = Encoding.UTF8.GetString(Convert.FromBase64String(
            envelope.Element(ns + "ArquivoRPSBase64")!.Value));
        var linhas = arquivo.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(5, linhas.Length);
        Assert.Equal(new[] { 25, 1970, 531, 697, 38 }, linhas.Select(x => x.Length));
        Assert.Equal(new[] { '1', '2', '4', '5', '9' }, linhas.Select(x => x[0]));
        Assert.Equal("7654321", linhas[0].Substring(1, 7));
        Assert.Equal("PMB004", linhas[0].Substring(8, 6));
        Assert.All(linhas[0].Substring(14, 11), x => Assert.True(char.IsDigit(x)));
        Assert.EndsWith("42", linhas[0]);
        Assert.Equal("0000000042", linhas[1].Substring(15, 10));
        Assert.Equal("20260815", linhas[1].Substring(25, 8));
        Assert.Equal("00098765432100", linhas[1].Substring(504, 14));
        Assert.Contains("Manutencao de equipamentos", linhas[1]);
        Assert.Equal("0000005", linhas[4].Substring(1, 7));
        Assert.Equal("000000000012345", linhas[4].Substring(8, 15));
        Assert.Equal("000000000000000", linhas[4].Substring(23, 15));
    }

    [Fact]
    public void EnvioSemLoteEhRecusadoAntesDeQualquerChamadaHttp()
    {
        var config = CriarConfiguracao();
        using var openNFSe = new OpenNFSe(config);
        CriarNota(openNFSe);
        using var provider = ProviderManager.GetProvider(config);

        var retorno = provider.Enviar(0, openNFSe.NotasServico);

        var erro = Assert.Single(retorno.Erros);
        Assert.Equal("0", erro.Codigo);
        Assert.Equal("Lote não informado.", erro.Descricao);
        Assert.Empty(retorno.XmlEnvio);
    }

    [Fact]
    public void OperacoesNaoSuportadasFalhamLocalmenteSemChamadaHttp()
    {
        using var openNFSe = new OpenNFSe(CriarConfiguracao());
        var nota = CriarNota(openNFSe);
        nota.IdentificacaoNFSe.Numero = "123";

        var erroXml = Assert.Throws<NotImplementedException>(() => nota.GetXml());
        Assert.Contains("não implementa geração de XML de NFSe", erroXml.Message);

        nota.IdentificacaoNFSe.Numero = string.Empty;
        var erroEnvio = Assert.Throws<NotImplementedException>(() => openNFSe.Enviar(1, true));
        Assert.Contains("não implementa envio síncrono", erroEnvio.Message);
    }

    [Theory]
    [InlineData("ConsultaNFeRecebidaCompetencia.v1.xsd")]
    [InlineData("ConsultaNFeRecebidaNumero.v1.xsd")]
    [InlineData("ConsultaNFeRecebidaPeriodo.v1.xsd")]
    [InlineData("NFeLoteBaixarArquivo.v1.xsd")]
    [InlineData("NFeLoteEnviarArquivo.v1.xsd")]
    [InlineData("NFeLoteListarArquivos.v1.xsd")]
    [InlineData("NFeLoteStatusArquivo.v1.xsd")]
    public void SchemaDistribuidoCompilaSemErros(string arquivo)
    {
        var caminho = Path.Combine(AppContext.BaseDirectory, "Schemas", "Barueri", "1.00", arquivo);
        var schemas = new XmlSchemaSet();

        schemas.Add(NamespaceBarueri, caminho);
        schemas.Compile();

        Assert.True(schemas.IsCompiled);
    }

    private static ConfigNFSe CriarConfiguracao()
    {
        var config = new ConfigNFSe();
        config.Geral.Salvar = false;
        config.Arquivos.Salvar = false;
        config.WebServices.CodigoMunicipio = CodigoMunicipioBarueri;
        config.PrestadorPadrao.CpfCnpj = "12345678000190";
        config.PrestadorPadrao.InscricaoMunicipal = "7654321";
        return config;
    }

    private static NotaServico CriarNota(OpenNFSe openNFSe)
    {
        var nota = openNFSe.NotasServico.AddNew();
        nota.IdentificacaoRps.Numero = "42";
        nota.IdentificacaoRps.Serie = "UNICA";
        nota.IdentificacaoRps.Tipo = TipoRps.RPS;
        nota.IdentificacaoRps.DataEmissao = new DateTime(2026, 8, 15, 14, 30, 45);
        nota.Situacao = SituacaoNFSeRps.Normal;
        nota.Servico.ItemListaServico = "1401";
        nota.Servico.Discriminacao = "Manutenção de equipamentos";
        nota.Servico.CodigoMunicipio = CodigoMunicipioBarueri;
        nota.Servico.Valores.ValorServicos = 123.45m;
        nota.Servico.Valores.ValorDeducoes = 10m;
        nota.Servico.Valores.Aliquota = 2.5m;
        nota.Servico.Valores.ValorPis = 0.8m;
        nota.Servico.Valores.ValorCofins = 3.7m;
        nota.Tomador.CpfCnpj = "98765432100";
        nota.Tomador.RazaoSocial = "Cliente de Teste";
        nota.Tomador.Endereco.Logradouro = "Rua das Flores";
        nota.Tomador.Endereco.Numero = "100";
        nota.Tomador.Endereco.Bairro = "Centro";
        nota.Tomador.Endereco.Municipio = "Barueri";
        nota.Tomador.Endereco.CodigoMunicipio = CodigoMunicipioBarueri;
        nota.Tomador.Endereco.Uf = "SP";
        nota.Tomador.Endereco.Cep = "06400000";
        nota.Tomador.DadosContato.Email = "cliente@example.com";
        return nota;
    }

    private static RetornoEnviar CriarRetornoEnviar(int lote)
    {
        var retorno = new RetornoEnviar();
        typeof(RetornoEnviar).GetProperty(nameof(RetornoEnviar.Lote))!.SetValue(retorno, lote);
        return retorno;
    }

    private static void InvocarPrepararEnviar(ProviderBase provider, RetornoEnviar retorno,
        NotaServicoCollection notas)
    {
        var metodo = provider.GetType().GetMethod("PrepararEnviar",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(metodo);
        metodo.Invoke(provider, [retorno, notas]);
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo culturaAnterior = CultureInfo.CurrentCulture;

        public CultureScope(CultureInfo cultura)
        {
            CultureInfo.CurrentCulture = cultura;
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = culturaAnterior;
        }
    }
}
