using System.Collections.Generic;

namespace ContextKit.Model.Collector;

public class ContextInfoReferenceIndex<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly Dictionary<string, TContext> _bySymbol;

    public ContextInfoReferenceIndex(IEnumerable<TContext> snapshot)
    {
        _bySymbol = ContextInfoFullNameIndexBuilder.Build(snapshot);
    }

    /// <summary>
    /// Быстрый доступ по FullName (без пересборки на каждом вызове).
    /// </summary>
    public IReadOnlyDictionary<string, TContext> BySymbolDisplayName => _bySymbol;

    /// <summary>
    /// Все элементы snapshot-а.
    /// </summary>
    public IEnumerable<TContext> All => _bySymbol.Values;

    /// <summary>
    /// Явное обновление индекса (например, если нужно добавить новые элементы).
    /// </summary>
    public void Update(IEnumerable<TContext> items)
    {
        foreach (var item in items)
        {
            if (!string.IsNullOrWhiteSpace(item.FullName))
                _bySymbol[item.FullName] = item;
        }
    }
}