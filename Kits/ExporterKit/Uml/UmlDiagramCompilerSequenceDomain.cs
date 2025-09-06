using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit;
using ExporterKit.Uml;
using ExporterKit.Uml.DiagramCompileOptions;
using LoggerKit;
using UmlKit.Builders;
using UmlKit.Compiler;
using UmlKit.Compiler.CoCompiler;
using UmlKit.Infrastructure.Options;

namespace ExporterKit.Uml;

// context: generator, sequence, domain
public class UmlDiagramCompilerSequenceDomain : IUmlDiagramCompiler
{
    protected readonly IAppLogger<AppLevel> _logger;

    public UmlDiagramCompilerSequenceDomain(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: uml, build
    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        var elements = contextInfoDataset.GetAll().ToList();
        var domains = contextInfoDataset.GetDomains().Distinct();

        var renderedCache = new Dictionary<string, bool>();
        foreach (var domain in domains)
        {
            var compileOptions = DiagramCompileOptionsFactory.DomainSequenceCompileOptions(domain);
            renderedCache[domain] = GenerateSingle(contextClassifier, exportOptions, compileOptions, diagramBuilderOptions, elements);
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

        var diagramCompilerSequence = new UmlDiagramCompilerSequence(contextClassifier, exportOptions, _logger, diagramBuilderOptions, bf);
        var rendered = diagramCompilerSequence.Compile(options.MetaItem, options.FetchType, options.DiagramId, options.DiagramTitle, options.OutputFileName, allContexts);

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
