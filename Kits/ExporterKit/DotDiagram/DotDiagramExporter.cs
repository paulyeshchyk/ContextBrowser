using System.Collections.Generic;
using System.IO;
using System.Linq;
using UmlKit.Builders.Model;

namespace ExporterKit.DotDiagram;

public static class DotDiagramExporter
{
    public static void Export(IEnumerable<UmlTransitionDto> transitions, string filePath)
    {
        using var writer = new StreamWriter(filePath);

        writer.WriteLine("digraph G {");
        writer.WriteLine("  rankdir=LR;");

        foreach (var t in transitions.Distinct())
        {
            writer.WriteLine($"  \"{t.CallerId}\" -> \"{t.CalleeId}\" [label=\"{t.Domain}\"];");
        }

        writer.WriteLine("}");
    }
}