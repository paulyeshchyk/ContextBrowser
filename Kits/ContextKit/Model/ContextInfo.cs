using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ContextKit.Model;

// coverage: 255
// context: ContextInfo, model, IContextWithReferences
public record ContextInfo : IContextWithReferences<ContextInfo>
{
    public ContextInfoElementType ElementType { get; set; } = ContextInfoElementType.none;

    public string Identifier { get; } = Guid.NewGuid().ToString();

    public string Namespace { get; set; }

    public string FullName { get; set; }

    public string Name { get; set; }

    public string ShortName { get; set; }

    public HashSet<string> Contexts { get; set; } = new();

    public IContextInfo? ClassOwner { get; set; }

    public IContextInfo? MethodOwner { get; set; }

    public string? Action { get; set; }

    public HashSet<string> Domains { get; set; } = new();

    [JsonIgnore]
    public HashSet<ContextInfo> References { get; set; } = new();

    [JsonIgnore]
    public HashSet<ContextInfo> InvokedBy { get; set; } = new();

    [JsonIgnore]
    public HashSet<ContextInfo> Owns { get; set; } = new();

    [JsonIgnore]
    public HashSet<ContextInfo> Properties { get; set; } = new();

    public Dictionary<string, string> Dimensions { get; set; } = new();

    public int SpanStart { get; set; } = 0;

    public int SpanEnd { get; set; } = 0;

    [JsonIgnore]
    public ISymbolInfo? SymbolWrapper { get; set; }

    [JsonIgnore]
    public ISyntaxNodeWrapper? SyntaxWrapper { get; set; }

    public override int GetHashCode() => FullName.GetHashCode();

    public string NameWithClassOwnerName
    {
        get
        {
            return ElementType == ContextInfoElementType.method && !string.IsNullOrWhiteSpace(ClassOwner?.Name)
                ? $"{ClassOwner?.Name}.{Name}"
                : $"{Name}";
        }
    }

    public virtual bool Equals(ContextInfo? obj) => obj is ContextInfo other && FullName.Equals(other.FullName);

    public ContextInfo(
        ContextInfoElementType elementType,
        string identifier,
        string name,
        string fullName,
        string shortName,
        string nameSpace,
        int spanStart,
        int spanEnd,
        string? action = null,
        HashSet<string>? domains = null,
        Dictionary<string, string>? dimensions = null,
        HashSet<string>? contexts = null,
        ISymbolInfo? symbolInfo = null,
        ISyntaxNodeWrapper? syntaxNode = null,
        IContextInfo? classOwner = null,
        IContextInfo? methodOwner = null)
    {
        ElementType = elementType;
        Identifier = identifier;
        Namespace = symbolInfo?.Namespace ?? nameSpace;
        FullName = symbolInfo?.GetFullName() ?? fullName;
        Name = symbolInfo?.GetName() ?? name;
        ShortName = symbolInfo?.GetShortName() ?? shortName;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
        SymbolWrapper = symbolInfo;
        SyntaxWrapper = syntaxNode;
        ClassOwner = classOwner;
        MethodOwner = methodOwner;
        Contexts = contexts ?? new();
        Action = action;
        Domains = domains ?? new();
        Dimensions = dimensions ?? new();
    }
}
