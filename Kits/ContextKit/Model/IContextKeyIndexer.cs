using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options.Export;
using ContextKit;
using ContextKit.Model;
using ContextKit.Model.Collector;

namespace ContextKit.Model;

public interface IContextKeyIndexer<TContext> : IContextKeyIndexBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
}

public interface IContextKeyIndexBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    void Build(IEnumerable<TContext> contextsList, ExportMatrixOptions matrixOptions, IContextClassifier contextClassifier);

    Dictionary<string, TContext>? GetIndexData();
}