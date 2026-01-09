using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace SemanticKit.Model;

public interface ISemanticModelWrapper
{
    Task<object?> GetSymbolInfoAsync(object node, CancellationToken cancellationToken);

    Task<TSymbol?> GetDeclaredSymbolAsync<TSymbol>(object syntax, CancellationToken cancellationToken);

    object? GetTypeInfo(object syntax);
}
