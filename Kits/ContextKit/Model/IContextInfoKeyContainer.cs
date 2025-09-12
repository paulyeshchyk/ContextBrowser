using System.Collections.Generic;

namespace ContextKit.Model;

public interface IContextInfoKeyContainer<Key>
{
    public Key ContextKey { get; }
}

public abstract class BaseKeyAndDataContainer<Key> : IContextInfoKeyContainer<Key>
{
    public Key ContextKey { get; init; }

    public IEnumerable<IContextInfo> ContextInfoList { get; init; }

    public BaseKeyAndDataContainer(Key contextKey, IEnumerable<IContextInfo> contextInfoList)
    {
        ContextKey = contextKey;
        ContextInfoList = contextInfoList;
    }
}