namespace SemanticKit.Model;

public interface ISemanticModelWrapper
{
    // Возвращает объект, предоставляющий информацию о символах и типах.
    object? GetSymbolInfo(object node, CancellationToken cancellationToken);

    // Возвращает информацию о вызове для данного узла.
    object? GetSymbolForInvocation(object invocationNode);

    object? GetDeclaredSymbol(object syntax, CancellationToken cancellationToken);

    object? GetTypeInfo(object syntax);
}
