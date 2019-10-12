﻿using System.Collections.Generic;
using System.Linq;

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
            var root = GenerateCore(dtoName, dtoProperties);

            return root
                .NormalizeWhitespace()
                .ToFullString();
        }

        private CompilationUnitSyntax GenerateCore(string dtoName, IEnumerable<PropertyDefinition> dtoProperties)
        {
            return CompilationUnit()
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
                                    List(
                                        new MemberDeclarationSyntax[]
                                        {
                                            GeneratePropertyFromDtoDefinition(dtoProperties.ElementAt(0)),
                                            GeneratePropertyFromDtoDefinition(dtoProperties.ElementAt(1))
                                        }))))));
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