using Microsoft.CodeAnalysis;

namespace ContextKit.Model;

public interface IContextFactory<T>
{
    T Create(T? owner, ContextInfoElementType elementType, string? nsName, string? name, string? fullName, SyntaxNode? syntaxNode, int spanStart, int spanEnd, ISymbol? symbol);
}

// context: ContextInfo, build
// pattern: Factory
public class ContextInfoFactory<T> : IContextFactory<T>
    where T : ContextInfo
{
    public const string SUnknownSymbolName = "unknown symbolname";

    public T Create(T? owner, ContextInfoElementType elementType, string? nsName, string? name, string? fullName, SyntaxNode? syntaxNode, int spanStart, int spanEnd, ISymbol? symbol)
    {
        var result = new ContextInfo
        {
            ElementType = elementType,
            Name = name ?? owner?.Name,
            Namespace = nsName,
            ClassOwner = elementType == ContextInfoElementType.method ? owner : null,
            SymbolName = fullName ?? SUnknownSymbolName,
            SyntaxNode = syntaxNode,
            SpanStart = spanStart,
            SpanEnd = spanEnd,
            Symbol = symbol
        };

        // устанавливаем ссылочную информацию
        if (elementType == ContextInfoElementType.method)
        {
            result.ClassOwner = owner;
            result.MethodOwner = result; // сам себе owner
        }
        else
        {
            result.ClassOwner = null;
            result.MethodOwner = null;
        }


        return (T)result;
    }
}