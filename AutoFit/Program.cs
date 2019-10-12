using System;
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
            GenerateDto(codeGenerator, "TransactionDto", dtoDefinitions);
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
                TypeName = ParseDtoType(propertyDefinition.Value),
                IdentifierName = Capitalize(propertyDefinition.Name)
            };
        }

        private static string ParseDtoType(JsonElement propertyDefinitionElement)
        {
            var type = propertyDefinitionElement.GetProperty("type").ToString();
            var format = GetFormat(propertyDefinitionElement);

            return type switch
            {
                "number" => propertyDefinitionElement.GetProperty("format").ToString(),
                "string" when format != null => ParseFormat(format),
                _ => type,
            };
        }

        private static string GetFormat(JsonElement propertyDefinitionElement)
        {
            return propertyDefinitionElement.TryGetProperty("format", out var format)
                ? format.ToString()
                : null;
        }

        private static string ParseFormat(string format)
        {
            return format switch
            {
                "date-time" => "DateTime",
                _ => throw new NotSupportedException()
            };
        }

        // TODO: move this elsewhere
        private static string Capitalize(string name)
        {
            return name.Length < 2 ? name : char.ToUpper(name[0]) + name.Substring(1);
        }
    }
}