using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace GenerateRefitClientFromOpenApi
{
    public class CodeGenerator
    {
        private readonly string _namespaceName;

        public CodeGenerator(string namespaceName)
        {
            _namespaceName = namespaceName;
        }

        public string GenerateDto(string dtoName, IEnumerable<PropertyDefinition> dtoProperties)
        {
            var root = CompilationUnit()
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        ClassDeclaration("Hoi")
                        .WithModifiers(
                            TokenList(
                                Token(SyntaxKind.PublicKeyword)))));

            return root
                .NormalizeWhitespace()
                .ToFullString();
        }
    }
}