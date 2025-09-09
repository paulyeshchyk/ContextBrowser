using System;
using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Wrappers;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class CSharpDelegateContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, DelegateDeclarationSyntax, ISemanticModelWrapper, CSharpDelegateSyntaxNodeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpDelegateContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@delegate;
}

public class CSharpDelegateSyntaxNodeWrapper : CSharpSyntaxNodeWrapper<DelegateDeclarationSyntax>, ISymbolInfo
{
    private DelegateDeclarationSyntax? _syntaxNode => GetCoSyntax<DelegateDeclarationSyntax>();

    public override string Identifier => _syntaxNode?.GetIdentifier() ?? throw new ArgumentException(nameof(_syntaxNode));

    public override string Namespace => _syntaxNode?.GetNamespaceOrGlobal() ?? throw new ArgumentException(nameof(_syntaxNode));

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetName() => _syntaxNode?.GetIdentifier() ?? throw new ArgumentException(nameof(_syntaxNode));

    public override string GetShortName() => GetName();
}