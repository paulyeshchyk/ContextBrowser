using ContextBrowser.model;

namespace ContextBrowser.exporter.UmlSamples;

// context: uml, build
internal static class UmlMethodPerActionDomainDiagram
{
    //context: build, uml
    public static void Build(Dictionary<ContextContainer, List<string>> matrix, string outputPath)
    {
        var diagram = new UmlDiagram();
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var cell in matrix)
        {
            var (action, domain) = cell.Key;
            var blockLabel = $"{action}_{domain}";
            var listOfClasses = cell.Value.Distinct();
            if (!(listOfClasses.Any()))
            {
                continue;
            }

            var package = new UmlPackage(blockLabel);

            foreach (var methodName in listOfClasses)
            {
                package.Add(new UmlComponent(methodName));
            }

            diagram.Add(package);
        }

        diagram.WriteToFile(outputPath);
    }
}
