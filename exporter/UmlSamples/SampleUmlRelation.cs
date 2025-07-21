using ContextBrowser.model;

namespace ContextBrowser.exporter.UmlSamples;

// context: uml, build
internal static class UmlSample
{
    //context: build, heatmap, uml, directory
    public static void GeneratePerCellDiagrams(Dictionary<ContextContainer, List<string>> matrix, string targetDirectory)
    {
        foreach (var cell in matrix)
        {
            var (action, domain) = cell.Key;
            var methods = cell.Value.Distinct().ToList();
            var fileName = Path.Combine(targetDirectory, $"composite_{action}_{domain}.puml");

            var diagram = new UmlDiagram();
            diagram.SetTitle($"{action.ToUpper()} → {domain}");
            diagram.SetSkinParam("componentStyle", "rectangle");

            foreach (var method in methods)
            {
                diagram.Add(new UmlComponent(method));
            }

            diagram.WriteToFile(fileName);
        }
    }
}