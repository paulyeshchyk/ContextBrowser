namespace ContextKit.Model.Collector;

// context: ContextInfo, build
public class ContextInfoCollector<TContext> : IContextCollector<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public readonly HashSet<TContext> Collection = new HashSet<TContext>();
    private readonly List<TContext> FakeCollection = new List<TContext>();

    public Dictionary<string, TContext> BySymbolDisplayName { get; }

    public ContextInfoCollector()
    {
        BySymbolDisplayName = new Dictionary<string, TContext>(StringComparer.Ordinal);
    }

    // context: ContextInfo, read
    public IEnumerable<TContext> GetAll() => Collection;

    // context: ContextInfo, build
    public void Add(TContext info)
    {
        Collection.Add(info);
        if(!string.IsNullOrWhiteSpace(info.FullName))
            BySymbolDisplayName[info.FullName] = info;
    }

    public void Append(TContext info)
    {
        FakeCollection.Add(info);
        if(!string.IsNullOrWhiteSpace(info.FullName))
            BySymbolDisplayName[info.FullName] = info;
    }

    public void MergeFakeItems()
    {
        foreach(var item in FakeCollection)
        {
            Add(item);
        }
        FakeCollection.Clear();
    }
}

public class ContextInfoReferenceCollector<TContext> : IContextCollector<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public Dictionary<string, TContext> BySymbolDisplayName { get; }

    private readonly List<TContext> FakeCollection = new List<TContext>();

    private readonly List<TContext> _existing;

    public ContextInfoReferenceCollector(IEnumerable<TContext> existing)
    {
        _existing = existing.ToList();

        BySymbolDisplayName = _existing
            .Where(x => !string.IsNullOrWhiteSpace(x.FullName))
            .GroupBy(x => x.FullName!)
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

    public void Append(TContext info)
    {
        FakeCollection.Add(info);
        if(!string.IsNullOrWhiteSpace(info.FullName))
            BySymbolDisplayName[info.FullName] = info;
    }


    public void MergeFakeItems()
    {
        foreach(var item in FakeCollection)
        {
            _existing.Add(item);
        }
        FakeCollection.Clear();
    }
}