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

public class UmlDiagramCompilerMindmapClassOnly : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;
    private readonly UmlMindmapRendererClassOnly _renderer;

    public UmlDiagramCompilerMindmapClassOnly(
        IAppLogger<AppLevel> logger, 
        IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, 
        IAppOptionsStore optionsStore, 
        INamingProcessor namingProcessor, 
        IUmlUrlBuilder umlUrlBuilder, 
        UmlMindmapRendererClassOnly renderer)
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
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "Compile Mindmap Action: Class only");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var elements = dataset.GetAll();
        var distinctClasses = elements.Where(e => e.ElementType.IsEntityDefinition() || e.MethodOwnedByItSelf == true).Distinct();

        foreach (var classItem in distinctClasses)
        {
            await ExportAsync(exportOptions, diagramBuilderOptions, classItem, _namingProcessor, cancellationToken);
        }

        return new Dictionary<ILabeledValue, bool>();
    }

    public async Task ExportAsync(ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, ContextInfo classContextInfo, INamingProcessor namingProcessor, CancellationToken cancellationToken)
    {
        // грязный хак для получения информации о владельце
        var classownerInfo = classContextInfo.MethodOwnedByItSelf
            ? (classContextInfo.ClassOwner ?? classContextInfo)
            : classContextInfo;

        var classNameWithNameSpace = $"{classownerInfo.Namespace}.{classownerInfo.ShortName}";

        var fileName = namingProcessor.MindmapClassOnlyPumlFilename(classNameWithNameSpace);
        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, fileName);

        var renderResult = await _renderer.RenderAsync(classContextInfo, classNameWithNameSpace, cancellationToken).ConfigureAwait(false);

        var diagram = renderResult.Diagram;
        if (diagram == null)
        {
            return;
        }

        var diagramId = namingProcessor.MindmapActionDiagramId(outputPath);
        diagram.DiagramId = diagramId;
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSeparator("none");
        diagram.AddStyle(UmlStyle.Builder("grey").BackgroundColor("#f0f0f0").LineColor("#e0e0e0").HyperlinkColor("#101010").HyperlinkUnderlineThickness(0).Build());
        diagram.AddStyle(UmlStyle.Builder("green").BackgroundColor("#90de90").LineColor("#60d060").HyperlinkColor("#1d601d").HyperlinkUnderlineThickness(0).Build());
        diagram.AddStyle(UmlStyle.Builder("selected").BackgroundColor("#ff9500").LineColor("#d47c00").HyperlinkColor("#1f1510").HyperlinkUnderlineThickness(0).Build());

        await diagram.WriteToFileAsync(outputPath, renderResult.WriteOptions, cancellationToken).ConfigureAwait(false);
    }
}