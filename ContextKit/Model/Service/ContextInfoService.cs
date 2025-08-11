namespace ContextKit.Model.Service;

public static class ContextInfoService
{
    public static bool AddToReferences<T>(T source, T callee)
        where T : ContextInfo, IContextWithReferences<T>
    {
        return source.References.Add(callee);
    }

    public static bool AddToInvokedBy<T>(T source, T obj)
        where T : ContextInfo, IContextWithReferences<T>
    {
        return source.InvokedBy.Add(obj);
    }

    public static bool AddToReference<T>(T source, T callee)
        where T : ContextInfo, IContextWithReferences<T>
    {
        return source.References.Add(callee);
    }

    public static IEnumerable<ContextInfo> GetReferencesSortedByInvocation<T>(T source)
        where T : ContextInfo, IContextWithReferences<T>
    {
        return source.References.OrderBy(t => t.SpanStart);
    }
}