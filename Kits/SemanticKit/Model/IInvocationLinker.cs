using ContextKit.Model;

namespace SemanticKit.Model;

public interface IInvocationLinker<TContext, TInvocationExpressionSyntax>
    where TContext : ContextInfo, IContextWithReferences<TContext>
    where TInvocationExpressionSyntax : class
{
    void Link(IEnumerable<TInvocationExpressionSyntax> invocationList, TContext callerContext, TContext callerContextInfo, CancellationToken cancellationToken);
}
