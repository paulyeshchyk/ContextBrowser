using System.Collections.Generic;
using System.IO;
using System.Linq;
using UmlKit.Builders.Model;

namespace ExporterKit.DotDiagram;

public static class DotDiagramExporter
{
    private const string SDiGraphItem = "  \"{0}\" -> \"{1}\" [label=\"{2}\"];";

    public static void Export(IEnumerable<UmlTransitionDto> transitions, string filePath)
    {
        using var writer = new StreamWriter(filePath);

        writer.WriteLine("digraph G {");
        writer.WriteLine("  rankdir=LR;");

        foreach (var t in transitions.Distinct())
        {
            var digraphItem = string.Format(SDiGraphItem, t.CallerId, t.CalleeId, t.Domain);
            writer.WriteLine(digraphItem);
        }

        writer.WriteLine("}");
    }
}