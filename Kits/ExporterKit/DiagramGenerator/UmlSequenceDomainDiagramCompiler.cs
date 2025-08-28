using ContextBrowser.Infrastructure.Compiler;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using UmlKit.Builders;
using UmlKit.Infrastructure.Options;

namespace ExporterKit.HtmlPageSamples;

// контекст: generator, sequence, domain
public class UmlSequenceDomainDiagramCompiler : DiagramGeneratorBase
{
    public UmlSequenceDomainDiagramCompiler(IContextInfoData matrix, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions options, OnWriteLog? onWriteLog)
        : base(matrix, contextClassifier, exportOptions, options, onWriteLog) { }

    public override Dictionary<string, bool> Generate(List<ContextInfo> allContexts)
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
        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Cntx, $"Compiling Sequence {options.FetchType} [{options.MetaItem}]", LogLevelNode.Start);
        var bf = ContextDiagramFactory.Transition(_options, _onWriteLog);

        var diagramCompilerSequence = new UmlSequenceDiagramCompiler(_contextClassifier, _exportOptions, _onWriteLog, _options, bf);
        var rendered = diagramCompilerSequence.Compile(options.MetaItem, options.FetchType, options.DiagramId, options.DiagramTitle, options.OutputFileName, allContexts);

        _onWriteLog?.Invoke(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
        return rendered;
    }
}
