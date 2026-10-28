using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Nota;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Demo;

public partial class MainWindow
{
    private async Task GenerateRpsAsync(CancellationToken token)
    {
        var municipio = ProviderManager.Municipios.Single(item => item.Codigo == client.Configuracoes.WebServices.CodigoMunicipio);
        if (municipio == null) return;

        var fields = await InputDialog.AskAsync(this, "Gerar RPS de exemplo",
            new("numero", "Número do RPS", "1"), new("item", "Item na lista de serviço", "07.10"),
            new("cnae", "CNAE", GetCnae(municipio)), new("tributacao", "Código de tributação municipal", "01.07.00"));
        if (fields == null) return;
        token.ThrowIfCancellationRequested();
        var numeroRps = fields["numero"];

        client.NotasServico.Clear();
        var nfSe = client.NotasServico.AddNew();

        nfSe.IdentificacaoRps.Numero = numeroRps;

        // Setar a serie de acordo com o provedor.
        switch (municipio.Provedor)
        {
            case NFSeProvider.ISSCuritiba:
                nfSe.IdentificacaoRps.Serie = "F";
                break;

            case NFSeProvider.ISSDSF:
                nfSe.IdentificacaoRps.Serie = "NF";
                nfSe.IdentificacaoRps.SeriePrestacao = "99";
                break;

            case NFSeProvider.GISS:
                nfSe.IdentificacaoRps.Serie = "NFT";
                break;

            default:
                nfSe.IdentificacaoRps.Serie = "1";
                break;
        }

        nfSe.IdentificacaoRps.Tipo = TipoRps.RPS;
        nfSe.IdentificacaoRps.DataEmissao = DateTime.Now;
        nfSe.Competencia = DateTime.Now;
        nfSe.Situacao = SituacaoNFSeRps.Normal;
        nfSe.OptanteSimplesNacional = NFSeSimNao.Sim;
        nfSe.OptanteMEISimei = NFSeSimNao.Nao;

        // Setar a natureza de operação de acordo com o provedor.
        switch (municipio.Provedor)
        {
            case NFSeProvider.ISSDSF:
                nfSe.NaturezaOperacao = NaturezaOperacao.DSF.SemDeducao;
                break;

            default:
                nfSe.NaturezaOperacao = NaturezaOperacao.ABRASF.TributacaoNoMunicipio;
                break;
        }

        nfSe.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
        nfSe.IncentivadorCultural = NFSeSimNao.Nao;

        var itemListaServico = fields["item"];

        // Setar o cnae de acordo com o schema aceito pelo provedor.
        var cnae = fields["cnae"];
        nfSe.Servico.CodigoCnae = cnae;

        nfSe.Servico.ItemListaServico = itemListaServico;
        nfSe.Servico.CodigoTributacaoMunicipio = fields["tributacao"];
        nfSe.Servico.Discriminacao = "MANUTENCAO TÉCNICA";
        nfSe.Servico.CodigoMunicipio = municipio.Codigo;
        nfSe.Servico.Municipio = municipio.Nome;
        nfSe.OutrasInformacoes = "VOCÊ PAGOU APROXIMADAMENTE R$ 41,15 DE TRIBUTOS FEDERAIS, R$ 8,26 DE TRIBUTOS MUNICIPAIS, R$ 256,57 PELOS PRODUTOS/SERVICOS, FONTE: IBPT.";
        if (municipio.Provedor.IsIn(NFSeProvider.SiapNet))
        {
            nfSe.Servico.ResponsavelRetencao = ResponsavelRetencao.Prestador;
            nfSe.Servico.MunicipioIncidencia = nfSe.Servico.CodigoMunicipio;
        }

        if (municipio.Provedor.IsIn(NFSeProvider.GISS))
            nfSe.Servico.CodigoPais = 0076;

        nfSe.Servico.Valores.ValorServicos = 1;
        nfSe.Servico.Valores.ValorDeducoes = 0;
        nfSe.Servico.Valores.ValorPis = 0;
        nfSe.Servico.Valores.ValorCofins = 0;
        nfSe.Servico.Valores.ValorInss = 0;
        nfSe.Servico.Valores.ValorIr = 0;
        nfSe.Servico.Valores.ValorCsll = 0;
        nfSe.Servico.Valores.IssRetido = SituacaoTributaria.Normal;
        nfSe.Servico.Valores.ValorIss = municipio.Provedor == NFSeProvider.SiapNet ? 2 : 0.20m;
        nfSe.Servico.Valores.ValorOutrasRetencoes = 0;
        nfSe.Servico.Valores.BaseCalculo = 1;
        nfSe.Servico.Valores.Aliquota =  municipio.Provedor == NFSeProvider.GISS ? 0.0200m : 2;
        nfSe.Servico.Valores.ValorLiquidoNfse = 1;
        nfSe.Servico.Valores.ValorIssRetido = 0;
        nfSe.Servico.Valores.DescontoCondicionado = 0;
        nfSe.Servico.Valores.DescontoIncondicionado = 0;
        nfSe.ValorCredito = 0;

        if (municipio.Provedor == NFSeProvider.ISSDSF)
        {
            var servico = nfSe.Servico.ItemsServico.AddNew();
            servico.Descricao = "Teste";
            servico.Quantidade = 1M;
            servico.ValorTotal = 1;
            servico.Tributavel = NFSeSimNao.Sim;
        }

        if (municipio.Provedor == NFSeProvider.GISS)
        {
            nfSe.Servico.MunicipioIncidencia = nfSe.Servico.CodigoMunicipio;
        }

        nfSe.Tomador.CpfCnpj = "49413978867";
        nfSe.Tomador.InscricaoMunicipal = "";
        nfSe.Tomador.RazaoSocial = "Nome";

        nfSe.Tomador.Endereco.TipoLogradouro = "";
        nfSe.Tomador.Endereco.Logradouro = "INDEPENDENCIA";
        nfSe.Tomador.Endereco.Numero = "123";
        nfSe.Tomador.Endereco.Complemento = "SL 10";
        nfSe.Tomador.Endereco.Bairro = "VILA SEIXAS";
        nfSe.Tomador.Endereco.CodigoMunicipio = municipio.Codigo;
        nfSe.Tomador.Endereco.Municipio = municipio.Nome;
        nfSe.Tomador.Endereco.Uf = municipio.UF.ToString();
        nfSe.Tomador.Endereco.Cep = "14876155";
        nfSe.Tomador.Endereco.CodigoPais = 1058;
        nfSe.Tomador.Endereco.Pais = "BRASIL";

        nfSe.Tomador.DadosContato.DDD = "16";
        nfSe.Tomador.DadosContato.Telefone = "1630111234";
        nfSe.Tomador.DadosContato.Email = "NOME@EMPRESA.COM.BR";
        UpdateDocumentCount();
        ResultText.Text = "RPS de exemplo criado. Revise os dados e os códigos de serviço antes de enviar.";
        SentXmlText.Text = await Task.Run(nfSe.GetXml, token);
    }

    private static string GetCnae(OpenMunicipioNFSe municipio)
    {
        return municipio.Provedor switch
        {
            NFSeProvider.SiapNet => "5211701",
            NFSeProvider.Sintese => "5211701",
            NFSeProvider.ABase => "5211701",
            NFSeProvider.Pronim when municipio.Versao == VersaoNFSe.ve203 => "5211701",
            NFSeProvider.SigISS when municipio.Versao == VersaoNFSe.ve103 => "5211701",
            NFSeProvider.Ginfes => "5211701",
            NFSeProvider.Tiplan => "5211701",
            _ => "861010101"
        };
    }

}
