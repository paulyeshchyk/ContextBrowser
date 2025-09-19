using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TensorKit.Model;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public interface IContextInfoDataset<TContext, TKey> : IEnumerable<KeyValuePair<TKey, List<TContext>>>
    where TContext : IContextWithReferences<TContext>
    where TKey : notnull
{
    Dictionary<TKey, List<TContext>> Data { get; }

    // context: ContextInfoMatrix, create
    IEnumerable<TContext> GetAll();

    // context: ContextInfoMatrix, create
    void Add(TContext? item, TKey toCell);

    // context: ContextInfoMatrix, read
    bool TryGetValue(TKey key, out List<TContext> value);
}
