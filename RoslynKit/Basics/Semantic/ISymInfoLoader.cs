using ContextKit.Model;

namespace RoslynKit.Basics.Semantic;

public record SymInfoDto : ISymInfoLoader
{
    public ISymbolInfo? SymInfo { get; }

    public ISyntaxNodeInfo? NodeInfo { get; }

    public ContextInfoElementType Kind { get; }

    public string FullName { get; }

    public string Name { get; }

    public string Namespace { get; }

    public int SpanEnd { get; }

    public int SpanStart { get; }

    public string Identifier { get; }

    public SymInfoDto(ContextInfoElementType kind, string fullName, string name, string @namespace, string identifier, int spanStart, int spanEnd, ISymbolInfo? symInfo = default, ISyntaxNodeInfo? nodeInfo = default)
    {
        SymInfo = symInfo;
        NodeInfo = nodeInfo;
        Kind = kind;
        FullName = fullName;
        Name = name;
        Namespace = @namespace;
        SpanEnd = spanEnd;
        SpanStart = spanStart;
        Identifier = identifier;
    }
}

public interface ISymInfoLoader
{
    ISymbolInfo? SymInfo { get; }

    ISyntaxNodeInfo? NodeInfo { get; }

    ContextInfoElementType Kind { get; }

    string FullName { get; }

    string Name { get; }

    string Namespace { get; }

    int SpanEnd { get; }

    int SpanStart { get; }

    string Identifier { get; }
}
