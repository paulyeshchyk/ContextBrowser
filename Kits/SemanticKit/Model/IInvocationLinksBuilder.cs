using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface IInvocationLinksBuilder<TContext> where TContext : ContextInfo, IContextWithReferences<TContext>
{
    TContext? LinkInvocation(TContext callerContextInfo, BaseSyntaxWrapper symbolDto, SemanticOptions options);
}
