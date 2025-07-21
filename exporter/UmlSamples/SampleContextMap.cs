using ContextBrowser.model;

namespace ContextBrowser.exporter.UmlSamples;

// context: uml, build
internal static class SampleContextMap
{
    //context: build, uml
    public static void GenerateMethodsUml(Dictionary<ContextContainer, List<string>> matrix, string outputPath)
    {
        var diagram = new UmlDiagram();
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var cell in matrix)
        {
            var (action, domain) = cell.Key;
            var blockLabel = $"{action}_{domain}";
            var package = new UmlPackage(blockLabel);

            foreach (var methodName in cell.Value.Distinct())
            {
                package.Add(new UmlComponent(methodName));
            }

            diagram.Add(package);
        }

        diagram.WriteToFile(outputPath);
    }
}
