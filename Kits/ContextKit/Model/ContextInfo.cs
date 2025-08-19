using System.Text.Json.Serialization;

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

    public HashSet<string> Domains { get; set; } = new();

    [JsonIgnore]
    public HashSet<ContextInfo> References { get; set; } = new();

    [JsonIgnore]
    public HashSet<ContextInfo> InvokedBy { get; set; } = new();

    [JsonIgnore]
    public HashSet<ContextInfo> Properties { get; set; } = new();

    public Dictionary<string, string> Dimensions { get; set; } = new();

    public int SpanStart { get; set; } = 0;

    public int SpanEnd { get; set; } = 0;

    public string Identifier { get; } = Guid.NewGuid().ToString();

    [JsonIgnore]
    public ISymbolInfo? Symbol { get; set; }

    [JsonIgnore]
    public ISyntaxNodeInfo? SyntaxNode { get; set; }

    public override int GetHashCode() => FullName.GetHashCode();

    public virtual bool Equals(ContextInfo? obj) => obj is ContextInfo other && FullName.Equals(other.FullName);

    public ContextInfo(
        ContextInfoElementType elementType,
        string identifier,
        string name,
        string fullName,
        string nameSpace,
        int spanStart,
        int spanEnd,
        string? action = null,
        HashSet<string>? domains = null,
        Dictionary<string, string>? dimensions = null,
        HashSet<string>? contexts = null,
        ISymbolInfo? symbol = null,
        ISyntaxNodeInfo? syntaxNode = null,
        IContextInfo? classOwner = null,
        IContextInfo? methodOwner = null)
    {
        ElementType = elementType;
        Identifier = identifier;
        Name = symbol?.GetShortestName() ?? name;
        FullName = symbol?.GetFullName() ?? fullName;
        Namespace = symbol?.GetNameSpace() ?? nameSpace;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
        Symbol = symbol;
        SyntaxNode = syntaxNode;
        ClassOwner = classOwner;
        MethodOwner = methodOwner;
        Contexts = contexts ?? new();
        Action = action;
        Domains = domains ?? new();
        Dimensions = dimensions ?? new();
    }
}
