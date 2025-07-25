namespace ContextBrowser.ContextKit.Model;

public interface IContextWithReferences<T>
        where T : IContextWithReferences<T>
{
    public string? Name { get; set; }

    public string? Action { get; set; }

    public string FfullName { get; }

    public string DisplayName { get; set; }

    public HashSet<string> Domains { get; }

    HashSet<T> References { get; }
}
