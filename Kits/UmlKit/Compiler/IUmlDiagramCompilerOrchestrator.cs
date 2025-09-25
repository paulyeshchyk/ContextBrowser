using System.Threading;
using System.Threading.Tasks;

namespace UmlKit.Compiler;

public interface IUmlDiagramCompilerOrchestrator
{
    Task CompileAllAsync(CancellationToken cancellationToken);
}
