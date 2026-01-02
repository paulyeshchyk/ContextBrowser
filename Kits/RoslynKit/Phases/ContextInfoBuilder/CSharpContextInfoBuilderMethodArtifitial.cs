using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model.SyntaxWrapper;
using RoslynKit.Phases.ContextInfoBuilder.SyntaxNodeWrapper;
using SemanticKit.Model;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Phases.ContextInfoBuilder;

// context: roslyn, build, contextInfo
public class CSharpContextInfoBuilderMethodArtifitial<TContext> : BaseContextInfoBuilder<TContext, MethodDeclarationSyntax, ISemanticModelWrapper, CSharpSyntaxNodeWrapperMethod>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpContextInfoBuilderMethodArtifitial(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IAppLogger<AppLevel> logger)
        : base(collector, factory, logger)
    {
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@method;
    public override bool CanBuild(ISyntaxWrapper contextInfo) => contextInfo is CSharpSyntaxWrapperMethodArtifitial;

}
