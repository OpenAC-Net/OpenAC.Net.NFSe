using OpenAC.Net.DFe.Core.Storage;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Storage;

namespace OpenAC.Net.NFSe.Test;

public class TestDocumentRouting
{
    [Test]
    [Arguments(NFSeTipoDocumento.Rps, "RPS")]
    [Arguments(NFSeTipoDocumento.NFSe, "NFSe")]
    [Arguments(NFSeTipoDocumento.SolicitacaoSoap, "NFSe/SOAP")]
    [Arguments(NFSeTipoDocumento.RespostaSoap, "NFSe/SOAP")]
    [Arguments(NFSeTipoDocumento.SolicitacaoTecnica, "Lote")]
    [Arguments(NFSeTipoDocumento.RespostaTecnica, "Lote")]
    public async Task EnumDoComponentePreservaDiretoriosSemDependerDoNome(NFSeTipoDocumento type, string destination)
    {
        var root = Path.Combine(Path.GetTempPath(), "nfse-routing-" + Guid.NewGuid().ToString("N"));
        try
        {
            var configuration = new ConfigNFSe();
            configuration.Arquivos.AdicionarLiteral = true;
            configuration.Arquivos.PathRps = Path.Combine(root, "RPS");
            configuration.Arquivos.PathNFSe = Path.Combine(root, "NFSe");
            configuration.Arquivos.PathLote = Path.Combine(root, "Lote");
            var document = new NFSeDocument(type, "conteúdo", "documento.xml", "", DateTime.Today);
            await DesktopNFSeDocumentStore.Instancia.SaveAsync(document, configuration);
            await Assert.That(document.Tipo.GetType()).IsEqualTo(typeof(NFSeTipoDocumento));
            await Assert.That(File.Exists(Path.Combine(root, destination, "documento.xml"))).IsTrue();
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    [Test]
    public async Task DocumentoGenericoEDocumentoFiscalMantemDestinosDistintos()
    {
        var root = Path.Combine(Path.GetTempPath(), "nfse-enum-" + Guid.NewGuid().ToString("N"));
        try
        {
            var configuration = new ConfigNFSe();
            configuration.Arquivos.PathSalvar = Path.Combine(root, "Core");
            configuration.Arquivos.PathLote = Path.Combine(root, "Lote");
            var coreDocument = new DFeDocument(Path.Combine(root, "Core", "documento.xml"), "core");
            var nfseDocument = new NFSeDocument(NFSeTipoDocumento.SolicitacaoTecnica, "nfse", "documento.xml", null, DateTime.Today);
            await DFeFileSystemDocumentStore.Instance.SaveAsync(coreDocument);
            await DesktopNFSeDocumentStore.Instancia.SaveAsync(nfseDocument, configuration);
            await Assert.That(await File.ReadAllTextAsync(Path.Combine(root, "Core", "documento.xml"))).IsEqualTo("core");
            await Assert.That(await File.ReadAllTextAsync(Path.Combine(root, "Lote", "documento.xml"))).IsEqualTo("nfse");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }
}