using ContextBrowserKit.Extensions;
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
        var diagramId = $"relation_{outputPath}".AlphanumericOnly();
        var diagram = new UmlClassDiagram(options, diagramId: diagramId);
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var (from, to) in links)
        {
            AddRelation(options, diagram, from, to);
        }

        diagram.WriteToFile(outputPath);
    }

    private static void AddRelation(DiagramBuilderOptions options, UmlClassDiagram diagram, string from, string to)
    {
        var arrow = new UmlArrow(flowType: options.Indication.UseAsync ? UmlArrowFlowType.Async : UmlArrowFlowType.Sync);
        var relation = new UmlRelation(from, to, arrow);
        diagram.Add(relation);
    }
}