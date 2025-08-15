namespace ContextKit.Model;

// coverage: 255
// context: ContextInfo, model, IContextWithReferences
public record ContextInfo : IContextWithReferences<ContextInfo>
{
    public ContextInfoElementType ElementType { get; set; } = ContextInfoElementType.none;

    public string Name { get; set; } = "unknown_context_info_name";

    public string FullName { get; set; } = string.Empty;

    public HashSet<string> Contexts { get; set; } = new();

    public string Namespace { get; set; } = "Global";

    public IContextInfo? ClassOwner { get; set; }

    public IContextInfo? MethodOwner { get; set; }

    public string? Action { get; set; }

    public HashSet<string> Domains { get; } = new();

    public HashSet<ContextInfo> References { get; set; } = new();

    public HashSet<ContextInfo> InvokedBy { get; set; } = new();

    public HashSet<ContextInfo> Properties { get; set; } = new();

    public Dictionary<string, string> Dimensions { get; set; } = new();

    public int SpanStart { get; set; } = 0;

    public int SpanEnd { get; set; } = 0;

    public string Identifier { get; } = Guid.NewGuid().ToString();

    public ISymbolInfo? Symbol { get; set; }

    public ISyntaxNodeInfo? SyntaxNode { get; set; }

    public override int GetHashCode() => FullName.GetHashCode();

    public virtual bool Equals(ContextInfo? obj) => obj is ContextInfo other && FullName.Equals(other.FullName);

    public ContextInfo(ContextInfoElementType elementType, string identifier, string name, string fullName, string nameSpace, int spanStart, int spanEnd, ISymbolInfo? symbol, ISyntaxNodeInfo? syntaxNode, IContextInfo? classOwner, IContextInfo? methodOwner)
    {
        ElementType = elementType;
        Identifier = identifier;
        Name = name;
        FullName = fullName;
        Namespace = nameSpace;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
        Symbol = symbol;
        SyntaxNode = syntaxNode;
        ClassOwner = classOwner;
        MethodOwner = methodOwner;
    }
}
