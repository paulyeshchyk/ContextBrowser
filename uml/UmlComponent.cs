namespace ContextBrowser.uml;

// context: model, uml
public class UmlComponent : IUmlElement
{
    public string Name { get; }

    public string? Url { get; }

    public UmlComponent(string? name, string? url = null)
    {
        Name = name ?? "???";
        Url = url;
    }

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        if(!string.IsNullOrWhiteSpace(Url))
            writer.WriteLine($"component \"{Name}\" [[{Url}]]");
        else
            writer.WriteLine($"component \"{Name}\"");
    }
}
