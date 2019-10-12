using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AutoFit
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

            // TODO: check if we need to do FormatAsync

            var workspace = new AdhocWorkspace();
            root = Formatter.Format(root, workspace);

            return root.ToFullString();
        }

        private SyntaxNode GenerateCore(string dtoName, IEnumerable<PropertyDefinition> dtoProperties)
        {
            var classDeclaration = ClassDeclaration(dtoName)
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithMembers(List(dtoProperties.Select(GeneratePropertyFromDtoDefinition)));

            var namespaceDeclaration = NamespaceDeclaration((NameSyntax)ParseTypeName(_namespaceName))
                .WithMembers(SingletonList<MemberDeclarationSyntax>(classDeclaration));

            return CompilationUnit().WithMembers(SingletonList<MemberDeclarationSyntax>(namespaceDeclaration));
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