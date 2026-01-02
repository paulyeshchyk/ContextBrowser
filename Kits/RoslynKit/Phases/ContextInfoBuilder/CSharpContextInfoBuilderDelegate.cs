using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Phases.ContextInfoBuilder.SyntaxNodeWrapper;
using SemanticKit.Model;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class CSharpContextInfoBuilderDelegate<TContext> : BaseContextInfoBuilder<TContext, DelegateDeclarationSyntax, ISemanticModelWrapper, CSharpSyntaxNodeWrapperDelegate>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpContextInfoBuilderDelegate(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IAppLogger<AppLevel> logger)
        : base(collector, factory, logger)
    {
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@delegate;

    public override bool CanBuild(ISyntaxWrapper contextInfo) => false;

}
