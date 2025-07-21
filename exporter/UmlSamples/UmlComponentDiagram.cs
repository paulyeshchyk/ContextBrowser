using ContextBrowser.model;

namespace ContextBrowser.exporter.UmlSamples;

// context: uml, build
internal static class UmlComponentDiagram
{
    //context: build, heatmap, uml, directory
    public static void Build(string targetDirectory, KeyValuePair<ContextContainer, List<string>> cell)
    {
        var (action, domain) = cell.Key;
        var methods = cell.Value.Distinct().ToList();
        var fileName = Path.Combine(targetDirectory, $"composite_{action}_{domain}.puml");

        var diagram = new UmlDiagram();
        diagram.SetTitle($"{action.ToUpper()} → {domain}");
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach(var method in methods)
        {
            diagram.Add(new UmlComponent(method));
        }

        diagram.WriteToFile(fileName);
    }

    public static void Build(Dictionary<ContextContainer, List<string>> matrix, string targetDirectory)
    {
        foreach(var cell in matrix)
        {
            UmlComponentDiagram.Build(targetDirectory, cell);
        }
    }
}