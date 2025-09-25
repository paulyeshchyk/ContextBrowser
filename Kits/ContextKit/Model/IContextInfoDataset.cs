using System.Collections.Generic;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public interface IContextInfoDataset<TContext, TTensor> : IEnumerable<KeyValuePair<TTensor, List<TContext>>>
    where TContext : IContextWithReferences<TContext>
    where TTensor : notnull
{
    Dictionary<TTensor, List<TContext>> Data { get; }

    // context: ContextInfoMatrix, create
    IEnumerable<TContext> GetAll();

    // context: ContextInfoMatrix, create
    void Add(TContext? item, TTensor toCell);

    // context: ContextInfoMatrix, read
    bool TryGetValue(TTensor key, out List<TContext> value);
}
