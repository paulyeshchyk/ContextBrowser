using ContextBrowserKit.Extensions;
using ContextKit.Model.Matrix;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public static class UmlContextMethodPerActionDomainDiagram
{
    //context: build, uml
    public static void Build(IContextInfoData matrix, string outputPath, DiagramBuilderOptions options)
    {
        var diagramId = $"method_per_action_domain_{outputPath}".AlphanumericOnly();
        var diagram = new UmlClassDiagram(options, diagramId: diagramId);
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var cell in matrix)
        {
            var (action, domain) = cell.Key;
            var blockLabel = $"{action}_{domain}";
            var listOfClasses = cell.Value.Distinct();
            if (!listOfClasses.Any())
            {
                continue;
            }

            AddPackage(diagram, blockLabel, listOfClasses);
        }

        diagram.WriteToFile(outputPath);
    }

    private static void AddPackage(UmlClassDiagram diagram, string blockLabel, IEnumerable<ContextKit.Model.ContextInfo> listOfClasses)
    {
        var package = new UmlPackage(blockLabel);

        foreach (var methodName in listOfClasses)
        {
            package.Add(new UmlComponent(methodName.FullName));
        }

        diagram.Add(package);
    }
}