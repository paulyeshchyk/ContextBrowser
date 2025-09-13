using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit;
using ExporterKit.Html;
using ExporterKit.Infrastucture;
using ExporterKit.Uml;
using ExporterKit.Uml.DiagramCompileOptions;
using LoggerKit;
using UmlKit.Builders;
using UmlKit.Builders.TransitionFactory;
using UmlKit.Compiler;
using UmlKit.Compiler.CoCompiler;
using UmlKit.DiagramGenerator;
using UmlKit.DiagramGenerator.Renderer;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;

namespace ExporterKit.Uml;

// context: uml, state, build
public class UmlDiagramCompilerStateDomain : IUmlDiagramCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider _datasetProvider;
    private readonly IContextInfoMapperProvider _mapperProvider;

    public UmlDiagramCompilerStateDomain(IAppLogger<AppLevel> logger, IContextInfoDatasetProvider datasetProvider, IContextInfoMapperProvider mapperProvider)
    {
        _logger = logger;
        _datasetProvider = datasetProvider;
        _mapperProvider = mapperProvider;
    }

    // context: uml, build
    public async Task<Dictionary<string, bool>> CompileAsync(IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, CancellationToken cancellationToken)
    {
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var elements = dataset.GetAll().ToList();
        var mapper = await _mapperProvider.GetMapperAsync(GlobalMapperKeys.DomainPerAction, cancellationToken);
        var domains = mapper.GetDomains().Distinct();

        var renderedCache = new Dictionary<string, bool>();
        foreach (var domain in domains)
        {
            var compileOptions = DiagramCompileOptionsFactory.DomainStateCompileOptions(domain);
            renderedCache[domain] = GenerateSingle(compileOptions, elements, contextClassifier, exportOptions, diagramBuilderOptions, cancellationToken);
        }
        return renderedCache;
    }

    /// <summary>
    /// Компилирует диаграмму состояний.
    /// </summary>
    protected bool GenerateSingle(IDiagramCompileOptions options, List<ContextInfo> allContexts, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, $"Compiling State {options.FetchType} [{options.MetaItem}]", LogLevelNode.Start);
        var _factory = new UmlTransitionStateFactory();
        var renderer = new SequenceDiagramRendererPlain<UmlState>(_logger, diagramBuilderOptions, _factory);
        var _generator = new SequenceDiagramGenerator<UmlState>(renderer, diagramBuilderOptions, _logger, _factory);
        var bf2 = ContextDiagramBuildersFactory.BuilderForType(diagramBuilderOptions.DiagramType, diagramBuilderOptions, _logger.WriteLog);

        var diagramCompilerState = new UmlStateDiagramCompilerDomain(contextClassifier, diagramBuilderOptions, bf2, exportOptions, _generator, _logger.WriteLog);
        var rendered = diagramCompilerState.CompileAsync(options.MetaItem, options.DiagramId, options.OutputFileName, allContexts, cancellationToken);

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
