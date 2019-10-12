﻿using System;
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
            var type = TryGetPropertyOrDefault(propertyDefinitionElement, "type");
            var format = TryGetPropertyOrDefault(propertyDefinitionElement, "format");

            return type switch
            {
                "number" => propertyDefinitionElement.GetProperty("format").ToString(),
                "string" when format != null => ParseFormat(format),
                null => ParseReferenceToOtherDefinition(propertyDefinitionElement),
                _ => type,
            };
        }

        private static string ParseReferenceToOtherDefinition(JsonElement propertyDefinitionElement)
        {
            var reference = propertyDefinitionElement.GetProperty("$ref").ToString();
            return reference.Substring("#/definitions/".Length);
        }

        private static string TryGetPropertyOrDefault(JsonElement propertyDefinitionElement, string jsonPropertyName)
        {
            return propertyDefinitionElement.TryGetProperty(jsonPropertyName, out var format)
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

        private static string Capitalize(string name)
        {
            return name.Length < 2 ? name : char.ToUpper(name[0]) + name.Substring(1);
        }
    }
}