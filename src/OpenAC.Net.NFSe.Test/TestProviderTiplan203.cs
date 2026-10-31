using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Test;

public class TestProviderTiplan203
{
    [Test]
    public async Task CamposNulosPreservamXmlLegado()
    {
        var nota = CriarNota();
        var xmlSemGrupo = nota.GetXml();

        nota.Servico.Valores.IBSCBS = new InfoIBSCBS();
        var xmlComObjetoVazio = nota.GetXml();

        await Assert.That(xmlComObjetoVazio).IsEqualTo(xmlSemGrupo);
        await Assert.That(xmlSemGrupo).DoesNotContain("<IBSCBS>");
        await Assert.That(xmlSemGrupo).Contains("<CodigoNbs>118054000</CodigoNbs>");
    }

    [Test]
    public async Task GrupoCompletoEhGeradoNaOrdemOficial()
    {
        var nota = CriarNota(3303302);
        PreencherGrupoCompleto(nota);

        var xml = XDocument.Parse(nota.GetXml());
        var grupo = xml.Descendants().Single(x => x.Name.LocalName == "IBSCBS");
        var nomes = grupo.Elements().Select(x => x.Name.LocalName).ToArray();
        var valoresTributos = grupo.Elements().Single(x => x.Name.LocalName == "ValoresTributos");

        await Assert.That(nomes).IsEquivalentTo(new[] { "OperacaoUsoConsumoPessoal", "Operacao", "ValoresTributos" }, TUnit.Assertions.Enums.CollectionOrdering.Matching);
        await Assert.That(grupo.Elements().Single(x => x.Name.LocalName == "OperacaoUsoConsumoPessoal").Value).IsEqualTo("1");
        await Assert.That(grupo.Elements().Single(x => x.Name.LocalName == "Operacao").Value).IsEqualTo("123456");
        await Assert.That(valoresTributos.Elements().Single(x => x.Name.LocalName == "SituacaoTributaria").Value).IsEqualTo("200");
        await Assert.That(valoresTributos.Elements().Single(x => x.Name.LocalName == "ClassificacaoTributaria").Value).IsEqualTo("200001");
    }

    public static IEnumerable<(string? IndicadorFinal, string? Operacao, string? Classificacao)> ConfiguracoesParciais()
    {
        yield return ("1", null, null);
        yield return (null, "123456", null);
        yield return (null, null, "200001");
        yield return ("1", "123456", null);
        yield return ("1", null, "200001");
        yield return (null, "123456", "200001");
    }

    [Test]
    [MethodDataSource(nameof(ConfiguracoesParciais))]
    public async Task ConfiguracaoParcialEhRecusada(string? indicadorFinal, string? operacao, string? classificacao)
    {
        var nota = CriarNota();
        nota.Servico.Valores.IBSCBS = new InfoIBSCBS
        {
            IndicadorFinal = indicadorFinal,
            CodigoIndicadorOperacao = operacao
        };
        nota.Servico.Valores.IBSCBS.Valores.Tributos.SituacaoClassificacao.CodigoClassificacaoTributaria =
            classificacao;

        var erro = await Assert.That(() => nota.GetXml()).Throws<Exception>();
        await Assert.That(erro!.Message).Contains("IBS/CBS da Tiplan");
    }

    [Test]
    public async Task SituacaoInformadaDiferenteDaClassificacaoEhRecusada()
    {
        var nota = CriarNota();
        PreencherGrupoCompleto(nota);
        nota.Servico.Valores.IBSCBS!.Valores.Tributos.SituacaoClassificacao.CodigoSituacaoTributaria = "999";

        var erro = await Assert.That(() => nota.GetXml()).Throws<Exception>();
        await Assert.That(erro!.Message).Contains("três primeiros dígitos");
    }

    [Test]
    public async Task XmlCompletoValidaNoXsdDistribuido()
    {
        var nota = CriarNota();
        PreencherGrupoCompleto(nota);
        await ValidarXml(nota.GetXml(), ObterSchema());
    }

