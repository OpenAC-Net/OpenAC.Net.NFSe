using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSDocumentoFornecedor : GenericClone<IBSCBSDocumentoFornecedor>
{
    /// <summary>
    /// Numero da inscricao no Cadastro Nacional de Pessoa Juridica (CNPJ) do fornecedor do servico.
    /// </summary>
    public string? Cnpj { get; set; }

    /// <summary>
    /// Numero da inscricao no Cadastro de Pessoa Fisica (CPF) do fornecedor do servico.
    /// </summary>
    public string? Cpf { get; set; }

    /// <summary>
    /// Deve ser preenchido apenas para fornecedores nao residentes no Brasil.
    /// </summary>
    public string? Nif { get; set; }

    /// <summary>
    /// Motivo para nao informacao do NIF: 0 - Nao informado na nota de origem; 1 - Dispensado do NIF; 2 - Nao exigencia do NIF.
    /// </summary>
    public string? CodigoNaoNif { get; set; }

    /// <summary>
    /// Nome/Razao Social do fornecedor do servico.
    /// </summary>
    public string? Nome { get; set; }
}
