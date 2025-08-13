namespace UmlKit.Model;

// context: model, uml
// pattern: Composite leaf
public class UmlMethodBox : IUmlElement
{
    private const string SFakeMethodName = "FakeMethodName";

    public string Name { get; }

    public string? Stereotype { get; }

    public UmlMethodBox(string? name, string? stereotype = null)
    {
        Name = name ?? SFakeMethodName;
        Stereotype = stereotype;
    }

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine();
        if(!string.IsNullOrWhiteSpace(Stereotype))
            writer.WriteLine($"    [{Name}] <<{Stereotype}>>");
        else
            writer.WriteLine($"    [{Name}]");
    }
}