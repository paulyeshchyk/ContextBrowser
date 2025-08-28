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

// контекст: abstract, base, generator
public abstract class DiagramGeneratorBase
{
    protected readonly IContextClassifier _contextClassifier;
    protected readonly ExportOptions _exportOptions;
    protected readonly DiagramBuilderOptions _options;
    protected readonly OnWriteLog? _onWriteLog;
    protected readonly IContextInfoData _matrix;

    public abstract Dictionary<string, bool> Generate(List<ContextInfo> allContexts);

    protected DiagramGeneratorBase(IContextInfoData matrix, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions options, OnWriteLog? onWriteLog)
    {
        _matrix = matrix;
        _contextClassifier = contextClassifier;
        _exportOptions = exportOptions;
        _options = options;
        _onWriteLog = onWriteLog;
    }

}
