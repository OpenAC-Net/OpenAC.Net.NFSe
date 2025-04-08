using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OpenAC.Net.NFSe.Commom.Model;

/// <inheritdoc />
[XmlSchemaProvider("GenerateSchema")]
public sealed class XmlCData : IXmlSerializable
{
    #region Fields

    private string? value;

    #endregion Fields

    #region Properties

    /// <summary>
    ///
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
    ///
    /// </summary>
    /// <param name="xs"></param>
    /// <returns></returns>
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
    ///
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator string?(XmlCData? value)
    {
        return value?.Value;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator XmlCData?(string? value)
    {
        return value == null ? null : new XmlCData { Value = value };
    }

    #endregion Operators
}