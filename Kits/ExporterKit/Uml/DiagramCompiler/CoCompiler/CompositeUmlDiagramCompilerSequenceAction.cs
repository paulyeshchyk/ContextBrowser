using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ContextKit.Model.Service;
using ExporterKit;
using ExporterKit.Uml;
using ExporterKit.Uml.DiagramCompileOptions;
using ExporterKit.Uml.DiagramCompileOptions.Strategies;
using ExporterKit.Uml.DiagramCompiler;
using ExporterKit.Uml.DiagramCompiler.CoCompiler;
using ExporterKit.Uml.DiagramCompiler.CoCompiler.Sequence;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Builders;
using UmlKit.Compiler;
using UmlKit.DiagramGenerator.Renderer;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml.DiagramCompiler.CoCompiler;

// context: uml, sequence, build
public class CompositeUmlDiagramCompilerSequenceAction : IUmlDiagramCompiler
{
    protected readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IContextInfoMapperProvider<DomainPerActionTensor> _mapperProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;
    private readonly IContextDiagramBuildersFactory _diagramBuildersFactory;
    private readonly IDiagramCompileOptionsFactory _compileOptionsFactory;
    private readonly UmlTransitionRendererFlat<UmlParticipant> _renderer;

    public CompositeUmlDiagramCompilerSequenceAction(
        IAppLogger<AppLevel> logger,
        IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider,
        IContextInfoMapperProvider<DomainPerActionTensor> mapperProvider,
        IAppOptionsStore optionsStore,
        INamingProcessor namingProcessor,
        IContextInfoManager<ContextInfo> contextInfoManager,
        IContextDiagramBuildersFactory diagramBuildersFactory,
        IDiagramCompileOptionsFactory compileOptionsFactory,
        UmlTransitionRendererFlat<UmlParticipant> renderer)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _mapperProvider = mapperProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
        _contextInfoManager = contextInfoManager;
        _diagramBuildersFactory = diagramBuildersFactory;
        _compileOptionsFactory = compileOptionsFactory;
        _renderer = renderer;

    }

    // context: uml, build
    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile SequenceAction");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var elements = dataset.GetAll().ToList();
        var mapper = await _mapperProvider.GetMapperAsync(GlobalMapperKeys.DomainPerAction, cancellationToken).ConfigureAwait(false);
        var distinctRows = mapper.GetRows().Distinct();

        var renderedCache = new Dictionary<ILabeledValue, bool>();
        foreach (var row in distinctRows)
        {
            var compileOptions = _compileOptionsFactory.Create(DiagramKind.ActionSequence, row);
            renderedCache[row] = await GenerateSingleAsync(exportOptions, diagramBuilderOptions, compileOptions, elements, cancellationToken).ConfigureAwait(false);
        }
        return renderedCache;
    }

    /// <summary>
    /// Компилирует диаграмму последовательностей.
    /// </summary>
    protected async Task<bool> GenerateSingleAsync(ExportOptions exportOptions, DiagramBuilderOptions diagramBuildingOptions, IDiagramCompileOptions compileOptions, List<ContextInfo> allContexts, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, $"Compiling Sequence {compileOptions.FetchType} [{compileOptions.MetaItem}]", LogLevelNode.Start);

        var bf = _diagramBuildersFactory.BuilderForType(DiagramBuilderKeys.Transition, diagramBuildingOptions, _logger, _contextInfoManager, _optionsStore);

        var diagramCompilerSequence = new UmlDiagramCompilerSequence(logger: _logger, exportOptions, diagramBuildingOptions, bf, _namingProcessor, _renderer);
        var rendered = await diagramCompilerSequence.CompileAsync(compileOptions.MetaItem, compileOptions.FetchType, compileOptions.DiagramId, compileOptions.DiagramTitle, compileOptions.OutputFileName, allContexts, cancellationToken).ConfigureAwait(false);

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
