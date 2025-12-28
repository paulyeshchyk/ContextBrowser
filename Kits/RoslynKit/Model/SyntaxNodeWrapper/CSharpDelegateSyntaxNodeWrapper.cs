using System;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;

namespace RoslynKit.Model.SyntaxNodeWrapper;

public class CSharpDelegateSyntaxNodeWrapper : CSharpSyntaxNodeWrapper<DelegateDeclarationSyntax>, ISymbolInfo
{
    private DelegateDeclarationSyntax? _syntaxNode => GetCoSyntax<DelegateDeclarationSyntax>();

    public override string Identifier => _syntaxNode?.GetIdentifier() ?? throw new ArgumentException(nameof(_syntaxNode));

    public override string Namespace => _syntaxNode?.GetNamespaceOrGlobal() ?? throw new ArgumentException(nameof(_syntaxNode));

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetName() => _syntaxNode?.GetIdentifier() ?? throw new ArgumentException(nameof(_syntaxNode));

    public override string GetShortName() => GetName();
}