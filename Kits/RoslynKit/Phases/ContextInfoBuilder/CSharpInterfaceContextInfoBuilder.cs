using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class CSharpInterfaceContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, InterfaceDeclarationSyntax, ISemanticModelWrapper, CSharpInterfaceSyntaxNodeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpInterfaceContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IAppLogger<AppLevel> logger)
        : base(collector, factory, logger)
    {
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@interface;
}
