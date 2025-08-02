using ContextKit.Model;
using UmlKit.Diagrams;
using UmlKit.Model;
using UmlKit.Model.Options;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public static class UmlContextMethodPerActionDomainDiagram
{
    //context: build, uml
    public static void Build(Dictionary<ContextContainer, List<string>> matrix, string outputPath, ContextTransitionDiagramBuilderOptions options)
    {
        var diagram = new UmlDiagramClasses(options);
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach(var cell in matrix)
        {
            var (action, domain) = cell.Key;
            var blockLabel = $"{action}_{domain}";
            var listOfClasses = cell.Value.Distinct();
            if(!listOfClasses.Any())
            {
                continue;
            }

            var package = new UmlPackage(blockLabel);

            foreach(var methodName in listOfClasses)
            {
                package.Add(new UmlComponent(methodName));
            }

            diagram.Add(package);
        }

        diagram.WriteToFile(outputPath);
    }
}