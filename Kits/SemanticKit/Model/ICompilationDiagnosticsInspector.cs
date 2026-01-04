using System.Threading;
using SemanticKit.Model;

namespace SemanticKit.Model;

public interface ICompilationDiagnosticsInspector<TCompilation>
{
    void LogAndFilterDiagnostics(TCompilation compilation, CancellationToken cancellationToken);
}
