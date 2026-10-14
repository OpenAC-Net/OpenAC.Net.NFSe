using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OpenAC.Net.NFSe.Commom.Model;

/// <summary>
/// Encapsula um valor de texto XML para ser serializado dentro de um bloco CDATA (&lt;![CDATA[ ... ]]&gt;).
/// </summary>
[XmlSchemaProvider("GenerateSchema")]
public sealed class XmlCData : IXmlSerializable
{
    #region Fields

    private string? value;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Obtém ou define o conteúdo textual do bloco CDATA (sem cabeçalhos XML).
    /// </summary>
    public string? Value
    {
        get => value.RemoverDeclaracaoXml();
        set => this.value = value;
    }

    #endregion Properties

    #region Methods

    /// <inheritdoc />
    public XmlSchema? GetSchema()
    {
        return null;
    }

    /// <summary>
    /// Gera o esquema XML compatível para o tipo string.
    /// </summary>
    /// <param name="xs">Conjunto de esquemas XML.</param>
    /// <returns>Nome qualificado do esquema XML.</returns>
    public static XmlQualifiedName GenerateSchema(XmlSchemaSet xs)
    {
        return XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).QualifiedName;
    }

    /// <inheritdoc />
    public void WriteXml(XmlWriter writer)
    {
        if (string.IsNullOrEmpty(Value)) return;

        if (Value != null && Value.Contains("") && !Value.Contains("]]>"))
        {
            writer.WriteCData(Value);
        }
        else
        {
            writer.WriteString(Value ?? string.Empty);
        }
    }

    /// <inheritdoc />
    public void ReadXml(XmlReader reader)
    {
        if (reader.IsEmptyElement)
        {
            Value = "";
        }
        else
        {
            reader.Read();

            Value = reader.NodeType switch
            {
                XmlNodeType.EndElement => "", // empty after all...
                XmlNodeType.Text or XmlNodeType.CDATA => reader.ReadContentAsString(),
                _ => throw new InvalidOperationException("Expected text/cdata")
            };
        }
    }

    /// <inheritdoc />
    public override string ToString() => Value ?? string.Empty;

    #endregion Methods

    #region Operators

    /// <summary>
    /// Converte implicitamente uma instância de <see cref="XmlCData"/> para <see cref="string"/>.
    /// </summary>
    /// <param name="value">Instância de XmlCData.</param>
    /// <returns>Valor em formato string.</returns>
    public static implicit operator string?(XmlCData? value)
    {
        return value?.Value;
    }

    /// <summary>
    /// Converte implicitamente uma <see cref="string"/> para uma instância de <see cref="XmlCData"/>.
    /// </summary>
    /// <param name="value">Valor em string.</param>
    /// <returns>Nova instância de XmlCData.</returns>
    public static implicit operator XmlCData?(string? value)
    {
        return value == null ? null : new XmlCData { Value = value };
    }

    #endregion Operators
}