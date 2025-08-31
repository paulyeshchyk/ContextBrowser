namespace ContextKit.Model.Service;

public static class ContextInfoService
{
    public static bool AddToReferences<T>(T? source, T? callee)
        where T : IContextWithReferences<T>
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

    public static bool AddToInvokedBy<T>(T caller, T callee)
        where T : IContextWithReferences<T>
    {
        _ = callee.InvokedBy.Add(caller);
        return true;
    }

    public static bool AddToProperties<T>(T source, T property)
        where T : IContextWithReferences<T>
    {
        _ = source.Properties.Add(property);
        return true;
    }

    public static IEnumerable<ContextInfo> GetReferencesSortedByInvocation<T>(T source)
        where T : IContextInfo, IContextWithReferences<T>
    {
        return (IEnumerable<ContextInfo>)source.References.OrderBy(t => t.SpanStart);
    }
}