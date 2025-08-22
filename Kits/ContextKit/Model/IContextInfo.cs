namespace ContextKit.Model;

public interface IContextInfo
{
    string Identifier { get; }

    int SpanStart { get; set; }

    int SpanEnd { get; set; }

    string Name { get; set; }

    string FullName { get; set; }

    public string Namespace { get; set; }

    ContextInfoElementType ElementType { get; set; }

    public ISymbolInfo? Symbol { get; set; }

    public ISyntaxNodeInfo? SyntaxNode { get; set; }

    public IContextInfo? ClassOwner { get; set; }

    public IContextInfo? MethodOwner { get; set; }

    public string NameWithClassOwnerName { get; }
}

public interface IContextDataContainer
{
    string? Action { get; set; }

    HashSet<string> Domains { get; }
}

// context: IContextWithReferences, model
public interface IContextWithReferences<T> : IContextInfo, IContextDataContainer
    where T : IContextWithReferences<T>
{
    // context: IContextWithReferences, read
    HashSet<T> References { get; }

    // context: IContextWithReferences, read
    HashSet<T> InvokedBy { get; }

    // context: IContextWithReferences, read
    HashSet<T> Properties { get; }
}
