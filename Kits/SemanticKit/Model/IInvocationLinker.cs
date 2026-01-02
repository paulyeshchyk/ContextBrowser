using System.Collections.Generic;
using System.Threading;
using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface IInvocationLinker<TContext, TInvocationExpressionSyntax>
    where TContext : IContextWithReferences<TContext>
    where TInvocationExpressionSyntax : class
{
    void Link(List<TInvocationExpressionSyntax> invocationList, TContext callerContext, TContext callerContextInfo, SemanticOptions options, CancellationToken cancellationToken);
}
