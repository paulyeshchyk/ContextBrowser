using System.Collections.Generic;
using System.Threading;
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
    object GetCompilationUnitRoot(CancellationToken cancellationToken);

    IEnumerable<object> GetAvailableSyntaxies(SemanticOptions options, CancellationToken cancellationToken);
}
