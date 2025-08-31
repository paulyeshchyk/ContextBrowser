using System.Collections.Generic;
using ContextBrowser.DiagramFactory.Exporters;
using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.DiagramCompiler;
using LoggerKit;
using UmlKit.Exporter;
using UmlKit.Infrastructure.Options;

namespace ExporterKit.DiagramCompiler;

// context:uml, build
public class UmlExtraDiagramsCompiler : IDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public UmlExtraDiagramsCompiler(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context:uml, build

    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataSet, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "--- ExtraDiagramsBuilder.Build ---");

        //ContextMatrixUmlExporter.GenerateCsv(matrix, $"{theOutputPath}matrix.csv");
        //HeatmapExporter.GenerateHeatmapCsv(matrix, $"{theOutputPath}heatmap.csv", unclassifiedPriority);

        UmlContextPackagesDiagram.Build(contextInfoDataSet.ContextsList, ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.pumlExtra, "uml.packages.domains.puml"), diagramBuilderOptions);
        UmlContextMethodsOnlyDiagram.Build(contextInfoDataSet.ContextsList, ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.pumlExtra, "methodlinks.puml"), diagramBuilderOptions);
        UmlContextMethodPerActionDomainDiagram.Build(contextInfoDataSet.ContextInfoData, ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.pumlExtra, "uml.packages.actions.puml"), diagramBuilderOptions);

        var linkGenerator = new ContextInfoDataLinkGenerator(contextClassifier, contextInfoDataSet.ContextsList);
        var links = linkGenerator.Generate();

        UmlContextRelationDiagram.GenerateLinksUml(links, ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.pumlExtra, "uml.4.links.puml"), diagramBuilderOptions);
        return new Dictionary<string, bool>() { };
    }

}