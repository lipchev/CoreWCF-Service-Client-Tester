# CoreWCF-Service-Client-Tester
This is an attempt to demonstrate an issue with the `svcutil` (using the latest dev build version: `8.0.0-dev.24427.1`), where the `"typeReuseMode": "All"` option no longer works when one of the `[DataMember]`s of a `[DataContract]` is itself not a `[DataContract]` but instead implements the `IXmlSerializable` interface. 

The solution contains the following projects:
1. `CommonTypes` - this contians the "shared" data contracts (including the problematic `CustomSerializableType` contract). It is currently multi-targeting `net472` and `net8.0` but that's not a problem here.
2. `SerializationTests` - a simple test that demonstrates that the `DataContractSerializer` is capable of dealing with all of our CommonTypes
3. `CoreWCF-Service` - a .NET8 service hosting a vanilla IService returning the contracts from `CommonTypes`.
4. `NetFramework-Client` - a simple `net472` client referencing the `CommonTypes` which demonstrates that the scenario used to work before.
5. `CoreWCF-Service` - a simple `net8.0` client referencing the `CommonTypes` which demonstrates that the scenario doesn't work any more (using the latest build `svcutil tool`: `8.0.0-dev.24427.1`, but also tested with the older versions: `2.1.0` or `2.2.0-preview1.23462.5`)

The following json file
``` 
{
  "ExtendedData": {
    "inputs": [
      "https://localhost:7246/Service.svc"
    ],
    "collectionTypes": [
      "System.Array",
      "System.Collections.Generic.Dictionary`2"
    ],
    "namespaceMappings": [
      "*, ServiceReference1"
    ],
    "targetFramework": "net8.0",
    "typeReuseMode": "All"
  }
}

```
is passed to the svcutil using:
`dotnet svcutil -u "Connected Services/ServiceReference1" -v Debug`.

Setting the `"serializer": "DataContractSerializer"` option _should_ work, but _doesn't_ (the generation fails).

Here is the relative parts of the WSDL:
```
<xs:schema elementFormDefault="qualified" targetNamespace="http://schemas.datacontract.org/2004/07/CommonTypes">
<xs:import schemaLocation="https://localhost:7246/Service.svc?xsd=xsd1" namespace="http://schemas.microsoft.com/2003/10/Serialization/"/>
<xs:complexType name="SomeSharedType">
<xs:annotation>
<xs:appinfo>
<IsValueType>true</IsValueType>
</xs:appinfo>
</xs:annotation>
<xs:sequence>
<xs:element minOccurs="0" name="SomeProperty" type="xs:int"/>
</xs:sequence>
</xs:complexType>
<xs:element name="SomeSharedType" nillable="true" type="tns:SomeSharedType"/>
<xs:complexType name="AnotherSharedType">
<xs:annotation>
<xs:appinfo>
<IsValueType>true</IsValueType>
</xs:appinfo>
</xs:annotation>
<xs:sequence>
<xs:element minOccurs="0" name="SomeProperty" type="xs:int"/>
<xs:element minOccurs="0" name="SomeXmlSerializableType" type="tns:CustomSerializableType"/>
</xs:sequence>
</xs:complexType>
<xs:element name="AnotherSharedType" nillable="true" type="tns:AnotherSharedType"/>
<xs:complexType name="CustomSerializableType">
<xs:annotation>
<xs:appinfo>
<IsValueType>true</IsValueType>
</xs:appinfo>
</xs:annotation>
<xs:sequence>
<xs:any namespace=""/>
</xs:sequence>
</xs:complexType>
<xs:element name="CustomSerializableType" nillable="true" type="tns:CustomSerializableType"/>
</xs:schema>

```

And here is the "problematic" type schema:
```
<xs:schema id="CustomSerializableTypeSchema">
<xs:simpleType name="BigIntegerString">
<xs:restriction base="xs:string"/>
</xs:simpleType>
</xs:schema>
```

The "xml" returned by this operation contract
```
    [OperationContract]
    AnotherSharedType GetComplexValue();
```
should contain something like this:
```
<AnotherSharedType xmlns="http://schemas.datacontract.org/2004/07/CommonTypes" xmlns:i="http://www.w3.org/2001/XMLSchema-instance"><SomeProperty>42</SomeProperty><SomeXmlSerializableType><BigIntegerString>42</BigIntegerString></SomeXmlSerializableType></AnotherSharedType>
```

Note: if we removed the above operation from the interface, the type-reuse works once again (at least for the remaining operation, which returns the simple contract)..
