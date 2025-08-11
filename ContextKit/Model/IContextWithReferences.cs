namespace ContextKit.Model;

public interface IContextWithReferences<T>
    where T : IContextWithReferences<T>
{
    Guid Uid { get; }

    string Name { get; set; }

    string? Action { get; set; }

    string SymbolName { get; set; }

    T? ClassOwner { get; set; }

    int SpanStart { get; set; }

    public string Namespace { get; set; }

    HashSet<string> Domains { get; }

    ContextInfoElementType ElementType { get; set; }

    HashSet<T> References { get; }

    HashSet<T> InvokedBy { get; }

    T? MethodOwner { get; set; }

    public ISymbolInfo? Symbol { get; set; }

    public ISyntaxNodeInfo? SyntaxNode { get; set; }
}