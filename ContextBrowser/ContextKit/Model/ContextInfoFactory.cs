using Microsoft.CodeAnalysis;

namespace ContextBrowser.ContextKit.Model;

public interface IContextFactory<T>
{
    T Create(T? parent, ContextInfoElementType type, string? ns, T? owner, string? itemName, string symbolName, SyntaxNode syntaxNode);
}

// context: ContextInfo, builder
// pattern: Factory
public class ContextInfoFactory<T> : IContextFactory<T>
    where T : ContextInfo
{
    public T Create(T? parent, ContextInfoElementType type, string? ns, T? owner, string? itemName, string symbolName, SyntaxNode syntaxNode)
    {
        var result = new ContextInfo
        {
            ElementType = type,
            Name = itemName ?? owner?.Name,
            Namespace = ns,
            ClassOwner = type == ContextInfoElementType.method ? owner : null,
            SymbolName = symbolName,
            SyntaxNode = syntaxNode
        };
        return (T)result;
    }
}