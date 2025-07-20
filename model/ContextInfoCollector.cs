namespace ContextBrowser.model;

public interface IContextCollector<T>
{
    void Add(T item);
}

internal class ContextInfoCollector<T> : IContextCollector<T>
    where T : ContextInfo
{
    public readonly List<T> Collection = new List<T>();

    public void Add(T info) => Collection.Add(info);
}
