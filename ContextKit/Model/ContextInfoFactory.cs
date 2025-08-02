using Microsoft.CodeAnalysis;

namespace ContextKit.Model;

public interface IContextFactory<T>
{
    T Create(T? owner, ContextInfoElementType type, string? ns, string? itemName, string? symbolName, SyntaxNode? syntaxNode);
}

// context: ContextInfo, build
// pattern: Factory
public class ContextInfoFactory<T> : IContextFactory<T>
    where T : ContextInfo
{
    public const string SUnknownSymbolName = "unknown symbolname";

    public T Create(T? owner, ContextInfoElementType type, string? ns, string? itemName, string? symbolName, SyntaxNode? syntaxNode)
    {
        var result = new ContextInfo
        {
            ElementType = type,
            Name = itemName ?? owner?.Name,
            Namespace = ns,
            ClassOwner = type == ContextInfoElementType.method ? owner : null,
            SymbolName = symbolName ?? SUnknownSymbolName,
            SyntaxNode = syntaxNode
        };

        // устанавливаем ссылочную информацию
        if(type == ContextInfoElementType.method)
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