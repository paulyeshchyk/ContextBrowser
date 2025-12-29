using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ContextKit.Model;

// context: ContextInfoMatrix, build
public interface IContextInfoMap<TContext, TTensor>
    where TContext : IContextWithReferences<TContext>
    where TTensor : notnull
{
    // context: ContextInfoMatrix, build
    Task BuildAsync(IEnumerable<TContext> contextsList, CancellationToken cancellationToken);

    Dictionary<TTensor, List<TContext>>? GetMapData();
}
