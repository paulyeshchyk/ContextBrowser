using System.Collections.Generic;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public interface IContextInfoDataset<TContext> : IEnumerable<KeyValuePair<IContextKey, List<TContext>>>, IContextKeyMap<TContext>
    where TContext : IContextWithReferences<TContext>
{
    // context: ContextInfoMatrix, create
    IEnumerable<TContext> GetAll();

    // context: ContextInfoMatrix, create
    void Add(TContext? item, IContextKey toCell);

    // context: ContextInfoMatrix, read
    bool TryGetValue(IContextKey key, out List<TContext> value);
}