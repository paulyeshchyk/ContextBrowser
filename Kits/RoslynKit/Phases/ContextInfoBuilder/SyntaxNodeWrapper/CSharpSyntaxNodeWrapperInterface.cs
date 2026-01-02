using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;

namespace RoslynKit.Phases.ContextInfoBuilder.SyntaxNodeWrapper;

public class CSharpSyntaxNodeWrapperInterface : CSharpSyntaxNodeWrapper<InterfaceDeclarationSyntax>, ISymbolInfo
{
    private InterfaceDeclarationSyntax _syntaxNode => GetCoSyntax<InterfaceDeclarationSyntax>();

    public override string Identifier => _syntaxNode.GetIdentifier();

    public override string Namespace => _syntaxNode.GetNamespaceOrGlobal();

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetName() => _syntaxNode.GetIdentifier();

#warning this is incorrect
    public override string GetShortName() => GetName();
}