namespace ContextKit.Model.Service;

public static class ContextInfoService
{
    public static bool AddToReferences<T>(T source, T callee)
        where T : IContextWithReferences<T>
    {
        return source.References.Add(callee);
    }

    public static bool AddToInvokedBy<T>(T caller, T callee)
        where T : IContextWithReferences<T>
    {
        return callee.InvokedBy.Add(caller);
    }

    public static bool AddToReference<T>(T source, T callee)
        where T : IContextWithReferences<T>
    {
        return source.References.Add(callee);
    }
    public static bool AddToProperties<T>(T source, T property)
        where T : IContextWithReferences<T>
    {
        return source.Properties.Add(property);
    }

    public static IEnumerable<ContextInfo> GetReferencesSortedByInvocation<T>(T source)
        where T : IContextInfo, IContextWithReferences<T>
    {
        return (IEnumerable<ContextInfo>)source.References.OrderBy(t => t.SpanStart);
    }
}