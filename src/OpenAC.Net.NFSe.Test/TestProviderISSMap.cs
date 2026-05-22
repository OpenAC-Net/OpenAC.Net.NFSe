// =====================================================================================
//  TESTE - PROVEDOR ISSMap (Gemmap Informática)
//
//  Preencha as credenciais de integração e os dados de prestador/tomador abaixo
//  antes de executar (deixe-os em branco ao versionar - não faça commit de credenciais).
//
//  Credenciais obtidas no portal IssMap em "Configuração" > "Integração":
//   - Código da Chave de Acesso  -> campo <key> (NÃO é criptografado)
//   - Chave de Acesso            -> chave AES usada para criptografar os demais campos
//   - Senha do contribuinte      -> campo <pass>
//
//  Em homologação a emissão usa o endpoint de Teste de Envio
//  (https://www.issmap.com.br/ws/rps/teste/enviar/[cidade]), que valida o RPS sem persistir.
//  Em produção usa /novo/enviar/[cidade], que gera a NFS-e.
// =====================================================================================

using System;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nota;
using Xunit;

namespace OpenAC.Net.NFSe.Test;

public class TestProviderISSMap
{
    #region Dados de configuração (preencher localmente)

    private const int CodigoMunicipio = 3555505; // Ubirajara/SP

    // Integração ISSMap (portal: Configuração > Integração)
    private const string IssMapKey = "";          // Código da Chave de Acesso (campo <key>)
    private const string IssMapChaveAcesso = "";   // Chave de Acesso (chave AES de criptografia)
    private const string IssMapPass = "";          // Senha do contribuinte (campo <pass>)

    // Prestador
    private const string PrestadorCnpj = "";
    private const string PrestadorRazao = "";
    private const string PrestadorInscricaoMunicipal = "";

    #endregion Dados de configuração

    private static OpenNFSe CriarOpenNFSe()
    {
        var openNFSe = new OpenNFSe();

        openNFSe.Configuracoes.Geral.Salvar = false;
        openNFSe.Configuracoes.Arquivos.Salvar = false;

        // WebService
        openNFSe.Configuracoes.WebServices.Ambiente = DFeTipoAmbiente.Homologacao;
        openNFSe.Configuracoes.WebServices.CodigoMunicipio = CodigoMunicipio;
        openNFSe.Configuracoes.WebServices.Usuario = IssMapKey;             // <key>
        openNFSe.Configuracoes.WebServices.Senha = IssMapPass;              // <pass>
        openNFSe.Configuracoes.WebServices.ChaveAcesso = IssMapChaveAcesso; // chave AES

        // Prestador
        openNFSe.Configuracoes.PrestadorPadrao.CpfCnpj = PrestadorCnpj;
        openNFSe.Configuracoes.PrestadorPadrao.RazaoSocial = PrestadorRazao;
        openNFSe.Configuracoes.PrestadorPadrao.InscricaoMunicipal = PrestadorInscricaoMunicipal;
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.Logradouro = "";
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.Numero = "";
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.Complemento = "";
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.Bairro = "";
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.Municipio = "";
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.Uf = "";
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.Cep = "";
        openNFSe.Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio = CodigoMunicipio;
        openNFSe.Configuracoes.PrestadorPadrao.DadosContato.Email = "";

        return openNFSe;
    }

    /// <summary>
    /// Emite um RPS para validação no ISSMap (em homologação usa o Teste de Envio).
    /// </summary>
    [Fact]
    public void EmissaoNota()
    {
        var openNFSe = CriarOpenNFSe();

        var nota = openNFSe.NotasServico.AddNew();

        nota.Prestador.CpfCnpj = PrestadorCnpj;
        nota.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;

        nota.IdentificacaoRps.Numero = "1";
        nota.IdentificacaoRps.Serie = "1";
        nota.IdentificacaoRps.DataEmissao = DateTime.Now;

        // Serviço
        nota.Servico.Descricao = "";
        nota.Servico.ItemListaServico = "";   // formato 998877 (item/subitem/desdobro)
        nota.Servico.CodigoNbs = "";           // código NBS (campo cNBS)
        nota.Servico.CodigoMunicipio = CodigoMunicipio; // localExecucao

        nota.Servico.Valores.ValorServicos = 0.01M;
        nota.Servico.Valores.BaseCalculo = 0.01M;
        nota.Servico.Valores.Aliquota = 0M;
        nota.Servico.Valores.ValorIss = 0M;
        nota.Servico.Valores.IssRetido = SituacaoTributaria.Normal;

        // Tomador
        nota.Tomador.Tipo = TipoTomador.PessoaFisica;
        nota.Tomador.CpfCnpj = "";
        nota.Tomador.RazaoSocial = "";
        nota.Tomador.Endereco.Logradouro = "";
        nota.Tomador.Endereco.Numero = "";
        nota.Tomador.Endereco.Municipio = "";
        nota.Tomador.Endereco.Uf = "";
        nota.Tomador.Endereco.Cep = "";
        nota.Tomador.DadosContato.Email = "";

        var retorno = openNFSe.Enviar(0);

        Assert.True(retorno.Sucesso,
            "Erro no envio: " + string.Join(" | ", retorno.Erros.ConvertAll(e => $"{e.Codigo}-{e.Descricao}")));
    }

    /// <summary>
    /// Consulta um RPS já transmitido. Ajuste o número do RPS.
    /// </summary>
    [Fact]
    public void ConsultarNota()
    {
        var openNFSe = CriarOpenNFSe();

        var retorno = openNFSe.ConsultaNFSe(1); // número do RPS

        Assert.True(retorno.Sucesso,
            "Erro na consulta: " + string.Join(" | ", retorno.Erros.ConvertAll(e => $"{e.Codigo}-{e.Descricao}")));
    }

    /// <summary>
    /// Envia uma Carta de Cancelamento para um RPS transmitido.
    /// O 6º parâmetro (codigoVerificacao) é usado como CPF/CNPJ do tomador na carta.
    /// </summary>
    [Fact]
    public void CancelarNota()
    {
        var openNFSe = CriarOpenNFSe();

        var retorno = openNFSe.CancelarNFSe("", "1", "1", 0.01M, "Cancelamento de teste", "");

        Assert.True(retorno.Sucesso,
            "Erro no cancelamento: " + string.Join(" | ", retorno.Erros.ConvertAll(e => $"{e.Codigo}-{e.Descricao}")));
    }
}
