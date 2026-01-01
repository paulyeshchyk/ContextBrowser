using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model.SyntaxNodeWrapper;
using SemanticKit.Model;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class CSharpContextInfoBuilderProperty<TContext> : BaseContextInfoBuilder<TContext, PropertyDeclarationSyntax, ISemanticModelWrapper, CSharpPropertySyntaxNodeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpContextInfoBuilderProperty(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IAppLogger<AppLevel> logger)
        : base(collector, factory, logger)
    {
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@property;
    public override bool CanBuild(ISyntaxWrapper contextInfo) => false;
}
