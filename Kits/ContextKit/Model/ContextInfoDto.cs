namespace ContextKit.Model;

public record ContextInfoDto : IContextInfo
{
    public ContentInfoElementVisibility ElementVisibility { get; set; }

    public ContextInfoElementType ElementType { get; set; }

    public string FullName { get; set; }

    public string Identifier { get; }

    public string Name { get; set; }

    public string ShortName { get; set; }

    public string Namespace { get; set; }

    public int SpanEnd { get; set; }

    public int SpanStart { get; set; }

    public ISymbolInfo? SymbolWrapper { get; set; }

    public ISyntaxNodeWrapper? SyntaxWrapper { get; set; }

    public IContextInfo? ClassOwner { get; set; }

    public IContextInfo? MethodOwner { get; set; }

    public bool MethodOwnedByItSelf
    {
        get
        {
            if (MethodOwner != null)
            {
                return MethodOwner.FullName.Equals(FullName);
            }
            return false;
        }
    }

    public string NameWithClassOwnerName
    {
        get
        {
            return ElementType == ContextInfoElementType.method && !string.IsNullOrWhiteSpace(ClassOwner?.Name)
                ? $"{ClassOwner?.Name}.{Name}"
                : $"{Name}";
        }
    }

    public ContextInfoDto(
        ContextInfoElementType elementType,
        ContentInfoElementVisibility elementVisibility,
        string fullName,
        string name,
        string shortName,
        string nameSpace,
        string identifier,
        int spanStart = -1,
        int spanEnd = -1,
        IContextInfo? classOwner = default,
        IContextInfo? methodOwner = default,
        ISymbolInfo? symbolWrapper = default,
        ISyntaxNodeWrapper? syntaxWrapper = default)
    {
        ElementType = elementType;
        ElementVisibility = elementVisibility;
        FullName = fullName;
        Name = name;
        ShortName = name;
        Namespace = nameSpace;
        Identifier = identifier;
        SpanEnd = (spanEnd != -1) ? spanEnd : (syntaxWrapper?.SpanEnd ?? -1);
        SpanStart = (spanStart != -1) ? spanStart : (syntaxWrapper?.SpanStart ?? -1);
        ClassOwner = classOwner;
        MethodOwner = methodOwner;
        SymbolWrapper = symbolWrapper;
        SyntaxWrapper = syntaxWrapper;
    }
}