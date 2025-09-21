using System.Collections.Generic;

namespace ContextKit.Model;

// context: ContextInfoMatrix, model
public interface IContextInfo2DMap<TContext, TTensor> : IContextInfoMap<TContext, TTensor>
    where TContext : IContextWithReferences<TContext>
    where TTensor : notnull
{
    // context: ContextInfoMatrix, read
    IEnumerable<object> GetRows();

    // context: ContextInfoMatrix, read
    IEnumerable<object> GetCols();
}
