using System.Collections.Generic;

namespace ContextKit.Model.Container;

public abstract class ContextInfoKeyContainerBase<Key> : IContextInfoKeyContainer<Key>
{
    public Key ContextKey { get; init; }

    public IEnumerable<IContextInfo> ContextInfoList { get; init; }

    public ContextInfoKeyContainerBase(Key contextKey, IEnumerable<IContextInfo> contextInfoList)
    {
        ContextKey = contextKey;
        ContextInfoList = contextInfoList;
    }
}