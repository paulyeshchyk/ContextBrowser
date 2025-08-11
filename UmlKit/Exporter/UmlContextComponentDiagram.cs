using ContextKit.Model;
using UmlKit.Diagrams;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public static class UmlContextComponentDiagram
{
    //context: uml, build, heatmap, directory
    public static void Build(string targetDirectory, KeyValuePair<ContextContainer, List<string>> cell, ContextTransitionDiagramBuilderOptions options)
    {
        var (action, domain) = cell.Key;
        var methods = cell.Value.Distinct().ToList();
        var fileName = Path.Combine(targetDirectory, $"composite_{action}_{domain}.puml");

        var diagram = new UmlDiagramClasses(options);
        diagram.SetTitle($"{action.ToUpper()} -> {domain}");
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach(var method in methods)
        {
            diagram.Add(new UmlComponent(method));
        }

        diagram.WriteToFile(fileName);
    }

    //context: uml, build, heatmap, directory
    public static void Build(Dictionary<ContextContainer, List<string>> matrix, string targetDirectory, ContextTransitionDiagramBuilderOptions options)
    {
        foreach(var cell in matrix)
        {
            Build(targetDirectory, cell, options);
        }
    }
}