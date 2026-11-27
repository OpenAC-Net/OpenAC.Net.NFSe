using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Storage;
using Xunit;

namespace OpenAC.Net.NFSe.Test;

public class TestStorageCompatibility
{
    [Theory]
    [InlineData(NFSeCategorias.Rps)]
    [InlineData(NFSeCategorias.NFSe)]
    [InlineData(NFSeCategorias.SolicitacaoSoap)]
    [InlineData(NFSeCategorias.RespostaSoap)]
    [InlineData(NFSeCategorias.SolicitacaoTecnica)]
    [InlineData(NFSeCategorias.RespostaTecnica)]
    public async Task CategoriesKeepPreviousPathsForEveryDirectoryGrouping(string categoria)
    {
        var root = Path.Combine(Path.GetTempPath(), $"nfse-path-compat-{Guid.NewGuid():N}");
        const string cnpj = "12.345.678/0001-90";
        var date = new DateTime(2024, 2, 29);
        try
        {
            for (var flags = 0; flags < 64; flags++)
            {
                var config = new ConfigNFSe();
                var files = config.Arquivos;
                var caseRoot = Path.Combine(root, flags.ToString());
                files.PathNFSe = Path.Combine(caseRoot, "Notas");
                files.PathRps = Path.Combine(caseRoot, "Solicitacoes");
                files.PathLote = Path.Combine(caseRoot, "Mensagens");
                files.SepararPorCNPJ = (flags & 1) != 0;
                files.SepararPorModelo = (flags & 2) != 0;
                files.SepararPorAno = (flags & 4) != 0;
                files.SepararPorMes = (flags & 8) != 0;
                files.SepararPorDia = (flags & 16) != 0;
                files.AdicionarLiteral = (flags & 32) != 0;
                var expected = categoria switch
                {
                    NFSeCategorias.Rps => files.GetPathRps(date, cnpj),
                    NFSeCategorias.NFSe => files.GetPathNFSe(date, cnpj),
                    NFSeCategorias.SolicitacaoSoap or NFSeCategorias.RespostaSoap => files.GetPathSoap(date, cnpj),
                    _ => files.GetPathLote(date, cnpj)
                };
                await config.DocumentStore.SaveAsync(
                    new NFSeDocument(config, categoria, "ação", "nota.xml", cnpj, date));
                Assert.Equal("ação", await File.ReadAllTextAsync(Path.Combine(expected, "nota.xml")));
                Assert.Single(Directory.GetFiles(caseRoot, "*", SearchOption.AllDirectories));
            }
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }
}