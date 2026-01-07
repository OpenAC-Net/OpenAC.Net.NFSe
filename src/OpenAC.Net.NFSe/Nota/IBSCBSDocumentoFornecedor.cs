using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class IBSCBSDocumentoFornecedor : GenericClone<IBSCBSDocumentoFornecedor>
{
    public string? Cnpj { get; set; }

    public string? Cpf { get; set; }

    public string? Nif { get; set; }

    public string? CodigoNaoNif { get; set; }

    public string? Nome { get; set; }
}