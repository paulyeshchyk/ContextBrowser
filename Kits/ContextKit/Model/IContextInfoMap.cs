using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public interface IContextInfoDataset<TContext> : IEnumerable<KeyValuePair<IContextKey, List<TContext>>>
    where TContext : IContextWithReferences<TContext>
{
    Dictionary<IContextKey, List<TContext>> Data { get; }

    // context: ContextInfoMatrix, create
    IEnumerable<TContext> GetAll();

    // context: ContextInfoMatrix, create
    void Add(TContext? item, IContextKey toCell);

    // context: ContextInfoMatrix, read
    bool TryGetValue(IContextKey key, out List<TContext> value);
}
