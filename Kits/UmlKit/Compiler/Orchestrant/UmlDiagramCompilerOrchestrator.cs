using System.Collections.Generic;
using ContextBrowser;
using ContextBrowser.Services;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using LoggerKit;
using UmlKit.Compiler;
using UmlKit.Compiler.Orchestrant;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Compiler.Orchestrant;

public class UmlDiagramCompilerOrchestrator : IUmlDiagramCompilerOrchestrator
{
    private readonly IEnumerable<IUmlDiagramCompiler> _compilers;
    private readonly IAppLogger<AppLevel> _appLogger;

    public UmlDiagramCompilerOrchestrator(
        IEnumerable<IUmlDiagramCompiler> compilers,
        IAppLogger<AppLevel> appLogger)
    {
        _compilers = compilers;
        _appLogger = appLogger;
    }

    public void CompileAll(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions builderOptions)
    {
        foreach (var compiler in _compilers)
        {
            compiler.Compile(contextInfoDataset, contextClassifier, exportOptions, builderOptions);
        }
    }
}