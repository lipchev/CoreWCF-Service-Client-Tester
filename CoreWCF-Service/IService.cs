using System.Numerics;
using CommonTypes;

namespace CoreWCF_Service;

[ServiceContract]
public interface IService
{
    [OperationContract]
    SomeSharedType GetSimpleValue();
    
    [OperationContract]
    AnotherSharedType GetComplexValue();
}

public class Service : IService
{

    public SomeSharedType GetSimpleValue()
    {
        return new SomeSharedType { SomeProperty = 42 };
    }

    public AnotherSharedType GetComplexValue()
    {
        return new AnotherSharedType { SomeProperty = 42, SomeXmlSerializableType = new CustomSerializableType { TestValue = new BigInteger(42) } };
    }
}