using System.Collections.Generic;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public interface IContextInfoDataset : IEnumerable<KeyValuePair<IContextKey, List<ContextInfo>>>, IContextKeyMap
{
    // context: ContextInfoMatrix, create
    IEnumerable<ContextInfo> GetAll();

    // context: ContextInfoMatrix, create
    void Add(ContextInfo? item, IContextKey toCell);

    // context: ContextInfoMatrix, read
    bool TryGetValue(IContextKey key, out List<ContextInfo> value);
}