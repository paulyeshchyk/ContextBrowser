using System.Collections.Generic;
using System.Threading;
using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

// context: invocation, build
public interface IInvocationLinker<in TContext, TInvocationExpressionSyntax>
    where TContext : IContextWithReferences<TContext>
    where TInvocationExpressionSyntax : class
{
    // context: invocation, build
    void Link(List<TInvocationExpressionSyntax> invocationList, TContext callerContext, TContext callerContextInfo, SemanticOptions options, CancellationToken cancellationToken);
}
