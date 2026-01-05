using System;
using System.Collections.Generic;
using OpenAC.Net.Core.Generics;

namespace OpenAC.Net.NFSe.Nota;

public sealed class InfoIBSCBS : GenericClone<InfoIBSCBS>
{
    public InfoIBSCBS()
    {
        ReferenciasNFSe = new List<string>();
        Valores = new InfoValoresIBSCBS();
    }

    public string? FinalidadeNFSe { get; set; }

    public string? IndicadorFinal { get; set; }

    public string? CodigoIndicadorOperacao { get; set; }

    public string? TipoOperacao { get; set; }

    public List<string> ReferenciasNFSe { get; }

    public string? TipoEnteGov { get; set; }

    public string? IndicadorDestinatario { get; set; }

    public InfoValoresIBSCBS Valores { get; set; }
}

public sealed class InfoValoresIBSCBS : GenericClone<InfoValoresIBSCBS>
{
    public InfoValoresIBSCBS()
    {
        Tributos = new InfoTributosIBSCBS();
    }

    public InfoReeRepRes? ReembolsoRepasseRessarcimento { get; set; }

    public InfoTributosIBSCBS Tributos { get; set; }

    public string? CodigoLocalidadeIncidencia { get; set; }

    public decimal PercentualRedutor { get; set; }

    public decimal ValorBaseCalculo { get; set; }
}

public sealed class InfoTributosIBSCBS : GenericClone<InfoTributosIBSCBS>
{
    public InfoTributosIBSCBS()
    {
        SituacaoClassificacao = new InfoTributosSitClass();
    }

    public InfoTributosSitClass SituacaoClassificacao { get; set; }
}

public sealed class InfoTributosSitClass : GenericClone<InfoTributosSitClass>
{
    public string? CodigoSituacaoTributaria { get; set; }

    public string? CodigoClassificacaoTributaria { get; set; }
}

public sealed class InfoReeRepRes : GenericClone<InfoReeRepRes>
{
    public InfoReeRepRes()
    {
        Documentos = new List<IBSCBSDocumento>();
    }

    public ICollection<IBSCBSDocumento> Documentos { get; }
}

public sealed class IBSCBSDocumento : GenericClone<IBSCBSDocumento>
{
    public IBSCBSDocumentoDFe? DocumentoDFeNacional { get; set; }

    public IBSCBSDocumentoFiscalOutro? DocumentoFiscalOutro { get; set; }

    public IBSCBSDocumentoOutro? DocumentoOutro { get; set; }

    public IBSCBSDocumentoFornecedor? Fornecedor { get; set; }

    public DateTime DataEmissaoDocumento { get; set; }

    public DateTime DataCompetenciaDocumento { get; set; }

    public string? TipoReeRepRes { get; set; }

    public string? DescricaoTipoReeRepRes { get; set; }

    public decimal ValorReeRepRes { get; set; }
}

public sealed class IBSCBSDocumentoFornecedor : GenericClone<IBSCBSDocumentoFornecedor>
{
    public string? Cnpj { get; set; }

    public string? Cpf { get; set; }

    public string? Nif { get; set; }

    public string? CodigoNaoNif { get; set; }

    public string? Nome { get; set; }
}

public sealed class IBSCBSDocumentoOutro : GenericClone<IBSCBSDocumentoOutro>
{
    public string? NumeroDocumento { get; set; }

    public string? DescricaoDocumento { get; set; }
}

public sealed class IBSCBSDocumentoFiscalOutro : GenericClone<IBSCBSDocumentoFiscalOutro>
{
    public string? CodigoMunicipioDocumentoFiscal { get; set; }

    public string? NumeroDocumentoFiscal { get; set; }

    public string? DescricaoDocumentoFiscal { get; set; }
}

public sealed class IBSCBSDocumentoDFe : GenericClone<IBSCBSDocumentoDFe>
{
    public string? TipoChaveDFe { get; set; }

    public string? DescricaoTipoChaveDFe { get; set; }

    public string? ChaveDFe { get; set; }
}

public sealed class IBSCBSTotal : GenericClone<IBSCBSTotal>
{
    public IBSCBSTotal()
    {
        Valores = new IBSCBSValores();
        Totalizadores = new IBSCBSTotalCIBS();
    }

    public string? DescricaoLocalidadeIncidencia { get; set; }

    public string? DescricaoClassificacaoTributaria { get; set; }

    public IBSCBSValores Valores { get; set; }

    public IBSCBSTotalCIBS Totalizadores { get; set; }
}

public sealed class IBSCBSValores : GenericClone<IBSCBSValores>
{
    public IBSCBSValores()
    {
        UF = new IBSCBSValoresUF();
        Municipio = new IBSCBSValoresMun();
        Federal = new IBSCBSValoresFed();
    }

