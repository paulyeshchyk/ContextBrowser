using System.Collections.Generic;
using System.IO;
using System.Linq;
using UmlKit.PlantUmlSpecification.Attributes;

namespace UmlKit.PlantUmlSpecification;

// context: model, uml
// pattern: Composite leaf
public class UmlPackage : IUmlElement, IUmlDeclarable, IUmlElementCollection
{
    public string Name { get; }

    public string Alias { get; }

    public string? Url { get; }

    public string Declaration => $"package \"{Name}\" as {Alias} ";

    public SortedList<int, IUmlElement> Elements { get; } = new();

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
    public void Add(IUmlElement e) => Elements.Add(Elements.Count, e);

    // context: uml, share
    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
    {
        writer.WriteLine();
        writer.WriteLine($"{Declaration} {GetUrl()}");
        writer.WriteLine("{");
        foreach (var element in Elements.OrderBy(e => e.Key).Select(e => e.Value))
            element.WriteTo(writer, writeOptions);
        writer.WriteLine("}");
    }
}
