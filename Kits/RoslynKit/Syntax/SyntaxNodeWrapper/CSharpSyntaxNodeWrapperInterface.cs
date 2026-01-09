using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Syntax;

namespace RoslynKit.Phases.ContextInfoBuilder.SyntaxNodeWrapper;

// context: syntax, model, roslyn, symbol
public class CSharpSyntaxNodeWrapperInterface : CSharpSyntaxNodeWrapper<InterfaceDeclarationSyntax>, ISymbolInfo
{
    private InterfaceDeclarationSyntax SyntaxNode => GetCoSyntax<InterfaceDeclarationSyntax>();

    public override string Identifier => SyntaxNode.GetIdentifier();

    public override string Namespace => SyntaxNode.GetNamespaceOrGlobal();

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetName() => SyntaxNode.GetIdentifier();

#warning this is incorrect
    public override string GetShortName() => GetName();
}