using System;
using System.Linq;
using System.Text.Json;

namespace AutoFit
{
    public static class OpenApiParser
    {
        public static DtoDefinition ParseDefinition(JsonProperty jsonDefinition)
        {
            var dtoProperties = jsonDefinition
                .Value
                .GetProperty("properties")
                .EnumerateObject()
                .Select(ToPropertyDefinition);

            var dtoDefinition = new DtoDefinition
            {
                Name = jsonDefinition.Name,
                Properties = dtoProperties
            };

            return dtoDefinition;
        }

        private static PropertyDefinition ToPropertyDefinition(JsonProperty propertyDefinition)
        {
            return new PropertyDefinition
            {
                TypeDefinition = ParseDtoType(propertyDefinition.Value),
                IdentifierName = propertyDefinition.Name.Capitalize()
            };
        }

        private static TypeDefinition ParseDtoType(JsonElement propertyDefinitionElement)
        {
            var type = propertyDefinitionElement.TryGetPropertyOrDefault("type");
            var format = propertyDefinitionElement.TryGetPropertyOrDefault("format");

            return type switch
            {
                "number" => propertyDefinitionElement.GetProperty("format").ToString(),
                "string" when format != null => ParseFormat(format),
                null => ParseReferenceToOtherDefinition(propertyDefinitionElement),
                _ => type,
            };
        }

        private static string ParseFormat(string format)
        {
            return format switch
            {
                "date-time" => "DateTime",
                _ => throw new NotSupportedException()
            };
        }

        private static string ParseReferenceToOtherDefinition(JsonElement propertyDefinitionElement)
        {
            var reference = propertyDefinitionElement.GetProperty("$ref").ToString();
            return reference.Substring("#/definitions/".Length);
        }
    }
}