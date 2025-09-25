using System;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class CSharpDelegateContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, DelegateDeclarationSyntax, ISemanticModelWrapper, CSharpDelegateSyntaxNodeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpDelegateContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IAppLogger<AppLevel> logger)
        : base(collector, factory, logger)
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