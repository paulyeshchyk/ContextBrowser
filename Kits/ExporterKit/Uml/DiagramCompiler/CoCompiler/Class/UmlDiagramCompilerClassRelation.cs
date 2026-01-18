using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Service;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace ExporterKit.Uml.DiagramCompiler.CoCompiler.Class;

// context: uml, links, build
// pattern: Builder
public class UmlDiagramCompilerClassRelation : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;
    private readonly UmlClassRendererLinks _renderer;

    public UmlDiagramCompilerClassRelation(
        IAppLogger<AppLevel> logger,
        IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider,
        IAppOptionsStore optionsStore,
        IContextInfoManager<ContextInfo> contextInfoManager,
        UmlClassRendererLinks renderer)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _contextInfoManager = contextInfoManager;
        _renderer = renderer;
    }

    //context: build, uml, links
    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile ClassRelation");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var contextInfoList = dataset.GetAll().ToList();
        var linkGenerator = new ContextInfoDataLinkGenerator(_optionsStore, _contextInfoManager);
        var links = linkGenerator.Generate(contextInfoList);

        await ExportAsync(exportOptions, diagramBuilderOptions, links, cancellationToken).ConfigureAwait(false);
        return new Dictionary<ILabeledValue, bool>();
    }

    public async Task ExportAsync(ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, HashSet<(string From, string To)> links, CancellationToken cancellationToken)
    {
        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pumlExtra, "uml.4.links.puml");
        var diagramId = $"relation_{outputPath}".AlphanumericOnly();

        var renderResult = await _renderer.RenderAsync(links, cancellationToken).ConfigureAwait(false);
        var diagram = renderResult.Diagram;
        if (diagram == null)
        {
            return;
        }

        diagram.DiagramId = $"relation_{outputPath}".AlphanumericOnly();
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSkinParam("componentStyle", "rectangle");

        await diagram.WriteToFileAsync(outputPath, renderResult.WriteOptions, cancellationToken).ConfigureAwait(false);
    }
}
