namespace ContextKit.Model.Factory;

// context: ContextInfo, build
// pattern: Factory
public class ContextInfoFactory<T> : IContextFactory<T>
    where T : ContextInfo
{
    public const string SUnknownSymbolName = "unknown symbolname";

    public T Create(
        T? owner,
        ContextInfoElementType elementType,
        string nsName,
        string name,
        string? fullName,
        ISyntaxNodeInfo? syntaxNode,
        int spanStart,
        int spanEnd,
        ISymbolInfo? symbol)
    {
        var result = new ContextInfo
        {
            ElementType = elementType,
            Name = name,
            Namespace = nsName,
            FullName = fullName ?? SUnknownSymbolName,
            SpanStart = spanStart,
            SpanEnd = spanEnd,
            Symbol = symbol, // Используем уже готовую обёртку
            SyntaxNode = syntaxNode, // Используем уже готовую обёртку
        };

        // Установка ссылочной информации
        if(elementType == ContextInfoElementType.method)
        {
            result.ClassOwner = owner;
            result.MethodOwner = result;
        }
        else if(elementType == ContextInfoElementType.property)
        {
            result.ClassOwner = owner;
            result.MethodOwner = null;
        }
        else
        {
            result.ClassOwner = null;
            result.MethodOwner = null;
        }

        return (T)result;
    }
}