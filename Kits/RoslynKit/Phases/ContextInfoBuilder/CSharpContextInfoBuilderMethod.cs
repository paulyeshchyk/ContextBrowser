using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model.SyntaxNodeWrapper;
using RoslynKit.Model.SyntaxWrapper;
using SemanticKit.Model;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Phases.ContextInfoBuilder;

// context: roslyn, build, contextInfo
public class CSharpContextInfoBuilderMethod<TContext> : BaseContextInfoBuilder<TContext, MethodDeclarationSyntax, ISemanticModelWrapper, CSharpMethodSyntaxNodeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpContextInfoBuilderMethod(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IAppLogger<AppLevel> logger)
        : base(collector, factory, logger)
    {
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@method;
    public override bool CanBuild(ISyntaxWrapper contextInfo) => contextInfo is CSharpSyntaxWrapperMethod;
}
