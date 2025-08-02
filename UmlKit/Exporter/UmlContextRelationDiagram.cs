using UmlKit.Diagrams;
using UmlKit.Model;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public static class UmlContextRelationDiagram
{
    //context: build, uml, links
    public static void GenerateLinksUml(HashSet<(string From, string To)> links, string outputPath)
    {
        var diagram = new UmlDiagramClasses();
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var (from, to) in links)
        {
            diagram.Add(new UmlRelation(from, to));
        }

        diagram.WriteToFile(outputPath);
    }
}