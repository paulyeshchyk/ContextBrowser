using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model.Options;
using SemanticKit.Model.SyntaxWrapper;

namespace SemanticKit.Model;

public interface IInvocationSyntaxResolver
{
    Task<ISyntaxWrapper?> ResolveInvocationSymbolAsync(object invocation, SemanticOptions options, CancellationToken cancellationToken);
}
