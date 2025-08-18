using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public static class UmlContextRelationDiagram
{
    //context: build, uml, links
    public static void GenerateLinksUml(HashSet<(string From, string To)> links, string outputPath, DiagramBuilderOptions options)
    {
        var diagram = new UmlDiagramClasses(options);
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var (from, to) in links)
        {
            diagram.Add(new UmlRelation(from, to, new UmlArrow(flowType: options.Indication.UseAsync ? UmlArrowFlowType.Async : UmlArrowFlowType.Sync)));
        }

        diagram.WriteToFile(outputPath);
    }
}