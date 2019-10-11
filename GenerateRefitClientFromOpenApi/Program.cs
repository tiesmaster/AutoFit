using System.IO;
using System.Linq;
using System.Text.Json;

namespace GenerateRefitClientFromOpenApi
{
    class Program
    {
        static void Main(string[] args)
        {
            using var document = JsonDocument.Parse(File.OpenRead("rdc-openapi.json"));
            var dtoDefinitions = document.RootElement.GetProperty("definitions");

            GenerateDto(
                @"C:\src\buddy\rabodirectconnect-api\RaboDirectConnectApi.Dto\BeneficiaryDto.cs",
                "Buddy.RaboDirectConnectApi.Dto",
                "BeneficiaryDto",
                dtoDefinitions);
        }

        private static void GenerateDto(string fullPath, string namespaceName, string dtoName, JsonElement dtoDefinitions)
        {
            var dtoDefinition = dtoDefinitions.GetProperty(dtoName);

            var propertyDefinitions = dtoDefinition.GetProperty("properties");
            var dtoProperties = propertyDefinitions.EnumerateObject().Select(ToPropertyDefinition);

            var source = new CodeGenerator(namespaceName).GenerateDto(dtoName, dtoProperties);
            File.WriteAllText(fullPath, source);
        }

        private static PropertyDefinition ToPropertyDefinition(JsonProperty propertyDefinition)
        {
            return new PropertyDefinition
            {
                Type = propertyDefinition.Value.GetProperty("type").ToString(),
                Identifier = propertyDefinition.Name
            };
        }
    }
}