using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using ContextKit.Model.Service;
using ExporterKit;
using ExporterKit.Uml;
using ExporterKit.Uml.DiagramCompileOptions;
using ExporterKit.Uml.DiagramCompileOptions.Strategies;
using ExporterKit.Uml.DiagramCompiler;
using ExporterKit.Uml.DiagramCompiler.CoCompiler;
using ExporterKit.Uml.DiagramCompiler.CoCompiler.State;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Builders;
using UmlKit.Builders.TransitionFactory;
using UmlKit.Compiler;
using UmlKit.DiagramGenerator;
using UmlKit.DiagramGenerator.Renderer;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml.DiagramCompiler.CoCompiler;

// context: uml, state, build
public class CompositeUmlDiagramCompilerStateDomain : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IContextInfoMapperProvider<DomainPerActionTensor> _mapperProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;
    private readonly IContextDiagramBuildersFactory _diagramBuildersFactory;
    private readonly IDiagramCompileOptionsFactory _compileOptionsFactory;
    private readonly UmlTransitionRendererFlat<UmlState> _renderer;

    public CompositeUmlDiagramCompilerStateDomain(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IContextInfoMapperProvider<DomainPerActionTensor> mapperProvider, IAppOptionsStore optionsStore, INamingProcessor namingProcessor, IContextInfoManager<ContextInfo> contextInfoManager, IContextDiagramBuildersFactory diagramBuildersFactory, IDiagramCompileOptionsFactory compileOptionsFactory, UmlTransitionRendererFlat<UmlState> renderer)
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
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile StateDomain");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        var contextClassifier = _optionsStore.GetOptions<ITensorClassifierDomainPerActionContext<ContextInfo>>();
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var elements = dataset.GetAll().ToList();
        var mapper = await _mapperProvider.GetMapperAsync(GlobalMapperKeys.DomainPerAction, cancellationToken).ConfigureAwait(false);
        var domains = mapper.GetCols().Distinct();

        var renderedCache = new Dictionary<ILabeledValue, bool>();
        foreach (var domain in domains)
        {
            var compileOptions = _compileOptionsFactory.Create(DiagramKind.DomainState, domain);
            renderedCache[domain] = await GenerateSingle(compileOptions, elements, contextClassifier, exportOptions, diagramBuilderOptions, cancellationToken);
        }
        return renderedCache;
    }

    /// <summary>
    /// Компилирует диаграмму состояний.
    /// </summary>
    protected Task<bool> GenerateSingle(IDiagramCompileOptions options, List<ContextInfo> allContexts, ITensorClassifierDomainPerActionContext<ContextInfo> contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, $"Compiling State {options.FetchType} [{options.MetaItem}]", LogLevelNode.Start);

        var diagramBuilder = _diagramBuildersFactory.BuilderForType(diagramBuilderOptions.DiagramType, diagramBuilderOptions, _logger, _contextInfoManager, _optionsStore);

        var diagramCompilerState = new UmlDiagramCompilerStateDomain(diagramBuilderOptions, diagramBuilder, exportOptions, _renderer, _logger);
        var rendered = diagramCompilerState.CompileAsync(options.MetaItem, options.DiagramId, options.OutputFileName, allContexts, cancellationToken);

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
