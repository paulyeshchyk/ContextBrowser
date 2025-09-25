using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UmlKit.PlantUmlSpecification;

public class UmlStyleDiagramMindmap : IUmlElement, IUmlElementCollection
{
    public SortedList<int, IUmlElement> Elements { get; private set; } = new SortedList<int, IUmlElement>() { };

    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
    {
        if (!Elements.Any())
        {
            return;
        }

        writer.WriteLine($"<style>");

        writer.WriteLine($"mindmapDiagram {{");

        foreach (var element in Elements.OrderBy(e => e.Key).Select(e => e.Value))
        {
            element.WriteTo(writer, writeOptions);
        }
        writer.WriteLine($"}}");

        writer.WriteLine($"</style>");
    }
}