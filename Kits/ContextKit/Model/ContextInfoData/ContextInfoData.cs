using System.Collections;

namespace ContextKit.Model.Matrix;

// context: ContextInfoMatrix, model
public partial class ContextInfoData : IContextInfoData
{
    private readonly Dictionary<ContextInfoDataCell, List<ContextInfo>> _data = new();

    // context: ContextInfoMatrix, read
    public IEnumerable<string> GetDomains() => _data.Select(k => k.Key.Domain);

    // context: ContextInfoMatrix, read
    public IEnumerable<string> GetActions() => _data.Select(k => k.Key.Action);

    // context: ContextInfoMatrix, read
    public List<ContextInfo> GetMethodsByAction(string action) => _data.Where(kvp => kvp.Key.Action == action).SelectMany(kvp => kvp.Value).Distinct().ToList();

    // context: ContextInfoMatrix, read
    public List<ContextInfo> GetMethodsByDomain(string domain) => _data.Where(kvp => kvp.Key.Domain == domain).SelectMany(kvp => kvp.Value).Distinct().ToList();

    // context: ContextInfoMatrix, update
    public void Add(ContextInfo? item, ContextInfoDataCell toCell)
    {
        if (!_data.ContainsKey(toCell))
            _data[toCell] = new List<ContextInfo>();
        if (item != null)
        {
            _data[toCell].Add(item);
        }
    }

    // context: ContextInfoMatrix, read
    public bool TryGetValue(ContextInfoDataCell key, out List<ContextInfo> value)
    {
        var extracted = _data.TryGetValue(key, out var theValue);
        value = extracted
            ? theValue!
            : new List<ContextInfo>();
        return extracted;
    }

    // context: ContextInfoMatrix, read
    public IEnumerator<KeyValuePair<ContextInfoDataCell, List<ContextInfo>>> GetEnumerator()
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