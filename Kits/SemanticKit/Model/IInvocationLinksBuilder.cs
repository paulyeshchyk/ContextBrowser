using ContextKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Model.SyntaxWrapper;

namespace SemanticKit.Model;

public interface IInvocationLinksBuilder<TContext> where TContext : ContextInfo, IContextWithReferences<TContext>
{
    TContext? LinkInvocation(TContext callerContextInfo, ISyntaxWrapper symbolDto, SemanticOptions options);
}
