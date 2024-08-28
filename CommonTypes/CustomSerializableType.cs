using System.Globalization;
using System.Numerics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonTypes;

public struct CustomSerializableType : IXmlSerializable
{
    public BigInteger TestValue { get; set; }

    #region Implementation of IXmlSerializable
    
    public XmlSchema? GetSchema()
    {
        // Create a new schema
        var schema = new XmlSchema
        {
            Id = "CustomSerializableTypeSchema"
        };
        // Define the simple type
        var simpleType = new XmlSchemaSimpleType
        {
            Name = "BigIntegerString"
        };
        // Define the restriction for the simple type
        var restriction = new XmlSchemaSimpleTypeRestriction
        {
            BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
        };
        // Set the restriction as the content of the simple type
        simpleType.Content = restriction;
        // Add the simple type to the schema
        schema.Items.Add(simpleType);
        return schema;
    }

    public void ReadXml(XmlReader reader)
    {
        reader.ReadStartElement();
        reader.ReadStartElement();
        var elementValue = reader.ReadContentAsString();
        TestValue = BigInteger.Parse(elementValue);
        reader.ReadEndElement();
        reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("BigIntegerString");
        writer.WriteValue(TestValue.ToString(CultureInfo.InvariantCulture));
        writer.WriteEndElement();
    }

    #endregion
}