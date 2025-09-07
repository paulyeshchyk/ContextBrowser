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

// context: uml, sequence, build
public class UmlDiagramCompilerSequenceAction : IUmlDiagramCompiler
{
    protected readonly IAppLogger<AppLevel> _logger;

    public UmlDiagramCompilerSequenceAction(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: uml, build
    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        var elements = contextInfoDataset.GetAll().ToList();
        var actions = contextInfoDataset.GetActions().Distinct();

        var renderedCache = new Dictionary<string, bool>();
        foreach (var action in actions)
        {
            var compileOptions = DiagramCompileOptionsFactory.ActionSequenceCompileOptions(action);
            renderedCache[action] = GenerateSingle(contextClassifier, exportOptions, diagramBuilderOptions, compileOptions, elements);
        }
        return renderedCache;
    }

    /// <summary>
    /// Компилирует диаграмму последовательностей.
    /// </summary>
    protected bool GenerateSingle(IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuildingOptions, IDiagramCompileOptions compileOptions, List<ContextInfo> allContexts)
    {
        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, $"Compiling Sequence {compileOptions.FetchType} [{compileOptions.MetaItem}]", LogLevelNode.Start);
        var bf = ContextDiagramBuildersFactory.TransitionBuilder(diagramBuildingOptions, _logger.WriteLog);

        var diagramCompilerSequence = new UmlDiagramCompilerSequence(contextClassifier, exportOptions, _logger, diagramBuildingOptions, bf);
        var rendered = diagramCompilerSequence.Compile(compileOptions.MetaItem, compileOptions.FetchType, compileOptions.DiagramId, compileOptions.DiagramTitle, compileOptions.OutputFileName, allContexts);

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
