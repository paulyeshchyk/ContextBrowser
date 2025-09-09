using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UmlKit.Model;

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

        writer.WriteLine(WebUtility.HtmlEncode($"<style>"));

        writer.WriteLine(WebUtility.HtmlEncode($"mindmapDiagram {{"));

        foreach (var element in Elements.OrderBy(e => e.Key).Select(e => e.Value))
        {
            element.WriteTo(writer, writeOptions);
        }
        writer.WriteLine(WebUtility.HtmlEncode($"}}"));

        writer.WriteLine(WebUtility.HtmlEncode($"</style>"));
    }
}