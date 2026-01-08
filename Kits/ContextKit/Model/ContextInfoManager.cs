using System.Collections.Generic;
using System.Linq;

namespace ContextKit.Model.Service;

public interface IContextInfoManager<T>
    where T : IContextWithReferences<T>
{
    bool AddToInvokedBy(T caller, T callee);
    bool AddToOwns(T source, T property);
    bool AddToProperties(T source, T property);
    bool AddToReferences(T? source, T? callee);
    IEnumerable<ContextInfo> GetReferencesSortedByInvocation(T source);
}

public class ContextInfoManager<T> : IContextInfoManager<T>
    where T : IContextWithReferences<T>
{
    public bool AddToReferences(T? source, T? callee)
    {
        if (source == null)
            return false;

        if (callee == null)
        {
            return false;
        }
        if (callee?.ElementType != ContextInfoElementType.method)
        {
            return false;
        }
        _ = source.References.Add(callee);
        return true;
    }

    public bool AddToInvokedBy(T caller, T callee)
    {
        _ = callee.InvokedBy.Add(caller);
        return true;
    }

    public bool AddToProperties(T source, T property)
    {
        _ = source.Properties.Add(property);
        return true;
    }

    public bool AddToOwns(T source, T property)
    {
        _ = source.Owns.Add(property);
        return true;
    }

    public IEnumerable<ContextInfo> GetReferencesSortedByInvocation(T source)
    {
        return (IEnumerable<ContextInfo>)source.References.OrderBy(t => t.SpanStart);
    }
}