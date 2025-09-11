using System;
using System.Collections.Generic;
using System.Linq;

namespace ContextKit.Model.Collector;

public interface IHtmlMatrixIndexer<TContext>
    where TContext : IContextWithReferences<TContext>
{
    Dictionary<string, TContext> Build();
}
