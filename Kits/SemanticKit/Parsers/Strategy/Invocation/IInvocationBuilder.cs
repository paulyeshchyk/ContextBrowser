using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Parsers.Strategy.Invocation;

public interface IInvocationBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    Task BuildReferencesAsync(TContext callerContext, SemanticOptions options, CancellationToken cancellationToken);
}
