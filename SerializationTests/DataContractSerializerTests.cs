using System.Runtime.Serialization;
using CommonTypes;

namespace SerializationTests;

public class DataContractSerializerTests
{
    [Fact]
    public void SerializationRoundTrips()
    {
        // note that the CustomSerializableType doesn't have [DataContract] annotation, but instead implements IXmlSerializable
        var testObject = new AnotherSharedType { SomeProperty = 42, SomeXmlSerializableType = new CustomSerializableType { TestValue = 42 } };

        var serializedObject = SerializeObject(testObject);
        var deserializedObject = DeserializeObject<AnotherSharedType>(serializedObject);

        Assert.Equal(testObject.SomeProperty, deserializedObject.SomeProperty);
        Assert.Equal(testObject.SomeXmlSerializableType.TestValue, deserializedObject.SomeXmlSerializableType.TestValue);
    }

    private string SerializeObject(object obj)
    {
        var serializer = new DataContractSerializer(obj.GetType());
        using var stream = new MemoryStream();
        serializer.WriteObject(stream, obj);
        stream.Position = 0;
        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }

    private T DeserializeObject<T>(string xml)
    {
        var serializer = new DataContractSerializer(typeof(T));
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        writer.Write(xml);
        writer.Flush();
        stream.Position = 0;
        return (T)(serializer.ReadObject(stream) ?? throw new InvalidOperationException("Read 'null' from stream."));
    }
}