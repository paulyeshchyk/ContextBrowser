namespace UmlKit.Model;

// context: model, uml
// pattern: Composite leaf
public class UmlComponent : IUmlElement, IUmlDeclarable
{
    private const string SFakeComponentName = "FakeComponentName";

    public string Name { get; }

    public string? Url { get; }

    public string Declaration => $"component \"{Name}\"";

    public string Alias => Name;

    public UmlComponent(string? name, string? url = null)
    {
        Name = name ?? SFakeComponentName;
        Url = url;
    }

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine();
        if (!string.IsNullOrWhiteSpace(Url))
            writer.WriteLine($"component \"{Name}\" [[{Url}]]");
        else
            writer.WriteLine(Declaration);
    }
}