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
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
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

        // A consulta do ISSMap exige o número do RPS e o CPF/CNPJ do tomador.
        // O 2º parâmetro (serieNfse) é reaproveitado pelo provedor para o documento do tomador.
        var retorno = openNFSe.ConsultaNFSe(1, "");

        Assert.True(retorno.Sucesso,
            "Erro na consulta: " + string.Join(" | ", retorno.Erros.ConvertAll(e => $"{e.Codigo}-{e.Descricao}")));
    }

    /// <summary>
    /// Consulta a versão digital (PDF) de um RPS já transmitido (serviço de QRCode).
    /// Ajuste o número do RPS. O PDF é salvo em disco quando a consulta tem sucesso.
    /// </summary>
    [Fact]
    public void ConsultarPdf()
    {
        var openNFSe = CriarOpenNFSe();

        var retorno = openNFSe.ConsultaNFSePdf(1);

        Assert.True(retorno.Sucesso,
            "Erro na consulta do PDF: " + string.Join(" | ", retorno.Erros.ConvertAll(e => $"{e.Codigo}-{e.Descricao}")));
        Assert.NotNull(retorno.Pdf);
        Assert.NotEmpty(retorno.Pdf!);

        var nomeArquivo = string.IsNullOrWhiteSpace(retorno.NomeArquivo) ? "issmap-rps.pdf" : retorno.NomeArquivo;
        var pastaArquivo = System.IO.Path.Combine(System.IO.Path.GetTempPath(), nomeArquivo);
        System.IO.File.WriteAllBytes(pastaArquivo, retorno.Pdf!);
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

    /// <summary>
    /// Descriptografa um XML do ISSMap (envio ou retorno) capturado da comunicação,
    /// usando a Chave de Acesso. Cole o XML criptografado em <c>xmlCriptografado</c>
    /// e preencha <see cref="IssMapChaveAcesso"/> antes de executar.
    /// </summary>
    [Fact]
    public void DescriptografarXmlIssMap()
    {
        const string xmlCriptografado = ""; // cole aqui o XML com os conteúdos das tags criptografados

        var xmlClaro = DescriptografarXml(xmlCriptografado);

        Console.WriteLine(xmlClaro);
        Assert.False(string.IsNullOrWhiteSpace(xmlClaro), "XML descriptografado vazio.");
    }

    #region Descriptografia ISSMap

    /// <summary>
    /// Descriptografa um XML do ISSMap, decifrando o conteúdo de todas as tags folha
    /// (exceto a tag <c>key</c>, que não é criptografada) com a Chave de Acesso (AES-128/ECB/PKCS7).
    /// Espelha o comportamento de <c>ProviderISSMap.DescriptografarXml</c>/<c>ISSMapCripto</c>,
    /// replicado aqui por serem tipos <c>internal</c> da biblioteca.
    /// </summary>
    /// <param name="xmlCriptografado">XML com os conteúdos das tags criptografados em Base64.</param>
    /// <param name="chaveAcessoBase64">Chave de Acesso (AES) em Base64. Quando vazio, usa <see cref="IssMapChaveAcesso"/>.</param>
    /// <returns>O mesmo XML, com os conteúdos das tags em claro.</returns>
    public static string DescriptografarXml(string xmlCriptografado, string chaveAcessoBase64 = "")
    {
        var chave = string.IsNullOrEmpty(chaveAcessoBase64) ? IssMapChaveAcesso : chaveAcessoBase64;
        if (string.IsNullOrWhiteSpace(chave))
            throw new InvalidOperationException("Chave de Acesso (criptografia) do ISSMap não informada.");

        var doc = XDocument.Parse(xmlCriptografado);
        foreach (var element in doc.Descendants())
        {
            if (element.HasElements) continue;
            if (element.Name.LocalName.Equals("key", StringComparison.OrdinalIgnoreCase)) continue;
            if (string.IsNullOrWhiteSpace(element.Value)) continue;

            try
            {
                element.Value = Descriptografar(element.Value, chave);
            }
            catch
            {
                // mantém o valor original caso a tag não esteja criptografada
            }
        }

        return doc.ToString(SaveOptions.DisableFormatting);
    }

    /// <summary>
    /// Descriptografa um texto cifrado pelo WebService IssMap (AES-128, modo ECB, padding PKCS7).
    /// </summary>
    private static string Descriptografar(string textoCifrado, string chaveBase64)
    {
        if (string.IsNullOrEmpty(textoCifrado)) return string.Empty;

        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(chaveBase64);
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        var dados = Convert.FromBase64String(textoCifrado);
        var decifrado = decryptor.TransformFinalBlock(dados, 0, dados.Length);

        return Encoding.UTF8.GetString(decifrado);
    }

    #endregion Descriptografia ISSMap
}
