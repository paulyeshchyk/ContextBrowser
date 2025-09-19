using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options.Export;

namespace ContextKit.Model;

// context: ContextInfoMatrix, build
public interface IContextInfoMap<TContext, TKey>
    where TContext : IContextWithReferences<TContext>
    where TKey : notnull
{
    // context: ContextInfoMatrix, build
    Task BuildAsync(IEnumerable<TContext> contextsList, CancellationToken cancellationToken);

    Dictionary<TKey, List<TContext>>? GetMapData();
}
