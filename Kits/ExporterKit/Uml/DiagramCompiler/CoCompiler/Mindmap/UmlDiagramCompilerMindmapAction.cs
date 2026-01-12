using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using GraphKit.Walkers;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace ExporterKit.Uml.DiagramCompiler.CoCompiler.Mindmap;

public class UmlDiagramCompilerMindmapAction : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;
    private readonly UmlMindmapRendererAction _renderer;

    public UmlDiagramCompilerMindmapAction(
        IAppLogger<AppLevel> logger, 
        IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, 
        IAppOptionsStore optionsStore, 
        INamingProcessor namingProcessor, 
        IUmlUrlBuilder umlUrlBuilder, 
        UmlMindmapRendererAction renderer)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;
        _renderer = renderer;
    }

    // context: build, uml
    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "Compile Mindmap Action");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var elements = dataset.GetAll();
        var distinctAction = elements.Select(e => e.Action).Distinct().Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!).Select(s => s.Split(';')).SelectMany(s => s).Distinct();

        var tasks = distinctAction.Select(async action => await ExportAsync(action, cancellationToken).ConfigureAwait(false));
        await Task.WhenAll(tasks);

        return new Dictionary<ILabeledValue, bool>();
    }


    public async Task ExportAsync(string action, CancellationToken cancellationToken)
    {
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();

        var fileName = _namingProcessor.MindmapActionPumlFilename(action);
        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, fileName);
        var diagramId = _namingProcessor.MindmapActionDiagramId(outputPath);


        var renderResult = await _renderer.RenderAsync(action, cancellationToken).ConfigureAwait(false);
        var diagram = renderResult.Diagram;
        if (diagram == null)
        {
            return;
        }

        diagram.DiagramId = diagramId;
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSeparator("none");
        diagram.AddStyle(UmlStyle.Builder("grey").BackgroundColor("#f0f0f0").LineColor("#e0e0e0").HyperlinkColor("#101010").HyperlinkUnderlineThickness(0).Build());
        diagram.AddStyle(UmlStyle.Builder("green").BackgroundColor("#90de90").LineColor("#60d060").HyperlinkColor("#1d601d").HyperlinkUnderlineThickness(0).Build());
        diagram.AddStyle(UmlStyle.Builder("selected").BackgroundColor("#ff9500").LineColor("#d47c00").HyperlinkColor("#1f1510").HyperlinkUnderlineThickness(0).Build());

        await diagram.WriteToFileAsync(outputPath, renderResult.WriteOptions, cancellationToken).ConfigureAwait(false);
    }
}
