using System;
using System.Collections.Generic;
using System.Linq;

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
    public bool AddIfNotExists(TContext info)
    {
        if (BySymbolDisplayName.TryGetValue(info.FullName, out var available))
        {
            return false;
        }

        Add(info);

        if (!string.IsNullOrWhiteSpace(info.FullName))
            BySymbolDisplayName[info.FullName] = info;

        return true;
    }

    public TContext? GetItem(string predicate)
    {
        BySymbolDisplayName.TryGetValue(predicate, out TContext? result);
        return result;
    }

    // context: ContextInfo, build
    public void Add(TContext info)
    {
        Collection.Add(info);

        if (!string.IsNullOrWhiteSpace(info.FullName))
            BySymbolDisplayName[info.FullName] = info;
    }

    public void Append(TContext info)
    {
        FakeCollection.Add(info);
        if (!string.IsNullOrWhiteSpace(info.FullName))
            BySymbolDisplayName[info.FullName] = info;
    }

    public void MergeFakeItems()
    {
        foreach (var item in FakeCollection)
        {
            AddIfNotExists(item);
        }
        FakeCollection.Clear();
    }

    public void Renew(IEnumerable<TContext> byItems)
    {
        // намеренно ничего не делаем — коллектор необновляемый здесь
    }
}

public class ContextInfoReferenceCollector<TContext> : IContextCollector<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public Dictionary<string, TContext> BySymbolDisplayName { get; private set; } = new Dictionary<string, TContext>();

    private readonly HashSet<TContext> FakeCollection = new();

    private List<TContext> _existing = new List<TContext>();

    public ContextInfoReferenceCollector(IEnumerable<TContext> existing)
    {
        Renew(existing);
    }

    public void Renew(IEnumerable<TContext> byItems)
    {
        _existing = byItems.ToList();

        BySymbolDisplayName = ContextInfoFullNameIndexBuilder.Build(_existing);
    }

    public TContext? GetItem(string predicate)
    {
        BySymbolDisplayName.TryGetValue(predicate, out TContext? result);
        return result;
    }

    public bool AddIfNotExists(TContext item)
    {
        // намеренно ничего не делаем — коллектор только читает
        return true;
    }

    public void Add(TContext item)
    {
        // намеренно ничего не делаем — коллектор только читает
    }

    public IEnumerable<TContext> GetAll() => _existing;

    public void Append(TContext info)
    {
        FakeCollection.Add(info);
        if (!string.IsNullOrWhiteSpace(info.FullName))
            BySymbolDisplayName[info.FullName] = info;
    }

    public void MergeFakeItems()
    {
        var legacy = FakeCollection.Where(item => _existing.Any(existingItem => existingItem.FullName == item.FullName)).ToList();

        var newItems = FakeCollection.Where(item => !_existing.Any(existingItem => existingItem.FullName == item.FullName)).ToList();
        _existing.AddRange(newItems);

        FakeCollection.Clear();
    }
}
