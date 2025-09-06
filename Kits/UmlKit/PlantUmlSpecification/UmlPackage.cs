using System.Collections.Generic;
using System.IO;
using System.Linq;
using UmlKit.PlantUmlSpecification.Attributes;

namespace UmlKit.Model;

// context: model, uml
// pattern: Composite leaf
public class UmlPackage : IUmlElement, IUmlDeclarable
{
    public string Name { get; }

    public string Alias { get; }

    public string? Url { get; }

    public string Declaration => $"package \"{Name}\" as {Alias} ";

    public List<IUmlElement> Elements { get; } = new();

    public UmlPackage(string name, string alias, string? url)
    {
        Name = name;
        Alias = alias;
        Url = url;
    }

    private string GetUrl()
    {
        return ClassAttributesBuilder.BuildUrl(Url) ?? string.Empty;
    }

    public UmlComponent? FindElement(string predicate)
    {
        return Elements.Cast<UmlComponent>().FirstOrDefault(e => e.Name.Equals(predicate));
    }

    // context: uml, create
    public void Add(IUmlElement e) => Elements.Add(e);

    // context: uml, share
    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine();
        writer.WriteLine($"{Declaration} {GetUrl()}");
        writer.WriteLine("{");
        foreach (var element in Elements)
            element.WriteTo(writer);
        writer.WriteLine("}");
    }
}
