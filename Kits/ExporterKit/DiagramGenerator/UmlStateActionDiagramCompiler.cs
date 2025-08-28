using ContextBrowser.Infrastructure.Compiler;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using UmlKit.Builders;
using UmlKit.Builders.TransitionFactory;
using UmlKit.DiagramGenerator;
using UmlKit.DiagramGenerator.Renderer;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;

namespace ExporterKit.HtmlPageSamples;

// контекст: generator, state, action
public class UmlStateActionDiagramCompiler : DiagramGeneratorBase
{
    public UmlStateActionDiagramCompiler(IContextInfoData matrix, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions options, OnWriteLog? onWriteLog)
        : base(matrix, contextClassifier, exportOptions, options, onWriteLog) { }

    public override Dictionary<string, bool> Generate(List<ContextInfo> allContexts)
    {
        var renderedCache = new Dictionary<string, bool>();
        var actions = _matrix.GetActions().Distinct();
        foreach (var action in actions)
        {
            var compileOptions = CompileOptionsFactory.ActionStateOptions(action);
            renderedCache[action] = GenerateSingle(compileOptions, allContexts);
        }
        return renderedCache;
    }


    /// <summary>
    /// Компилирует диаграмму состояний.
    /// </summary>
    protected bool GenerateSingle(IDiagramCompileOptions options, List<ContextInfo> allContexts)
    {
        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Cntx, $"Compiling State {options.FetchType} [{options.MetaItem}]", LogLevelNode.Start);
        var _factory = new UmlTransitionStateFactory();
        var renderer = new SequenceDiagramRendererPlain<UmlState>(_onWriteLog, _options, _factory);
        var _generator = new SequenceDiagramGenerator<UmlState>(renderer, _options, _onWriteLog, _factory);
        var bf2 = ContextDiagramFactory.Custom(_options.DiagramType, _options, _onWriteLog);

        var compiler = new UmlStateDiagramCompiler(_contextClassifier, _options, bf2, _exportOptions, _generator, _onWriteLog);
        var rendered = compiler.CompileAction(options.MetaItem, options.DiagramId, options.OutputFileName, allContexts);

        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
