namespace ContextBrowser.ContextKit.Model;

public interface IContextFactory<T>
{
    T Create(T? parent, ContextInfoElementType type, string? ns, T? owner, string? fullName = null);
}

// context: contextInfo, builder
// pattern: Factory
internal class ContextInfoFactory<T> : IContextFactory<T>
    where T : ContextInfo
{
    public T Create(T? parent, ContextInfoElementType type, string? ns, T? owner, string? fullName = null)
    {
        var result = new ContextInfo
        {
            ElementType = type,
            Name = fullName ?? owner?.Name,
            Namespace = ns,
            ClassOwner = type == ContextInfoElementType.method ? owner : null
        };
        return (T)result;
    }
}