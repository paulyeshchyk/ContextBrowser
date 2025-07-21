namespace ContextBrowser.model;

public interface IContextCollector<T>
{
    void Add(T item);

    public Dictionary<string, T> ByFullName { get; }
}

internal class ContextInfoCollector<T> : IContextCollector<T>
    where T : IContextWithReferences<T>
{
    public readonly List<T> Collection = new List<T>();

    public Dictionary<string, T> ByFullName { get; }


    public ContextInfoCollector()
    {
        ByFullName = new Dictionary<string, T>(StringComparer.Ordinal);
    }

    public void Add(T info)
    {
        Collection.Add(info);
        if(!string.IsNullOrWhiteSpace(info.Name))
            ByFullName[info.Name] = info;
    }
}
