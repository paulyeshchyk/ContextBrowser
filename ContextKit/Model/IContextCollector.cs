namespace ContextKit.Model;

public interface IContextCollector<T>
{
    void Add(T item);

    void Append(T item);

    IEnumerable<T> GetAll();

    void MergeFakeItems();

    public Dictionary<string, T> BySymbolDisplayName { get; }
}
