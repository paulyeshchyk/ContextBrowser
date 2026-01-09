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
using ExporterKit.Uml.DiagramCompileOptions;
using ExporterKit.Uml.DiagramCompileOptions.Strategies;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Builders;
using UmlKit.Compiler;
using UmlKit.Compiler.CoCompiler;
using UmlKit.Infrastructure.Options;

namespace ExporterKit.Uml.DiagramCompiler;

// context: uml, sequence, build
public class UmlDiagramCompilerSequenceDomain : IUmlDiagramCompiler
{
    protected readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IContextInfoMapperProvider<DomainPerActionTensor> _mapperProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;
    private readonly IContextDiagramBuildersFactory _diagramBuildersFactory;
    private readonly IDiagramCompileOptionsFactory _compileOptionsFactory;

    public UmlDiagramCompilerSequenceDomain(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IContextInfoMapperProvider<DomainPerActionTensor> mapperProvider, IAppOptionsStore optionsStore, INamingProcessor namingProcessor, IContextInfoManager<ContextInfo> contextInfoManager, IContextDiagramBuildersFactory diagramBuildersFactory, IDiagramCompileOptionsFactory compileOptionsFactory)
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
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile SequenceDomain");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var elements = dataset.GetAll().ToList();
        var mapper = await _mapperProvider.GetMapperAsync(GlobalMapperKeys.DomainPerAction, cancellationToken).ConfigureAwait(false);
        var distinctCols = mapper.GetCols().Distinct();

        var renderedCache = new Dictionary<ILabeledValue, bool>();
        foreach (var col in distinctCols)
        {
            var compileOptions = _compileOptionsFactory.Create(DiagramKind.DomainSequence, col);
            renderedCache[col] = GenerateSingle(exportOptions, compileOptions, diagramBuilderOptions, elements);
        }
        return renderedCache;
    }

    /// <summary>
    /// Компилирует диаграмму последовательностей.
    /// </summary>
    protected bool GenerateSingle(ExportOptions exportOptions, IDiagramCompileOptions options, DiagramBuilderOptions diagramBuilderOptions, List<ContextInfo> allContexts)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, $"Compiling Sequence {options.FetchType} [{options.MetaItem}]", LogLevelNode.Start);
        var transitionBuilder = _diagramBuildersFactory.BuilderForType(DiagramBuilderKeys.Transition, diagramBuilderOptions, _logger, _contextInfoManager, _optionsStore);

        var diagramCompilerSequence = new UmlDiagramCompilerSequence(logger: _logger, exportOptions, diagramBuilderOptions, transitionBuilder, _namingProcessor);
        var rendered = diagramCompilerSequence.Compile(options.MetaItem, options.FetchType, options.DiagramId, options.DiagramTitle, options.OutputFileName, allContexts, CancellationToken.None);

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
