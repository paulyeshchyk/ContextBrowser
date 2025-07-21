using ContextBrowser.model;

namespace ContextBrowser.exporter.UmlSamples;

// context: uml, build
internal static class UmlActionPerDomainDiagram
{
    //context: build, uml, links, file, heatmap
    public static void Build(Dictionary<ContextContainer, List<string>> matrix, Func<string, string, string> linkGenerator, string outputPath)
    {
        var diagram = new UmlDiagram();
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
