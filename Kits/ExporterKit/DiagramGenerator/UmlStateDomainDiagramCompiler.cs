using ContextBrowser.Infrastructure.Compiler;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using LoggerKit;
using UmlKit.Builders;
using UmlKit.Builders.TransitionFactory;
using UmlKit.DiagramGenerator;
using UmlKit.DiagramGenerator.Renderer;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;

namespace ExporterKit.HtmlPageSamples;

// контекст: generator, state, domain
public class UmlStateDomainDiagramCompiler : DiagramCompilerBase
{
    public UmlStateDomainDiagramCompiler(IContextInfoData matrix, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions options, IAppLogger<AppLevel> logger)
        : base(matrix, contextClassifier, exportOptions, options, logger) { }

    public override Dictionary<string, bool> Compile(List<ContextInfo> allContexts)
    {
        var renderedCache = new Dictionary<string, bool>();
        var domains = _matrix.GetDomains().Distinct();
        foreach (var domain in domains)
        {
            var compileOptions = CompileOptionsFactory.DomainStateCompileOptions(domain);
            renderedCache[domain] = GenerateSingle(compileOptions, allContexts);
        }
        return renderedCache;
    }

    /// <summary>
    /// Компилирует диаграмму состояний.
    /// </summary>
    protected bool GenerateSingle(IDiagramCompileOptions options, List<ContextInfo> allContexts)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, $"Compiling State {options.FetchType} [{options.MetaItem}]", LogLevelNode.Start);
        var _factory = new UmlTransitionStateFactory();
        var renderer = new SequenceDiagramRendererPlain<UmlState>(_logger, _options, _factory);
        var _generator = new SequenceDiagramGenerator<UmlState>(renderer, _options, _logger, _factory);
        var bf2 = ContextDiagramBuildersFactory.BuilderForType(_options.DiagramType, _options, _logger.WriteLog);

        var diagramCompilerState = new UmlStateDiagramCompiler(_contextClassifier, _options, bf2, _exportOptions, _generator, _logger.WriteLog);
        var rendered = diagramCompilerState.CompileDomain(options.MetaItem, options.DiagramId, options.OutputFileName, allContexts);

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
