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
using ExporterKit.Uml.DiagramCompileOptions;
using ExporterKit.Uml.DiagramCompileOptions.Strategies;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Builders;
using UmlKit.Builders.TransitionFactory;
using UmlKit.Compiler;
using UmlKit.Compiler.CoCompiler;
using UmlKit.DiagramGenerator;
using UmlKit.DiagramGenerator.Renderer;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml;

// context: uml, state, build
public class UmlDiagramCompilerStateAction : IUmlDiagramCompiler
{
    protected readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IContextInfoMapperProvider<DomainPerActionTensor> _mapperProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;
    private readonly IContextDiagramBuildersFactory _diagramBuildersFactory;
    private readonly IDiagramCompileOptionsFactory _compileOptionsFactory;

    public UmlDiagramCompilerStateAction(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IContextInfoMapperProvider<DomainPerActionTensor> mapperProvider, IAppOptionsStore optionsStore, INamingProcessor namingProcessor, IContextInfoManager<ContextInfo> contextInfoManager, IContextDiagramBuildersFactory diagramBuildersFactory, IDiagramCompileOptionsFactory compileOptionsFactory)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _mapperProvider = mapperProvider;
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
        _contextInfoManager = contextInfoManager;
        _diagramBuildersFactory = diagramBuildersFactory;
        _compileOptionsFactory = compileOptionsFactory;
    }

    // context: uml, build
    public async Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile StateAction");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var contextClassifier = _optionsStore.GetOptions<ITensorClassifierDomainPerActionContext<ContextInfo>>();
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var elements = dataset.GetAll().ToList();
        var mapper = await _mapperProvider.GetMapperAsync(GlobalMapperKeys.DomainPerAction, cancellationToken).ConfigureAwait(false);
        var actions = mapper.GetRows().Distinct();

        var tasks = actions.Select(async action =>
        {
            var compileOptions = _compileOptionsFactory.Create(DiagramKind.ActionState, action);
            var wasCompiled = await GenerateSingleAsync(compileOptions, elements, contextClassifier, exportOptions, diagramBuilderOptions, cancellationToken).ConfigureAwait(false);
            return new { Action = action, Result = wasCompiled };
        });

        var results = await Task.WhenAll(tasks);
        return results.ToDictionary(item => item.Action, item => item.Result);
    }

    /// <summary>
    /// Компилирует диаграмму состояний.
    /// </summary>
    protected Task<bool> GenerateSingleAsync(IDiagramCompileOptions options, List<ContextInfo> allContexts, ITensorClassifierDomainPerActionContext<ContextInfo> contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, CancellationToken cancellationToken)
    {
        var factory = new UmlTransitionStateFactory();
        var renderer = new SequenceDiagramRendererPlain<UmlState>(_logger, diagramBuilderOptions, factory, _namingProcessor);
        var generator = new SequenceDiagramGenerator<UmlState>(renderer, diagramBuilderOptions, _logger, factory);
        var diagramBuilder = _diagramBuildersFactory.BuilderForType(diagramBuilderOptions.DiagramType, diagramBuilderOptions, _logger, _contextInfoManager, _optionsStore);

        var compiler = new UmlStateDiagramCompilerAction(diagramBuilderOptions, diagramBuilder, exportOptions, generator, _logger);
        return compiler.CompileAsync(options.MetaItem, options.DiagramId, options.OutputFileName, allContexts, cancellationToken);
    }
}
