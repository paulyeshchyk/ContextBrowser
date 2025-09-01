using System.Threading;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface IInvocationSyntaxResolver
{
    ISyntaxWrapper? ResolveInvocationSymbol(object invocation, SemanticOptions options, CancellationToken cancellationToken);
}
