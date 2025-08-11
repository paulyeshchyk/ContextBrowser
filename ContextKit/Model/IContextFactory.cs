namespace ContextKit.Model;

public interface IContextFactory<T>
{
    T Create(
        T? owner,
        ContextInfoElementType elementType,
        string nsName,
        string name,
        string? fullName,
        ISyntaxNodeInfo? syntaxNode,
        int spanStart,
        int spanEnd,
        ISymbolInfo? symbol);
}
