using System.Collections.Generic;
using ContextBrowser.Infrastructure;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ExporterKit.DiagramCompiler;
using ExporterKit.HtmlPageSamples;
using LoggerKit;

namespace ContextBrowser.Services;

public interface IDiagramCompilerOrchestrator
{
    void CompileAll(IContextInfoDataset dataset, AppOptions options);
}

public class DiagramCompilerOrchestrator : IDiagramCompilerOrchestrator
{
    private readonly IEnumerable<IDiagramCompiler> _compilers;
    private readonly IAppLogger<AppLevel> _appLogger;

    public DiagramCompilerOrchestrator(
        IEnumerable<IDiagramCompiler> compilers,
        IAppLogger<AppLevel> appLogger)
    {
        _compilers = compilers;
        _appLogger = appLogger;
    }

    public void CompileAll(IContextInfoDataset dataset, AppOptions options)
    {
        foreach (var compiler in _compilers)
        {
            compiler.Compile(dataset, options.Classifier, options.Export, options.DiagramBuilder);
        }
    }
}