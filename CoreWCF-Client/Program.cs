namespace CoreWCF_Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Contacting service..");
            var client = new ServiceReference1.ServiceClient();
            
            var simpleType = await client.GetSimpleValueAsync(); // even the simple type is not "re-used" (from CommonTypes.SomeSharedType) when the "GetComplexValue" operation is present
            Console.WriteLine($"simpleType is {simpleType.GetType()}, SomeProperty = {simpleType.SomeProperty}");
            
            var complexType = await client.GetComplexValueAsync(); // the presence of this operation breaks ALL type-reuse (from CommonTypes)
            // Console.WriteLine($"complexType is {complexType.GetType()}, SomeProperty = {complexType.SomeProperty}, AnotherProperty = {complexType.SomeXmlSerializableType.TestValue}"); // the property does not exist
            Console.WriteLine($"complexType is {complexType.GetType()}, SomeProperty = {complexType.SomeProperty}, AnotherProperty = {complexType.SomeXmlSerializableType.GetType()}");

            await client.CloseAsync();
            Console.ReadLine();
        }
    }
}
