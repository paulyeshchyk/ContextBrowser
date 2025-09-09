using System.Collections.Generic;

namespace ContextKit.Model;

public interface IContextKey
{
    string Action { get; set; }

    string Domain { get; set; }
}

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

public interface IContextKeyMap
{
    // context: ContextInfoMatrix, read
    IEnumerable<string> GetActions();

    // context: ContextInfoMatrix, read
    IEnumerable<string> GetDomains();

    // context: ContextInfoMatrix, read
    List<ContextInfo> GetMethodsByAction(string action);

    // context: ContextInfoMatrix, read
    List<ContextInfo> GetMethodsByDomain(string domain);
}