namespace ContextKit.Model;

public record ContextInfoDto : IContextInfo
{
    public ContextInfoElementType ElementType { get; set; }

    public string FullName { get; set; }

    public string Identifier { get; }

    public string Name { get; set; }

    public string Namespace { get; set; }

    public int SpanEnd { get; set; }

    public int SpanStart { get; set; }

    public ISymbolInfo? Symbol { get; set; }

    public ISyntaxNodeInfo? SyntaxNode { get; set; }

    public IContextInfo? ClassOwner { get; set; }

    public IContextInfo? MethodOwner { get; set; }

    public ContextInfoDto(
        ContextInfoElementType elementType,
        string fullName,
        string name,
        string nameSpace,
        string identifier,
        int spanStart,
        int spanEnd,
        IContextInfo? classOwner = default,
        IContextInfo? methodOwner = default,
        ISymbolInfo? symbol = default,
        ISyntaxNodeInfo? syntaxNode = default)
    {
        ElementType = elementType;
        FullName = fullName;
        Name = name;
        Namespace = nameSpace;
        Identifier = identifier;
        SpanEnd = spanEnd;
        SpanStart = spanStart;
        ClassOwner = classOwner;
        MethodOwner = methodOwner;
        Symbol = symbol;
        SyntaxNode = syntaxNode;
    }
}