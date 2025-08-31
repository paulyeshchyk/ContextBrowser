using ContextBrowser.Infrastructure.Compiler;
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

// контекст: abstract, base, generator
public abstract class DiagramCompilerBase
{
    protected readonly IContextClassifier _contextClassifier;
    protected readonly ExportOptions _exportOptions;
    protected readonly DiagramBuilderOptions _options;
    protected readonly IAppLogger<AppLevel> _logger;
    protected readonly IContextInfoData _matrix;

    public abstract Dictionary<string, bool> Compile(List<ContextInfo> allContexts);

    protected DiagramCompilerBase(IContextInfoData matrix, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions options, IAppLogger<AppLevel> logger)
    {
        _matrix = matrix;
        _contextClassifier = contextClassifier;
        _exportOptions = exportOptions;
        _options = options;
        _logger = logger;
    }

}
