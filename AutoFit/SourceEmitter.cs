using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AutoFit
{
    public class SourceEmitter
    {
        private readonly string _namespaceName;
        private readonly string _outputPath;

        private readonly AdhocWorkspace _workspace;

        public SourceEmitter(string namespaceName, string outputPath)
        {
            _namespaceName = namespaceName;
            _outputPath = outputPath;

            _workspace = new AdhocWorkspace();
        }

        public void EmitDto(DtoDefinition definition)
        {
            var root = GenerateDtoNode(definition);
            EmitSource(root, $"{definition.Name}.cs");
        }

        private void EmitSource(SyntaxNode root, string sourceFileName)
        {
            root = Formatter.Format(root, _workspace);

            var source = root.ToFullString();
            var outputFile = Path.Combine(_outputPath, sourceFileName);

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
                .WithUsings(List(definition.RequestedNamespaces.Select(ToUsingDirective)))
                .WithMembers(SingletonList<MemberDeclarationSyntax>(namespaceDeclaration));
        }

        private UsingDirectiveSyntax ToUsingDirective(string namespaceName)
        {
            return UsingDirective(ParseName(namespaceName));
        }

        private MemberDeclarationSyntax GeneratePropertyFromDtoDefinition(PropertyDefinition dtoDefinition)
        {
            static AccessorDeclarationSyntax GetOrSetNode(SyntaxKind getOrSetSyntaxKind)
                => AccessorDeclaration(getOrSetSyntaxKind).WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            return PropertyDeclaration(ParseTypeName(dtoDefinition.TypeDefinition.Name), Identifier(dtoDefinition.IdentifierName))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(AccessorList(List(new AccessorDeclarationSyntax[]
                {
                    GetOrSetNode(SyntaxKind.GetAccessorDeclaration),
                    GetOrSetNode(SyntaxKind.SetAccessorDeclaration)
                })));
        }
    }
}