using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model.SyntaxNodeWrapper;
using SemanticKit.Model;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class CSharpContextInfoBuilderInterface<TContext> : BaseContextInfoBuilder<TContext, InterfaceDeclarationSyntax, ISemanticModelWrapper, CSharpInterfaceSyntaxNodeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpContextInfoBuilderInterface(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IAppLogger<AppLevel> logger)
        : base(collector, factory, logger)
    {
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@interface;
    public override bool CanBuild(ISyntaxWrapper contextInfo) => false;
}
