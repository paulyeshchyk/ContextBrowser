using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class CSharpTypeContextInfoBulder<TContext> : BaseContextInfoBuilder<TContext, MemberDeclarationSyntax, ISemanticModelWrapper, CSharpTypeSyntaxNodeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpTypeContextInfoBulder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IAppLogger<AppLevel> logger)
        : base(collector, factory, logger)
    {
    }

    public TContext? BuildContextInfo(TContext? ownerContext, CSharpTypeSyntaxWrapper wrapper)
    {
        var symInfo = wrapper.GetContextInfoDto();

        return BuildContextInfo(ownerContext, symInfo);
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@class;
}
