namespace ContextBrowser.Parser;

// coverage: 255
// context: ContextInfo, model
public class ContextInfo
{
    public string? ElementType { get; set; } // class, method, etc.

    public string? Name { get; set; }

    public HashSet<string> Contexts { get; set; } = new();

    public string? Namespace { get; set; }

    public string? ClassOwner { get; set; }

    public HashSet<string> References { get; set; } = new();

    public Dictionary<string, string> Dimensions { get; set; } = new();
}

public static class ContextInfoExtensions
{
    // context: read, dimension, ContextInfo
    public static int GetDimensionIntValue(this ContextInfo contextInfo, string dimensionName)
    {
        if(contextInfo.Dimensions.TryGetValue(dimensionName, out var raw))
        {
            if(int.TryParse(raw, out var v))
            {
                return v;
            }
            return 0;
        }
        return 0;
    }
}