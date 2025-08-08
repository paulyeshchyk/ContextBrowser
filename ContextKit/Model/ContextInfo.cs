using Microsoft.CodeAnalysis;

namespace ContextKit.Model;

// coverage: 255
// context: ContextInfo, model
public class ContextInfo : IContextWithReferences<ContextInfo>
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

    private SortedList<int, ContextInfo> References { get; set; } = new();

    private HashSet<ContextInfo> InvokedBy { get; set; } = new();

    public Dictionary<string, string> Dimensions { get; set; } = new();

    public int SpanStart { get; set; } = 0;

    public int SpanEnd { get; set; } = 0;

#warning move this out of model
    public ISymbol? Symbol { get; set; }

    public SyntaxNode? SyntaxNode { get; set; }

    HashSet<ContextInfo> IContextWithReferences<ContextInfo>.InvokedBy => InvokedBy;

    SortedList<int, ContextInfo> IContextWithReferences<ContextInfo>.References => References;

    public override int GetHashCode() => SymbolName.GetHashCode();

    public override bool Equals(object? obj) => obj is ContextInfo other && SymbolName.Equals(other.SymbolName);

    public string GetDebugName()
    {
        if(Symbol != null)
        {
            return Symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        }
        return $"{(MethodOwner?.Name ?? string.Empty)}.{Name}";
    }


    public void AddToReferences(ContextInfo obj)
    {
        var index = References.Count;
        References.Add(index, obj);
    }

    public void AddToInvokedBy(ContextInfo obj)
    {
        InvokedBy.Add(obj);
    }

    public SortedList<int, ContextInfo> GetReferences()
    {
        return References;
    }

    public IEnumerable<ContextInfo> GetReferencesSortedByInvocation()
    {
        return References.OrderBy(t => t.Key).Select(t => t.Value);
    }

    public HashSet<ContextInfo> GetInvokedBy()
    {
        return InvokedBy;
    }
}