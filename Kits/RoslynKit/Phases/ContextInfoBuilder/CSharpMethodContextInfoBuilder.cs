using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Phases.ContextInfoBuilder;

// context: roslyn, build, contextInfo
public class CSharpMethodContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, MethodDeclarationSyntax, ISemanticModelWrapper, CSharpMethodSyntaxNodeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpMethodContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IAppLogger<AppLevel> logger)
        : base(collector, factory, logger)
    {
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@method;

    public TContext? BuildInvocationContextInfo(TContext? ownerContext, CSharpMethodSyntaxWrapper wrapper)
    {
        var contextInfoDto = wrapper.GetContextInfoDto();
        if (contextInfoDto == null)
        {
            return default;
        }

        contextInfoDto.ClassOwner = ownerContext;
        contextInfoDto.MethodOwner = contextInfoDto;// делаем себя же владельцем метода

        return BuildContextInfo(ownerContext, contextInfoDto);
    }
}
