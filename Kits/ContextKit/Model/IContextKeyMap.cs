using System.Collections.Generic;
using ContextBrowserKit.Options.Export;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public interface DomainPerActionKeyMap<TContext, TKey> : DomainPerActionKeyMapBuilder<TContext, TKey>
    where TContext : IContextWithReferences<TContext>
    where TKey : notnull
{
    // context: ContextInfoMatrix, read
    IEnumerable<string> GetRows();

    // context: ContextInfoMatrix, read
    IEnumerable<string> GetCols();

    // context: ContextInfoMatrix, read
    List<TContext> GetDataByRow(string action);

    // context: ContextInfoMatrix, read
    List<TContext> GetDataByCol(string domain);
}

// context: ContextInfoMatrix, build
public interface DomainPerActionKeyMapBuilder<TContext, TKey>
    where TContext : IContextWithReferences<TContext>
    where TKey : notnull
{
    // context: ContextInfoMatrix, build
    void Build(IEnumerable<TContext> contextsList, ExportMatrixOptions matrixOptions, IDomainPerActionContextClassifier contextClassifier);

    Dictionary<TKey, List<TContext>>? GetMapData();
}
