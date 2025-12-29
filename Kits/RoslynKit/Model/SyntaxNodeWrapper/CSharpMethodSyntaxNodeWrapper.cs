using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;

namespace RoslynKit.Model.SyntaxNodeWrapper;

public class CSharpMethodSyntaxNodeWrapper : CSharpSyntaxNodeWrapper<MethodDeclarationSyntax>, ISymbolInfo
{
    private MethodDeclarationSyntax _syntaxNode => GetCoSyntax<MethodDeclarationSyntax>();

    public override string Identifier => _syntaxNode.GetIdentifier();

    public override string Namespace => _syntaxNode.GetNamespaceOrGlobal();

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetName() => _syntaxNode.GetIdentifier();

    public override string GetShortName() => GetName();
}