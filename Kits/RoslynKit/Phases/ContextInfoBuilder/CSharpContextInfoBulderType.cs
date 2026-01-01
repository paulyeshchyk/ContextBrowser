using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model.SyntaxNodeWrapper;
using RoslynKit.Model.SyntaxWrapper;
using SemanticKit.Model;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class CSharpContextInfoBulderType<TContext> : BaseContextInfoBuilder<TContext, MemberDeclarationSyntax, ISemanticModelWrapper, CSharpTypeSyntaxNodeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpContextInfoBulderType(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IAppLogger<AppLevel> logger)
        : base(collector, factory, logger)
    {
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@class;
    public override bool CanBuild(ISyntaxWrapper contextInfo) => contextInfo is CSharpSyntaxWrapperType;
}
