using System.Collections.Generic;
using System.IO;

namespace UmlKit.Model;

// context: model, uml
// pattern: Composite leaf
public class UmlComponentGroup : IUmlElement
{
    private const string SFakeComponentGroupName = "FakeComponentGroupName";

    public string Name { get; }

    public string Stereotype { get; }

    public List<IUmlElement> Members { get; } = new();

    public UmlComponentGroup(string? name, string stereotype)
    {
        Name = name ?? SFakeComponentGroupName;
        Stereotype = stereotype;
    }

    // context: uml, create
    public void Add(IUmlElement e) => Members.Add(e);

    // context: uml, share
    public void WriteTo(TextWriter writer, int alignNameMaxWidth)
    {
        writer.WriteLine();
        writer.WriteLine($"  component \"{Name}\" <<{Stereotype}>> {{");
        foreach (var member in Members)
            member.WriteTo(writer, alignNameMaxWidth);
        writer.WriteLine("  }");
    }
}