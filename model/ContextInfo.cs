namespace ContextBrowser.model;

// coverage: 255
// context: ContextInfo, model
public class ContextInfo
{
    public ContextInfoElementType ElementType { get; set; } = ContextInfoElementType.none;

    public string? Name { get; set; }

    public HashSet<string> Contexts { get; set; } = new();

    public string? Namespace { get; set; }

    public string? ClassOwner { get; set; }

    public HashSet<string> References { get; set; } = new();

    public Dictionary<string, string> Dimensions { get; set; } = new();
}

// context: ContextInfo, model
public enum ContextInfoElementType
{
    @none,
    @method,
    @class,
    @struct,
    @record,
    @enum,
    @namespace
}

// context: read, dimension, ContextInfo
public static class ContextInfoExtensions
{
    // context: read, dimension, ContextInfo
    public static int GetDimensionIntValue(this ContextInfo contextInfo, string dimensionName)
    {
        if (contextInfo.Dimensions.TryGetValue(dimensionName, out var raw))
        {
            if (int.TryParse(raw, out var v))
            {
                return v;
            }
            return 0;
        }
        return 0;
    }
}
