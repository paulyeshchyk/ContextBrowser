namespace ContextBrowser.uml;

// context: model, uml
public class UmlMethodBox : IUmlElement
{
    public string Name { get; }

    public string? Stereotype { get; }

    public UmlMethodBox(string? name, string? stereotype = null)
    {
        Name = name ?? "???";
        Stereotype = stereotype;
    }

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        if (!string.IsNullOrWhiteSpace(Stereotype))
            writer.WriteLine($"    [{Name}] <<{Stereotype}>>");
        else
            writer.WriteLine($"    [{Name}]");
    }
}
