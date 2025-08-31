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
using UmlKit.Infrastructure.Options;

namespace ExporterKit.HtmlPageSamples;

// контекст: generator, sequence, domain
public class UmlSequenceDomainDiagramCompiler : DiagramCompilerBase
{
    public UmlSequenceDomainDiagramCompiler(IContextInfoData matrix, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions options, IAppLogger<AppLevel> logger)
        : base(matrix, contextClassifier, exportOptions, options, logger) { }

    public override Dictionary<string, bool> Compile(List<ContextInfo> allContexts)
    {
        var renderedCache = new Dictionary<string, bool>();
        var domains = _matrix.GetDomains().Distinct();
        foreach (var domain in domains)
        {
            var compileOptions = CompileOptionsFactory.DomainSequenceCompileOptions(domain);
            renderedCache[domain] = GenerateSingle(compileOptions, allContexts);
        }
        return renderedCache;
    }

    /// <summary>
    /// Компилирует диаграмму последовательностей.
    /// </summary>
    protected bool GenerateSingle(IDiagramCompileOptions options, List<ContextInfo> allContexts)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, $"Compiling Sequence {options.FetchType} [{options.MetaItem}]", LogLevelNode.Start);
        var bf = ContextDiagramBuildersFactory.TransitionBuilder(_options, _logger.WriteLog);

        var diagramCompilerSequence = new UmlSequenceDiagramCompiler(_contextClassifier, _exportOptions, _logger, _options, bf);
        var rendered = diagramCompilerSequence.Compile(options.MetaItem, options.FetchType, options.DiagramId, options.DiagramTitle, options.OutputFileName, allContexts);

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
