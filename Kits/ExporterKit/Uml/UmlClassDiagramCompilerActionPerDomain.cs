using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using ExporterKit.Uml;
using ExporterKit.Uml.ClassDiagram;
using LoggerKit;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, build
// pattern: Builder
public class UmlClassDiagramCompilerActionPerDomain : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public UmlClassDiagramCompilerActionPerDomain(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataSet, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        foreach (var cell in contextInfoDataSet.ContextInfoData)
        {
            Build(cell, exportOptions, diagramBuilderOptions);
        }
        return new Dictionary<string, bool> { };
    }

    //context: uml, build, heatmap, directory
    internal void Build(KeyValuePair<ContextInfoDataCell, List<ContextInfo>> cell, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        var (action, domain) = cell.Key;
        var methods = cell.Value.Distinct().ToList();
        var fileName = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, $"class_{action}_{domain}.puml");

        var diagramId = $"class_{action}_{domain}".AlphanumericOnly();
        var diagramTitle = $"{action.ToUpper()} -> {domain}";

        var diagram = new UmlClassDiagram(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetTitle(diagramTitle);
        diagram.SetSkinParam("componentStyle", "rectangle");
        diagram.SetSeparator("none");

        // Группируем по Namespace, затем по ClassOwnerFullName
        var allElements = UmlClassDiagramDataMapper.Map(methods);

        UmlClassDiagramBuilder.Build(diagram, allElements);
        UmlClassDiagramBuilder.BuildSquaredLayout(diagram, allElements);

        diagram.WriteToFile(fileName);
    }
}
