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

// context: generator, sequence, action
public class UmlSequenceActionDiagramCompiler : IDiagramCompiler
{
    protected readonly IAppLogger<AppLevel> _logger;

    public UmlSequenceActionDiagramCompiler(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: uml, build
    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataSet, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        var renderedCache = new Dictionary<string, bool>();
        var actions = contextInfoDataSet.ContextInfoData.GetActions().Distinct();
        foreach (var action in actions)
        {
            var compileOptions = DiagramCompileOptionsFactory.ActionSequenceCompileOptions(action);
            renderedCache[action] = GenerateSingle(contextClassifier, exportOptions, diagramBuilderOptions, compileOptions, contextInfoDataSet.ContextsList);
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

        var diagramCompilerSequence = new UmlSequenceDiagramCompiler(contextClassifier, exportOptions, _logger, diagramBuildingOptions, bf);
        var rendered = diagramCompilerSequence.Compile(compileOptions.MetaItem, compileOptions.FetchType, compileOptions.DiagramId, compileOptions.DiagramTitle, compileOptions.OutputFileName, allContexts);

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
