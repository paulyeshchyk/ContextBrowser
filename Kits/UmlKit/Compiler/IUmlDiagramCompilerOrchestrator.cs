using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using UmlKit.Infrastructure.Options;

namespace ContextBrowser.Services;

public interface IUmlDiagramCompilerOrchestrator
{
    Task CompileAllAsync(CancellationToken cancellationToken);
}
