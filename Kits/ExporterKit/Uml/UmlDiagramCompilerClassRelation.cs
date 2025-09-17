using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.DiagramFactory.Exporters;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Uml;
using LoggerKit;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Exporter;

// context: uml, links, build
// pattern: Builder
public class UmlDiagramCompilerClassRelation : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;

    public UmlDiagramCompilerClassRelation(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider datasetProvider, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
    }

    //context: build, uml, links
    public async Task<Dictionary<string, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile ClassRelation");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var contextClassifier = _optionsStore.GetOptions<IDomainPerActionContextClassifier>();
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var contextInfoList = dataset.GetAll().ToList();
        var linkGenerator = new ContextInfoDataLinkGenerator(contextClassifier, contextInfoList);
        var links = linkGenerator.Generate();

        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pumlExtra, "uml.4.links.puml");
        var diagramId = $"relation_{outputPath}".AlphanumericOnly();
        var diagram = new UmlDiagramClass(diagramBuilderOptions, diagramId: diagramId);
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSkinParam("componentStyle", "rectangle");

        foreach (var (from, to) in links)
        {
            AddRelation(diagramBuilderOptions, diagram, from, to);
        }

        var writeOptons = new UmlWriteOptions(alignMaxWidth: -1) { };
        diagram.WriteToFile(outputPath, writeOptons);
        return new Dictionary<string, bool>();
    }

    private static void AddRelation(DiagramBuilderOptions options, UmlDiagramClass diagram, string from, string to)
    {
        var arrow = new UmlArrow(flowType: options.Indication.UseAsync ? UmlArrowFlowType.Async : UmlArrowFlowType.Sync);
        var relation = new UmlRelation(from, to, arrow);
        diagram.Add(relation);
    }
}
