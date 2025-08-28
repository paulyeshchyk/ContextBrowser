using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.UmlDiagrams.ClassDiagram;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public static class UmlContextComponentDiagram
{
    //context: uml, build, heatmap, directory
    public static void Build(ExportOptions exportOptions, KeyValuePair<ContextInfoDataCell, List<ContextInfo>> cell, DiagramBuilderOptions options)
    {
        var (action, domain) = cell.Key;
        var methods = cell.Value.Distinct().ToList();
        var fileName = ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.puml, $"class_{action}_{domain}.puml");

        var diagramId = $"class_{action}_{domain}".AlphanumericOnly();
        var diagramTitle = $"{action.ToUpper()} -> {domain}";

        var diagram = new UmlClassDiagram(options, diagramId: diagramId);
        diagram.SetTitle(diagramTitle);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");


        // Группируем по Namespace, затем по ClassOwnerFullName
        var allElements = UmlClassDiagramDataMapper.Map(methods);

        UmlClassDiagramBuilder.Build(diagram, allElements);
        UmlClassDiagramBuilder.BuildSquaredLayout(diagram, allElements);

        diagram.WriteToFile(fileName);
    }

    //context: uml, build, heatmap, directory
    public static void Build(IContextInfoData matrix, ExportOptions exportOptions, DiagramBuilderOptions options)
    {
        foreach (var cell in matrix)
        {
            Build(exportOptions, cell, options);
        }
    }
}
