namespace ContextBrowser.ContextKit.Model;

// coverage: 255
// context: ContextInfo, model
public class ContextInfo : IContextWithReferences<ContextInfo>
{
    public ContextInfoElementType ElementType { get; set; } = ContextInfoElementType.none;

    public string? Name { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public HashSet<string> Contexts { get; set; } = new();

    public string? Namespace { get; set; }

    public ContextInfo? ClassOwner { get; set; }

    public ContextInfo? MethodOwner { get; set; }

    public string? Action { get; set; }

    public HashSet<string> Domains { get; } = new();

    public HashSet<ContextInfo> References { get; set; } = new();

    public HashSet<ContextInfo>? InvokedBy { get; set; } = null;

    public Dictionary<string, string> Dimensions { get; set; } = new();

    public string DisplayNameAlphaNumericOnly
    {
        get
        {
            var nsName = string.IsNullOrWhiteSpace(Namespace) ? string.Empty : Namespace;
            var thename = string.IsNullOrWhiteSpace(Name) ? string.Empty : Name;
            return string.Join(".", new[] { nsName, thename });
        }
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Namespace, ElementType);
    }

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(this, obj))
            return true;
        if(obj is null || GetType() != obj.GetType())
            return false;

        ContextInfo other = (ContextInfo)obj;
        return Name == other.Name &&
               Namespace == other.Namespace &&
               ElementType == other.ElementType;
    }
}
