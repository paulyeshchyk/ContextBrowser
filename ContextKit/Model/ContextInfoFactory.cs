namespace ContextBrowser.ContextKit.Model;

public interface IContextFactory<T>
{
    T Create(T? parent, ContextInfoElementType type, string? ns, T? owner, string? itemName, string? displayName);
}

// context: contextInfo, builder
// pattern: Factory
internal class ContextInfoFactory<T> : IContextFactory<T>
    where T : ContextInfo
{
    public T Create(T? parent, ContextInfoElementType type, string? ns, T? owner, string? itemName, string? displayName)
    {
        var result = new ContextInfo
        {
            ElementType = type,
            Name = itemName ?? owner?.Name,
            Namespace = ns,
            ClassOwner = type == ContextInfoElementType.method ? owner : null,
            DisplayName = displayName ?? string.Empty,
        };
        return (T)result;
    }
}