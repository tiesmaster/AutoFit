using System.IO;
using System.Linq;
using System.Text.Json;

namespace AutoFit
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var codeGenerator = new CodeGenerator(
                namespaceName: "Buddy.RaboDirectConnectApi.Dto",
                outputPath: @"C:\src\buddy\rabodirectconnect-api\RaboDirectConnectApi.Dto");

            using var document = JsonDocument.Parse(File.OpenRead("rdc-openapi.json"));
            var dtoDefinitions = document.RootElement.GetProperty("definitions");

            GenerateDto(codeGenerator, "BeneficiaryDto", dtoDefinitions);
        }

        private static void GenerateDto(CodeGenerator codeGenerator, string dtoName, JsonElement dtoDefinitions)
        {
            var dtoDefinition = dtoDefinitions.GetProperty(dtoName);

            var propertyDefinitions = dtoDefinition.GetProperty("properties");
            var dtoProperties = propertyDefinitions.EnumerateObject().Select(ToPropertyDefinition);

            codeGenerator.GenerateDto(dtoName, dtoProperties);
        }

        private static PropertyDefinition ToPropertyDefinition(JsonProperty propertyDefinition)
        {
            return new PropertyDefinition
            {
                TypeName = propertyDefinition.Value.GetProperty("type").ToString(),
                IdentifierName = Capitalize(propertyDefinition.Name)
            };
        }

        // TODO: move this elsewhere
        private static string Capitalize(string name)
        {
            return name.Length < 2 ? name : char.ToUpper(name[0]) + name.Substring(1);
        }
    }
}