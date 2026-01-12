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
public class UmlDiagramCompilerClassMethodsList : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;
    private readonly UmlClassRendererMethods _renderer;

    public UmlDiagramCompilerClassMethodsList(
        IAppLogger<AppLevel> logger, 
        IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, 
        IAppOptionsStore optionsStore, 
        IContextInfoManager<ContextInfo> contextInfoManager, 
        UmlClassRendererMethods renderer)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _contextInfoManager = contextInfoManager;
        _renderer = renderer;
    }

    // context: build, uml, links
    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile ClassMethodList");

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();

        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pumlExtra, "methodlinks.puml");
        var diagramId = $"methods_only_{outputPath}".AlphanumericOnly();

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        var methods = dataset.GetAll()
                .Where(e => e.ElementType == ContextInfoElementType.method)
                .ToList();

        var renderResult = await _renderer.RenderAsync(methods, cancellationToken).ConfigureAwait(false);

        var diagram = renderResult.Diagram;
        if (diagram == null)
        {
            return new Dictionary<ILabeledValue, bool>();
        }
        diagram.DiagramId = diagramId;
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);

        await diagram.WriteToFileAsync(outputPath, renderResult.WriteOptions, cancellationToken).ConfigureAwait(false);

        return new Dictionary<ILabeledValue, bool>();
    }
}
