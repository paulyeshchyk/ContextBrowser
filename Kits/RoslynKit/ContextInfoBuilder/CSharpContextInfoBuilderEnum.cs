using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Phases.ContextInfoBuilder.SyntaxNodeWrapper;
using SemanticKit.Model;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Phases.ContextInfoBuilder;

// context: ContextInfo, build, roslyn
public class CSharpContextInfoBuilderEnum<TContext> : ContextInfoBuilder<TContext, EnumDeclarationSyntax, CSharpSyntaxNodeWrapperEnum>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpContextInfoBuilderEnum(
        IContextCollector<TContext> collector,
        IContextFactory<TContext> factory,
        ISymbolWrapperConverter symbolWrapperConverter,
        IContextInfoDtoConverter<TContext, ISyntaxNodeWrapper> contextInfoDtoConverter,
        IAppLogger<AppLevel> logger)
        : base(collector, factory, symbolWrapperConverter, contextInfoDtoConverter, logger)
    {
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@enum;
    public override bool CanBuild(ISyntaxWrapper contextInfo) => false;
}