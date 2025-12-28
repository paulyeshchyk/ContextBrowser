using System.Threading;
using SemanticKit.Model.Options;
using SemanticKit.Model.SyntaxWrapper;

namespace SemanticKit.Model;

public interface IInvocationSyntaxResolver
{
    ISyntaxWrapper? ResolveInvocationSymbol(object invocation, SemanticOptions options, CancellationToken cancellationToken);
}
