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
using UmlKit.Infrastructure.Options;

namespace ExporterKit.DiagramCompiler;

// context: generator, sequence, domain
public class UmlSequenceDomainDiagramCompiler : IDiagramCompiler
{
    protected readonly IAppLogger<AppLevel> _logger;

    public UmlSequenceDomainDiagramCompiler(IAppLogger<AppLevel> logger)
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
            var compileOptions = DiagramCompileOptionsFactory.DomainSequenceCompileOptions(domain);
            renderedCache[domain] = GenerateSingle(contextClassifier, exportOptions, compileOptions, diagramBuilderOptions, contextInfoDataSet.ContextsList);
        }
        return renderedCache;
    }

    /// <summary>
    /// Компилирует диаграмму последовательностей.
    /// </summary>
    protected bool GenerateSingle(IContextClassifier contextClassifier, ExportOptions exportOptions, IDiagramCompileOptions options, DiagramBuilderOptions diagramBuilderOptions, List<ContextInfo> allContexts)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, $"Compiling Sequence {options.FetchType} [{options.MetaItem}]", LogLevelNode.Start);
        var bf = ContextDiagramBuildersFactory.TransitionBuilder(diagramBuilderOptions, _logger.WriteLog);

        var diagramCompilerSequence = new UmlSequenceDiagramCompiler(contextClassifier, exportOptions, _logger, diagramBuilderOptions, bf);
        var rendered = diagramCompilerSequence.Compile(options.MetaItem, options.FetchType, options.DiagramId, options.DiagramTitle, options.OutputFileName, allContexts);

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
