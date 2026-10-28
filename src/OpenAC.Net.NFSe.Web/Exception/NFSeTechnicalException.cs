namespace OpenAC.Net.NFSe.Web.Exception;

/// <summary>Representa uma falha técnica de transporte ou infraestrutura, distinta de uma rejeição fiscal.</summary>
public sealed class NFSeTechnicalException : System.Exception
{
    /// <summary>Operação que falhou.</summary>
    public string Operacao { get; }

    /// <summary>Cria uma falha técnica para a operação informada.</summary>
    /// <param name="operacao">Nome da operação do cliente que falhou.</param>
    /// <param name="mensagem">Mensagem que descreve a falha técnica.</param>
    /// <param name="excecaoInterna">Exceção original produzida pelo transporte ou pipeline de resiliência.</param>
    public NFSeTechnicalException(string operacao, string mensagem, System.Exception excecaoInterna)
        : base(mensagem, excecaoInterna) => Operacao = operacao;
}
