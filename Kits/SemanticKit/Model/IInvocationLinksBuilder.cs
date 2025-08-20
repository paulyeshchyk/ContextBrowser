using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface IInvocationLinksBuilder<TContext> where TContext : ContextInfo, IContextWithReferences<TContext>
{
    TContext? LinkInvocation(TContext callerContextInfo, IInvocationSyntaxWrapper symbolDto, SemanticOptions options);
}
