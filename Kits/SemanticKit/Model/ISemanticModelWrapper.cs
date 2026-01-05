using System.Threading;
using System.Threading.Tasks;

namespace SemanticKit.Model;

public interface ISemanticModelWrapper
{
    Task<object?> GetSymbolInfoAsync(object node, CancellationToken cancellationToken);

    object? GetSymbolForInvocation(object invocationNode);

    object? GetDeclaredSymbol(object syntax, CancellationToken cancellationToken);

    object? GetTypeInfo(object syntax);
}
