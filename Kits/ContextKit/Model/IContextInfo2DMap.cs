using System.Collections.Generic;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public interface IContextInfo2DMap<TContext, TKey> : IContextInfoMap<TContext, TKey>
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
