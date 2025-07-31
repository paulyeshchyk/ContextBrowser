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

    // context: ContextInfo, read
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

    private readonly List<TContext> _existing;

    public ContextInfoReferenceCollector(IEnumerable<TContext> existing)
    {
        _existing = existing.ToList();

        ByFullName = _existing
            .Where(x => !string.IsNullOrWhiteSpace(x.SymbolName))
            .GroupBy(x => x.SymbolName!)
            .ToDictionary(
                g => g.Key,
                g => g.First(),
                StringComparer.OrdinalIgnoreCase);
    }

    public void Add(TContext item)
    {
        // намеренно ничего не делаем — коллектор только читает
    }

    public IEnumerable<TContext> GetAll() => _existing;
}