namespace SemanticKit.Model;

public interface ISemanticModelWrapper
{
    object? GetSymbolInfo(object node, CancellationToken cancellationToken);

    object? GetSymbolForInvocation(object invocationNode);

    object? GetDeclaredSymbol(object syntax, CancellationToken cancellationToken);

    object? GetTypeInfo(object syntax);
}
