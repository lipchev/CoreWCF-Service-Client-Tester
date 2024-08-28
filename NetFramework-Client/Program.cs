using System;
using System.Threading.Tasks;
using NetFramework_Client.ServiceReference1;

namespace NetFramework_Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Contacting service..");
            var client = new ServiceClient();
            
            var simpleType = await client.GetSimpleValueAsync();
            Console.WriteLine($"simpleType is {simpleType.GetType()}, SomeProperty = {simpleType.SomeProperty}");
            
            var complexType = await client.GetComplexValueAsync();
            Console.WriteLine($"complexType is {complexType.GetType()}, SomeProperty = {complexType.SomeProperty}, AnotherProperty = {complexType.SomeXmlSerializableType.TestValue}");
            
            client.Close();
            Console.ReadLine();
        }
    }
}