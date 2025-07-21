namespace ContextBrowser.model;

public interface IContextFactory<T>
{
    T Create(T? parent, ContextInfoElementType type, string? ns, string? owner, string? fullName = null);
}

internal class ContextInfoFactory<T> : IContextFactory<T>
    where T : ContextInfo
{
    public T Create(T? parent, ContextInfoElementType type, string? ns, string? owner, string? fullName = null)
    {
        var result = new ContextInfo
        {
            ElementType = type,
            Name = fullName ?? owner,
            Namespace = ns,
            ClassOwner = type == ContextInfoElementType.method ? owner : null
        };
        return (T)result;
    }
}