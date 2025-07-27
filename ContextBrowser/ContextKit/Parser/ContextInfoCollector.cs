using ContextBrowser.ContextKit.Model;

namespace ContextBrowser.ContextKit.Parser;

public interface IContextCollector<T>
{
    void Add(T item);

    IEnumerable<T> GetAll();

    public Dictionary<string, T> ByFullName { get; }
}

// context: ContextInfo, build
public class ContextInfoCollector<T> : IContextCollector<T>
    where T : IContextWithReferences<T>
{
    public readonly List<T> Collection = new List<T>();

    public Dictionary<string, T> ByFullName { get; }

    public ContextInfoCollector()
    {
        ByFullName = new Dictionary<string, T>(StringComparer.Ordinal);
    }

    public IEnumerable<T> GetAll() => Collection;

    // context: ContextInfo, build
    public void Add(T info)
    {
        Collection.Add(info);
        if(!string.IsNullOrWhiteSpace(info.SymbolName))
            ByFullName[info.SymbolName] = info;
    }
}

public class ContextInfoReferenceCollector<TContext> : IContextCollector<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public Dictionary<string, TContext> ByFullName { get; }

    public ContextInfoReferenceCollector(IEnumerable<TContext> existing)
    {
        // Строим индекс по полному имени (например: Namespace.Class.Method)
        ByFullName = existing
                    .Where(x => !string.IsNullOrWhiteSpace(x.SymbolName))// FullName
                    .GroupBy(x => x.SymbolName)//FullName
                    .ToDictionary(
                        g => g.Key,
                        g => g.First(),
                        StringComparer.OrdinalIgnoreCase);
    }

    public void Add(TContext item)
    {
    }
    public IEnumerable<TContext> GetAll() => Enumerable.Empty<TContext>();
}