using System.Collections.Generic;
using System.Linq;
using ContextBrowser.Infrastructure.Compiler;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using ExporterKit;
using ExporterKit.Uml;
using ExporterKit.Uml.DiagramCompileOptions;
using LoggerKit;
using UmlKit.Builders;
using UmlKit.Builders.TransitionFactory;
using UmlKit.DiagramGenerator;
using UmlKit.DiagramGenerator.Renderer;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;

namespace ExporterKit.Uml;

// context: generator, state, action
public class UmlStateActionDiagramCompiler : IUmlDiagramCompiler
{
    protected readonly IAppLogger<AppLevel> _logger;

    public UmlStateActionDiagramCompiler(IAppLogger<AppLevel> logger)
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
            var compileOptions = DiagramCompileOptionsFactory.ActionStateOptions(action);
            renderedCache[action] = GenerateSingle(compileOptions, contextInfoDataSet.ContextsList, contextClassifier, exportOptions, diagramBuilderOptions);
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
        var diagramBuilder = ContextDiagramBuildersFactory.BuilderForType(diagramBuilderOptions.DiagramType, diagramBuilderOptions, _logger.WriteLog);

        var compiler = new UmlStateDiagramCompilerAction(contextClassifier, diagramBuilderOptions, diagramBuilder, exportOptions, _generator, _logger.WriteLog);
        var rendered = compiler.Compile(options.MetaItem, options.DiagramId, options.OutputFileName, allContexts);

        _logger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
