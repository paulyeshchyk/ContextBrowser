using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;

namespace RoslynKit.Phases.ContextInfoBuilder.SyntaxNodeWrapper;

// context: syntax, model, roslyn, symbol
public class CSharpSyntaxNodeWrapperMethod : CSharpSyntaxNodeWrapper<MethodDeclarationSyntax>, ISymbolInfo
{
    private MethodDeclarationSyntax SyntaxNode => GetCoSyntax<MethodDeclarationSyntax>();

    public override string Identifier => SyntaxNode.GetIdentifier();

    public override string Namespace => SyntaxNode.GetNamespaceOrGlobal();

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetName() => SyntaxNode.GetIdentifier();

    public override string GetShortName() => GetName();
}