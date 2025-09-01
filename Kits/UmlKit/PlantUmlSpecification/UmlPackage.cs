using System.Collections.Generic;
using System.IO;

namespace UmlKit.Model;

// context: model, uml
// pattern: Composite leaf
public class UmlPackage : IUmlElement, IUmlDeclarable
{
    public string Name { get; }

    public List<IUmlElement> Elements { get; } = new();

    public UmlPackage(string name) => Name = name;

    public string Declaration => $"package \"{Name}\"";

    public string Alias => Name;

    // context: uml, create
    public void Add(IUmlElement e) => Elements.Add(e);

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine();
        writer.WriteLine($"{Declaration} {{");
        foreach (var element in Elements)
            element.WriteTo(writer);
        writer.WriteLine("}");
    }
}
