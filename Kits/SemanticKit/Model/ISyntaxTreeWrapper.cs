using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISyntaxTreeWrapper
{
    string FilePath { get; }

    object Tree { get; }

    /// <summary>
    /// for roslyn: CompilationUnitSyntax
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<object> GetCompilationUnitRootAsync(CancellationToken cancellationToken);

    IEnumerable<object> GetAvailableSyntaxies(SemanticOptions options, CancellationToken cancellationToken);
}
