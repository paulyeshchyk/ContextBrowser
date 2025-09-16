using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TensorKit.Model;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public interface IContextInfoDataset<TContext> : IEnumerable<KeyValuePair<DomainPerActionTensor, List<TContext>>>
    where TContext : IContextWithReferences<TContext>
{
    Dictionary<DomainPerActionTensor, List<TContext>> Data { get; }

    // context: ContextInfoMatrix, create
    IEnumerable<TContext> GetAll();

    // context: ContextInfoMatrix, create
    void Add(TContext? item, DomainPerActionTensor toCell);

    // context: ContextInfoMatrix, read
    bool TryGetValue(DomainPerActionTensor key, out List<TContext> value);
}
