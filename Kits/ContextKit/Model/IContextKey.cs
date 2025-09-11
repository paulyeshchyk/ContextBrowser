using System.Collections.Generic;
using ContextBrowserKit.Options.Export;

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

// context: ContextInfoMatrix, model
public interface IContextKeyMap<TContext>
    where TContext : IContextWithReferences<TContext>
{
    // context: ContextInfoMatrix, build
    void Build(IEnumerable<TContext> contextsList, ExportMatrixOptions matrixOptions, IContextClassifier contextClassifier);

    // context: ContextInfoMatrix, read
    IEnumerable<string> GetActions();

    // context: ContextInfoMatrix, read
    IEnumerable<string> GetDomains();

    // context: ContextInfoMatrix, read
    List<TContext> GetMethodsByAction(string action);

    // context: ContextInfoMatrix, read
    List<TContext> GetMethodsByDomain(string domain);
}