namespace ContextKit.Model;

// coverage: 255
// context: ContextInfo, model
public record ContextInfo : IContextWithReferences<ContextInfo>
{
    public ContextInfoElementType ElementType { get; set; } = ContextInfoElementType.none;

    public string Name { get; set; } = "unknown_context_info_name";

    public string SymbolName { get; set; } = string.Empty;

    public HashSet<string> Contexts { get; set; } = new();

    public string Namespace { get; set; } = "Global";

    private ContextInfo? _classOwner { get; set; }

    public ContextInfo? ClassOwner
    {
        get { return _classOwner; }
        set { _classOwner = value; }
    }

    private ContextInfo? _methodOwner;

    public ContextInfo? MethodOwner
    {
        get { return _methodOwner; }
        set { _methodOwner = value; }
    }

    public string? Action { get; set; }

    public HashSet<string> Domains { get; } = new();

    public HashSet<ContextInfo> References { get; set; } = new();

    public HashSet<ContextInfo> InvokedBy { get; set; } = new();

    public Dictionary<string, string> Dimensions { get; set; } = new();

    public int SpanStart { get; set; } = 0;

    public int SpanEnd { get; set; } = 0;

    public Guid Uid { get; } = Guid.NewGuid();

    public ISymbolInfo? Symbol { get; set; }

    public ISyntaxNodeInfo? SyntaxNode { get; set; }

    public override int GetHashCode() => SymbolName.GetHashCode();

    public virtual bool Equals(ContextInfo? obj) => obj is ContextInfo other && SymbolName.Equals(other.SymbolName);
}
