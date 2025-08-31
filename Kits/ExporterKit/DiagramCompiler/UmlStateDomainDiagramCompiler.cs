using ContextBrowser.Infrastructure.Compiler;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using ExporterKit.DiagramCompiler.DiagramCompilerOptions;
using LoggerKit;
using UmlKit.Builders;
using UmlKit.Builders.TransitionFactory;
using UmlKit.DiagramGenerator;
using UmlKit.DiagramGenerator.Renderer;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;

namespace ExporterKit.DiagramCompiler;

// context: generator, state, domain
public class UmlStateDomainDiagramCompiler : IDiagramCompiler
{
    protected readonly IAppLogger<AppLevel> _logger;

    public UmlStateDomainDiagramCompiler(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: uml, build
    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataSet, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        var renderedCache = new Dictionary<string, bool>();
        var domains = contextInfoDataSet.ContextInfoData.GetDomains().Distinct();
        foreach (var domain in domains)
        {
            var compileOptions = DiagramCompileOptionsFactory.DomainStateCompileOptions(domain);
            renderedCache[domain] = GenerateSingle(compileOptions, contextInfoDataSet.ContextsList, contextClassifier, exportOptions, diagramBuilderOptions);
        }
        return renderedCache;
    }

    /// <summary>
    /// Компилирует диаграмму состояний.
    /// </summary>
    protected bool GenerateSingle(IDiagramCompileOptions options, List<ContextInfo> allContexts, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, $"Compiling State {options.FetchType} [{options.MetaItem}]", LogLevelNode.Start);
        var _factory = new UmlTransitionStateFactory();
        var renderer = new SequenceDiagramRendererPlain<UmlState>(_logger, diagramBuilderOptions, _factory);
        var _generator = new SequenceDiagramGenerator<UmlState>(renderer, diagramBuilderOptions, _logger, _factory);
        var bf2 = ContextDiagramBuildersFactory.BuilderForType(diagramBuilderOptions.DiagramType, diagramBuilderOptions, _logger.WriteLog);

        var diagramCompilerState = new UmlStateDiagramCompiler(contextClassifier, diagramBuilderOptions, bf2, exportOptions, _generator, _logger.WriteLog);
        var rendered = diagramCompilerState.CompileDomain(options.MetaItem, options.DiagramId, options.OutputFileName, allContexts);

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
