using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AutoFit
{
    public class DtoGenerator
    {
        private readonly string _namespaceName;
        private readonly string _outputPath;

        public DtoGenerator(string namespaceName, string outputPath)
        {
            _namespaceName = namespaceName;
            _outputPath = outputPath;
        }

        public void EmitDto(DtoDefinition definition)
        {
            var workspace = new AdhocWorkspace();
            var root = GenerateDtoNode(definition);

            root = Formatter.Format(root, workspace);

            var source = root.ToFullString();
            var outputFile = Path.Combine(_outputPath, $"{definition.Name}.cs");

            File.WriteAllText(outputFile, source);
        }

        private SyntaxNode GenerateDtoNode(DtoDefinition definition)
        {
            var classDeclaration = ClassDeclaration(definition.Name)
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithMembers(List(definition.Properties.Select(GeneratePropertyFromDtoDefinition)));

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