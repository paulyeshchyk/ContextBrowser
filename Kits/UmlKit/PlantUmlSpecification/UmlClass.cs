using System.Collections.Generic;
using System.IO;

namespace UmlKit.Model;

// context: model, uml
// pattern: Composite leaf
public class UmlClass : IUmlElement, IUmlDeclarable
{
    private const string SFakeClassName = "FakeClassName";

    public string Name { get; }

    public List<IUmlElement> Elements { get; } = new();

    public string? Url { get; }

    public string Declaration => $"class \"{Name}\"";

    public string Alias => Name;

    public UmlClass(string? name, string? url = null)
    {
        Name = name ?? SFakeClassName;
        Url = url;
    }

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