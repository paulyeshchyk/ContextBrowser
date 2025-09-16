using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options.Export;
using ContextKit;
using ContextKit.Model;
using ContextKit.Model.Collector;

namespace ContextKit.Model;

public interface IKeyIndexBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    void Build(IEnumerable<TContext> contextsList);

    Dictionary<string, TContext>? GetIndexData();
}