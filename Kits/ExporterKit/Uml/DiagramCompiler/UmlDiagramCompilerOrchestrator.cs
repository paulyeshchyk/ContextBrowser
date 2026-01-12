using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;

namespace UmlKit.Compiler.Orchestrant;

public class UmlDiagramCompilerOrchestrator : IUmlDiagramCompilerOrchestrator
{
    private readonly IEnumerable<IUmlDiagramCompiler> _compilers;
    private readonly IAppLogger<AppLevel> _appLogger;

    public UmlDiagramCompilerOrchestrator(IEnumerable<IUmlDiagramCompiler> compilers, IAppLogger<AppLevel> appLogger)
    {
        _compilers = compilers;
        _appLogger = appLogger;
    }

    public async Task CompileAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _appLogger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, "Compile diagrams", LogLevelNode.Start);
        
        var tasks = _compilers.Select(async compiler => await compiler.CompileAsync(cancellationToken).ConfigureAwait(false));
        
        await Task.WhenAll(tasks);
        
        _appLogger.WriteLog(AppLevel.P_Cpl, LogLevel.Cntx, string.Empty, LogLevelNode.End);
    }
}