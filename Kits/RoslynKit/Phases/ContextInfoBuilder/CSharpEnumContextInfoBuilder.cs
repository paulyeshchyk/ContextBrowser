using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class CSharpEnumContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, EnumDeclarationSyntax, ISemanticModelWrapper, CSharpEnumSyntaxNodeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpEnumContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IAppLogger<AppLevel> logger)
        : base(collector, factory, logger)
    {
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@enum;
}