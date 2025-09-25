using System.Collections.Generic;

namespace ContextKit.Model;

public interface IKeyIndexBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    Dictionary<object, TContext>? Build(IEnumerable<TContext> contextsList);
}