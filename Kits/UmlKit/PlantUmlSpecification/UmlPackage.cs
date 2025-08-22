namespace UmlKit.Model;

// context: model, uml
// pattern: Composite leaf
public class UmlPackage : IUmlElement
{
    public string Name { get; }

    public List<IUmlElement> Elements { get; } = new();

    public UmlPackage(string name) => Name = name;

    // context: uml, create
    public void Add(IUmlElement e) => Elements.Add(e);

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine();
        writer.WriteLine($"package \"{Name}\" {{");
        foreach (var element in Elements)
            element.WriteTo(writer);
        writer.WriteLine("}");
    }
}