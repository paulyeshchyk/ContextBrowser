using ContextBrowser.model;

namespace ContextBrowser.exporter.UmlSamples;

// context: uml, build
internal static class SampleLinkedDomain
{
    //context: build, uml, links
    public static void GenerateLinksUml(HashSet<(string From, string To)> links, string outputPath)
    {
        var diagram = new UmlDiagram();
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var (from, to) in links)
        {
            diagram.Add(new UmlRelation(from, to));
        }

        diagram.WriteToFile(outputPath);
    }
}
