using ContextKit.Model;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public static class UmlContextActionPerDomainDiagram
{
    //context: build, uml, links, heatmap
    public static void Build(Dictionary<ContextContainer, List<string>> matrix, Func<string, string, string> linkGenerator, string outputPath, DiagramBuilderOptions options)
    {
        var diagram = new UmlDiagramClasses(options);
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach(var cell in matrix)
        {
            var (action, domain) = cell.Key;
            var count = cell.Value.Count;
            if(count == 0)
                continue;

            var packageId = $"{action}_{domain}";
            var label = $"{packageId}\\nMethods: {count}";
            var url = linkGenerator(action, domain);

            var package = new UmlPackage(packageId);
            package.Add(new UmlComponent(label, url));
            diagram.Add(package);
        }

        diagram.WriteToFile(outputPath);
    }
}