using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class CSharpPropertyContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, PropertyDeclarationSyntax, ISemanticModelWrapper, CSharpPropertySyntaxNodeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpPropertyContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IAppLogger<AppLevel> logger)
        : base(collector, factory, logger)
    {
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@property;
}

public class CSharpPropertySyntaxNodeWrapper : CSharpSyntaxNodeWrapper<PropertyDeclarationSyntax>, ISymbolInfo
{
    private PropertyDeclarationSyntax _syntaxNode => GetCoSyntax<PropertyDeclarationSyntax>();

    public override string Identifier => _syntaxNode.GetIdentifier();

    public override string Namespace => _syntaxNode.GetNamespaceOrGlobal();

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetName() => _syntaxNode.GetIdentifier();

#warning this is incorrect
    public override string GetShortName() => GetName();
}