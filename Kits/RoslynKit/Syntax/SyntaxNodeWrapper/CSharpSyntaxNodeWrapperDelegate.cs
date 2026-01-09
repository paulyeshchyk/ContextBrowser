using System;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Syntax;

namespace RoslynKit.Phases.ContextInfoBuilder.SyntaxNodeWrapper;

// context: syntax, model, roslyn, symbol
public class CSharpSyntaxNodeWrapperDelegate : CSharpSyntaxNodeWrapper<DelegateDeclarationSyntax>, ISymbolInfo
{
    private DelegateDeclarationSyntax SyntaxNode => GetCoSyntax<DelegateDeclarationSyntax>();

    public override string Identifier => SyntaxNode.GetIdentifier() ?? throw new ArgumentException(nameof(SyntaxNode));

    public override string Namespace => SyntaxNode.GetNamespaceOrGlobal() ?? throw new ArgumentException(nameof(SyntaxNode));

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetName() => SyntaxNode.GetIdentifier() ?? throw new ArgumentException(nameof(SyntaxNode));

    public override string GetShortName() => GetName();
}