    public decimal ValorCalculadoReeRepRes { get; set; }

    public IBSCBSValoresUF UF { get; set; }

    public IBSCBSValoresMun Municipio { get; set; }

    public IBSCBSValoresFed Federal { get; set; }
}

public sealed class IBSCBSValoresUF : GenericClone<IBSCBSValoresUF>
{
    public decimal PercentualIBSUF { get; set; }

    public decimal PercentualReducaoAliquotaUF { get; set; }

    public decimal PercentualAliquotaEfetivaUF { get; set; }
}

public sealed class IBSCBSValoresMun : GenericClone<IBSCBSValoresMun>
{
    public decimal PercentualIBSMun { get; set; }

    public decimal PercentualReducaoAliquotaMun { get; set; }

    public decimal PercentualAliquotaEfetivaMun { get; set; }
}

public sealed class IBSCBSValoresFed : GenericClone<IBSCBSValoresFed>
{
    public decimal PercentualCBS { get; set; }

    public decimal PercentualReducaoAliquotaCBS { get; set; }

    public decimal PercentualAliquotaEfetivaCBS { get; set; }
}

public sealed class IBSCBSTotalCIBS : GenericClone<IBSCBSTotalCIBS>
{
    public IBSCBSTotalCIBS()
    {
        IBS = new IBSCBSTotalIBS();
        CBS = new IBSCBSTotalCBS();
    }

    public decimal ValorTotalNF { get; set; }

    public IBSCBSTotalIBS IBS { get; set; }

    public IBSCBSTotalCBS CBS { get; set; }

    public IBSCBSTotalTribRegular? TributacaoRegular { get; set; }

    public IBSCBSTotalTribCompraGov? TributacaoCompraGov { get; set; }
}

public sealed class IBSCBSTotalIBS : GenericClone<IBSCBSTotalIBS>
{
    public IBSCBSTotalIBS()
    {
        TotalIBSUF = new IBSCBSTotalIBSUF();
        TotalIBSMun = new IBSCBSTotalIBSMun();
    }

    public decimal ValorIBSTotal { get; set; }

    public IBSCBSTotalIBSCredPres? CreditoPresumido { get; set; }

    public IBSCBSTotalIBSUF TotalIBSUF { get; set; }

    public IBSCBSTotalIBSMun TotalIBSMun { get; set; }
}

public sealed class IBSCBSTotalIBSCredPres : GenericClone<IBSCBSTotalIBSCredPres>
{
    public decimal PercentualCreditoPresumido { get; set; }

    public decimal ValorCreditoPresumido { get; set; }
}

public sealed class IBSCBSTotalIBSUF : GenericClone<IBSCBSTotalIBSUF>
{
    public decimal ValorDiferimento { get; set; }

    public decimal ValorIBSUF { get; set; }
}

public sealed class IBSCBSTotalIBSMun : GenericClone<IBSCBSTotalIBSMun>
{
    public decimal ValorDiferimento { get; set; }

    public decimal ValorIBSMun { get; set; }
}

public sealed class IBSCBSTotalCBS : GenericClone<IBSCBSTotalCBS>
{
    public decimal ValorDiferimento { get; set; }

    public decimal ValorCBS { get; set; }

    public IBSCBSTotalCBSCredPres? CreditoPresumido { get; set; }
}

public sealed class IBSCBSTotalCBSCredPres : GenericClone<IBSCBSTotalCBSCredPres>
{
    public decimal PercentualCreditoPresumido { get; set; }

    public decimal ValorCreditoPresumido { get; set; }
}

public sealed class IBSCBSTotalTribRegular : GenericClone<IBSCBSTotalTribRegular>
{
    public decimal PercentualAliquotaEfetivaRegIBSUF { get; set; }

    public decimal ValorTributacaoRegIBSUF { get; set; }

    public decimal PercentualAliquotaEfetivaRegIBSMun { get; set; }

    public decimal ValorTributacaoRegIBSMun { get; set; }

    public decimal PercentualAliquotaEfetivaRegCBS { get; set; }

    public decimal ValorTributacaoRegCBS { get; set; }
}

public sealed class IBSCBSTotalTribCompraGov : GenericClone<IBSCBSTotalTribCompraGov>
{
    public decimal PercentualIBSUF { get; set; }

    public decimal ValorIBSUF { get; set; }

    public decimal PercentualIBSMun { get; set; }

    public decimal ValorIBSMun { get; set; }

    public decimal PercentualCBS { get; set; }

    public decimal ValorCBS { get; set; }
}
