using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Model.SyntaxWrapper;

namespace SemanticKit.Model;

public interface IInvocationLinksBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    Task<TContext?> LinkInvocationAsync(TContext callerContextInfo, ISyntaxWrapper symbolDto, SemanticOptions options, CancellationToken cancellationToken);
}
