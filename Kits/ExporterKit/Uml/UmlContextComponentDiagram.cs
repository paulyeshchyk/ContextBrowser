using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public static class UmlContextComponentDiagram
{
    //context: uml, build, heatmap, directory
    public static void Build(ExportOptions exportOptions, KeyValuePair<ContextInfoMatrixCell, List<ContextInfo>> cell, DiagramBuilderOptions options)
    {
        var (action, domain) = cell.Key;
        var methods = cell.Value.Distinct().ToList();
        var fileName = ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.puml, $"composite_{action}_{domain}.puml");

        var diagramId = $"composite_{action}_{domain}".AlphanumericOnly();
        var diagramTitle = $"{action.ToUpper()} -> {domain}";

        var diagram = new UmlDiagramClasses(options, diagramId: diagramId);
        diagram.SetTitle(diagramTitle);
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var method in methods)
        {
            var component = new UmlComponent(method.FullName);
            diagram.Add(component);
        }

        diagram.WriteToFile(fileName);
    }

    //context: uml, build, heatmap, directory
    public static void Build(IContextInfoMatrix matrix, ExportOptions exportOptions, DiagramBuilderOptions options)
    {
        foreach (var cell in matrix)
        {
            Build(exportOptions, cell, options);
        }
    }
}