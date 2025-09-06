using System.Collections.Generic;
using System.IO;
using System.Linq;
using UmlKit.PlantUmlSpecification.Attributes;
using static System.Net.Mime.MediaTypeNames;

namespace UmlKit.Model;

// context: model, uml
// pattern: Composite leaf
public class UmlClass : IUmlElement, IUmlDeclarable
{
    private const string SFakeClassName = "FakeClassName";

    public string Name { get; }

    public string Alias { get; }

    public List<IUmlElement> Elements { get; } = new();

    public string? Url { get; }

    public string Declaration => $"class \"{Name}\" as {Alias}";

    public UmlClass(string? name, string alias, string? url = null)
    {
        Name = name ?? SFakeClassName;
        Alias = alias;
        Url = url;
    }

    public void Add(IUmlElement e) => Elements.Add(e);

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        //List<string?> properties = new();
        //properties.Add($"{Declaration}");
        //properties.Add(MethodAttributesBuilder.BuildUrl(Url));
        //properties.Add($"{{");
        //
        //var result = string.Join(" ", properties.Cast<string>());

        writer.WriteLine();
        writer.WriteLine($"{Declaration} {ClassAttributesBuilder.BuildUrl(Url)}");
        writer.WriteLine("{");
        foreach (var element in Elements)
            element.WriteTo(writer);
        writer.WriteLine("}");
    }
}