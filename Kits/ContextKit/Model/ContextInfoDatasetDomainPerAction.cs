using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TensorKit.Model;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public class ContextInfoDatasetDomainPerAction<TContext> : IContextInfoDataset<TContext, DomainPerActionTensor>
    where TContext : IContextWithReferences<TContext>
{
    public Dictionary<DomainPerActionTensor, List<TContext>> Data { get; } = new();

    public IEnumerable<TContext> GetAll() => Data.SelectMany(pair => pair.Value);

    // context: ContextInfoMatrix, update
    public void Add(TContext? item, DomainPerActionTensor toCell)
    {
        if (!Data.ContainsKey(toCell))
            Data[toCell] = new List<TContext>();
        if (item != null)
        {
            Data[toCell].Add(item);
        }
    }

    // context: ContextInfoMatrix, read
    public bool TryGetValue(DomainPerActionTensor key, out List<TContext> value)
    {
        var extracted = Data.TryGetValue(key, out var theValue);
        value = extracted
            ? theValue!
            : new List<TContext>();
        return extracted;
    }

    // context: ContextInfoMatrix, read
    public IEnumerator<KeyValuePair<DomainPerActionTensor, List<TContext>>> GetEnumerator()
    {
        foreach (var entry in Data)
        {
            yield return entry;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Data.GetEnumerator();
    }
}