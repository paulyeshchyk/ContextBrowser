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

    public string? Action { get; set; }

    public HashSet<string> Domains { get; } = new();

    public HashSet<ContextInfo> References { get; set; } = new();

    public Dictionary<string, string> Dimensions { get; set; } = new();

    public string FfullName
    {
        get
        {
            var lst = new List<string>();
            var ns = Namespace?.Trim() ?? "Global";
            if(!string.IsNullOrWhiteSpace(ns))
                lst.Add(ns);
            var owner = ClassOwner?.Name?.Trim() ?? string.Empty;
            if(!string.IsNullOrWhiteSpace(owner))
                lst.Add(owner);
            var name = Name?.Trim() ?? string.Empty;
            if(!string.IsNullOrWhiteSpace(name))
                lst.Add(name);
            var result = string.Join(".", lst);
            return result;
        }
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Namespace, ElementType);
    }

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(this, obj))
        {
            return true;
        }

        if(obj is null || GetType() != obj.GetType())
        {
            return false;
        }

        // 3. Приведение к нужному типу и сравнение по значениям ключевых полей
        ContextInfo other = (ContextInfo)obj;
        return Name == other.Name && // Сравнение строк
               Namespace == other.Namespace && // Сравнение строк
               ElementType == other.ElementType; // Сравнение перечислений
    }
}
