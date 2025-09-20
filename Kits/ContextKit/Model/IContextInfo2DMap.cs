using System.Collections.Generic;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public interface IContextInfo2DMap<TContext, TTensor> : IContextInfoMap<TContext, TTensor>
    where TContext : IContextWithReferences<TContext>
    where TTensor : notnull
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
