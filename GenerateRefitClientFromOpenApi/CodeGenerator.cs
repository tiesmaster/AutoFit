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
                        NamespaceDeclaration((NameSyntax)ParseTypeName(_namespaceName))
                        .WithMembers(
                            SingletonList<MemberDeclarationSyntax>(
                                ClassDeclaration(dtoName)
                                .WithModifiers(
                                    TokenList(
                                        Token(SyntaxKind.PublicKeyword)))
                                .WithMembers(
                                    List<MemberDeclarationSyntax>(
                                        new MemberDeclarationSyntax[]{
                                            PropertyDeclaration(
                                                PredefinedType(
                                                    Token(SyntaxKind.StringKeyword)),
                                                Identifier("Name"))
                                            .WithModifiers(
                                                TokenList(
                                                    Token(SyntaxKind.PublicKeyword)))
                                            .WithAccessorList(
                                                AccessorList(
                                                    List<AccessorDeclarationSyntax>(
                                                        new AccessorDeclarationSyntax[]{
                                                            AccessorDeclaration(
                                                                SyntaxKind.GetAccessorDeclaration)
                                                            .WithSemicolonToken(
                                                                Token(SyntaxKind.SemicolonToken)),
                                                            AccessorDeclaration(
                                                                SyntaxKind.SetAccessorDeclaration)
                                                            .WithSemicolonToken(
                                                                Token(SyntaxKind.SemicolonToken))}))),
                                            PropertyDeclaration(
                                                PredefinedType(
                                                    Token(SyntaxKind.StringKeyword)),
                                                Identifier("AccountNumber"))
                                            .WithModifiers(
                                                TokenList(
                                                    Token(SyntaxKind.PublicKeyword)))
                                            .WithAccessorList(
                                                AccessorList(
                                                    List<AccessorDeclarationSyntax>(
                                                        new AccessorDeclarationSyntax[]{
                                                            AccessorDeclaration(
                                                                SyntaxKind.GetAccessorDeclaration)
                                                            .WithSemicolonToken(
                                                                Token(SyntaxKind.SemicolonToken)),
                                                            AccessorDeclaration(
                                                                SyntaxKind.SetAccessorDeclaration)
                                                            .WithSemicolonToken(
                                                                Token(SyntaxKind.SemicolonToken))})))}))))));

            return root
                .NormalizeWhitespace()
                .ToFullString();
        }
    }
}