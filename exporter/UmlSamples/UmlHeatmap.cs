using ContextBrowser.model;

namespace ContextBrowser.exporter.UmlSamples;

// context: uml, build
internal static class UmlHeatmap
{
    //context: build, uml, links, file, heatmap
    public static void GenerateHeatmapUmlWithLinks(Dictionary<ContextContainer, List<string>> matrix, Func<string, string, string> linkGenerator, string outputPath)
    {
        var diagram = new UmlDiagram();
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var cell in matrix)
        {
            var (action, domain) = cell.Key;
            var count = cell.Value.Count;
            var packageId = $"{action}_{domain}";
            var label = $"{packageId}\\nMethods: {count}";
            var url = linkGenerator(action, domain);

            var package = new UmlPackage(packageId);
            package.Add(new UmlComponent(label, url));
            diagram.Add(package);
        }

        diagram.WriteToFile(outputPath);
    }

    //context: build, uml, file, heatmap
    public static void GenerateHeatmapUml(Dictionary<ContextContainer, List<string>> matrix, string outputPath)
    {
        var diagram = new UmlDiagram();
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var cell in matrix)
        {
            var (action, domain) = cell.Key;
            var count = cell.Value.Count;
            var packageName = $"{action}_{domain}";
            var label = $"{packageName}\\nMethods: {count}";

            var package = new UmlPackage(packageName);
            package.Add(new UmlComponent(label));
            diagram.Add(package);
        }

        diagram.WriteToFile(outputPath);
    }
}
