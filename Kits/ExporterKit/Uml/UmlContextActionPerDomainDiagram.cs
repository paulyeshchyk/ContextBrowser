using ContextBrowserKit.Extensions;
using ContextKit.Model.Matrix;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public static class UmlContextActionPerDomainDiagram
{
    //context: build, uml, links, heatmap
    public static void Build(IContextInfoMatrix matrix, Func<string, string, string> linkGenerator, string outputPath, DiagramBuilderOptions options)
    {
        var diagramId = $"actionPerDomain_{outputPath}".AlphanumericOnly();

        var diagram = new UmlDiagramClasses(options, diagramId: diagramId);
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach(var item in matrix)
        {
            var count = item.Value.Count;
            if(count == 0)
                continue;

            AddPackage(linkGenerator, diagram, item.Key, count);
        }

        diagram.WriteToFile(outputPath);
    }

    private static void AddPackage(Func<string, string, string> linkGenerator, UmlDiagramClasses diagram, ContextInfoMatrixCell cell, int count)
    {
        var packageId = $"{cell.Action}_{cell.Domain}";
        var label = $"{packageId}\\nMethods: {count}";
        var url = linkGenerator(cell.Action, cell.Domain);

        var package = new UmlPackage(packageId);
        package.Add(new UmlComponent(label, url));
        diagram.Add(package);
    }
}