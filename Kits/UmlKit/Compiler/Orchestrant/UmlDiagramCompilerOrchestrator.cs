using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    private readonly IContextInfoDatasetProvider _datasetProvider;

    public UmlDiagramCompilerOrchestrator(
        IEnumerable<IUmlDiagramCompiler> compilers,
        IContextInfoDatasetProvider datasetProvider,
        IAppLogger<AppLevel> appLogger)
    {
        _compilers = compilers;
        _datasetProvider = datasetProvider;
        _appLogger = appLogger;
    }

    public async Task CompileAllAsync(IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions builderOptions, CancellationToken cancellationToken)
    {
        foreach (var compiler in _compilers)
        {
            await compiler.CompileAsync(contextClassifier, exportOptions, builderOptions, cancellationToken);
        }
    }
}