using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface IInvocationLinker<TContext, TInvocationExpressionSyntax>
    where TContext : ContextInfo, IContextWithReferences<TContext>
    where TInvocationExpressionSyntax : class
{
    void Link(IEnumerable<TInvocationExpressionSyntax> invocationList, TContext callerContext, TContext callerContextInfo, SemanticOptions _options, CancellationToken cancellationToken);
}
