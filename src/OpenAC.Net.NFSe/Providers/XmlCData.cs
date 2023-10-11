using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OpenAC.Net.NFSe.Providers;

/// <inheritdoc />
[XmlSchemaProvider("GenerateSchema")]
public sealed class XmlCData : IXmlSerializable
{
    #region Fields

    private string value;

    #endregion Fields

    #region Properties

    /// <summary>
    ///
    /// </summary>
    public string Value
    {
        get => value.RemoverDeclaracaoXml();
        set => this.value = value;
    }

    #endregion Properties

    #region Methods

    /// <inheritdoc />
    public XmlSchema GetSchema()
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

        if (Value.Contains("") && !Value.Contains("]]>"))
        {
            writer.WriteCData(Value);
        }
        else
        {
            writer.WriteString(Value);
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

            switch (reader.NodeType)
            {
                case XmlNodeType.EndElement:
                    Value = ""; // empty after all...
                    break;

                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                    Value = reader.ReadContentAsString();
                    break;

                default:
                    throw new InvalidOperationException("Expected text/cdata");
            }
        }
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    #endregion Methods

    #region Operators

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator string(XmlCData value)
    {
        return value?.Value;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator XmlCData(string value)
    {
        return value == null ? null : new XmlCData { Value = value };
    }

    #endregion Operators
}