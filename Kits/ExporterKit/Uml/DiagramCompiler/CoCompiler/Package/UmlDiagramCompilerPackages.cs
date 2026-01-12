using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace ExporterKit.Uml.DiagramCompiler.CoCompiler.Package;

// context: uml, build
// pattern: Builder
public class UmlDiagramCompilerPackages : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly IUmlUrlBuilder _umlUrlBuilder;
    private readonly INamingProcessor _namingProcessor;
    private readonly UmlClassRendererPackages _renderer;

    public UmlDiagramCompilerPackages(
        IAppLogger<AppLevel> logger, 
        IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, 
        IAppOptionsStore optionsStore, 
        IUmlUrlBuilder umlUrlBuilder, 
        INamingProcessor namingProcessor, 
        UmlClassRendererPackages renderer)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _umlUrlBuilder = umlUrlBuilder;
        _namingProcessor = namingProcessor;
        _renderer = renderer;
    }

    // context: build, uml
    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile Packages");

        var contextInfoDataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);
        var elements = contextInfoDataset.GetAll().ToList();

        var renderResult = await _renderer.RenderAsync(elements , cancellationToken).ConfigureAwait(false);

        var diagram = renderResult.Diagram;
        if (diagram == null)
        {
            return new Dictionary<ILabeledValue, bool>();
        }

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var outputPath = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, _namingProcessor.NamespaceOnlyPumlFilename());
        var diagramId = $"packages_{outputPath}".AlphanumericOnly();
        diagram.DiagramId = diagramId;
        diagram.SetLayoutDirection(UmlLayoutDirection.Direction.LeftToRight);
        diagram.SetSeparator("none");

        await diagram.WriteToFileAsync(outputPath, renderResult.WriteOptions, cancellationToken).ConfigureAwait(false);

        return new Dictionary<ILabeledValue, bool>();
    }
}
