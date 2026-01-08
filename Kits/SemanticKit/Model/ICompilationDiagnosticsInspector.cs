using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model;

namespace SemanticKit.Model;

public interface ICompilationDiagnosticsInspector<TCompilation, TDiagnostic>
{
    Task<ImmutableArray<TDiagnostic>> LogAndFilterDiagnosticsAsync(TCompilation compilation, CancellationToken cancellationToken);
}
