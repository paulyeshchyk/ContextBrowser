using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public static class UmlContextComponentDiagram
{
    //context: uml, build, heatmap, directory
    public static void Build(ExportOptions exportOptions, KeyValuePair<ContextContainer, List<string>> cell, DiagramBuilderOptions options)
    {
        var (action, domain) = cell.Key;
        var methods = cell.Value.Distinct().ToList();
        var fileName = ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.puml, $"composite_{action}_{domain}.puml");

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
    public static void Build(Dictionary<ContextContainer, List<string>> matrix, ExportOptions exportOptions, DiagramBuilderOptions options)
    {
        foreach(var cell in matrix)
        {
            Build(exportOptions, cell, options);
        }
    }
}