    private static NotaServico CriarNota(int codigoMunicipio = 3501608)
    {
        var openNFSe = new OpenNFSe(new ConfigNFSe());
        openNFSe.Configuracoes.Geral.Salvar = false;
        openNFSe.Configuracoes.Arquivos.Salvar = false;
        openNFSe.Configuracoes.WebServices.CodigoMunicipio = codigoMunicipio;
        openNFSe.Configuracoes.PrestadorPadrao.CpfCnpj = "12345678000190";
        openNFSe.Configuracoes.PrestadorPadrao.InscricaoMunicipal = "12345";

        var nota = openNFSe.NotasServico.AddNew();
        nota.IdentificacaoRps.Numero = "1";
        nota.IdentificacaoRps.Serie = "A";
        nota.IdentificacaoRps.Tipo = TipoRps.RPS;
        nota.IdentificacaoRps.DataEmissao = new DateTime(2026, 8, 1);
        nota.Competencia = new DateTime(2026, 8, 1);
        nota.Situacao = SituacaoNFSeRps.Normal;
        nota.RegimeEspecialTributacao = RegimeEspecialTributacao.Nenhum;
        nota.OptanteSimplesNacional = NFSeSimNao.Nao;
        nota.IncentivadorCultural = NFSeSimNao.Nao;

        nota.Prestador.CpfCnpj = "12345678000190";
        nota.Prestador.InscricaoMunicipal = "12345";
        nota.Servico.Valores.ValorServicos = 100M;
        nota.Servico.Valores.BaseCalculo = 100M;
        nota.Servico.Valores.Aliquota = 2M;
        nota.Servico.Valores.IssRetido = SituacaoTributaria.Normal;
        nota.Servico.ItemListaServico = "0107";
        nota.Servico.CodigoNbs = "118054000";
        nota.Servico.Discriminacao = "Serviço de teste sem dados reais";
        nota.Servico.CodigoMunicipio = codigoMunicipio;
        nota.Servico.MunicipioIncidencia = codigoMunicipio;
        nota.Servico.ExigibilidadeIss = ExigibilidadeIss.Exigivel;

        nota.Tomador.CpfCnpj = "12345678901";
        nota.Tomador.RazaoSocial = "Tomador de Teste";
        nota.Tomador.Endereco.Logradouro = "Rua de Teste";
        nota.Tomador.Endereco.Numero = "1";
        nota.Tomador.Endereco.Bairro = "Centro";
        nota.Tomador.Endereco.CodigoMunicipio = codigoMunicipio;
        nota.Tomador.Endereco.Uf = "SP";
        nota.Tomador.Endereco.Cep = "13465000";
        return nota;
    }

    private static void PreencherGrupoCompleto(NotaServico nota)
    {
        nota.Servico.Valores.IBSCBS = new InfoIBSCBS
        {
            IndicadorFinal = "1",
            CodigoIndicadorOperacao = "123456"
        };
        nota.Servico.Valores.IBSCBS.Valores.Tributos.SituacaoClassificacao.CodigoClassificacaoTributaria =
            "200001";
    }

    private static string ObterSchema()
    {
        return Path.Combine(AppContext.BaseDirectory, "Schemas", "Tiplan", "2.03", "nfse.xsd");
    }

    private static async Task ValidarXml(string xml, string schema)
    {
        var erros = new List<string>();
        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema
        };
        settings.Schemas.XmlResolver = new XmlUrlResolver();
        settings.Schemas.Add(null, schema);
        settings.ValidationEventHandler += (_, args) => erros.Add(args.Message);
        using var reader = XmlReader.Create(new StringReader(xml), settings);
        while (reader.Read())
        {
        }

        await Assert.That(erros).IsEmpty().Because(string.Join(Environment.NewLine, erros));
    }
}
