using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public partial class ContextInfoDataset<TContext> : IContextInfoDataset<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly Dictionary<IContextKey, List<TContext>> _data = new();

    // context: ContextInfoMatrix, read
    public IEnumerable<string> GetDomains() => _data.Select(k => k.Key.Domain);

    // context: ContextInfoMatrix, read
    public IEnumerable<string> GetActions() => _data.Select(k => k.Key.Action);

    // context: ContextInfoMatrix, read
    public List<TContext> GetMethodsByAction(string action) => _data.Where(kvp => kvp.Key.Action == action).SelectMany(kvp => kvp.Value).Distinct().ToList();

    // context: ContextInfoMatrix, read
    public List<TContext> GetMethodsByDomain(string domain) => _data.Where(kvp => kvp.Key.Domain == domain).SelectMany(kvp => kvp.Value).Distinct().ToList();

    public IEnumerable<TContext> GetAll() => _data.SelectMany(pair => pair.Value);

    // context: ContextInfoMatrix, update
    public void Add(TContext? item, IContextKey toCell)
    {
        if (!_data.ContainsKey(toCell))
            _data[toCell] = new List<TContext>();
        if (item != null)
        {
            _data[toCell].Add(item);
        }
    }

    // context: ContextInfoMatrix, read
    public bool TryGetValue(IContextKey key, out List<TContext> value)
    {
        var extracted = _data.TryGetValue(key, out var theValue);
        value = extracted
            ? theValue!
            : new List<TContext>();
        return extracted;
    }

    // context: ContextInfoMatrix, read
    public IEnumerator<KeyValuePair<IContextKey, List<TContext>>> GetEnumerator()
    {
        foreach (var entry in _data)
        {
            yield return entry;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _data.GetEnumerator();
    }
}