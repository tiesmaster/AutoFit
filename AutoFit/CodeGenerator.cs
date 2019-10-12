using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AutoFit
{
    public class CodeGenerator
    {
        private readonly string _inputFile;
        private readonly string _outputPath;

        private readonly string _namespaceName;

        public CodeGenerator(string inputFile, string outputPath, string namespaceName)
        {
            _inputFile = inputFile;
            _outputPath = outputPath;
            _namespaceName = namespaceName;
        }

        public void Emit()
        {
            using var document = JsonDocument.Parse(File.OpenRead(_inputFile));
            var dtoDefinitions = document.RootElement.GetProperty("definitions");

            GenerateDto("BeneficiaryDto", dtoDefinitions);
            GenerateDto("TransactionDto", dtoDefinitions);
        }

        private void GenerateDto(string dtoName, JsonElement dtoDefinitions)
        {
            var dtoDefinition = dtoDefinitions.GetProperty(dtoName);

            var propertyDefinitions = dtoDefinition.GetProperty("properties");
            var dtoProperties = propertyDefinitions.EnumerateObject().Select(ToPropertyDefinition);

            GenerateDto(dtoName, dtoProperties);
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

        public void GenerateDto(string dtoName, IEnumerable<PropertyDefinition> dtoProperties)
        {
            var workspace = new AdhocWorkspace();
            var root = GenerateCore(dtoName, dtoProperties);

            root = Formatter.Format(root, workspace);

            var source = root.ToFullString();
            var outputFile = Path.Combine(_outputPath, $"{dtoName}.cs");

            File.WriteAllText(outputFile, source);
        }

        private SyntaxNode GenerateCore(string dtoName, IEnumerable<PropertyDefinition> dtoProperties)
        {
            var classDeclaration = ClassDeclaration(dtoName)
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithMembers(List(dtoProperties.Select(GeneratePropertyFromDtoDefinition)));

            var namespaceDeclaration = NamespaceDeclaration((NameSyntax)ParseTypeName(_namespaceName))
                .WithMembers(SingletonList<MemberDeclarationSyntax>(classDeclaration));

            return CompilationUnit()
                .WithUsings(SingletonList(UsingDirective(IdentifierName("System"))))
                .WithMembers(SingletonList<MemberDeclarationSyntax>(namespaceDeclaration));
        }

        private MemberDeclarationSyntax GeneratePropertyFromDtoDefinition(PropertyDefinition dtoDefinition)
        {
            static AccessorDeclarationSyntax GetOrSetNode(SyntaxKind getOrSetSyntaxKind)
                => AccessorDeclaration(getOrSetSyntaxKind).WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            return PropertyDeclaration(ParseTypeName(dtoDefinition.TypeName), Identifier(dtoDefinition.IdentifierName))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(AccessorList(List(new AccessorDeclarationSyntax[]
                {
                    GetOrSetNode(SyntaxKind.GetAccessorDeclaration),
                    GetOrSetNode(SyntaxKind.SetAccessorDeclaration)
                })));
        }
    }
}