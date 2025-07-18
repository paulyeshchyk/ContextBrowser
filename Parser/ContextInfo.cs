namespace ContextBrowser.Parser;

public class ContextInfo
{
    public string? ElementType { get; set; } // class, method, etc.

    public string? Name { get; set; }

    public HashSet<string> Contexts { get; set; } = new();

    public string? Namespace { get; set; }

    public string? ClassOwner { get; set; }

    public HashSet<string> References { get; set; } = new();
}
